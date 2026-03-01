using LegendaryExplorerCore.Misc.ME3Tweaks;
using LegendaryExplorerCore.Packages;
using MassEffectModBuilder.LEXHelpers;
using static MassEffectModBuilder.LEXHelpers.LooseClassCompile;
using static MassEffectModBuilder.Merge.MergeBuilder;

namespace MassEffectModBuilder.Merge
{
    public record class AddNewClasses(string TargetFile, IEnumerable<ClassToCompile> Classes) : IMergeTask
    {
        public AddNewClasses(string TargetFile, params ClassToCompile[] Classes)
            : this(TargetFile, (IEnumerable<ClassToCompile>) Classes) { }

        public bool SkipMergeMod { get; set; } = false;
        public void RunMergeTask(MergeBuilderContext context)
        {
            var backupPath = ME3TweaksBackups.GetGameBackupPath(context.ModBuilderContext.Game);
            // TODO check that we are actually allowed to merge mod this file
            if (!PackageHelpers.TryGetHighestMountedOfficialFile(TargetFile, context.ModBuilderContext.Game, out var targetFilePath, backupPath))
            {
                throw new Exception($"cannot find target file {TargetFile}");
            }

            var assetFilePath = Path.Combine(context.ModBuilderContext.MergeModsFolder, Path.GetFileNameWithoutExtension(TargetFile) + "Classes.pcc");

            var targetPackage = MEPackageHandler.OpenMEPackage(targetFilePath);

            foreach (var clazz in Classes)
            {
                var existingClass = targetPackage.FindExport(clazz.ClassName);
                if (existingClass != null)
                {
                    throw new Exception($"You are trying to compile the mergemods against a non vanilla basegame; class {clazz.ClassName} already exists in {TargetFile}");
                }
                // this can be useful if you want to make other programatic modifications to the file and don't need this particular merge mod.
                if (!SkipMergeMod)
                {
                    // Tell the builder I want this as a merge mod
                    context.Builder.AddChange(TargetFile, new AssetUpdate(clazz.InstancedFullPath, clazz.InstancedFullPath, Path.GetFileName(assetFilePath), true));
                }
            }

            using IMEPackage mergePackage = MEPackageHandler.CreateAndOpenPackage(assetFilePath, context.ModBuilderContext.Game);

            CompileClasses(Classes, mergePackage, backupPath);

            mergePackage.Save();
        }
    }
}
