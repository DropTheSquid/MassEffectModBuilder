using LegendaryExplorerCore.GameFilesystem;
using LegendaryExplorerCore.Packages;
using MassEffectModBuilder.LEXHelpers;
using static MassEffectModBuilder.MergeTasks.MergeMods.MergeMod;

namespace MassEffectModBuilder.MergeTasks
{
    public record class AddNewClass(string TargetFile, string PathToClassFile, string ClassName, string TargetM3m) : ModBuilderTask
    {
        public bool SkipMergeMod { get; set; } = false;
        public void RunModTask(ModBuilderContext context)
        {
            // this needs to generate a new pcc file containing the class compiled from source
            // into the MergeMods directory of the output
            // as well as a json file for the merge mod
            // the rest can be done manually for now, but could be automated later

            if (!MELoadedFiles.TryGetHighestMountedFile(context.Game, TargetFile, out var targetFilePath))
            {
                throw new Exception($"cannot find target file {TargetFile}");
            }
            var targetPackage = MEPackageHandler.OpenMEPackage(targetFilePath);
            var existingClass = targetPackage.FindExport(ClassName);
            if (existingClass != null)
            {
                throw new Exception($"You are trying to compile the mergemods against a non vanilla basegame; class {ClassName} already exists in {TargetFile}");
            }
            if (!File.Exists(PathToClassFile))
            {
                throw new Exception($"script file {PathToClassFile} not found");
            }

            // create the merge asset file
            var assetFilePath = Path.Combine(context.MergeModsFolder, Path.GetFileNameWithoutExtension(TargetFile) + "Classes.pcc");
            CreateMergeAssetFile(assetFilePath, context.Game);

            // this can be useful if you want to make other programatic modifications to the file and don't need this particular merge mod. 
            if (!SkipMergeMod)
            {
                // Tell the builder I want this as a merge mod
                context.MergeMods.AddMergeMod(TargetM3m, TargetFile, new AssetUpdate(ClassName, ClassName, Path.GetFileName(assetFilePath), true));
            }
        }

        private IMEPackage CreateMergeAssetFile(string stagingFilePath, MEGame game)
        {
            using IMEPackage stagingPackage = MEPackageHandler.CreateAndOpenPackage(stagingFilePath, game);

            var singleClass = LooseClassCompile.GetSingleClassForCompile(PathToClassFile);
            LooseClassCompile.CompileClasses(game, singleClass, stagingPackage);

            stagingPackage.Save();

            return stagingPackage;
        }
    }
}
