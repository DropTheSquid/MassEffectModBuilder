namespace MassEffectModBuilder.DLCTasks
{
    public class OutputConfigMerge : IModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            foreach (var merge in context.ConfigMergeFiles)
            {
                var lines = merge.OutputFileContents(context.Game);
                var path = Path.Combine(context.CookedPCConsoleFolder, merge.OutputFileName);
                // make sure the folder exists
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllLines(path, lines);
            }
        }
    }
}
