using LegendaryExplorerCore.Coalesced;
using System.Xml.Linq;

namespace MassEffectModBuilder.DLCTasks
{
    public record class CompileGame3Config(string XmlFilePath) : ModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            var xmldocument = XDocument.Load(XmlFilePath);
            var rootElement = xmldocument.Root;
            
            var dest = Path.Combine(context.CookedPCConsoleFolder, rootElement.Attribute(@"name").Value);
            CoalescedConverter.ConvertToBin(XmlFilePath, dest);
        }
    }
}
