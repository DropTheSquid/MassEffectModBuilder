﻿using LegendaryExplorerCore.Packages;
using MassEffectModBuilder.MergeTasks;

namespace MassEffectModBuilder
{
    public class ModBuilderContext(ModBuilder builder)
    {
        private readonly ModBuilder Builder = builder;

        public MEGame Game => Builder.Game;

        public string ModOutputPathBase => Builder.ModOutputPathBase;

        public string ModDLCName => Builder.ModDLCName;

        public string StartupName => Builder.StartupName;

        public string ModTempFolder => Path.Combine(Builder.ModOutputPathBase, "ModBuilderStaging");

        public string DLCBaseFolder => Path.Combine(Builder.ModOutputPathBase, Builder.ModDLCName);

        public string CookedPCConsoleFolder => Path.Combine(DLCBaseFolder, "CookedPCConsole");

        public string MergeModsFolder => Path.Combine(ModOutputPathBase, "MergeMods");

        public MergeMods MergeMods { get; } = new MergeMods();

        private IMEPackage? _startupFile;
        internal IMEPackage GetStartupFile()
        {
            _startupFile ??= MEPackageHandler.CreateAndOpenPackage(Path.Combine(Builder.ModOutputPathBase, Builder.ModDLCName, "CookedPCConsole", Builder.StartupName), Builder.Game);
            return _startupFile;
        }
    }
}
