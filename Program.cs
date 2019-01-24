using System;

namespace ConsoleApplication2
{
    static class Game
    {
        public static int FirstWizardId => EnemyGoalCenter.x == 16010 ? 0 : 2;
        public static int SecondWizardId => EnemyGoalCenter.x == 16010 ? 1 : 3;
        public static Side MySide => EnemyGoalCenter.x == 16010 ? Side.Left : Side.Right;
        public static (int x, int y) EnemyGoalCenter;
        public static (int x, int y) MyGoalCenter;
        private static ILogic logic = new SimpleAi();

        public static void Main(string[] args)
        {
            int myTeamId = int.Parse(Console.ReadLine());
            EnemyGoalCenter = myTeamId == 1 ? (-10, 3500) : (16010, 3500);
            MyGoalCenter = myTeamId != 1 ? (-10, 3500) : (16010, 3500);
            Console.Error.WriteLine(MySide);
            while (true)
                logic.Turn(Helper.ReadInput());
        }
    }

    public enum Side
    {
        Left,
        Right
    }
}