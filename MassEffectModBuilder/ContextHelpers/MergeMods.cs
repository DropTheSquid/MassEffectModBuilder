using static MassEffectModBuilder.ContextHelpers.MergeMods.MergeMod;

namespace MassEffectModBuilder.ContextHelpers
{
    public class MergeMods
    {
        private readonly List<MergeMod> mods = [];

        public void AddMergeMod(string m3mName, string file, MergeModChange change)
        {
            var mergeMod = mods.FirstOrDefault(mods => mods.M3mName == m3mName);
            if (mergeMod == null)
            {
                mergeMod = new MergeMod(m3mName);
                mods.Add(mergeMod);
            }
            mergeMod.AddChange(file, change);
        }

        public void CreateJsonFiles(ModBuilderContext context)
        {
            foreach (var mod in mods)
            {
                File.WriteAllText(Path.Combine(context.MergeModsFolder, mod.M3mName + ".json"), mod.GenerateJson(context));
            }
        }

        public record class MergeMod(string M3mName)
        {
            private readonly List<MergeModFileRecord> Files = [];

            public void AddChange(string file, MergeModChange change)
            {
                var fileEntry = Files.FirstOrDefault(x => x.TargetFile == file);
                if (fileEntry == null)
                {
                    fileEntry = new MergeModFileRecord(file);
                    Files.Add(fileEntry);
                }

                fileEntry.AddChange(change);
            }

            public string GenerateJson(ModBuilderContext context)
            {
                return
@$"{{
    ""game"": ""{context.Game}"",
    ""files"": [
{string.Join(",\r\n", Files.Select(x => x.GenerateFileJson()))}
    ]
}}";
            }

            public record class MergeModFileRecord(string TargetFile, bool ApplyToAllLocalizations = false)
            {
                private List<MergeModChange> ChangeList = [];

                public void AddChange(MergeModChange change)
                {
                    ChangeList.Add(change);
                }
                public string GenerateFileJson()
                {
                    // TODO include the localizations bool
                    return
$@"        {{
            ""filename"": ""{TargetFile}"",
            ""changes"": [
{string.Join(",\r\n", ChangeList.Select(x => x.GenerateChangeJson()))}
            ]
        }}";
                }
            }

            public abstract record class MergeModChange(string EntryName)
            {
                public abstract string GenerateChangeJson();
            }

            public record class AssetUpdate(
                string VanillaEntryName,
                string NewEntryName,
                string AssetFileName,
                bool CanMergeAsNew = false) : MergeModChange(VanillaEntryName)
            {
                public override string GenerateChangeJson()
                {
                    return
$@"                {{
                    ""entryname"": ""{VanillaEntryName}"",
                    ""assetupdate"": {{
                        ""assetname"":""{AssetFileName}"",
                        ""entryname"":""{NewEntryName}"",
                        ""canmergeasnew"":{CanMergeAsNew.ToString().ToLower()}
                    }}
                }}";
                }
            }

            public record class ScriptUpdate(string EntryName, string ScriptFileName) : MergeModChange(EntryName)
            {
                public override string GenerateChangeJson()
                {
                    return
$@"                {{
                    ""entryname"": ""{EntryName}"",
                    ""scriptupdate"": {{
                        ""scriptfilename"":""{ScriptFileName}""
                    }}
                }}";
                }
            }

            public record class AddToClassOrReplace(string EntryName, params string[] ScriptFilenames) : MergeModChange(EntryName)
            {
                public override string GenerateChangeJson()
                {
                    return
$@"                {{
                    ""entryname"": ""{EntryName}"",
                    ""addtoclassorreplace"": {{
                        ""scriptfilenames"": [
                            {string.Join(",/r/n                            ", ScriptFilenames.Select(x => $@"""{x}"""))}
                        ]
                    }}
                }}";
                }
            }

            public record class PropertyUpdates(string EntryName, params PropertyUpdateEntry[] Updates) : MergeModChange(EntryName)
            {

                public override string GenerateChangeJson()
                {
                    return
$@"                {{
                    ""entryname"": ""{EntryName}"",
                    ""propertyupdates"": {{
                        ""scriptfilenames"": [
                            {string.Join(",/r/n                            ", Updates.Select(x => x.GenerateJson()))}
                        ]
                    }}
                }}";
                }
            }

            // MM9+ only
            public record class ClassUpdate(string ClassName) : MergeModChange(ClassName)
            {
                public override string GenerateChangeJson()
                {
                    return
$@"                {{
                   ""entryname"": ""{EntryName}"",
                   ""classupdate"": {{
                       ""assetname"":""{ClassName}.uc""
                   }}
                }}";
                }
            }

            public enum PropertyType
            {
                BoolProperty,
                FloatProperty,
                IntProperty,
                Stringproperty,
                NameProperty,
                EnumProperty,
                ObjectProperty,
                Arrayproperty
            }

            public record class PropertyUpdateEntry(string PropertyName, PropertyType PropertyType, string Value)
            {
                public string GenerateJson()
                {
                    return
$@"{{
                            ""propertyname"": ""{PropertyName}"",
                            ""propertytype"": ""{PropertyType}"",
                            ""{(PropertyType == PropertyType.Arrayproperty ? "propertyasset" : "propertyvalue")}"": ""{Value}""
                        }},";
                }
            }

            //public record class BoolPropertyUpdate(string PropertyName, bool Value) : PropertyUpdateEntry(PropertyName, "BoolProperty")
            //{
            //   protected override List<(string, string)> AdditionalProperties => [("propertyvalue", Value.ToString())];
            //}

            //public record class FloatPropertyUpdate(string PropertyName, float Value) : PropertyUpdateEntry(PropertyName, "FloatProperty")
            //{
            //    protected override List<(string, string)> AdditionalProperties => [("propertyvalue", Value.ToString())];
            //}

            //public record class IntPropertyUpdate(string PropertyName, int Value) : PropertyUpdateEntry(PropertyName, "IntProperty")
            //{
            //    protected override List<(string, string)> AdditionalProperties => [("propertyvalue", Value.ToString())];
            //}

            //public record class StringPropertyUpdate(string PropertyName, string Value) : PropertyUpdateEntry(PropertyName, "StringProperty")
            //{
            //    protected override List<(string, string)> AdditionalProperties => [("propertyvalue", Value)];
            //}

            //public record class NamePropertyUpdate(string PropertyName, string Value, int Index = 0) : PropertyUpdateEntry(PropertyName, "NameProperty")
            //{
            //    protected override List<(string, string)> AdditionalProperties => [("propertyvalue", Value)];
            //}

            //public record class EnumPropertyUpdate(string PropertyName, string Value) : PropertyUpdateEntry(PropertyName, "EnumProperty")
            //{
            //    protected override List<(string, string)> AdditionalProperties => [("propertyvalue", Value)];
            //}

            //public record class ObjectPropertyUpdate(string PropertyName, string Value) : PropertyUpdateEntry(PropertyName, "ObjectProperty")
            //{
            //    protected override List<(string, string)> AdditionalProperties => [("propertyvalue", Value)];
            //}

            //public record class ArrayPropertyUpdate(string PropertyName, string ScriptFilename) : PropertyUpdateEntry(PropertyName, "ArrayProperty")
            //{
            //    protected override List<(string, string)> AdditionalProperties => [("propertyasset", ScriptFilename)];
            //}
        }
    }
}
