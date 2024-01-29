using LegendaryExplorerCore.Packages;
using LegendaryExplorerCore.Packages.CloningImportingAndRelinking;
using MassEffectModBuilder.LEXHelpers;

namespace MassEffectModBuilder.DLCTasks
{
    public class AddMergeClassesToFile(string basegameTargetFile, string className, Func<ModBuilderContext, IMEPackage> packageFunc) : IModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            // add the class into the file to ensure the game doesn't crash if the basegame changes are uninstalled
            var file = packageFunc(context);
            var assetFilePath = Path.Combine(context.MergeModsFolder, Path.GetFileNameWithoutExtension(basegameTargetFile) + "Classes.pcc");

            var mergeAssetFile = MEPackageHandler.OpenMEPackages([assetFilePath]).Single();

            // creates a properly formed import under which to put the basegame classes
            var newExport = ExportCreator.CreatePackageExport(file, Path.GetFileNameWithoutExtension(basegameTargetFile));
            var basegameImport = EntryImporter.ConvertExportToImport(newExport);

            var classToPort = mergeAssetFile.FindExport(className);
            EntryExporter.ExportExportToPackage(classToPort, file, out var portedClass);

            foreach (var (reference, _) in portedClass.GetEntriesThatReferenceThisOne())
            {
                var root = reference.GetRootEntry();
                if (root != basegameImport)
                {
                    root.Parent = basegameImport;
                }
            }

            file.Save();
        }
    }
}
