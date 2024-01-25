using LegendaryExplorerCore.Packages;

namespace MassEffectModBuilder.DLCTasks
{
    public record class ImportGame1TlkLocaliazation(MELocalization Localization, string MaleXmlPath, string? FemaleXmlPath = null) : ModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            switch (context.Game)
            {
                case MEGame.LE1:
                case MEGame.ME1:
                    break;
                case MEGame.ME2:
                case MEGame.LE2:
                case MEGame.ME3:
                case MEGame.LE3:
                    Console.WriteLine($"Warning: you are importing ME1/LE1 tlk xml into a mod that is building for {context.Game}. This can be valid if you are using the same strings for multiple mods, but otherwise, you have probably made a mistake and should be using ImportGame23TlkLocalization");
                    break;
                default:
                    throw new Exception($"Invalid game {context.Game}");
            }

            if (!File.Exists(MaleXmlPath))
            {
                throw new Exception($"Tlk Xml {MaleXmlPath} not found");
            }
            context.TlkBuilder.ImportME1Xml(MaleXmlPath, Localization, false);
            if (FemaleXmlPath != null)
            {
                if (!File.Exists(FemaleXmlPath))
                {
                    throw new Exception($"Tlk Xml {FemaleXmlPath} not found");
                }
                context.TlkBuilder.ImportME1Xml(FemaleXmlPath, Localization, true);
            }
        }
    }
}
