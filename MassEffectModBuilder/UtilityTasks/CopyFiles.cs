namespace MassEffectModBuilder.UtilityTasks
{
    public record class CopyFiles(string SourceDirectory, Func<ModBuilderContext, string> DestinationDirectoryFunc) : ModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            var destinationDirectory = DestinationDirectoryFunc(context);
            if (!Directory.Exists(SourceDirectory))
            {
                throw new Exception($"source directory {SourceDirectory} not found");
            }
            foreach (var file in Directory.EnumerateFiles(SourceDirectory))
            {
                File.Copy(file, Path.Combine(destinationDirectory, Path.GetFileName(file)));
            }
        }
    }
}
