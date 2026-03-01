using LegendaryExplorerCore.GameFilesystem;

namespace MassEffectModBuilder.DLC
{
    public class LE1AutoLoad(int modBaseTlkId, string modName, int modMount)
    {
        public void OutputAutloadIni(string folder)
        {
            var ini = new AutoloadIni
            {
                DLCModNameStrRef = ModBaseTlkId,
                ModMount = ModMount,
                ModName = ModName,
                GlobalTalkTables = GlobalTlks,
                GlobalPackages = GlobalPackages,
                Bio2DAs = Bio2DAs,
                PlotManagerStateTransitionMaps = PlotManagerStateTransitionMaps,
                PlotManagerConsequenceMaps = PlotManagerConsequenceMaps,
                PlotManagerOutcomeMaps = PlotManagerOutcomeMaps,
                PlotManagerQuestMaps = PlotManagerQuestMaps,
                PlotManagerCodexMaps = PlotManagerCodexMaps,
                PlotManagerConditionals = PlotManagerConditionals,
            };

            var iniString = ini.ToString();
            File.WriteAllText(Path.Combine(folder, "AutoLoad.ini"), iniString);
        }

        public int ModBaseTlkId { get; set; } = modBaseTlkId;

        public string ModName { get; set; } = modName;

        public int ModMount { get; set; } = modMount;

        public List<string> GlobalTlks { get; } = [];

        public List<string> GlobalPackages { get; } = [];

        public List<string> Bio2DAs = [];
        public List<string> PlotManagerStateTransitionMaps = [];
        public List<string> PlotManagerConsequenceMaps = [];
        public List<string> PlotManagerOutcomeMaps = [];
        public List<string> PlotManagerQuestMaps = [];
        public List<string> PlotManagerCodexMaps = [];
        public List<string> PlotManagerConditionals = [];
    }
}
