using System;
using System.Linq;
using System.Net;

namespace ConsoleApplication2
{
    public class Flipendo : SimpleAi.WizardAction
    {
        private const int GoalSize = 1750;
        private const int FlipendoMaxRange = 3500;
        private const int FlipendoMinRange = 700;

        public override bool Action(GameState s, int wizardId)
        {
            var res = GetSnaffleForFlipendo(s, wizardId);
            if (res.snaffle == null)
                return false;
            Console.Error.WriteLine(
                $"Wizard {wizardId} FLIPENDO {res.snaffle.ID}, [{res.snaffle.X},{res.snaffle.Y}]");
            Console.WriteLine($"FLIPENDO {res.snaffle.ID}");
            return true;
        }

        public override void Reset()
        {
        }

        private double CodinGameSucks(GameObject snaffle)
        {
            return MathHelper.GetNorm((snaffle.Vx, snaffle.Vy)) >= 100 ? 0.75 : 1;
        }

        private (GameObject wizard, GameObject snaffle) GetSnaffleForFlipendo(GameState state, int wizardId)
        {
            var wizard = state.MyWizards.First(w => w.ID == wizardId);
            var snaffle = state.Snaffles.Where(s =>
                    state.OpponentWizards.Any(ow => MathHelper.EuclideanRange(ow, s) < 20 * GameConsts.WizardRadius) ||
                    Math.Abs(s.X - Game.EnemyGoalCenter.x) >= 3750)
                .FirstOrDefault(s => state.MyMagic >= GameConsts.FlipendoCost
                                     && s.State == State.NotGrab
                                     && (s.Vy <= 25 || Math.Abs(s.X-Game.EnemyGoalCenter.x)<500 && Math.Abs(s.Y-Game.EnemyGoalCenter.y)<GoalSize)
                                     && Math.Abs(s.X - Game.EnemyGoalCenter.x) <
                                     Math.Abs(wizard.X - Game.EnemyGoalCenter.x)
                                     && MathHelper.AreIntersect(wizard, s, Game.EnemyGoalCenter,
                                         (int) (GoalSize * CodinGameSucks(s)))
                                     && MathHelper.EuclideanRange(s, wizard) <= FlipendoMaxRange
                                     && (MathHelper.EuclideanRange(s, wizard) >= FlipendoMinRange ||MathHelper.EuclideanRange(s, wizard) < (GameConsts.WizardRadius+GameConsts.SnaffleRadius)*1.5)
                                     && state.OpponentWizards.Where(ow =>
                                             MathHelper.EuclideanRange(ow, s) <= 6000
                                             && MathHelper.EuclideanRange(ow, wizard) > GameConsts.WizardRadius * 3)
                                         .All(w => MathHelper.RangeFromLineToPoint(wizard, s, w) >
                                             (GameConsts.WizardRadius + GameConsts.SnaffleRadius) * 2
                                             || (double) (s.X - Game.EnemyGoalCenter.x) / (s.X - w.X) < 0)
                                     && state.Snaffles.Where(sn => sn.ID != s.ID)
                                         .Where(ow => MathHelper.EuclideanRange(ow, s) <= 6000)
                                         .All(w => MathHelper.RangeFromLineToPoint(wizard, s, w) >
                                             (GameConsts.SnaffleRadius + GameConsts.SnaffleRadius) * 2
                                             || (double) (s.X - Game.EnemyGoalCenter.x) / (s.X - w.X) < 0)
                                     && state.Bludgers
                                         .Where(ow => MathHelper.EuclideanRange(ow, s) <= 6000)
                                         .All(w => MathHelper.RangeFromLineToPoint(wizard, s, w) >
                                                   (GameConsts.BludgerRadius + GameConsts.SnaffleRadius) * 2
                                                   || (double) (s.X - Game.EnemyGoalCenter.x) / (s.X - w.X) < 0));
            return (wizard, snaffle);
        }
    }
}