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
        private const int FreezeXDistanceSmall = 700;   
        private const int FreezeSpeedSmall = 155;
        private const int FreezeXDistanceBig = 1500;   
        private const int FreezeSpeedBig = 200;
        private const double BludgerAngle = Math.PI / 6;
        private const int FlipendoMaxRange = 3000;
        private const int FlipendoMinRange = 750;
        private const int AccioMaxRange = 9000;
        private const int AccioMinRange = 3000;
        private const int GoalSize = 1800;
        
        private int markedForThrowSnaffleId;
        private int markedForFreezeSnaffleId;
        private int markedForAccioSnaffleId;
        public void Turn(GameState state)
        {
            markedForThrowSnaffleId = int.MinValue;
            markedForFreezeSnaffleId = int.MinValue;
            markedForAccioSnaffleId = int.MinValue;
            DoWizardTurn(state, Game.FirstWizardId);
            DoWizardTurn(state, Game.SecondWizardId);
        }

        private void DoWizardTurn(GameState state, int wizardId)
        {
            var flipendo = GetSnaffleForFlipendo(state, wizardId);
            if (flipendo.snaffle != null)
            {
                Console.Error.WriteLine(
                    $"Wizard {wizardId} FLIPENDO {flipendo.snaffle.ID}, [{flipendo.snaffle.X},{flipendo.snaffle.Y}]");
                Console.WriteLine($"FLIPENDO {flipendo.snaffle.ID}");
                return;
            }        
            var snaffleForAccio = GetSnaffleForAccio(state, wizardId);
            if (snaffleForAccio.snaffle != null)
            {
                Console.Error.WriteLine($"Wizard {wizardId} ACCIO {snaffleForAccio.snaffle.ID}, [{snaffleForAccio.snaffle.X},{snaffleForAccio.snaffle.Y}]");
                Console.WriteLine($"ACCIO {snaffleForAccio.snaffle.ID}");
                return;
            } 
            var snaffForFreeze = GetSnaffleForFreeze(state, wizardId);
            if (snaffForFreeze.snaffle != null)
            {
                Console.Error.WriteLine($"Wizard {wizardId} FREEZE {snaffForFreeze.snaffle.ID}, [{snaffForFreeze.snaffle.X},{snaffForFreeze.snaffle.Y}]");
                Console.WriteLine($"PETRIFICUS {snaffForFreeze.snaffle.ID}");
                return;
            }
            var bludger = FuckUpBludger(state, wizardId);
            if (bludger.bludger != null)
            {
                Console.Error.WriteLine(
                    $"Wizard {wizardId} OBLIVIATE {bludger.bludger.ID}, [{bludger.bludger.X},{bludger.bludger.Y}]");
                Console.WriteLine($"OBLIVIATE {bludger.bludger.ID}");
                return;
            }
      
            var snaffle = GetNearestSnaffleForWizard(state, wizardId);
            if(snaffle.snaffle == null)
                snaffle.snaffle = state.Snaffles.First();
            DoSomethingWithTheNearestSnaffle(snaffle.wizard, snaffle.snaffle);

        }

        private bool FUCK_YOU(GameObject s, int myGoalX)
        {
            return s.ID != markedForFreezeSnaffleId 
                   && (Math.Abs(s.X - myGoalX) <= FreezeXDistanceSmall 
                   && MathHelper.GetNorm((s.Vx, s.Vy)) >= FreezeSpeedSmall
                   || Math.Abs(s.X - myGoalX) <= FreezeXDistanceBig 
                   && MathHelper.GetNorm((s.Vx, s.Vy)) >= FreezeSpeedBig
                   || MathHelper.GetNorm((s.Vx,s.Vy))>750
                   && (Game.MySide == Side.Left ? s.X <= 8000 : s.X > 8000))
                   && (Game.MySide == Side.Left ? s.Vx < 0 : s.Vx > 0)
                   && s.Y <= Game.EnemyGoalCenter.y+GoalSize
                   && s.Y >= Game.EnemyGoalCenter.y-GoalSize;
        }

        private (GameObject wizard, GameObject snaffle) GetSnaffleForAccio(GameState state, int wizardId)
        {
            var wizard = state.MyWizards.First(w => w.ID == wizardId);
            var otherWizard = state.MyWizards.First(w => w.ID != wizardId);
            if (state.MyMagic < GameConsts.AccioCost
                || (state.Snaffles
                        .Where(s => s.ID != markedForAccioSnaffleId)
                        .Select(s => Math.Min(MathHelper.EuclideanRange(s, wizard), MathHelper.EuclideanRange(s, otherWizard)))
                        .Any(l => l < AccioMinRange)
                    && state.Snaffles
                        .Where(s => s.ID != markedForAccioSnaffleId)
                        .Any(s => Math.Min(MathHelper.EuclideanRange(s, wizard), MathHelper.EuclideanRange(s, otherWizard))
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

        private (GameObject wizard, GameObject snaffle) GetSnaffleForFreeze(GameState state, int wizardId)
        {
            var wizard = state.MyWizards.First(w => w.ID == wizardId);
            int myGoalX = Math.Abs(Game.EnemyGoalCenter.x - 16000);
            var res = state.Snaffles.FirstOrDefault(s => FUCK_YOU(s,myGoalX));
            if (res == null || state.MyMagic < GameConsts.PetrificusCost*2)
                return (wizard, null);
            if (markedForFreezeSnaffleId == int.MinValue)
                markedForFreezeSnaffleId = res.ID;
            return (wizard, res);
        }

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
            var bludger = state.Bludgers.FirstOrDefault(b => RUTrollFuckingFaggot(state,b,wizard));
            return (wizard, bludger);
        }

        public double CodinGameSucks(GameObject snaffle)
        {
            return MathHelper.GetNorm((snaffle.Vx, snaffle.Vy)) >= 100 ? 0.75 : 1;
        }

        private (GameObject wizard, GameObject snaffle) GetSnaffleForFlipendo(GameState state,int wizardId)
        {
            var wizard = state.MyWizards.First(w => w.ID == wizardId);
            var snaffle =  state.Snaffles
                .FirstOrDefault(s => state.MyMagic >= GameConsts.FlipendoCost
                                     && Math.Abs(s.X-Game.EnemyGoalCenter.x) < Math.Abs(wizard.X-Game.EnemyGoalCenter.x)
                                     && MathHelper.AreIntersect(wizard, s, Game.EnemyGoalCenter, (int)(GoalSize*CodinGameSucks(s)))
                                     && MathHelper.EuclideanRange(s, wizard) <= FlipendoMaxRange
                                     && MathHelper.EuclideanRange(s, wizard) >= FlipendoMinRange
                                     && state.OpponentWizards.Where(ow => MathHelper.EuclideanRange(ow,wizard) > GameConsts.WizardRadius*2.1).All(w =>
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
            if (markedForThrowSnaffleId == Int32.MinValue)
                markedForThrowSnaffleId = snaffle.ID;
            else if(snaffle.ID == markedForThrowSnaffleId)
                snaffle = state.Snaffles.Where(s => s.ID !=- markedForThrowSnaffleId &&
                     MathHelper.EuclideanRange(s, currentWizard) <= MathHelper.EuclideanRange(s, otherWizard))
                    .OrderBy(s => MathHelper.EuclideanRange(s, currentWizard))
                    .FirstOrDefault();
            Console.Error.WriteLine($"Snaffle is {snaffle?.ID} [{snaffle?.X},{snaffle?.Y}]");
            return (currentWizard, snaffle);
        }
    }
}