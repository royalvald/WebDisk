using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FileXmlRecord
{
    class FileToXml
    {
        private string DirPath;

        XmlDocument Document = new XmlDocument();

        public string SavaPath
        {
            set;
            get;
        }

        public FileToXml()
        {

        }

        public FileToXml(string DirectoryPath, string savePath)
        {
            this.DirPath = DirectoryPath;
            this.SavaPath = savePath;
            ToXml(DirPath);
        }

        //根据文件夹生成xml树
        private bool ToXml(string path)
        {
            XmlDocument document = new XmlDocument();

            if (Directory.Exists(path))
            {
                DirectoryInfo info = new DirectoryInfo(path);
                XmlElement element = document.CreateElement("file");
                element.SetAttribute("name", Path.GetFileName(path));
                element.SetAttribute("type", "Directory");
                element.SetAttribute("src", Path.GetFullPath(path));
                element.SetAttribute("lastModify", info.LastWriteTime.ToString());
                document.AppendChild(element);
                DirToXml(path, element, document);
            }
            else if (File.Exists(path))
            {
                FileInfo info = new FileInfo(path);
                XmlElement element = document.CreateElement("file");
                element.SetAttribute("name", info.Name);
                element.SetAttribute("size", info.Length.ToString());
                element.SetAttribute("lastModify", info.LastWriteTime.ToString());
                element.SetAttribute("type", "File");
                element.SetAttribute("src", info.FullName);
                document.AppendChild(element);
            }
            document.Save(SavaPath);
            return true;
        }

        private void DirToXml(string path, XmlElement element, XmlDocument document)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            foreach (var item in info.GetFiles())
            {
                FilesToXml(item.FullName, element, document);
            }


            DirectoryInfo[] dirsInfo = info.GetDirectories();
            //XmlElement xmlElement = (XmlElement)GetXmlElement(document, path);
            foreach (var item in dirsInfo)
            {
                XmlElement tempElement = document.CreateElement("file");
                tempElement.SetAttribute("name", item.Name);
                tempElement.SetAttribute("type", "Directory");
                tempElement.SetAttribute("src", item.FullName);
                tempElement.SetAttribute("lastModify", info.LastWriteTime.ToString());
                element.AppendChild(tempElement);
                DirToXml(item.FullName, tempElement, document);
            }


        }

        private void FilesToXml(string path, XmlElement element, XmlDocument document)
        {
            //XmlElement xmlElement = (XmlElement)GetXmlElement(document, Path.GetDirectoryName(path));
            FileInfo info = new FileInfo(path);
            XmlElement tempElement = document.CreateElement("file");
            tempElement.SetAttribute("name", info.Name);
            tempElement.SetAttribute("type", "File");
            tempElement.SetAttribute("size", info.Length.ToString());
            tempElement.SetAttribute("lastModify", info.LastWriteTime.ToString());
            tempElement.SetAttribute("src", info.FullName);

            element.AppendChild(tempElement);

        }

        /// <summary>
        /// 返回特定xml 中的元素
        /// </summary>
        /// <param name="document"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public XmlNode GetXmlElement(XmlDocument document, string filePath)
        {
            string tempPath = null;
            StringBuilder builder = new StringBuilder();
            if (filePath.Contains('/'))
                tempPath = filePath.Replace('/', '\\');
            else tempPath = filePath;
            string[] paths = tempPath.Split('\\');
            foreach (var item in paths)
            {
                builder.Append("//file[@name='" + item + "']");
            }
            string nodePath = builder.ToString();
            XmlNode node;
            try
            {
                node = document.SelectSingleNode(nodePath);
            }
            catch (System.Xml.XPath.XPathException e)
            {
                node = null;
            }

            return node;
        }

        public XmlNode GetXmlElement(string xmlPath, string filePath)
        {
            XmlNode node = null;
            XmlDocument document = new XmlDocument();
            if (File.Exists(xmlPath))
            {
                string tempPath = null;
                StringBuilder builder = new StringBuilder();
                if (filePath.Contains('/'))
                    tempPath = filePath.Replace('/', '\\');
                else tempPath = filePath;
                string[] paths = tempPath.Split('\\');
                foreach (var item in paths)
                {
                    builder.Append("//file[@name='" + item + "']");
                }
                string nodePath = builder.ToString();
               
                try
                {
                    node = document.SelectSingleNode(nodePath);
                }
                catch (System.Xml.XPath.XPathException e)
                {
                    node = null;
                }
            }

            return node;
        }

        /// <summary>
        /// 返回选定节点的属性值
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <param name="dirPath"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public string GetNodeAttribute(string xmlPath, string dirPath, string attributeName)
        {
            XmlDocument document = new XmlDocument();
            document.Load(xmlPath);

            string tempPath = null;
            if (dirPath.Contains('/'))
                tempPath = dirPath.Replace('/', '\\');
            else
                tempPath = dirPath;
            string[] xpath = tempPath.Split('\\');
            StringBuilder builder = new StringBuilder();
            foreach (var item in xpath)
            {
                builder.Append("//file[@name='" + item + "']");
            }

            XmlElement element = (XmlElement)document.SelectSingleNode(builder.ToString());
            return element.GetAttribute(attributeName);
        }

        public string GetNodeAttribute(XmlDocument document, string dirPath, string attributeName)
        {

            string[] xpath = dirPath.Split('\\');
            StringBuilder builder = new StringBuilder();
            foreach (var item in xpath)
            {
                builder.Append("//file[@name='" + item + "']");
            }

            XmlElement element = (XmlElement)document.SelectSingleNode(builder.ToString());
            return element.GetAttribute(attributeName);
        }

        /// <summary>
        /// 设定节点的属性值
        /// </summary>
        /// <param name="document"></param>
        /// <param name="dirPath"></param>
        /// <param name="attributeName"></param>
        /// <param name="attValue"></param>
        public void SetNodeAttribute(XmlDocument document, string dirPath, string attributeName, string attValue)
        {

            string[] xpath = dirPath.Split('\\');
            StringBuilder builder = new StringBuilder();
            foreach (var item in xpath)
            {
                builder.Append("//file[@name='" + item + "']");
            }

            XmlElement element = (XmlElement)document.SelectSingleNode(builder.ToString());
            element.SetAttribute(attributeName, attValue);

        }

        /// <summary>
        /// 删除指定节点
        /// </summary>
        /// <param name="document"></param>
        /// <param name="xpath"></param>
        public void DeleteElement(XmlDocument document, string xpath)
        {
            string[] path = xpath.Split('\\');
            StringBuilder builder = new StringBuilder();
            foreach (var item in path)
            {
                builder.Append("//file[@name='" + item + "']");
            }
            XmlElement element = (XmlElement)document.SelectSingleNode(builder.ToString());
            if (element.ParentNode != null)
            {
                XmlElement parent = (XmlElement)element.ParentNode;
                parent.RemoveChild(element);
            }
        }

        public void DeleteElement(string xmlPath, string xpath)
        {
            if (File.Exists(xmlPath))
            {
                XmlDocument document = new XmlDocument();
                document.Load(xmlPath);
                string[] path = xpath.Split('\\');
                StringBuilder builder = new StringBuilder();
                foreach (var item in path)
                {
                    builder.Append("//file[@name='" + item + "']");
                }
                XmlElement element = (XmlElement)document.SelectSingleNode(builder.ToString());
                if (element.ParentNode != null)
                {
                    XmlElement parent = (XmlElement)element.ParentNode;
                    parent.RemoveChild(element);
                }
            }
        }

        /// <summary>
        /// 根据文件路径返回xml路径
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public string GetXmlPath(string filePath)
        {
            string[] path = filePath.Split('\\');
            StringBuilder builder = new StringBuilder();
            foreach (var item in path)
            {
                builder.Append("//file[@name='" + item + "']");
            }
            return builder.ToString();
        }

        /// <summary>
        /// 判断节点是否存在
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool NodeIsExisted(string xmlPath, string filePath)
        {
            XmlDocument document = new XmlDocument();
            document.Load(xmlPath);
            string tempPath = null;
            if (filePath.Contains('/'))
                tempPath = filePath.Replace('/', '\\');
            else tempPath = filePath;
            string[] path = tempPath.Split('\\');
            StringBuilder builder = new StringBuilder();
            foreach (var item in path)
            {
                builder.Append("//file[@name='" + item + "']");
            }
            XmlElement element = (XmlElement)document.SelectSingleNode(builder.ToString());
            if (element == null)
                return false;
            else return true;
        }

        public bool NodeIsExisted(XmlDocument document, string filePath)
        {            
            string tempPath = null;
            if (filePath.Contains('/'))
                tempPath = filePath.Replace('/', '\\');
            else tempPath = filePath;
            string[] path = tempPath.Split('\\');
            StringBuilder builder = new StringBuilder();
            foreach (var item in path)
            {
                builder.Append("//file[@name='" + item + "']");
            }
            XmlElement element = (XmlElement)document.SelectSingleNode(builder.ToString());
            if (element == null)
                return false;
            else return true;
        }
    }
}
