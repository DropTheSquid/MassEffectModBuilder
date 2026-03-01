using LegendaryExplorerCore.Coalesced;

namespace MassEffectModBuilder.Models
{
    /// <summary>
    /// Represents either an ME2/LE2 ini file within a mod or an ME3/LE3 xml file that will be compiled into the coalesced.bin of the mod
    /// </summary>
    /// <param name="TargetConfigFileName">the ini/xml this mod should extend</param>
    public class DlcConfigFile
    {
        protected List<ModConfigClass> ClassConfigs = [];
        public DlcConfigFile(string targetConfigFileName)
        {
            TargetConfigFileName = targetConfigFileName;
        }

        // This represents one or more of either an ME2 ini file in a DLC mod or an xml that gets compiled into the coalesced of an ME3 mod
        // does not support LE1. use configMerge version for that.
        // OT1 TBD

        public string TargetConfigFileName { get; private set; }

        public ModConfigClass GetOrCreateClass(string classFullPath)
        {
            var config = ClassConfigs.FirstOrDefault(x => x.ClassFullPath == classFullPath);
            if (config == null)
            {
                config = new ModConfigClass(classFullPath);
                ClassConfigs.Add(config);
            }
            return config;
        }

        public string ToGame2Ini()
        {
            List<string> lines = [];

            foreach (var section in ClassConfigs)
            {
                // do not output anything for empty sections
                if (section.Keys.Count == 0)
                {
                    continue;
                }
                // output the comment, if any
                if (!string.IsNullOrWhiteSpace(section.Comment))
                {
                    lines.AddRange(section.Comment.Split("\n").Select(x => $"; {x}"));
                }
                // output the header
                lines.Add($"[{section.ClassFullPath}]");
                // output each property
                foreach (var (propName, prop) in section)
                {
                    foreach (var val in prop)
                    {
                        lines.AddRange(FormatPropertyLinesGame2(propName, val));
                    }
                }
                // output an empty line at the end of a class config
                lines.Add("");
            }

            return string.Join("\n", lines);
        }

        public void OutputGame2Ini(string folder)
        {
            File.WriteAllText(Path.Combine(folder, TargetConfigFileName), ToGame2Ini());
        }

        private static IEnumerable<string> FormatPropertyLinesGame2(string propertyName, CoalesceValue value)
        {
            IEnumerable<string> comment;
            if (!string.IsNullOrWhiteSpace(value.Comment))
            {
                comment = value.Comment.Split("\n").Select(x => $"; {x}");
            }
            else
            {
                comment = [];
            }
            var prefix = GetGame2Prefix(value);
            return [.. comment, $"{prefix}{propertyName}={value.Value}"];
        }

        private static string GetGame2Prefix(CoalesceValue value)
        {
            return value.ParseAction switch
            {
                CoalesceParseAction.Remove => "-",
                CoalesceParseAction.AddUnique => "+",
                CoalesceParseAction.RemoveProperty => "!",
                _ => ""
            };
        }

        public string ToGame3Xml()
        {
            var iniName = TargetConfigFileName.ToLower().Replace(".xml", ".ini");
            var xml = new CoalesceAsset(iniName, [.. ClassConfigs.Select(x => new KeyValuePair<string, CoalesceSection>(x.ClassFullPath, x))])
            {
                Source = $@"..\..\biogame\config\{iniName}"
            };
            return xml.ToXmlString();
        }

        public void OutputGame3Xml(string folder)
        {
            File.WriteAllText(Path.Combine(folder, TargetConfigFileName), ToGame3Xml());
        }
    }
}
