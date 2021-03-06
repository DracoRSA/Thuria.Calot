﻿using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
    /// Validate that if a specified argument value is null, that a ArgumentNullException is thrown on an Async method
    /// </summary>
    /// <typeparam name="T">Type under test</typeparam>
    /// <param name="methodName">Method to be test</param>
    /// <param name="parameterName">Method Parameter Name to verify</param>
    /// <param name="parameterValue">Parameter Value (Default null)</param>
    public static void ValidateArgumentNullExceptionIfParameterIsNullAsync<T>(string methodName, string parameterName, object parameterValue = null)
    {
      if (methodName == null) { throw new ArgumentNullException(nameof(methodName)); }
      if (parameterName == null) { throw new ArgumentNullException(nameof(parameterName)); }

      var methodInfo = GetMethodInformation<T>(methodName, parameterName);
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

      var constructedObject   = ConstructorTestHelper.ConstructObject(typeof(T));
      var argumentNullException = Assert.ThrowsAsync<ArgumentNullException>(async () => await (Task)methodInfo.Invoke(constructedObject, methodParameterValues.ToArray()),
                                                                          $"No Exception not throw for Method [{methodName}] Parameter [{parameterName}] on {typeof(T).FullName}");
      argumentNullException.ParamName.Should().Be(parameterName);
    }

    /// <summary>
    /// Validate that if a specified argument value is null, that a specified exception is thrown on an Async method
    /// </summary>
    /// <typeparam name="T">Type under test</typeparam>
    /// <typeparam name="TException">Exception expected to be thrown</typeparam>
    /// <param name="methodName">Method to be test</param>
    /// <param name="parameterName">Method Parameter Name to verify</param>
    /// <param name="parameterValue">Parameter Value (Default null)</param>
    public static void ValidateExceptionIsThrownIfParameterIsNullAsync<T, TException>(string methodName, string parameterName, object parameterValue = null)
      where TException : Exception
    {
      if (methodName == null) { throw new ArgumentNullException(nameof(methodName)); }
      if (parameterName == null) { throw new ArgumentNullException(nameof(parameterName)); }

      var methodInfo = GetMethodInformation<T>(methodName, parameterName);
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

      var constructedObject     = ConstructorTestHelper.ConstructObject(typeof(T));
      var argumentNullException = Assert.ThrowsAsync(typeof(TException), 
                                                     () => (Task)methodInfo.Invoke(constructedObject, methodParameterValues.ToArray()),
                                                     $"{typeof(TException).Namespace} Exception not throw for Method [{methodName}] Parameter [{parameterName}] on {typeof(T).FullName}");
      argumentNullException.Message.Should().Contain(parameterName);
    }

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

      var methodInfo            = GetMethodInformation<T>(methodName, parameterName);
      var methodParameters      = methodInfo.GetParameters();
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

      var constructedObject   = ConstructorTestHelper.ConstructObject(typeof(T));
      var invocationException = Assert.Throws<TargetInvocationException>(() => methodInfo.Invoke(constructedObject, methodParameterValues.ToArray()));

      var argumentNullException = (invocationException.InnerException as ArgumentNullException);
      if (argumentNullException == null)
      {
        Assert.Fail($"ArgumentNullException not throw for Method [{methodName}] Parameter [{parameterName}] on {typeof(T).FullName}");
      }

      argumentNullException.ParamName.Should().Be(parameterName);
    }

    /// <summary>
    /// Validate that if a specified argument value is null, that a specified exception is thrown
    /// </summary>
    /// <typeparam name="T">Type under test</typeparam>
    /// <typeparam name="TException">Exception expected to be thrown</typeparam>
    /// <param name="methodName">Method to be test</param>
    /// <param name="parameterName">Method Parameter Name to verify</param>
    /// <param name="parameterValue">Parameter Value (Default null)</param>
    public static void ValidateExceptionIsThrownIfParameterIsNull<T, TException>(string methodName, string parameterName, object parameterValue = null)
      where TException : Exception
    {
      if (methodName == null) { throw new ArgumentNullException(nameof(methodName)); }
      if (parameterName == null) { throw new ArgumentNullException(nameof(parameterName)); }

      var methodInfo            = GetMethodInformation<T>(methodName, parameterName);
      var methodParameters      = methodInfo.GetParameters();
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

      var constructedObject   = ConstructorTestHelper.ConstructObject(typeof(T));
      var invocationException = Assert.Throws<TargetInvocationException>(() => methodInfo.Invoke(constructedObject, methodParameterValues.ToArray()));

      var argumentNullException = (invocationException.InnerException as TException);
      if (argumentNullException == null)
      {
        Assert.Fail($"{typeof(TException).Name} Exception not throw for Method [{methodName}] Parameter [{parameterName}] on {typeof(T).FullName}");
      }

      argumentNullException.Message.Should().Contain(parameterName);
    }

    /// <summary>
    /// Validate that the method is Decorated with the expected Attribute
    /// </summary>
    /// <typeparam name="T">Object Type under test</typeparam>
    /// <param name="methodName">Object Method Name</param>
    /// <param name="attributeType">Attribute Type</param>
    /// <param name="attributePropertyValues">Attribute Property Values</param>
    /// <param name="supportMultipleMethods">Allow multiple matching methods (Default : false)</param>
    /// <param name="matchingParameters">List of parameters to find the method to test</param>
    public static void ValidateDecoratedWithAttribute<T>(string methodName, 
                                                         Type attributeType,
                                                         List<(string propertyName, object propertyValue)> attributePropertyValues = null,
                                                         bool supportMultipleMethods = false,
                                                         List<string> matchingParameters = null) 
      where T : class
    {
      if (string.IsNullOrWhiteSpace(methodName))
      {
        throw new ArgumentNullException(nameof(methodName));
      }

      if (attributeType == null)
      {
        throw new ArgumentNullException(nameof(attributeType));
      }

      var objectUnderTest = ConstructorTestHelper.ConstructObject<T>();
      if (objectUnderTest == null)
      {
        Assert.Fail($"Failed to create {typeof(T).FullName} to validate Method {methodName} decorated with attribute {attributeType.Name}");
      }

      var allMethods = objectUnderTest.GetType().GetMethods().Where(info => info.Name == methodName).ToList();
      if (allMethods == null || !allMethods.Any())
      {
        Assert.Fail($"Method [{methodName}] does not exists on {objectUnderTest.GetType().FullName}");
      }

      MethodInfo methodUnderTest = null;
      if (allMethods.Count() == 1)
      {
        methodUnderTest = allMethods.First();
      }
      else if (!supportMultipleMethods)
      {
        Assert.Fail("Multiple methods with the same name found. Please specify the method to use");
      }
      else if (matchingParameters == null || !matchingParameters.Any())
      {
        Assert.Fail("Multiple methods with the same name found. Please specify the method to use by specifying the matching parameters");
      }
      else
      {
        foreach (var currentMethod in allMethods)
        {
          var methodParameters = currentMethod.GetParameters();
          if (matchingParameters.Any(parameterName => methodParameters.FirstOrDefault(info => info.Name == parameterName) == null))
          {
            continue;
          }

          methodUnderTest = currentMethod;
          break;
        }

        if (methodUnderTest == null)
        {
          Assert.Fail("Failed to find a matching method using the specified matching parameters");
        }
      }

      var customAttributes = methodUnderTest.GetCustomAttributes(attributeType).ToList();
      if (customAttributes == null || !customAttributes.Any())
      {
        Assert.Fail($"Method {methodName} is not decorated with {attributeType.Name} Attribute");
      }

      if (attributePropertyValues == null)
      {
        return;
      }

      var evalDetails = new List<(string PropertyNames, object propertyValue, bool foundMethod, bool foundValue)>();

      foreach (var (propertyName, propertyValue) in attributePropertyValues)
      {
        (string PropertyName, object propertyValue, bool foundMethod, bool foundValue) evalMetadata = (propertyName, propertyValue, false, false);

        foreach (var currentAttribute in customAttributes)
        {
          var propertyInfo = currentAttribute.GetType().GetProperty(propertyName);
          if (propertyInfo == null) { continue; }

          evalMetadata.foundMethod = true;

          if (propertyInfo.GetValue(currentAttribute).Equals(propertyValue))
          {
            evalMetadata.foundValue = true;
          }
        }

        evalDetails.Add(evalMetadata);
      }

      if (evalDetails.Any(evalMetadata => evalMetadata.foundValue && evalMetadata.foundValue))
      {
        return;
      }

      var errorMessage = new StringBuilder();
      foreach (var (propertyName, propertyValue, foundMethod, foundValue) in evalDetails)
      {
        if (!foundMethod)
        {
          errorMessage.AppendLine($"{methodName} Method is decorated with {attributeType.Name} " +
                                  $"but the attribute property {propertyName} does not exist on the attribute");
        }

        if (!foundValue)
        {
          errorMessage.AppendLine($"{methodName} Method is decorated with {attributeType.Name} " +
                                  $"but the attribute property {propertyName} is not set to {propertyValue}");
        }
      }
      
      Assert.Fail(errorMessage.ToString());
    }

    private static MethodInfo GetMethodInformation<T>(string methodName, string parameterName)
    {
      var allMethodInfos = typeof(T).GetMethods().Where(info => info.Name == methodName).ToList();
      if (allMethodInfos == null || !allMethodInfos.Any())
      {
        Assert.Fail($"Method [{methodName}] does not exists on {typeof(T).FullName}");
      }

      MethodInfo methodInfo = null;
      if (allMethodInfos.Count > 1)
      {
        foreach (var currentMethodInfo in allMethodInfos)
        {
          var parameterInfo = currentMethodInfo.GetParameters().FirstOrDefault(info => info.Name == parameterName);
          if (parameterInfo == null)
          {
            continue;
          }

          methodInfo = currentMethodInfo;
          break;
        }
      }
      else
      {
        methodInfo = allMethodInfos.First();
      }

      if (methodInfo == null)
      {
        throw new InvalidOperationException($"Method [{methodName}] does not contain parameter named {parameterName}");
      }

      return methodInfo;
    }
  }
}
