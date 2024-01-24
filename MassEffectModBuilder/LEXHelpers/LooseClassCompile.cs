using LegendaryExplorerCore.GameFilesystem;
using LegendaryExplorerCore.Packages;
using LegendaryExplorerCore.Unreal;
using LegendaryExplorerCore.Unreal.BinaryConverters;
using LegendaryExplorerCore.Unreal.ObjectInfo;
using LegendaryExplorerCore.UnrealScript;

namespace MassEffectModBuilder.LEXHelpers
{
    public static class LooseClassCompile
    {

        public static void CompileClasses(MEGame game, List<UnrealScriptCompiler.LooseClassPackageEx> classesToCompile, IMEPackage pcc)
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

            //terrible function, there are better ways of implementing it.
            List<string> VTableDonorGetter(string className)
            {
                var classInfo = GlobalUnrealObjectInfo.GetClassOrStructInfo(game, className);
                if (classInfo is not null)
                {
                    string path = Path.Combine(MEDirectories.GetBioGamePath(game), classInfo.pccPath);
                    if (File.Exists(path))
                    {
                        IMEPackage partialLoad = MEPackageHandler.UnsafePartialLoad(path, exp => exp.IsClass && exp.ObjectName == className);
                        foreach (ExportEntry export in partialLoad.Exports)
                        {
                            if (export.IsClass && export.ObjectName == className)
                            {
                                var obj = export.GetBinaryData<UClass>();
                                return obj.VirtualFunctionTable.Select(uIdx => partialLoad.GetEntry(uIdx).ObjectName.Name).ToList();
                            }
                        }
                    }
                }
                return null;
            }

            var messages = UnrealScriptCompiler.CompileLooseClassesEx(pcc, classesToCompile, MissingObjectResolver, vtableDonorGetter: VTableDonorGetter);

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

        public static List<UnrealScriptCompiler.LooseClassPackageEx> GetClassesFromDirectoryStructure(string classFolderRoot)
        {
            var looseClassPackages = new List<UnrealScriptCompiler.LooseClassPackageEx>();

            // first, enumerate all directories (recursively), including the root folder
            var dirs = new List<string>
            {
                { classFolderRoot }
            };
            dirs.AddRange(Directory.EnumerateDirectories(classFolderRoot, "*", SearchOption.AllDirectories));
            foreach (var directory in dirs)
            {
                var relativePath = Path.GetRelativePath(classFolderRoot, directory);
                string[] relativePathSegments;
                if (relativePath == ".")
                {
                    relativePathSegments = [];
                }
                else
                {
                    relativePathSegments = relativePath.Split('\\');
                }
                var classes = new List<UnrealScriptCompiler.LooseClass>();

                foreach (string ucFilePath in Directory.EnumerateFiles(directory, "*.uc"))
                {
                    string source = File.ReadAllText(ucFilePath);
                    //files must be named Classname.uc
                    classes.Add(new UnrealScriptCompiler.LooseClass(Path.GetFileNameWithoutExtension(ucFilePath), source));
                }
                looseClassPackages.Add(new UnrealScriptCompiler.LooseClassPackageEx(relativePathSegments, classes));
            }

            return looseClassPackages;
        }

        public static List<UnrealScriptCompiler.LooseClassPackageEx> GetSingleClassForCompile(string path)
        {
            var looseClassPackages = new List<UnrealScriptCompiler.LooseClassPackageEx>();
            var classes = new List<UnrealScriptCompiler.LooseClass>();

            string source = File.ReadAllText(path);
            // TODO read the class name out of the file
            var className = Path.GetFileNameWithoutExtension(path); 
            classes.Add(new UnrealScriptCompiler.LooseClass(className, source));

            looseClassPackages.Add(new UnrealScriptCompiler.LooseClassPackageEx([], classes));

            return looseClassPackages;
        }

        //public static List<UnrealScriptCompiler.LooseClassPackageEx> CompileLooseClassesFromFolder(MEGame game, string classFolderRoot, FileLib lib)
        //{
        //    // copied from SirC's LEX experiments for loose class compile
        //    static IEntry MissingObjectResolver(IMEPackage pcc, string instancedPath)
        //    {
        //        ExportEntry export = null;
        //        foreach (string name in instancedPath.Split('.'))
        //        {
        //            export = ExportCreator.CreatePackageExport(pcc, NameReference.FromInstancedString(name), export);
        //        }
        //        return export;
        //    }

        //    //terrible function, there are better ways of implementing it.
        //    List<string> VTableDonorGetter(string className)
        //    {
        //        var classInfo = GlobalUnrealObjectInfo.GetClassOrStructInfo(game, className);
        //        if (classInfo is not null)
        //        {
        //            string path = Path.Combine(MEDirectories.GetBioGamePath(game), classInfo.pccPath);
        //            if (File.Exists(path))
        //            {
        //                IMEPackage partialLoad = MEPackageHandler.UnsafePartialLoad(path, exp => exp.IsClass && exp.ObjectName == className);
        //                foreach (ExportEntry export in partialLoad.Exports)
        //                {
        //                    if (export.IsClass && export.ObjectName == className)
        //                    {
        //                        var obj = export.GetBinaryData<UClass>();
        //                        return obj.VirtualFunctionTable.Select(uIdx => partialLoad.GetEntry(uIdx).ObjectName.Name).ToList();
        //                    }
        //                }
        //            }
        //        }
        //        return null;
        //    }

        //    var looseClassPackages = new List<UnrealScriptCompiler.LooseClassPackageEx>();

        //    // first, enumerate all directories (recursively), including the root folder
        //    var dirs = new List<string>
        //    {
        //        { classFolderRoot }
        //    };
        //    dirs.AddRange(Directory.EnumerateDirectories(classFolderRoot, "*", SearchOption.AllDirectories));
        //    foreach (var directory in dirs)
        //    {
        //        var relativePath = Path.GetRelativePath(classFolderRoot, directory);
        //        string[] relativePathSegments;
        //        if (relativePath == ".")
        //        {
        //            relativePathSegments = [];
        //        }
        //        else
        //        {
        //            relativePathSegments = relativePath.Split('\\');
        //        }
        //        var classes = new List<UnrealScriptCompiler.LooseClass>();

        //        foreach (string ucFilePath in Directory.EnumerateFiles(directory, "*.uc"))
        //        {
        //            string source = File.ReadAllText(ucFilePath);
        //            //files must be named Classname.uc
        //            classes.Add(new UnrealScriptCompiler.LooseClass(Path.GetFileNameWithoutExtension(ucFilePath), source));
        //        }
        //        looseClassPackages.Add(new UnrealScriptCompiler.LooseClassPackageEx(relativePathSegments, classes));
        //    }

        //    var messages = UnrealScriptCompiler.CompileLooseClassesEx(lib.Pcc, looseClassPackages, MissingObjectResolver, vtableDonorGetter: VTableDonorGetter);

        //    if (messages.HasErrors)
        //    {
        //        Console.Error.WriteLine($"could not compile loose classes to target file {lib.Pcc.FileNameNoExtension}");
        //        foreach (var err in messages.AllErrors)
        //        {
        //            Console.Error.WriteLine(err.Message);
        //        }
        //        throw new Exception($"could not compile loose classes to target file {lib.Pcc.FileNameNoExtension}");
        //    }

        //    return looseClassPackages;
        //}
    }
}
