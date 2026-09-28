using LegendaryExplorerCore.Helpers;
using MassEffectModBuilder.Package;
using System.Security.Cryptography.X509Certificates;

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

        protected readonly List<IMergeTask> MergeBuilderTasks = [];

        public MergeBuilder AddTask(IMergeTask task)
        {
            MergeBuilderTasks.Add(task);
            return this;
        }

        public MergeBuilder AddTask(Action<MergeBuilderContext> task)
        {
            MergeBuilderTasks.Add(new CustomMergeTask(task));
            return this;
        }

        public bool IsEmpty()
        {
            return MergeBuilderTasks.IsEmpty();
        }

        private MergeBuilder AddChange(string file, MergeModChange change)
        {

            var fileEntry = Files.FirstOrDefault(x => x.TargetFile == file);
            if (fileEntry == null)
            {
                fileEntry = new MergeModFileRecord(file);
                Files.Add(fileEntry);
            }

            fileEntry.AddChange(change);
            return this;
        }

        public MergeBuilder WithScriptUpdate(string targetFile, string entryName, string scriptFilePath)
        {
            if (!File.Exists(scriptFilePath))
            {
                throw new Exception($"script file {scriptFilePath} not found");
            }
            return AddTask(context =>
            {
                File.Copy(scriptFilePath, Path.Combine(context.ModBuilderContext.MergeModsFolder, Path.GetFileName(scriptFilePath)));
                AddChange(targetFile, new ScriptUpdate(entryName, Path.GetFileName(scriptFilePath)));
            });
        }

        public MergeBuilder WithAddToClassOrReplace(string targetFile, string entryName, params string[] scriptFilePaths)
        {
            foreach (var scriptFilePath in scriptFilePaths)
            {
                if (!File.Exists(scriptFilePath))
                {
                    throw new Exception($"script file {scriptFilePath} not found");
                }
            }
            return AddTask(context =>
            {
                foreach (var scriptFilePath in scriptFilePaths)
                {
                    File.Copy(scriptFilePath, Path.Combine(context.ModBuilderContext.MergeModsFolder, Path.GetFileName(scriptFilePath)));
                }
                AddChange(targetFile, new AddToClassOrReplace(entryName, scriptFilePaths.Select(x => Path.GetFileName(x))));
            });
        }

        public MergeBuilder WithClassUpdate(string targetFile, string scriptFilePath)
        {
            if (!File.Exists(scriptFilePath))
            {
                throw new Exception($"script file {scriptFilePath} not found");
            }
            return AddTask(context =>
            {
                File.Copy(scriptFilePath, Path.Combine(context.ModBuilderContext.MergeModsFolder, Path.GetFileName(scriptFilePath)));
                AddChange(targetFile, new ClassUpdate(Path.GetFileName(scriptFilePath)));
            });
        }

        public MergeBuilder WithPropertyUpdate(string targetFile, string entryName, params PropertyUpdateEntry[] updateEntries)
        {
            return AddTask(context =>
            {
                AddChange(targetFile, new PropertyUpdates(entryName, updateEntries));
            });
        }

        public MergeBuilder WithAssetUpdate(string targetFile, MergePackageBuilder package, params AssetUpdateEntry[] assetUpdates)
        {
            return AddTask(context =>
            {
                package.Build(context.ModBuilderContext, context);
                foreach (var entry in assetUpdates)
                {
                    AddChange(targetFile, new AssetUpdate(entry.VanillaEntryName, entry.NewEntryName, package.PackageName, entry.CanMergeAsNew));
                }
            });
        }

        protected string GenerateJson(ModBuilderContext context)
        {
            return
        @$"{{
    ""game"": ""{context.Game}"",
    ""files"": [
{string.Join(",\r\n", Files.Where(x => !x.IsEmpty()).Select(x => x.GenerateFileJson()))}
    ]
}}";
        }

        private record class MergeModFileRecord(string TargetFile, bool ApplyToAllLocalizations = false)
        {
            private List<MergeModChange> ChangeList = [];

            public bool IsEmpty()
            {
                return ChangeList.IsEmpty();
            }

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

        private abstract record class MergeModChange(string EntryName)
        {
            public abstract string GenerateChangeJson();
        }

        public record class AssetUpdateEntry(string VanillaEntryName, string NewEntryName, bool CanMergeAsNew = false);

        private record class AssetUpdate(
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

        private record class ScriptUpdate(string EntryName, string ScriptFileName) : MergeModChange(EntryName)
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

        private record class AddToClassOrReplace(string EntryName, IEnumerable<string> ScriptFilenames) : MergeModChange(EntryName)
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

        private record class PropertyUpdates(string EntryName, params PropertyUpdateEntry[] Updates) : MergeModChange(EntryName)
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
        private record class ClassUpdate(string ClassName) : MergeModChange(ClassName)
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

        public void Build(MergeBuilderContext context)
        {
            foreach (var task in MergeBuilderTasks)
            {
                task.RunMergeTask(context);
            }
            File.WriteAllText(Path.Combine(context.MergeModsFolder, M3mName + ".json"), GenerateJson(context.ModBuilderContext));
        }
    }
}
