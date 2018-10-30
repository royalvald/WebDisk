using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WebDisk
{
    class DirectoryOperate
    {
        /// <summary>
        /// 文件夹创建
        /// </summary>
        /// <param name="path"></param>
        public void DirectoryCreat(string path)
        {
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 文件夹删除，如果option为true则删除子目录下所有文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public bool DirectoryDelete(string path,bool option)
        {
            if (!Directory.Exists(path))
                return false;
            else
            {
                Directory.Delete(path, true);
                return true;
            }
        }

        public bool DirectoryCopy(string source,string destination)
        {
            if (Directory.Exists(source))
            {
                DirCopy(source, destination, true);
                return true;
            }
            else
                return false;
        }

        private void DirCopy(string source,string des,bool overwrite)
        {
            string desDir = Path.Combine(des, Path.GetFileName(source));
            foreach (var item in Directory.GetFiles(source))
            {
                File.Copy(source, Path.Combine(desDir, Path.GetFileName(source)), overwrite);
            }
            foreach (var item in Directory.GetDirectories(source))
            {
                DirCopy(item, desDir, overwrite);
            }
        }
    }
}
