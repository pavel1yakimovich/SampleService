using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;
using MyServiceLibrary.Interfaces;

namespace MyServiceLibrary
{
    public class XmlSaver<T> : ISaver<T>
    {
        private string fileName;

        public XmlSaver()
        {
            this.fileName = ConfigurationSettings.AppSettings.Get("xmlfilename");
        }

        public void Save(IEnumerable<T> list)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(T));

            using (FileStream fs = new FileStream(this.fileName, FileMode.OpenOrCreate))
            {
                foreach (T item in list)
                {
                    formatter.Serialize(fs, item);
                }
            }
        }
    }
}
