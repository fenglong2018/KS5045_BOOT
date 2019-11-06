using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KS5045上位机
{
    class BinParse
    {
        #region Members
        uint flashExAddress = 0x1000;
        int file_len =0;
        byte[] binchar = new byte[] { };
        UInt16 DataCrc16 = 0;


        #endregion Members
        public byte[] BIN
        {
            get
            {
                return binchar;
            }
            set
            {
                binchar = value;
            }
        }
        public BinParse()
        {
            //String projectName = Assembly.GetExecutingAssembly().GetName().Name.ToString();
            ////Stream Myfile = Assembly.GetExecutingAssembly().GetManifestResourceStream(projectName + ".Resources.PHC7288.txt");
            //Stream Myfile = Assembly.GetExecutingAssembly().GetManifestResourceStream(projectName +Consts.UP_FILE_NAME);
            //file_len = (int)Myfile.Length;//获取bin文件长度
            //BinaryReader binreader = new BinaryReader(Myfile);
            //binchar = binreader.ReadBytes(file_len);
            //binreader.Close();
        }
        public string get_file_ver()
        {
            string deult_ver ="V00.00";
            //deult_ver = Consts.UP_FILE_NAME;
            
            return deult_ver.Substring(24, 6);
        }
        public int get_file_len()
        {
            return file_len;
        }

        public string get_bin_file()
        {
            string Mytext = "";
            foreach (byte j in binchar)
            {
                Mytext += j.ToString("X2");
                Mytext += " ";
            }

            return Mytext;
        }

        public UInt16 get_file_crc()
        {
            DataCrc16 = Crc16Calc(binchar, file_len, flashExAddress);//file_len 

            return DataCrc16;
        }

        private UInt16 Crc16Calc(byte[] data_arr, int data_len, UInt32 ExAddess)
        {
            UInt16 crc16 = 0;
            UInt32 i;
            for (i = ExAddess; i < (data_len); i++)//ExAddess
            {
                crc16 = (UInt16)((crc16 >> 8) | (crc16 << 8));
                crc16 ^= data_arr[i];
                crc16 ^= (UInt16)((crc16 & 0xFF) >> 4);
                crc16 ^= (UInt16)((crc16 << 8) << 4);
                crc16 ^= (UInt16)(((crc16 & 0xFF) << 4) << 1);
            }
            return crc16;
        }

    }
}
