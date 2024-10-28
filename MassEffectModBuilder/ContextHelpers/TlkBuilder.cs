using LegendaryExplorerCore.Packages;
using LegendaryExplorerCore.TLK;
using MassEffectModBuilder.LEXHelpers;
using Game1Huffman = LegendaryExplorerCore.TLK.ME1.HuffmanCompression;
using Game23Huffman = LegendaryExplorerCore.TLK.ME2ME3.HuffmanCompression;


namespace MassEffectModBuilder.ContextHelpers
{
    public record class TlkBuilder(MEGame Game)
    {
        public static bool WarnOnMissingLocalization = false;

        private readonly Dictionary<int, StringRefBuilder> stringRefs = [];
        public void ImportME1Xml(string xmlPath, MELocalization locale, bool female)
        {
            var stringRefs = TlkHelpers.ParseGame1TlkXml(xmlPath);
            foreach (var stringRef in stringRefs)
            {
                AddStringRef(stringRef.CalculatedID, locale, stringRef.Data, female);
            }
        }

        public void ImportME2ME3Xml(string xmlPath, MELocalization locale)
        {
            var maleDict = new Dictionary<int, TLKStringRef>();
            var femaleDict = new Dictionary<int, TLKStringRef>();
            var stringRefs = TlkHelpers.ParseGame23TlkXml(xmlPath);
            // if we encounter something for the first time, put it in the male lines dictionary
            // if we encounter a duplicate, make it the female line
            foreach (TLKStringRef strRef in stringRefs)
            {
                if (maleDict.ContainsKey(strRef.CalculatedID))
                {
                    femaleDict[strRef.CalculatedID] = strRef;
                }
                else
                {
                    maleDict[strRef.CalculatedID] = strRef;
                }
            }

            foreach ((int id, TLKStringRef strRef) in maleDict)
            {
                AddStringRef(id, locale, strRef.Data, false);
                if (femaleDict.TryGetValue(id, out var femaledata))
                {
                    AddStringRef(id, locale, femaledata.Data, true);
                }
            }
        }

        public void AddStringRef(int id, MELocalization locale, string data, bool female)
        {
            if (!stringRefs.TryGetValue(id, out var stringRef))
            {
                stringRef = new StringRefBuilder(id);
                stringRefs.Add(id, stringRef);
            }
            stringRef.AddLocalization(locale, data, female);

        }

        public void OutputGame1Tlks(string outputFolder, string fileNameBase)
        {
            // for each localization, I need to create a pcc file containing the expected exports
            OutputSingleGame1Tlk(outputFolder, fileNameBase + "_GlobalTlk.pcc", MELocalization.INT);
            OutputSingleGame1Tlk(outputFolder, fileNameBase + "_GlobalTlk_DE.pcc", MELocalization.DEU, fileNameBase + "_GlobalTlk_GE.pcc");
            OutputSingleGame1Tlk(outputFolder, fileNameBase + "_GlobalTlk_ES.pcc", MELocalization.ESN);
            OutputSingleGame1Tlk(outputFolder, fileNameBase + "_GlobalTlk_FR.pcc", MELocalization.FRA, fileNameBase + "_GlobalTlk_FE.pcc");
            OutputSingleGame1Tlk(outputFolder, fileNameBase + "_GlobalTlk_IT.pcc", MELocalization.ITA, fileNameBase + "_GlobalTlk_IE.pcc");
            OutputSingleGame1Tlk(outputFolder, fileNameBase + "_GlobalTlk_JA.pcc", MELocalization.JPN);
            OutputSingleGame1Tlk(outputFolder, fileNameBase + "_GlobalTlk_PL.pcc", MELocalization.POL, fileNameBase + "_GlobalTlk_PLPC.pcc");
            OutputSingleGame1Tlk(outputFolder, fileNameBase + "_GlobalTlk_RU.pcc", MELocalization.RUS, fileNameBase + "_GlobalTlk_RA.pcc");
        }

        private void OutputSingleGame1Tlk(string outputFolder, string fileName, MELocalization locale, params string[] duplicateFiles)
        {
            // create the actual package
            var pkg = MEPackageHandler.CreateAndOpenPackage(Path.Combine(outputFolder, fileName), Game);

            // create the empty exports
            var femaleExport = ExportCreator.CreateExport(pkg, "GlobalTlk_tlk", "BioTlkFile", indexed: false);
            var maleExport = ExportCreator.CreateExport(pkg, "GlobalTlk_tlk_M", "BioTlkFile", indexed: false);

            var femaleHuffman = new Game1Huffman();

            var femaleTlks = new List<TLKStringRef>();
            foreach (var tlk in stringRefs)
            {
                femaleTlks.Add(new TLKStringRef(tlk.Key, tlk.Value.GetData(locale, true)));
            }

            femaleHuffman.LoadInputData(femaleTlks);
            femaleHuffman.SerializeTalkfileToExport(femaleExport);

            var maleHuffman = new Game1Huffman();

            var maleTlks = new List<TLKStringRef>();
            foreach (var tlk in stringRefs)
            {
                maleTlks.Add(new TLKStringRef(tlk.Key, tlk.Value.GetData(locale, false)));
            }

            maleHuffman.LoadInputData(maleTlks);
            maleHuffman.SerializeTalkfileToExport(maleExport);

            pkg.Save();

            foreach (var dupe in duplicateFiles)
            {
                File.Copy(Path.Combine(outputFolder, fileName), Path.Combine(outputFolder, dupe));
            }
        }

        public void OutputGame23Tlks(string outputFolder, string filenameBase)
        {
            OutputSingleGame23Tlk(Path.Combine(outputFolder, filenameBase + "_DEU.tlk"), MELocalization.DEU);
            OutputSingleGame23Tlk(Path.Combine(outputFolder, filenameBase + "_ESN.tlk"), MELocalization.ESN);
            OutputSingleGame23Tlk(Path.Combine(outputFolder, filenameBase + "_FRA.tlk"), MELocalization.FRA);
            OutputSingleGame23Tlk(Path.Combine(outputFolder, filenameBase + "_INT.tlk"), MELocalization.INT);
            OutputSingleGame23Tlk(Path.Combine(outputFolder, filenameBase + "_ITA.tlk"), MELocalization.ITA);
            OutputSingleGame23Tlk(Path.Combine(outputFolder, filenameBase + "_JPN.tlk"), MELocalization.JPN);
            OutputSingleGame23Tlk(Path.Combine(outputFolder, filenameBase + "_POL.tlk"), MELocalization.POL);
            OutputSingleGame23Tlk(Path.Combine(outputFolder, filenameBase + "_RUS.tlk"), MELocalization.RUS);
        }

        private void OutputSingleGame23Tlk(string filePath, MELocalization locale)
        {
            var outputStringRefs = new List<TLKStringRef>();
            foreach (var tlk in stringRefs)
            {
                outputStringRefs.Add(new TLKStringRef(tlk.Key, tlk.Value.GetData(locale, false)));
            }
            foreach (var tlk in stringRefs)
            {
                // don't output duplicate strings; the game will output the male line if there is no distinct female line
                if (tlk.Value.HasDistinctFemaleString(locale))
                {
                    outputStringRefs.Add(new TLKStringRef(tlk.Key, tlk.Value.GetData(locale, true)));
                }
            }

            Game23Huffman.SaveToTlkFile(filePath, outputStringRefs);
        }
    }

    public record class StringRefBuilder(int Id)
    {
        private readonly Dictionary<MELocalization, (string? maleData, string? femaleData)> localizedData = [];
        public void AddLocalization(MELocalization locale, string data, bool female)
        {
            if (!localizedData.TryGetValue(locale, out var localizedString))
            {
                localizedString = (null, null);
                localizedData.Add(locale, localizedString);
            }
            if (female)
            {
                localizedString.femaleData = data;
            }
            else
            {
                localizedString.maleData = data;
            }
            localizedData[locale] = localizedString;
        }
        public string GetData(MELocalization locale, bool female = false)
        {
            var (maleData, femaleData) = GetLocalizedOrFallback(locale);
            if (maleData == null)
            {
                throw new Exception($"You have set up your tlks wrong. There is no male version for stringref {Id} in localization {locale}");
            }
            return female && femaleData != null ? femaleData : maleData;
        }

        private (string? maleData, string? femaleData) GetLocalizedOrFallback(MELocalization locale)
        {
            if (locale != MELocalization.INT)
            {
                if (localizedData.TryGetValue(locale, out var nonIntData))
                {
                    return nonIntData;
                }
                if (TlkBuilder.WarnOnMissingLocalization)
                {
                    Console.WriteLine($"Warning: you asked for the {locale} version of stringref {Id} but there is no localization; falling back to English");
                }
                // fall back to int if there is no specific localization
                locale = MELocalization.INT;
            }
            // try to get the int version
            if (localizedData.TryGetValue(locale, out var data))
            {
                return data;
            }

            throw new Exception($"You asked for stringref {Id} but it has no localized or Int data");
        }

        public bool HasDistinctFemaleString(MELocalization locale)
        {
            var (maleData, femaleData) = GetLocalizedOrFallback(locale);
            if (maleData == null)
            {
                throw new Exception($"You have set up your tlks wrong. There is no male version for strinref {Id} in localization {locale}");
            }
            return femaleData != null && maleData != femaleData;
        }
    }
}
