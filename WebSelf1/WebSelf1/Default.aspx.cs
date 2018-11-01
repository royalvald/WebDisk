using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;

namespace WebSelf1
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //文件夹信息读取
                IntiFolderTree();

                //磁盘使用信息
                IntiFolderUseInfo();
            }
        }

      

        protected void selectList_SelectedIndexChanged(object sender, EventArgs e)
        {
            multiView1.ActiveViewIndex = Convert.ToInt32(selectList.SelectedValue) - 1;
        }

        protected void Upload_button_Click(object sender, EventArgs e)
        {
            if (fileUpload.HasFile)
            {
                if (IsFollowFileSize())
                {
                    string rootPath = ConfigurationManager.AppSettings["FileSavePath"];
                    string savePath = Server.MapPath(rootPath + "/") + fileUpload.FileName;
                    if (!File.Exists(savePath))
                    {
                        fileUpload.SaveAs(savePath);

                        //此处更新文件夹信息
                        IntiFolderTree();

                        ShowMessage("上传成功");
                    }
                    else
                    {
                        ShowMessage("文件已经存在");
                    }
                }
                else
                {
                    ShowMessage("文件太大应小于1G");
                }
            }
        }



        protected void fileList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string rootPath = ConfigurationManager.AppSettings["FileSavePath"].ToString();
            string filePath = Server.MapPath(rootPath + "/") + fileList.SelectedValue;


            if (!Directory.Exists(filePath))
            {
                FileInfo fileInfoTemp = new FileInfo(filePath);
                long size = fileInfoTemp.Length;
                string infos = string.Format("{0}   {1}", size, fileInfoTemp.LastWriteTime);
                fileInfo.Text = infos;
                file_newName.Text = Path.GetFileName(filePath);
            }
        }

        protected void fileDownload_Click(object sender, EventArgs e)
        {
            string rootPath = ConfigurationManager.AppSettings["FileSavePath"].ToString();
            string filePath = Server.MapPath(rootPath + "/") + fileList.SelectedValue;
            FileInfo fileInfo = new FileInfo(filePath);

            Response.Clear();
            Response.ClearHeaders();
            Response.Buffer = false;

            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileInfo.Name,System.Text.Encoding.UTF8));
            Response.AddHeader("Content-Length", fileInfo.Length.ToString());
            Response.WriteFile(filePath);
            Response.Flush();
            Response.End();
        }

        protected void fileDelete_Click(object sender, EventArgs e)
        {
            if(fileList.SelectedValue!=null)
            {
                string rootPath = ConfigurationManager.AppSettings["FileSavePath"].ToString();
                string filePath = Server.MapPath(rootPath + "/") + fileList.SelectedValue;

                File.Delete(filePath);
                fileInfo.Text = "";
                IntiFolderTree();
            }
            else
            {
                ShowMessage("请先选定文件");
            }
        }











        private bool IsFollowFileSize()
        {
            int size = Convert.ToInt32(ConfigurationManager.AppSettings["FileSize"]) * 1024;

            if (fileUpload.PostedFile.ContentLength > size)
                return false;
            else return true;
        }

        private void ShowMessage(string info)
        {
            Response.Write(string.Format("<script>alert('{0}')</script>", info));
        }

        /// <summary>
        /// 展示文件夹下文件结构
        /// </summary>
        private void IntiFolderTree()
        {
            string tempPath = ConfigurationManager.AppSettings["FileSavePath"];
            string folderPath = Server.MapPath(tempPath);

            string[] infos = Directory.GetFileSystemEntries(folderPath);
            fileList.Items.Clear();
            foreach (var item in infos)
            {
                fileList.Items.Add(Path.GetFileName(item));
            }
        }

        /// <summary>
        /// 初始化磁盘使用信息
        /// </summary>
        private void IntiFolderUseInfo()
        {
            string size = ConfigurationManager.AppSettings["FileSize"].ToString();
            fileMaxsize.Text = size;


        }

        protected void file_rename_Click(object sender, EventArgs e)
        {
            if(file_newName.Text!=null)
            {
                string rootPath = ConfigurationManager.AppSettings["FileSavePath"].ToString();
                string filePath = Server.MapPath(rootPath + "/") + fileList.SelectedValue;
                string newFile = filePath.Substring(0, filePath.LastIndexOf("\\"))+"\\" + file_newName.Text;
                File.Move(filePath, newFile);
                IntiFolderTree();
                ShowMessage("重命名成功");

            }
        }
    }
}