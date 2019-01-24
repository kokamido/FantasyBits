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
        public abstract class WizardAction
        {
            protected WizardAction()
            {
                Reset();
            }

            public abstract bool Action(GameState s, int wizardId);
            public abstract void Reset();
        }
 
        private readonly List<WizardAction> strategy = new List<WizardAction>
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