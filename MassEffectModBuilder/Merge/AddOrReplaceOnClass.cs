using static MassEffectModBuilder.Merge.MergeBuilder;

namespace MassEffectModBuilder.Merge
{
    public record class AddOrReplaceOnClass(string TargetFile, string TargetM3m, string EntryName, params string[] ScriptFilenames) : IMergeTask
    {
        public void RunMergeTask(MergeBuilderContext context)
        {
            foreach (var scriptFile in ScriptFilenames)
            {
                if (!File.Exists(scriptFile))
                {
                    throw new Exception($"merge mod script {scriptFile} does not exist.");
                }
                File.Copy(scriptFile, Path.Combine(context.ModBuilderContext.MergeModsFolder, Path.GetFileName(scriptFile)), true );
            }
            context.Builder.AddChange(TargetFile, new AddToClassOrReplace(EntryName, ScriptFilenames.Select(x => Path.GetFileName(x)).ToArray()));
        }
    }
}
