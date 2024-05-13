namespace MassEffectModBuilder.UtilityTasks
{
    public class CleanDlcDirectory : IModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            // delete all files in the root output directory
            foreach (var file in Directory.EnumerateFiles(context.ModOutputPathBase))
            {
                File.Delete(file);
            }
            // delete all folders (except mergemods)
            foreach (var dir in Directory.EnumerateDirectories(context.ModOutputPathBase))
            {
                if (dir == context.MergeModsFolder) { continue; }
                Directory.Delete(dir, true);
            }
            // create some directories it expects to be there
            Directory.CreateDirectory(context.DLCBaseFolder);
            Directory.CreateDirectory(context.CookedPCConsoleFolder);
        }
    }
}
