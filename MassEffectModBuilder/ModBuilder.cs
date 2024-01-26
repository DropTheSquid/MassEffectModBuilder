using LegendaryExplorerCore;
using LegendaryExplorerCore.Packages;

namespace MassEffectModBuilder
{
    public class ModBuilder
    {
        public required MEGame Game { get; init; }
        /// <summary>
        /// Full or relative path to the base of the mod (the folder which should contain the moddesc)
        /// </summary>
        public required string ModOutputPathBase { get; init; }

        /// <summary>
        /// Name of the DLC mod, for example "DLC_MOD_Whatever"
        /// </summary>
        public required string ModDLCName { get; init; }

        public required string StartupName { get; init; }

        /// <summary>
        /// Only applicable to ME2/LE2, but required for some tasks for those games. Set during starter kit, stored in the mount file. 
        /// </summary>
        public int? ModuleNumber { get; init; }

        private readonly List<ModBuilderTask> ModBuilderTasks = [];

        public ModBuilder AddTask(ModBuilderTask task)
        {
            ModBuilderTasks.Add(task);
            return this;
        }

        public void Build()
        {
            Console.WriteLine($"Starting mod build into {ModOutputPathBase}");
            // init the library
            LegendaryExplorerCoreLib.InitLib(TaskScheduler.Current, x => Console.Error.WriteLine($"Failed to save package: {x}"));

            var context = new ModBuilderContext(this);

            foreach (var task in ModBuilderTasks)
            {
                task.RunModTask(context);
            }
        }
    }
}
