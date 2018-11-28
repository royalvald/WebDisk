using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Xml.Serialization;

namespace ReceiveFile
{
    public partial class Form1 : Form
    {
        public class FileDetails
        {
            public String FILETYPE = "";
            public long FILESIZE = 0;
        }
        private FileDetails fileDet;
        private UdpClient udpClient;
        private IPEndPoint ipEndPoint = null;
        private FileStream fs = null;
        private byte[] receiveBytes = new byte[0];
        private Thread threadReceive;

        //更新控件的状态。开始按钮与停止按钮（txtport）的状态相反
        private void UpdateControls(bool state)
        {
            button1.Enabled = state;
            button2.Enabled = !state;
            txtPort.ReadOnly = !state;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void ReceiveMessages()
        {
            try
            {
                listBox1.Items.Add("开始等待文件细节……");
                receiveBytes = udpClient.Receive(ref ipEndPoint);
                txtIP.Text = ipEndPoint.Address.ToString();
                listBox1.Items.Add("读取文件细节");
                XmlSerializer fileSerializer = new XmlSerializer(typeof(FileDetails));
                MemoryStream stream = new MemoryStream();
                stream.Write(receiveBytes, 0, receiveBytes.Length);
                stream.Position = 0;
                fileDet = (FileDetails)fileSerializer.Deserialize(stream);
                listBox1.Items.Add("接收到的文件其类型是：" + fileDet.FILETYPE + "其大小是：" + fileDet.FILESIZE.ToString() + "字节");
            }
            catch (Exception exc)
            {
                listBox1.Items.Add(exc.Message);
            }
        }
        private void ReceiveFile()
        {
            try
            {
                listBox1.Items.Add("等待文件……");
                receiveBytes = udpClient.Receive(ref ipEndPoint);
                listBox1.Items.Add("开始接收文件……");
                //文件名筛选器的说明
                string s = "文件(*" + fileDet.FILETYPE + ")";
                saveFileDialog1.Filter = s.ToString() + "|*" + fileDet.FILETYPE;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    fs = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    fs.Write(receiveBytes, 0, receiveBytes.Length);
                    listBox1.Items.Add("文件已保存成功！");
                    fs.Flush(); //真正将数据写入文件中
                }
            }
            catch (Exception exc)
            {
                listBox1.Items.Add(exc.Message);
            }


        }


        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateControls(true);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //当服务器在运行时，不允许直接关闭，必须先停止服务，才可关闭
            if (button1.Enabled == false)
            {
                e.Cancel = true;
                MessageBox.Show("请先停止！", "提示");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //改变按钮控件的状态
                UpdateControls(false);
                udpClient = new UdpClient(Int32.Parse(txtPort.Text));
                threadReceive = new Thread(new ThreadStart(ReceiveMessages));
                threadReceive.Start();
            }
            catch (Exception exc)
            {
                listBox1.Items.Add(exc.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateControtrls(true);
                txtIP.Text = "";
                threadReceive.Abort();
                if (fs != null)
                    fs.Close();
                udpClient.Close();
            }
            catch (Exception exc)
            {
                listBox1.Items.Add(exc.Message);
            }
        }

        private void UpdateControtrls(bool state)
        {
            button1.Enabled = state;
            button2.Enabled = !state;
            txtPort.ReadOnly = !state;  
        }
    }
}