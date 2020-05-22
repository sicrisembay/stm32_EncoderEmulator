#include "main.h"
#include "encoder.h"

#define TIM_CLK_FREQ        (72000000U) /* 72MHz */
#define MAX_PERIOD_VAL      (0xFFFF)
#define MAX_PRESCALE_VAL    (0xFFFF)

volatile uint32_t state = 0;
volatile int32_t positionCount = 0;
volatile int32_t rateHz = 0;
volatile int32_t shadowRateHz = 0;
volatile uint32_t bCountUp = 1;
volatile uint32_t bEnabled = 0;
volatile uint16_t timer_prescaler = 0;
volatile uint16_t timer_period = 0;

void encoder_init(void)
{
    rateHz = 0;
    shadowRateHz = 0;
    bCountUp = 1;
    bEnabled = 0;
    state = 0;
    LL_GPIO_ResetOutputPin(ENC_PHA_GPIO_Port, ENC_PHA_Pin);
    LL_GPIO_ResetOutputPin(ENC_PHB_GPIO_Port, ENC_PHB_Pin);
}

static void encoder_enable(uint8_t bEnable)
{
    if(bEnable) {
        LL_TIM_SetCounter(TIM1, 0);
        LL_TIM_ClearFlag_UPDATE(TIM1);
        LL_TIM_EnableIT_UPDATE(TIM1);
        NVIC_EnableIRQ(TIM1_UP_IRQn);
        LL_TIM_EnableCounter(TIM1);
        bEnabled = 1;
    } else {
        LL_TIM_DisableCounter(TIM1);
        LL_TIM_DisableIT_UPDATE(TIM1);
        NVIC_DisableIRQ(TIM1_UP_IRQn);
        bEnabled = 0;
    }
}

static void _encoder_calc_prescale_period(void)
{
    int32_t targetRate4x = 0;
    uint32_t calcPrescale = 1;
    uint32_t calcPeriod = 0;

    if(rateHz < 0) {
        targetRate4x = -(rateHz * 4);
    } else {
        targetRate4x = rateHz * 4;
    }

    while(TIM_CLK_FREQ/((MAX_PERIOD_VAL + 1) * calcPrescale) > targetRate4x) {
        calcPrescale++;
    }
    /* add 1 to cover truncation */
    calcPrescale++;

    if((calcPrescale - 1) > MAX_PRESCALE_VAL) {
        /* Not a valid timer prescale */
        return;
    }
    calcPeriod = (TIM_CLK_FREQ / (calcPrescale)) / targetRate4x;
    if((calcPeriod - 1) > MAX_PERIOD_VAL) {
        /* Not a valid timer period */
        return;
    }

    timer_prescaler = calcPrescale - 1;
    timer_period = calcPeriod - 1;
}

uint8_t encoder_set_rate(int32_t newVal)
{
    uint8_t ret = 0;
    shadowRateHz = newVal;
    if(shadowRateHz == 0) {
        encoder_enable(0);
    } else {
        if(bEnabled == 0) {
            rateHz = shadowRateHz;
            _encoder_calc_prescale_period();
            LL_TIM_SetPrescaler(TIM1, timer_prescaler);
            LL_TIM_SetAutoReload(TIM1, timer_period);
            encoder_enable(1);
        }
    }

    return(ret);
}

int32_t encoder_get_rate(void)
{
    return(rateHz);
}

uint8_t encoder_set_posCount(int32_t newVal)
{
    positionCount = newVal;
    return 0;
}

int32_t encoder_get_posCount(void)
{
    return(positionCount);
}


void encoder_event_update(void)
{
    /*
     * Update timer
     */
    if(rateHz != shadowRateHz) {
        rateHz = shadowRateHz;
        _encoder_calc_prescale_period();
        LL_TIM_SetPrescaler(TIM1, timer_prescaler);
        LL_TIM_SetAutoReload(TIM1, timer_period);
    }

    if(rateHz == 0) {
        return;
    }

    /*
     * bCountUp is 1: 0b00 -> 0b01 -> 0b11 -> 0b10 -> 0b00
     * bCountUp is 0: 0b00 -> 0b10 -> 0b11 -> 0b01 -> 0b00
     */
    if(rateHz < 0) {
        bCountUp = 0;
    } else {
        bCountUp = 1;
    }
    switch(state) {
        case 0b00:
            if(bCountUp == 1) {
                positionCount++;
                state = 0b01;
                LL_GPIO_SetOutputPin(ENC_PHA_GPIO_Port, ENC_PHA_Pin);
                LL_GPIO_ResetOutputPin(ENC_PHB_GPIO_Port, ENC_PHB_Pin);
            } else {
                positionCount--;
                state = 0b10;
                LL_GPIO_ResetOutputPin(ENC_PHA_GPIO_Port, ENC_PHA_Pin);
                LL_GPIO_SetOutputPin(ENC_PHB_GPIO_Port, ENC_PHB_Pin);
            }
            break;
        case 0b01:
            if(bCountUp == 1) {
                positionCount++;
                state = 0b11;
                LL_GPIO_SetOutputPin(ENC_PHA_GPIO_Port, ENC_PHA_Pin);
                LL_GPIO_SetOutputPin(ENC_PHB_GPIO_Port, ENC_PHB_Pin);
            } else {
                positionCount--;
                state = 0b00;
                LL_GPIO_ResetOutputPin(ENC_PHA_GPIO_Port, ENC_PHA_Pin);
                LL_GPIO_ResetOutputPin(ENC_PHB_GPIO_Port, ENC_PHB_Pin);
            }
            break;
        case 0b11:
            if(bCountUp == 1) {
                positionCount++;
                state = 0b10;
                LL_GPIO_ResetOutputPin(ENC_PHA_GPIO_Port, ENC_PHA_Pin);
                LL_GPIO_SetOutputPin(ENC_PHB_GPIO_Port, ENC_PHB_Pin);
            } else {
                positionCount--;
                state = 0b01;
                LL_GPIO_SetOutputPin(ENC_PHA_GPIO_Port, ENC_PHA_Pin);
                LL_GPIO_ResetOutputPin(ENC_PHB_GPIO_Port, ENC_PHB_Pin);
            }
            break;
        case 0b10:
            if(bCountUp == 1) {
                positionCount++;
                state = 0b00;
                LL_GPIO_ResetOutputPin(ENC_PHA_GPIO_Port, ENC_PHA_Pin);
                LL_GPIO_ResetOutputPin(ENC_PHB_GPIO_Port, ENC_PHB_Pin);
            } else {
                positionCount--;
                state = 0b11;
                LL_GPIO_SetOutputPin(ENC_PHA_GPIO_Port, ENC_PHA_Pin);
                LL_GPIO_SetOutputPin(ENC_PHB_GPIO_Port, ENC_PHB_Pin);
            }
            break;
        default:
            state = 0b00;
            LL_GPIO_ResetOutputPin(ENC_PHA_GPIO_Port, ENC_PHA_Pin);
            LL_GPIO_ResetOutputPin(ENC_PHB_GPIO_Port, ENC_PHB_Pin);
            break;
    }
}
