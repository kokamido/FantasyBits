using System;
using System.Linq;

namespace ConsoleApplication2
{
    public class MoveAndThrow : SimpleAi.WizardAction
        {
            private int markedSnaffleId;

            public override bool Action(GameState s, int wizardId)
            {
                DoSomethingWithTheNearestSnaffle(GetNearestSnaffleForWizard(s, wizardId));
                return true;
            }

            public override void Reset()
            {
                markedSnaffleId = int.MinValue;
            }

            private void DoSomethingWithTheNearestSnaffle((GameObject wizard, GameObject snaffle) args)
            {
                var wizard = args.wizard;
                var snaffle = args.snaffle;
                if (wizard.State == State.Grab)
                {
                    Console.Error.WriteLine(
                        $"{wizard.ID} in [{wizard.X}, {wizard.Y}] throw snaffle {snaffle.ID} in [{snaffle.X}, {snaffle.Y}] to [{Game.EnemyGoalCenter.x}, {Game.EnemyGoalCenter.y}]");
                    Console.WriteLine(
                        $"THROW {Game.EnemyGoalCenter.x} {Game.EnemyGoalCenter.y} {GameConsts.MaxThrowStrength}");
                }
                else
                {
                    Console.Error.WriteLine(
                        $"{wizard.ID} in [{wizard.X}, {wizard.Y}] moves to[{snaffle.X}, {snaffle.Y}]");
                    Console.WriteLine($"MOVE {snaffle.X} {snaffle.Y} {GameConsts.MaxSpeed}");
                }
            }


            private double SnaffleGoodness(GameState state, GameObject snaffle, GameObject wizard)
            {
                return MathHelper.EuclideanRange(snaffle, wizard);
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
                    snaffle = state.Snaffles.Where(s =>
                            MathHelper.EuclideanRange(s, currentWizard) <= MathHelper.EuclideanRange(s, otherWizard))
                        .OrderBy(s => SnaffleGoodness(state, s, currentWizard))
                        .FirstOrDefault();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"Snaffle finding is fucked up :( {e}");
                }

                if (snaffle == null)
                {
                    Console.Error.WriteLine($"Snaffle not found");
                    if (state.Snaffles.Count == 1)
                        return (currentWizard, state.Snaffles.First());
                    
                    return (currentWizard, state.Snaffles
                        .Where(s => s.ID !=markedSnaffleId)
                        .OrderBy(s => MathHelper.EuclideanRange(s,currentWizard))
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