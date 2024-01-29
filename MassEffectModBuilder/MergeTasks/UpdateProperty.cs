using static MassEffectModBuilder.ContextHelpers.MergeMods.MergeMod;

namespace MassEffectModBuilder.MergeTasks
{
    public record class UpdateProperty(string TargetFile, string TargetM3m, string EntryName) : IModBuilderTask
    {
        public required PropertyUpdateEntry[] Updates { get; init; }
        public void RunModTask(ModBuilderContext context)
        {
            foreach (var entry in Updates)
            {
                if (entry.PropertyType == PropertyType.Arrayproperty)
                {
                    if (!File.Exists(entry.Value))
                    {
                        throw new Exception($"script file {entry.Value} not found");
                    }
                    File.Copy(entry.Value, Path.Combine(context.MergeModsFolder, Path.GetFileName(entry.Value)));
                }
            }
            context.MergeMods.AddMergeMod(TargetM3m, TargetFile, new PropertyUpdates(EntryName, Updates));
        }
    }
}
