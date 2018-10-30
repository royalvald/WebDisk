using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WebDisk
{
    class FileOperate
    {
        /// <summary>
        /// 文件创建，option为true时覆盖文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="option"></param>
        public void FileCreat(string path, string fileName,bool option)
        {
            if (File.Exists(path))
            {
                if(option)
                {
                    File.Delete(path);
                    File.Create(path);
                }
                else
                {
                    File.Create(path + "1");
                }
            }
            else
            {
                File.Create(path);
            }
        }

        /// <summary>
        /// 文件删除，如果文件不存在返回false
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool FileDelete(string path)
        {
            if(File.Exists(path))
            {
                File.Delete(path);
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 文件复制
        /// </summary>
        /// <param name="path"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public bool FileCopy(string path,string destination)
        {
            if(File.Exists(path))
            {
                if(Directory.Exists(destination))
                {
                    File.Copy(path, destination);
                    return true;
                }
                else
                {
                    Directory.CreateDirectory(destination);
                    File.Copy(path, destination);
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
