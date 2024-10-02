﻿using LegendaryExplorerCore.Coalesced;
using LegendaryExplorerCore.Coalesced.Config;

namespace MassEffectModBuilder.Models
{
    public class ModConfigMergeFile : ModConfigFile
    {
        public string OutputFileName { get; private set; }

        public ModConfigMergeFile(string outputFileName)
        {
            if (!outputFileName.StartsWith(ConfigMerge.CONFIG_MERGE_PREFIX) 
                || !outputFileName.EndsWith(ConfigMerge.CONFIG_MERGE_EXTENSION)
                || outputFileName == $"{ConfigMerge.CONFIG_MERGE_PREFIX}{ConfigMerge.CONFIG_MERGE_EXTENSION}")
            {
                throw new Exception($"Invalid config merge file name {outputFileName}");
            }
            OutputFileName = outputFileName;
        }

        public override ModConfigClass GetOrCreateClass(string classFullPath)
        {
            var config = ClassConfigs.FirstOrDefault(x => x.ClassFullPath == classFullPath);
            return config ?? throw new Exception($"Could not find existing class with path {classFullPath}; you must specify the target config file when creating a new class config for a config merge");
        }

        public ModConfigClass GetOrCreateClass(string classFullPath, string targetConfigFile)
        {
            var config = ClassConfigs.FirstOrDefault(x => x.ClassFullPath == classFullPath);
            if (config == null)
            {
                config = new ModConfigClass(classFullPath, targetConfigFile);
                ClassConfigs.Add(config);
            }
            else if (config.TargetConfigFile != targetConfigFile)
            {
                throw new Exception($"Class config {classFullPath} has already been added, but with a different target file");
            }
            return config;
        }

        public void AddOrMergeClassConfig(ModConfigClass config)
        {
            if (config.TargetConfigFile == null)
            {
                throw new Exception("this class config cannot be added to a config merge; it has no target config file");
            }

            var existing = ClassConfigs.FirstOrDefault(x => x.ClassFullPath == config.ClassFullPath && x.TargetConfigFile == config.TargetConfigFile);
            if (existing == null)
            {
                ClassConfigs.Add(config);
                return;
            }
            else
            {
                // TODO merge it in
                throw new NotImplementedException();
            }
        }

        public IEnumerable<string> OutputFileContents()
        {
            List<string> lines = [];

            foreach (var section in ClassConfigs)
            {
                // output the comment, if any
                if (!string.IsNullOrWhiteSpace(section.Comment))
                {
                    lines.AddRange(section.Comment.Split("\n").Select(x => $"; {x}"));
                }
                // output the header
                lines.Add($"[{section.TargetConfigFile} {section.ClassFullPath}]");
                // output each property
                foreach (var (propName, prop) in section)
                {
                    foreach (var val in prop)
                    {
                        lines.AddRange(FormatPropertyLines(propName, val));
                    }
                }
                // output an empty line at the end of a class config
                lines.Add("");
            }
            
            return lines;
        }

        private static IEnumerable<string> FormatPropertyLines(string propertyName, CoalesceValue value)
        {
            IEnumerable<string> comment;
            if (!string.IsNullOrWhiteSpace(value.Comment))
            {
                comment = value.Comment.Split("\n").Select(x => $"; {x}");
            }
            else
            {
                comment = [];
            }
            var prefix = GetPrefix(value.ParseAction);
            return [.. comment, $"{prefix}{propertyName}={value.Value}"];
        }

        private static string GetPrefix(CoalesceParseAction action)
        {
            // TODO deal with double typed things properly?
            // I think I can just leave the other type in the name
            switch (action)
            {
                case CoalesceParseAction.AddUnique:
                    return "+";
                case CoalesceParseAction.Remove:
                    return "-";
                case CoalesceParseAction.RemoveProperty:
                    return "!";
                case CoalesceParseAction.New:
                    return ">";
                case CoalesceParseAction.None:
                case CoalesceParseAction.Add:
                default:
                    // could also return "." here but I think it looks cleaner this way
                    return "";
            }
        }
    }
}
