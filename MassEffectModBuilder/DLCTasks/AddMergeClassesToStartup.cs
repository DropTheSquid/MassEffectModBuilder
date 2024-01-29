namespace MassEffectModBuilder.DLCTasks
{
    public class AddMergeClassesToStartup(string basegameTargetFile, string className) 
        : AddMergeClassesToFile(basegameTargetFile, className, context => context.GetStartupFile())
    {
    }
}
