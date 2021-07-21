using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Capabilities;
using Model.Extensions;

namespace Model.Tests.Extensions
{
    [TestClass]
    public class StringExtensionsTest
    {
        [TestMethod]
        public void ToEnum_WhenStringValue_ConvertsStringToTheEnumType()
        {
            var enumString = "1001";
            var enumValue = enumString.ToEnum<ExceptionCode>();
            Assert.AreEqual(enumValue, ExceptionCode.InvalidProductException);
        }
    }
}
