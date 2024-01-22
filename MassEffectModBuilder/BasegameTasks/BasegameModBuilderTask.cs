using LegendaryExplorerCore.GameFilesystem;
using LegendaryExplorerCore.Packages;
using LegendaryExplorerCore.UnrealScript;

namespace AmmBuilder.BasegameTasks
{
    public abstract class BasegameModBuilderTask : ModBuilderTask
    {
        /// <summary>
        /// Which basegame file is affected by the basegame change. It must be exactly one, and it is required. 
        /// </summary>
        public required string TargetFile { get; set; }

        /// <summary>
        /// What m3m it should end up as part of. If unspecified, it will use a default which is installed first. 
        /// </summary>
        public string TargetM3m { get; set; }

        /// <summary>
        /// Whether this basegame change is required for mod compilation; 
        /// For example, adding a new class to SFXGame.pcc or adding a new function to a class which is called from the mod's code, without which compilation of the mod code will fail.
        /// If this is false, it will only be included as part of the merge mods in the mod output. 
        /// If it is true, it will also be applied to the basegame as part of mod building so the mod code can compile.
        /// Leave this false unless you need it. Also ensure that if it is true, you test installing the generated mod on a vanilla game. 
        /// </summary>
        public bool IsRequiredForModCompilation { get; set; } = false;

        public override async Task RunModTask(ModBuilder modBuilder)
        {
            // make the actual change to the basegame file, if needed
            if (IsRequiredForModCompilation)
            {
                if (MELoadedFiles.TryGetHighestMountedFile(modBuilder.Game, TargetFile, out var filename))
                {
                    var pcc = MEPackageHandler.OpenMEPackage(filename);
                    var lib = new FileLib(pcc);
                    lib.Initialize();

                    await ApplyChange(modBuilder, lib);
                    await pcc.SaveAsync();
                }
                else
                {
                    throw new Exception($"Could not find basegame file {TargetFile} in game {modBuilder.Game}");
                }
            }
            // TODO generate something resembling a merge mod with this change in it
        }

        protected abstract Task ApplyChange(ModBuilder modBuilder, FileLib lib);
    }
}
