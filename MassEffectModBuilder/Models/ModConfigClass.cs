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

        protected void Add(CoalesceProperty property)
        {
            Add(property.Name, property);
        }

        protected void AddRange(params CoalesceProperty[] props)
        {
            AddRange((IEnumerable<CoalesceProperty>)props);
        }

        protected void AddRange(IEnumerable<CoalesceProperty> props)
        {
            foreach (var prop in props)
            {
                Add(prop);
            }
        }

        public string? GetSingleValue(string propertyName)
        {
            if (TryGetValue(propertyName, out var value))
            {
                if (value.Count > 1)
                {
                    throw new Exception($"you are trying to get a single property {propertyName} but it is an array");
                }
                if (value.Count == 1 && value[0].ValueType == 2)
                {
                    return value[0].Value;
                }
            }
            return null;
        }

        public string? GetStringValue(string propertyName)
        {
            return GetSingleValue(propertyName);
        }

        public void SetStringValue(string propertyName, string? value)
        {
            Remove(propertyName);
            if (value != null)
            {
                Add(new CoalesceProperty(propertyName, new CoalesceValue(value, CoalesceParseAction.Add)));
            }
        }

        public TEnum? GetEnumValue<TEnum>(string propertyName) where TEnum : struct
        {
            if (Enum.TryParse(GetStringValue(propertyName), out TEnum value))
            {
                return value;
            }
            return null;
        }

        public void SetEnumValue<TEnum>(string propertyName, TEnum? value) where TEnum : struct
        {
            Remove(propertyName);
            if (value != null)
            {
                Add(new CoalesceProperty(propertyName, new CoalesceValue(value.ToString(), CoalesceParseAction.Add)));
            }
        }

        public int? GetIntValue(string propertyName)
        {
            var rawValue = GetSingleValue(propertyName);
            if (rawValue != null)
            {
                if (int.TryParse(rawValue, out var value))
                {
                    return value;
                }
                throw new Exception($"property {propertyName} with value {rawValue} is not an int");
            }
            return null;
        }

        public void SetIntValue(string name, int? value)
        {
            SetStringValue(name, value?.ToString());
        }

        public bool? GetBoolValue(string propertyName)
        {
            var rawValue = GetSingleValue(propertyName);
            if (rawValue != null)
            {
                if (bool.TryParse(rawValue, out var value))
                {
                    return value;
                }
                throw new Exception($"property {propertyName} with value {rawValue} is not a bool");
            }
            return null;
        }

        public void SetBoolValue(string name, bool? value)
        {
            SetStringValue(name, value?.ToString());
        }

        public void SetStructValue(string propertyName, StructCoalesceValue? value)
        {
            Remove(propertyName);
            if (value != null)
            {
                Add(new CoalesceProperty(propertyName, new CoalesceValue(value.OutputValue(), CoalesceParseAction.Add)));
            }
        }

        public void AddArrayEntries(string name, params string[] values)
        {
            AddArrayEntries(name, (IEnumerable<string>)values);
        }

        public void AddArrayEntries(string name, IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                AddEntry(new CoalesceProperty(name, new CoalesceValue(value, CoalesceParseAction.AddUnique)));
            }
        }

        public string? Comment { get; set; }
    }
}
