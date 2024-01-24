using static MassEffectModBuilder.MergeTasks.MergeMods.MergeMod;

namespace MassEffectModBuilder.MergeTasks
{
    // for all asset updates that are not adding a new class; ie replacing or adding new non class assets given a PCC
    public record class UpdateAsset(string TargetFile, string TargetM3m, string VanillaEntryName, string NewEntryName, string AssetFileName, bool canMergeAsNew = false) : ModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            // TODO copy the asset file over
            context.MergeMods.AddMergeMod(TargetM3m, TargetFile, new AssetUpdate(VanillaEntryName, NewEntryName, AssetFileName, canMergeAsNew));
        }
    }
}
