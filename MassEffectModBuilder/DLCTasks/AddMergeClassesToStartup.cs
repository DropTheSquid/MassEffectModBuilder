using LegendaryExplorerCore.Packages;
using LegendaryExplorerCore.Packages.CloningImportingAndRelinking;
using MassEffectModBuilder.LEXHelpers;

namespace MassEffectModBuilder.DLCTasks
{
    public class AddMergeClassesToStartup(string basegameTargetFile, string className) : ModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            // add the class into the startup file to ensure the game doesn't crash if the basegame changes are uninstalled
            var startupFile = context.GetStartupFile();
            var assetFilePath = Path.Combine(context.MergeModsFolder, Path.GetFileNameWithoutExtension(basegameTargetFile) + "Classes.pcc");

            var mergeAssetFile = MEPackageHandler.OpenMEPackages([assetFilePath]).Single();

            // creates a properly formed import under which to put the basegame classes
            var newExport = ExportCreator.CreatePackageExport(startupFile, Path.GetFileNameWithoutExtension(basegameTargetFile));
            var basegameImport = EntryImporter.ConvertExportToImport(newExport);

            var classToPort = mergeAssetFile.FindExport(className);
            EntryExporter.ExportExportToPackage(classToPort, startupFile, out var portedClass);

            foreach (var (reference, _) in portedClass.GetEntriesThatReferenceThisOne())
            {
                var root = reference.GetRootEntry();
                if (root != basegameImport)
                {
                    root.Parent = basegameImport;
                }
            }

            startupFile.Save();
        }
    }
}
