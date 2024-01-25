namespace MassEffectModBuilder.UtilityTasks
{
    public class CleanDlcDirectory : ModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            // clear out files directly in the root of the mod
            foreach (var file in Directory.EnumerateFiles(context.ModOutputPathBase))
            {
                File.Delete(file);
            }
            if (Directory.Exists(context.DLCBaseFolder))
            {
                Directory.Delete(context.DLCBaseFolder, true);
            }
            Directory.CreateDirectory(context.DLCBaseFolder);
            Directory.CreateDirectory(context.CookedPCConsoleFolder);
        }
    }
}
