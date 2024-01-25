using LegendaryExplorerCore.Packages;

namespace MassEffectModBuilder.DLCTasks
{
    public record class ImportGame23TlkFolder(string XmlFolderPath) : ModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            if (!Directory.Exists(XmlFolderPath))
            {
                throw new Exception($"Folder {XmlFolderPath} not found");
            }

            foreach(var xml in Directory.EnumerateFiles(XmlFolderPath))
            {
                if (Path.GetExtension(xml).Equals("xml", StringComparison.CurrentCultureIgnoreCase))
                {
                    var loc = Path.GetFileNameWithoutExtension(xml).GetUnrealLocalization();
                    if (loc == MELocalization.None)
                    {
                        throw new Exception($"unable to determine localization of {xml}; please rename it to use a standard language suffix or add it explicitly using a ImportGame23TlkLocaliazation task");
                    }
                    var subTask = new ImportGame23TlkLocaliazation(loc, xml);
                    subTask.RunModTask(context);
                }
            }
        }
    }
}
