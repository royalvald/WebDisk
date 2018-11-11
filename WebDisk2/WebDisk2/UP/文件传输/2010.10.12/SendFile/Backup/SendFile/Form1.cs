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
using System.Xml.Serialization;
using System.Threading;

namespace SendFile
{
    public partial class Form1 : Form
    {
        private FileDetails fileDet = new FileDetails();
        private IPAddress ipAddress;
        private IPEndPoint ipEndPoint;
        private Int32 port;
        private static UdpClient udpClient;
        private FileStream fs;
        public Form1()
        {
            InitializeComponent();
        }
        public class FileDetails
        {
            public string FILETYPE = "";
            public long FILESIZE = 0;
        }
        //发送文件的信息
        private bool SendFileInfo()
        {
            try
            {
                FileSystemInfo fileInfo = new FileInfo(txtLocation.Text);
                //获取文件的扩展名
                fileDet.FILETYPE = fileInfo.Extension;
                //获取文件的长度
                fileDet.FILESIZE = fs.Length;
                XmlSerializer fileSerializer = new XmlSerializer(typeof(FileDetails));
                MemoryStream stream = new MemoryStream();
                fileSerializer.Serialize(stream, fileDet);
                stream.Position = 0;
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, Convert.ToInt32(stream.Length));
                //发送文件的细节
                udpClient.Send(bytes, bytes.Length);
                listBox1.Items.Add("文件正在发送中……");
                stream.Close();
                return true;
            }
            catch (Exception exc)
            {
                listBox1.Items.Add(exc.Message);
                return false;
            }
        }


        private void SendFile()
        {
            try
            {
                //创建文件流
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                listBox1.Items.Add("发送的文件大小是：" + fs.Length + "字节");
                //发送文件
                udpClient.Send(bytes, bytes.Length);
                listBox1.Items.Add("文件成功发送!");
            }
            catch (Exception exc)
            {
                listBox1.Items.Add(exc.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ipAddress = IPAddress.Parse(txtIP.Text);
                port = Int32.Parse(txtPort.Text);
                ipEndPoint = new IPEndPoint(ipAddress, port);
                udpClient = new UdpClient(Dns.GetHostByAddress(ipAddress).HostName, port);
                fs = new FileStream(txtLocation.Text, FileMode.Open, FileAccess.Read);
                //判断是否长度超过8k最大允许值
                if (fs.Length > 2000000)
                {
                    listBox1.Items.Add("文件传输文件大小必须小于2000000字节!");
                    fs.Close();
                    udpClient.Close();
                    return;
                }
                //如果成功发送文件信息给接收端，则继续发送文件的内容给接收端
                if (SendFileInfo())
                {
                    Thread.Sleep(2000);
                    SendFile();
                    udpClient.Close();
                    fs.Close();
                }
            }
            catch (Exception exc)
            {
                listBox1.Items.Add(exc.Message + "1");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    txtLocation.Text = openFileDialog1.FileName;
                }
            }
            catch (Exception exc)
            {
                listBox1.Items.Add(exc.Message);
            }
        }


    }
}