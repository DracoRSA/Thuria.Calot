using System;
using System.Collections.Generic;

using NUnit.Framework;
using FluentAssertions;

namespace Thuria.Calot.TestUtilities.Tests
{
  [TestFixture]
  public class TestRandomValueGenerator
  {
    [TestCase(1, 10)]
    [TestCase(20, 55)]
    [TestCase(255, 1024)]
    public void CreateRandomNumber_GivenMinimumAndMaximumValues_ShouldGenerateValueBetweenGivenValues(int minimumValue, int maximumValue)
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var randomNumber = RandomValueGenerator.CreateRandomNumber(minimumValue, maximumValue);
      //---------------Test Result -----------------------
      randomNumber.Should().BeGreaterOrEqualTo(minimumValue);
      randomNumber.Should().BeLessOrEqualTo(maximumValue);
    }

    [TestCase(1, 10)]
    [TestCase(20, 55)]
    [TestCase(255, 1024)]
    public void CreateRandomInt_GivenMinimumAndMaximumValues_ShouldGenerateValueBetweenGivenValues(int minimumValue, int maximumValue)
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var randomNumber = RandomValueGenerator.CreateRandomInt(minimumValue, maximumValue);
      //---------------Test Result -----------------------
      randomNumber.Should().BeGreaterOrEqualTo(minimumValue);
      randomNumber.Should().BeLessOrEqualTo(maximumValue);
    }

    [TestCase(1, 10)]
    [TestCase(20, 55)]
    [TestCase(255, 1024)]
    public void CreateRandomLong_GivenMinimumAndMaximumValues_ShouldGenerateValueBetweenGivenValues(int minimumValue, int maximumValue)
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var randomNumber = RandomValueGenerator.CreateRandomLong(minimumValue, maximumValue);
      //---------------Test Result -----------------------
      randomNumber.Should().BeGreaterOrEqualTo(minimumValue);
      randomNumber.Should().BeLessOrEqualTo(maximumValue);
    }

    [TestCase(1, 10)]
    [TestCase(7, 51)]
    [TestCase(100, 1024)]
    public void CreateRandomString_GivenMinimumAndMaximumValues_ShouldGenerateStringWithLenBetweenGivenValues(int minimumLength, int maximumLength)
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var randomString = RandomValueGenerator.CreateRandomString(minimumLength, maximumLength);
      //---------------Test Result -----------------------
      randomString.Length.Should().BeGreaterOrEqualTo(minimumLength);
      randomString.Length.Should().BeLessOrEqualTo(maximumLength);
    }

    [TestCase(3)]
    [TestCase(10)]
    [TestCase(21)]
    public void CreateRandomString_GivenMultipleCalls_ShouldGenerateUniqueStrings(int numberOfStrings)
    {
      //---------------Set up test pack-------------------
      var generatedStrings = new List<string>();
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      for (int loopCount = 0; loopCount < numberOfStrings; loopCount++)
      {
        var randomString = RandomValueGenerator.CreateRandomString(1, 10);
        //---------------Test Result -----------------------
        if (generatedStrings.Contains(randomString))
        {
          Console.WriteLine($"Duplicate string found: {randomString}");
        }
        generatedStrings.Add(randomString);
      }
    }

    [TestCase(1, 50)]
    [TestCase(781209, 987654)]
    public void CreateRandomNumber_GivenLongAndMinimumAndMaximumValues_ShouldGenerateValueBetweenGivenValues(int minimumValue, int maximumValue)
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var randomNumber = RandomValueGenerator.CreateRandomNumber(minimumValue, maximumValue);
      //---------------Test Result -----------------------
      randomNumber.Should().BePositive();
      randomNumber.Should().BeGreaterOrEqualTo(minimumValue);
      randomNumber.Should().BeLessOrEqualTo(maximumValue);
    }

    [Test]
    public void CreateRandomNumber_GivenLargeLongAndMinimumAndMaximumValues_ShouldGenerateValueBetweenGivenValues()
    {
      //---------------Set up test pack-------------------
      var minimumValue = new DateTime(1990, 1, 1).Ticks;
      var maximumValue = new DateTime(2020, 12, 31).Ticks;
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var randomNumber = RandomValueGenerator.CreateRandomNumber(minimumValue, maximumValue);
      //---------------Test Result -----------------------
      randomNumber.Should().BePositive();
      randomNumber.Should().BeGreaterOrEqualTo(minimumValue);
      randomNumber.Should().BeLessOrEqualTo(maximumValue);
    }

    [Test]
    public void CreateRandomDate_GivenMultipleCalls_ShouldyCreateRandomDates()
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var randomDate1 = RandomValueGenerator.CreateRandomValue(typeof(DateTime));
      var randomDate2 = RandomValueGenerator.CreateRandomValue(typeof(DateTime));
      var randomDate3 = RandomValueGenerator.CreateRandomValue(typeof(DateTime));
      //---------------Test Result -----------------------
      randomDate1.Should().NotBeSameAs(randomDate2);
      randomDate1.Should().NotBeSameAs(randomDate3);
      randomDate2.Should().NotBeSameAs(randomDate3);
    }

    [TestCase(typeof(FakeException), typeof(FakeException))]
    [TestCase(typeof(Dictionary<string, object>), typeof(Dictionary<string, object>))]
    [TestCase(typeof(IDictionary<string, object>), typeof(Dictionary<string, object>))]
    public void CreateRandomValue_GivenType_ShouldNotThrowExceptionAndCreateRandomValue(Type objectType, Type expectedType)
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      Assert.DoesNotThrow(() =>
        {
          var randomValue = RandomValueGenerator.CreateRandomValue(objectType);
          //---------------Test Result -----------------------
          randomValue.Should().NotBeNull();
          randomValue.Should().BeOfType(expectedType);
        });
    }

    [TestCase(typeof(bool?), typeof(bool))]
    [TestCase(typeof(int?), typeof(int))]
    [TestCase(typeof(decimal?), typeof(decimal))]
    [TestCase(typeof(DateTime?), typeof(DateTime))]
    public void CreateRandomNullableValue_GivenType_ShouldNotThrowExceptionAndCreateRandomValue(Type objectType, Type expectedType)
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      Assert.DoesNotThrow(() =>
        {
          var randomValue = RandomValueGenerator.CreateRandomValue(objectType);
          //---------------Test Result -----------------------
          randomValue.Should().NotBeNull();
          randomValue.Should().BeOfType(expectedType);
        });
    }
  }
}
