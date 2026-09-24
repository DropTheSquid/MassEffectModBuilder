using LegendaryExplorerCore.Coalesced;
using LegendaryExplorerCore.Misc;
using LegendaryExplorerCore.Helpers;

namespace MassEffectModBuilder.Models
{
    public class Game23Configs
    {
        protected List<DlcConfigFile> ConfigFiles = [];

        public DlcConfigFile GetOrCreateConfigFile(string configFileName)
        {
            var configFile = ConfigFiles.FirstOrDefault(x => x.TargetConfigFileName == configFileName);
            if (configFile == null)
            {
                configFile = new DlcConfigFile(configFileName);
                ConfigFiles.Add(configFile);
            }
            return configFile;
        }

        public void InitGame2Configs(string dlcFolderName, int module, int internalTlkId, int flags)
        {
            // just needs a BioEngine.ini
            var bioEngine = GetOrCreateConfigFile("BIOEngine.ini");

            var coreSystem = bioEngine.GetOrCreateClass("Core.System");
            coreSystem.Add(new CoalesceProperty("CookPaths", new CoalesceValue("CLEAR", CoalesceParseAction.RemoveProperty)));
            // yes, it is still CookedPC even for LE; this matches the started kit
            coreSystem.Add(new CoalesceProperty("SeekFreePCPaths", new CoalesceValue($@"..\BIOGame\DLC\{dlcFolderName}\CookedPC", CoalesceParseAction.AddUnique)));

            var engineDlcModules = bioEngine.GetOrCreateClass("Engine.DLCModules");
            engineDlcModules.SetIntValue(dlcFolderName, module);

            var dlcInfo = bioEngine.GetOrCreateClass("DLCInfo");
            dlcInfo.SetIntValue("Version", 0);
            dlcInfo.SetIntValue("Flags", flags);
            dlcInfo.SetIntValue("Name", internalTlkId);
        }

        public void InitGame3Configs(string dlcModFolderName)
        {
            GetOrCreateConfigFile("BioAI.xml");

            var bioCredits = GetOrCreateConfigFile("BioCredits.xml");
            var configuration = bioCredits.GetOrCreateClass("configuration");
            configuration.Add(new CoalesceProperty("basedon", new CoalesceValue(@"..\..\BIOGame\DLC\DLC_Shared_MP\Config\DefaultDifficulty.ini", CoalesceParseAction.RemoveProperty)));

            var bioDifficulty = GetOrCreateConfigFile("BioDifficulty.xml");
            configuration = bioDifficulty.GetOrCreateClass("configuration");
            configuration.Add(new CoalesceProperty("basedon", new CoalesceValue(@"..\..\BIOGame\DLC\DLC_Shared_MP\Config\DefaultDifficulty.ini", CoalesceParseAction.RemoveProperty)));

            // BioEngine.xml
            var bioEngine = GetOrCreateConfigFile("BioEngine.xml");
            // not sure how critical this stuff is, but I am going to match the starter kit
            configuration = bioEngine.GetOrCreateClass("configuration");
            configuration.Add(new CoalesceProperty("basedon", new CoalesceValue($@"..\..\BIOGame\DLC\{dlcModFolderName}\Config\DefaultEngine.ini", CoalesceParseAction.Add)));

            var coreSystem = bioEngine.GetOrCreateClass("core.system");
            
            coreSystem.AddEntry(new CoalesceProperty("cookpaths", new CoalesceValue("null", CoalesceParseAction.RemoveProperty)));
            coreSystem.AddEntry(new CoalesceProperty("cookpaths", new CoalesceValue($@"..\..\BIOGame\DLC\{dlcModFolderName}\Content", CoalesceParseAction.Add)));
            coreSystem.AddEntry(new CoalesceProperty("cookpaths", new CoalesceValue($@"..\..\BIOGame\DLC\{dlcModFolderName}\Script", CoalesceParseAction.Add)));
            coreSystem.AddEntry(new CoalesceProperty("cookpaths", new CoalesceValue($@"..\..\BIOGame\DLC\{dlcModFolderName}\ScriptFinalRelease", CoalesceParseAction.Add)));

            coreSystem.AddEntry(new CoalesceProperty("frscriptpaths", new CoalesceValue(@"..\..\BIOGame\DLC\DLC_Shared\ScriptFinalRelease", CoalesceParseAction.Add)));
            coreSystem.AddEntry(new CoalesceProperty("frscriptpaths", new CoalesceValue($@"..\..\BIOGame\DLC\{dlcModFolderName}\ScriptFinalRelease", CoalesceParseAction.Add)));

            coreSystem.AddEntry(new CoalesceProperty("paths", new CoalesceValue(@"..\..\BIOGame\DLC\DLC_Shared\__Trashcan", CoalesceParseAction.Add)));
            coreSystem.AddEntry(new CoalesceProperty("paths", new CoalesceValue(@"..\..\BIOGame\DLC\DLC_Shared\Content", CoalesceParseAction.Add)));
            coreSystem.AddEntry(new CoalesceProperty("paths", new CoalesceValue(@"..\..\BIOGame\DLC\DLC_Shared\TestContent", CoalesceParseAction.Add)));
            coreSystem.AddEntry(new CoalesceProperty("paths", new CoalesceValue($@"..\..\BIOGame\DLC\{dlcModFolderName}\__Trashcan", CoalesceParseAction.Add)));
            coreSystem.AddEntry(new CoalesceProperty("paths", new CoalesceValue($@"..\..\BIOGame\DLC\{dlcModFolderName}\Content", CoalesceParseAction.Add)));
            coreSystem.AddEntry(new CoalesceProperty("paths", new CoalesceValue($@"..\..\BIOGame\DLC\{dlcModFolderName}\TestContent", CoalesceParseAction.Add)));

            coreSystem.AddEntry(new CoalesceProperty("scriptpaths", new CoalesceValue(@"..\..\BIOGame\DLC\DLC_Shared\Script", CoalesceParseAction.Add)));
            coreSystem.AddEntry(new CoalesceProperty("scriptpaths", new CoalesceValue($@"..\..\BIOGame\DLC\{dlcModFolderName}\Script", CoalesceParseAction.Add)));

            coreSystem.AddEntry(new CoalesceProperty("seekfreepcpaths", new CoalesceValue(@"..\..\BIOGame\DLC\DLC_Shared\CookedPCConsole", CoalesceParseAction.Add)));
            coreSystem.AddEntry(new CoalesceProperty("seekfreepcpaths", new CoalesceValue($@"..\..\BIOGame\DLC\{dlcModFolderName}\CookedPCConsole", CoalesceParseAction.Add)));

            var unrealEdEditorEngine = bioEngine.GetOrCreateClass("unrealed.editorengine");
            unrealEdEditorEngine.AddEntry(new CoalesceProperty("editpackages", new CoalesceValue("SFXGameContentDLC_Shared", CoalesceParseAction.Add)));
            unrealEdEditorEngine.AddEntry(new CoalesceProperty("editpackages", new CoalesceValue($"SFXGameContent{dlcModFolderName}", CoalesceParseAction.Add)));

            GetOrCreateConfigFile("BioGame.xml");

            var bioInput = GetOrCreateConfigFile("BioInput.xml");
            configuration = bioInput.GetOrCreateClass("configuration");
            configuration.Add(new CoalesceProperty("basedon", new CoalesceValue(@"..\..\BIOGame\DLC\DLC_Shared\Config\DefaultInput.ini", CoalesceParseAction.RemoveProperty)));

            GetOrCreateConfigFile("BioUI.xml");
            GetOrCreateConfigFile("BioWeapon.xml");
        }

        public void OutputGame2Inis(string folder)
        {
            foreach (var file in ConfigFiles)
            {
                file.OutputGame2Ini(folder);
            }
        }

        public void OutputGame3Bin(string folder, string fileName)
        {
            var assets = new CaseInsensitiveDictionary<string>();
            foreach (var file in ConfigFiles)
            {
                assets.Add(file.TargetConfigFileName, file.ToGame3Xml());
            }
            var compiled = CoalescedConverter.CompileFromMemory(assets);
            compiled.WriteToFile(Path.Combine(folder, fileName));
        }
    }
}
