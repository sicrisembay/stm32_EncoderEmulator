#ifndef ENCODER_H
#define ENCODER_H

#include "stdint.h"

void encoder_init(void);
uint8_t encoder_set_rate(int32_t newVal);
int32_t encoder_get_rate(void);
uint8_t encoder_set_posCount(int32_t newVal);
int32_t encoder_get_posCount(void);
void encoder_event_update(void);

#endif /* ENCODER_H */
