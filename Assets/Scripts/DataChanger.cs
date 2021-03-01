using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public static class DataChanger
    {
        public static int MaxCommandLength = 5;
        public static string BytesToString(byte[] bytes, bool isTrim)
        {
            var str = Encoding.ASCII.GetString(bytes);
            if (isTrim)
            {
                str = str.TrimEnd();
                str = str.Substring(0, str.IndexOf("\0"));

            }
            return str;
        }
        public static byte[] StringToBytes(string msg)
        {
            var bytes = Encoding.ASCII.GetBytes(msg);
            return bytes;
        }
    }
}
