using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class CheesServerData
    {

        public string Type { get; set; }
        public string Move { get; set; }
        public string command { get; set; }
        public int BytesReceived { get; set; }
        public byte[] Bytes { get; set; }
        public CheesServerData()
        {
            this.Bytes = new byte[DataChanger.MaxCommandLength];
        }
        public void loadData(byte[] bytes, int bytesIN)
        {
            Array.Copy(bytes, 0, this.Bytes, this.BytesReceived, bytesIN);
            this.BytesReceived += bytesIN;
        }

        public void prepareMessage()
        {
            byte[] commandBytes = new byte[5];
            Array.Copy(this.Bytes, 0, commandBytes, 0, 5);
            this.Type = DataChanger.BytesToString(this.Bytes,false);
        }
    }
}
