using LegendaryExplorerCore.Packages;
using MassEffectModBuilder.Package;

namespace MassEffectModBuilder.DLC
{
    public class DlcBuilder
    {
        /// <summary>
        /// The name of the DLC folder WITHOUT the DLC_MOD_ portion
        /// </summary>
        public required string DlcName { get; set; }

        public string DlcFolderName => $"DLC_MOD_{DlcName}";

        /// <summary>
        /// Internal name; appears in mount files and default tlks
        /// </summary>
        public required string InternalName { get; set; }

        /// <summary>
        /// the base tlk entry for this mod. This and several later entries will be reserved
        /// </summary>
        public required int InternalNameTlkId { get; set; }

        /// <summary>
        /// The mount priority of this DLC mod
        /// </summary>
        public required int MountPriority { get; set; }

        /// <summary>
        /// The module number of this DLC. Only applicable to ME2/LE2
        /// </summary>
        public int? Game2ModuleNumber { get; set; }

        protected readonly List<IDlcTask> DlcBuilderTasks = [];

        public DlcBuilder WithPackage(PackageBuilder package)
        {
            AddTask(new BuildPackageTask(package));
            return this;
        }

        public virtual DlcBuilder AddTask(IDlcTask task)
        {
            DlcBuilderTasks.Add(task);
            return this;
        }

        public virtual DlcBuilder AddTasks(params IDlcTask[] tasks)
        {
            foreach (var task in tasks)
            {
                AddTask(task);
            }
            return this;
        }

        public void Build(ModBuilderContext modContext)
        {
            var context = new DlcBuilderContext(this, modContext);

            // clean any older version of things
            if (Directory.Exists(context.DLCBaseFolder))
            {
                Directory.Delete(context.DLCBaseFolder, true);
            }

            // create the directories
            Directory.CreateDirectory(context.CookedFolderPath);

            // insert a few tasks that should run first
            DlcBuilderTasks.InsertRange(0, new DlcConfigInitTask(), new TlkInitTask());

            // add a few tasks that should run last
            AddTasks(new AutoLoadMountOutputTask(), new DlcConfigOutputTask(), new TlkOutpuTask(), new ConfigMergeOutputTask());

            foreach (var task in DlcBuilderTasks)
            {
                task.RunDlcTask(context);
            }
        }

        public class TlkInitTask : IDlcTask
        {
            public void RunDlcTask(DlcBuilderContext context)
            {
                // initialize the basic strings that are expected for a DLC
                context.TlkBuilder.InitTlk(context.ModBaseTlkId, context.InternalName, context.ModDlcFolderName, context.Game2ModuleNumber);
                // if LE1, add the tlk file to the autoload
                if (context.Game == MEGame.LE1)
                {
                    context.Le1AutoLoad?.GlobalTlks.Add($"{context.ModDlcFolderName}_GlobalTlk.GlobalTlk_tlk");
                }
            }
        }

        public class TlkOutpuTask : IDlcTask
        {
            public void RunDlcTask(DlcBuilderContext context)
            {
                switch (context.Game)
                {
                    case MEGame.LE1:
                        context.TlkBuilder.OutputGame1Tlks(context.CookedFolderPath, context.ModDlcFolderName);
                        break;
                    case MEGame.ME2:
                    case MEGame.LE2:
                        context.TlkBuilder.OutputGame23Tlks(context.CookedFolderPath, $"DLC_{context.Game2ModuleNumber}");
                        break;
                    case MEGame.ME3:
                    case MEGame.LE3:
                        context.TlkBuilder.OutputGame23Tlks(context.CookedFolderPath, context.ModDlcFolderName);
                        break;
                }
            }
        }

        public class AutoLoadMountOutputTask : IDlcTask
        {
            public void RunDlcTask(DlcBuilderContext context)
            {
                switch (context.Game)
                {
                    case MEGame.LE1:
                        context.Le1AutoLoad?.OutputAutloadIni(context.DLCBaseFolder);
                        break;
                    case MEGame.ME2:
                    case MEGame.LE2:
                    case MEGame.ME3:
                    case MEGame.LE3:
                        context.Game23Mount?.OutputMountFile(context.CookedFolderPath);
                        break;
                }
            }
        }

        public class DlcConfigInitTask : IDlcTask
        {
            public void RunDlcTask(DlcBuilderContext context)
            {
                if (context.Game.IsGame2())
                {
                    if (!context.Game2ModuleNumber.HasValue)
                    {
                        throw new ApplicationException("Game2ModuleNumber is required for mods targeting game 2");
                    }
                    context.Game23Configs!.InitGame2Configs(context.ModDlcFolderName, context.Game2ModuleNumber.Value, context.ModBaseTlkId, 0);
                }
                else if (context.Game.IsGame3())
                {
                    context.Game23Configs!.InitGame3Configs(context.ModDlcFolderName);
                }
            }
        }

        public class DlcConfigOutputTask : IDlcTask
        {
            public void RunDlcTask(DlcBuilderContext context)
            {
                if (context.Game.IsGame2())
                {
                    context.Game23Configs!.OutputGame2Inis(context.CookedFolderPath);
                }
                else if (context.Game.IsGame3())
                {
                    context.Game23Configs!.OutputGame3Bin(context.CookedFolderPath, $"Default_{context.ModDlcFolderName}.bin");
                }
            }
        }

        public class ConfigMergeOutputTask : IDlcTask
        {
            public void RunDlcTask(DlcBuilderContext context)
            {
                if (context.Game.IsLEGame())
                {
                    foreach (var configMerge in context.ConfigMergeFiles)
                    {
                        var lines = configMerge.OutputFileContents(context.Game);
                        File.WriteAllLines(Path.Combine(context.CookedFolderPath, configMerge.OutputFileName), lines);
                    }
                }
            }
        }

        public class BuildPackageTask(PackageBuilder packageBuilder) : IDlcTask
        {
            public void RunDlcTask(DlcBuilderContext context)
            {
                packageBuilder.Build();
            }
        }
    }
}
