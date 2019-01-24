using System;
using System.Linq;

namespace ConsoleApplication2
{
    public class FreezeSnaffle : SimpleAi.WizardAction
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
}