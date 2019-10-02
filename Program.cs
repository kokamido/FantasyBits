using System;

namespace ConsoleApplication2
{
    static class Game
    {
        private static Tuple<int, int> enemySide;
        private static ILogic logic = new SimpleAI();

        public static void Main(string[] args)
        {
            int myTeamId = int.Parse(Console.ReadLine());
            enemySide = myTeamId == 1 ? Tuple.Create(-10, 3250) : Tuple.Create(16010, 3250);
            while (true)
                logic.Turn(Helper.ReadInput());
        }
    }
}

