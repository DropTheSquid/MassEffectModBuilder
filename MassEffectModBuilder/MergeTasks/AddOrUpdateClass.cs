using static MassEffectModBuilder.ContextHelpers.MergeMods.MergeMod;

namespace MassEffectModBuilder.MergeTasks
{
    public record class AddOrUpdateClass(string TargetFile, string TargetM3m, string ScriptFileName) : IModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            if (!File.Exists(ScriptFileName))
            {
                throw new Exception($"script file {ScriptFileName} not found");
            }
            File.Copy(ScriptFileName, Path.Combine(context.MergeModsFolder, Path.GetFileName(ScriptFileName)));
            context.MergeMods.AddMergeMod(TargetM3m, TargetFile, new ClassUpdate(Path.GetFileNameWithoutExtension(ScriptFileName)));
        }
    }
}
