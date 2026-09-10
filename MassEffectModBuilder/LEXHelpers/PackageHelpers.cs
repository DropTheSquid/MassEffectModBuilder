using LegendaryExplorerCore.GameFilesystem;
using LegendaryExplorerCore.Misc;
using LegendaryExplorerCore.Packages;
using LegendaryExplorerCore.Textures;
using LegendaryExplorerCore.Unreal;
using LegendaryExplorerCore.Unreal.Classes;
using MassEffectModBuilder.Tfc;

namespace MassEffectModBuilder.LEXHelpers
{
    public static class PackageHelpers
    {
        public static ExportEntry EnsurePackagePathExists(this IMEPackage pcc, IEnumerable<string> path, bool forcedExport = true)
        {
            ExportEntry export = null;
            foreach (string name in path)
            {
                export = ExportCreator.CreatePackageExport(pcc, NameReference.FromInstancedString(name), export, forcedExport: forcedExport);
            }
            return export!;
        }

        public static ExportEntry? GetObjectReferencer(this IMEPackage pcc)
        {
            if (pcc.Flags.HasFlag(UnrealFlags.EPackageFlags.Map))
            {
                return null;
            }

            return pcc.Exports.FirstOrDefault(x => x.ClassName == "ObjectReferencer" && !x.IsDefaultObject);
        }

        public static ExportEntry GetOrCreateObjectReferencer(this IMEPackage pcc)
        {
            if (pcc.Flags.HasFlag(UnrealFlags.EPackageFlags.Map))
            {
                throw new Exception("You cannot have an object referencer in a map file");
            }

            var objRef = pcc.GetObjectReferencer();
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

            foreach (var entry in entries)
            {
                referenceProp.Add(new ObjectProperty(entry));
                referencer.WriteProperty(referenceProp);
            }
        }

        public static IEnumerable<IEntry> GetReferencedEntries(this ExportEntry objectReferencer)
        {
            if (objectReferencer == null || objectReferencer.ClassName != "ObjectReferencer")
            {
                return [];
            }

            var referenceProp = objectReferencer.GetProperties()?.GetProp<ArrayProperty<ObjectProperty>>("ReferencedObjects");
            if (referenceProp == null)
            {
                return [];
            }

            return referenceProp.Select(x => x.ResolveToEntry(objectReferencer.FileRef));
        }

        public static bool TryGetHighestMountedOfficialFile(string desiredPackageName, MEGame game, out string resultPath, string? gameRootOverride = null)
        {
            var loadedFiles = new CaseInsensitiveDictionary<string>();
            string FauxStartupPath = Path.Combine("DLC_METR_Patch01", "CookedPCConsole", "Startup.pcc");

            var bgPath = MEDirectories.GetBioGamePath(game, gameRootOverride);
            if (bgPath != null)
            {
                IEnumerable<string> directories;
                if (game is MEGame.UDK)
                {
                    directories = new[] { UDKDirectory.GetScriptPath(gameRootOverride), bgPath };
                }
                else
                {
                    directories = MELoadedDLC.GetEnabledDLCFolders(game, gameRootOverride)
                        .Where(dir => MELoadedDLC.IsOfficialDLC(dir, game))
                        .OrderBy(dir => MELoadedDLC.GetMountPriority(dir, game))
                        .Prepend(bgPath);
                }
                foreach (string directory in directories)
                {
                    foreach (string filePath in MELoadedFiles.GetCookedFiles(game, directory))
                    {
                        string fileName = Path.GetFileName(filePath);
                        if (game == MEGame.LE3 && filePath.EndsWith(FauxStartupPath, StringComparison.InvariantCultureIgnoreCase))
                        {
                            continue; // This file is not used by game and will break lots of stuff if we don't filter it out. This is a bug in how LE3 was cooked
                        }
                        if (fileName != null) loadedFiles[fileName] = filePath;
                    }
                }
            }

            return loadedFiles.TryGetValue(desiredPackageName, out resultPath);
        }

        public static IEntry? FindClass(this IMEPackage pcc, string className)
        {
            foreach (ExportEntry exp in pcc.Exports)
            {
                //find header references
                if (exp.ObjectName.Name == className && exp.ClassName == "Class")
                {
                    return exp;
                }
            }

            foreach (ImportEntry import in pcc.Imports)
            {
                if (import.ClassName == className)
                {
                    return import;
                }
            }

            return null;
        }

        public struct TextureInfo
        {
            public PixelFormat PixelFormat { get; set; }
            public bool SRGB { get; set; }
            public bool Mipped { get; set; }
            public string? TextureGroup { get; set; }
            public string? TfcPath { get; set; }
        }

        public static ExportEntry CreateTextureFromImageFile(this IMEPackage package, string textureIFP, string imageFilePath, TextureInfo textureInfo)
        {
            var image = Image.LoadFromFile(imageFilePath, PixelFormat.ARGB);
            return package.CreateTextureFromImage(textureIFP, image, textureInfo);
        }

        public static ExportEntry CreateTextureFromImage(this IMEPackage package, string textureIFP, Image image, TextureInfo textureInfo)
        {
            if (textureInfo.TfcPath != null)
            {
                TfcHelpers.EnsureTfcExists(textureInfo.TfcPath);
            }
            var segments = textureIFP.Split('.');
            var name = segments.Last();
            var packagePath = segments.Take(segments.Length - 1).ToArray();

            var parent = package.EnsurePackagePathExists(packagePath);

            var texExport = Texture2D.CreateTexture(package, name, image.mipMaps[0].origWidth, image.mipMaps[0].origHeight, textureInfo.PixelFormat, textureInfo.Mipped, parent, textureInfo.TextureGroup);

            var texture = new Texture2D(texExport);
            texture.Replace(image, texExport.GetProperties(), null, Path.GetFileNameWithoutExtension(textureInfo.TfcPath), null, textureInfo.TfcPath == null, textureInfo.PixelFormat);

            return texExport;
        }

        public static void ReplaceTextureFromFile(this IMEPackage package, string textureIFP, string imageFilePath, TextureInfo textureInfo)
        {
            var image = Image.LoadFromFile(imageFilePath, PixelFormat.ARGB);
            package.ReplaceTextureFromImage(textureIFP, image, textureInfo);
        }

        public static void ReplaceTextureFromImage(this IMEPackage package, string textureIFP, Image image, TextureInfo textureInfo)
        {
            if (textureInfo.TfcPath != null)
            {
                TfcHelpers.EnsureTfcExists(textureInfo.TfcPath);
            }
            var texExport = package.FindExport(textureIFP);
            //var props = texExport.GetProperties();
            //var tfcName = textureInfo.TfcPath == null ? null : Path.GetFileNameWithoutExtension(textureInfo.TfcPath);
            var texture = new Texture2D(texExport);
            //texture.Replace(image, props, forcedTFCName: tfcName, forcedTFCPath: textureInfo.TfcPath, isPackageStored: textureInfo.TfcPath == null, forcedNewFormat: textureInfo.PixelFormat);
            texture.Replace(image, texExport.GetProperties(), null, Path.GetFileNameWithoutExtension(textureInfo.TfcPath), null, textureInfo.TfcPath == null, textureInfo.PixelFormat);

        }
    }
}
