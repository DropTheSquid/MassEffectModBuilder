﻿namespace MassEffectModBuilder.UtilityTasks
{
    public class CleanDlcDirectory : ModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            if (Directory.Exists(context.DLCBaseFolder))
            {
                Directory.Delete(context.DLCBaseFolder, true);
            }
            Directory.CreateDirectory(context.DLCBaseFolder);
            Directory.CreateDirectory(context.CookedPCConsoleFolder);
        }
    }
}
