using LegendaryExplorerCore;
using LegendaryExplorerCore.Packages;

namespace AmmBuilder
{
    public class ModBuilder
    {
        public MEGame Game { get; set; }
        /// <summary>
        /// Full or relative path to the base of the mod (the folder which should contain the moddesc)
        /// </summary>
        public required string ModOutputPathBase { get; set; }

        /// <summary>
        /// Name of the DLC mod, for example "DLC_MOD_Whatever"
        /// </summary>
        public required string ModDLCName { get; set; }

        public List<ModBuilderTask> ModBuilderTasks { get; set; } = new List<ModBuilderTask>();

        public async Task BuildMod()
        {
            // init the library
            LegendaryExplorerCoreLib.InitLib(TaskScheduler.Current, x => Console.Error.WriteLine($"Failed to save package: {x}"));

            foreach (var task in ModBuilderTasks)
            {
                await task.RunModTask(this);
            }
        }
    }
}
