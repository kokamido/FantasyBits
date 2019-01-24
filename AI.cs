using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApplication2
{
    public interface ILogic
    {
        void Turn(GameState state);
    }

    public class SimpleAi : ILogic
    {
        private abstract class WizardAction
        {
            public WizardAction()
            {
                Reset();
            }

            public abstract bool Action(GameState s, int wizardId);
            public abstract void Reset();
        }

        private class MoveAndThrow : WizardAction
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

        private class Flipendo : WizardAction
        {          
            private const int GoalSize = 1800;
            private const int FlipendoMaxRange = 3000;
            private const int FlipendoMinRange = 750;
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
            {}

            private double CodinGameSucks(GameObject snaffle)
            {
                return MathHelper.GetNorm((snaffle.Vx, snaffle.Vy)) >= 100 ? 0.75 : 1;
            }

            private (GameObject wizard, GameObject snaffle) GetSnaffleForFlipendo(GameState state, int wizardId)
            {
                var wizard = state.MyWizards.First(w => w.ID == wizardId);
                var snaffle = state.Snaffles
                    .FirstOrDefault(s => state.MyMagic >= GameConsts.FlipendoCost
                                         && Math.Abs(s.X - Game.EnemyGoalCenter.x) <
                                         Math.Abs(wizard.X - Game.EnemyGoalCenter.x)
                                         && MathHelper.AreIntersect(wizard, s, Game.EnemyGoalCenter,
                                             (int) (GoalSize * CodinGameSucks(s)))
                                         && MathHelper.EuclideanRange(s, wizard) <= FlipendoMaxRange
                                         && MathHelper.EuclideanRange(s, wizard) >= FlipendoMinRange
                                         && state.OpponentWizards.Where(ow =>
                                             MathHelper.EuclideanRange(ow, wizard) > GameConsts.WizardRadius * 2.1).All(
                                             w =>
                                                 MathHelper.RangeFromLineToPoint(wizard, s, w) >
                                                 (GameConsts.WizardRadius + GameConsts.SnuffleRadius) * 2
                                                 || (double) (s.X - Game.EnemyGoalCenter.x) / (s.X - w.X) < 0)
                                         && state.Snaffles.Where(sn => sn.ID != s.ID).All(w =>
                                             MathHelper.RangeFromLineToPoint(wizard, s, w) >
                                             (GameConsts.SnuffleRadius + GameConsts.SnuffleRadius) * 2
                                             || (double) (s.X - Game.EnemyGoalCenter.x) / (s.X - w.X) < 0)
                                         && state.Bludgers.TrueForAll(w =>
                                             MathHelper.RangeFromLineToPoint(wizard, s, w) >
                                             (GameConsts.BludgerRadius + GameConsts.SnuffleRadius) * 2
                                             || (double) (s.X - Game.EnemyGoalCenter.x) / (s.X - w.X) < 0));
                return (wizard, snaffle);
            }
        }

        private class FreezeSnaffle : WizardAction
        {
            private const int FreezeXDistanceSmall = 700;
            private const int FreezeSpeedSmall = 155;
            private const int FreezeXDistanceBig = 1500;
            private const int FreezeSpeedBig = 200;           
            private const int GoalSize = 1800;
            private int markedForFreezeSnaffleId;
            public override bool Action(GameState s, int wizardId)
            {
                var res = GetSnaffleForFreeze(s, wizardId);
                if (res.snaffle == null)
                    return false;
                Console.Error.WriteLine(
                    $"Wizard {wizardId} FREEZE {res.snaffle.ID}, [{res.snaffle.X},{res.snaffle.Y}]");
                Console.WriteLine($"PETRIFICUS {res.snaffle.ID}");
                return true;
            }

            public override void Reset()
            {
                markedForFreezeSnaffleId = int.MinValue;
            }
            
            private bool FUCK_YOU(GameObject s, int myGoalX)
            {
                return s.ID != markedForFreezeSnaffleId
                       && (Math.Abs(s.X - myGoalX) <= FreezeXDistanceSmall
                           && MathHelper.GetNorm((s.Vx, s.Vy)) >= FreezeSpeedSmall
                           || Math.Abs(s.X - myGoalX) <= FreezeXDistanceBig
                           && MathHelper.GetNorm((s.Vx, s.Vy)) >= FreezeSpeedBig
                           || MathHelper.GetNorm((s.Vx, s.Vy)) > 750
                           && (Game.MySide == Side.Left ? s.X <= 8000 : s.X > 8000))
                       && (Game.MySide == Side.Left ? s.Vx < 0 : s.Vx > 0)
                       && s.Y <= Game.EnemyGoalCenter.y + GoalSize
                       && s.Y >= Game.EnemyGoalCenter.y - GoalSize;
            }
            private (GameObject wizard, GameObject snaffle) GetSnaffleForFreeze(GameState state, int wizardId)
            {
                var wizard = state.MyWizards.First(w => w.ID == wizardId);
                int myGoalX = Math.Abs(Game.EnemyGoalCenter.x - 16000);
                var res = state.Snaffles.FirstOrDefault(s => FUCK_YOU(s, myGoalX));
                if (res == null || state.MyMagic < GameConsts.PetrificusCost * 2)
                    return (wizard, null);
                if (markedForFreezeSnaffleId == int.MinValue)
                    markedForFreezeSnaffleId = res.ID;
                return (wizard, res);
            }
        }

        private class FreezeBludger : WizardAction
        {        
            private const double BludgerAngle = Math.PI / 6;
            public override bool Action(GameState s, int wizardId)
            {
                var res = FuckUpBludger(s, wizardId);
                if (res.bludger == null)
                    return false;
                Console.Error.WriteLine(
                    $"Wizard {wizardId} OBLIVIATE {res.bludger.ID}, [{res.bludger.X},{res.bludger.Y}]");
                Console.WriteLine($"OBLIVIATE {res.bludger.ID}");
                return true;
            }

            public override void Reset(){}
           
            private double RUTroll(int vX, int vY)
            {
                return MathHelper.EuclideanRange((vX, vY), (0, 0)) >= 150 ? 2.5 : 1.5;
            }

            private bool RUTrollFuckingFaggot(GameState state, GameObject b, GameObject wizard)
            {
                return state.MyMagic >= GameConsts.ObliviateCost * 3
                       && MathHelper.EuclideanRange(b, wizard) <
                       (GameConsts.WizardRadius + GameConsts.BludgerRadius) * RUTroll(b.Vx, b.Vy)
                       && MathHelper.GetAngle((b.X - wizard.X, b.Y - wizard.Y), (-b.Vx, -b.Vy)) < BludgerAngle;
            }

            private (GameObject wizard, GameObject bludger) FuckUpBludger(GameState state, int wizardId)
            {
                var wizard = state.MyWizards.First(w => w.ID == wizardId);
                var bludger = state.Bludgers.FirstOrDefault(b => RUTrollFuckingFaggot(state, b, wizard));
                return (wizard, bludger);
            }          
        }

        private class Accio : WizardAction
        {
            private const int AccioMaxRange = 9000;
            private const int AccioMinRange = 3000;
            private int markedForAccioSnaffleId;
            
            public override bool Action(GameState s, int wizardId)
            {
                var res = GetSnaffleForAccio(s, wizardId);
                if (res.snaffle == null)
                    return false;
                Console.Error.WriteLine(
                    $"Wizard {wizardId} ACCIO {res.snaffle.ID}, [{res.snaffle.X},{res.snaffle.Y}]");
                Console.WriteLine($"ACCIO {res.snaffle.ID}");
                return true;
            }

            public override void Reset()
            {
                markedForAccioSnaffleId = int.MinValue;
            }
            
            private (GameObject wizard, GameObject snaffle) GetSnaffleForAccio(GameState state, int wizardId)
            {
                var wizard = state.MyWizards.First(w => w.ID == wizardId);
                var otherWizard = state.MyWizards.First(w => w.ID != wizardId);
                if (state.MyMagic < GameConsts.AccioCost
                    || (state.Snaffles
                            .Where(s => s.ID != markedForAccioSnaffleId)
                            .Select(s => Math.Min(MathHelper.EuclideanRange(s, wizard),
                                MathHelper.EuclideanRange(s, otherWizard)))
                            .Any(l => l < AccioMinRange)
                        && state.Snaffles
                            .Where(s => s.ID != markedForAccioSnaffleId)
                            .Any(s => Math.Min(MathHelper.EuclideanRange(s, wizard),
                                          MathHelper.EuclideanRange(s, otherWizard))
                                      < state.OpponentWizards.Select(ow => MathHelper.EuclideanRange(ow, s)).Min()))
                    || state.Snaffles.Where(s => s.ID != markedForAccioSnaffleId)
                        .Select(s => MathHelper.EuclideanRange(s, wizard)).Any(l => l > AccioMaxRange))
                    return (wizard, null);
                var res = state.Snaffles.Where(s => s.ID != markedForAccioSnaffleId)
                    .OrderBy(s => MathHelper.EuclideanRange(s, wizard)).FirstOrDefault();
                if (res == null)
                    return (wizard, null);
                if (markedForAccioSnaffleId == int.MinValue)
                    markedForAccioSnaffleId = res.ID;
                return (wizard, res);
            }
        }      
        
        private List<WizardAction> strategy = new List<WizardAction>
        {
            new Accio(),
            new Flipendo(),
            new FreezeSnaffle(),
            new FreezeBludger(),
            new MoveAndThrow()
        };

        public void Turn(GameState state)
        {
            Console.Error.WriteLine(Directory.GetCurrentDirectory());
            strategy.FirstOrDefault(a => a.Action(state, Game.FirstWizardId));
            strategy.FirstOrDefault(a => a.Action(state, Game.SecondWizardId));
            strategy.ForEach(a => a.Reset());
        }       
    }
}