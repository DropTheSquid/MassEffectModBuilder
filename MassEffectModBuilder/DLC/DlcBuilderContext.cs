using LegendaryExplorerCore.Helpers;
using LegendaryExplorerCore.Packages;
using MassEffectModBuilder.Models;

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

        public ModBuilderContext ModContext;

        public string ModLibraryBase => ModContext.ModLibraryBase;

        public MEGame Game => ModContext.Game;

        public int MountPriority => DlcBuilder.MountPriority;

        public int? Game2ModuleNumber => DlcBuilder.Game2ModuleNumber;

        public string ModDLCName => DlcBuilder.DlcName;

        public string ModDlcFolderName => DlcBuilder.DlcFolderName;

        public string DLCBaseFolder => Path.Combine(ModContext.ModOutputBasePath, DlcBuilder.DlcFolderName);

        public string CookedFolderPath => Path.Combine(DLCBaseFolder, Game.CookedDirName());

        public string InternalName => DlcBuilder.InternalName;

        public int ModBaseTlkId => DlcBuilder.InternalNameTlkId;

        public string DefaultTfcPath => Path.Combine(CookedFolderPath, $"Textures_DLC_MOD_{ModDLCName}.tfc");

        public LE1AutoLoad? Le1AutoLoad { get; }
        public Game23MountFile? Game23Mount { get; }

        public void WithStartupPackage(string packageName)
        {
            if (Game == MEGame.LE1)
            {
                Le1AutoLoad!.GlobalPackages.Add(packageName);
            }
            else if (Game.IsGame2())
            {
                Game23Configs!.GetOrCreateConfigFile("BIOEngine.ini").GetOrCreateClass("Engine.StartupPackages").AddArrayEntries("DLCStartupPackage", [packageName]);
            }
            else if (Game.IsGame3())
            {
                var configClass = Game23Configs!.GetOrCreateConfigFile("BioEngine.xml").GetOrCreateClass("Engine.StartupPackages");
                configClass.AddArrayEntries("dlcstartuppackage", [packageName]);
                configClass.AddEntry(new LegendaryExplorerCore.Coalesced.CoalesceProperty("dlcstartuppackagename", [new StringCoalesceValue(packageName, LegendaryExplorerCore.Coalesced.CoalesceParseAction.New).ToCoalesceValue()]));
                configClass.AddArrayEntries("package", [packageName]);
            }
        }

        //public void WithTfc(string? tfcName = null)
        //{
        //    tfcName ??= $"Textures_DLC_MOD_{ModDLCName}";
        //    // ensure there is a new local tfc file
        //    var tfcPath = Path.Combine(CookedFolderPath, $"{tfcName}.tfc");
            
        //}

        // package stuff
        //private CaseInsensitiveDictionary<PackageBuilder> _packages = [];

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

        //public IEnumerable<PackageBuilder> Packages => _packages.Values;

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
    }
}
