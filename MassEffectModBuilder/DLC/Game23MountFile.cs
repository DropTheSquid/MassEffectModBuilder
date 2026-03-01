using LegendaryExplorerCore.GameFilesystem;
using LegendaryExplorerCore.Packages;

namespace MassEffectModBuilder.DLC
{
    public class Game23MountFile(MEGame game, int mountPriority, int dlcTlkId, string modInternalName, string modDLCName)
    {
        public void OutputMountFile(string path)
        {
            MountFile mount = game switch
            {
                MEGame.ME2 or MEGame.LE2 => new MountFile
                {
                    Game = game,
                    MountPriority = mountPriority,
                    TLKID = dlcTlkId,
                    MountFlags = MountFlags,
                    ME2Only_DLCHumanName = modInternalName,
                    ME2Only_DLCFolderName = modDLCName,
                },
                MEGame.ME3 or MEGame.LE3 => new MountFile
                {
                    Game = game,
                    MountPriority = mountPriority,
                    TLKID = dlcTlkId,
                    MountFlags = MountFlags,
                },
                _ => throw new ApplicationException($"Game {game} is not applicable to this mount file type"),
            };
            mount.WriteMountFile(Path.Combine(path, "Mount.dlc"));
        }

        public MountFlag MountFlags { get; set; } = game.IsGame2() ? new MountFlag(0, true) : new MountFlag(EME3MountFileFlag.LoadsInSingleplayer);
    }
}
