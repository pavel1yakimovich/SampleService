using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServiceLibrary.CustomSection
{
    public class Element : ConfigurationElement
    {
        [ConfigurationProperty("ip", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string ip
        {
            get { return (string)base["ip"]; }
            set { base["ip"] = value; }
        }

        [ConfigurationProperty("port", DefaultValue = 0, IsKey = false, IsRequired = true)]
        public int port
        {
            get { return (int)base["port"]; }
            set { base["port"] = value; }
        }
    }
}
