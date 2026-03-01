using LegendaryExplorerCore.Packages;
using LegendaryExplorerCore.TLK;
using MassEffectModBuilder.LEXHelpers;
using Game1Huffman = LegendaryExplorerCore.TLK.ME1.HuffmanCompression;
using Game23Huffman = LegendaryExplorerCore.TLK.ME2ME3.HuffmanCompression;


namespace MassEffectModBuilder.DLC
{
    /// <summary>
    /// Allows you to build all the TLKs for a dlc.
    /// </summary>
    /// <param name="Game"></param>
    public record class TlkBuilder(MEGame Game)
    {
        /// <summary>
        /// Whether to log a warning if a stringref is missing any localizations. Default false.
        /// </summary>
        public bool WarnOnMissingLocalization { get; set; } = false;

        private readonly Dictionary<int, StringRefBuilder> stringRefs = [];

        /// <summary>
        /// Imports an XML file exported from a Game1 tlk
        /// </summary>
        /// <param name="xmlPath">The path to the XML file</param>
        /// <param name="locale">The locale this xml file is for.</param>
        /// <param name="female">whether this is the female specific strings.</param>
        public void ImportME1Xml(string xmlPath, MELocalization locale, bool female)
        {
            var stringRefs = TlkHelpers.ParseGame1TlkXml(xmlPath);
            foreach (var stringRef in stringRefs)
            {
                AddStringRef(stringRef.StringID, locale, stringRef.Data, female);
            }
        }

        /// <summary>
        /// Imports an XML exported from a Game2/3 TLK file.
        /// </summary>
        /// <param name="xmlPath">The path to the xml file.</param>
        /// <param name="locale">The locale this xml is for</param>
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

        /// <summary>
        /// Adds a single stringref, for a specific id, locale, and gender.
        /// </summary>
        /// <param name="id">The stringref ID</param>
        /// <param name="locale">The locale for this stringref</param>
        /// <param name="data">The string data</param>
        /// <param name="female">whether this is a female specific string. Otherwise, it is counted as male/gender neutral</param>
        public void AddStringRef(int id, MELocalization locale, string data, bool female)
        {
            if (!stringRefs.TryGetValue(id, out var stringRef))
            {
                stringRef = new StringRefBuilder(id);
                stringRefs.Add(id, stringRef);
            }
            stringRef.AddLocalization(locale, data, female);
        }

        /// <summary>
        /// Adds a stringref for every localization.
        /// </summary>
        /// <param name="id">Stringref id</param>
        /// <param name="data">the string data</param>
        /// <param name="female">whether this is a female specific stringref</param>
        public void AddConstantStringRef(int id, string data, bool female = false)
        {
            AddStringRef(id, MELocalization.DEU, data, female);
            AddStringRef(id, MELocalization.ESN, data, female);
            AddStringRef(id, MELocalization.FRA, data, female);
            AddStringRef(id, MELocalization.INT, data, female);
            AddStringRef(id, MELocalization.ITA, data, female);
            AddStringRef(id, MELocalization.JPN, data, female);
            AddStringRef(id, MELocalization.POL, data, female);
            AddStringRef(id, MELocalization.RUS, data, female);
        }

        /// <summary>
        /// Output Game 1 tlk files for all localizations
        /// </summary>
        /// <param name="outputFolder">The folder to output them in</param>
        /// <param name="fileNameBase">The base of the filenames</param>
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

        /// <summary>
        /// Output Game2/3 TLK files for all localizations
        /// </summary>
        /// <param name="outputFolder">The folder in which to output them</param>
        /// <param name="filenameBase"></param>
        public void OutputGame23Tlks(string outputFolder, string filenameBase)
        {
            // game 2/3 tlks have a stringref for the localization. I am not sure if it is important, but it is easy enough to make it match vanilla TLKs
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

        public void InitTlk(int tlkBaseId, string modInternalName, string ModDlcFolderName, int? Game2ModuleNumber = null)
        {
            switch (Game)
            {
                //case MEGame.ME1:
                case MEGame.LE1:
                    AddConstantStringRef(tlkBaseId, modInternalName);
                    break;
                case MEGame.ME2:
                case MEGame.LE2:
                case MEGame.ME3:
                case MEGame.LE3:
                    AddConstantStringRef(tlkBaseId, modInternalName);
                    if (Game.IsGame2())
                    {
                        // DLC_Module#
                        AddConstantStringRef(tlkBaseId + 1, $"DLC_{Game2ModuleNumber}");
                    }
                    if (Game.IsGame3())
                    {
                        // DLC_MOD_Whatever
                        AddConstantStringRef(tlkBaseId + 1, $"{ModDlcFolderName}");
                    }

                    // add the localization
                    AddStringRef(tlkBaseId + 2, MELocalization.DEU, "de-de", false);
                    AddStringRef(tlkBaseId + 2, MELocalization.ESN, "es-es", false);
                    AddStringRef(tlkBaseId + 2, MELocalization.FRA, "fr-fr", false);
                    AddStringRef(tlkBaseId + 2, MELocalization.INT, "en-us", false);
                    AddStringRef(tlkBaseId + 2, MELocalization.ITA, "it-it", false);
                    AddStringRef(tlkBaseId + 2, MELocalization.JPN, "jp-jp", false);
                    AddStringRef(tlkBaseId + 2, MELocalization.POL, "pl-pl", false);
                    AddStringRef(tlkBaseId + 2, MELocalization.RUS, "ru-ru", false);

                    // add the male/female entries
                    AddConstantStringRef(tlkBaseId + 3, "Male");
                    AddConstantStringRef(tlkBaseId + 3, "Female", true);
                    break;
                default:
                    throw new ApplicationException($"unsupported game {Game}");
            }
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
                //if (WarnOnMissingLocalization)
                //{
                //    Console.WriteLine($"Warning: you asked for the {locale} version of stringref {Id} but there is no localization; falling back to English");
                //}
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
