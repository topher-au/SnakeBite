using System.IO;
using System.Net;
using System.Xml.Serialization;

[XmlType("UpdateFile")]
public class UpdateFile
{
    [XmlElement("SnakeBite")]
    public UpdateData SnakeBite { get; set; }

    [XmlElement("Updater")]
    public UpdateData Updater { get; set; }

    public UpdateFile()
    {
        SnakeBite = new UpdateData();
        Updater = new UpdateData();
    }

    public UpdateFile(string XmlFile)
    {
        SnakeBite = new UpdateData();
        Updater = new UpdateData();
        ReadXml(XmlFile);
    }

    public void ReadXml(string XmlFile)
    {
        UpdateFile readXml;
        using (StreamReader reader = new StreamReader(XmlFile))
        {
            XmlSerializer xml = new XmlSerializer(typeof(UpdateFile));
            readXml = (UpdateFile)xml.Deserialize(reader);
        }

        SnakeBite = readXml.SnakeBite;
        Updater = readXml.Updater;
    }

    public void WriteXml(string XmlFile)
    {
        using (StreamWriter writer = new StreamWriter(XmlFile))
        {
            XmlSerializer xml = new XmlSerializer(typeof(UpdateFile));
            xml.Serialize(writer, this);
        }
    }

    public bool ReadXmlFromInterweb(string URL)
    {
        WebClient xmlClient = new WebClient();
        try
        {
            string xmlData = xmlClient.DownloadString(URL);
            UpdateFile readXml;
            using (StringReader reader = new StringReader(xmlData))
            {
                XmlSerializer xml = new XmlSerializer(typeof(UpdateFile));
                readXml = (UpdateFile)xml.Deserialize(reader);
            }
            SnakeBite = readXml.SnakeBite;
            Updater = readXml.Updater;
            return true;
        }
        catch { }

        return false;
    }
}

[XmlType("UpdateData")]
public class UpdateData
{
    [XmlAttribute("Version")]
    public int Version { get; set; }

    [XmlAttribute("URL")]
    public string URL { get; set; }
}