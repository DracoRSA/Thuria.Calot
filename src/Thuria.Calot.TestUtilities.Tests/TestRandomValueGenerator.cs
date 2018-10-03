using System;

using NUnit.Framework;
using FluentAssertions;

namespace Thuria.Calot.TestUtilities.Tests
{
  [TestFixture]
  public class TestRandomValueGenerator
  {
    [TestCase(1, 50)]
    [TestCase(781209, 987654)]
    public void CreateRandomNumber_GivenLongAndMinimumAndMaximumValues_ShouldGenerateValueBetweenGivenValues(long minimumValue, long maximumValue)
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
  }
}
