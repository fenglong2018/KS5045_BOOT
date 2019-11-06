using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KC_CAN_MONI
{
    class UpFrame
    {

        Byte[] data_send = new Byte[300];
        byte lenAdd = 0;//总长度为数据长度添加附加长度

        public byte[] Frame
        {
            get
            {
                return data_send;
            }
            set
            {
                data_send = value;
            }
        }
        public int tx_app_jump()
        {
            data_send[0] = (byte)(0xA1);
            data_send[1] = (byte)255;
            data_send[2] = 0xAA;
            data_send[3] = 0;
            data_send[4] = 0;
            data_send[5] = 0;
            data_send[6] = 0;
            data_send[7] = 0;
            lenAdd = 8;
            return 0;
        }
        //0x01
        public int tx_handshake()
        {
            byte crc;
            lenAdd = (byte)(0 + 6);
            data_send[0] = 0x55;
            data_send[1] = 0x00;
            data_send[2] = 0x00;
            data_send[3] = 0;
            data_send[4] = 0x01;
            crc = CrcSum8(data_send, (UInt16)(lenAdd - 2), 3);
            data_send[lenAdd - 1] = (byte)crc;

            return 0;
        }
        //0X03
        public int tx_file_data(UInt32 address, Byte[] write_buffer, Byte len)
        {
            byte crc;
            data_send[0] = 0x55;
            data_send[1] = 0x00;
            data_send[2] = 0x00;
            lenAdd = (byte)(len + 10);
            data_send[3] = (byte)(len + 4);
            data_send[4] = 0x03;
            data_send[5] = (Byte)(address >> 24);
            data_send[6] = (Byte)(address >> 16);
            data_send[7] = (Byte)(address >> 8);
            data_send[8] = (Byte)(address);

            for (int i = 0; i < len; i++)
            {
                data_send[i + 9] = write_buffer[i];// mirrirDate(write_buffer[i]);//数据待做镜面处理
            }
            crc = CrcSum8(data_send, (UInt16)(lenAdd - 2), 3);
            data_send[lenAdd - 1] = (byte)crc;

            return 0;
        }
        //0x05
        public int tx_file_crc(UInt32 address,UInt16 DataCrc16)
        {
            byte crc;
            data_send[0] = 0x55;
            data_send[1] = 0x00;
            data_send[2] = 0x00;
            lenAdd = (byte)(2 + 10);
            data_send[3] = (byte)(2 + 4);
            data_send[4] = 0x05;
            data_send[5] = (Byte)(address >> 24);//换成总长度了
            data_send[6] = (Byte)(address >> 16);
            data_send[7] = (Byte)(address >> 8);
            data_send[8] = (Byte)(address);
            data_send[9] = (byte)(DataCrc16 >> 8);
            data_send[10] = (byte)(DataCrc16);
            crc = CrcSum8(data_send, (UInt16)(lenAdd - 2), 3);
            data_send[lenAdd - 1] = (byte)crc;

            return 0;
        }

        public byte length()
        {
            return lenAdd;
        }


        public byte CrcSum8(byte[] data_arr, UInt32 len, UInt32 starLen)      //ptr为数据指针，len为数组长度（传输字节数）
        {

            byte CRC_Value = 0;                   //每次校验前都需要清零

            for (; starLen <= len; starLen++)
            {
                CRC_Value += data_arr[starLen];
            }
            
            return ((byte)(0x100 - CRC_Value));
        }
        private UInt16 Write_Data(UInt16 canID, byte cmd, UInt32 address, Byte[] write_buffer, Byte len)
        {
            byte crc;

            lenAdd = (byte)(len + 6);//总长度为数据长度添加附加长度
 
            data_send[0] = 0x55;
            data_send[1] = 0x00;
            data_send[2] = 0x00;

            switch (cmd)
            {
                case 0x01:
                    lenAdd = (byte)(len + 6);
                    data_send[3] = len;
                    data_send[4] = cmd;
                    break;

                case 0x03:

                    lenAdd = (byte)(len + 10);
                    data_send[3] = (byte)(len + 4);
                    data_send[4] = cmd;
                    data_send[5] = (Byte)(address >> 24);
                    data_send[6] = (Byte)(address >> 16);
                    data_send[7] = (Byte)(address >> 8);
                    data_send[8] = (Byte)(address);

                    for (int i = 0; i < len; i++)
                    {
                        data_send[i + 9] = write_buffer[i];// mirrirDate(write_buffer[i]);//数据待做镜面处理
                    }
                    break;
                case 0x05:
                    lenAdd = (byte)(len + 10);
                    data_send[3] = (byte)(len + 4);
                    data_send[4] = cmd;
                    data_send[5] = (Byte)(address >> 24);//换成总长度了
                    data_send[6] = (Byte)(address >> 16);
                    data_send[7] = (Byte)(address >> 8);
                    data_send[8] = (Byte)(address);

                    for (int i = 0; i < len; i++)
                    {
                        data_send[i + 9] = write_buffer[i];// mirrirDate(write_buffer[i]);//数据待做镜面处理
                    }
                    break;
                default:

                    break;
            }
            crc = CrcSum8(data_send, (UInt16)(lenAdd - 2), 3);
         
            data_send[lenAdd - 1] = (byte)crc;


            return 0;
        }

    }
}
