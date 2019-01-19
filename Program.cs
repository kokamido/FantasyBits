using System;

namespace ConsoleApplication2
{
    static class Game
    {
        public static (int x, int y) EnemySide;
        private static ILogic logic = new SimpleAi();

        public static void Main(string[] args)
        {
            int myTeamId = int.Parse(Console.ReadLine());
            EnemySide = myTeamId == 1 ? (-10, 3250) : (16010, 3250);
            while (true)
                logic.Turn(Helper.ReadInput());
        }
    }
}