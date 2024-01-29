using static MassEffectModBuilder.ContextHelpers.MergeMods.MergeMod;

namespace MassEffectModBuilder.MergeTasks
{
    public record class UpdateFunction(string TargetFile, string TargetM3m, string EntryName, string ScriptFileName) : IModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            if (!File.Exists(ScriptFileName))
            {
                throw new Exception($"script file {ScriptFileName} not found");
            }
            File.Copy(ScriptFileName, Path.Combine(context.MergeModsFolder, Path.GetFileName(ScriptFileName)));
            context.MergeMods.AddMergeMod(TargetM3m, TargetFile, new ScriptUpdate(EntryName, Path.GetFileName(ScriptFileName)));
        }
    }
}
