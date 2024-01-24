using static MassEffectModBuilder.MergeTasks.MergeMods.MergeMod;

namespace MassEffectModBuilder.MergeTasks
{
    public record class AddOrReplaceOnClass(string TargetFile, string TargetM3m, string EntryName, params string[] ScriptFilenames) : ModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            // TODO ensure the script files exist, copy them over
            context.MergeMods.AddMergeMod(TargetM3m, TargetFile, new AddToClassOrReplace(EntryName, ScriptFilenames));
        }
    }
}
