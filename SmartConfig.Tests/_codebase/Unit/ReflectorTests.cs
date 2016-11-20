using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reusable;
using Reusable.Validations;
using SmartConfig.DataAnnotations;

namespace SmartConfig.Core.Tests
{ // ReSharper disable InconsistentNaming
// ReSharper disable once CheckNamespace

         
    internal static class Foo
    {
        public static string Bar { get; set; }

        [Rename("Qux")]
        public static string Baz { get; set; }
    }

    

    [TestClass]
    public class GetCustomNameOrDefault_ErrorHandling
    {
        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void NullType()
        {
            ((MemberInfo)null).GetCustomNameOrDefault();
        }
    }
}