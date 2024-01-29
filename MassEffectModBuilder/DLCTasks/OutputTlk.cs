using LegendaryExplorerCore.Packages;

namespace MassEffectModBuilder.DLCTasks
{
    public class OutputTlk : IModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            switch (context.Game)
            {
                case MEGame.ME1:
                case MEGame.LE1:
                    context.TlkBuilder.OutputGame1Tlks(context.CookedPCConsoleFolder, context.ModDLCName);
                    break;
                case MEGame.ME2:
                case MEGame.LE2:
                    if (!context.ModuleNumber.HasValue)
                    {
                        throw new Exception("You must set the module number in order to generate ME2/LE2 tlks");
                    }
                    context.TlkBuilder.OutputGame23Tlks(context.CookedPCConsoleFolder, "DLC_" + context.ModuleNumber);
                    break;
                case MEGame.ME3:
                case MEGame.LE3:
                    context.TlkBuilder.OutputGame23Tlks(context.CookedPCConsoleFolder, context.ModDLCName);
                    break;
                default:
                    throw new Exception($"Invalid game {context.Game}");
            }
        }
    }
}
