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
            foreach (var file in Directory.EnumerateFiles(SourceDirectory, "*", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(SourceDirectory, file);
                var pathSegments = relativePath.Split(Path.DirectorySeparatorChar).SkipLast(1);
                string pathSoFar = destinationDirectory;
                foreach (var dir in pathSegments)
                {
                    var dirPath = Path.Combine(pathSoFar, dir);
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    pathSoFar = Path.Combine(pathSoFar, dir);
                }
                var dest = Path.Combine(destinationDirectory, relativePath);
                if (File.Exists(dest))
                {
                    File.Delete(dest);
                }
                File.Copy(file, dest);
            }
        }
    }
}
