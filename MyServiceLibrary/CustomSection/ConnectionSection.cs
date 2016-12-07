using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServiceLibrary.CustomSection
{
    public class ConnectionSection : ConfigurationSection
    {
        [ConfigurationProperty("Servers")]
        public ServerAppearanceCollection ServerElement
        {
            get { return ((ServerAppearanceCollection)(base["Servers"])); }
            set { base["Servers"] = value; }
        }
    }
}
