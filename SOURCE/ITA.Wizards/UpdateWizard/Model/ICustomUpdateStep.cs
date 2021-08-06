namespace ITA.Wizards.UpdateWizard.Model
{
    /// <summary>
    /// Интерфейс шага обновления БД.
    /// </summary>
    public interface ICustomUpdateStep
    {
        void Execute(IUpdateContext context, UpdateStepArgument[] args);
    }
}
