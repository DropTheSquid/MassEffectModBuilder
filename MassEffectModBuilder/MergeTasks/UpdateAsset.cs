using static MassEffectModBuilder.MergeTasks.MergeMods.MergeMod;

namespace MassEffectModBuilder.MergeTasks
{
    // for all asset updates that are not adding a new class; ie replacing or adding new non class assets given a PCC
    public record class UpdateAsset(string TargetFile, string TargetM3m, string VanillaEntryName, string NewEntryName, string AssetFileName, bool CanMergeAsNew = false) : ModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            if (!File.Exists(AssetFileName))
            {
                throw new Exception($"asset file {AssetFileName} not found");
            }
            var destinationPath = Path.Combine(context.MergeModsFolder, Path.GetFileName(AssetFileName));
            // if it is already in the destination folder, don't try to copy it over itself
            if (destinationPath  != AssetFileName)
            {
                File.Copy(AssetFileName, destinationPath);
            }
            context.MergeMods.AddMergeMod(TargetM3m, TargetFile, new AssetUpdate(VanillaEntryName, NewEntryName, Path.GetFileName(AssetFileName), CanMergeAsNew));
        }
    }
}
