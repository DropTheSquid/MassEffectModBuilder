using LegendaryExplorerCore.Packages;
using MassEffectModBuilder.LEXHelpers;

namespace MassEffectModBuilder.DLCTasks
{
    public class AddClassesToStartup(string RootClassFolder) : ModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            var startup = context.GetStartupFile();

            var classesToCompile = LooseClassCompile.GetClassesFromDirectoryStructure(RootClassFolder);
            LooseClassCompile.CompileClasses(context.Game, classesToCompile, startup);

            // TODO add all the classes to the object referencer...

            startup.Save();
        }
    }
}
