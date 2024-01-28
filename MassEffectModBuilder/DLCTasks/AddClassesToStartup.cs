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

            List<string> addedClassPaths = [];
            foreach (var package in classesToCompile)
            {
                foreach (var addedClass in package.Classes) 
                {
                    if (package.packagePath.Any())
                    {
                        addedClassPaths.Add(string.Join(".", package.packagePath) + "." + addedClass.ClassName);
                    }
                    else
                    {
                        addedClassPaths.Add(addedClass.ClassName);
                    }
                }
            }

            List<ExportEntry> newClassExports = [];
            foreach (var classPath in addedClassPaths)
            {
                var newClassExport = startup.FindExport(classPath);
                newClassExports.Add(newClassExport);
            }

            startup.AddToObjectReferencer([.. newClassExports]);

            startup.Save();
        }
    }
}
