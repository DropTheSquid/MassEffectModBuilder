using LegendaryExplorerCore.Packages;
using MassEffectModBuilder.LEXHelpers;

namespace MassEffectModBuilder.DLCTasks
{
    public class AddClassesToStartup : ModBuilderTask
    {
        public required string RootClassFolder { get; init; }
        public void RunModTask(ModBuilderContext context)
        {
            var startup = context.GetStartupFile();

            var classesToCompile = LooseClassCompile.GetClassesFromDirectoryStructure(RootClassFolder);
            LooseClassCompile.CompileClasses(context.Game, classesToCompile, startup);

            startup.Save();
        }
    }
}
