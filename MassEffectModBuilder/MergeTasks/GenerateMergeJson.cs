namespace MassEffectModBuilder.MergeTasks
{
    public class GenerateMergeJson : ModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            context.MergeMods.CreateJsonFiles(context);
        }
    }
}
