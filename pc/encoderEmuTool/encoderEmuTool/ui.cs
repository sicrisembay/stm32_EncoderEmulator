using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dongle;

namespace encoderEmuTool {
    namespace Events {
        public delegate void FrameReceiveEventHandler(object sender, Events.FrameReceiveEventArgs e);
        public class FrameReceiveEventArgs : EventArgs {
            public readonly byte[] buf;
            public FrameReceiveEventArgs(byte[] buf, int len)
            {
                this.buf = new byte[len];
                for (int i = 0; i < len; i++)
                {
                    this.buf[i] = buf[i];
                }
            }
        }
    }

    public partial class ui : Form {
        private parser myParser;

        public ui()
        {
            InitializeComponent();
            this.myParser = new parser(this);
            this.GetDongleSerialNumber();
        }

        private void GetDongleSerialNumber()
        {
            cboDongleSN.Items.Clear();
            cboDongleSN.Text = null;
            this.myParser.myDongle.ListDevice();
            if (this.myParser.myDongle.DevSerialList.Count != 0)
            {
                for (int i = 0; i < this.myParser.myDongle.DevSerialList.Count; i++)
                {
                    cboDongleSN.Items.Add(this.myParser.myDongle.DevSerialList[i]);
                }
                cboDongleSN.Text = this.myParser.myDongle.DevSerialList[0];
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.GetDongleSerialNumber();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                Int32 newRate = Convert.ToInt32(tbxRPM.Text);
                newRate = (newRate * Convert.ToInt32(tbxCPR.Text)) / 60;
                this.myParser.UpdateRate(newRate);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (btnConnect.Text == "Connect")
            {
                try
                {
                    this.myParser.myDongle.Connect(cboDongleSN.Text);
                    btnConnect.Text = "Disconnect";
                    gbUsbComm.Enabled = false;
                    gbSpeed.Enabled = true;
//                    this.deviceConnected = true;
//                    this.OpenDataStream();
//                    Thread.Sleep(2000); /* Add delay to give enough time for the dongle to be in CONNECTED state */
//                    this.parser.GetDeviceType();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                btnConnect.Text = "Connect";
                gbUsbComm.Enabled = true;
                gbSpeed.Enabled = false;
                this.myParser.myDongle.Disconnect();
//                this.deviceConnected = false;
//                this.CloseDataStream();
            }
        }
    }

    public enum commandId {
        ENCODER_SET_RATE = 0x22,
        ENCODER_GET_RATE = 0x23,
        ENCODER_SET_COUNT = 0x24,
        ENCODER_GET_COUNT = 0x25
    }

    class parser {
        const int vid = 0x0483;
        const int pid = 0xC1B0;
        const int bufSize = 2048;

        const byte TAG_CMD = 0xFF;
        const byte TAG_STATUS = 0xFE;

        private Form uiForm;
        public dongleInterface myDongle;
        private int rdRcvIdx;
        private int wrRcvIdx;
        private byte[] receiveBuf;

        public event Events.FrameReceiveEventHandler EventFrameReceive;

        public parser(Form uiForm)
        {
            this.uiForm = uiForm;
            this.receiveBuf = new byte[bufSize];
            this.rdRcvIdx = 0;
            this.wrRcvIdx = 0;
            this.myDongle = new dongleInterface(vid, pid, bufSize);
            this.myDongle.EventDataReceive += this.DataReceive;
        }

        public bool UpdateRate(Int32 newRate)
        {
            const int bufLen = 11;
            if (this.myDongle.state != dongleState.CONNECTED)
            {
                Console.WriteLine("Device not connected!");
                return false;
            }
            byte[] buf = new byte[bufLen];
            buf[0] = TAG_CMD;
            /* 4-Byte Length */
            buf[1] = 0;
            buf[2] = 0;
            buf[3] = 0;
            buf[4] = 0;
            buf[5] = (byte)(commandId.ENCODER_SET_RATE);
            buf[6] = (byte)(newRate & 0xFF);
            buf[7] = (byte)((newRate >> 8) & 0xFF);
            buf[8] = (byte)((newRate >> 16) & 0xFF);
            buf[9] = (byte)((newRate >> 24) & 0xFF);
            buf[10] = 0; /* checksum */
            this.SendCommand(buf, bufLen);
            return true;
        }

        private void SendCommand(byte[] buf, int length)
        {
            byte sum = 0;
            buf[1] = (byte)(length & 0xFF);
            buf[2] = (byte)((length >> 8) & 0xFF);
            buf[3] = (byte)((length >> 16) & 0xFF);
            buf[4] = (byte)((length >> 24) & 0xFF);
            for (int i = 0; i < (length - 1); i++)
            {
                sum = (byte)(sum + buf[i]);
            }
            buf[length - 1] = (byte)(0x100 - sum);
            this.myDongle.Send(buf, length);
        }

        private void DataReceive(object sender, dongle.Events.DataReceiveEventArgs e)
        {
            int availableBytes = 0;
            int length = 0;
            int idx = 0;
            byte sum = 0;

            if (e.buf.Length > 0)
            {
                for (int i = 0; i < e.buf.Length; i++)
                {
                    this.receiveBuf[this.wrRcvIdx] = e.buf[i];
                    this.wrRcvIdx = (this.wrRcvIdx + 1) % bufSize;
                }
            }

            while (this.wrRcvIdx != this.rdRcvIdx)
            {
                /* Check start of command TAG */
                if ((TAG_CMD != this.receiveBuf[this.rdRcvIdx]) &&
                    (TAG_STATUS != this.receiveBuf[this.rdRcvIdx]))
                {
                    // Skip character
                    this.rdRcvIdx = (this.rdRcvIdx + 1) % bufSize;
                    continue;
                }

                /* Get available bytes in the buffer */
                if (this.wrRcvIdx >= this.rdRcvIdx)
                {
                    availableBytes = this.wrRcvIdx - this.rdRcvIdx;
                }
                else
                {
                    availableBytes = (this.wrRcvIdx + bufSize) - this.rdRcvIdx;
                }
                if (availableBytes < 5)
                {
                    /*
                     * Minimum of five bytes to proceed
                     * 1byte(TAG) + 4bytes(Length)
                     */
                    break;
                }
                // See if the packet size byte is valid.  A command packet must be at
                // least four bytes and can not be larger than the receive buffer size.
                length = (int)(this.receiveBuf[(this.rdRcvIdx + 1) % bufSize]) +
                        ((int)(this.receiveBuf[(this.rdRcvIdx + 2) % bufSize]) << 8) +
                        ((int)(this.receiveBuf[(this.rdRcvIdx + 3) % bufSize]) << 16) +
                        ((int)(this.receiveBuf[(this.rdRcvIdx + 4) % bufSize]) << 24);

                if ((length < 6) || (length > (bufSize - 1)))
                {
                    // The packet size is too large, so either this is not the start of
                    // a packet or an invalid packet was received.  Skip this start of
                    // command packet tag.
                    this.rdRcvIdx = (this.rdRcvIdx + 1) % bufSize;

                    // Keep scanning for a start of command packet tag.
                    continue;
                }

                // If the entire command packet is not in the receive buffer then stop
                if (availableBytes < length)
                {
                    break;
                }

                // The entire command packet is in the receive buffer, so compute its
                // checksum.
                for (idx = 0, sum = 0; idx < length; idx++)
                {
                    sum += this.receiveBuf[(this.rdRcvIdx + idx) % bufSize];
                }

                // Skip this packet if the checksum is not correct (that is, it is
                // probably not really the start of a packet).
                if (sum != 0)
                {
                    // Skip this character
                    this.rdRcvIdx = (this.rdRcvIdx + 1) % bufSize;

                    // Keep scanning for a start of command packet tag.
                    continue;
                }

                // A valid command packet was received, so process it now.
                byte[] frameBuf = new byte[length - 6];
                for (idx = 0; idx < (length - 6); idx++)
                {
                    frameBuf[idx] = this.receiveBuf[(this.rdRcvIdx + 5 + idx) % bufSize];
                }
                if (this.EventFrameReceive != null)
                {
                    if (this.uiForm != null)
                    {
                        this.uiForm.BeginInvoke(this.EventFrameReceive, this,
                            new Events.FrameReceiveEventArgs(frameBuf, frameBuf.Length));
                    }
                    else
                    {
                        this.EventFrameReceive.Invoke(this,
                            new Events.FrameReceiveEventArgs(frameBuf, frameBuf.Length));
                    }
                }

                // Done with processing this command packet.
                this.rdRcvIdx = (this.rdRcvIdx + length) % bufSize;
            }
        }

    }
}
