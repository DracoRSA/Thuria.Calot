using System;

using Moq;

namespace Thuria.Calot.TestUtilities
{
  /// <summary>
  /// Moq Extensions
  /// </summary>
  public static class MockExtensions
  {
    /// <summary>
    /// Create a Mock object from a type (For some reason not supported by Moq)
    /// </summary>
    /// <param name="objectTypeToMock"></param>
    /// <returns>Mocked object</returns>
    public static dynamic MockAs(this Type objectTypeToMock)
    {
      var genericType = typeof(Mock<>).MakeGenericType(objectTypeToMock);
      return Activator.CreateInstance(genericType);
    }
  }
}
