using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

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
