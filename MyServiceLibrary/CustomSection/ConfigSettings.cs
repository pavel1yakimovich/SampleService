using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServiceLibrary.CustomSection
{
    public class ConfigSettings
    {
        public ConnectionSection ServerAppearanceConfiguration
        {
            get
            {
                return (ConnectionSection)ConfigurationManager.GetSection("serverSection");
            }
        }

        public ServerAppearanceCollection ServerApperances
        {
            get
            {
                return this.ServerAppearanceConfiguration.ServerElement;
            }
        }

        public IEnumerable<Element> ServerElements
        {
            get
            {
                foreach (Element selement in this.ServerApperances)
                {
                    if (selement != null)
                        yield return selement;
                }
            }
        }
    }
}
