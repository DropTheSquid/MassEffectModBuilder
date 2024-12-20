﻿using LegendaryExplorerCore.Misc.ME3Tweaks;
using LegendaryExplorerCore.Packages;
using MassEffectModBuilder.LEXHelpers;
using static MassEffectModBuilder.LEXHelpers.LooseClassCompile;

namespace MassEffectModBuilder.DLCTasks
{
    /// <summary>
    /// Compiles all given classes and adds them to the file. Adds them to an ObjectReferencer (unless it is a level file).
    /// </summary>
    /// <param name="getFileFunc">returns the file to add the classes to.</param>
    /// <param name="Classes">the list of classes to be added to the file</param>
    public class AddClassesToFile(Func<ModBuilderContext, IMEPackage> getFileFunc, IEnumerable<ClassToCompile> Classes) : IModBuilderTask
    {
        public AddClassesToFile(Func<ModBuilderContext, IMEPackage> getFileFunc, params ClassToCompile[] Classes)
            : this(getFileFunc, (IEnumerable<ClassToCompile>)Classes) { }
        public void RunModTask(ModBuilderContext context)
        {
            var file = getFileFunc(context);

            // compile against the backup; this ensures you are not relying on non vanilla classes that could lead to the game crashing if they are not present
            var backupPath = ME3TweaksBackups.GetGameBackupPath(context.Game);

            CompileClasses(Classes, file, backupPath);

            var objectReferencer = file.GetObjectReferencer();

            if (objectReferencer != null)
            {
                var addedClassExports = Classes
                    .Select(x => file.FindExport(x.InstancedFullPath));

                objectReferencer.AddToObjectReferencer(addedClassExports);
            }

            file.Save();
        }
    }
}
