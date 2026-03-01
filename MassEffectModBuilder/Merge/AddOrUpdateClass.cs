using static MassEffectModBuilder.Merge.MergeBuilder;

namespace MassEffectModBuilder.Merge
{
    public record class AddOrUpdateClass(string TargetFile, string TargetM3m, string ScriptFileName) : IMergeTask
    {
        public void RunMergeTask(MergeBuilderContext context)
        {
            if (!File.Exists(ScriptFileName))
            {
                throw new Exception($"script file {ScriptFileName} not found");
            }
            File.Copy(ScriptFileName, Path.Combine(context.ModBuilderContext.MergeModsFolder, Path.GetFileName(ScriptFileName)));
            context.Builder.AddChange(TargetFile, new ClassUpdate(Path.GetFileNameWithoutExtension(ScriptFileName)));
        }
    }
}
