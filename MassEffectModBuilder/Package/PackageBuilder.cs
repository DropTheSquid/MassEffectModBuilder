using LegendaryExplorerCore.Packages;
using MassEffectModBuilder.LEXHelpers;

namespace MassEffectModBuilder.Package
{
    public abstract class PackageBuilder<T>(string packageName, string[]? subfolder = null, string? source = null)
    {

        /// <summary>
        /// The source for this pcc file to be modified.
        /// If null, it will be created at the beginning of this task. If there is already an existing file with this name in this builder, it will throw an error. 
        /// If "existing" it will use an existing file in the mod folder. If there is no existing one, it will throw an error.
        /// if "basegame" it will use the highest mounted vanilla version of this file. If no such file exists, it will throw an error.
        /// else, it will be a mod folder name, which tells the builder to pull the file from that mod. For example, it is common to use the patch as a base. If it is not found, it will throw an error. 
        /// </summary>
        public string? Source { get; } = source;

        public string PackageName { get; } = packageName;
        public string[]? Subfolder { get; } = subfolder;

        /// <summary>
        /// Setting this to true means it will not be output into the final build, just used as temporary/intermediate file.
        /// Overrides the startup bool.
        /// </summary>
        //public bool Temporary { get; set; } = false;

        protected List<IPackageBuilderTask<T>> Tasks { get; } = [];

        public PackageBuilder<T> AddTask(IPackageBuilderTask<T> task)
        {
            Tasks.Add(task);
            return this;
        }

        public PackageBuilder<T> AddTask(Action<IMEPackage, T> action)
        {
            Tasks.Add(new CustomPackageBuilderTask<T>(action));
            return this;
        }

        protected abstract string GetSaveLocationRoot(T context);

        public virtual void Build(ModBuilderContext modContext, T context)
        {
            string fileName = Path.GetFileNameWithoutExtension(PackageName) + ".pcc";

            // get the destination folder, create it if needed
            string saveDirectory = Path.Combine([GetSaveLocationRoot(context), .. Subfolder ?? []]);
            Directory.CreateDirectory(saveDirectory);

            string saveLocation = Path.Combine(saveDirectory, fileName);

            IMEPackage? package = null;
            // get the package:
            if (Source == null)
            {
                // default/null means it is a newly created file in the mod folder, but it can be created now or already exist from a previous step
                if (File.Exists(saveLocation))
                {
                    package = MEPackageHandler.OpenMEPackage(saveLocation);
                }
                else
                {
                    package = MEPackageHandler.CreateAndOpenPackage(saveLocation, modContext.Game);
                }
            }
            else if (Source == "existing")
            {
                if (!File.Exists(saveLocation))
                {
                    throw new InvalidOperationException($"You are attempting to modify an existing package that does not exist: {saveLocation}");
                }
                package = MEPackageHandler.OpenMEPackage(saveLocation);
            }
            else if (Source == "new")
            {
                if (File.Exists(saveLocation))
                {
                    throw new InvalidOperationException($"You are attempting to create a new package but it already exists: {saveLocation}");
                }
                package = MEPackageHandler.OpenMEPackage(saveLocation);
            }
            else if (Source == "basegame")
            {
                if (PackageHelpers.TryGetHighestMountedOfficialFile(fileName, modContext.Game, out var basegamePath))
                {
                    File.Copy(basegamePath, saveLocation);
                    package = MEPackageHandler.OpenMEPackage(saveLocation);
                }
                else
                {
                    throw new InvalidOperationException($"You are attempting to copy a basegame file that does not exist: {fileName}");
                }
            }
            else
            {
                var sourceLocation = Path.Combine(modContext.ModLibraryBase, modContext.Game.ToString(), Source);
                if (!File.Exists(sourceLocation))
                {
                    throw new InvalidOperationException($"You are trying to use another file from the mod library that does not exist: {sourceLocation}");
                }
                File.Copy(sourceLocation, saveLocation);
                package = MEPackageHandler.OpenMEPackage(saveLocation);
            }

            if (package == null)
            {
                throw new InvalidOperationException($"cannot operate on package {fileName}.");
            }

            foreach (var task in Tasks)
            {
                task.RunPackageTask(package, context);
            }

            package.Save();
        }
    }
}
