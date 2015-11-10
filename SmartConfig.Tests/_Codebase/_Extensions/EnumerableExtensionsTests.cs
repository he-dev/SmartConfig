using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SmartConfig.Tests
{
    [TestClass]
    public class EnumerableExtensionsTests
    {
        [TestMethod]
        public void Check_CanFindContraint()
        {
            var constraints = new ConstraintAttribute[] { new DateTimeFormatAttribute("dd") };

            var result = false;
            constraints.Check<DateTimeFormatAttribute>(dateTimeFormat =>
            {
                result = true;
            });
            Assert.IsTrue(result);
        }        
    }
}