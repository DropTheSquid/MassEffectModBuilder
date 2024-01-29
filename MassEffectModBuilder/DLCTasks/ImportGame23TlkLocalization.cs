using LegendaryExplorerCore.Packages;

namespace MassEffectModBuilder.DLCTasks
{
    public record class ImportGame23TlkLocaliazation(MELocalization Localization, string XmlPath) : IModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            switch (context.Game)
            {
                case MEGame.LE1:
                case MEGame.ME1:
                    Console.WriteLine($"Warning: you are importing ME2/LE2/ME3/LE3 tlk xml into a mod that is building for {context.Game}. This can be valid if you are using the same strings for multiple mods, but otherwise, you have probably made a mistake and should be using ImportGame1TlkLocalization");
                    break;
                case MEGame.ME2:
                case MEGame.LE2:
                case MEGame.ME3:
                case MEGame.LE3:
                    break;
                default:
                    throw new Exception($"Invalid game {context.Game}");
            }

            if (!File.Exists(XmlPath))
            {
                throw new Exception($"Tlk Xml {XmlPath} not found");
            }
            context.TlkBuilder.ImportME2ME3Xml(XmlPath, Localization);
        }
    }
}
