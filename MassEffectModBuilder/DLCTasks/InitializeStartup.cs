﻿using LegendaryExplorerCore.Packages;
using MassEffectModBuilder.LEXHelpers;

namespace MassEffectModBuilder.DLCTasks
{
    public class InitializeStartup : IModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            var startup = context.GetStartupFile();

            startup.AddObjectReferencer("CombinedStartupReferencer", false);

            startup.Save();
        }
    }
}
