using System.Xml.Serialization;

namespace Classes.Parser
{
    [XmlRoot("Address")]
    public class Address
    {
        [XmlAttribute("city")]
        public string City { get; set; }

        [XmlAttribute("street")]
        public string Street { get; set; }

        [XmlIgnore]
        public string FullAddress { get; set; }
    }
}
