namespace MassEffectModBuilder.UtilityTasks
{
    public record class EnsureMergeModsAreGenerated(params string[] expectedM3ms) : ModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            if (expectedM3ms.Length != 0 && !Directory.Exists(context.MergeModsFolder))
            {
                throw new InvalidOperationException("You must run the merge generator before you run this generator");
            }
            foreach (var expectedFile in expectedM3ms)
            {
                if (!File.Exists(Path.Combine(context.MergeModsFolder, expectedFile + ".m3m"))) 
                {
                    throw new InvalidOperationException("You must compile the mergemods before running this pipeline");
                }
            }
        }
    }
}
