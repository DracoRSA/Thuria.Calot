using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Thuria.Zitidar.Extensions;

namespace Thuria.Calot.TestUtilities
{
  /// <summary>
  /// Random Value Generator helper methods
  /// </summary>
  public class RandomValueGenerator
  {
    private const long MinimumNumber = 0;
    private const long MaximumNumber = 20;
    private const int MinimumLengthString = 8;
    private const string DefaultRandomStringChars = "abcdefghijklmnopqrstuvwxyz1234567890";
    private const int MinimumCollectionItems = 1;
    private const int MaximumCollectionItems = 10;

    private static readonly Random RandomValue = new Random();

    private static readonly Dictionary<Type, Func<object>> RandomValueGenerators = new Dictionary<Type, Func<object>>
      {
        { typeof(bool), () => CreateRandomBoolean() },
        { typeof(uint), () => CreateRandomUInt() },
        { typeof(int), () => CreateRandomInt() },
        { typeof(long), () => CreateRandomLong() },
        { typeof(double), () => CreateRandomDouble() },
        { typeof(string), () => CreateRandomString() },
        { typeof(byte), () => CreateRandomBytes(1)[0] },
        { typeof(byte[]), () => CreateRandomBytes(CreateRandomInt(1, 25)) },
        { typeof(Guid), () => Guid.NewGuid()  },
        { typeof(object), () => new object() },
        { typeof(DateTime), () => CreateRandomDate() }
      };

    /// <summary>
    /// Create Random Number
    /// </summary>
    /// <param name="minimumValue">Minimum Value</param>
    /// <param name="maximumValue">Maximum Number</param>
    /// <returns>Random long</returns>
    public static long CreateRandomNumber(long minimumValue = MinimumNumber, long maximumValue = MaximumNumber)
    {
      var randomNextValue = (long)RandomValue.NextDouble();
      var range           = maximumValue - minimumValue + 1;

      return minimumValue + (long)(range * randomNextValue);
    }

    /// <summary>
    /// Create a Random Value of Type 
    /// </summary>
    /// <param name="objectType"></param>
    /// <returns>Random value of Type</returns>
    public static object CreateRandomValue(Type objectType)
    {
      if (objectType == typeof(Type)) { return objectType; }

      var randomType = objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>)
                            ? Nullable.GetUnderlyingType(objectType)
                            : objectType;

      var valueGenerator = RandomValueGenerators.FirstOrDefault(pair => pair.Key == randomType);
      if (valueGenerator.Value != null)
      {
        return valueGenerator.Value();
      }

      if (randomType != null && randomType.IsEnum)
      {
        return CreateRandomEnum(randomType);
      }

      if (randomType != null && randomType.IsGenericType)
      {
        if (randomType.GetGenericTypeDefinition() == typeof(IEnumerable<>) || 
            randomType.GetGenericTypeDefinition() == typeof(IList<>) || 
            randomType.GetGenericTypeDefinition() == typeof(List<>))
        {
          return CreateRandomCollection(randomType.GenericTypeArguments[0]);
        }

        if (randomType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
          return CreateRandomDictionary();
        }

        throw new Exception($"Generic Type Generator for {randomType.Name} does not exist");
      }

      if (randomType == null || !(randomType.IsInterface || randomType.IsClass))
      {
        throw new Exception($"Random Value Generator for {objectType.Name} does not exist");
      }

      var mockedObject = randomType.CreateSubstitute();
      return mockedObject;
    }

    /// <summary>
    /// Create Random String
    /// </summary>
    /// <param name="minLength">Minimum length of the string</param>
    /// <param name="maxLength">Maximum length of the string</param>
    /// <param name="charSet">Character Set to be used in the string (Optional)</param>
    /// <returns>Random string</returns>
    public static string CreateRandomString(int minLength = MinimumLengthString, int? maxLength = null, string charSet = null)
    {
      var actualMaxLength = maxLength ?? minLength + MinimumLengthString;
      var actualLength = CreateRandomInt(minLength, actualMaxLength);

      var chars = new List<char>();
      if (charSet == null)
      {
        charSet = DefaultRandomStringChars;
      }

      var charSetLength = charSet.Length;

      for (var i = 0; i < actualLength; i++)
      {
        var characterPosition = CreateRandomInt(0, charSetLength - 1);
        chars.Add(charSet[characterPosition]);
      }

      return string.Join(string.Empty, chars.Select(c => c.ToString()).ToArray());
    }

    /// <summary>
    /// Create Random boolean value
    /// </summary>
    /// <returns>Random boolean value</returns>
    public static bool CreateRandomBoolean()
    {
      return CreateRandomInt(1, 100) < 50;
    }

    /// <summary>
    /// Create Random uint
    /// </summary>
    /// <param name="minimumValue">Minimum Value (Optional)</param>
    /// <param name="maximumValue">Maximum Value (Optional)</param>
    /// <returns>Random uint value</returns>
    public static uint CreateRandomUInt(uint minimumValue = uint.MinValue, uint maximumValue = uint.MaxValue)
    {
      return (uint)CreateRandomLong(minimumValue, maximumValue);
    }

    /// <summary>
    /// Create Random Int
    /// </summary>
    /// <param name="minimumValue">Minimum Value (Optional)</param>
    /// <param name="maximumValue">Maximum Value (Optional)</param>
    /// <returns>Random int value</returns>
    private static int CreateRandomInt(int minimumValue = int.MinValue, int maximumValue = int.MaxValue)
    {
      return (int)CreateRandomLong(minimumValue, maximumValue);
    }

    /// <summary>
    /// Create Random Long
    /// </summary>
    /// <param name="minimumValue">Minimum Value (Optional)</param>
    /// <param name="maximumValue">Maximum Value (Optional)</param>
    /// <returns>Random long value</returns>
    public static long CreateRandomLong(long minimumValue = long.MinValue, long maximumValue = long.MaxValue)
    {
      var randomNumber = CreateRandomNumber(minimumValue, maximumValue);
      var range        = maximumValue - minimumValue + 1;

      return minimumValue + (range * randomNumber);
    }

    /// <summary>
    /// Create Random Double
    /// </summary>
    /// <param name="minimumValue">Minimum Value (Optional)</param>
    /// <param name="maximumValue">Maximum Value (Optional)</param>
    /// <returns>Random double value</returns>
    public static double CreateRandomDouble(long minimumValue = long.MinValue, long maximumValue = long.MaxValue)
    {
      double randomNumber = CreateRandomNumber(minimumValue, maximumValue);
      double range        = maximumValue - minimumValue + 1;

      return (double)(minimumValue + (range * randomNumber));
    }

    /// <summary>
    /// Create Random Bytes
    /// </summary>
    /// <param name="noOfBytes">No of Bytes to Generate</param>
    /// <returns>An array with the generated byte(s)</returns>
    public static byte[] CreateRandomBytes(int noOfBytes)
    {
      var actualLength  = CreateRandomInt(1, noOfBytes);
      var chars         = new List<char>();
      var charSet       = DefaultRandomStringChars;
      var charSetLength = charSet.Length;

      for (var i = 0; i < actualLength; i++)
      {
        var characterPosition = CreateRandomInt(0, charSetLength - 1);
        chars.Add(charSet[characterPosition]);
      }

      return Encoding.ASCII.GetBytes(chars.ToArray());
    }

    /// <summary>
    /// Create Random Enum of Type
    /// </summary>
    /// <param name="enumType">Enum Type</param>
    /// <returns>Random enum value</returns>
    public static object CreateRandomEnum(Type enumType)
    {
      if (!enumType.IsEnum)
      {
        throw new ArgumentException($"GetRandomEnum cannot be called on something other than an enum [{enumType.Name}]");
      }

      var possible = Enum.GetValues(enumType).Cast<object>();
      return CreateRandomFrom(possible);
    }

    /// <summary>
    /// Return Random item from supplied collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <returns></returns>
    private static T CreateRandomFrom<T>(IEnumerable<T> items)
    {
      var itemArray = items as T[] ?? items.ToArray();
      var maxValue  = itemArray.Length - 1;
      return itemArray.Skip(CreateRandomInt(0, maxValue)).First();
    }

    /// <summary>
    /// Create Collection of Random values
    /// </summary>
    /// <param name="genericCollectionType">Collection Type</param>
    /// <param name="minItems">Minimum number of items (Optional)</param>
    /// <param name="maxItems">Maximum number of items (Optional)</param>
    /// <returns>A Collection of Random values of the specified type</returns>
    public static object CreateRandomCollection(Type genericCollectionType, int minItems = MinimumCollectionItems, int maxItems = MaximumCollectionItems)
    {
      var listType         = typeof(List<>).MakeGenericType(new[] { genericCollectionType });
      var randomCollection = (IList)Activator.CreateInstance(listType);
      var howMany          = CreateRandomInt(minItems, maxItems);

      for (var loopCount = 0; loopCount < howMany; loopCount++)
      {
        randomCollection.Add(CreateRandomValue(genericCollectionType));
      }

      return randomCollection;
    }

    /// <summary>
    /// Create Random Dictionary
    /// </summary>
    /// <returns>A Random Dictionary</returns>
    public static IDictionary<dynamic, dynamic> CreateRandomDictionary()
    {
      return null;
    }

    /// <summary>
    /// Create Random Date
    /// </summary>
    /// <param name="minDate">Minimum Date (Optional)</param>
    /// <param name="maxDate">Maximum Date (Optional)</param>
    /// <param name="dateOnly">Date Only and not Time</param>
    /// <param name="minTime">Minimum Time (Optional)</param>
    /// <param name="maxTime">Maximum Time (Optional)</param>
    /// <returns>Random created date</returns>
    public static DateTime CreateRandomDate(DateTime? minDate = null, DateTime? maxDate = null, bool dateOnly = false,
                                            DateTime? minTime = null, DateTime? maxTime = null)
    {
      return CreateRandomDate(DateTimeKind.Local, minDate, maxDate, dateOnly, minTime, maxTime);
    }

    /// <summary>
    /// Create Random Date
    /// </summary>
    /// <param name="dateKind">Kind of Date</param>
    /// <param name="minDate">Minimum Date (Optional)</param>
    /// <param name="maxDate">Maximum Date (Optional)</param>
    /// <param name="dateOnly">Date Only and not Time</param>
    /// <param name="minTime">Minimum Time (Optional)</param>
    /// <param name="maxTime">Maximum Time (Optional)</param>
    /// <returns>Random created date</returns>
    public static DateTime CreateRandomDate(DateTimeKind dateKind, DateTime? minDate = null, DateTime? maxDate = null, bool dateOnly = false,
                                            DateTime? minTime = null, DateTime? maxTime = null)
    {
      if (dateOnly)
      {
        minDate = minDate?.StartOfDay().AddDays(1);
        maxDate = maxDate?.StartOfDay();
      }

      var minTicks = (minDate ?? new DateTime(1990, 1, 1)).Ticks;
      var maxTicks = (maxDate ?? new DateTime(2020, 12, 31)).Ticks;
      var range    = maxTicks - minTicks;

      var actualTicks = minTicks + (range + RandomValue.NextDouble());
      var rawDateTime = new DateTime((long) actualTicks);
      var sanitised   = new DateTime(rawDateTime.Year, rawDateTime.Month, rawDateTime.Day,
                                     rawDateTime.Hour, rawDateTime.Minute, rawDateTime.Second, rawDateTime.Millisecond,
                                     dateKind);

      return RangeCheckTimeOnRandomDate(minTime, maxTime, dateOnly ? sanitised.StartOfDay() : sanitised);
    }

    private static DateTime RangeCheckTimeOnRandomDate(DateTime? minTime, DateTime? maxTime, DateTime value)
    {
      minTime = new DateTime(value.Year, value.Month, value.Day, minTime?.Hour ?? 0, minTime?.Minute ?? 0, minTime?.Second ?? 0);
      maxTime = new DateTime(value.Year, value.Month, value.Day, maxTime?.Hour ?? 23, maxTime?.Minute ?? 59, maxTime?.Second ?? 59);

      if (minTime > maxTime)
      {
        var swap = minTime;
        minTime  = maxTime;
        maxTime  = swap;
      }

      return value > maxTime || value < minTime
                    ? RandomValueGenerator.CreateRandomDate(minTime, maxTime)
                    : value;
    }
  }
}
