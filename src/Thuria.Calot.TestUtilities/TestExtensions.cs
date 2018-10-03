using System.Reflection;

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
  }
}
