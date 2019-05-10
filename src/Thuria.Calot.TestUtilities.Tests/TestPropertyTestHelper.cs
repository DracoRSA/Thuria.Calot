using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Thuria.Calot.TestUtilities.Tests
{
  [TestFixture]
  public class TestPropertyTestHelper
  {
    [TestCase("TestDateTime", typeof(FakeTestAttribute))]
    [TestCase("ComplexObject1", typeof(FakeTestAttribute))]
    [TestCase("FakeList", typeof(FakeTestAttribute))]
    public void ValidateDecoratedWithAttribute_GivenPropertyNotDecorated_ShouldFailTest(string propertyName, Type attributeType)
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var exception = Assert.Throws<AssertionException>(() => PropertyTestHelper.ValidateDecoratedWithAttribute<FakeTestClass>(propertyName, attributeType));
      //---------------Test Result -----------------------
      exception.Message.Contains($"Property {propertyName} is not decorated with {attributeType.Name} Attribute");
    }

    [TestCase("ComplexObject2", typeof(FakeTestAttribute))]
    [TestCase("TestDictionary", typeof(FakeTestAttribute))]
    public void ValidateDecoratedWithAttribute_GivenPropertyDecorated_ShouldPassTest(string propertyName, Type attributeType)
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      Assert.DoesNotThrow(() => PropertyTestHelper.ValidateDecoratedWithAttribute<FakeTestClass>(propertyName, attributeType));
      //---------------Test Result -----------------------
    }

    [Test]
    public void ValidateDecoratedWithAttribute_GivenPropertyDecoratedWithAttributeWithExpectedParameters_ShouldPassTest()
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      Assert.DoesNotThrow(() => PropertyTestHelper.ValidateDecoratedWithAttribute<FakeTestClass>("TestDictionary",
                                                                                                 typeof(FakeTestAttribute),
                                                                                                 new List<(string propertyName, object propertyValue)>
                                                                                                   {
                                                                                                     ("PropertyName", "TestDictionary"),
                                                                                                     ("Sequence", 23)
                                                                                                   }));
      //---------------Test Result -----------------------
    }

    [Test]
    public void ValidateDecoratedWithAttribute_GivenPropertyDecoratedWithAttributeButPropertyDoesNotExist_ShouldFailTest()
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var exception = Assert.Throws<AssertionException>(() => PropertyTestHelper.ValidateDecoratedWithAttribute<FakeTestClass>("TestDictionary",
                                                                                                                               typeof(FakeTestAttribute),
                                                                                                                               new List<(string propertyName, object propertyValue)>
                                                                                                                                  {
                                                                                                                                    ("PropertyName", "TestDictionary"),
                                                                                                                                    ("Sequence", 23),
                                                                                                                                    ("UnknownProperty", "SomeValue")
                                                                                                                                  }));
      //---------------Test Result -----------------------
      exception.Message.Should().Contain("TestDictionary Property is decorated with FakeTestAttribute " +
                                         "but the attribute property UnknownProperty does not exist on the attribute");
    }

    [Test]
    public void ValidateDecoratedWithAttribute_GivenPropertyDecoratedWithAttributeButPropertyValueIsNotExpected_ShouldFailTest()
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var exception = Assert.Throws<AssertionException>(() => PropertyTestHelper.ValidateDecoratedWithAttribute<FakeTestClass>("TestDictionary",
                                                                                                                               typeof(FakeTestAttribute),
                                                                                                                               new List<(string propertyName, object propertyValue)>
                                                                                                                                  {
                                                                                                                                    ("PropertyName", "TestDictionary"),
                                                                                                                                    ("Sequence", 55)
                                                                                                                                  }));
      //---------------Test Result -----------------------
      exception.Message.Should().Contain("TestDictionary Property is decorated with FakeTestAttribute " +
                                         "but the attribute property Sequence is not set to 55");
    }
  }
}
