namespace MassEffectModBuilder.UtilityTasks
{
    public record class CopyFiles(string SourceDirectory, Func<ModBuilderContext, string> DestinationDirectoryFunc) : IModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            var destinationDirectory = DestinationDirectoryFunc(context);
            if (!Directory.Exists(SourceDirectory))
            {
                Console.WriteLine($"warning: you attempted to copy files out of {SourceDirectory} but it was not found; is it empty and therefore skipped by the build copy?");
                return;
            }
            foreach (var file in Directory.EnumerateFiles(SourceDirectory))
            {
                var dest = Path.Combine(destinationDirectory, Path.GetFileName(file));
                if (File.Exists(dest))
                {
                    File.Delete(dest);
                }
                File.Copy(file, dest);
            }
        }
    }
}
