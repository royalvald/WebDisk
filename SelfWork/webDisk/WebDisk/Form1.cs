using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WebDisk
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始树目录展开
        /// </summary>
        /// <param name="path"></param>
        public void ShowDir(string path)
        {
            //树目录可以编辑
            treeView1.LabelEdit = true;
            treeView1.PathSeparator = "\\";

            foreach (var item in Directory.GetFiles(path))
            {
                TreeNode node = new TreeNode();
                node.Text = Path.GetFileName(item);
                treeView1.Nodes.Add(node);
            }

            foreach (var item in Directory.GetDirectories(path))
            {
                TreeNode node = new TreeNode();
                node.Text = Path.GetFileName(item);
                treeView1.Nodes.Add(node);
            }
        }


        private void NodeExpand(string path)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShowDir(@"G:\1");
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string path = e.Node.FullPath;
            ShowDir(@"G:\1" + path);
        }
    }
}
