using System;
using NUnit.Framework;

namespace Thuria.Calot.TestUtilities.Tests
{
  [TestFixture]
  public class TestPropertyTestHelper
  {
    [TestCase("TestDateTime", typeof(FakeTestAttribute))]
    public void ValidateDecoratedWithAttribute_GivenPropertyNotValidated_ShouldFailTest(string propertyName, Type attributeType)
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      PropertyTestHelper.ValidateDecoratedWithAttribute<FakeTestClass>(propertyName, attributeType);
      //---------------Test Result -----------------------
    }
  }
}
