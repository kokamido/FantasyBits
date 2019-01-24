using System;
using System.Linq;

namespace ConsoleApplication2
{
    public class FreezeBludger : SimpleAi.WizardAction
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
}