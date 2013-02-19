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
            MassRange a = new MassRange(5, 7);
            MassRange b = new MassRange(0, 10);
            a.IsSubRange(b).Should().Be.True();
        }

    }
}
