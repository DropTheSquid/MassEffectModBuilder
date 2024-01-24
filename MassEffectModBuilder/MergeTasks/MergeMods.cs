using static MassEffectModBuilder.MergeTasks.MergeMods.MergeMod;

namespace MassEffectModBuilder.MergeTasks
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

            public abstract record class MergeModChange(string entryName)
            {
                public abstract string GenerateChangeJson();
            }

            public record class AssetUpdate(string vanillaEntryName, string newEntryName, string assetName, bool CanMergeAsNew = false) : MergeModChange(vanillaEntryName)
            {
                public override string GenerateChangeJson()
                {
                    return
$@"                {{
                    ""entryname"": ""{vanillaEntryName}"",
                    ""assetupdate"": {{
                        ""assetname"":""{assetName}"",
                        ""entryname"":""{newEntryName}"",
                        ""canmergeasnew"":{CanMergeAsNew.ToString().ToLower()}
                    }}
                }}";
                }
            }

            // TODO other update types
            // propertyUpdate (I will probably not use this much)
            // scriptUpdate (will use this)
            // addtoclassorreplace (will use this)
        }

    }
}
