using LegendaryExplorerCore.Coalesced;
using static MassEffectModBuilder.Models.StructCoalesceValue;

namespace MassEffectModBuilder.Models
{
    public interface ModBuilderCoalesceValue
    {
        string OutputValue();
        string? Comment { get; }

        CoalesceParseAction Action { get; }

        // used for the M3CD double typing where you can precede an entry with ++ or +- to add the signel typed entry into your mod's local stuff without "merging" it
        // so "-foo=bar" would remove "foo=bar" from your local config (if present) but would not affect if that entry exists in another lower mounted mod/basegame
        // "+-foo=bar" will add "-foo=bar" into your local ini/bin files, thus removing it from lower mounted/basegame config when your dlc is mounted
        // only relevant for 2 and 3. LE1 does not use this, sinc emerges happen in the basegame coalesced at install time
        // the only valid values are + (add if unique) and . (add even if it is a duplicate)
        string? DoubleType { get; }

        CoalesceValue ToCoalesceValue(CoalesceParseAction? action = null)
        {
            if (!action.HasValue)
            {
                action = Action;
            }
            return new CoalesceValue(OutputValue(), action.Value) { Comment = Comment, DoubleTypePrefix = DoubleType };
        }
    }

    public abstract record class ModBuilderCoalesceValue<V>(V Value, CoalesceParseAction Action = CoalesceParseAction.None) : ModBuilderCoalesceValue
    {
        public string? Comment { get; set; }

        public string? DoubleType { get; set; }
        public abstract string OutputValue();
    }

    public record class IntCoalesceValue(int Value, CoalesceParseAction Action = CoalesceParseAction.None) : ModBuilderCoalesceValue<int>(Value, Action)
    {
        public override string OutputValue()
        {
            return Value.ToString();
        }
    }

    public record class StringCoalesceValue(string Value, CoalesceParseAction Action = CoalesceParseAction.None) : ModBuilderCoalesceValue<string>(Value, Action)
    {
        public override string OutputValue()
        {
            return @$"""{Value}""";
        }
    }

    public record class BoolCoalesceValue(bool Value, CoalesceParseAction Action = CoalesceParseAction.None) : ModBuilderCoalesceValue<bool>(Value, Action)
    {
        public override string OutputValue()
        {
            return Value.ToString();
        }
    }

    public class StringArrayCoalesceValue : List<string>, ModBuilderCoalesceValue
    {
        public string? Comment { get; set; }

        public string? DoubleType { get; set;  }

        public CoalesceParseAction Action { get; set; } = CoalesceParseAction.None;

        public string OutputValue()
        {
            var items = new List<string>();
            foreach (var item in this)
            {
                items.Add(@$"""{item}""");
            }

            return $"({string.Join(",", items)})";
        }
    }

    // TODO arrays for other primatives as they come up

    public class StructCoalesceValue : Dictionary<string, ModBuilderCoalesceValue>, ModBuilderCoalesceValue
    {
        public class StructArrayCoalesceValue<T> : List<T>, ModBuilderCoalesceValue where T : StructCoalesceValue
        {
            public string? Comment { get; set; }

            public CoalesceParseAction Action { get; set; } = CoalesceParseAction.None;

            public string? DoubleType { get; set; }

            public string OutputValue()
            {
                var items = new List<string>();
                foreach (var item in this)
                {
                    items.Add(item.OutputValue());
                }

                return $"({string.Join(",", items)})";
            }
        }

        public CoalesceParseAction Action { get; set; } = CoalesceParseAction.None;
        public string? Comment { get; set; }

        public string? DoubleType { get; set; }
        public int? GetInt(string propertyName)
        {
            return ((IntCoalesceValue)this[propertyName])?.Value;
        }

        public void SetInt(string propertyName, int? value)
        {
            if (value.HasValue)
            {
                this[propertyName] = new IntCoalesceValue(value.Value, CoalesceParseAction.None);
            }
            else
            {
                Remove(propertyName);
            }
        }

        public string? GetString(string propertyName)
        {
            return ((StringCoalesceValue)this[propertyName])?.Value;
        }

        public void SetString(string propertyName, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Remove(propertyName);
            }
            else
            {
                this[propertyName] = new StringCoalesceValue(value, CoalesceParseAction.None);
            }
        }

        public bool? GetBool(string propertyName)
        {
            return ((BoolCoalesceValue)this[propertyName])?.Value;
        }

        public void SetBool(string propertyName, bool? value)
        {
            if (value.HasValue)
            {
                this[propertyName] = new BoolCoalesceValue(value.Value, CoalesceParseAction.None);
            }
            else
            {
                Remove(propertyName);
            }
        }

        public T? GetEnum<T>(string propertyName) where T : struct, Enum
        {
            string? rawValue = ((StringCoalesceValue)this[propertyName])?.Value;
            if (rawValue != null)
            {
                return Enum.Parse<T>(rawValue);
            }
            return null;
        }

        public void SetEnum<T>(string propertyName, T? value) where T : struct, Enum
        {
            if (value.HasValue)
            {
                this[propertyName] = new StringCoalesceValue(value.Value.ToString(), CoalesceParseAction.None);
            }
            else
            {
                Remove(propertyName);
            }
        }

        public string[]? GetStringArray(string propertyName)
        {
            return ((StringArrayCoalesceValue)this[propertyName])?.ToArray();
        }

        public void SetStringArray(string propertyName, string[]? value)
        {
            if (value != null)
            {
                StringArrayCoalesceValue stringArrayProp;
                if (TryGetValue(propertyName, out var prop) && prop is StringArrayCoalesceValue)
                {
                    stringArrayProp = (StringArrayCoalesceValue)prop;
                    stringArrayProp.Clear();
                    stringArrayProp.AddRange(value);
                }
                else
                {
                    stringArrayProp = [.. value];
                    this[propertyName] = stringArrayProp;
                }
            }
            else
            {
                Remove(propertyName);
            }
        }

        public StructArrayCoalesceValue<T>? GetStructArray<T>(string propertyName) where T : StructCoalesceValue
        {
            return ((StructArrayCoalesceValue<T>)this[propertyName]);
        }

        public void SetStructArray<T>(string propertyName, IEnumerable<T>? value) where T : StructCoalesceValue
        {
            if (value != null)
            {
                StructArrayCoalesceValue<T> structArrayProp;
                if (TryGetValue(propertyName, out var prop) && prop is StructArrayCoalesceValue<T>)
                {
                    structArrayProp = (StructArrayCoalesceValue<T>)prop;
                    structArrayProp.Clear();
                    structArrayProp.AddRange(value);
                }
                else
                {
                    structArrayProp = [.. value];
                    this[propertyName] = structArrayProp;
                }
            }
            else
            {
                Remove(propertyName);
            }
        }

        public T? GetStruct<T>(string propertyName) where T : StructCoalesceValue
        {
            return (T)this[propertyName];
        }

        public void SetStruct<T>(string propertyName, T? value) where T : StructCoalesceValue
        {
            if (value != null)
            {
                this[propertyName] = value;
            }
            else
            {
                Remove(propertyName);
            }
        }

        public string OutputValue()
        {
            var items = new List<string>();
            foreach (var item in this)
            {
                items.Add(item.Key + "=" + item.Value.OutputValue());
            }

            return $"({string.Join(",", items)})";
        }
    }
}
