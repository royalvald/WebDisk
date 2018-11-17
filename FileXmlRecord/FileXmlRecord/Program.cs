
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FileXmlRecord
{
    class Program
    {
        static void Main(string[] args)
        {
            FileToXml xml = new FileToXml(@"E:\Shadowsocks-4.1.2", @"F:\test.xml");
            
        }
    }
}
