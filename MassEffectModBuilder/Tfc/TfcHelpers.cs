using LegendaryExplorerCore.Helpers;

namespace MassEffectModBuilder.Tfc
{
    internal class TfcHelpers
    {
        public static void EnsureTfcExists(string tfcPath)
        {
            if (!File.Exists(tfcPath))
            {
                try
                {
                    Guid tfcGuid = Guid.NewGuid(); // make new guid as storage
                    using var fs = new FileStream(tfcPath, FileMode.OpenOrCreate, FileAccess.Write);
                    fs.WriteGuid(tfcGuid);
                }
                catch (Exception e)
                {
                    throw new Exception("Problem creating new TFC file " + tfcPath + ": " + e.Message);
                }
            }
        }
    }
}
