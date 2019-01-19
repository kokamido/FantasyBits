using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ConsoleApplication2
{
    public interface ILogic
    {
        void Turn(GameState state);
    }

    public class SimpleAi : ILogic
    {
        private const int FlipendoMaxRange = 2500;
        private const int FlipendoMinRange = 1000;
        private const int GoalSize = 1600;
        
        private int markedSnaffleId;
        public void Turn(GameState state)
        {
            markedSnaffleId = int.MinValue;
            DoWizardTurn(state, Game.FirstWizardId);
            DoWizardTurn(state, Game.SecondWizardId);
        }

        private void DoWizardTurn(GameState state, int wizardId)
        {
            var bludger = FuckUpBludger(state, wizardId);
            if (bludger.bludger != null)
            {
                Console.Error.WriteLine(
                    $"Wizard {wizardId} OBLIVIATE {bludger.bludger.ID}, [{bludger.bludger.X},{bludger.bludger.Y}]");
                Console.WriteLine($"OBLIVIATE {bludger.bludger.ID}");
                return;
            }
            
            var flipendo = GetSnaffleForFlipendo(state, wizardId);
            if (flipendo.snaffle != null)
            {
                Console.Error.WriteLine(
                    $"Wizard {wizardId} FLIPENDO {flipendo.snaffle.ID}, [{flipendo.snaffle.X},{flipendo.snaffle.Y}]");
                Console.WriteLine($"FLIPENDO {flipendo.snaffle.ID}");
                return;
            }
            
            var snaffle = GetNearestSnaffleForWizard(state, wizardId);
            if(snaffle.snaffle == null)
                snaffle.snaffle = state.Snaffles.First();
            DoSomethingWithTheNearestSnaffle(snaffle.wizard, snaffle.snaffle);

        }

        private double RUTroll(int vX, int vY)
        {
            return MathHelper.EuclideanRange((vX, vY), (0, 0)) >= 150 ? 2.5 : 1.5;
        }
        private (GameObject wizard, GameObject bludger) FuckUpBludger(GameState state, int wizardId)
        {
            var wizard = state.MyWizards.First(w => w.ID == wizardId);
            var bludger = state.Bludgers.FirstOrDefault(b => state.MyMagic >= GameConsts.ObliviateCost +GameConsts.FlipendoCost&&
                MathHelper.EuclideanRange(b, wizard) < (GameConsts.WizardRadius + GameConsts.BludgerRadius)*RUTroll(b.Vx,b.Vy) );
            return (wizard, bludger);
        }

        private (GameObject wizard, GameObject snaffle) GetSnaffleForFlipendo(GameState state,int wizardId)
        {
            var wizard = state.MyWizards.First(w => w.ID == wizardId);
            var snaffle =  state.Snaffles
                .FirstOrDefault(s => state.MyMagic >= GameConsts.FlipendoCost
                                     && MathHelper.AreIntersect(wizard, s, Game.EnemyGoalCenter, GoalSize)
                                     && MathHelper.EuclideanRange(s, wizard) <= FlipendoMaxRange
                                     && MathHelper.EuclideanRange(s, wizard) >= FlipendoMinRange
                                     && state.OpponentWizards.TrueForAll(w =>
                                         MathHelper.RangeFromLineToPoint(wizard, s, w) > (GameConsts.WizardRadius + GameConsts.SnuffleRadius) * 2
                                         || (double)(s.X-Game.EnemyGoalCenter.x)/(s.X-w.X) < 0)
                                     && state.Snaffles.Where(sn => sn.ID != s.ID).All(w => 
                                         MathHelper.RangeFromLineToPoint(wizard, s, w) > (GameConsts.SnuffleRadius + GameConsts.SnuffleRadius) * 2
                                         || (double)(s.X-Game.EnemyGoalCenter.x)/(s.X-w.X) < 0)
                                     && state.Bludgers.TrueForAll(w =>
                                         MathHelper.RangeFromLineToPoint(wizard, s, w) > (GameConsts.BludgerRadius + GameConsts.SnuffleRadius) * 2
                                         || (double)(s.X-Game.EnemyGoalCenter.x)/(s.X-w.X) < 0));
            return (wizard, snaffle);

        }

        /*private bool FuckUpEnemyWithAccio(GameObject wizard, GameObject enemy, GameState state)
        {
            if()
        }*/

        private (GameObject wizard, GameObject enemy) GetNearestEnemyWithSnaffle(GameObject wizard, GameState state)
        {
            var enemy = state.OpponentWizards
                .Where(w => w.State == State.Grab)
                .OrderBy(w => MathHelper.EuclideanRange(w, wizard))
                .FirstOrDefault();
            return (wizard, enemy);
        }

        private void DoSomethingWithTheNearestSnaffle(GameObject wizard, GameObject snaffle)
        {
            if (wizard.State == State.Grab)
            {            
                Console.Error.WriteLine($"{wizard.ID} in [{wizard.X}, {wizard.Y}] throw snaffle {snaffle.ID} in [{snaffle.X}, {snaffle.Y}] to [{Game.EnemyGoalCenter.x}, {Game.EnemyGoalCenter.y}]");
                Console.WriteLine($"THROW {Game.EnemyGoalCenter.x} {Game.EnemyGoalCenter.y} {GameConsts.MaxThrowStrength}");
            }
            else
            {
                Console.Error.WriteLine($"{wizard.ID} in [{wizard.X}, {wizard.Y}] moves to[{snaffle.X}, {snaffle.Y}]");
                Console.WriteLine($"MOVE {snaffle.X} {snaffle.Y} {GameConsts.MaxSpeed}");
            }
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
                    .OrderBy(s => MathHelper.EuclideanRange(s, currentWizard))
                    .FirstOrDefault();
            }
            catch(Exception e)
            {
                Console.Error.WriteLine($"Snaffle finding is fucked up :( {e}");
            }

            if (snaffle == null)
            {
                Console.Error.WriteLine($"Snaffle not found");
                return (currentWizard, null);
            }
            if (markedSnaffleId == Int32.MinValue)
                markedSnaffleId = snaffle.ID;
            else if(snaffle.ID == markedSnaffleId)
                snaffle = state.Snaffles.Where(s => s.ID !=- markedSnaffleId &&
                     MathHelper.EuclideanRange(s, currentWizard) <= MathHelper.EuclideanRange(s, otherWizard))
                    .OrderBy(s => MathHelper.EuclideanRange(s, currentWizard))
                    .FirstOrDefault();
            Console.Error.WriteLine($"Snaffle is {snaffle?.ID} [{snaffle?.X},{snaffle?.Y}]");
            return (currentWizard, snaffle);
        }
    }
}