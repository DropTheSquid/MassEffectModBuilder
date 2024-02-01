using LegendaryExplorerCore.Packages;
using LegendaryExplorerCore.Packages.CloningImportingAndRelinking;
using MassEffectModBuilder.LEXHelpers;

namespace MassEffectModBuilder.DLCTasks
{
    public record class PortAssetsIntoFile(Func<ModBuilderContext, IMEPackage> GetTargetPackageFunc, params string[] ResourceFiles) : IModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            var targetFile = GetTargetPackageFunc(context);

            var resourcePackages = MEPackageHandler.OpenMEPackages(ResourceFiles);

            foreach ( var resourcePackage in resourcePackages )
            {
                var referencer = resourcePackage.GetObjectReferencer();
                if (referencer != null)
                {
                    var entries = referencer.GetReferencedEntries();
                    foreach(var entry in entries )
                    {
                        if (entry is ExportEntry export)
                        {
                            EntryExporter.ExportExportToPackage(export, targetFile, out var portedEntry);
                            targetFile.GetObjectReferencer()?.AddToObjectReferencer((ExportEntry)portedEntry);
                        }
                    }
                }
            }

            targetFile.Save();
        }
    }
}
