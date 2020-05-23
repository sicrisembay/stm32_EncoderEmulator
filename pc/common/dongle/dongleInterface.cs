using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace dongle
{
    namespace Events {
        public delegate void DataReceiveEventHandler(object sender, dongle.Events.DataReceiveEventArgs e);
        public class DataReceiveEventArgs : EventArgs {
            public readonly byte[] buf;
            public DataReceiveEventArgs(byte[] buf, int len)
            {
                this.buf = new byte[len];
                for(int i=0; i < len; i++)
                {
                    this.buf[i] = buf[i];
                }
            }
        }

        public delegate void DeviceConnectEventHandler(object sender, dongle.Events.DeviceConnectEventArgs e);
        public class DeviceConnectEventArgs : EventArgs {
            public readonly bool IsConnected;
            public DeviceConnectEventArgs(bool isConnected)
            {
                this.IsConnected = isConnected;
            }
        }
    }

    public enum dongleState {
        STOPPED = 0,
        WAITING_CONNECTION,
        CONNECTED
    }

    public class dongleInterface
    {
        // Private members
        private int bufferSize = 2048;
        private bool running;
        private bool device_connected;
        private UsbDevice MyUsbDevice;
        private UsbDeviceFinder MyUsbFinder;
        private UsbEndpointReader epReader;
        private UsbEndpointWriter epWriter;
        private ErrorCode ec = ErrorCode.None;
        private Thread USB_receive;
        private byte[] readBuffer;
        private int nBytesReceived;
        private Thread USB_connect;
        private byte[] writeBuffer;
        private int vid;
        private int pid;
        private string sn;

        // Public members
        public dongleState state { get; private set; }
        public List<string> DevSerialList { get; private set; }

        // Event
        public event dongle.Events.DataReceiveEventHandler EventDataReceive;
        public event dongle.Events.DeviceConnectEventHandler EventDeviceConnect;

        public dongleInterface(int vid, int pid, int bufSize)
        {
            this.bufferSize = bufSize;
            this.running = false;
            this.device_connected = false;
            this.vid = vid;
            this.pid = pid;
            this.MyUsbDevice = null;
            this.readBuffer = new byte[this.bufferSize];
            this.writeBuffer = new byte[this.bufferSize];
            this.state = dongleState.STOPPED;
            this.ListDevice();
        }

        public void Connect(string serialNumber)
        {
            if(!this.running)
            {
                try
                {
                    this.sn = serialNumber;
                    this.MyUsbFinder = new UsbDeviceFinder(this.vid, this.pid, this.sn);
                    USB_connect = new Thread(USB_connection);
                    this.running = true;
                    USB_connect.Start();
                } catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    this.running = false;
                }
            }
        }

        public void ListDevice()
        {
            UsbRegDeviceList usbRegistryDevices = UsbDevice.AllDevices.FindAll(new UsbDeviceFinder(this.vid, this.pid));
            this.DevSerialList = new List<string>();
            if (usbRegistryDevices.Count == 0)
            {
                Console.WriteLine("No Dongle Found!");
                return;
            }

            Console.WriteLine(usbRegistryDevices.Count + " Dongle found.");
            for(int i = 0; i < usbRegistryDevices.Count; i++)
            {
                Console.WriteLine("Dev" + i + " : " + usbRegistryDevices[i].Device.UsbRegistryInfo.Device.Info.SerialString);
                this.DevSerialList.Add(usbRegistryDevices[i].Device.UsbRegistryInfo.Device.Info.SerialString);
            }

        }

        public void Disconnect()
        {
            this.running = false;
            this.device_connected = false;
            Thread.Sleep(100);
            this.Close();
            this.state = dongleState.STOPPED;
        }

        public void Exit()
        {
            this.Disconnect();
            UsbDevice.Exit();
        }

        public bool Send(byte[] buf, int len)
        {
            int bytesWritten = 0;
            if(len > this.bufferSize)
            {
                Console.WriteLine("Exceed buffer size!");
                return false;
            }
            for(int i = 0; i < len; i++)
            {
                this.writeBuffer[i] = buf[i];
            }
            ErrorCode errCode = this.epWriter.Transfer(this.writeBuffer, 0, len, 5000, out bytesWritten);
            if (errCode == ErrorCode.None)
            {
                return true;
            } else
            {
                Console.WriteLine("Send Failed! " + errCode);
                return false;
            }
        }

        private void USB_connection()
        {
            this.state = dongleState.WAITING_CONNECTION;
            Console.WriteLine("Waiting for device connection");

            while(this.running == true)
            {
                if(this.device_connected == false)
                {
                    while((this.MyUsbDevice == null) && (this.running == true))
                    {
                        Thread.Sleep(200);
                        this.MyUsbDevice = UsbDevice.OpenUsbDevice(this.MyUsbFinder);
                    }
                    if(this.running == true)
                    {
                        try
                        {
                            this.USB_init();
                            this.device_connected = true;
                            this.state = dongleState.CONNECTED;
                            Console.WriteLine("Device Connected");
                        } catch(Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
                Thread.Sleep(100);
            }

            this.state = dongleState.STOPPED;
        }

        private void USB_init()
        {
            IUsbDevice wholeUsbDevice = this.MyUsbDevice as IUsbDevice;
            if(!ReferenceEquals(wholeUsbDevice, null))
            {
                // Select Config #1
                wholeUsbDevice.SetConfiguration(1);
                // Claim Interface #0
                wholeUsbDevice.ClaimInterface(0);
            }
            this.epReader = this.MyUsbDevice.OpenEndpointReader(ReadEndpointID.Ep01);
            this.epWriter = this.MyUsbDevice.OpenEndpointWriter(WriteEndpointID.Ep01);
            this.USB_receive = new Thread(USBReceive);
            this.USB_receive.Start();
        }

        private void USBReceive()
        {
            try
            {
                while(this.device_connected && ((this.ec == ErrorCode.None) || (this.ec == ErrorCode.IoTimedOut)))
                {
                    int bytesRead = 0;
                    this.ec = this.epReader.Transfer(this.readBuffer, 0, this.bufferSize, 500, out bytesRead);
                    this.nBytesReceived = bytesRead;
                    lock(USB_receive)
                    {
                        if(this.running && (this.ec == ErrorCode.None))
                        {
                            if(this.EventDataReceive != null)
                            {
                                this.EventDataReceive(this, new Events.DataReceiveEventArgs(this.readBuffer, bytesRead));
                            }
                        }
                    }
                }
            } catch(Exception ex)
            {
                Console.WriteLine((this.ec != ErrorCode.None ? ec + ":" : String.Empty) + ex.Message);
            } finally
            {
                if(this.running)
                {
                    Thread.Sleep(200);
                    this.device_connected = false;
                    this.state = dongleState.WAITING_CONNECTION;
                    Console.WriteLine("Waiting for device connection");
                    this.Close();
                    this.ec = ErrorCode.None;
                }
            }
        }

        private void Close()
        {
            if (this.MyUsbDevice != null)
            {
                IUsbDevice wholeUsbDevice = this.MyUsbDevice as IUsbDevice;
                if (!ReferenceEquals(wholeUsbDevice, null))
                {
                    wholeUsbDevice.ReleaseInterface(0);
                }
                this.MyUsbDevice.Close();
            }
            this.MyUsbDevice = null;
        }
    }
}
