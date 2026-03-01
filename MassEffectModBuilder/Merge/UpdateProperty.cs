using static MassEffectModBuilder.Merge.MergeBuilder;

namespace MassEffectModBuilder.Merge
{
    public record class UpdateProperty(string TargetFile, string TargetM3m, string EntryName) : IMergeTask
    {
        public required PropertyUpdateEntry[] Updates { get; init; }
        public void RunMergeTask(MergeBuilderContext context)
        {
            foreach (var entry in Updates)
            {
                if (entry.PropertyType == PropertyType.Arrayproperty)
                {
                    if (!File.Exists(entry.Value))
                    {
                        throw new Exception($"script file {entry.Value} not found");
                    }
                    File.Copy(entry.Value, Path.Combine(context.ModBuilderContext.MergeModsFolder, Path.GetFileName(entry.Value)));
                }
            }
            context.Builder.AddChange(TargetFile, new PropertyUpdates(EntryName, Updates));
        }
    }
}
