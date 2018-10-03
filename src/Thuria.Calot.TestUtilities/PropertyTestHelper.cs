using System;

using NUnit.Framework;
using FluentAssertions;

using Thuria.Zitidar.Extensions;

namespace Thuria.Calot.TestUtilities
{
  /// <summary>
  /// Property Test Helper class
  /// </summary>
  public static class PropertyTestHelper
  {
    /// <summary>
    /// Validate when setting a property, the value is actually available via get
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyName"></param>
    public static void ValidateGetAndSet<T>(string propertyName)
      where T : class
    {
      var objectUnderTest = ConstructorTestHelper.ConstructObject<T>();
      if (objectUnderTest == null)
      {
        Assert.Fail($"Failed to create {typeof(T).FullName} to test Property Get and Set for {propertyName}");
      }

      objectUnderTest.ValidateGetAndSet(propertyName);
    }

    /// <summary>
    /// Validate when setting a property, the value is actually available via get
    /// </summary>
    /// <param name="objectUnderTest"></param>
    /// <param name="propertyName"></param>
    public static void ValidateGetAndSet(this object objectUnderTest, string propertyName)
    {
      if (propertyName == null) { throw new ArgumentNullException(nameof(propertyName)); }
      if (!objectUnderTest.DoesPropertyExist(propertyName))
      {
        throw new InvalidOperationException($"Property [{propertyName}] does not exists on {objectUnderTest.GetType().FullName}");
      }

      var propertyInfo = objectUnderTest.GetType().GetProperty(propertyName);
      var propertyValue = propertyInfo.CreateRandomValue();

      objectUnderTest.SetPropertyValue(propertyName, propertyValue);
      var returnedValue = objectUnderTest.GetPropertyValue(propertyName);

      returnedValue.Should().BeEquivalentTo(propertyValue);
    }
  }
}
