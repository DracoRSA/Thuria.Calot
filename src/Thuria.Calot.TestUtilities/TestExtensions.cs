using System;
using System.Linq;
using System.Reflection;
using NSubstitute;

namespace Thuria.Calot.TestUtilities
{
  /// <summary>
  /// Test Extension methods
  /// </summary>
  public static class TestExtensions
  {
    /// <summary>
    /// Create Random Value for Parameter
    /// </summary>
    /// <param name="parameterInfo"></param>
    /// <returns></returns>
    public static object CreateRandomValue(this ParameterInfo parameterInfo)
    {
      return RandomValueGenerator.CreateRandomValue(parameterInfo.ParameterType);
    }

    /// <summary>
    /// Create Random Value for Property
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <returns></returns>
    public static object CreateRandomValue(this PropertyInfo propertyInfo)
    {
      return RandomValueGenerator.CreateRandomValue(propertyInfo.PropertyType);
    }

    /// <summary>
    /// Create a NSubstitute Mocked object
    /// </summary>
    /// <param name="objectType">Object Type</param>
    /// <returns>NSubstitute Mocked object</returns>
    public static object CreateSubstitute(this Type objectType)
    {
      var constructorInfo       = objectType.GetConstructors().OrderByDescending(info => info.GetParameters().Length).FirstOrDefault();
      var constructorParameters = constructorInfo?.GetParameters();

      if (constructorInfo == null || !constructorParameters.Any())
      {
        return Substitute.For(new[] { objectType }, new object[0]);
      }

      var parameterValues = TestHelper.CreateParameterValues(constructorParameters);
      return Substitute.For(new[] { objectType }, parameterValues.ToArray());
    }
  }
}
