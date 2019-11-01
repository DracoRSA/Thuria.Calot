using System;
using System.Reflection;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
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

    /// <summary>
    /// Create Substitute Data Reader
    /// </summary>
    /// <typeparam name="T">Data Reader Data Type</typeparam>
    /// <param name="dataReaderData">Data Reader Data</param>
    /// <param name="additionalColumnValues">Additional Column Values (Optional)</param>
    /// <param name="customColumnAttribute">Custom Column Attribute to verify the Column Name with (Optional [Property - DbColumnName])</param>
    /// <returns>A mocked (NSubstitute) IDataReader object</returns>
    public static IDataReader CreateSubstituteDataReader<T>(List<T> dataReaderData, 
                                                            Dictionary<string, object> additionalColumnValues = null,
                                                            Type customColumnAttribute = null)
    {
      var currentReaderCount = -1;

      var dataReader = Substitute.For<IDataReader>();
      dataReader.Read().Returns(info =>
                                  {
                                    currentReaderCount++;
                                    return currentReaderCount < dataReaderData.Count;
                                  });
      dataReader[Arg.Any<string>()].Returns(info =>
                                              {
                                                var columnName     = info.Arg<string>();
                                                var currentDataRow = dataReaderData[currentReaderCount];
                                                if (currentDataRow == null) { return null; }

                                                if (currentDataRow.DoesPropertyExist(columnName))
                                                {
                                                  return currentDataRow.GetPropertyValue(columnName);
                                                }

                                                if (customColumnAttribute != null)
                                                {
                                                  foreach (var currentProperty in currentDataRow.GetType().GetProperties())
                                                  {
                                                    var columnAttribute = (dynamic)currentProperty.GetCustomAttribute(customColumnAttribute);
                                                    if (columnAttribute != null && columnAttribute.DbColumnName == columnName)
                                                    {
                                                      return currentProperty.GetValue(currentDataRow);
                                                    }
                                                  }
                                                }

                                                if (additionalColumnValues != null && additionalColumnValues.ContainsKey(columnName))
                                                {
                                                  return additionalColumnValues[columnName];
                                                }

                                                Assert.Fail($"Property / Parameter {info.Arg<string>()} not found on {currentDataRow.GetType().Name}");
                                                return null;
                                              });

      return dataReader;
    }
  }
}
