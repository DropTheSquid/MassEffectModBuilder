namespace MassEffectModBuilder.DLCTasks
{
    public class OutputConfigMerge : IModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            foreach (var merge in context.ConfigMergeFiles)
            {
                var lines = merge.OutputFileContents(context.Game);
                File.WriteAllLines(Path.Combine(context.CookedPCConsoleFolder, merge.OutputFileName), lines);
            }
        }
    }
}
