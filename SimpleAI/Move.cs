using System;
using System.Linq;

namespace ConsoleApplication2
{
    public class Move : SimpleAi.WizardAction
    {
        private int markedSnaffleId;

        public override bool Action(GameState s, int wizardId)
        {
            DoSomethingWithTheNearestSnaffle(s, GetNearestSnaffleForWizard(s, wizardId));
            return true;
        }

        public override void Reset()
        {
            markedSnaffleId = int.MinValue;
        }

       

        private void DoSomethingWithTheNearestSnaffle(GameState state, (GameObject wizard, GameObject snaffle) args)
        {
            var wizard = args.wizard;
            var snaffle = args.snaffle;
            

            Console.Error.WriteLine(
                $"{wizard.ID} in [{wizard.X}, {wizard.Y}] moves to[{snaffle.X}, {snaffle.Y}]");
            Console.WriteLine(
                $"MOVE {(snaffle.X )} {(snaffle.Y )} {GameConsts.MaxSpeed}");
        }

        private double SnaffleGoodness(GameState state, GameObject snaffle, GameObject wizard)
        {
            var minRange = GameConsts.BludgerRadius + GameConsts.SnaffleRadius;
            var rotationCoeff =
                MathHelper.GetAngle((wizard.X - snaffle.X, wizard.Y - snaffle.Y), (wizard.Vx, wizard.Vy)) /
                Math.PI * 6 * 150;
            var returnCoeff = Math.Abs(snaffle.X - Game.MyGoalCenter.x) < Math.Abs(wizard.X - Game.MyGoalCenter.x)
                ? MathHelper.EuclideanRange(snaffle, wizard)*1.5
                : 0;
            var enemyCoeff = 0; /* state.OpponentWizards.Select(w =>
                MathHelper.EuclideanRange(wizard, snaffle) - MathHelper.EuclideanRange(w, snaffle)).Min()*5;*/
            return MathHelper.EuclideanRange(snaffle, wizard) + (wizard.ID == state.MyWizards.Min(w => w.ID)
                       ? 0
                       : rotationCoeff + returnCoeff + enemyCoeff);
        }

        private (GameObject wizard, GameObject snaffle) GetNearestSnaffleForWizard(GameState state, int w)
        {
            Console.Error.WriteLine($"Finding nearest snaffle for {w}");
            var currentWizard = state.MyWizards.First(wiz => wiz.ID == w);
            var otherWizard = state.MyWizards.First(wiz => wiz.ID != w);
            Console.Error.WriteLine("wizards are ready");
            GameObject snaffle = null;
            try
            {
                snaffle = state.Snaffles.Where(s => s.ID != markedSnaffleId &&
                                                    MathHelper.EuclideanRange(s, currentWizard) <=
                                                    MathHelper.EuclideanRange(s, otherWizard))
                    .OrderBy(s => SnaffleGoodness(state, s, currentWizard))
                    .FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Snaffle finding is fucked up :( {e}");
            }

            if (snaffle == null && markedSnaffleId == int.MinValue)
            {
                snaffle = state.Snaffles
                    .OrderBy(s => SnaffleGoodness(state, s, currentWizard))
                    .FirstOrDefault();
                markedSnaffleId = snaffle.ID;
                return (currentWizard, snaffle);
            }

            if (snaffle == null)
            {
                Console.Error.WriteLine($"Snaffle not found");
                if (state.Snaffles.Count == 1)
                    return (currentWizard, state.Snaffles.First());

                return (currentWizard, state.Snaffles
                    .Where(s => s.ID != markedSnaffleId)
                    .OrderBy(s => MathHelper.EuclideanRange(s, currentWizard))
                    .First());
            }

            if (markedSnaffleId == Int32.MinValue)
                markedSnaffleId = snaffle.ID;
            else if (snaffle.ID == markedSnaffleId)
                snaffle = state.Snaffles.Where(s => s.ID != -markedSnaffleId &&
                                                    MathHelper.EuclideanRange(s, currentWizard) <=
                                                    MathHelper.EuclideanRange(s, otherWizard))
                    .OrderBy(s => MathHelper.EuclideanRange(s, currentWizard))
                    .FirstOrDefault();
            Console.Error.WriteLine($"Snaffle is {snaffle?.ID} [{snaffle?.X},{snaffle?.Y}]");
            return (currentWizard, snaffle);
        }
    }
}