using ITA.Wizards.UpdateWizard.Model;

namespace ITA.WizardsUITest.DemoDbWizard.NET
{
    public class DemoUpdateStep : ICustomUpdateStep
    {
        public void Execute(IUpdateContext context, UpdateStepArgument[] args)
        {           
            System.Console.WriteLine("Done...");
        }
    }
}
