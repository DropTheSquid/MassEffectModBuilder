using LegendaryExplorerCore.Packages;

namespace MassEffectModBuilder
{
    public class Examples
    {
        public static void BuildLE1Mod(string modLibrary)
        {
            // the base mod, containing mostly just the moddesc.ini
            var mod = new ModBuilder(
                MEGame.LE1,
                "LE1 Example Mod",
                "Your Name Here",
                "1.0",
                "An Example LE1 mod");

            // a DLC portion to add to the mod
            var dlc = new DLC.DlcBuilder()
            {
                DlcName = "ExampleMod",
                InternalName = "LE1 Example",
                MountPriority = 12345,
                InternalNameTlkId = 54321
            };

            // a merge portion to add to the mod
            var merge = new Merge.MergeBuilder()
            {
                M3mName = "ExampleMerge"
            };

            // actually put it all together and build it
            mod
                .RequiresTextureOverrideAsi()
                // add a DLC mod to it
                .WithDlc(dlc)
                // add a merge mod to it
                .WithMergeMod(merge)
                // actually output everything into the folder
                .Build(Path.Combine(modLibrary, "LE1", "Example Mod"));
        }

        public static void BuildLE2Mod(string modLibrary)
        {
            // the base mod, containing mostly just the moddesc.ini
            var mod = new ModBuilder(
                MEGame.LE2,
                "LE1 Example Mod",
                "Your Name Here",
                "1.0",
                "An Example LE2 mod");

            // a DLC portion to add to the mod
            var dlc = new DLC.DlcBuilder()
            {
                DlcName = "ExampleMod",
                InternalName = "LE2 Example",
                MountPriority = 12345,
                InternalNameTlkId = 54321,
                Game2ModuleNumber = 5678
            };

            // a merge portion to add to the mod
            var merge = new Merge.MergeBuilder()
            {
                M3mName = "ExampleMerge"
            };

            // actually put it all together and build it
            mod
                .RequiresTextureOverrideAsi()
                // add a DLC mod to it
                .WithDlc(dlc)
                // add a merge mod to it
                .WithMergeMod(merge)
                // actually output everything into the folder
                .Build(Path.Combine(modLibrary, "LE2", "Example Mod"));
        }

        public static void BuildLE3Mod(string modLibrary)
        {
            // the base mod, containing mostly just the moddesc.ini
            var mod = new ModBuilder(
                MEGame.LE3,
                "LE2 Example Mod",
                "Your Name Here",
                "1.0",
                "An Example LE3 mod");

            // a DLC portion to add to the mod
            var dlc = new DLC.DlcBuilder()
            {
                DlcName = "ExampleMod",
                InternalName = "LE3 Example",
                MountPriority = 12345,
                InternalNameTlkId = 54321
            };

            // a merge portion to add to the mod
            var merge = new Merge.MergeBuilder()
            {
                M3mName = "ExampleMerge"
            };

            // actually put it all together and build it
            mod
                .RequiresTextureOverrideAsi()
                // add a DLC mod to it
                .WithDlc(dlc)
                // add a merge mod to it
                .WithMergeMod(merge)
                // actually output everything into the folder
                .Build(Path.Combine(modLibrary, "LE3", "Example Mod"));
        }
    }
}
