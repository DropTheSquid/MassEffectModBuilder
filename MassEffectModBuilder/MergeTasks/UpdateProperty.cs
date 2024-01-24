using static MassEffectModBuilder.MergeTasks.MergeMods.MergeMod;

namespace MassEffectModBuilder.MergeTasks
{
    public record class UpdateProperty(string TargetFile, string TargetM3m, string EntryName) : ModBuilderTask
    {
        public required PropertyUpdateEntry[] Updates { get; init; }
        public void RunModTask(ModBuilderContext context)
        {
            // TODO ensure that any arrayproperty updates have the script files copied over
            context.MergeMods.AddMergeMod(TargetM3m, TargetFile, new PropertyUpdates(EntryName, Updates));
        }
    }
}
