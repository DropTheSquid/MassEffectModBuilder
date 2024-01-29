using LegendaryExplorerCore.Packages;
using LegendaryExplorerCore.Packages.CloningImportingAndRelinking;
using LegendaryExplorerCore.Unreal;

namespace MassEffectModBuilder.LEXHelpers
{
    public static class PackageHelpers
    {
        public static ExportEntry? GetObjectReferencer(this IMEPackage pcc)
        {
            if (pcc.Flags.HasFlag(UnrealFlags.EPackageFlags.Map))
            {
                return null;
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
            if (pcc.Flags.HasFlag(UnrealFlags.EPackageFlags.Map))
            {
                throw new Exception("Can't add an object referencer to map file");
            }

            return ExportCreator.CreateExport(pcc, name, "ObjectReferencer", indexed: indexed);
        }

        public static void AddToObjectReferencer(this ExportEntry referencer, params ExportEntry[] entries)
        {
            AddToObjectReferencer(referencer, (IEnumerable<ExportEntry>)entries);
        }

        public static void AddToObjectReferencer(this ExportEntry referencer, IEnumerable<ExportEntry> entries)
        {
            if (referencer.ClassName != "ObjectReferencer")
            {
                throw new Exception($"You are trying to add referenced objects to a {referencer.ClassName}");
            }
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
