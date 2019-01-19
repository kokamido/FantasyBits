using System;
using System.Collections.Generic;

namespace ConsoleApplication2
{
    public class GameObject
    {
        public int ID;
        public ObjectType Type;
        public State State;
        public int X;
        public int Y;
        public int Vx;
        public int Vy;
    }

    public class GameState
    {
        public const int WizardRadius = 400;
        public const int SnuffleRadius = 150;
        public const int BludgerRadius = 200;
        public readonly int MyMagic;
        public readonly int OpponentMagic;
        public readonly int MyScore;
        public readonly int OpponentScore;
        public List<GameObject> MyWizards = new List<GameObject>();
        public List<GameObject> OpponentWizards = new List<GameObject>();
        public List<GameObject> Snaffles = new List<GameObject>();
        public List<GameObject> Bludgers = new List<GameObject>();

        public GameState(int myMagic, int opponentMagic, int myScore, int opponentScore)
        {
            MyMagic = myMagic;
            OpponentMagic = opponentMagic;
            MyScore = myScore;
            OpponentScore = opponentScore;
        }
    }

    public enum WizardId
    {
        First = 0,
        Second = 1
    }

    public enum State
    {
        NotGrab = 0,
        Grab = 1
    }

    public enum ObjectType
    {
        WIZARD,
        OPPONENT_WIZARD,
        SNAFFLE,
        BLUDGER
    }

    public static class Helper
    {
        public static GameState ReadInput()
        {
            string[] inputs;

            inputs = Console.ReadLine().Split(' ');
            var myScore = int.Parse(inputs[0]);
            var myMagic = int.Parse(inputs[1]);

            inputs = Console.ReadLine().Split(' ');
            var opponentScore = int.Parse(inputs[0]);
            var opponentMagic = int.Parse(inputs[1]);

            var gameState = new GameState(myMagic, opponentMagic, myScore, opponentScore);
            int entitiesCount = int.Parse(Console.ReadLine()); // number of entities still in game

            for (int i = 0; i < entitiesCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int entityId = int.Parse(inputs[0]); // entity identifier
                Enum.TryParse(inputs[1], out ObjectType entityType);
                int x = int.Parse(inputs[2]); // position    
                int y = int.Parse(inputs[3]); // position
                int vx = int.Parse(inputs[4]); // velocity
                int vy = int.Parse(inputs[5]); // velocity
                int objState = int.Parse(inputs[6]); // 1 if the wizard is holding a Snaffle, 0 otherwise
                var obj = new GameObject {X = x, Y = y, Vx = vx, Vy = vy, ID = entityId, State = (State) objState};
                switch (entityType)
                {
                    case ObjectType.WIZARD:
                        gameState.MyWizards.Add(obj);
                        break;
                    case ObjectType.OPPONENT_WIZARD:
                        gameState.OpponentWizards.Add(obj);
                        break;
                    case ObjectType.SNAFFLE:
                        gameState.Snaffles.Add(obj);
                        break;
                    case ObjectType.BLUDGER:
                        gameState.Bludgers.Add(obj);
                        break;
                }
            }

            return gameState;
        }
    }
}