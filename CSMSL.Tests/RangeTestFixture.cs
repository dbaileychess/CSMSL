using NUnit.Framework;
using Should.Fluent;

namespace CSMSL.Tests
{
    [TestFixture]
    public sealed class RangeTestFixture
    {
        
        [Test]
        public void RangeSubRange()
        {
            Range a = new Range(5, 7);
            Range b = new Range(0, 10);
            a.IsSubRange(b).Should().Be.True();
        }

    }
}
