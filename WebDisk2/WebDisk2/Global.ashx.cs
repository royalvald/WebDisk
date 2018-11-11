using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace WebDisk2
{
    /// <summary>
    /// Global1 的摘要说明
    /// </summary>
    public class Global1 : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            //context.Response.ContentType = "text/plain";
            context.Response.Buffer = true;//互不影响
            context.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(0);
            context.Response.Expires = 0;
            context.Response.AddHeader("Pragma", "No-Cache");
            string action = context.Request["action"];//获取操作类型

            switch (action)
            {
                case "LIST": ReturnFileList(context); break;
                case "DOWNLOAD": DownloadFiles(context); break;
                case "UPLOAD": UploadFile(context); break;
                case "DELETE": DeleteFile(context); break;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        #region 返回文件列表
        private void ReturnFileList(HttpContext context)
        {
            //string value = Request.QueryString["value1"];
            string value = context.Request["value1"];
            string path = context.Server.MapPath(value);

            StringBuilder sb = new StringBuilder("var GetList={\"Directory\":[", 500);
            DirectoryInfo info = new DirectoryInfo(path);

            //记录当前位置文件夹信息
            foreach (var item in info.GetDirectories())
            {
                sb.Append("{\"Name\":\"" + item.Name + "\"," + "\"LastWriteTime\":\"" + item.LastWriteTime
                    + "\"},");
            }

            //去掉最后多余的逗号（','）
            string tempString = sb.ToString();
            if (tempString.EndsWith(","))
            {
                tempString = tempString.Substring(0, tempString.Length - 1);
            }
            sb = new StringBuilder(tempString, 600);
            sb.Append("],\"File\":[");


            foreach (var item in info.GetFiles())
            {
                string size = string.Empty;
                if (item.Length > 1024000000)
                    size = (item.Length / 1024000000).ToString() + "GB";
                else if (item.Length > 1024000)
                    size = (item.Length / 1024000).ToString() + "MB";
                else if (item.Length > 1024)
                    size = (item.Length / 1024).ToString() + "KB";

                sb.Append("{\"Name\":\"" + item.Name + "\"," + "\"LastWriteTime\":\"" + item.LastWriteTime
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
            context.Response.Write(sb.ToString());

        }
        #endregion

        #region 文件下载
        private void DownloadFiles(HttpContext context)
        {
            string value = context.Request["value1"];
            //注意，文件可能不止一个，不同文件之间用'|'分割
            string[] values = value.Split('|');

            foreach (var item in values)
            {
                string filePath = context.Server.MapPath(item);
                if (File.Exists(filePath))
                {
                    DownloadFile.ResponseFile(filePath, context, false);
                }
            }

        }
        #endregion

        #region 文件上传
        private void UploadFile(HttpContext context)
        {
            string requestDir = context.Request["value1"];
            string fullDirPath = context.Server.MapPath(requestDir);
            var files = context.Request.Files;

            for (int i = 0; i < files.Count; i++)
            {
                files[i].SaveAs(fullDirPath + Path.GetFileName(files[i].FileName));
            }

            context.Response.Write("OK");
        }
        #endregion

        #region 文件删除
        private void DeleteFile(HttpContext context)
        {
            string value = context.Request["value1"];
            string[] deleteFilesCollection = value.Split('|');

            foreach (var item in deleteFilesCollection)
            {
                string tempFilePath = context.Server.MapPath(item);
                //文件删除,文件夹删除
                if (File.Exists(tempFilePath))
                    File.Delete(tempFilePath);
                else if (Directory.Exists(tempFilePath))
                    Directory.Delete(tempFilePath);
            }

            context.Response.Write("OK");
        }
        #endregion
    }
}