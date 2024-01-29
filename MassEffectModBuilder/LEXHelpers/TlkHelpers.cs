using LegendaryExplorerCore.TLK;
using System.Xml;

namespace MassEffectModBuilder.LEXHelpers
{
    public static class TlkHelpers
    {
        public static List<TLKStringRef> ParseGame1TlkXml(string filePath)
        {
            // copied and slightly modified from LegendaryExplorerCore.TLK.ME1.HuffmanCompression.LoadXmlInputData
            var results = new List<TLKStringRef>();
            var xmlReader = new XmlTextReader(filePath);

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "string")
                {
                    int id = 0, flags = 0;
                    string data = "";
                    while (xmlReader.Name != "string" || xmlReader.NodeType != XmlNodeType.EndElement)
                    {
                        if (!xmlReader.Read() || xmlReader.NodeType != XmlNodeType.Element)
                            continue;
                        if (xmlReader.Name == "id")
                            id = xmlReader.ReadElementContentAsInt();
                        else if (xmlReader.Name == "flags")
                            flags = xmlReader.ReadElementContentAsInt();
                        else if (xmlReader.Name == "data")
                            data = xmlReader.ReadString();
                    }
                    data = data.Replace("\r\n", "\n");
                    /* every string should be NULL-terminated */
                    if (id > 0)
                        data += '\0';
                    results.Add(new TLKStringRef(id, data, flags));
                }
            }
            xmlReader.Close();
            return results;
        }

        public static List<TLKStringRef> ParseGame23TlkXml(string filePath)
        {
            // copied and slightly modified from LegendaryExplorerCore.TLK.ME2ME3.HuffmanCompression.LoadXmlInputData
            // note that this just returns it in the order it appears in the tlk. The first instance of an id is the male string.
            // The second instance, if any, is the female version
            Version? inputFileVersion = null;
            var results = new List<TLKStringRef>();

            var xmlReader = new XmlTextReader(filePath);

            /* read and store TLK Tool version, which was used to create the XML file */
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name.Equals("tlkFile", StringComparison.OrdinalIgnoreCase))
                {
                    if (xmlReader.GetAttribute("TLKToolVersion") is string toolVersion)
                    {
                        inputFileVersion = new Version(toolVersion.Trim('v'));
                    }
                    break;
                }
            }
            if (inputFileVersion == null || inputFileVersion >= new Version("2.0.12"))
            {
                int position = 0;
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name.Equals("String", StringComparison.OrdinalIgnoreCase))
                    {
                        int id = 0;
                        if (!int.TryParse(xmlReader.GetAttribute("id"), out id))
                        {
                            throw new XmlException("id not an integer.", null, xmlReader.LineNumber, xmlReader.LinePosition);
                        }
                        string data = xmlReader.ReadElementContentAsString();

                        data = data.Replace("\r\n", "\n");
                        /* every string should be NULL-terminated */
                        if (id >= 0)
                            data += '\0';
                        
                        results.Add(new TLKStringRef(id, data, position, position));
                        position++;
                    }
                }
            }
            else //legacy support
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name.Equals("string", StringComparison.OrdinalIgnoreCase))
                    {
                        int id = 0, position = 0;
                        string data = "";
                        while (xmlReader.Name != "string" || xmlReader.NodeType != XmlNodeType.EndElement)
                        {
                            if (!xmlReader.Read() || xmlReader.NodeType != XmlNodeType.Element)
                                continue;
                            if (xmlReader.Name == "id")
                                id = xmlReader.ReadElementContentAsInt();
                            else if (xmlReader.Name == "position")
                                position = xmlReader.ReadElementContentAsInt();
                            else if (xmlReader.Name == "data")
                                data = xmlReader.ReadString();
                        }
                        data = data.Replace("\r\n", "\n");
                        /* every string should be NULL-terminated */
                        if (id >= 0)
                            data += '\0';
                        
                        results.Add(new TLKStringRef(id, data, 0, position));
                    }
                }
            }
            xmlReader.Close();
            return results;
        }
    }
}
