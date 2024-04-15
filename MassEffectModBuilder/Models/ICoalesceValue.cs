using LegendaryExplorerCore.Coalesced;

namespace MassEffectModBuilder.Models
{
    public interface ICoalesceValue
    {
        string OutputValue();

        CoalesceValue ToCoalesceValue(CoalesceParseAction type)
        {
            return new CoalesceValue(OutputValue(), type);
        }
    }

    public record class IntCoalesceValue(int Value) : ICoalesceValue
    {
        public string OutputValue()
        {
            return Value.ToString();
        }
    }

    public record class StringCoalesceValue(string Value) : ICoalesceValue
    {
        public string OutputValue()
        {
            return @$"""{Value}""";
        }
    }

    public record class BoolCoalesceValue(bool Value) : ICoalesceValue
    {
        public string OutputValue()
        {
            return Value.ToString();
        }
    }

    public record class StringArrayCoalesceValue(string[] Value) : ICoalesceValue
    {
        public string OutputValue()
        {
            var items = new List<string>();
            foreach (var item in Value)
            {
                items.Add(@$"""{item}""");
            }

            return $"({string.Join(",", items)})";
        }
    }

    public class StructCoalesceValue : Dictionary<string, ICoalesceValue>, ICoalesceValue
    {
        public int? GetInt(string propertyName)
        {
            return ((IntCoalesceValue)this[propertyName])?.Value;
        }

        public void SetInt(string propertyName, int? value)
        {
            if (value.HasValue)
            {
                this[propertyName] = new IntCoalesceValue(value.Value);
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
                this[propertyName] = new StringCoalesceValue(value);
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
                this[propertyName] = new BoolCoalesceValue(value.Value);
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
                this[propertyName] = new StringCoalesceValue(value.Value.ToString());
            }
            else
            {
                Remove(propertyName);
            }
        }

        public string[]? GetStringArray(string propertyName)
        {
            return ((StringArrayCoalesceValue)this[propertyName])?.Value;
        }

        public void SetStringArray(string propertyName, string[]? value)
        {
            if (value != null)
            {
                this[propertyName] = new StringArrayCoalesceValue(value);
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
