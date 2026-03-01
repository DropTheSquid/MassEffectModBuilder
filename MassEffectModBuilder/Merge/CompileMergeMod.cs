using Microsoft.Win32;
using System.Diagnostics;

namespace MassEffectModBuilder.Merge
{
    public record class CompileMergeMod(string TargetJson, string? M3Path = null, string FeatureLevel = "8.0") : IMergeTask
    {
        public void RunMergeTask(MergeBuilderContext context)
        {
            var m3Path = M3Path ?? (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\ME3Tweaks", "ExecutableLocation", null);

            if (m3Path == null)
            {
                Console.WriteLine("Could not compile merge mod; Mod Manager not found");
                return;
            }
           
            ProcessStartInfo psi = new(m3Path, $"--compilemergemod \"{Path.Combine(context.ModBuilderContext.MergeModsFolder, TargetJson)}.json\" --featurelevel {FeatureLevel}");
            Process.Start(psi);
        }
    }
}
