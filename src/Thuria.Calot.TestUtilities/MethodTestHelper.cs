using System;
using System.Reflection;
using System.Collections.Generic;

using NUnit.Framework;
using FluentAssertions;

namespace Thuria.Calot.TestUtilities
{
  /// <summary>
  /// Method Test Helper class
  /// </summary>
  public static class MethodTestHelper
  {
    /// <summary>
    /// Validate that if a specified argument value is null, that a ArgumentNullException is thrown
    /// </summary>
    /// <typeparam name="T">Type under test</typeparam>
    /// <param name="methodName">Method to be test</param>
    /// <param name="parameterName">Method Parameter Name to verify</param>
    /// <param name="parameterValue">Parameter Value (Default null)</param>
    public static void ValidateArgumentNullExceptionIfParameterIsNull<T>(string methodName, string parameterName, object parameterValue = null)
    {
      if (methodName == null) { throw new ArgumentNullException(nameof(methodName)); }
      if (parameterName == null) { throw new ArgumentNullException(nameof(parameterName)); }

      var methodInfo = typeof(T).GetMethod(methodName);
      if (methodInfo == null)
      {
        throw new InvalidOperationException($"Method [{methodName}] does not exists on {typeof(T).FullName}");
      }

      var methodParameters = methodInfo.GetParameters();
      var methodParameterValues = new List<object>();

      foreach (var currentParameter in methodParameters)
      {
        if (!string.IsNullOrWhiteSpace(parameterName) && currentParameter.Name == parameterName)
        {
          methodParameterValues.Add(parameterValue);
          continue;
        }

        methodParameterValues.Add(currentParameter.CreateRandomValue());
      }

      var constructedObject = ConstructorTestHelper.ConstructObject(typeof(T));

      var invocationException = Assert.Throws<TargetInvocationException>(() => methodInfo.Invoke(constructedObject, methodParameterValues.ToArray()));
      var argumentNullException = (invocationException.InnerException as ArgumentNullException);
      if (argumentNullException == null)
      {
        Assert.Fail($"ArgumentNullException not throw for Method [{methodName}] Parameter [{parameterName}] on {typeof(T).FullName}");
      }

      argumentNullException.ParamName.Should().Be(parameterName);
    }
  }
}
