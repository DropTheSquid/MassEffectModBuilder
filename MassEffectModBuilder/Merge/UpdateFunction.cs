using static MassEffectModBuilder.Merge.MergeBuilder;

namespace MassEffectModBuilder.Merge
{
    public record class UpdateFunction(string TargetFile, string TargetM3m, string EntryName, string ScriptFileName) : IMergeTask
    {
        public void RunMergeTask(MergeBuilderContext context)
        {
            if (!File.Exists(ScriptFileName))
            {
                throw new Exception($"script file {ScriptFileName} not found");
            }
            File.Copy(ScriptFileName, Path.Combine(context.ModBuilderContext.MergeModsFolder, Path.GetFileName(ScriptFileName)));
            context.Builder.AddChange(TargetFile, new ScriptUpdate(EntryName, Path.GetFileName(ScriptFileName)));
        }
    }
}
