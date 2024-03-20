using LegendaryExplorerCore.Coalesced;

namespace MassEffectModBuilder.Models
{
    public class ModConfigClass(string classFullPath, string? targetConfigFile = null) 
        : CoalesceSection(string.IsNullOrEmpty(targetConfigFile) ? classFullPath : $"{targetConfigFile} {classFullPath}")
    {
        public string ClassFullPath { get; set; } = classFullPath;

        /// <summary>
        /// The config file this class's options should merge into. Only applicable for m3cd files. 
        /// </summary>
        public string? TargetConfigFile { get; set; } = targetConfigFile;

        public override string Name
        {
            get => TargetConfigFile == null ? ClassFullPath : $"{TargetConfigFile} {ClassFullPath}";
            set
            {
                var parts = value.Split(' ');
                if (parts.Length > 1)
                {
                    TargetConfigFile = parts[0];
                    ClassFullPath = parts[1];
                }
                else
                {
                    ClassFullPath = value;
                }
            }
        }

        public void Add(CoalesceProperty property)
        {
            Add(property.Name, property);
        }

        public void AddRange(params CoalesceProperty[] props)
        {
            foreach (var prop in props)
            {
                Add(prop);
            }
        }

        public void AddRange(IEnumerable<CoalesceProperty> props)
        {
            foreach (var prop in props)
            {
                Add(prop);
            }
        }

        public void SetValue(string name, string value)
        {
            Remove(name);
            Add(new CoalesceProperty(name, new CoalesceValue(value, CoalesceParseAction.Add)));
        }

        public void SetValue(string name, int value)
        {
            SetValue(name, value.ToString());
        }

        public void AddArrayEntries(string name, params string[] values)
        {
            foreach (var value in values)
            {
                AddEntry(new CoalesceProperty(name, new CoalesceValue(value, CoalesceParseAction.AddUnique)));
            }
        }

        public void AddArrayEntries(string name, IEnumerable<string> values)
        {
            AddArrayEntries(name, values.ToArray());
        }
    }
}
