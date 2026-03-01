namespace MassEffectModBuilder.Merge
{
    /// <summary>
    /// A builder for a single m3m file as part of a mod. 
    /// </summary>
    public class MergeBuilder
    {
        public required string M3mName { get; set; }

        // the files modified by this m3m
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

        private record class MergeModFileRecord(string TargetFile, bool ApplyToAllLocalizations = false)
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

        public void Build(ModBuilderContext context)
        {
             File.WriteAllText(Path.Combine(context.MergeModsFolder, M3mName + ".json"), GenerateJson(context));
        }
    }
}
