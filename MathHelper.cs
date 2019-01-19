using System;

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
    }
}