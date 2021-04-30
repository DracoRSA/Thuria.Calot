using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using NUnit.Framework;
using FluentAssertions;
using Thuria.Calot.TestUtilities.Tests.Fake;

namespace Thuria.Calot.TestUtilities.Tests
{
  [TestFixture]
  public class TestMethodTestHelper
  {
    [TestCase("testData")]
    [TestCase("testObjectList")]
    public void ValidateArgumentNullExceptionIfParameterIsNull_GivenOverloadedMethods_ShouldTestMethodWithParameterNameMatch(string parameterName)
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      MethodTestHelper.ValidateArgumentNullExceptionIfParameterIsNull<FakeMethodTest>("OverloadedMethod", parameterName);
      //---------------Test Result -----------------------
    }

    [Test]
    public void ValidateArgumentNullExceptionIfParameterIsNullAsync_GivenNullParameter_ShouldDetectArgumentNullException()
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      MethodTestHelper.ValidateArgumentNullExceptionIfParameterIsNullAsync<FakeMethodTest>("TestMethodAsync", "nonNullParameter");
      //---------------Test Result -----------------------
    }

    [TestCase("testData")]
    [TestCase("testObjectList")]
    public void ValidateExceptionIsThrownIfParameterIsNull_GivenOverloadedMethods_ShouldTestMethodWithParameterNameMatch(string parameterName)
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      MethodTestHelper.ValidateExceptionIsThrownIfParameterIsNull<FakeMethodTest, ArgumentNullException>("OverloadedMethod", parameterName);
      //---------------Test Result -----------------------
    }
    
    [Test]
    public void ValidateExceptionIsThrownIfParameterIsNullAsync_GivenNullParameter_ShouldDetectArgumentNullException()
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      MethodTestHelper.ValidateExceptionIsThrownIfParameterIsNullAsync<FakeMethodTest, ArgumentNullException>("TestMethodAsync", "nonNullParameter");
      //---------------Test Result -----------------------
    }

    [Test]
    public void ValidateDecoratedWithAttribute_GivenNullMethodName_ShouldFailTest()
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var exception = Assert.Throws<ArgumentNullException>(() => MethodTestHelper.ValidateDecoratedWithAttribute<FakeMethodTest>(null, typeof(FakeTestAttribute)));
      //---------------Test Result -----------------------
      exception.ParamName.Should().Be("methodName");
    }

    [Test]
    public void ValidateDecoratedWithAttribute_GivenMethodNameDoesNotExist_ShouldFailTest()
    {
      //---------------Set up test pack-------------------
      var methodName = "UnknownMethod";
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var exception = Assert.Throws<AssertionException>(() => MethodTestHelper.ValidateDecoratedWithAttribute<FakeMethodTest>(methodName, typeof(FakeTestAttribute)));
      //---------------Test Result -----------------------
      exception.Message.Should().Be($"Method [{methodName}] does not exists on {typeof(FakeMethodTest).FullName}");
    }

    [Test]
    public void ValidateDecoratedWithAttribute_GivenNullAttributeType_ShouldFailTest()
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var exception = Assert.Throws<ArgumentNullException>(() => MethodTestHelper.ValidateDecoratedWithAttribute<FakeMethodTest>("OverloadedMethod", null));
      //---------------Test Result -----------------------
      exception.ParamName.Should().Be("attributeType");
    }

    [Test]
    public void ValidateDecoratedWithAttribute_GivenMethodDecoratedWithAttribute_ShouldPassTest()
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      MethodTestHelper.ValidateDecoratedWithAttribute<FakeTestClass>("TestMethod1", typeof(FakeTestAttribute));
      //---------------Test Result -----------------------
    }

    [Test]
    public void ValidateDecoratedWithAttribute_GivenMethodDecoratedWithAttribute_And_AttributeParametersMatch_ShouldPassTest()
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      MethodTestHelper.ValidateDecoratedWithAttribute<FakeTestClass>("TestMethod1", typeof(FakeTestAttribute),
                                                                     new List<(string propertyName, object propertyValue)>
                                                                       {
                                                                         ("PropertyName", "TestMethod1"), ("Sequence", 2)
                                                                       });
      //---------------Test Result -----------------------
    }

    [Test]
    public void ValidateDecoratedWithAttribute_GivenMethodDecoratedWithAttribute_And_AttributeParametersDoNotMatch_ShouldFailTest()
    {
      //---------------Set up test pack-------------------
      var methodName    = "TestMethod1";
      var attributeType = typeof(FakeTestAttribute);
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var exception = Assert.Throws<AssertionException>(() => MethodTestHelper.ValidateDecoratedWithAttribute<FakeTestClass>(methodName, attributeType, 
                                                                                                                             new List<(string propertyName, object propertyValue)>
                                                                                                                               {
                                                                                                                                 ("PropertyName", "TestMethod2"),
                                                                                                                                 ("Sequence", 1)
                                                                                                                               }));
      //---------------Test Result -----------------------
      exception.Message.Should().Contain($"{methodName} Method is decorated with {attributeType.Name} but the attribute property PropertyName is not set to TestMethod2");
      exception.Message.Should().Contain($"{methodName} Method is decorated with {attributeType.Name} but the attribute property Sequence is not set to 1");
    }

    [Test]
    public void ValidateDecoratedWithAttribute_GivenMethodNotDecoratedWithAttribute_ShouldPassTest()
    {
      //---------------Set up test pack-------------------
      var methodName    = "TestMethod2";
      var attributeType = typeof(FakeTestAttribute);
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var exception = Assert.Throws<AssertionException>(() => MethodTestHelper.ValidateDecoratedWithAttribute<FakeTestClass>(methodName, attributeType));
      //---------------Test Result -----------------------
      exception.Message.Should().Be($"Method {methodName} is not decorated with {attributeType.Name} Attribute");
    }

    [Test]
    public void ValidateDecoratedWithAttribute_GivenMultipleMethodsWithSameName_And_NotAllowMultipleMethods_ShouldFailTest()
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var exception = Assert.Throws<AssertionException>(() => MethodTestHelper.ValidateDecoratedWithAttribute<FakeTestClass2>("FakeTestMethod", 
                                                                                                                              typeof(FakeTestAttribute),
                                                                                                                              supportMultipleMethods: false));
      //---------------Test Result -----------------------
      exception.Message.Should().Be("Multiple methods with the same name found. Please specify the method to use");
    }

    [Test]
    public void ValidateDecoratedWithAttribute_GivenMultipleMethodsWithSameName_And_AllowMultipleMethods_And_NoMatchingParameters_ShouldFailTest()
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var exception = Assert.Throws<AssertionException>(() => MethodTestHelper.ValidateDecoratedWithAttribute<FakeTestClass2>("FakeTestMethod", 
                                                                                                                              typeof(FakeTestAttribute), 
                                                                                                                              supportMultipleMethods: true));
      //---------------Test Result -----------------------
      exception.Message.Should().Be("Multiple methods with the same name found. Please specify the method to use by specifying the matching parameters");
    }

    [Test]
    public void ValidateDecoratedWithAttribute_GivenMultipleMethodsWithSameName_And_AllowMultipleMethods_And_NoParametersFail_ShouldFailTest()
    {
      //---------------Set up test pack-------------------
      var matchingParameters = new List<string>
                                 {
                                   "someParameter",
                                   "unmatchedParameter"
                                 };
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var exception = Assert.Throws<AssertionException>(() => MethodTestHelper.ValidateDecoratedWithAttribute<FakeTestClass2>("FakeTestMethod", 
                                                                                                                              typeof(FakeTestAttribute), 
                                                                                                                              supportMultipleMethods: true,
                                                                                                                              matchingParameters: matchingParameters));
      //---------------Test Result -----------------------
      exception.Message.Should().Be("Failed to find a matching method using the specified matching parameters");
    }

    [Test]
    public void ValidateDecoratedWithAttribute_GivenMultipleMethodsWithSameName_And_MethodDecoratedWithAttribute_ShouldPassTest()
    {
      //---------------Set up test pack-------------------
      var matchingParameters = new List<string>
                   {
                     "someParameter"
                   };
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      MethodTestHelper.ValidateDecoratedWithAttribute<FakeTestClass2>("FakeTestMethod", typeof(FakeTestAttribute), 
                                                                      supportMultipleMethods: true, 
                                                                      matchingParameters: matchingParameters);
      //---------------Test Result -----------------------
    }

    private class FakeMethodTest
    {
      public void OverloadedMethod(string testData, object testObject)
      {
        if (string.IsNullOrWhiteSpace(testData))
        {
          throw new ArgumentNullException(nameof(testData));
        }
      }

      public void OverloadedMethod(List<object> testObjectList)
      {
        if (testObjectList == null)
        {
          throw new ArgumentNullException(nameof(testObjectList));
        }
      }

      public Task TestMethodAsync(string nonNullParameter)
      {
        var taskCompletionSource = new TaskCompletionSource<bool>();

        try
        {
          if (string.IsNullOrWhiteSpace(nonNullParameter))
          {
            throw new ArgumentNullException(nameof(nonNullParameter));
          }

          taskCompletionSource.SetResult(true);
        }
        catch (Exception runtimeException)
        {
          taskCompletionSource.SetException(runtimeException);
        }

        return taskCompletionSource.Task;
      }
    }
  }
}
