using LegendaryExplorerCore.Packages;

namespace MassEffectModBuilder.DLCTasks
{
    public class InitializeStartup : ModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            var startup = context.GetStartupFile();

            ExportCreator.CreateExport(startup, "CombinedStartupReferencer", "ObjectReferencer", indexed: false);

            startup.Save();
        }
    }
}
