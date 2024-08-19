using System;
using System.Xml.Serialization;

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
            try
            {
                version = new Version(value);
            } catch
            {
                version = new Version("0.0.0.0");
            }
            
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