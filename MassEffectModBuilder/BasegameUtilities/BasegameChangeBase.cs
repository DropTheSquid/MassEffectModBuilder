using LegendaryExplorerCore.Packages;

namespace AmmBuilder.BasegameUtilities
{
    public abstract class BasegameChangeBase
    {
        /// <summary>
        /// Which basegame file is affected by the basegame change. It must be exactly one, and it is required. 
        /// </summary>
        public required string AffectedFile { get; set; }

        /// <summary>
        /// Which game is affected by this basegame change. 
        /// </summary>
        public required MEGame TargetGame { get; set; }

        public string M3mTarget { get; set; }

        /// <summary>
        /// Whether this basegame change is required for mod compilation; 
        /// for example, adding a new class to SFXGame.pcc or adding a new function to a class which is called from the mod's code, without which compilation of the mod code will fail.
        /// If this is false, it will only be included as part of the merge mods in the mod output
        /// If it is true, it will also be applied to the basegame as part of mod building so the mod code can compile.
        /// Leave this false unless you need it. Also ensure that if it is true, you test installing the generated mod on a vanilla game. 
        /// </summary>
        public bool IsRequiredForModCompilation { get; set; } = false;

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(AffectedFile))
            {
                return false;
            }
            // TODO I need to add supported games somewhere
            //if (TargetGame)
            return true;
        }


    }
}
