using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using FluentAssertions;
using Thuria.Calot.TestUtilities.Tests.Fake;
using Thuria.Zitidar.Extensions;

namespace Thuria.Calot.TestUtilities.Tests
{
  [TestFixture]
  public class TestConstructorTestHelper
  {
    [Test]
    public void ConstructObject_GivenGenericType_ShouldNotThrowExceptionAndConstructObject()
    {
      //---------------Set up test pack-------------------
      FakeTestClass testClass = null;
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      Assert.DoesNotThrow(() => testClass = ConstructorTestHelper.ConstructObject<FakeTestClass>());
      //---------------Test Result -----------------------
      testClass.Should().NotBeNull();
    }

    [TestCase("TestDictionary", typeof(Dictionary<string, string>))]
    [TestCase("TestDictionary2", typeof(Dictionary<string, object>))]
    public void ConstructObject_GivenGenericType_ShouldNotThrowExceptionAndConstructObjectWithValuesAsExpected(string propertyName, Type expectedType)
    {
      //---------------Set up test pack-------------------
      FakeTestClass testClass = null;
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      Assert.DoesNotThrow(() => testClass = ConstructorTestHelper.ConstructObject<FakeTestClass>());
      //---------------Test Result -----------------------
      testClass.Should().NotBeNull();

      var propertyValue = testClass.GetPropertyValue(propertyName);
      propertyValue.Should().NotBeNull();
      propertyValue.Should().BeOfType(expectedType);
    }

    [Test]
    public void ConstructObject_GivenGenericAndParameterAndNoValue_ShouldConstructObjectWithNullValue()
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var testClass = ConstructorTestHelper.ConstructObject<FakeTestClass>("allFakes");
      //---------------Test Result -----------------------
      testClass.Should().NotBeNull();
      testClass.FakeList.Should().BeNullOrEmpty();
    }

    [Test]
    public void ConstructObject_GivenGenericAndParameterAndValue_ShouldConstructObjectWithExpectedValue()
    {
      //---------------Set up test pack-------------------
      var parameterValue = "FakeTestName";
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var testClass = ConstructorTestHelper.ConstructObject<FakeTestClass>("testName", parameterValue);
      //---------------Test Result -----------------------
      testClass.Should().NotBeNull();
      testClass.Name.Should().Be(parameterValue);
    }

    [Test]
    public void ConstructObject_GivenType_ShouldNotThrowExceptionAndConstructObject()
    {
      //---------------Set up test pack-------------------
      FakeTestClass testClass = null;
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      Assert.DoesNotThrow(() => testClass = (FakeTestClass) ConstructorTestHelper.ConstructObject(typeof(FakeTestClass)));
      //---------------Test Result -----------------------
      testClass.Should().NotBeNull();
    }

    [Test]
    public void ConstructObject_GivenTypeAndParameterAndNoValue_ShouldConstructObjectWithNullValue()
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var testClass = (FakeTestClass) ConstructorTestHelper.ConstructObject(typeof(FakeTestClass), "allFakes");
      //---------------Test Result -----------------------
      testClass.Should().NotBeNull();
      testClass.FakeList.Should().BeNullOrEmpty();
    }

    [Test]
    public void ConstructObject_GivenTypeAndParameterAndValue_ShouldConstructObjectWithExpectedValue()
    {
      //---------------Set up test pack-------------------
      var parameterValue = "FakeTestName";
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var testClass = (FakeTestClass) ConstructorTestHelper.ConstructObject(typeof(FakeTestClass), "testName", parameterValue);
      //---------------Test Result -----------------------
      testClass.Should().NotBeNull();
      testClass.Name.Should().Be(parameterValue);
    }

    [Test]
    public void ConstructObject_GivenParameterValues_ShouldConstructObjectWithParameterValues()
    {
      //---------------Set up test pack-------------------
      var testDateTime = DateTime.Now;
      var fakeComplex  = new FakeComplex();

      var parameterValues = new List<(string paramName, object paramValue)>
        {
          ("testDateTime", testDateTime), ("complexObject", fakeComplex)
        };
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var testClass = ConstructorTestHelper.ConstructObject<FakeTestClass>(constructorParams: parameterValues.ToArray());
      //---------------Test Result -----------------------
      testClass.TestDateTime.Should().BeSameDateAs(testDateTime);
      testClass.ComplexObject2.Should().Be(fakeComplex);
    }

    [Test]
    public void ConstructObject_GivenException_ShouldConstructObjectAndNotThrowException()
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      Assert.DoesNotThrow(() =>
        {
          var fakeException = ConstructorTestHelper.ConstructObject<FakeException>();
          //---------------Test Result -----------------------
          fakeException.Should().NotBeNull();
        });
    }

    [Test]
    public void ConstructObject_GivenArray_ShouldNotThrowException()
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      Assert.DoesNotThrow(() => ConstructorTestHelper.ConstructObject<FakeTestClass2>());
      //---------------Test Result -----------------------
    }

    [TestCase("fakeComplex")]
    [TestCase("fakeClass")]
    public void ConstructObject_GivenObjectWithMultipleConstructors_ShouldConstructUsingMatchingConstructor(string parameterName)
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var exception = Assert.Throws<TargetInvocationException>(() => ConstructorTestHelper.ConstructObject<FakeTestClass3>(parameterName));
      //---------------Test Result -----------------------
      exception.Should().NotBeNull();
      exception.InnerException.Should().BeOfType<ArgumentNullException>();
      ((ArgumentNullException) exception.InnerException)?.ParamName.Should().Be(parameterName);
    }

    [Test]
    public void ValidateArgumentNullExceptionIfParameterIsNull_GivenParameterWhereExceptionIsNotThrown_ShouldFailTest()
    {
      //---------------Set up test pack-------------------
      var parameterName = "complexObjectNotTested";
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var exception = Assert.Throws<AssertionException>(() => ConstructorTestHelper.ValidateArgumentNullExceptionIfParameterIsNull<FakeTestClass>(parameterName));
      //---------------Test Result -----------------------
      exception.Message.Should().Be($"ArgumentNullException not throw for Constructor Parameter [{parameterName}] on {typeof(FakeTestClass).FullName}");
    }

    [TestCase("testName")]
    [TestCase("complexObject")]
    [TestCase("complexInterface")]
    [TestCase("testDictionary")]
    [TestCase("testDictionary2")]
    public void ValidateArgumentNullExceptionIfParameterIsNull_GivenParameterWhereExceptionIsThrown_ShouldPassTest(string parameterName)
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      Assert.DoesNotThrow(() => ConstructorTestHelper.ValidateArgumentNullExceptionIfParameterIsNull<FakeTestClass>(parameterName));
      //---------------Test Result -----------------------
    }

    [Test]
    public void ValidateArgumentNullExceptionIfParameterIsNull_GivenParameterValuesAndExceptionNotThrown_ShouldFailTest()
    {
      //---------------Set up test pack-------------------
      var parameterName = "notSetParameter";
      var testDateTime  = DateTime.Now;
      var fakeComplex   = new FakeComplex();

      var parameterValues = new List<(string paramName, object paramValue)>
        {
          ("testDateTime", testDateTime), ("complexObject", fakeComplex)
        };
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var exception = Assert.Throws<AssertionException>(() => ConstructorTestHelper.ValidateArgumentNullExceptionIfParameterIsNull<FakeTestClass>(parameterName, 
                                                                                                                                                  parameterValues.ToArray()));
      //---------------Test Result -----------------------
      exception.Message.Should().Contain($"ArgumentNullException not throw for Constructor Parameter [{parameterName}] on {typeof(FakeTestClass).FullName}");
    }

    [TestCase("testName")]
    [TestCase("complexObject")]
    [TestCase("complexInterface")]
    public void ValidateArgumentNullExceptionIfParameterIsNull_GivenParameterWhereExceptionIsThrownAndParameterValues_ShouldPassTest(string parameterName)
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      Assert.DoesNotThrow(() => ConstructorTestHelper.ValidateArgumentNullExceptionIfParameterIsNull<FakeTestClass>(parameterName, ("testDateTime", DateTime.Now)));
      //---------------Test Result -----------------------
    }

    [Test]
    public void ValidatePropertySetWithParameter_GivenParameterNotSettingProperty_ShouldFailTest()
    {
      //---------------Set up test pack-------------------
      var parameterName = "notSetParameter";
      var propertyName  = "NotSetProperty";
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var exception = Assert.Throws<AssertionException>(() => ConstructorTestHelper.ValidatePropertySetWithParameter<FakeTestClass>(parameterName, propertyName));
      //---------------Test Result -----------------------
      exception.Message.Should().Contain($"because parameter [{parameterName}] of the constructor of [{typeof(FakeTestClass).FullName}] should set property [{propertyName}]");
    }

    [Test]
    public void ValidatePropertySetWithParameter_GivenParameterNotSettingProperty_ShouldPassTest()
    {
      //---------------Set up test pack-------------------
      var parameterName = "complexObject";
      var propertyName  = "ComplexObject2";
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      Assert.DoesNotThrow(() => ConstructorTestHelper.ValidatePropertySetWithParameter<FakeTestClass>(parameterName, propertyName));
      //---------------Test Result -----------------------
    }

    [TestCase("testName")]
    [TestCase("complexObject")]
    [TestCase("complexInterface")]
    [TestCase("testDictionary")]
    [TestCase("testDictionary2")]
    public void ValidateExceptionIsThrownIfParameterIsNull_GivenParameterWhereExceptionIsThrown_ShouldPassTest(string parameterName)
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      Assert.DoesNotThrow(() => ConstructorTestHelper.ValidateExceptionIsThrownIfParameterIsNull<FakeTestClass, ArgumentNullException>(parameterName));
      //---------------Test Result -----------------------
    }

    [Test]
    public void ValidateExceptionIsThrownIfParameterIsNull_GivenParameterValuesAndExceptionNotThrown_ShouldFailTest()
    {
      //---------------Set up test pack-------------------
      var parameterName = "notSetParameter";
      var testDateTime = DateTime.Now;
      var fakeComplex = new FakeComplex();
    
      var parameterValues = new List<(string paramName, object paramValue)>
        {
          ("testDateTime", testDateTime), ("complexObject", fakeComplex)
        };
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var exception = Assert.Throws<AssertionException>(() => ConstructorTestHelper.ValidateExceptionIsThrownIfParameterIsNull<FakeTestClass, ArgumentNullException>(parameterName,
                                                                                                                                                  parameterValues.ToArray()));
      //---------------Test Result -----------------------
      exception.Message.Should().Contain($"ArgumentNullException Exception not throw for Constructor Parameter [{parameterName}] on {typeof(FakeTestClass).FullName}");
    }
    
    [TestCase("testName")]
    [TestCase("complexObject")]
    [TestCase("complexInterface")]
    public void ValidateExceptionIsThrownIfParameterIsNull_GivenParameterWhereExceptionIsThrownAndParameterValues_ShouldPassTest(string parameterName)
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      Assert.DoesNotThrow(() => ConstructorTestHelper.ValidateExceptionIsThrownIfParameterIsNull<FakeTestClass, ArgumentNullException>(parameterName, ("testDateTime", DateTime.Now)));
      //---------------Test Result -----------------------
    }
  }
}
