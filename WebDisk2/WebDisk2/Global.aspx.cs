using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;

namespace WebDisk2
{
    public partial class Global : System.Web.UI.Page
    {

        /// <summary>
        /// 主处理程序，根据不同请求分别处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            string actionType = Request.QueryString["action"];

            switch (actionType)
            {
                case "LIST": ReturnFileList(); break;
                case "DOWNLOAD": DownloadFile(); break;
                case "UPLOAD": UploadFile(); break;
            }
        }
        #region 返回文件列表
        private void ReturnFileList()
        {
            string value = Request.QueryString["value1"];
            string path = Server.MapPath(value);

            StringBuilder sb = new StringBuilder("var GetList={\"Directory\":[", 500);
            DirectoryInfo info = new DirectoryInfo(path);

            //记录当前位置文件夹信息
            foreach (var item in info.GetDirectories())
            {
                sb.Append("{\"Name\":\"" + item.LastAccessTime + "\"," + "\"LastWriteTime\":\"" + item.LastWriteTime
                    + "\"},");
            }

            //去掉最后多余的逗号（','）
            string tempString = sb.ToString();
            if (tempString.EndsWith(","))
            {
                tempString = tempString.Substring(0, tempString.Length - 1);
            }
            sb = new StringBuilder(tempString, 600);
            sb.Append("]\"File\":[");


            foreach (var item in info.GetFiles())
            {
                string size = string.Empty;
                if (item.Length > 1024000000)
                    size = (item.Length / 1024000000).ToString() + "GB";
                else if (item.Length > 1024000)
                    size = (item.Length / 1024000).ToString() + "MB";
                else if (item.Length > 1024)
                    size = (item.Length / 1024).ToString() + "KB";

                sb.Append("{\"Name\":\"" + item.LastAccessTime + "\"," + "\"LastWriteTime\":\"" + item.LastWriteTime
                    + "\",\"Size\":\"" + size + "\"},");
            }
            tempString = sb.ToString();
            if (tempString.EndsWith(","))
            {
                tempString = tempString.Substring(0, tempString.Length - 1);
            }
            sb = new StringBuilder(tempString, 600);
            sb.Append("]}");

            //返回json格式数组
            //Response.Write(sb.ToString());
            Response.Write(sb.ToString());

        }
        #endregion

        #region 文件下载
        private void DownloadFile()
        {
            string value = Request.QueryString["value1"];
            //注意，文件可能不止一个，不同文件之间用'|'分割
            string[] values = value.Split('|');

            foreach (var item in values)
            {
                string filePath = Server.MapPath(item);
                if (File.Exists(filePath))
                {
                    Response.Clear();
                    Response.ClearHeaders();
                    Response.Buffer = false;

                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("Content-Disposition", "attachment;filename=" + 
                        HttpUtility.UrlEncode(Path.GetFileName(filePath), Encoding.UTF8));
                    Response.AddHeader("Content-Length", item.Length.ToString());
                    Response.WriteFile(filePath);
                    Response.Flush();
                    Response.End();
                }
            }

        }
        #endregion

        #region 文件上传
        private void UploadFile()
        {

        }
        #endregion
    }
}