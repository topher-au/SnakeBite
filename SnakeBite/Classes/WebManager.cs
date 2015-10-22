using System.Threading;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeBite
{
    public static class WebManager
    {
        private const string MODLIST_URL = "http://www.xobanimot.com/mods.xml";

        public static List<WebMod> GetOnlineMods()
        {
            WebClient modListClient = new WebClient();
            List<WebMod> WebMods;

            try
            {
                string webModData = modListClient.DownloadString(new Uri(MODLIST_URL));

                XmlSerializer x = new XmlSerializer(typeof(List<WebMod>));
                StringReader s = new StringReader(webModData);
                
                XmlReader r = XmlReader.Create(s);

                WebMods = (List<WebMod>)x.Deserialize(r);

                foreach (WebMod mod in WebMods)
                {
                    mod.Description = mod.Description.Replace("\n", "\r\n");
                }
            } catch
            {
                WebMods = new List<WebMod>();
            }

            return WebMods;
        }

        public static void DownloadModFile(string SourceUrl, string DestFile)
        {
            WebClient modWebClient = new WebClient();
            modWebClient.DownloadFile(new Uri(SourceUrl), DestFile);
        }

        public static void CreateSampleData()
        {
            List<WebMod> SampleData = new List<WebMod>();
            for(int i=1;i<=1;i++)
            {
                SampleData.Add(new WebMod() { Name = i.ToString(), Author = i.ToString(), Description = i.ToString(), DownloadUrl = i.ToString(), Version = i.ToString(), Website = i.ToString(), DownloadSize = (uint)i });
            }
            XmlSerializer x = new XmlSerializer(typeof(List<WebMod>));
            using(FileStream fs = new FileStream("mods.xml",FileMode.Create))
            {
                x.Serialize(fs, SampleData);
            }
            
        }


    }

    [XmlType("WebMod")]
    public class WebMod
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Author")]
        public string Author { get; set; }

        [XmlAttribute("Version")]
        public string Version { get; set; }

        [XmlAttribute("Website")]
        public string Website { get; set; }

        [XmlAttribute("DownloadUrl")]
        public string DownloadUrl { get; set; }

        [XmlAttribute("DownloadSize")]
        public uint DownloadSize { get; set; }

        [XmlElement("Description")]
        public string Description { get; set; }
    }

}
