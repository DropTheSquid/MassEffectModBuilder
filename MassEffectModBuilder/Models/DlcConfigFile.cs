namespace MassEffectModBuilder.Models
{
    /// <summary>
    /// Represents either an ME2/LE2 ini file within a mod or an ME3/LE3 xml file that will be compiled into the coalesced.bin of the mod
    /// </summary>
    /// <param name="TargetConfigFileName">the ini/xml this mod should extend</param>
    public class DlcConfigFile : ModConfigFile
    {
        public DlcConfigFile(string targetConfigFileName) 
        {
            TargetConfigFileName = targetConfigFileName;
        }
        // This represents one or more of either an ME2 ini file in a DLC mod or an xml that gets compiled into the coalesced of an ME3 mod
        // does not support LE1. use configMerge version for that.
        // OT1 TBD

        public string TargetConfigFileName { get; private set; }

        public override ModConfigClass GetOrCreateClass(string classFullPath)
        {
            var config = ClassConfigs.FirstOrDefault(x => x.ClassFullPath == classFullPath);
            if (config == null)
            {
                config = new ModConfigClass(classFullPath);
                ClassConfigs.Add(config);
            }
            return config;
        }

        //public void OutputGame2Configs(string outputFolder)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
