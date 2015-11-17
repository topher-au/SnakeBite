using System.Drawing;
using System.Xml.Serialization;

namespace ThemeXml
{
    [XmlType("Theme")]
    public class Theme
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlElement("BaseColour")]
        public XmlColour BaseColour { get; set; }

        [XmlElement("HoverColour")]
        public XmlColour HoverColour { get; set; }

        [XmlElement("ExitColour")]
        public XmlColour ExitColour { get; set; }
    }

    [XmlType("Colour")]
    public class XmlColour
    {
        public XmlColour()
        {
            alpha = 0;
            red = 0;
            blue = 0;
            green = 0;
        }

        public XmlColour(Color color)
        {
            alpha = color.A;
            red = color.R;
            blue = color.B;
            green = color.G;
        }

        [XmlAttribute("A")]
        public int alpha;

        [XmlAttribute("R")]
        public int red;

        [XmlAttribute("B")]
        public int blue;

        [XmlAttribute("G")]
        public int green;
    }
}
