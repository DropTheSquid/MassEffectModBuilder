using LegendaryExplorerCore.Packages;
using LegendaryExplorerCore.Packages.CloningImportingAndRelinking;
using LegendaryExplorerCore.Unreal;

namespace MassEffectModBuilder.LEXHelpers
{
    public static class PackageHelpers
    {
        public static ExportEntry GetObjectReferencer(this IMEPackage pcc)
        {
            if (pcc.Flags.HasFlag(UnrealFlags.EPackageFlags.Map))
            {
                throw new Exception("Can't add an object referencer to map file");
            }

            var objRef = pcc.Exports.FirstOrDefault(x => x.ClassName == "ObjectReferencer" && !x.IsDefaultObject);
            if (objRef == null)
            {
                return pcc.AddObjectReferencer();
            }
            return objRef;
        }

        public static ExportEntry AddObjectReferencer(this IMEPackage pcc, string name = "ObjectReferencer", bool indexed = true)
        {
            return ExportCreator.CreateExport(pcc, name, "ObjectReferencer", indexed: indexed);
        }

        public static void AddToObjectReferencer(this IMEPackage pcc, params ExportEntry[] entries)
        {
            var referencer = pcc.GetObjectReferencer();
            var referenceProp = referencer.GetProperties()?.GetProp<ArrayProperty<ObjectProperty>>("ReferencedObjects");
            referenceProp ??= new ArrayProperty<ObjectProperty>("ReferencedObjects");

            foreach (var entry in entries )
            {
                referenceProp.Add(new ObjectProperty(entry));
                referencer.WriteProperty(referenceProp);
            }
        }
    }
}
