namespace MassEffectModBuilder.UtilityTasks
{
    public class CleanMergeModDirectory : ModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            // clear out the last attempt/build, make a new directory
            if (Directory.Exists(context.MergeModsFolder))
            {
                Directory.Delete(context.MergeModsFolder, true);
            }
            Directory.CreateDirectory(context.MergeModsFolder);
        }
    }
}
