using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace KS5045上位机
{
     public class disp_list
    {
         KS5045上位机.ListViewFN my_list_view = new KS5045上位机.ListViewFN();
        class colums_info
        {


        }

        public void set_list_view(KS5045上位机.ListViewFN m_list_view)
        {
            my_list_view = m_list_view;
        }

        public class COLUMS_INFO
        {
            public int info_id { get; set; }
            public string Length { get; set; }
            public string Name { get; set; }
            public string Value1 { get; set; }
            public string Value2 { get; set; }
            public string Value3 { get; set; }
            public string Value4 { get; set; }
            public string Value5 { get; set; }
            public string Value6 { get; set; }
            public string Value7 { get; set; }
            public string Value8 { get; set; }

            public string getName()
            {
                return Name;
            }

            public string getLength()
            {
                return Length;
            }


            public string getValue(byte val)
            {
                switch (val)
                {
                    case 1:
                        return Value1;
                    case 2:
                        return Value2;
                    case 3:
                        return Value3;
                    case 4:
                        return Value4;
                    case 5:
                        return Value5;
                    case 6:
                        return Value6;
                    case 7:
                        return Value7;
                    case 8:
                        return Value8;
                    default:
                        return Value1;
                }
            }

            public void setValue(string val,byte index)
            {
                switch (index)
                {
                    case 1:
                        this.Value1 = val;
                        break;
                    case 2:
                        this.Value2 = val;
                        break;
                    case 3:
                        this.Value3 = val;
                        break;
                    case 4:
                        this.Value4 = val;
                        break;
                    case 5:
                        this.Value5 = val;
                        break;
                    case 6:
                        this.Value6 = val;
                        break;
                    case 7:
                        this.Value7 = val;
                        break;
                    case 8:
                        this.Value8 = val;
                        break;
                    default:
                        break;
                }
            }
        }


        public List<COLUMS_INFO> users = new List<COLUMS_INFO>
        {
            new COLUMS_INFO{ info_id=0x01, Name="电池电压电流",                     Length="4",          Value1=" ",Value2=" ",Value3=" ",Value4=" ",Value5=" ",Value6=" ",Value7=" ",Value8=" "},
            new COLUMS_INFO{ info_id=0x02, Name="电池容量信息",                     Length="8",          Value1=" ",Value2=" ",Value3=" ",Value4=" ",Value5=" ",Value6=" ",Value7=" ",Value8=" "},
            new COLUMS_INFO{ info_id=0x03, Name="单节电压4",                        Length="8",          Value1=" ",Value2=" ",Value3=" ",Value4=" ",Value5=" ",Value6=" ",Value7=" ",Value8=" "},
            new COLUMS_INFO{ info_id=0x04, Name="单节电压8",                        Length="8",          Value1=" ",Value2=" ",Value3=" ",Value4=" ",Value5=" ",Value6=" ",Value7=" ",Value8=" "},
            new COLUMS_INFO{ info_id=0x05, Name="单节电压12",                       Length="4",          Value1=" ",Value2=" ",Value3=" ",Value4=" ",Value5=" ",Value6=" ",Value7=" ",Value8=" "},
            new COLUMS_INFO{ info_id=0x06, Name="电池温度信息",                     Length="8",          Value1=" ",Value2=" ",Value3=" ",Value4=" ",Value5=" ",Value6=" ",Value7=" ",Value8=" "},
            new COLUMS_INFO{ info_id=0x07, Name="电池版本信息",                     Length="8",          Value1=" ",Value2=" ",Value3=" ",Value4=" ",Value5=" ",Value6=" ",Value7=" ",Value8=" "},
            new COLUMS_INFO{ info_id=0x08, Name="电池序列号",                       Length="8",          Value1=" ",Value2=" ",Value3=" ",Value4=" ",Value5=" ",Value6=" ",Value7=" ",Value8=" "},
            new COLUMS_INFO{ info_id=0x09, Name="电池状态信息",                     Length="8",          Value1=" ",Value2=" ",Value3=" ",Value4=" ",Value5=" ",Value6=" ",Value7=" ",Value8=" "},
            new COLUMS_INFO{ info_id=0x0A, Name="电池记录信息",                     Length="8",          Value1=" ",Value2=" ",Value3=" ",Value4=" ",Value5=" ",Value6=" ",Value7=" ",Value8=" "},
            new COLUMS_INFO{ info_id=0xF0, Name="内部预留信息",                     Length="8",          Value1=" ",Value2=" ",Value3=" ",Value4=" ",Value5=" ",Value6=" ",Value7=" ",Value8=" "},
          
        };

        public void clearList()
        {
            foreach (COLUMS_INFO item in users)
            {
                item.Value1 = " ";
                item.Value2 = " ";
                item.Value3 = " ";
                item.Value4 = " ";
                item.Value5 = " ";
                item.Value6 = " ";
                item.Value7 = " ";
                item.Value8 = " ";
            }
        }


        public void set_value(int id, string value,byte index)
        {
            int find_index = users.FindIndex(o => o.info_id == id);
            if (find_index >= 0)
            {
                switch (index)
                {
                    case 0:
                        users[find_index].Value1 = value;
                        break;
                    case 1:
                        users[find_index].Value2 = value;
                        break;
                    case 2:
                        users[find_index].Value3 = value;
                        break;
                    case 3:
                        users[find_index].Value4 = value;
                        break;
                    case 4:
                        users[find_index].Value5 = value;
                        break;
                    case 5:
                        users[find_index].Value6 = value;
                        break;
                    case 6:
                        users[find_index].Value7 = value;
                        break;
                    case 7:
                        users[find_index].Value8 = value;
                        break;
                    default:
                        break;
                }
            }
        }


        public string get_value(int id,byte index)
        {
            int find_index = users.FindIndex(o => o.info_id == id);
            if (find_index >= 0)
            {
                switch (index)
                {
                    case 1:
                        return users[find_index].Value1;
                    case 2:
                        return users[find_index].Value2;
                    case 3:
                        return users[find_index].Value3;
                    case 4:
                        return users[find_index].Value4;
                    case 5:
                        return users[find_index].Value5;
                    case 6:
                        return users[find_index].Value6;
                    case 7:
                        return users[find_index].Value7;
                    case 8:
                        return users[find_index].Value8;
                    default:
                        return "";
                }
            }
            else
            {
                return "";
            }
        }

        public string get_length(int id)
        {
            int find_index = users.FindIndex(o => o.info_id == id);
            if (find_index >= 0)
            {
                return users[find_index].Length;
            }
            else
            {
                return "";
            }
        }

        public List<COLUMS_INFO> GetTUserList(int id)
        {
            List<COLUMS_INFO> userList = users.FindAll(o => o.info_id == id);
            if (userList != null)
            {

            }
            return userList;
        }


        public void disp_init()
        {
            this.my_list_view.View = System.Windows.Forms.View.Details;

            this.my_list_view.Columns.Add("BMS ID", 80, HorizontalAlignment.Left);
            this.my_list_view.Columns.Add("Name", 180, HorizontalAlignment.Left);
            this.my_list_view.Columns.Add("DLC", 60, HorizontalAlignment.Center);
            this.my_list_view.Columns.Add("Data1", 80, HorizontalAlignment.Left);
            this.my_list_view.Columns.Add("Data2", 80, HorizontalAlignment.Left);
            this.my_list_view.Columns.Add("Data3", 80, HorizontalAlignment.Left);
            this.my_list_view.Columns.Add("Data4", 80, HorizontalAlignment.Left);
            this.my_list_view.Columns.Add("Data5", 80, HorizontalAlignment.Left);
            this.my_list_view.Columns.Add("Data6", 80, HorizontalAlignment.Left);
            this.my_list_view.Columns.Add("Data7", 80, HorizontalAlignment.Left);
            this.my_list_view.Columns.Add("Data8", 80, HorizontalAlignment.Left);

            ImageList imgList = new ImageList();

            imgList.ImageSize = new Size(3, 3);// 设置行高 20 //分别是宽和高

            my_list_view.SmallImageList = imgList; //这里设置listView的SmallImageList ,用imgList将其撑大

            this.my_list_view.GridLines = true;


        }
        //整体更新listview 若出现 下拉条,刷新时无法下拉
        //单行更新功能
        public void list_view_update()
        {

            this.my_list_view.Invoke(new EventHandler(delegate
            {
                this.my_list_view.BeginUpdate();   //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度
                //int j = 0;
                this.my_list_view.Items.Clear();

                foreach (COLUMS_INFO item in users)
                {

                    ListViewItem l_id = new ListViewItem(item.info_id.ToString("X2").ToUpper());//item.info_id.ToString("X2").ToUpper()

                    l_id.SubItems.Add(item.Name);
                    l_id.SubItems.Add(item.Length);
                    l_id.SubItems.Add(item.Value1);
                    l_id.SubItems.Add(item.Value2);
                    l_id.SubItems.Add(item.Value3);
                    l_id.SubItems.Add(item.Value4);
                    l_id.SubItems.Add(item.Value5);
                    l_id.SubItems.Add(item.Value6);
                    l_id.SubItems.Add(item.Value7);
                    l_id.SubItems.Add(item.Value8);
                    //my_list_view.Items[j] = l_id;
                    //j++;
                    //my_list_view.Items.AddRange(l_id);
                    my_list_view.Items.Add(l_id);
                }

                this.my_list_view.EndUpdate();  //结束数据处理，UI界面一次性绘制。
            }));


        }
        public string save_list_view()
        {
            String save_line = "";
            this.my_list_view.Invoke(new EventHandler(delegate
            {
                foreach (COLUMS_INFO item in users)
                {
                    save_line += item.Value1 + '\t' + item.Value2 + '\t' + item.Value3 + '\t'
                         + item.Value4 + '\t' + item.Value5 + '\t' + item.Value6 + '\t'
                          + item.Value7 + '\t' + item.Value8 + '\t';
                }
            }));

            return save_line;
        }
        public string save_list_name()
        {
            String save_line = "";
            this.my_list_view.Invoke(new EventHandler(delegate
            {
                foreach (COLUMS_INFO item in users)
                {
                    save_line += item.Name + '\t';
                }
            }));

            return save_line;
        }
    }
}
