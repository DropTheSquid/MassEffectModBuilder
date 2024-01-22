using LegendaryExplorerCore.GameFilesystem;
using LegendaryExplorerCore.Packages;
using LegendaryExplorerCore.UnrealScript;

namespace AmmBuilder.BasegameTasks
{
    public class AddNewClasses : BasegameModBuilderTask
    {
        public string ClassFilePath { get; set; }

        protected override async Task ApplyChange(ModBuilder modBuilder, FileLib lib)
        {
            var className = Path.GetFileNameWithoutExtension(ClassFilePath);
            var classText = File.ReadAllText(ClassFilePath);
            // TODO verify the file name matches the class name in the class text

            // find the export to compile; create a blank class if it does not exist
            var export = lib.Pcc.FindExport(Path.GetFileNameWithoutExtension(ClassFilePath));
            if (export == null)
            {
                UnrealScriptCompiler.CompileClass(lib.Pcc, $"class {className};", lib);
                export = lib.Pcc.FindExport(Path.GetFileNameWithoutExtension(ClassFilePath));
            }

            // ensure the empty class export exists so that it can compile when referencing itself
            var (_, messages) = UnrealScriptCompiler.CompileClass(lib.Pcc, classText, lib, export);
            if (messages.HasErrors)
            {
                Console.Error.WriteLine($"could not apply basegame change adding class {className}");
                foreach (var err in messages.AllErrors)
                {
                    Console.Error.WriteLine(err.Message);
                }
                throw new Exception($"could not apply basegame change adding class {className}");
            }
        }
    }
}
