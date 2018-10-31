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
                IntiFolderTree();
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

        }

        protected void fileDownload_Click(object sender, EventArgs e)
        {

        }

        protected void fileDelete_Click(object sender, EventArgs e)
        {

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
            Response.Write(string.Format("<script>alert('{0}')<script/>", info));
        }

        private void IntiFolderTree()
        {
            string tempPath = ConfigurationManager.AppSettings["FileSavePath"];
            string folderPath = Server.MapPath(tempPath);

            string[] infos = Directory.GetFileSystemEntries(folderPath);
            fileList.Items.Clear();
            foreach (var item in infos)
            {
                fileList.Items.Add(item);
            }
        }
    }
}