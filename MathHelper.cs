using System;
using System.Collections.Generic;

namespace ConsoleApplication2
{
    public static class MathHelper
    {
        public static double EuclideanRange(GameObject o0, GameObject o1)
        {
            return EuclideanRange((o0.X,o0.Y),(o1.X,o1.Y));
        }
        public static double EuclideanRange((int x, int y) p0, (int x, int y) p1)
        {
            return Math.Sqrt((p0.x - p1.x) * (p0.x - p1.x) + (p0.y - p1.y) * (p0.y - p1.y));
        }

        public static bool AreIntersect(GameObject wizard, GameObject snaffle, (int x, int y) enemyGoalCenter, int goalSize)
        {
            return AreIntersect((wizard.X, wizard.Y), (snaffle.X, snaffle.Y), enemyGoalCenter, goalSize);
        }

        public static bool AreIntersect((int x, int y) wizard, (int x, int y) snaffle, (int x, int y) enemyGoalCenter, int goalSize)
        {
            var attackVector = GetNormedVec((snaffle.x - wizard.x, snaffle.y - wizard.y));
            var topBoardVector = GetNormedVec((enemyGoalCenter.x - wizard.x, enemyGoalCenter.y + goalSize - wizard.y));
            var bottomBoardVector = GetNormedVec((enemyGoalCenter.x - wizard.x, enemyGoalCenter.y - goalSize - wizard.y));
            return (attackVector.y - topBoardVector.y) / (attackVector.y - bottomBoardVector.y) < 0;
        }

        public static double ScalarProduct((int x, int y) v0, (int x, int y) v1)
        {
            return v0.x * v1.x + v0.y * v1.y;
        }

        public static double GetAngle((int x, int y) v0, (int x, int y) v1)
        {
            return Math.Acos(ScalarProduct(v0, v1) / GetNorm(v0) / GetNorm(v1));
        }

        public static double GetNorm((int x, int y) v)
        {
            return EuclideanRange((0, 0), (v.x, v.y));
        }
        
        public static (double x, double y) GetNormedVec((int x, int y) v0)
        {
            var v0Norm = GetNorm(v0);
            return (v0.x / v0Norm, v0.y / v0Norm);
        }

        public static double RangeFromLineToPoint(GameObject o1, GameObject o2, GameObject o)
        {
            return RangeFromLineToPoint((o1.X, o1.Y), (o2.X, o2.Y), (o.X, o.Y));
        }
        public static double RangeFromLineToPoint((int x, int y) p1, (int x, int y) p2, (int x, int y) point)
        {
            return Math.Abs((p2.y - p1.y) * point.x - (p2.x - p1.x) * point.y + p2.x * p1.y - p2.y * p1.x) / EuclideanRange(p2, p1);
        }
    }

    public static class SharpHepler
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
                return;
            foreach (var e in source)
            {
                action(e);
            }
        }
    }
}