using LegendaryExplorerCore.Packages;
using LegendaryExplorerCore.Unreal;
using LegendaryExplorerCore.UnrealScript;

namespace MassEffectModBuilder.LEXHelpers
{
    public static class LooseClassCompile
    {
        public record struct ClassToCompile(string ClassName, string SourceCode, string[]? PackagePath = null)
        {
            public readonly string InstancedFullPath => string.Join(".", [.. PackagePath ?? [], ClassName]);
        }

        public static void CompileClasses(IEnumerable<ClassToCompile> classes, IMEPackage package)
        {
            IEnumerable<UnrealScriptCompiler.LooseClassPackageEx> internalClasses = [];

            internalClasses = classes
                .GroupBy(x => x.PackagePath ?? [])
                .Select(x => new UnrealScriptCompiler.LooseClassPackageEx(x.Key, x.Select(y => new UnrealScriptCompiler.LooseClass(y.ClassName, y.SourceCode)).ToList()));

            CompileClassesInternal(internalClasses.ToList(), package);
        }

        private static void CompileClassesInternal(List<UnrealScriptCompiler.LooseClassPackageEx> classesToCompile, IMEPackage pcc)
        {
            // copied from SirC's LEX experiments for loose class compile
            static IEntry MissingObjectResolver(IMEPackage pcc, string instancedPath)
            {
                ExportEntry export = null;
                foreach (string name in instancedPath.Split('.'))
                {
                    export = ExportCreator.CreatePackageExport(pcc, NameReference.FromInstancedString(name), export);
                }
                return export;
            }

            var messages = UnrealScriptCompiler.CompileLooseClassesEx(pcc, classesToCompile, MissingObjectResolver);

            if (messages.HasErrors)
            {
                Console.Error.WriteLine($"could not compile loose classes to target file {pcc.FileNameNoExtension}");
                foreach (var err in messages.AllErrors)
                {
                    Console.Error.WriteLine(err.Message);
                }
                throw new Exception($"could not compile loose classes to target file {pcc.FileNameNoExtension}");
            }
        }

        public static IEnumerable<ClassToCompile> GetClassesFromDirectories(params string[] classRootFolders)
        {
            List<ClassToCompile> classes = [];

            foreach (var classRootFolder in classRootFolders)
            {
                // first, enumerate all directories (recursively), including the root folder
                var dirs = new List<string>
                {
                    { classRootFolder }
                };
                dirs.AddRange(Directory.EnumerateDirectories(classRootFolder, "*", SearchOption.AllDirectories));
                foreach (var directory in dirs)
                {
                    var relativePath = Path.GetRelativePath(classRootFolder, directory);
                    string[] relativePathSegments;
                    if (relativePath == ".")
                    {
                        relativePathSegments = [];
                    }
                    else
                    {
                        relativePathSegments = relativePath.Split('\\');
                    }

                    foreach (string ucFilePath in Directory.EnumerateFiles(directory, "*.uc"))
                    {
                        classes.Add(GetClassFromFile(ucFilePath, relativePathSegments));
                    }
                }
            }
            
            return classes;
        }

        public static ClassToCompile GetClassFromFile(string filePath, string[]? packagePath = null)
        {
            var source = File.ReadAllText(filePath);
            var className = Path.GetFileNameWithoutExtension(filePath);
            return new ClassToCompile(className, source, packagePath ?? []);
        }
    }
}
