#include "frameParser.h"
#include "main.h"
#include "encoder.h"

#define FRAME_RX_SIZE       (1024)
#define FRAME_TX_SIZE       (1024)

volatile uint32_t rdPtr = 0;
volatile uint32_t wrPtr = 0;
static uint8_t rxFrameBuffer[FRAME_RX_SIZE];
volatile uint32_t rdTxPtr = 0;
volatile uint32_t wrTxPtr = 0;
static uint8_t txFrameBuffer[FRAME_TX_SIZE];

static void _ProcessValidFrame(uint32_t index, uint32_t len)
{
    uint8_t responseBuffer[128];
    uint32_t respLen = 0;
    uint8_t cmd;
    /* Command */
    index = (index + 5) % FRAME_RX_SIZE;
    cmd = rxFrameBuffer[index];
    switch(cmd) {
        case CMD_GET_DEVICE_ID: {
            respLen = 8;
            responseBuffer[0] = TAG_STATUS;
            responseBuffer[1] = 0;
            responseBuffer[2] = 0;
            responseBuffer[3] = 0;
            responseBuffer[4] = 0;
            responseBuffer[5] = CMD_GET_DEVICE_ID;
            responseBuffer[6] = 0xAA;
            PARSER_Send(responseBuffer, respLen);
            break;
        }
        case ENCODER_SET_RATE: {
            int32_t newRate;
            /* Rate */
            index = (index + 1) % FRAME_RX_SIZE;
            newRate = rxFrameBuffer[index];
            index = (index + 1) % FRAME_RX_SIZE;
            newRate += (int32_t)(((uint32_t)rxFrameBuffer[index]) << 8);
            index = (index + 1) % FRAME_RX_SIZE;
            newRate += (int32_t)(((uint32_t)rxFrameBuffer[index]) << 16);
            index = (index + 1) % FRAME_RX_SIZE;
            newRate += (int32_t)(((uint32_t)rxFrameBuffer[index]) << 24);

            respLen = 0;
            responseBuffer[respLen++] = TAG_STATUS;
            /* 4-byte Length */
            responseBuffer[respLen++] = 0;
            responseBuffer[respLen++] = 0;
            responseBuffer[respLen++] = 0;
            responseBuffer[respLen++] = 0;
            responseBuffer[respLen++] = ENCODER_SET_RATE;
            responseBuffer[respLen++] = encoder_set_rate(newRate);
            respLen++; // Checksum
            PARSER_Send(responseBuffer, respLen);
            break;
        }
        case ENCODER_GET_RATE: {
            int32_t rateHz;
            rateHz = encoder_get_rate();
            respLen = 0;
            responseBuffer[respLen++] = TAG_STATUS;
            /* 4-byte Length */
            responseBuffer[respLen++] = 0;
            responseBuffer[respLen++] = 0;
            responseBuffer[respLen++] = 0;
            responseBuffer[respLen++] = 0;
            responseBuffer[respLen++] = ENCODER_GET_RATE;
            responseBuffer[respLen++] = (uint8_t)(rateHz & 0xFF);
            responseBuffer[respLen++] = (uint8_t)((rateHz >> 8) & 0xFF);
            responseBuffer[respLen++] = (uint8_t)((rateHz >> 16) & 0xFF);
            responseBuffer[respLen++] = (uint8_t)((rateHz >> 24) & 0xFF);
            respLen++; // Checksum
            PARSER_Send(responseBuffer, respLen);
            break;
        }
        case ENCODER_SET_COUNT: {
            int32_t newPos;
            /* Rate */
            index = (index + 1) % FRAME_RX_SIZE;
            newPos = rxFrameBuffer[index];
            index = (index + 1) % FRAME_RX_SIZE;
            newPos += (int32_t)(((uint32_t)rxFrameBuffer[index]) << 8);
            index = (index + 1) % FRAME_RX_SIZE;
            newPos += (int32_t)(((uint32_t)rxFrameBuffer[index]) << 16);
            index = (index + 1) % FRAME_RX_SIZE;
            newPos += (int32_t)(((uint32_t)rxFrameBuffer[index]) << 24);

            respLen = 0;
            responseBuffer[respLen++] = TAG_STATUS;
            /* 4-byte Length */
            responseBuffer[respLen++] = 0;
            responseBuffer[respLen++] = 0;
            responseBuffer[respLen++] = 0;
            responseBuffer[respLen++] = 0;
            responseBuffer[respLen++] = ENCODER_SET_COUNT;
            responseBuffer[respLen++] = encoder_set_posCount(newPos);
            respLen++; // Checksum
            PARSER_Send(responseBuffer, respLen);
            break;
        }
        case ENCODER_GET_COUNT: {
            int32_t posCount;
            posCount = encoder_get_posCount();
            respLen = 0;
            responseBuffer[respLen++] = TAG_STATUS;
            /* 4-byte Length */
            responseBuffer[respLen++] = 0;
            responseBuffer[respLen++] = 0;
            responseBuffer[respLen++] = 0;
            responseBuffer[respLen++] = 0;
            responseBuffer[respLen++] = ENCODER_GET_COUNT;
            responseBuffer[respLen++] = (uint8_t)(posCount & 0xFF);
            responseBuffer[respLen++] = (uint8_t)((posCount >> 8) & 0xFF);
            responseBuffer[respLen++] = (uint8_t)((posCount >> 16) & 0xFF);
            responseBuffer[respLen++] = (uint8_t)((posCount >> 24) & 0xFF);
            respLen++; // Checksum
            PARSER_Send(responseBuffer, respLen);
            break;
        }
        default:
            break;
    }
}

void PARSER_Store(uint8_t *pBuf, uint32_t len)
{
    uint32_t i = 0;
    for(i = 0; i < len; i++) {
        rxFrameBuffer[wrPtr] = pBuf[i];
        wrPtr = (wrPtr + 1) % FRAME_RX_SIZE;
    }
}

void PARSER_Process()
{
    uint32_t availableBytes = 0;
    uint32_t length = 0;
    uint32_t idx = 0;
    uint8_t sum = 0;

    while(wrPtr != rdPtr) {
        /* Check start of command TAG */
        if(TAG_CMD != rxFrameBuffer[rdPtr])
        {
            // Skip character
            rdPtr = (rdPtr + 1) % FRAME_RX_SIZE;
            continue;
        }

        /* Get available bytes in the buffer */
        if(wrPtr >= rdPtr) {
            availableBytes = wrPtr - rdPtr;
        } else {
            availableBytes = (wrPtr + FRAME_RX_SIZE) - rdPtr;
        }
        if(availableBytes < 5) {
            /*
             * Minimum of five bytes to proceed
             * 1byte(TAG) + 4bytes(Length)
             */
            break;
        }
        // See if the packet size byte is valid.  A command packet must be at
        // least four bytes and can not be larger than the receive buffer size.
        length = (uint32_t)(rxFrameBuffer[(rdPtr+1)%FRAME_RX_SIZE]) +
                ((uint32_t)(rxFrameBuffer[(rdPtr+2)%FRAME_RX_SIZE]) << 8) +
                ((uint32_t)(rxFrameBuffer[(rdPtr+3)%FRAME_RX_SIZE]) << 16) +
                ((uint32_t)(rxFrameBuffer[(rdPtr+4)%FRAME_RX_SIZE]) << 24);

        if((length < 6) || (length > (FRAME_RX_SIZE-1)))
        {
            // The packet size is too large, so either this is not the start of
            // a packet or an invalid packet was received.  Skip this start of
            // command packet tag.
            rdPtr = (rdPtr + 1) % FRAME_RX_SIZE;

            // Keep scanning for a start of command packet tag.
            continue;
        }

        // If the entire command packet is not in the receive buffer then stop
        if(availableBytes < length)
        {
            break;
        }

        // The entire command packet is in the receive buffer, so compute its
        // checksum.
        for(idx = 0, sum = 0; idx < length; idx++)
        {
            sum += rxFrameBuffer[(rdPtr + idx)%FRAME_RX_SIZE];
        }

        // Skip this packet if the checksum is not correct (that is, it is
        // probably not really the start of a packet).
        if(sum != 0)
        {
            // Skip this character
            rdPtr = (rdPtr + 1) % FRAME_RX_SIZE;

            // Keep scanning for a start of command packet tag.
            continue;
        }

        // A valid command packet was received, so process it now.
        _ProcessValidFrame(rdPtr, length);

        // Done with processing this command packet.
        rdPtr = (rdPtr + length) % FRAME_RX_SIZE;
    }
}

void PARSER_GetTxBlock(uint8_t * pBuf, uint32_t * pSize)
{
    uint32_t availableBytes;
    uint32_t i;
    if(wrTxPtr >= rdTxPtr) {
        availableBytes = wrTxPtr - rdTxPtr;
    } else {
        availableBytes = (FRAME_TX_SIZE + wrTxPtr) - rdTxPtr;
    }

    if(*pSize > availableBytes) {
        *pSize = availableBytes;
    }

    if(*pSize == 0) {
        return;
    }

    for(i = 0; i < *pSize; i++) {
        pBuf[i] = txFrameBuffer[rdTxPtr];
        rdTxPtr = (rdTxPtr + 1) % FRAME_TX_SIZE;
    }
}

void PARSER_Send(uint8_t *pBuf, uint32_t len)
{
    uint32_t i;
    uint8_t sum = 0;
    /*
     * Set Length
     */
    for(i = 0; i < 4; i++) {
        pBuf[1+i] = (uint8_t)((len >> (8*i)) & 0xFF);
    }
    /*
     * Calculate Checksum
     */
    for(i = 0; i < (len-1); i++) {
        sum += pBuf[i];
    }
    pBuf[i] = (uint8_t)((~sum) + 1);
    /*
     * Write to Tx Buffer
     */
    for(i = 0; i < len; i++) {
        txFrameBuffer[wrTxPtr] = pBuf[i];
        wrTxPtr = (wrTxPtr + 1) % FRAME_TX_SIZE;
    }
}
