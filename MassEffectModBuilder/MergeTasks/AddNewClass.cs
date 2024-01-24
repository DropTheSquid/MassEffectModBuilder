using LegendaryExplorerCore.Packages;
using MassEffectModBuilder.LEXHelpers;
using static MassEffectModBuilder.MergeTasks.MergeMods.MergeMod;

namespace MassEffectModBuilder.MergeTasks
{
    public record class AddNewClass(string TargetFile, string PathToClassFile, string className, string TargetM3m) : ModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            // this needs to generate a new pcc file containing the class compiled from source
            // into the MergeMods directory of the output
            // as well as a json file for the merge mod
            // the rest can be done manually for now, but could be automated later


            // TODO ensure the class does not already exist in the target file. This will likely break things if it does. 

            // create the merge asset file
            var assetFilePath = Path.Combine(context.MergeModsFolder, Path.GetFileNameWithoutExtension(TargetFile) + "Classes.pcc");
            CreateMergeAssetFile(assetFilePath, context.Game);

            // Tell the builder I want this as a merge mod
            context.MergeMods.AddMergeMod(TargetM3m, TargetFile, new AssetUpdate(className, className, Path.GetFileName(assetFilePath), true));
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
