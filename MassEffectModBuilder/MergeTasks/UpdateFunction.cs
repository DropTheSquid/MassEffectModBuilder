using static MassEffectModBuilder.MergeTasks.MergeMods.MergeMod;

namespace MassEffectModBuilder.MergeTasks
{
    public record class UpdateFunction(string TargetFile, string TargetM3m, string EntryName, string ScriptFileName) : ModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            // TODO copy script from working directory to merge mod folder
            context.MergeMods.AddMergeMod(TargetM3m, TargetFile, new ScriptUpdate(EntryName, ScriptFileName));
        }
    }
}
