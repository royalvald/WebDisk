﻿using FileXmlRecord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace WebDisk2
{
    /// <summary>
    /// Global1 的摘要说明
    /// </summary>
    public class Global1 : IHttpHandler
    {
        string rootPath = "UP/";
        string xmlRootPath = "./test.xml";
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
                case "LIST": ReturnFileList(context, true); break;
                case "DOWNLOAD": DownloadFiles(context); break;
                case "UPLOAD": UploadFile(context); break;
                case "DELETE": DeleteFile(context); break;
                case "COPY": MoveFile(context, true); break;
                case "CUT": MoveFile(context, false); break;
                case "RENAME": Rename(context); break;
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

        private void ReturnFileList(HttpContext context, bool tag)
        {
            //string value = Request.QueryString["value1"];

            string value = context.Request["value1"];
            string path = context.Server.MapPath(value);

            FileToXml xml = new FileToXml();
            XmlDocument document = new XmlDocument();
            if (File.Exists("./test.xml"))
                document.Load("./test.xml");
            else
            {
                FileToXml tempXml = new FileToXml(context.Server.MapPath("./UP"), context.Server.MapPath("./test.xml"));
                document.Load(context.Server.MapPath("./test.xml"));
            }
            StringBuilder sb = new StringBuilder("var GetList={\"Directory\":[", 500);
            DirectoryInfo info = new DirectoryInfo(path);

            //记录当前位置文件夹信息
            /*foreach (var item in info.GetDirectories())
            {
                sb.Append("{\"Name\":\"" + item.Name + "\"," + "\"LastWriteTime\":\"" + item.LastWriteTime
                    + "\"},");
            }*/

            string xmlPath = value.Substring(2, value.Length - 3);

            XmlNode node = xml.GetXmlElement(document, xmlPath.Replace('/', '\\'));
            XmlNodeList list = node.ChildNodes;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Attributes["type"].Value.Equals("Directory"))
                    sb.Append("{\"Name\":\"" + list[i].Attributes["name"].Value + "\"," + "\"LastWriteTime\":\"" + list[i].Attributes["lastModify"].Value
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


            /*foreach (var item in info.GetFiles())
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
            }*/

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Attributes["type"].Value.Equals("File"))
                    sb.Append("{\"Name\":\"" + list[i].Attributes["name"].Value + "\"," + "\"LastWriteTime\":\"" + list[i].Attributes["lastModify"].Value
                    + "\",\"Size\":\"" + list[i].Attributes["size"].Value + "\"},");
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
            FileToXml xml = new FileToXml();

            foreach (var item in values)
            {
                string filePath = context.Server.MapPath(item);
                string xmlPath = filePath.Substring(2, filePath.Length - 3);

                //XmlNode node = xml.GetXmlElement(document, xmlPath.Replace('/', '\\'));
                if (xml.NodeIsExisted(xmlRootPath, xmlPath))
                {

                    DownloadFile.ResponseFile(xml.GetNodeAttribute(xmlRootPath, item, "src"), context, false);
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
            string LogPath = context.Server.MapPath(rootPath) + System.DateTime.Now.ToString();
            FileToXml xml = new FileToXml();


            Directory.CreateDirectory(LogPath);
            string rootDir = context.Server.MapPath(rootPath);
            string parentPath = Path.GetDirectoryName(rootDir);
            //创建时间记录文件夹
            string trackRoot = parentPath + "/BackTrack";
            if (!Directory.Exists(trackRoot))
                Directory.CreateDirectory(trackRoot);
            string nowTime = System.DateTime.Now.ToString();
            //需要对获得的时间进行字符串处理
            string tempTrackPath = trackRoot + nowTime.Replace('/', ' ').Replace(':', '-');
            Directory.CreateDirectory(tempTrackPath);
            File.Copy("./test.xml", tempTrackPath + "./test.xml");

            XmlDocument document = new XmlDocument();
            document.Load("./test.xml");
            int suffix = 0;

            for (int i = 0; i < files.Count; i++)
            {
                if (File.Exists(fullDirPath + Path.GetFileName(files[i].FileName)))
                {

                    //File.Move(fullDirPath + Path.GetFileName(files[i].FileName),)
                    //files[i].SaveAs(fullDirPath + Path.GetFileName(files[i].FileName));

                    if (File.Exists(tempTrackPath + Path.GetFileName(files[i].FileName)))
                        while (true)
                        {
                            if (File.Exists(tempTrackPath + Path.GetFileName(files[i].FileName) + suffix))
                                i++;
                            else
                            {
                                files[i].SaveAs(tempTrackPath + Path.GetFileName(files[i].FileName) + suffix);
                                xml.SetNodeAttribute(document, fullDirPath + Path.GetFileName(files[i].FileName), "src", tempTrackPath + Path.GetFileName(files[i].FileName) + suffix);
                            }
                        }
                    else
                    {
                        files[i].SaveAs(tempTrackPath + Path.GetFileName(files[i].FileName));
                        xml.SetNodeAttribute(document, fullDirPath + Path.GetFileName(files[i].FileName), "src", tempTrackPath + Path.GetFileName(files[i].FileName));
                    }

                }
                else
                {
                    files[i].SaveAs(fullDirPath + Path.GetFileName(files[i].FileName));
                    FileInfo info = new FileInfo(fullDirPath + Path.GetFileName(files[i].FileName));
                    XmlElement element = (XmlElement)xml.GetXmlElement(document, fullDirPath);
                    XmlElement child = document.CreateElement("file");
                    child.SetAttribute("name", info.Name);
                    child.SetAttribute("type", "File");
                    child.SetAttribute("size", info.Length.ToString());
                    child.SetAttribute("lastModify", info.LastWriteTime.ToString());
                    child.SetAttribute("src", info.FullName);
                    element.AppendChild(child);
                }
            }

            document.Save("./test.xml");

            context.Response.Write("OK");
        }
        #endregion

        #region 文件删除
        private void DeleteFile(HttpContext context)
        {
            string value = context.Request["value1"];
            string[] deleteFilesCollection = value.Split('|');

            FileToXml xml = new FileToXml();
            string rootDir = context.Server.MapPath(rootPath);
            string parentPath = Path.GetDirectoryName(rootDir);
            string trackRoot = parentPath + "/BackTrack";
            if (!Directory.Exists(trackRoot))
                Directory.CreateDirectory(trackRoot);
            string nowTime = System.DateTime.Now.ToString();
            //需要对获得的时间进行字符串处理
            string tempTrackPath = trackRoot + nowTime.Replace('/', ' ').Replace(':', '-');
            Directory.CreateDirectory(tempTrackPath);
            File.Copy("./test.xml", tempTrackPath + "./test.xml");

            XmlDocument document = new XmlDocument();
            document.Load("./test.xml");
            foreach (var item in deleteFilesCollection)
            {
                string tempFilePath = context.Server.MapPath(item);
                //文件删除,文件夹删除
                if (File.Exists(tempFilePath))
                {
                    //File.Delete(tempFilePath);
                    xml.DeleteElement(document, tempFilePath);
                }
                else if (Directory.Exists(tempFilePath))
                {
                    //Directory.Delete(tempFilePath);
                    xml.DeleteElement(document, tempFilePath);
                }
            }

            document.Save("./test.xml");
            context.Response.Write("OK");
        }
        #endregion

        #region 文件(夹)复制和剪切
        /// <summary>
        /// /flag为true时代表是文件复制，为false时代表是文件剪切
        /// </summary>
        /// <param name="context"></param>
        /// <param name="flag"></param>
        private void MoveFile(HttpContext context, bool flag)
        {
            //获取请求内容value1和value2

            //目的文件夹
            string value = context.Request["value1"];
            string desFolder = context.Server.MapPath(value);

            //待复制（粘贴）文件信息
            string fileInfo = context.Request["value2"];
            //可能多个文件信息以'|'分割
            string[] fileList = fileInfo.Split('|');

            //时间记录
            GenerateTrack(context);

            /*foreach (var item in fileList)
            {
                if (File.Exists(item))
                {
                    if (flag == true)
                        File.Copy(item, desFolder + Path.GetFileName(item));
                    else if (flag == false)
                        File.Move(item, desFolder + Path.GetFileName(item));
                }
                else if (Directory.Exists(item))
                {
                    if (flag == true)
                        DirCopy(item, desFolder + Path.GetFileName(item));
                    else if (flag == false)
                        Directory.Move(item, desFolder + Path.GetFileName(item));
                }
            }*/
            FileToXml xml = new FileToXml();
            XmlDocument document = new XmlDocument();
            document.Load("./test.xml");
            foreach (var item in fileList)
            {
                if (xml.NodeIsExisted("./test.xml", item))
                {

                    if (flag == true)
                    {
                        XmlElement desElement = (XmlElement)xml.GetXmlElement(document, desFolder);
                        XmlElement sourceElement = (XmlElement)xml.GetXmlElement(document, item);
                        desElement.AppendChild(sourceElement);
                        //File.Copy(item, desFolder + Path.GetFileName(item));
                    }
                    else if (flag == false)
                    {
                        XmlElement desElement = (XmlElement)xml.GetXmlElement(document, desFolder);
                        XmlElement sourceElement = (XmlElement)xml.GetXmlElement(document, item);
                        XmlElement copyElement = (XmlElement)sourceElement.Clone();
                        desElement.AppendChild(copyElement);
                        string sourceParentPath = Path.GetDirectoryName(item);
                        XmlElement parentNode = (XmlElement)xml.GetXmlElement(document, sourceParentPath);
                        parentNode.RemoveChild(sourceElement);
                    }

                }

            }
            document.Save("./test.xml");
            context.Response.Write("OK");
        }

        //文件夹复制
        private void DirCopy(string sourcePath, string desFolder)
        {
            string desPath = desFolder + Path.GetFileName(sourcePath);
            Directory.CreateDirectory(desPath);
            DirectoryInfo info = new DirectoryInfo(sourcePath);

            foreach (var item in info.GetFiles())
            {
                File.Copy(item.FullName, desPath + "\\" + Path.GetFileName(item.Name));
            }

            foreach (var item in info.GetDirectories())
            {
                DirCopy(item.FullName, desPath + "\\" + Path.GetFileName(item.Name));
            }

        }
        #endregion

        #region 文件重命名

        private void Rename(HttpContext context)
        {

            //时间记录
            GenerateTrack(context);

            string oldFile = context.Server.MapPath(context.Request["value1"]);
            string newFile = context.Server.MapPath(context.Request["value2"]);
            /*if (File.Exists(oldFile))
                File.Move(oldFile, newFile);
            else if (Directory.Exists(oldFile))
                Directory.Move(oldFile, newFile);*/

            FileToXml xml = new FileToXml();
            XmlDocument document = new XmlDocument();
            document.Load("./test.xml");

            if (xml.NodeIsExisted(document, oldFile))
            { 
                string newName = Path.GetFileName(newFile);
                xml.SetNodeAttribute(document, oldFile, "name", newName);
            }

            document.Save("./test.xml");
        }

        #endregion

        #region 基于xml版本控制
        /// <summary>
        /// 编辑xml
        /// </summary>
        /// <param name="document"></param>
        /// <param name="filePath"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        //private bool EditFilelist(XmlDocument document,string filePath,string attribute)
        //{
        // FileToXml xml = new FileToXml();
        // xml.GetNodeAttribute(document, filePath, attribute);
        //}

        private bool EditFilelist(string xmlPath, string filePath, string attribute, string value)
        {
            XmlDocument document = new XmlDocument();
            document.Load(xmlPath);

            FileToXml xml = new FileToXml();
            xml.SetNodeAttribute(document, filePath, attribute, value);
            return true;
        }


        private void GenerateTrack(HttpContext context)
        {
            string rootDir = context.Server.MapPath(rootPath);
            string parentPath = Path.GetDirectoryName(rootDir);
            //创建时间记录文件夹
            string trackRoot = parentPath + "/BackTrack";
            if (!Directory.Exists(trackRoot))
                Directory.CreateDirectory(trackRoot);
            string nowTime = System.DateTime.Now.ToString();
            //需要对获得的时间进行字符串处理
            string tempTrackPath = trackRoot + nowTime.Replace('/', ' ').Replace(':', '-');
            Directory.CreateDirectory(tempTrackPath);
            File.Copy("./test.xml", tempTrackPath + "./test.xml");
        }
        private void GenerateXmlFile(string filePath, string xmlSavePath)
        {
            FileToXml xml = new FileToXml(filePath, xmlSavePath);
        }
        #endregion


    }
}