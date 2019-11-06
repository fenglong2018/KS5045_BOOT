using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KS5045上位机
{
    public static class Consts
    {
        public const UInt32 CAN_ID = 0x20000;
        public const UInt32 TX_CAN_ID = 0x10000;
        public const UInt32 BAT_CAN_ID = 0x0300;


        public const UInt32 TX_CAN_ID_1 = 0x150F0;
        public const UInt32 TX_CAN_ID_2 = 0x110F0;
        public const UInt32 TX_CAN_ID_3 = 0x120F0;
        public const UInt32 TX_CAN_ID_4 = 0x130F0;
        public const UInt32 TX_CAN_ID_5 = 0x140F0;

        public const UInt32 RX_CAN_ID = 0x260F0;

        public const UInt32 BAT_ID = 0x18FF5005;
        public const UInt32 MASTER_ID = 0x18FF5000;

        public const UInt16 Per_Pack_num = 192;


    }
}
