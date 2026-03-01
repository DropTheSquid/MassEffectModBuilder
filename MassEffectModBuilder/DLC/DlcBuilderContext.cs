using LegendaryExplorerCore.Misc;
using LegendaryExplorerCore.Packages;
using MassEffectModBuilder.Models;
using MassEffectModBuilder.Package;

namespace MassEffectModBuilder.DLC
{
    public class DlcBuilderContext
    {
        public DlcBuilderContext(DlcBuilder builder, ModBuilderContext modContext)
        {
            DlcBuilder = builder;
            ModContext = modContext;
            TlkBuilder = new TlkBuilder(modContext.Game);
            if (Game == MEGame.LE1)
            {
                Le1AutoLoad = new LE1AutoLoad(ModBaseTlkId, InternalName, MountPriority);
            }
            if (Game.IsGame2() || Game.IsGame3())
            {
                Game23Configs = new Game23Configs();
                Game23Mount = new Game23MountFile(Game, MountPriority, ModBaseTlkId, InternalName, ModDlcFolderName);
            }
        }

        protected DlcBuilder DlcBuilder { get; }

        protected ModBuilderContext ModContext;

        public MEGame Game => ModContext.Game;

        public int MountPriority => DlcBuilder.MountPriority;

        public int? Game2ModuleNumber => DlcBuilder.Game2ModuleNumber;

        public string ModDLCName => DlcBuilder.DlcName;

        public string ModDlcFolderName => DlcBuilder.DlcFolderName;

        public string DLCBaseFolder => Path.Combine(ModContext.ModOutputBasePath, DlcBuilder.DlcFolderName);

        public string CookedFolderPath => Path.Combine(DLCBaseFolder, Game.CookedDirName());

        public string InternalName => DlcBuilder.InternalName;

        public int ModBaseTlkId => DlcBuilder.InternalNameTlkId;

        public LE1AutoLoad? Le1AutoLoad { get; }
        public Game23MountFile? Game23Mount { get; }

        // package stuff
        private CaseInsensitiveDictionary<PackageBuilder> _packages = [];

        //public void AddPackage(PackageBuilder packagebuilder)
        //{
        //    _packages.Add(packagebuilder.PackageName, packagebuilder);
        //}

        //public PackageBuilder? GetPackage(string packageName)
        //{
        //    if (_packages.TryGetValue(packageName, out var package))
        //    { 
        //        return package;
        //    }
        //    return null;
        //}

        public IEnumerable<PackageBuilder> Packages => _packages.Values;

        // TLK stuff
        public TlkBuilder TlkBuilder { get; }

        // config merge stuff

        private readonly List<ModConfigMergeFile> _configMergeFiles = [];
        public IEnumerable<ModConfigMergeFile> ConfigMergeFiles => _configMergeFiles;
        public ModConfigMergeFile GetOrCreateConfigMergeFile(string configMergeFilename)
        {
            if (!Game.IsLEGame())
            {
                throw new Exception($"game {Game} does not support config merge");
            }
            var existing = _configMergeFiles.FirstOrDefault(x => x.OutputFileName == configMergeFilename);
            if (existing != null)
            {
                return existing;
            }

            var newFile = new ModConfigMergeFile(configMergeFilename);
            _configMergeFiles.Add(newFile);
            return newFile;
        }

        // regular config stuff (game 2 and 3 only; OT1 tbd)
        public Game23Configs? Game23Configs { get; private set; }

        // TODO let me add M3TOs to this DLC
    }
}
