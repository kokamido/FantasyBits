using System;
using System.Linq;

namespace ConsoleApplication2
{
    public interface ILogic
    {
        void Turn(GameState state);
    }
    
    public class SimpleAi : ILogic
    {
        public void Turn(GameState state)
        {
            var snaffle0 = GetNearestSnaffleForWizard(state, WizardId.First);
            var snaffle1 = GetNearestSnaffleForWizard(state, WizardId.Second);
            if (snaffle0.snaffle == null)
                snaffle0.snaffle = snaffle1.snaffle;
            if (snaffle1.snaffle == null)
                snaffle1.snaffle = snaffle0.snaffle;
            DoSomethingWithTheNearestSnaffle(snaffle0.wizard, snaffle0.snaffle);
            DoSomethingWithTheNearestSnaffle(snaffle1.wizard, snaffle1.snaffle);
        }

        private void DoSomethingWithTheNearestSnaffle(GameObject wizard, GameObject snaffle)
        {
            if(wizard.State == State.Grab)
                Console.WriteLine($"THROW {Game.EnemySide.x} {Game.EnemySide.y}");
            else
                Console.WriteLine($"THROW {snaffle.X} {snaffle.Y}");
        }

        private (GameObject wizard, GameObject snaffle) GetNearestSnaffleForWizard(GameState state, WizardId w)
        {
            var currentWizard = state.MyWizards.First(wiz => wiz.ID == (int) w);
            var otherWizard = state.MyWizards.First(wiz => wiz.ID != (int) w);
            var snaffle = state.Snaffles.Where(s => MathHelper.EuclideanRange(s, currentWizard) <= MathHelper.EuclideanRange(s, otherWizard))
                .OrderBy(s => MathHelper.EuclideanRange(s, currentWizard))
                .FirstOrDefault();
            return (currentWizard, snaffle);
        }
    }
    
   
}