using LegendaryExplorerCore.Packages;
using MassEffectModBuilder.ContextHelpers;
using MassEffectModBuilder.Models;

namespace MassEffectModBuilder
{
    public class ModBuilderContext(ModBuilder builder)
    {
        private readonly ModBuilder Builder = builder;

        public MEGame Game => Builder.Game;

        public string ModOutputPathBase => Builder.ModOutputPathBase;

        public string ModDLCName => Builder.ModDLCName;

        public string StartupName => Builder.StartupName;

        public int? ModuleNumber => Builder.ModuleNumber;

        public string DLCBaseFolder => Path.Combine(Builder.ModOutputPathBase, Builder.ModDLCName);

        public string CookedPCConsoleFolder => Path.Combine(DLCBaseFolder, "CookedPCConsole");

        public string MergeModsFolder => Path.Combine(ModOutputPathBase, "MergeMods");

        private MergeMods? _mergeMods;
        public MergeMods MergeMods
        {
            get
            {
                _mergeMods ??= new MergeMods();
                return _mergeMods;
            }
        }

        private TlkBuilder? _tlkBuilder;
        public TlkBuilder TlkBuilder
        {
            get
            {
                _tlkBuilder ??= new TlkBuilder(Game);
                return _tlkBuilder;
            }
        }
        
        private IMEPackage? _startupFile;
        public IMEPackage GetStartupFile()
        {
            _startupFile ??= MEPackageHandler.CreateAndOpenPackage(Path.Combine(Builder.ModOutputPathBase, Builder.ModDLCName, "CookedPCConsole", Builder.StartupName), Builder.Game);
            return _startupFile;
        }

        private readonly List<ModConfigMergeFile> _configMergeFiles = [];
        private readonly List<DlcConfigFile> _dlcConfigFiles = [];

        public IEnumerable<ModConfigMergeFile> ConfigMergeFiles => _configMergeFiles;

        public IEnumerable<DlcConfigFile> DlcConfigFiles => _dlcConfigFiles;

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

        public DlcConfigFile GetOrCreateDlcConfigFile(string targetConfigFilename)
        {
            if (Game == MEGame.LE1)
            {
                throw new Exception("LE1 does not support DLC config files. Use config merge instead");
            }
            var existing = _dlcConfigFiles.FirstOrDefault(x => x.TargetConfigFileName == targetConfigFilename);

            if (existing != null)
            {
                return existing;
            }

            var newFile = new DlcConfigFile(targetConfigFilename);
            _dlcConfigFiles.Add(newFile);
            return newFile;
        }
    }
}
