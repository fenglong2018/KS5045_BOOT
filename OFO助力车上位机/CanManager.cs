using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace KS5045上位机
{
    class CanManager
    {
        #region Members
        UInt32 m_devtype;
        UInt32 m_devind;
        UInt32 m_canind;
        UInt32 m_timing;
        UInt32 m_canid;
        #endregion Members


        public UInt32 CanId
        {
            get
            {
                return m_canid;
            }
            set
            {
                m_canid = value;
            }

        }

        public CanManager()
        {
            m_devtype = (UInt32)4;
            m_devind = (UInt32)0;
            m_timing = (UInt32)8;
            m_canid = 0;
        }
        public void init(UInt32 chanl, UInt32 baute)
        {
            m_devtype = (UInt32)4;//类型
            m_devind = (UInt32)0;//索引号
            m_canind = (UInt32)chanl;//通道1
            m_timing = (UInt32)baute;//250k 波特率
        }
        public void start()
        {

            //init((UInt32)0, 8);//8 = 250k  10 = 500k


            //VCI_INIT_CONFIG config = new VCI_INIT_CONFIG();
            ////           UInt32 can_id = Consts.MASTER_CAN_ID;   //滤波
            ////            can_id <<= 21;


            //config.AccCode = System.Convert.ToUInt32("0x80000000", 16);//can_id;// System.Convert.ToUInt32("0x80000000", 16);
            //config.AccMask = System.Convert.ToUInt32("0xFFFFFFFF", 16);//0x1fffffff;//过滤前3BIT  0x1f000000

            //config.Timing0 = System.Convert.ToByte(Kbps((int)m_timing, 0), 16);
            //config.Timing1 = System.Convert.ToByte(Kbps((int)m_timing, 1), 16);

            //config.Filter = (Byte)(1);//接收方式 1:接收全部类型 2:只接收标准帧 3:只接收扩展帧

            //config.Mode = (Byte)0;//模式 0 正常 1只听 2 自测

            //UInt32 res = new UInt32();


            //res = VCI_InitCAN(m_devtype, m_devind, m_canind, ref config);//初始化CAN参数f
            //if (res == 1)       //操作成功
            //{
            //    VCI_StartCAN(m_devtype, m_devind, m_canind); //启动CAN
            //}


            VCI_INIT_CONFIG config = new VCI_INIT_CONFIG();
            UInt32 can_id = Consts.CAN_ID;   //滤波
            can_id <<= 21;
            config.AccCode = System.Convert.ToUInt32("0x80000000", 16);//can_id;// System.Convert.ToUInt32("0x80000000", 16);
            config.AccMask = System.Convert.ToUInt32("0xFFFFFFFF", 16);//0x1fffffff;//过滤前3BIT  0x1f000000

            config.Timing0 = System.Convert.ToByte(Kbps((int)m_timing, 0), 16);
            config.Timing1 = System.Convert.ToByte(Kbps((int)m_timing, 1), 16);

            config.Filter = (Byte)(1);//接收方式 1:接收全部类型 2:只接收标准帧 3:只接收扩展帧

            config.Mode = (Byte)0;//模式 0 正常 1只听 2 自测
            UInt32 res = new UInt32();
            res = VCI_InitCAN(m_devtype, m_devind, m_canind, ref config);//初始化CAN参数f
            if (res == 1)
            {
                VCI_StartCAN(m_devtype, m_devind, m_canind); //启动CAN
            }
        }

        public bool Open()
        {
            if (VCI_OpenDevice(m_devtype, m_devind, m_canind) == 0)//0：表示操作失败
            {
                return false;
            }
            return true;

        }

        public void Close()
        {
            VCI_CloseDevice(m_devtype, m_devind);
        }

        public void Reset()
        {
            VCI_ResetCAN(m_devtype, m_devind, m_canind);
            VCI_ClearBuffer(m_devtype, m_devind, m_canind);
            VCI_CloseDevice(m_devtype, m_devind);

            if (Open())
            {
                VCI_INIT_CONFIG config = new VCI_INIT_CONFIG();
                UInt32 can_id = Consts.CAN_ID;   //滤波
                can_id <<= 21;
                config.AccCode = System.Convert.ToUInt32("0x80000000", 16);//can_id;// System.Convert.ToUInt32("0x80000000", 16);
                config.AccMask = System.Convert.ToUInt32("0xFFFFFFFF", 16);//0x1fffffff;//过滤前3BIT  0x1f000000

                config.Timing0 = System.Convert.ToByte(Kbps((int)m_timing, 0), 16);
                config.Timing1 = System.Convert.ToByte(Kbps((int)m_timing, 1), 16);
                config.Filter = (Byte)(1);//接收方式 1:接收全部类型 2:只接收标准帧 3:只接收扩展帧

                config.Mode = (Byte)0;//模式 0 正常 1只听 2 自测

                UInt32 res = new UInt32();
                res = VCI_InitCAN(m_devtype, m_devind, m_canind, ref config);//初始化CAN参数f
                if (res == 1)
                {
                    VCI_StartCAN(m_devtype, m_devind, m_canind); //启动CAN
                }
            }
        }

        public void ClearBuffer()
        {
            VCI_ClearBuffer(m_devtype, m_devind, m_canind);
        }

        public UInt32 GetReceiveNum()
        {
            
            UInt32 a = (VCI_GetReceiveNum(m_devtype, m_devind, m_canind));
            return a;
        }

        public UInt32 Receive(ref VCI_CAN_OBJ pReceive)
        {
            return (VCI_Receive(m_devtype, m_devind, m_canind, ref pReceive, 1000, 100));
        }

        public Int32 StandardWrite(Byte[] write_buffer,byte len,int state)
        {
            VCI_CAN_OBJ[] vco = new VCI_CAN_OBJ[1];
            uint dwRel;

            vco[0].ID = Consts.MASTER_ID;

            //switch (state)
            //{ 
            //    case 0x01:
            //        vco[0].ID = Consts.TX_CAN_ID_1;
            //        break;
            //    case 0x02:
            //        vco[0].ID = Consts.TX_CAN_ID_2;
            //        break;
            //    case 0x03:
            //        vco[0].ID = Consts.TX_CAN_ID_3;
            //        break;
            //    case 0x04:
            //        vco[0].ID = Consts.TX_CAN_ID_4;
            //        break;
            //    case 0x05:
            //        vco[0].ID = Consts.TX_CAN_ID_5;
            //        break;
            //}
            vco[0].RemoteFlag = 0;
            vco[0].ExternFlag = 1;
            vco[0].DataLen = len;
            vco[0].SendType = 1; //只发一次
            unsafe
            {
                fixed (VCI_CAN_OBJ* m_recobj1 = &vco[0])
                {
                    for (int j = 0; j < 8; j++)
                    {
                        m_recobj1->Data[j] = write_buffer[j];
                    }
                }
            }

            
            try
            {

                dwRel = VCI_Transmit(m_devtype, m_devind, m_canind, ref vco[0], 1);
                if (dwRel == 1)
                {
                    return 1;
                }
                else if (dwRel == 0xffffffff)
                {
                    VCI_ResetCAN(m_devtype, m_devind, m_canind);
                    VCI_ClearBuffer(m_devtype, m_devind, m_canind);
                    //VCI_CloseDevice(m_devtype, m_devind);
                    //VCI_OpenDevice(m_devtype, m_devind, m_canind);
                    return 0;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception e)
            {
                //e.Message;
                return -1;
            }

        }

        //can通信定义区
        #region         
        //can通信定义
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        string str_ini_path = System.Environment.CurrentDirectory.ToString() + "//can.ini";

        StringBuilder skin_num = new StringBuilder();



        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_OpenDevice(UInt32 DeviceType, UInt32 DeviceInd, UInt32 Reserved);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_CloseDevice(UInt32 DeviceType, UInt32 DeviceInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_InitCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_INIT_CONFIG pInitConfig);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ReadBoardInfo(UInt32 DeviceType, UInt32 DeviceInd, ref VCI_BOARD_INFO pInfo);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ReadErrInfo(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_ERR_INFO pErrInfo);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ReadCANStatus(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_STATUS pCANStatus);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_GetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, ref byte pData);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_SetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, ref byte pData);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_GetReceiveNum(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ClearBuffer(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_StartCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ResetCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_Transmit(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pSend, UInt32 Len);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pReceive, UInt32 Len, Int32 WaitTime);

        [DllImport("controlcan.dll", CharSet = CharSet.Ansi)]
        static extern UInt32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, IntPtr pReceive, UInt32 Len, Int32 WaitTime);

        /*------------其他函数描述---------------------------------*/
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_GetReference2(UInt32 DevType, UInt32 DevIndex, UInt32 CANIndex, UInt32 Reserved, ref VCI_REF_STRUCT pRefStruct);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_SetReference2(UInt32 DevType, UInt32 DevIndex, UInt32 CANIndex, UInt32 RefType, ref byte pData);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ResumeConfig(UInt32 DevType, UInt32 DevIndex, UInt32 CANIndex);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ConnectDevice(UInt32 DevType, UInt32 DevIndex);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_UsbDeviceReset(UInt32 DevType, UInt32 DevIndex, UInt32 Reserved);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_FindUsbDevice(ref VCI_BOARD_INFO1 pInfo);


        private string Kbps(int kb, int flag)
        {
            string result = "";
            if (flag == 0)//波特率 timing0
            {
                switch (kb)
                {
                    case 0:
                        result = "0x9F";
                        break;
                    case 1:
                        result = "0x18";
                        break;
                    case 2:
                        result = "0x87";
                        break;
                    case 3:
                        result = "0x09";
                        break;
                    case 4:
                        result = "0x83";
                        break;
                    case 5:
                        result = "0x04";
                        break;
                    case 6:
                        result = "0x03";
                        break;
                    case 7:
                        result = "0x81";
                        break;
                    case 8:
                        result = "0x01";
                        break;
                    case 9:
                        result = "0x80";
                        break;
                    case 10:
                        result = "0x00";
                        break;
                    case 11:
                        result = "0x80";
                        break;
                    case 12:
                        result = "0x00";
                        break;
                    case 13:
                        result = "0x00";
                        break;
                    case 14:
                        result = "0x09";
                        break;
                    case 15:
                        result = "0x04";
                        break;
                    case 16:
                        result = "0x03";
                        break;
                    default:
                        break;
                }
            }
            else if (flag == 1)
            {
                switch (kb)
                {
                    case 0:
                        result = "0xFF";
                        break;
                    case 1:
                        result = "0x1C";
                        break;
                    case 2:
                        result = "0xFF";
                        break;
                    case 3:
                        result = "0x1C";
                        break;
                    case 4:
                        result = "0xFF";
                        break;
                    case 5:
                        result = "0x1C";
                        break;
                    case 6:
                        result = "0x1C";
                        break;
                    case 7:
                        result = "0xFA";
                        break;
                    case 8:
                        result = "0x1C";
                        break;
                    case 9:
                        result = "0xFA";
                        break;
                    case 10:
                        result = "0x1C";
                        break;
                    case 11:
                        result = "0xB6";
                        break;
                    case 12:
                        result = "0x16";
                        break;
                    case 13:
                        result = "0x14";
                        break;
                    case 14:
                        result = "0x6F";
                        break;
                    case 15:
                        result = "0x6F";
                        break;
                    case 16:
                        result = "0x6F";
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        /*------------数据类型---------------------------------*/

        //1.CAN系列接口卡信息的数据类型。
        public struct VCI_BOARD_INFO
        {

            //public UInt16 hw_Version;
            //public UInt16 fw_Version;
            //public UInt16 dr_Version;
            //public UInt16 in_Version;
            //public UInt16 irq_Num;
            //public byte can_Num;
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            //public byte[] str_Serial_Num;
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
            //public byte[] str_hw_Type;
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            //public byte[] Reserved;
        }

        /////////////////////////////////////////////////////
        //2.定义CAN信息帧的数据类型。
        unsafe public struct VCI_CAN_OBJ  //使用不安全代码
        {
            public uint ID;
            public uint TimeStamp;        //时间标识
            public byte TimeFlag;         //是否使用时间标识 0: 否 1：是
            public byte SendType;         //发送标志。保留，未用
            public byte RemoteFlag;       //是否是远程帧 0:数据帧 1: 远程帧
            public byte ExternFlag;       //是否是扩展帧 0：标准帧 1: 扩展帧
            public byte DataLen;

            public fixed byte Data[8];

            public fixed byte Reserved[3];

        }

        //3.定义CAN控制器状态的数据类型。
        public struct VCI_CAN_STATUS
        {
            //public byte ErrInterrupt;
            //public byte regMode;
            //public byte regStatus;
            //public byte regALCapture;
            //public byte regECCapture;
            //public byte regEWLimit;
            //public byte regRECounter;
            //public byte regTECounter;
            //public uint Reserved;
        }

        //4.定义错误信息的数据类型。
        public struct VCI_ERR_INFO
        {
            //public uint ErrCode;
            //public byte Passive_ErrData1;
            //public byte Passive_ErrData2;
            //public byte Passive_ErrData3;
            //public byte ArLost_ErrData;
        }

        //5.定义初始化CAN的数据类型
        public struct VCI_INIT_CONFIG
        {
            public UInt32 AccCode;
            public UInt32 AccMask;
            public UInt32 Reserved;
            public byte Filter;   //1接收所有帧。2标准帧滤波，3是扩展帧滤波。
            public byte Timing0;
            public byte Timing1;
            public byte Mode;     //模式，0表示正常模式，1表示只听模式,2自测模式
        }

        /*------------其他数据结构描述---------------------------------*/
        //6.USB-CAN总线适配器板卡信息的数据类型1，该类型为VCI_FindUsbDevice函数的返回参数。

        public struct VCI_BOARD_INFO1
        {
            //public UInt16 hw_Version;
            //public UInt16 fw_Version;
            //public UInt16 dr_Version;
            //public UInt16 in_Version;
            //public UInt16 irq_Num;
            //public byte can_Num;
            //public byte Reserved;
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            //public byte[] str_Serial_Num;
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            //public byte[] str_hw_Type;
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            //public byte[] str_Usb_Serial;
        }



        //7.定义常规参数类型
        public struct VCI_REF_NORMAL
        {
            //public byte Mode;     //模式，0表示正常模式，1表示只听模式,2自测模式
            //public byte Filter;   //1接收所有帧。2标准帧滤波，3是扩展帧滤波。
            //public UInt32 AccCode;//接收滤波验收码
            //public UInt32 AccMask;//接收滤波屏蔽码
            //public byte kBaudRate;//波特率索引号，0-SelfDefine,1-5Kbps(未用),2-18依次为：10kbps,20kbps,40kbps,50kbps,80kbps,100kbps,125kbps,200kbps,250kbps,400kbps,500kbps,666kbps,800kbps,1000kbps,33.33kbps,66.66kbps,83.33kbps
            //public byte Timing0;
            //public byte Timing1;
            //public byte CANRX_EN;//保留，未用
            //public byte UARTBAUD;//保留，未用
        }

        //8.定义波特率设置参数类型
        public struct VCI_BAUD_TYPE
        {
            //public UInt32 Baud;				//存储波特率实际值
            //public byte SJW;				//同步跳转宽度，取值1-4
            //public byte BRP;				//预分频值，取值1-64
            //public byte SAM;				//采样点，取值0=采样一次，1=采样三次
            //public byte PHSEG2_SEL;		    //相位缓冲段2选择位，取值0=由相位缓冲段1时间决定,1=可编程
            //public byte PRSEG;				//传播时间段，取值1-8
            //public byte PHSEG1;			    //相位缓冲段1，取值1-8
            //public byte PHSEG2;			    //相位缓冲段2，取值1-8

        }

        //9.定义Reference参数类型
        public struct VCI_REF_STRUCT
        {
            //public VCI_REF_NORMAL RefNormal;
            //public byte Reserved;
            //public VCI_BAUD_TYPE BaudType;
        }

        /*------------数据结构描述完成---------------------------------*/

        public struct CHGDESIPANDPORT
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public byte[] szpwd;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] szdesip;
           // public Int32 desport;

            public void Init()
            {
                szpwd = new byte[10];
                szdesip = new byte[20];
            }
        }

        //定义数据临时存储格式
        public struct data_save_info
        {
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            //public string[] str_info;//= new string[17];//临时存储系统信息
            //public string[] str_vol;//= new string[62];//临时存储单体电芯电压信息
            //public string[] str_tem;//= new string[32];//临时存储单体电芯温度信息
            //public string[] str_equ;//= new string[62];//临时存储单体电芯均衡信息
            //public string[] str_fig;// 临时存储参数信息
            //public string data_sa;
            //public string data_ti;
            //public string show_info;
            //public string show_vol;
            //public string show_tem;
            //public string show_equ;
            //public string show_fig;

        }

        public struct data_save_bcu
        {
            //public string[] str_bcu;
            //public string show_bcu;
            //public string data_ti;
        }

        //定义sa的临时存储格式
        public struct data_save_flag
        {
            //public string flag_sa;
        }
        #endregion   

    }
}
