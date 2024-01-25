using static MassEffectModBuilder.MergeTasks.MergeMods.MergeMod;

namespace MassEffectModBuilder.MergeTasks
{
    public record class AddOrReplaceOnClass(string TargetFile, string TargetM3m, string EntryName, params string[] ScriptFilenames) : ModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            foreach (var scriptFile in ScriptFilenames)
            {
                if (!File.Exists(scriptFile))
                {
                    throw new Exception($"merge mod script {scriptFile} does not exist.");
                }
                File.Copy(scriptFile, Path.Combine(context.MergeModsFolder, Path.GetFileName(scriptFile)), true );
            }
            context.MergeMods.AddMergeMod(TargetM3m, TargetFile, new AddToClassOrReplace(EntryName, ScriptFilenames));
        }
    }
}
