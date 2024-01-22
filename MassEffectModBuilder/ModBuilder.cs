using LegendaryExplorerCore.Packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmmBuilder
{
    public class ModBuilder
    {
        /// <summary>
        /// Name of the DLC mod, for example "DLC_MOD_Whatever"
        /// </summary>
        public required string ModDLCName { get; set; }

        /// <summary>
        /// Full or relative path to the base of the mod (the folder which should contain the moddesc)
        /// </summary>
        public required string ModOutputPathBase { get; set; }

        /// <summary>
        /// The name of the startup file, if one exists. 
        /// </summary>
        public string StartupFileName { get; set; }
        public MEGame Game { get; set; }
        public IEnumerable<ModBuilderTask> ModBuilderTasks { get; set; }

        public async Task BuildMod()
        {

        }
    }
}
