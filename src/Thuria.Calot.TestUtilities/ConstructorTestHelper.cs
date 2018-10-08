﻿using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using NUnit.Framework;
using FluentAssertions;

using Thuria.Zitidar.Extensions;

namespace Thuria.Calot.TestUtilities
{
  /// <summary>
  /// Constructor Test Helper Methods
  /// </summary>
  public static class ConstructorTestHelper
  {
    /// <summary>
    /// Helper method to construct any object and create default parameters as necessary
    /// </summary>
    /// <typeparam name="T">Object Type to be Constructed</typeparam>
    /// <param name="parameterName">Parameter Name of parameter that should contain null or specified value (Optional)</param>
    /// <param name="parameterValue">Parameter Value that should be used for specified parameter</param>
    /// <returns>Newly constructed object</returns>
    public static T ConstructObject<T>(string parameterName = null, object parameterValue = null) 
      where T : class
    {
      return (T) ConstructObject(typeof(T), parameterName, parameterValue);
    }

    /// <summary>
    /// Helper method to construct any object and create default parameters as necessary
    /// </summary>
    /// <param name="objectType">Object Type to be Constructed</param>
    /// <param name="parameterName">Parameter Name of parameter that should contain null or specified value (Optional)</param>
    /// <param name="parameterValue">Parameter Value that should be used for specified parameter</param>
    /// <returns>Newly constructed object</returns>
    public static object ConstructObject(Type objectType, string parameterName = null, object parameterValue = null)
    {
      var constructorInfo = objectType.GetConstructors().OrderBy(info => info.GetParameters().Length).FirstOrDefault();
      if (constructorInfo == null)
      {
        throw new Exception($"No Constructors found for object {objectType.FullName}");
      }

      var constructorParameters      = constructorInfo.GetParameters();
      var constructorParameterValues = new List<object>();

      foreach (var currentParameter in constructorParameters)
      {
        if (!string.IsNullOrWhiteSpace(parameterName) && currentParameter.Name == parameterName)
        {
          constructorParameterValues.Add(parameterValue);
          continue;
        }

        constructorParameterValues.Add(currentParameter.CreateRandomValue());
      }

      return constructorParameterValues.Any() ? constructorInfo.Invoke(constructorParameterValues.ToArray()) : Activator.CreateInstance(objectType);
    }

    /// <summary>
    /// Validate that when a null parameter is given to a constructor, an ArgumentNullException is thrown
    /// </summary>
    /// <typeparam name="T">Object Type to test</typeparam>
    /// <param name="parameterName">Parameter Name to test</param>
    public static void ValidateArgumentNullExceptionIfParameterIsNull<T>(string parameterName) 
      where T : class
    {
      try
      {
        ConstructObject<T>(parameterName);
        Assert.Fail($"ArgumentNullException not throw for Constructor Parameter [{parameterName}] on {typeof(T).FullName}");
      }
      catch (TargetInvocationException invocationException)
      {
        var argumentNullException = (invocationException.InnerException as ArgumentNullException);
        if (argumentNullException == null)
        {
          Assert.Fail($"ArgumentNullException not throw for Constructor Parameter [{parameterName}] on {typeof(T).FullName}");
        }

        argumentNullException.ParamName.Should().Be(parameterName);
      }
    }

    /// <summary>
    /// Validate that a property is set with the value given during the constructing of the object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parameterName">Constructor parameter Name</param>
    /// <param name="propertyName">Object Property Name</param>
    public static void ValidatePropertySetWithParameter<T>(string parameterName, string propertyName) 
      where T : class
    {
      var parameterValue = typeof(T).GetProperty(propertyName).CreateRandomValue();
      var objectUnderTest = ConstructObject<T>(parameterName, parameterValue);
      if (objectUnderTest == null)
      {
        Assert.Fail($"Failed to create {typeof(T).FullName} to test Property Get and Set for {propertyName}");
      }

      var propertyValue = objectUnderTest.GetPropertyValue(propertyName);
      propertyValue.Should().Be(parameterValue, $"because parameter [{parameterName}] of the constructor of [{typeof(T).FullName}] should set property [{propertyName}]");
    }
  }
}