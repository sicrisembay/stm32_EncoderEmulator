#ifndef _FRAME_PARSER_H_
#define _FRAME_PARSER_H_

#include "stdint.h"

/*
 * Frame Format
 *   TAG    : 1 byte
 *   Length : 4 bytes
 *   Payload: N Bytes
 *   Checksum: 1 byte
 */
#define TAG_CMD                 (0xff)  //!< The value of the tag byte for a command packet.
#define TAG_STATUS              (0xfe)  //!< The value of the tag byte for a status packet.

/*
 * Payload Format
 *
 */

#define CMD_GET_DEVICE_ID       (0x00)

#define ENCODER_SET_RATE        (0x22)
#define ENCODER_GET_RATE        (0x23)
#define ENCODER_SET_COUNT       (0x24)
#define ENCODER_GET_COUNT       (0x25)

void PARSER_Store(uint8_t *pBuf, uint32_t len);
void PARSER_Process();
void PARSER_GetTxBlock(uint8_t * pBuf, uint32_t * pSize);
void PARSER_Send(uint8_t *pBuf, uint32_t len);

#endif /* _FRAME_PARSER_H_ */
