namespace MassEffectModBuilder.MergeTasks
{
    public class GenerateMergeJson : IModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            context.MergeMods.CreateJsonFiles(context);
        }
    }
}
