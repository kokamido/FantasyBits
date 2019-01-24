using System;
using System.Linq;

namespace ConsoleApplication2
{
    public class Throw : SimpleAi.WizardAction
    {
       
        public override bool Action(GameState s, int wizardId)
        {
            var wizard = s.MyWizards.First(w => w.ID == wizardId);
            if (wizard.State == State.NotGrab)
                return false;
            Console.Error.WriteLine(
                $"{wizard.ID} in [{wizard.X}, {wizard.Y}] throw snaffle to [{Game.EnemyGoalCenter.x}, {Game.EnemyGoalCenter.y}]");
            Console.WriteLine(
                $"THROW {Game.EnemyGoalCenter.x} {Game.EnemyGoalCenter.y} {GameConsts.MaxThrowStrength}");
            return true;
        }

        public override void Reset()
        {
            
        }

       
    }
}