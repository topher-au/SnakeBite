﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Serialization;

namespace SnakeBite
{
    public static class WebManager
    {
        private const string MODLIST_URL = "http://www.xobanimot.com/snakebite/webmod/mods.xml";

        public static List<WebMod> GetOnlineMods()
        {
            Debug.LogLine("[Web] Fetching online mod list");
            WebClient modListClient = new WebClient();
            modListClient.Headers.Add("User-Agent", string.Format("{0}/SnakeBite-{1}", Environment.OSVersion.VersionString, ModManager.GetSBVersion()));

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
                Debug.LogLine("[Web] Download successful");
            }
            catch
            {
                Debug.LogLine("[Web] Download failed", Debug.LogLevel.Debug);
                WebMods = new List<WebMod>();
            }

            return WebMods;
        }

        public static void DownloadModFile(string SourceUrl, string DestFile)
        {
            Debug.LogLine(String.Format("[Web] Downloading {0}", SourceUrl));
            WebClient modWebClient = new WebClient();
            modWebClient.Headers.Add("User-Agent", string.Format("SnakeBite/{0}", ModManager.GetSBVersion()));
            modWebClient.DownloadFile(new Uri(SourceUrl), DestFile);
        }

        public static void CreateSampleData()
        {
            List<WebMod> SampleData = new List<WebMod>();
            for (int i = 1; i <= 1; i++)
            {
                SampleData.Add(new WebMod() { Name = i.ToString(), Author = i.ToString(), Description = i.ToString(), DownloadUrl = i.ToString(), Version = i.ToString(), Website = i.ToString(), DownloadSize = (uint)i });
            }
            XmlSerializer x = new XmlSerializer(typeof(List<WebMod>));
            using (FileStream fs = new FileStream("mods.xml", FileMode.Create))
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