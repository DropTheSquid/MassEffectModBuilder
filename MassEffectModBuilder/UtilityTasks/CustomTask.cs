namespace MassEffectModBuilder.UtilityTasks
{
    public record class CustomTask(Action<ModBuilderContext> CustomTaskAction) : ModBuilderTask
    {
        public void RunModTask(ModBuilderContext context)
        {
            CustomTaskAction(context);
        }
    }
}
