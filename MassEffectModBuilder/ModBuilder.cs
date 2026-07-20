using LegendaryExplorerCore;
using LegendaryExplorerCore.Packages;
using MassEffectModBuilder.DLC;
using MassEffectModBuilder.Folder;
using MassEffectModBuilder.M3Tasks;
using MassEffectModBuilder.Merge;

namespace MassEffectModBuilder
{
    public class ModBuilder
    {
        public ModBuilder(MEGame game, string modName, string developerName, string version, string description)
        {
            ModDesc = new ModDesc(game, modName, developerName, version, description);
            Game = game;
            ModName = modName;
            DeveloperName = developerName;
            Version = version;
            Description = description;
            ModFolderName = modName;
        }

        public string ModName { get; }

        public string ModFolderName { get; set; }

        public string DeveloperName { get;}

        public string Version { get; }

        public string Description { get; }

        public string? ModWebsite
        {
            get => ModDesc.ModSite;
            set => ModDesc.ModSite = value;
        }

        public ModBuilder RequiresTextureOverrideAsi()
        {
            var groupId = Game switch
            {
                MEGame.LE1 => 88,
                MEGame.LE2 => 89,
                MEGame.LE3 => 87,
                _ => 0
            };
            if (groupId != 0)
            {
                ModDesc.RequiresAsi(groupId);
            }
            return this;
        }

        /// <summary>
        ///  The game this mod targets
        /// </summary>
        public MEGame Game { get; }

        protected readonly List<IModBuilderTask> ModBuilderTasks = [];

        public ModDesc ModDesc { get; private set; }

        protected readonly HashSet<string> MergeMods = [];

        /// <summary>
        /// Allows you to add one or more DLCs to this mod
        /// </summary>
        /// <param name="dlc"></param>
        public ModBuilder WithDlc(DlcBuilder dlc, bool alwaysInstall = true)
        {
            return AddTask(new DlcBuilderTask(dlc, alwaysInstall));
        }

        /// <summary>
        /// Allows you to add one or more m3m merge mods to this mod
        /// </summary>
        /// <param name="merge"></param>
        public ModBuilder WithMergeMod(MergeBuilder merge, bool alwaysInstall = true, bool compile = true)
        {
            MergeMods.Add(merge.M3mName);
            return AddTask(new MergeBuilderTask(merge, alwaysInstall));
        }

        /// <summary>
        /// Allows you to add one or more extra folders to this mod (options, patches, templates, etc)
        /// </summary>
        /// <param name="folder"></param>
        public ModBuilder WithExtraFolder(FolderBuilder folder)
        {
            return AddTask(new FolderBuilderTask(folder));
        }

        /// <summary>
        /// Allows you to add tasks that change the mod outside of any subfolders
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public virtual ModBuilder AddTask(IModBuilderTask task)
        {
            ModBuilderTasks.Add(task);
            return this;
        }

        public virtual ModBuilder AddTasks(params IModBuilderTask[] tasks)
        {
            foreach (var task in tasks)
            {
                AddTask(task);
            }
            return this;
        }

        public virtual void Build(string modLibraryBase, bool compileMergeMods = true)
        {
            var modOutputBasePath = Path.Combine(modLibraryBase, Game.ToString(), ModFolderName);
            Console.WriteLine($"Starting mod build into {modOutputBasePath}");
            // init the library
            LegendaryExplorerCoreLib.InitLib(TaskScheduler.Current, x => Console.Error.WriteLine($"Failed to save package: {x}"));

            var context = GetNewContext(modOutputBasePath, modLibraryBase);

            if (Directory.Exists(modOutputBasePath))
            {
                Directory.Delete(modOutputBasePath, true);
            }
            Directory.CreateDirectory(modOutputBasePath);

            if (compileMergeMods)
            {
                foreach (var mergeMod in MergeMods)
                {
                    AddTask(new CompileMergeMod(mergeMod));
                }
            }

            foreach (var task in ModBuilderTasks)
            {
                task.RunModTask(context);
            }

            ModDesc.OutputModDesc(modOutputBasePath);
        }

        protected virtual ModBuilderContext GetNewContext(string modOutputBasePath, string modLibraryBase)
        {
            return new ModBuilderContext(this, modOutputBasePath, modLibraryBase);
        }

        public class DlcBuilderTask(DlcBuilder builder, bool alwaysInstall) : IModBuilderTask
        {
            public void RunModTask(ModBuilderContext context)
            {
                if (alwaysInstall)
                {
                    context.ModDesc.AddDlc(builder.DlcFolderName, builder.DlcFolderName);
                }
                builder.Build(context);
            }
        }

        public class MergeBuilderTask(MergeBuilder builder, bool alwaysInstall) : IModBuilderTask
        {
            public void RunModTask(ModBuilderContext context)
            {
                if (alwaysInstall)
                {
                    context.ModDesc.AddMerge(builder.M3mName);
                }
                // make sure the merge folder exists
                Directory.CreateDirectory(context.MergeModsFolder);
                var mergeContext = new MergeBuilderContext(builder, context);
                builder.Build(mergeContext);
            }
        }

        public class FolderBuilderTask(FolderBuilder builder) : IModBuilderTask
        {
            public void RunModTask(ModBuilderContext context)
            {
                builder.Build(context);
            }
        }
    }

    //public class ModBuilder<T> : ModBuilder where T : new()
    //{
    //    public ModBuilder AddTask(IModBuilderTask<T> task)
    //    {
    //        ModBuilderTasks.Add(task);
    //        return this;
    //    }

    //    public ModBuilder AddTasks(params IModBuilderTask<T>[] tasks)
    //    {
    //        foreach (var task in tasks)
    //        {
    //            AddTask(task);
    //        }
    //        return this;
    //    }

    //    protected override ModBuilderContext GetNewContext(string modOutputBasePath)
    //    {
    //        return new ModBuilderContext<T>(this, modOutputBasePath);
    //    }
    //}
}
