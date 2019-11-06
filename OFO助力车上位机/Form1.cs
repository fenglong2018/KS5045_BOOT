using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.IO.Ports;
using System.Data.OleDb;
using System.Reflection; // 引用这个才能使用Missing字段 
using System.Runtime.InteropServices;
using System.Diagnostics;




namespace KS5045上位机
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            label7.Text = "C301190821";
        }

        KS5045上位机.disp_list disp = new KS5045上位机.disp_list();

        bool CanRunning = false;
        bool CanSending = false;
        UInt32 add_len = 0;
        volatile bool IsUpdate = false;
        
        byte[] Can_Rev_Buf = new byte[8];
        byte[] info2 = new byte[200000];
        UInt16 FileSize = 0;
        byte state = 0;
        string v1;
        UInt16 data_num = 0;
        UInt16 num_datalen = 0;
        UInt16 APP_CRC16 = 0;
        UInt32 len_num = 0;
        public UInt16 num = 0;
        public UInt16 num_frames = 0;
        public UInt16 version_val;
        UInt32 data_index = 0;
        UInt32 len = 0;
        UInt32 res = new UInt32();
        int write_len = 0;
        int can_rev = 0;
        KS5045上位机.CanManager loadercan;
        KS5045上位机.CanManager.VCI_CAN_OBJ[] ReceiveBuffer = new KS5045上位机.CanManager.VCI_CAN_OBJ[1000];
        int send_cnt = 0;
        UInt32 rx_cnt;
        byte[] flash_data_arr = new byte[100];
        public string extension;
        int rev = 0;
        UInt16 code_addr;
        UInt16 data_len;
        UInt16 data_addr;
        string data = string.Empty;
        string sdata = string.Empty;
        string newLine = string.Empty;

        UInt16 BigPackNums = 0;
        UInt16 SmallPackNums = 0;
        
        byte[] TX_Data_Arr = new byte[4];
        byte[] tx_buffer = new byte[8];

        UInt16[] Crc16Table = new UInt16[256]
         {
            0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50a5, 0x60c6, 0x70e7, 0x8108, 0x9129,
            0xa14a, 0xb16b, 0xc18c, 0xd1ad, 0xe1ce, 0xf1ef, 0x1231, 0x0210, 0x3273, 0x2252,
            0x52b5, 0x4294, 0x72f7, 0x62d6, 0x9339, 0x8318, 0xb37b, 0xa35a, 0xd3bd, 0xc39c,
            0xf3ff, 0xe3de, 0x2462, 0x3443, 0x0420, 0x1401, 0x64e6, 0x74c7, 0x44a4, 0x5485,
            0xa56a, 0xb54b, 0x8528, 0x9509, 0xe5ee, 0xf5cf, 0xc5ac, 0xd58d, 0x3653, 0x2672,
            0x1611, 0x0630, 0x76d7, 0x66f6, 0x5695, 0x46b4, 0xb75b, 0xa77a, 0x9719, 0x8738,
            0xf7df, 0xe7fe, 0xd79d, 0xc7bc, 0x48c4, 0x58e5, 0x6886, 0x78a7, 0x0840, 0x1861,
            0x2802, 0x3823, 0xc9cc, 0xd9ed, 0xe98e, 0xf9af, 0x8948, 0x9969, 0xa90a, 0xb92b,
            0x5af5, 0x4ad4, 0x7ab7, 0x6a96, 0x1a71, 0x0a50, 0x3a33, 0x2a12, 0xdbfd, 0xcbdc,
            0xfbbf, 0xeb9e, 0x9b79, 0x8b58, 0xbb3b, 0xab1a, 0x6ca6, 0x7c87, 0x4ce4, 0x5cc5,
            0x2c22, 0x3c03, 0x0c60, 0x1c41, 0xedae, 0xfd8f, 0xcdec, 0xddcd, 0xad2a, 0xbd0b,
            0x8d68, 0x9d49, 0x7e97, 0x6eb6, 0x5ed5, 0x4ef4, 0x3e13, 0x2e32, 0x1e51, 0x0e70,
            0xff9f, 0xefbe, 0xdfdd, 0xcffc, 0xbf1b, 0xaf3a, 0x9f59, 0x8f78, 0x9188, 0x81a9, 0xb1ca,
            0xa1eb, 0xd10c, 0xc12d, 0xf14e, 0xe16f, 0x1080, 0x00a1, 0x30c2, 0x20e3, 0x5004,
            0x4025, 0x7046, 0x6067, 0x83b9, 0x9398, 0xa3fb, 0xb3da, 0xc33d, 0xd31c, 0xe37f,
            0xf35e, 0x02b1, 0x1290, 0x22f3, 0x32d2, 0x4235, 0x5214, 0x6277, 0x7256, 0xb5ea,
            0xa5cb, 0x95a8, 0x8589, 0xf56e, 0xe54f, 0xd52c, 0xc50d, 0x34e2, 0x24c3, 0x14a0,
            0x0481, 0x7466, 0x6447, 0x5424, 0x4405, 0xa7db, 0xb7fa, 0x8799, 0x97b8, 0xe75f,
            0xf77e, 0xc71d, 0xd73c, 0x26d3, 0x36f2, 0x0691, 0x16b0, 0x6657, 0x7676, 0x4615,
            0x5634, 0xd94c, 0xc96d, 0xf90e, 0xe92f, 0x99c8, 0x89e9, 0xb98a, 0xa9ab, 0x5844,
            0x4865, 0x7806, 0x6827, 0x18c0, 0x08e1, 0x3882, 0x28a3, 0xcb7d, 0xdb5c, 0xeb3f,
            0xfb1e, 0x8bf9, 0x9bd8, 0xabbb, 0xbb9a, 0x4a75, 0x5a54, 0x6a37, 0x7a16, 0x0af1,
            0x1ad0, 0x2ab3, 0x3a92, 0xfd2e, 0xed0f, 0xdd6c, 0xcd4d, 0xbdaa, 0xad8b, 0x9de8,
            0x8dc9, 0x7c26, 0x6c07, 0x5c64, 0x4c45, 0x3ca2, 0x2c83, 0x1ce0, 0x0cc1, 0xef1f,
            0xff3e, 0xcf5d, 0xdf7c, 0xaf9b, 0xbfba, 0x8fd9, 0x9ff8, 0x6e17, 0x7e36, 0x4e55, 0x5e74,
            0x2e93, 0x3eb2, 0x0ed1, 0x1ef0
        };
         UInt16 CRC16_Citt(UInt16 index, UInt32 length, UInt16 crc)
         {
             //UInt32 crc = 0;
             UInt16 ii = 0;
             crc = (UInt16)((UInt16)(crc << 8) ^ (UInt16)Crc16Table[((crc >> 8) ^ info2[index]) & 0x00FF]);
             return (UInt16)(crc & 0xFFFF);
         }
        private void Form1_Load(object sender, EventArgs e)
        {
            buttonOTA.Enabled = false;
            SysStatu.Text = "System state：";
            CommStatus.Text = "Serial port close";
 //           tsslDate.Text = DateTime.Today.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();
            nulllabel.Text = "                                                                 ";
            //try
            //{
            //    foreach (string com in System.IO.Ports.SerialPort.GetPortNames())  //自动获取串行口名称
            //        this.cmPort.Items.Add(com);
            //    cmPort.SelectedIndex = 0;
            //}
            //catch
            //{
            //    CommStatus.Text = "No serial port";
            //}

            loadercan = new KS5045上位机.CanManager();
            loadercan.CanId = KS5045上位机.Consts.BAT_ID;
            //combps.Enabled = false;
        }
        private void OpenPortTool_Click(object sender, EventArgs e)
        {
            try
            {
                //loadercan.init((UInt32)cmPort.SelectedIndex, 8);
                loadercan.init((UInt32)0, 8);//8 = 250k  10 = 500k
                if (loadercan.Open())
                {
                    
                    loadercan.start();
                    can_start();
                }
                else
                {
                    CommStatus.Text = "打开设备失败,请检查设备类型和设备索引号是否正确!";
                    CanRunning = false;
                    //cmPort.Enabled = true;
                    Thread.Sleep(100);
                    loadercan.Close();
                }
            }

            catch (Exception Err)
            {
                loadercan.Close();
                //             TxCmdTimer.Enabled = false;
                CommStatus.Text = "串口出错！                                                                             ";   // 清空状态栏
                MessageBox.Show(Err.Message, "串口打开出错！");

            }
        }

        public void can_start()
        {
            string str_info = "";
            if (CanRunning == false)
            {

                CanRunning = true;
                Thread thread_can = new Thread(new ThreadStart(can_recive));
                thread_can.IsBackground = true;
                thread_can.Start();

                CanSending = true;
                str_info = " CAN2.0：" + //cmPort.Text +
                 " BPS：250Kbps";
                CommStatus.Text = str_info;
                //cmPort.Enabled = false;
                rx_cnt = 0;
            }
            else
            {
                loadercan.Close();
                CanRunning = false;
                CommStatus.Text = "设备：未连接";
                //cmPort.Enabled = true;
            }
        }

        /// <summary>
        /// 线程接收数据函数
        /// </summary>
        /// 
        unsafe private void can_recive()
        {
            UInt32 res_1 = new UInt32();
            UInt32 tmp_id = new UInt32();
            byte cmd;
            byte len;
            try
            {
                while (CanRunning)
                {
                    this.Invoke(new EventHandler(delegate
                    {
                        res = loadercan.GetReceiveNum();
                        res_1 = res;
                    }));
                    if ((res > 0) && (res < 4294967295))
                    {
                        res = loadercan.Receive(ref ReceiveBuffer[0]);
                        UInt32 u32_id = 0;
                        for (UInt32 i = 0; i < res; i++)
                        {
                            u32_id = ReceiveBuffer[i].ID;
                            len = 0;
                            tmp_id = u32_id;
                            cmd = (byte)(u32_id);
                            if (tmp_id == Consts.BAT_ID)
                            {
                                if (ReceiveBuffer[i].RemoteFlag == 0)
                                {
                                    len = (byte)(ReceiveBuffer[i].DataLen % 9);     //?

                                    fixed (KS5045上位机.CanManager.VCI_CAN_OBJ* m_recobj1 = &ReceiveBuffer[i])
                                    {
                                        for (int j = 0; j < 8; j++)
                                        {
                                            Can_Rev_Buf[j] = Convert.ToByte(m_recobj1->Data[j]);
                                        }
                                        can_rev = 1;
                                        rev = 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                CommStatus.Text = e.Message;
                loadercan.Close();
                loadercan.Open();
            }
        }
        /// <summary>
        /// 线程发送函数
        /// </summary>
        /// 
        private void can_send(byte state)
        {
            try
            {
                byte len_DLC;
                //byte[] tx_buffer = new byte[8];
                len_DLC = 8;
                switch (state)
                {
                    case 0x01:
                        len_DLC = 8;
                        tx_buffer[0] = 0x51;        //目标ID
                        tx_buffer[1] = 0x02;        //带相应指令
                        tx_buffer[2] = 0xA3;        //便宜地址
                        tx_buffer[3] = 0x01;        //发送升级触发标志
                        tx_buffer[4] = 0x00;
                        tx_buffer[5] = 0x00;
                        tx_buffer[6] = 0x00;
                        tx_buffer[7] = 0x00;

                        break;

                    case 0x02:

                        len_DLC = 8;
                        tx_buffer[0] = 0x51;        //目标ID
                        tx_buffer[1] = 0x02;        //带相应指令
                        tx_buffer[2] = 0xA3;        //便宜地址
                        tx_buffer[3] = 0x03;        //发送握手成功标志
                        tx_buffer[4] = 0x00;
                        tx_buffer[5] = 0x00;
                        tx_buffer[6] = 0x00;
                        tx_buffer[7] = 0x00;

                        break;
                    case 0x03:                  //发校验码
                        len_DLC = 8;
                        tx_buffer[0] = 0x51;        //目标ID
                        tx_buffer[1] = 0x03;        //带相应指令
                        tx_buffer[2] = 0xA5;        //CRC地址

                        tx_buffer[3] = (byte)(APP_CRC16 &0xFF);         //发送校验码低位
                        tx_buffer[4] = (byte)(APP_CRC16 >> 8);        //发送校验码高位                       

                        tx_buffer[5] = 0x00;
                        tx_buffer[6] = 0x00;
                        tx_buffer[7] = 0x00;
                        break;

                    case 0x04:                   //写05

                        len_DLC = 8;
                        tx_buffer[0] = 0x51;        //目标ID
                        tx_buffer[1] = 0x03;        //带相应指令
                        tx_buffer[2] = 0xA3;        //便宜地址
                        tx_buffer[3] = 0x05;        //发送包参数标志
                        tx_buffer[4] = 0x00;
                        tx_buffer[5] = 0x00;
                        tx_buffer[6] = 0x00;
                        tx_buffer[7] = 0x00;

                        break;

                    case 0x05:                  //写大包

                        len_DLC = 8;
                        tx_buffer[0] = 0x51;        //目标ID
                        tx_buffer[1] = 0x03;        //带相应指令
                        tx_buffer[2] = 0xA6;        //UPDATE_PACSIZE_REG地址

                        tx_buffer[3] = (byte)(BigPackNums&0xFF);        //发送大包数量低位 
                        tx_buffer[4] = (byte)(BigPackNums>>8);        //发送大包数量高位 

                        tx_buffer[5] = 0x00;
                        tx_buffer[6] = 0x00;
                        tx_buffer[7] = 0x00;

                        break;


                    case 0x06:              //写数据

                   //     TX_Data_Arr[k];
                        //len_DLC = 8;
                        //tx_buffer[0] = 0x51;        //目标ID
                        //tx_buffer[1] = 0x03;        //带相应指令
                        //tx_buffer[2] = 0xA4;        //UPDATE_PACSIZE_REG地址

                        //tx_buffer[3] = 0;  
                        
                        //tx_buffer[4] = TX_Data_Arr[0]; 
                        //tx_buffer[5] = TX_Data_Arr[1];
                        //tx_buffer[6] = TX_Data_Arr[2];
                        //tx_buffer[7] = TX_Data_Arr[3];

                        break;

                    case 0x07:              //写小包数




                        break;



                    default:

                        break;
                    
                }



                loadercan.StandardWrite(tx_buffer, len_DLC, state);
                CanSending = false;
            }
            catch (Exception e)
            {
                CommStatus.Text = e.Message;
                loadercan.Close();
                loadercan.Open();
            }
        }

        private void ClosePortTool_Click(object sender, EventArgs e)
        {
            CanRunning = false;
            CommStatus.Text = "串口未打开";    // 清空状态栏
            //cmPort.Enabled = true;            // 使能选择串口  
            loadercan.Close();
        }

        private void ReseachTool_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    cmPort.Items.Clear();
            //    foreach (string com in System.IO.Ports.SerialPort.GetPortNames())  //自动获取串行口名称
            //        this.cmPort.Items.Add(com);
            //    cmPort.SelectedIndex = 0;
            //}
            //catch
            //{
            //    CommStatus.Text = "未找到串口";
            //}
        }

        private void ReadFile_Click(object sender, EventArgs e)
        {
            UInt16 crc16;
            string hex_info_str = string.Empty;
            string version_str = string.Empty;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "选择文件";
            fileDialog.Filter = "hex files (*.hex)|*.hex";
            //fileDialog.Filter = "All files (*.*)|*.*|hex files (*.hex)|*.hex|bin files (*.bin)|*.bin";
            fileDialog.FilterIndex = 3;      //1表示对话框默认选择Filter里面的第一种文件类型，2表示第二种，以此类推
            fileDialog.RestoreDirectory = false;   //true表示下次选择文件时仍从初始路径读取文件，false则从上次打开文件的位置开始
            if (fileDialog.ShowDialog() == DialogResult.OK)  //dialog open ?
            {
                String fileName = fileDialog.FileName;
                version_str = Path.GetFileNameWithoutExtension(fileName);
                extension = Path.GetExtension(fileName);
                string sLine = string.Empty;
                listBox1.Items.Clear();
                if (extension == ".hex")
                { 
                    int time = 0;
                    int time2 = 0;
                    int time3 = 0;
                    int state = 0;
                    int x = 0;
                    sdata = "";
                    FileSize = 0;
                    StreamReader objReader = new StreamReader(fileName, System.Text.Encoding.Default);//读取文本的时候要指定编码格式,才不会出现乱码。Default为当前系统的编码格式



                    while (sLine != null)       //一直读到文件结束
                    {



                        if (state == 0)
                        {
                            sLine = objReader.ReadLine();   //读完指针自动指向下一行，直到文件的末尾。
                            if (time3 == 0)//去掉一行
                            {
                                if (sLine.Substring(7, 2) != "00")
                                {
                                    sLine = objReader.ReadLine();
                                }
                                time3++;
                            }
                        }
                        else
                        {
                            state = 0;
                        }
                        if (sLine != null)
                        {
                            // 数据区
                            if (sLine.Substring(7, 2) == "00")
                            {

                                if (time2 == 0)
                                {
                                    code_addr = Convert.ToUInt16(sLine.Substring(1, 2), 16);
                                    int b = code_addr;
                                    newLine = sLine.Substring(9, (b * 2));
                                    data_len = Convert.ToUInt16(sLine.Substring(1, 2), 16);
                                    data_addr = Convert.ToUInt16(sLine.Substring(3, 4), 16);
                                    sLine = newLine;
                                    sdata += newLine;
                                    string sLine2 = objReader.ReadLine();
                                    UInt16 data_addr2 = Convert.ToUInt16(sLine2.Substring(3, 4), 16);

                                    int d = data_addr2 - data_len;



                                    for (int i = 0; i < d; i++)     //在一行不足16个字节的地方补0xFF
                                    {
                                        newLine = "FF";
                                        sdata += newLine;
                                    }
                                    state++;
                                    time2++;
                                    sLine = sLine2;
                                }
                                else
                                {
                                    if (time == 0)
                                    {
                                        code_addr = Convert.ToUInt16(sLine.Substring(1, 2), 16);
                                        int b = code_addr;
                                        newLine = sLine.Substring(9, (b * 2));
                                        data_len = Convert.ToUInt16(sLine.Substring(1, 2), 16);
                                        data_addr = Convert.ToUInt16(sLine.Substring(3, 4), 16);
                                        num_datalen = data_addr;
                                        //sLine = newLine;
                                        sdata += newLine;
                                        time++;
                                    }
                                    else
                                    {
                                        UInt16 t = (UInt16)(data_len + data_addr);
                                        string str = Convert.ToString(t, 16).ToUpper();
                                        switch (str.Length)
                                        {
                                            case 0x01:
                                                str = "000" + str;
                                                break;
                                            case 0x02:
                                                str = "00" + str;
                                                break;
                                            case 0x03:
                                                str = "0" + str;
                                                break;
                                            default:
                                                break;
                                        }
                                        if (str == sLine.Substring(3, 4))
                                        {
                                            if (sLine.Substring(1, 2) == "10")
                                            {
                                                code_addr = Convert.ToUInt16(sLine.Substring(1, 2), 16);
                                                int c = code_addr;
                                                newLine = sLine.Substring(9, (c * 2));
                                                data_len = Convert.ToUInt16(sLine.Substring(1, 2), 16);
                                                data_addr = Convert.ToUInt16(sLine.Substring(3, 4), 16);
                                                //sLine = newLine;
                                                sdata += newLine;
                                            }
                                            else
                                            {
                                                code_addr = Convert.ToUInt16(sLine.Substring(1, 2), 16);
                                                int c = code_addr;
                                                newLine = sLine.Substring(9, (c * 2));
                                                data_len = Convert.ToUInt16(sLine.Substring(1, 2), 16);
                                                data_addr = Convert.ToUInt16(sLine.Substring(3, 4), 16);
                                                //sLine = newLine;
                                                sdata += newLine;

                                                string sLine2 = objReader.ReadLine();
                                                if (sLine2.Substring(7, 2) == "00")
                                                {
                                                    UInt16 data_len2 = Convert.ToUInt16(sLine2.Substring(1, 2), 16);
                                                    UInt16 data_addr2 = Convert.ToUInt16(sLine2.Substring(3, 4), 16);
                                                    int d = data_addr2 - data_addr - data_len;
                                                    if ((data_addr2 - data_addr) > 16)
                                                    {
                                                        state = 1;
                                                        sLine = sLine2;
                                                    }
                                                    else
                                                    {
                                                        for (int i = 0; i < d; i++)
                                                        {
                                                            newLine = "FF";
                                                            sdata += newLine;
                                                        }
                                                        newLine = sLine2.Substring(9, (data_len2 * 2));
                                                        
                                                        int f = data_len + data_len2 + d;
                                                        sdata += newLine;
                                                        
                                                        for (int i = 0; i < (16 - f); i++)
                                                        {
                                                            newLine = "FF";
                                                            sdata += newLine;
                                                            x = 1;

                                                        }
                                                        if (data_len2 < 16)
                                                        {
                                                            if (x == 1)
                                                            {
                                                                data_len = 16;
                                                            }
                                                            else
                                                            {
                                                                data_addr = data_addr2;
                                                                data_len = data_len2;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            data_len = data_len2;
                                                            data_addr = data_addr2;
                                                        }
                                                        
                                                    }
                                                }
                                                else
                                                {
                                                    for (int i = 0; i < ((7000 - data_addr) - data_len /*- num_datalen*/); i++)
                                                    {
                                                        newLine = "FF";
                                                        sdata += newLine;
                                                    }
                                                    sLine = null;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            string strt = str;
                                            while (strt != sLine.Substring(3, 4))
                                            {

                                                newLine = "FF";
                                                sdata += newLine;
                                                data_len++;
                                                UInt16 tt = (UInt16)(data_len + data_addr);
                                                strt = Convert.ToString(tt, 16).ToUpper();
                                                switch (strt.Length)
                                                {
                                                    case 0x01:
                                                        strt = "000" + strt;
                                                        break;
                                                    case 0x02:
                                                        strt = "00" + strt;
                                                        break;
                                                    case 0x03:
                                                        strt = "0" + strt;
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                            state = 1;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    int a = 0;
                    int m = 0;
                    UInt32 index = 0, lenn = 0;

                    FileStream fs = new FileStream("KS5045.txt", FileMode.Create);
                    ////获得字节数组
                    //byte[] Txtdata = System.Text.Encoding.Default.GetBytes("Hello World!");
                    ////开始写入
                    //fs.Write(Txtdata, 0, Txtdata.Length);
                    ////清空缓冲区、关闭流
                    //fs.Flush();
                    //fs.Close();


                    for (int j = 0; j < sdata.Length / 2; j++)
                    {
                        lenn = 0;
                        string text = sdata.Substring(((int)(index + lenn)), 1);
                        lenn++;
                        text += sdata.Substring(((int)(index + lenn)), 1);
                        lenn++;
                        a++;
                        info2[j] = Convert.ToByte(text, 16);
                        index += lenn;
                        data += text;
                        m++;
                        if (m == 16)
                        {
                            listBox1.Items.Add(data);
                            data += "\r\n";
                            byte[] Txtdata = System.Text.Encoding.Default.GetBytes(data);                            

                            fs.Write(Txtdata, 0, Txtdata.Length);
                            
                            data = null;
                            m = 0;
                        }
                        if (sdata.Length == index)
                        {
                            break;
                        }
                    }
                    //清空缓冲区、关闭流
                    fs.Flush();
                    fs.Close();

                    APP_CRC16 = 0;
                    for (int cr = 0; cr < (sdata.Length / 2); cr++)
                    {
                        APP_CRC16 = CRC16_Citt((UInt16)cr, (UInt16)1, APP_CRC16);
                    }
                    FileSize += (UInt16)a;

                    objReader.Close();
                    UInt32 num_frame = (UInt32)(FileSize / 7);
                    if ((FileSize % 7) > 0)
                    {
                        num_frame = (UInt32)(num_frame + 1);
                    }
                    label8.Text = num_frame.ToString();
                }
                else
                {
                    System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open);
                    System.IO.BinaryReader read = new System.IO.BinaryReader(fs);
                    int index = 0;
                    APP_CRC16 = 0;
                    while (fs.Length > index)
                    {
                        int lenght = (int)fs.Length - index < 16 ? (int)fs.Length - index : 16;
                        byte[] buff = read.ReadBytes(lenght);
                        byte[] buff2 = new byte[2];
                        for (int j = 0; j < buff.Length; j++)
                        {
                            buff2[0] = buff[j];
                            //APP_CRC16 = CRC16_Citt(buff2, 1, APP_CRC16);
                        }
                        if (lenght == 0x10)
                        {
                            for (int i = 0; i < lenght; i++)
                            {
                                if (buff[i] < 0x10)
                                {
                                    sLine += "0" + buff[i].ToString("X");
                                }
                                else
                                {
                                    sLine += buff[i].ToString("X");
                                }
                            }
                        }
                        else
                        {
                            if (lenght < 0x10)
                            {
                                for (int i = 0; i < lenght; i++)
                                {
                                    if (buff[i] < 0x10)
                                    {
                                        sLine += "0" + buff[i].ToString("X");
                                    }
                                    else
                                    {
                                        sLine += buff[i].ToString("X");
                                    }
                                }
                                for (int i = 0; i < (16 - lenght); i++)
                                {
                                    sLine += "FF";
                                    add_len++;
                                }
                            }
                        }



                        listBox1.Items.Add(sLine);
                        sLine = string.Empty;
                        index += lenght;
                        FileSize += (UInt16)lenght;
                        FileSize = (UInt16)(FileSize / 2);
                    }
                    UInt32 num_frame = (UInt32)(FileSize/7);
                    if ((FileSize % 7) > 0)
                    {
                        num_frame = (UInt32)(num_frame + 1);
                    }
                    label8.Text = num_frame.ToString();
                }
                if (FileSize > 65500)
                {
                    MessageBox.Show("文件过大");
                }
                label9.Text = Convert.ToString(FileSize);
                hex_info_str = Convert.ToString(APP_CRC16);
                label10.Text = APP_CRC16.ToString("X");
                buttonOTA.Enabled = true;
                //new 12/14
               // listBox1.SelectedIndex = 0;    //  0表示选中第一行，1表示选中第二行，以此类推

                //v1 = version_str.Substring(19, 3);
                //string v2 = version_str.Substring(0, 6);
                //if (v2 == "KS5045")
                //{
                //    label1.Text = "KS5045";
                //}
                //else
                //{
                //    MessageBox.Show("待升级产品非KS5045电池产品");
                //    return;
                //}
                //string v5 = version_str.Substring(5, 4);
                //if (v5 == "SCUD")
                //{
                //    label3.Text = "飞毛腿";
                //}
                //else
                //{
                //    MessageBox.Show("待升级文件不是飞毛腿文件");
                //    return;
                //}
                //string v6 = version_str.Substring(10, 3);
                //label2.Text = v6;
                //string v3 = version_str.Substring(14, 3);
                //label5.Text = v3;
                //string v4 = version_str.Substring(18, 4);
                //label23.Text = v4;
                //label4.Text = v1;
                //version_val = Convert.ToUInt16(v1);

                //end new
            }
        }
       
        private void OTA_Click(object sender, EventArgs e)
        {
            if (loadercan.Open() == false)
            { MessageBox.Show("串口还未打开"); return; }
            this.label6.Text = Convert.ToString("升级中");
            OpenPortTool.Enabled = false;
           // Max_tx_retry_cnt = 0;
            this.FrameCnt.Text = Convert.ToString(0);//
            buttonOTA.Enabled = false;
            IsUpdate = true;
            can_rev = 0;
            Thread R2 = new Thread(DevDataProg);  //creat a new thread 然后实例化成某个函数
            R2.IsBackground = true;    //后台运行，主线程可以继续执行。一般都要开，不然主线程有些东西可能不能执行。
            R2.Start();
        }

        private void DevDataProg()
        {
            UInt16 i, j;
            byte tx_len = 0;
            string tmp_str = string.Empty;
            int select_index = 0;
            byte tx_retry = 3;
            bool WR_rec_flag = true;
            int ii = 0;

            UInt16 Local_num = 0;
            byte len_DLC;
            byte ReSent_Num = 0;

            while (IsUpdate)
            {
                write_len = 0;

                //计算大包字节数，

                BigPackNums = (UInt16)((sdata.Length / 2) / Consts.Per_Pack_num);       //计算大包数量
                if (((sdata.Length / 2) % Consts.Per_Pack_num) != 0)
                {
                    BigPackNums++;      //有余数，说明打包还有一包未满Consts.Per_Pack_num
                }


                if (HandShake() == 1)//握手成功
                {
                    this.label6.Text = Convert.ToString("握手成功！");
                    Thread.Sleep(10);
                    if (extension == ".hex")
                    {
                        while (IsUpdate)
                        {

                            for(i=0;i< BigPackNums;i++)     //大包
                            {
                                if(i == BigPackNums-1)
                                {
                                    SmallPackNums = (UInt16)((sdata.Length / 2) % Consts.Per_Pack_num);
                                }
                                else
                                {
                                    SmallPackNums = Consts.Per_Pack_num;
                                }
                                for(j=0;j< SmallPackNums/4;j++)   //小包暂定192， 一次发4字节
                                {

                                    for (byte k=0; k<4; k++)
                                    {
                                        this.label6.Text = Convert.ToString("升级中");
                                        string text = sdata.Substring(Local_num, 2);
                                        Local_num += 2;
                                        TX_Data_Arr[k] = Convert.ToByte(text, 2);           //取一个字节
                                    }

                                    //发送一个小包

                                    len_DLC = 8;
                                    tx_buffer[0] = 0x51;        //目标ID
                                    tx_buffer[1] = 0x03;        //带相应指令
                                    tx_buffer[2] = 0xA4;        //UPDATE_PACSIZE_REG地址

                                    tx_buffer[3] = (byte)j;

                                    tx_buffer[4] = TX_Data_Arr[0];
                                    tx_buffer[5] = TX_Data_Arr[1];
                                    tx_buffer[6] = TX_Data_Arr[2];
                                    tx_buffer[7] = TX_Data_Arr[3];

                                    state = 6;
                                    loadercan.StandardWrite(tx_buffer, len_DLC, state);
                                    CanSending = false;

                                    System.Threading.Thread.Sleep(20);

                                    byte feedback = RecCheck();
                                    if (feedback  !=  0)      //有信息反馈回来
                                    {
                                        switch (feedback)
                                        {
                                            case 0x06:          //短包计数不一致，重新发送该长包

                                                j--;
                                                ReSent_Num++;
                                                this.label6.Text ="第" + Convert.ToString(j) +  "大包" + "第" + Convert.ToString(ReSent_Num) + "次重新发送!" ;

                                                if (ReSent_Num>3)
                                                {

                                                    //发送一个小包

                                                    len_DLC = 8;
                                                    tx_buffer[0] = 0x51;        //目标ID
                                                    tx_buffer[1] = 0x03;        //带相应指令
                                                    tx_buffer[2] = 0xA3;        //UPDATE_PACSIZE_REG地址
                                                    tx_buffer[3] = 0x07;        //通知从机通信失败，需要重启
                                                    tx_buffer[4] = 0x00;
                                                    tx_buffer[5] = 0x00;
                                                    tx_buffer[6] = 0x00;
                                                    tx_buffer[7] = 0x00;

                                                    state = 6;
                                                    loadercan.StandardWrite(tx_buffer, len_DLC, state);
                                                    CanSending = false;

                                                    //处理相应数据，退出烧录
                                                    i = BigPackNums;
                                                    j = (UInt16)(SmallPackNums / 4);
                                                    IsUpdate = false;
                                                    MessageBox.Show("升级失败！需重新启动！");
                                                }

                                                break;

                                            case 0x07:          //重新发送超过3次，进入下一次重启

                                                //处理相应数据，退出烧录
                                                i = BigPackNums;
                                                j = (UInt16)(SmallPackNums / 4);
                                                IsUpdate = false;
                                                MessageBox.Show("升级失败！需重新启动！");
                                                break;

                                            case 0x09:          //校验成功

                                                //处理相应数据，退出烧录
                                                i = BigPackNums;
                                                j = (UInt16)(SmallPackNums / 4);
                                                IsUpdate = false;

                                                MessageBox.Show("升级完成！");

                                                break;

                                            case 0x0A:

                                                //处理相应数据，退出烧录
                                                i = BigPackNums;
                                                j = (UInt16)(SmallPackNums / 4);
                                                IsUpdate = false;
                                                MessageBox.Show("校验失败！需重新启动！");

                                                break;
                                        }
                                    }
                                }
                            }





                            len = 0;
                            for (i = 0; i < 3; i++)
                            {
                                if (sdata.Length == data_index + len)
                                {
                                    len_num = data_index + len;
                                    tx_len += (byte)len;
                                    break;
                                }
                                string text = sdata.Substring(((int)(data_index + len)), 1);
                                len++;
                                text += sdata.Substring(((int)(data_index + len)), 1);
                                len++;
                                flash_data_arr[i] = Convert.ToByte(text, 16);
                                tx_len++;
                            }

                            write_len++;
                            data_num++;
                            if (data_num > 15)
                            {
                                data_num = 1;
                            }
                            FrameCnt.Invoke(new EventHandler(delegate        //委托，跨线程调用控件或函数时使用。
                            {
                                this.FrameCnt.Text = Convert.ToString(write_len);
                            }));
                            WR_rec_flag = true;

                            listBox1.Invoke(new EventHandler(delegate        //委托，跨线程调用控件或函数时使用。
                            {

                                listBox1.SelectedIndex = select_index;    //  0表示选中第一行，1表示选中第二行，以此类推

                                progressBar1.Value = (select_index * 100) / listBox1.Items.Count;
                            }));
                            if (listBox1.Items.Count > (select_index + 6))
                            {
                                select_index = (int)data_index / 32;
                            }
                            else
                            {
                                select_index = listBox1.Items.Count - 1;
                            }
                            //发送数据
                            while (true)
                            {
                                can_rev = 0;
                                state = 3;
                                can_send(state);
                                Thread.Sleep(10);
                                if (can_rev == 0)
                                {
                                    System.Threading.Thread.Sleep(10);//缩短时间 2018_12_20  50——25
                                    if (can_rev == 0)
                                    {
                                        System.Threading.Thread.Sleep(10);//缩短时间 2018_12_20  50——25
                                        if (can_rev == 0)
                                        {
                                            System.Threading.Thread.Sleep(10);//缩短时间 2018_12_20  50——25
                                            if (can_rev == 0)
                                            {
                                                System.Threading.Thread.Sleep(50);//缩短时间 2018_12_20  50——25
                                                if (can_rev == 0)
                                                {
                                                    WR_rec_flag = false;
                                                }
                                            }
                                        }
                                    }
                                }
                                if (WR_rec_flag == true)
                                {
                                    if (IsAppWriteRespondOk() == 0)//数据返回确认
                                    {
                                        WR_rec_flag = true;
                                        break;
                                    }
                                }
                                else
                                {
                                    WR_rec_flag = true;
                                }
                                can_rev = 0;
                                if (tx_retry > 0)
                                {
                                    tx_retry--;
                                }
                                else
                                {
                                    IsUpdate = false;
                                    MessageBox.Show("第" + FrameCnt.Text.ToString() + "帧写入失败!");
                                    buttonOTA.Invoke(new EventHandler(delegate        //委托，跨线程调用控件或函数时使用。
                                    {
                                        buttonOTA.Enabled = true;
                                    }));
                                    data_index = 0;
                                    state = 0;
                                    send_cnt = 0;
                                    return;
                                }
                            }
                            tx_retry = 3;
                            tx_len = 0;
                            if (sdata.Length == data_index)
                            {
                                break;
                            }
                        }
                        for (i = 0; i < 3;i++ )
                        {
                            rev = 0;
                            state = 4;
                            can_send(state);
                            System.Threading.Thread.Sleep(5000);
                            if (rev == 1)
                            {
                                if (ISreturnok() == 0)
                                {
                                    break;
                                }
                                else
                                {
                                    buttonOTA.Invoke(new EventHandler(delegate        //委托，跨线程调用控件或函数时使用。
                                    {
                                        buttonOTA.Enabled = true;
                                    }));
                                    state = 0;
                                    data_index = 0;
                                    send_cnt = 0;
                                }
                            }
                            Thread.Sleep(50);
                        }
                        if (i == 3)
                        {
                            IsUpdate = false;
                            MessageBox.Show("文件结束包没有回复！");
                            buttonOTA.Invoke(new EventHandler(delegate        //委托，跨线程调用控件或函数时使用。
                            {
                                buttonOTA.Enabled = true;
                            }));
                            data_index = 0;
                            state = 0;
                            send_cnt = 0;
                            return;
                        }
                        
                        //校验升级结束
                        for (i = 0; i < 3; i++)
                        {
                            rev = 0;
                            state = 5;
                            can_send(state);
                            System.Threading.Thread.Sleep(2000);
                            if (rev == 1)
                            {
                                if (IsFlashWriteSuccess() == 0)
                                {
                                    MessageBox.Show("成功，升级完成！");
                                    IsUpdate = false;
                                    buttonOTA.Invoke(new EventHandler(delegate        //委托，跨线程调用控件或函数时使用。
                                    {
                                        buttonOTA.Enabled = true;
                                    }));
                                    data_index = 0;
                                    state = 0;
                                    send_cnt = 0;
                                    return;
                                }
                            }
                        }
                        if (i == 3)
                        {
                            IsUpdate = false;
                            MessageBox.Show("确认升级失败");
                            buttonOTA.Invoke(new EventHandler(delegate        //委托，跨线程调用控件或函数时使用。
                            {
                                buttonOTA.Enabled = true;
                            }));
                            state = 0;
                            data_index = 0;
                            send_cnt = 0;
                            return;
                        }
                    }
                }
                else
                {
                    buttonOTA.Invoke(new EventHandler(delegate        //委托，跨线程调用控件或函数时使用。
                    {
                        buttonOTA.Enabled = true;
                    }));
                    state = 0;
                    data_index = 0;
                    send_cnt = 0;
                    //IsUpdate = false;
                    //break;
                }
                
            }
            state = 0;
            buttonOTA.Invoke(new EventHandler(delegate        //委托，跨线程调用控件或函数时使用。
            {
                buttonOTA.Enabled = true;
            }));
        }


        private byte RecCheck()
        {

            if (can_rev == 1)           //检测是否有收到反馈的异常等信息
            {
                if ((Can_Rev_Buf[0] == 0x00) && (Can_Rev_Buf[1] == 0x04) && (Can_Rev_Buf[2] == 0xA3) && (Can_Rev_Buf[3] == 0x06))
                {
                    can_rev = 0;
                    return 6;        //接收短包计数不一致，请求重新发送长包     
                }
                else if ((Can_Rev_Buf[0] == 0x00) && (Can_Rev_Buf[1] == 0x04) && (Can_Rev_Buf[2] == 0xA3) && (Can_Rev_Buf[3] == 0x07))
                {
                    can_rev = 0;
                    return 7;        //重新发送长包超过3次，通信失败，需要重启     
                }
                else if ((Can_Rev_Buf[0] == 0x00) && (Can_Rev_Buf[1] == 0x04) && (Can_Rev_Buf[2] == 0xA3) && (Can_Rev_Buf[3] == 0x09))
                {
                    can_rev = 0;
                    return 9;        //校验成功     
                }
                else if ((Can_Rev_Buf[0] == 0x00) && (Can_Rev_Buf[1] == 0x04) && (Can_Rev_Buf[2] == 0xA3) && (Can_Rev_Buf[3] == 0x0A))
                {
                    can_rev = 0;
                    return 10;        //校验失败   
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }            
        }



        private byte HandShake()
        {
            byte i = 0;
            //bool handshake_flag = false;
            //bool handshake_receive = true;
            //bool handshake_flag_cnt = false;
            //bool handshake_receive_cnt = true;
            //byte cnt = 1;
            //byte output_return = 0;
            if (state == 0)
            {
                for (i = 0; i < 1; i++)         //握手5次？
                {
                    state = 1;
                    can_rev = 0;
                    can_send(state);                //发送启动升级标志

                    timer1.Enabled = true;          //需在timer1 定时中断处理超时

                    System.Threading.Thread.Sleep(10);
                    if (can_rev == 0)
                    {
                        System.Threading.Thread.Sleep(10);
                        if (can_rev == 0)
                        {
                            System.Threading.Thread.Sleep(10);
                            if (can_rev == 0)
                            {
                                System.Threading.Thread.Sleep(50);
                                if (can_rev == 0)
                                {
                                    //output_return = 0;
                                    return 0;
                                }
                            }
                        }
                    }

                    if (can_rev == 1)
                    {
                        if ((Can_Rev_Buf[0] == 0x00) && (Can_Rev_Buf[1] == 0x04) && (Can_Rev_Buf[2] == 0xA3) && (Can_Rev_Buf[3] == 0x02))
                        {
                            timer1.Interval = 0;
                            state = 2;
                            can_rev = 0;
                            can_send(state);            //发送握手成功标志                
                        }
                        else
                        {
                            return 0;
                        }
                    }

                    System.Threading.Thread.Sleep(10);
                    if (can_rev == 0)
                    {
                        System.Threading.Thread.Sleep(10);
                        if (can_rev == 0)
                        {
                            System.Threading.Thread.Sleep(10);
                            if (can_rev == 0)
                            {
                                System.Threading.Thread.Sleep(50);
                                if (can_rev == 0)
                                {
                                    return 0;
                                }
                            }
                        }
                    }

                    if (can_rev == 1)
                    {
                        if ((Can_Rev_Buf[0] == 0x00) && (Can_Rev_Buf[1] == 0x04) && (Can_Rev_Buf[2] == 0xA3) && (Can_Rev_Buf[3] == 0x04))
                        {
                            timer1.Interval = 0;
                            state = 3;
                            can_rev = 0;
                            can_send(state);            //发送校验码

                            System.Threading.Thread.Sleep(50);

                            timer1.Interval = 0;
                            state = 4;
                            can_rev = 0;
                            can_send(state);            //发送05

                            System.Threading.Thread.Sleep(50);

                            timer1.Interval = 0;
                            state = 5;
                            can_rev = 0;
                            can_send(state);            //发送写大包字节数

                            System.Threading.Thread.Sleep(50);

                            //可以开始发送小包



                            timer1.Enabled = false;          //需在timer1 定时中断处理超时
                            return 1;


                        }
                    }
                }
            }
            return 0;
        }
        private UInt16 IsAppHandRespondOk(int respon)
        {
            if (rev == 1)
            {
                rev = 0;
                if (respon == 0)
                {
                    switch (Can_Rev_Buf[0])
                    {
                        case 0x00:
                            return 0;
                        case 0x01:
                            MessageBox.Show(state.ToString() + "错误代码：" + Can_Rev_Buf[0].ToString() + "   " + "电池在充电!");
                            break;
                        case 0x02:
                            MessageBox.Show(state.ToString() + "错误代码：" + Can_Rev_Buf[0].ToString() + "   " + "SOC小于20%!");
                            break;
                        case 0x03:
                            MessageBox.Show(state.ToString() + "错误代码：" + Can_Rev_Buf[0].ToString() + "   " + "版本号小于现版本号!");
                            break;
                        case 0x04:
                            MessageBox.Show(state.ToString() + "错误代码：" + Can_Rev_Buf[0].ToString() + "   " + "正在升级!");
                            break;
                        case 0x05:
                            MessageBox.Show(state.ToString() + "错误代码：" + Can_Rev_Buf[0].ToString() + "   " + "未知错误!");
                            break;
                        default:
                            break;
                    }
                    return 1;
                }
                else
                {
                    if (Can_Rev_Buf[0] == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        MessageBox.Show(state.ToString() + "   " + "长度异常!");
                        return 1;
                    }
                }
            }
            return 1;
        }
        private byte IsAppWriteRespondOk()
        {
            if (rev == 1)
            {
                rev = 0;
                if (Can_Rev_Buf[0] == 0 || Can_Rev_Buf[0] == 1)
                {
                    if (Can_Rev_Buf[0] == 1)
                    {
                        write_len -= 1;
                        data_num -= 1;
                        return 1;
                    }
                    if (sdata.Length == data_index)
                    {
                    }
                    else
                    {
                        if (send_cnt == 273)
                        {
                            int a = 0;
                        }
                        data_index = (UInt16)((send_cnt * 15) * len + (Can_Rev_Buf[1] * len));
                        // data_index += len;//Can_Rev_Buf[1] * len;
                        if (Can_Rev_Buf[1] == 0x0F)
                        {
                            send_cnt += 1;
                        }
                    }
                    //data_index += len;
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            return 1;
        }
        private byte ISreturnok()
        {
            int i,j;
            if (Can_Rev_Buf[0] == 0)
            {
                return 0;
            }
            else
            {
                j = Can_Rev_Buf[2];
                j <<= 8;
                j |= Can_Rev_Buf[3];
                i = (byte)(APP_CRC16 >> 8);
                i <<= 8;
                i = (byte)(APP_CRC16 & 0xff); 
                MessageBox.Show(state.ToString() + ":" + j  +  ";" + i + "   "+ "数据校验错误!");

                return 1;
            }
        }
        private byte IsFlashWriteSuccess()
        {
            rev = 0;
            switch (Can_Rev_Buf[0])
            {
                case 0x00:
                    return 0;
                case 0x01:
                    MessageBox.Show(state.ToString() + "   " + "数据校验错误!");
                    break;
                case 0x02:
                    MessageBox.Show(state.ToString() + "   " + "版本错误错误!");
                    break;
                case 0x03:
                    MessageBox.Show(state.ToString() + "   " + "长度错误!");
                    break;
                case 0x04:
                    MessageBox.Show(state.ToString() + "   " + "升级写过程出错，无法继续升级!");
                    break;
            }
            return 1;
        }

    }
}
