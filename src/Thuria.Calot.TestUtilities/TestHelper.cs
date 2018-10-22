using System.Reflection;
using System.Collections.Generic;

using Thuria.Zitidar.Extensions;

namespace Thuria.Calot.TestUtilities
{
  /// <summary>
  /// General Test Helper methods
  /// </summary>
  public static class TestHelper
  {
    /// <summary>
    /// Create Random Parameter Values
    /// </summary>
    /// <param name="allParameters">List of Parameters</param>
    /// <param name="nullParameterName">Parameter that should be null</param>
    /// <returns>List of random generated parameter values</returns>
    public static List<object> CreateParameterValues(IEnumerable<ParameterInfo> allParameters, string nullParameterName = "")
    {
      var parameterValues = new List<object>();

      foreach (var currentParameter in allParameters)
      {
        var parameterValue = currentParameter.Name == nullParameterName
                                  ? currentParameter.ParameterType.GetDefaultData()
                                  : RandomValueGenerator.CreateRandomValue(currentParameter.ParameterType);
        parameterValues.Add(parameterValue);
      }

      return parameterValues;
    }
  }
}
