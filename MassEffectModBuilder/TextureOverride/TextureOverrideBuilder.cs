using LegendaryExplorerCore.Packages;
using LegendaryExplorerCore.Packages.CloningImportingAndRelinking;
using LegendaryExplorerCore.Textures;
using MassEffectModBuilder.LEXHelpers;
using System.Text;
using static MassEffectModBuilder.LEXHelpers.PackageHelpers;

namespace MassEffectModBuilder.TextureOverride
{
    public class TextureOverrideBuilder(string toName)
    {
        public readonly struct TextureOverrideInfo
        {
            // gets a texture by one of several methods:
            // an already existing texture, which will be cloned
            public readonly string ExistingIFP { get; init; }

            // a file, which will be imported using the provided textureInfo
            public readonly string? TextureFilePath { get; init; }

            // an Image object that has already been imported/processed and which will be turned into a Texture2d export using the provided TextureInfo
            public readonly Image? TextureImage { get; init; }
            public readonly TextureInfo TextureInfo { get; init; }
        }

        public string TextureOverrideName { get; } = toName;
        public string? TfcPath { get; set; }

        protected Dictionary<string, TextureOverrideInfo> Textures = [];

        public TextureOverrideBuilder WithTextureFromFile(string textureIfp, string textureFilePath, TextureInfo textureInfo)
        {
            if (!File.Exists(textureFilePath))
            {
                throw new FileNotFoundException("Cannot find texture file", textureFilePath);
            }
            if (Textures.ContainsKey(textureIfp))
            {
                throw new InvalidOperationException($"there is already a texture at this path: {textureIfp}");
            }
            Textures.Add(textureIfp, new TextureOverrideInfo { TextureFilePath = textureFilePath, TextureInfo = textureInfo });
            return this;
        }

        public TextureOverrideBuilder WithTextureFromImage(string textureIfp, Image image, TextureInfo textureInfo)
        {
            if (Textures.ContainsKey(textureIfp))
            {
                throw new InvalidOperationException($"there is already a texture at this path: {textureIfp}");
            }
            Textures.Add(textureIfp, new TextureOverrideInfo { TextureImage = image, TextureInfo = textureInfo });
            return this;
        }

        public TextureOverrideBuilder WithExistingTexture(string textureIfp, string existingTextureIfp)
        {
            if (Textures.ContainsKey(textureIfp))
            {
                throw new InvalidOperationException($"there is already a texture at this path: {textureIfp}");
            }
            Textures.Add(textureIfp, new TextureOverrideInfo { ExistingIFP = existingTextureIfp });
            return this;
        }

        public void Build(string containingFolder, ModBuilderContext context)
        {
            var TOPackageName = $"TO_{TextureOverrideName}.pcc";
            var TOPackage = MEPackageHandler.CreateAndOpenPackage(Path.Combine(containingFolder, TOPackageName), context.Game);

            foreach (var (textureIfp, textureOverrideInfo) in Textures)
            {
                if (textureOverrideInfo.ExistingIFP != null)
                {
                    var segments = textureIfp.Split('.');
                    var name = segments.Last();
                    var packagePath = segments.Take(segments.Length - 1).ToArray();

                    var parent = TOPackage.EnsurePackagePathExists(packagePath);
                    var existingExport = TOPackage.FindExport(textureOverrideInfo.ExistingIFP);
                    var newEntry = EntryCloner.CloneEntry(existingExport, incrementIndex: false, newParentUIndex: parent.UIndex);
                    newEntry.ObjectName = name;
                }
                else if (textureOverrideInfo.TextureImage != null)
                {
                    var textureInfo = textureOverrideInfo.TextureInfo;
                    textureInfo.TfcPath ??= TfcPath;
                    TOPackage.CreateTextureFromImage(textureIfp, textureOverrideInfo.TextureImage, textureInfo);
                }
                else if (textureOverrideInfo.TextureFilePath != null)
                {
                    var textureInfo = textureOverrideInfo.TextureInfo;
                    textureInfo.TfcPath ??= TfcPath;
                    TOPackage.CreateTextureFromImageFile(textureIfp, textureOverrideInfo.TextureFilePath, textureInfo);
                }
                else
                {
                    throw new InvalidOperationException($"invalid texture override texture {textureIfp}");
                }
            }

            TOPackage.Save();

            // output the m3to file
            var m3toContent = new StringBuilder();
            m3toContent.AppendLine("{");
            m3toContent.AppendLine(@$"    ""Game"": ""{context.Game.ToString()}"",");
            m3toContent.AppendLine(@"    ""Textures"": [");
            var textureContent = new List<string>();
            // output both a TO pcc and an m3to file
            foreach (var (texturePath, _) in Textures)
            {
                textureContent.Add($@"        {{
            ""sourcePackage"": ""{TOPackageName}"",
            ""textureifp"": ""{texturePath}""
        }}");
            }
            m3toContent.AppendLine(string.Join(",\n", textureContent));
            m3toContent.AppendLine("   ]");
            m3toContent.AppendLine("}");

            File.WriteAllText(Path.Combine(containingFolder, $"TextureOverride-{TextureOverrideName}.m3to"), m3toContent.ToString());
        }
    }
}
