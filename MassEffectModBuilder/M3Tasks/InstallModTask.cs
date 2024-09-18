using Microsoft.Win32;
using System.Diagnostics;

namespace MassEffectModBuilder.M3Tasks
{
    public record class InstallModTask(bool LaunchGame) : IModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            var m3Path = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\ME3Tweaks", "ExecutableLocation", null);

            if (m3Path == null)
            {
                Console.WriteLine("Could not install mod; Mod Manager not found");
                return;
            }

            var modDescPath = Path.Combine(context.ModOutputPathBase, "moddesc.ini");

            var args = $"--installmod \"{modDescPath}\"";

            if (LaunchGame)
            {
                args += $" --bootgame {context.Game}";
            }

            ProcessStartInfo psi = new ProcessStartInfo(m3Path, args);
            Process.Start(psi);
        }
    }
}
