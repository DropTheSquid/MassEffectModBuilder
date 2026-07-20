using LegendaryExplorerCore.Misc;
using LegendaryExplorerCore.Packages;

namespace MassEffectModBuilder
{
    public class ModDesc : DuplicatingIni
    {
        private const string ModInfoHeader = "ModInfo";

        public ModDesc(MEGame game, string modName, string modDev, string modVersion, string modDescription)
        {
            CmmVersion = "9.2";
            Game = game;
            ModName = modName;
            ModDev = modDev;
            ModVersion = modVersion;
            ModDescription = modDescription;
        }

        public void OutputModDesc(string folder)
        {
            // add the DLC stuff
            SetValue("CUSTOMDLC", "sourcedirs", String.Join(';', DLCs.Select(x => x.SourceDirectoryPath)));
            SetValue("CUSTOMDLC", "destdirs", String.Join(';', DLCs.Select(x => x.InstalledDlcName)));

            if (merges.Count != 0)
            {
                SetValue("BASEGAME", "moddir", ".");
                SetValue("BASEGAME", "mergemods", string.Join(";", merges));
            }

            if (ASIs.Count != 0)
            {
                SetValue("ASIMODS", "asimodstoinstall", $"({string.Join(", ", ASIs.Select(x => $"(GroupID={x})"))})");
            }

            WriteToFile(Path.Combine(folder, "moddesc.ini"));
        }

        public string CmmVersion
        {
            get => GetValue("ModManager", "cmmver").Value;
            set => SetValue("ModManager", "cmmver", value);
        }

        public MEGame Game
        {
            get => Enum.Parse<MEGame>(GetValue(ModInfoHeader, "game").Value);
            protected set => SetValue(ModInfoHeader, "game", value.ToString().ToUpper());
        }
        public string ModName
        {
            get => GetValue(ModInfoHeader, "modname").Value;
            protected set => SetValue(ModInfoHeader, "modname", value);
        }
        public string ModDev
        {
            get => GetValue(ModInfoHeader, "moddev").Value;
            protected set => SetValue(ModInfoHeader, "moddev", value);
        }
        public string ModVersion
        {
            get => GetValue(ModInfoHeader, "modver").Value;
            protected set => SetValue(ModInfoHeader, "modver", value);
        }
        public string ModDescription
        {
            get => GetValue(ModInfoHeader, "moddesc").Value;
            protected set => SetValue(ModInfoHeader, "moddesc", value);
        }
        public string? ModSite
        {
            get => GetValue(ModInfoHeader, "modsite")?.Value;
            set => SetValue(ModInfoHeader, "modsite", value);
        }

        public void SetValue(string section, string key, string? value)
        {
            this[section].SetSingleEntry(key, value);
        }

        public void AddDlc(string sourceDirectory, string installDirectory)
        {
            DLCs.Add(new CustomDLC() { SourceDirectoryPath = sourceDirectory, InstalledDlcName = installDirectory });
        }

        public void AddMerge(string m3mName)
        {
            if (!m3mName.EndsWith(".m3m", StringComparison.InvariantCultureIgnoreCase))
            {
                m3mName = m3mName + ".m3m";
            }
            merges.Add(m3mName);
        }

        public void RequiresAsi(int groupId)
        {
            if (!ASIs.Contains(groupId))
            {
                ASIs.Add(groupId);
            }
        }

        protected List<CustomDLC> DLCs = [];

        protected List<string> merges = [];

        protected List<int> ASIs = [];
        // TODO banner images, requiredDlc, a few other things?

        // TODO customDLC header, other stuff?

        public class CustomDLC
        {
            public string SourceDirectoryPath { get; set; }
            public string InstalledDlcName { get; set; }
            //public string? HumanReadableName { get; set; }
            // Outdated dlc, incompatible DLCs, multilists?
        }
    }
}
