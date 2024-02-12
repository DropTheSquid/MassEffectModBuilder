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

            var deduplicatedClasses = classes.GroupBy(x => x.ClassName).Select(x =>
            {
                if (x.Count() > 1)
                {
                    // this shouldn't happen, but there are two levels of bad; 
                    var first = x.First();
                    var (_, source, _) = first;
                    var ifp = first.InstancedFullPath;
                    if (x.All(y => y.InstancedFullPath == ifp && y.SourceCode == source))
                    {
                        // if they are truly duplicates, just log a warning and move on
                        Console.WriteLine($"Warning: you are trying to compile {x.Count()} copies of {ifp}; deduplicating, but you have a mistake in your code somewhere");
                    }
                    else
                    {
                        // if there are two classes at different paths or with different code, that is a show stopper. You made a bad mistake.
                        throw new Exception($"You are trying to compile multiple classes called {x.First().ClassName} at different paths or with different code.");
                    }
                }

                // check that a class with this name does not already exist. reject if it does. loose class compiler is not set up to handle this case. 
                if (package.FindClass(x.Key) != null)
                {
                    throw new Exception($"package already contains class {x.Key}");
                }

                return x.First();
            });

            internalClasses = deduplicatedClasses
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
                    Console.Error.WriteLine(err);
                }
                throw new Exception($"could not compile loose classes to target file {pcc.FileNameNoExtension}");
            }
            foreach (var warning in messages.AllWarnings)
            {
                Console.WriteLine(warning);
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
