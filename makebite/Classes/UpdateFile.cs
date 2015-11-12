using System;
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
    public UpdateData()
    {
        Version = new SerialVersion();
        URL = string.Empty;
    }
    [XmlAttribute("Version")]
    public int OldVersion { get; set; }

    [XmlElement("Version")]
    public SerialVersion Version { get; set; }

    [XmlElement("URL")]
    public string URL { get; set; }
}

[XmlType("SerialVersion")]
public class SerialVersion
{
    public SerialVersion()
    {
        Version = "0.0.0.0";
    }

    public SerialVersion(string Ver)
    {
        Version = Ver;
    }

    Version version = new Version();

    [XmlAttribute("Version")]
    public string Version
    {
        get
        {
            return version.ToString();
        }

        set
        {
            version = new Version(value);
        }
    }

    public Version AsVersion()
    {
        return version;
    }

    public string AsString()
    {
        return version.ToString();
    }
}