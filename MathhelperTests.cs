using System;
using NUnit.Framework;

namespace ConsoleApplication2
{
    [TestFixture]
    public class MathhelperTests
    {
        [Test]
        public void EuclideanRangeZero()
        {
            Assert.AreEqual(MathHelper.EuclideanRange((1,1),(1,1)),0);
        }
        
        [Test]
        public void EuclideanRangeSameX()
        {
            Assert.AreEqual(MathHelper.EuclideanRange((1,0),(1,1)),1);
        }
        
        [Test]
        public void EuclideanRangeSameY()
        {
            Assert.AreEqual(MathHelper.EuclideanRange((0,1),(1,1)),1);
        }
        
        [Test]
        public void EuclideanRangeRealValue()
        {
            Assert.AreEqual(MathHelper.EuclideanRange((0,0),(1,1)),Math.Sqrt(2),0.0000001);
        }

        [Test]
        public void AreIntersect0()
        {
            Assert.IsTrue(MathHelper.AreIntersect((0,8000),(100,8000),(16000,8000),5));
        }
        
        [Test]
        public void AreIntersect1()
        {
            Assert.IsFalse(MathHelper.AreIntersect((0,7000),(100,7000),(16000,8000),5));
        }
        
        [Test]
        public void AreIntersect2()
        {
            Assert.IsTrue(MathHelper.AreIntersect((0,5),(1,4),(2,5),2));
        }
        
        [Test]
        public void AreIntersect3()
        {
            Assert.IsFalse(MathHelper.AreIntersect((0,5),(1,4),(2,5),1));
        }
        
        [Test]
        public void AreIntersect4()
        {
            Assert.IsTrue(MathHelper.AreIntersect((2,4),(1,5),(0,6),1));
        }
        
        [Test]
        public void AreIntersect5()
        {
            Assert.IsFalse(MathHelper.AreIntersect((2,4),(1,5),(0,8),1));
        }
        
        [Test]
        public void AreIntersect6()
        {
            Assert.IsFalse(MathHelper.AreIntersect((1,4),(2,2),(4,1),2));
        }

        [Test]
        public void RangeFromLineToPoint0()
        {
            Assert.AreEqual(MathHelper.RangeFromLineToPoint((0, 0), (1, 0), (1, 1)), 1);
        }
        [Test]
        public void RangeFromLineToPoint1()
        {
            Assert.AreEqual(MathHelper.RangeFromLineToPoint((0, 0), (0, 1), (1, 1)), 1);
        }
        
        [Test]
        public void RangeFromLineToPoint2()
        {
            Assert.AreEqual(MathHelper.RangeFromLineToPoint((0, 0), (1, 1), (2, 2)), 0,0.00000001);
        }
        
        [Test]
        public void RangeFromLineToPoint3()
        {
            Assert.AreEqual(MathHelper.RangeFromLineToPoint((0, 0), (1, 1), (0, 1)), Math.Sqrt(2)/2,0.000001);
        }
    }
}