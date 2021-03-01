using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class ClientConfig
    {

        private static ClientConfig instance = null;
        private static readonly object padlock = new object();

 
        public Client client { get; set; }
        
        public string EndTurnValue { get; set; }


        ClientConfig()
        {
        
        }

        public static ClientConfig Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new ClientConfig();
                    }
                    return instance;
                }
            }
        }
    }
}
