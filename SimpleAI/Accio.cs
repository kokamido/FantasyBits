using System;
using System.Linq;

namespace ConsoleApplication2
{
    public class Accio : SimpleAi.WizardAction
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
}