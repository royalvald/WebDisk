﻿using System;
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
                XmlElement element = document.CreateElement("file");
                element.SetAttribute("name", Path.GetFileName(path));
                element.SetAttribute("type", "Directory");
                element.SetAttribute("src", Path.GetFullPath(path));
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

        public XmlNode GetXmlElement(XmlDocument document, string xpath)
        {
            StringBuilder builder = new StringBuilder();
            string[] paths = xpath.Split('\\');
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

        public string GetNodeAttribute(string xmlPath, string dirPath, string attributeName)
        {
            XmlDocument document = new XmlDocument();
            document.Load(xmlPath);

            string[] xpath = dirPath.Split('\\');
            StringBuilder builder = new StringBuilder();
            foreach (var item in xpath)
            {
                builder.Append("//file[@name='" + item + "']");
            }

            XmlElement element = (XmlElement)document.SelectSingleNode(builder.ToString());
            return element.GetAttribute(attributeName);
        }
    }
}
