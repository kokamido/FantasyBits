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
    }
}