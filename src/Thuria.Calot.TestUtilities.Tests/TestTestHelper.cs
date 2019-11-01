using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;

using NUnit.Framework;
using FluentAssertions;

using Thuria.Zitidar.Extensions;

namespace Thuria.Calot.TestUtilities.Tests
{
  [TestFixture]
  public class TestTestHelper
  {
    [Test]
    public void CreateSubstituteDataReader_GivenData_ShouldReturnDataReader()
    {
      //---------------Set up test pack-------------------
      var allTestData = new List<FakeDataClass>
        {
          CreateRandomFakeDataClass(), CreateRandomFakeDataClass(), CreateRandomFakeDataClass(),
        };
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var dataReader = TestHelper.CreateSubstituteDataReader(allTestData);
      //---------------Test Result -----------------------
      dataReader.Should().NotBeNull();
      dataReader.Should().BeAssignableTo<IDataReader>();
    }

    [Test]
    public void CreateSubstituteDataReader_GivenDataOnly_ShouldReturnDataReaderWithExpectedData()
    {
      //---------------Set up test pack-------------------
      var dataReaderData = new List<dynamic>
        {
          new { Id = Guid.NewGuid(), Name = RandomValueGenerator.CreateRandomString(10, 50) },
          new { Id = Guid.NewGuid(), Name = RandomValueGenerator.CreateRandomString(10, 50) },
          new { Id = Guid.NewGuid(), Name = RandomValueGenerator.CreateRandomString(10, 50) }
        };
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var dataReader = TestHelper.CreateSubstituteDataReader(dataReaderData);
      //---------------Test Result -----------------------
      var allFakeData = new List<FakeDataClass>();
      while (dataReader.Read())
      {
        var dataClass = new FakeDataClass
          {
            Id   = dataReader.GetValue<Guid>(nameof(FakeDataClass.Id)),
            Name = dataReader.GetValue<string>(nameof(FakeDataClass.Name)),
          };
        allFakeData.Add(dataClass);
      }

      allFakeData.Count.Should().Be(3);

      foreach (var currentData in dataReaderData)
      {
        var foundDataClass = allFakeData.FirstOrDefault(dataClass => dataClass.Id.Equals(currentData.Id));
        foundDataClass.Should().NotBeNull();
        foundDataClass?.Name.Should().Be(currentData.Name);
      }
    }

    [Test]
    public void CreateSubstituteDataReader_GivenData_And_AdditionalColumnValues_ShouldReturnDataReaderWithExpectedData()
    {
      //---------------Set up test pack-------------------
      var dataReaderData = new List<dynamic>
        {
          new { Id = Guid.NewGuid(), Name = RandomValueGenerator.CreateRandomString(10, 50) },
          new { Id = Guid.NewGuid(), Name = RandomValueGenerator.CreateRandomString(10, 50) },
          new { Id = Guid.NewGuid(), Name = RandomValueGenerator.CreateRandomString(10, 50) }
        };
      var additionalColumnValues = new Dictionary<string, object>
        {
          { "Sequence", RandomValueGenerator.CreateRandomInt(100, 500) },
          { "Amount", RandomValueGenerator.CreateRandomDecimal(1000, 5000) },
        };
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var dataReader = TestHelper.CreateSubstituteDataReader(dataReaderData, additionalColumnValues);
      //---------------Test Result -----------------------
      var allFakeData = new List<FakeDataClass>();
      while (dataReader.Read())
      {
        var dataClass = new FakeDataClass
          {
            Id       = dataReader.GetValue<Guid>(nameof(FakeDataClass.Id)),
            Name     = dataReader.GetValue<string>(nameof(FakeDataClass.Name)),
            Sequence = dataReader.GetValue<int>(nameof(FakeDataClass.Sequence)),
            Amount   = dataReader.GetValue<decimal>(nameof(FakeDataClass.Amount)),
          };
        allFakeData.Add(dataClass);
      }

      allFakeData.Count.Should().Be(3);

      foreach (var currentData in dataReaderData)
      {
        var foundDataClass = allFakeData.FirstOrDefault(dataClass => dataClass.Id.Equals(currentData.Id));
        foundDataClass.Should().NotBeNull();
        foundDataClass?.Name.Should().Be(currentData.Name);
        foundDataClass?.Sequence.Should().Be((int) additionalColumnValues["Sequence"]);
        foundDataClass?.Amount.Should().Be((decimal) additionalColumnValues["Amount"]);
      }
    }

    [Test]
    public void CreateSubstituteDataReader_GivenData_And_CustomColumAttribute_ShouldReturnDataReaderWithExpectedData()
    {
      //---------------Set up test pack-------------------
      var dataReaderData = new List<FakeDataClass>
        {
          CreateRandomFakeDataClass(), CreateRandomFakeDataClass(), CreateRandomFakeDataClass()
        };
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var dataReader = TestHelper.CreateSubstituteDataReader(dataReaderData, customColumnAttribute: typeof(FakeDbColumnAttribute));
      //---------------Test Result -----------------------
      var allFakeData = new List<FakeDataClass>();
      while (dataReader.Read())
      {
        var dataClass = new FakeDataClass
          {
            Id          = dataReader.GetValue<Guid>(nameof(FakeDataClass.Id)),
            Name        = dataReader.GetValue<string>(nameof(FakeDataClass.Name)),
            DbAliasName = dataReader.GetValue<string>("DbAlias")
          };
        allFakeData.Add(dataClass);
      }

      allFakeData.Count.Should().Be(3);

      foreach (var currentData in dataReaderData)
      {
        var foundDataClass = allFakeData.FirstOrDefault(dataClass => dataClass.Id.Equals(currentData.Id));
        foundDataClass.Should().NotBeNull();
        foundDataClass?.Name.Should().Be(currentData.Name);
        foundDataClass?.DbAliasName.Should().Be(currentData.DbAliasName);
      }
    }

    private FakeDataClass CreateRandomFakeDataClass()
    {
      return new FakeDataClass
        {
          Id          = Guid.NewGuid(),
          Name        = RandomValueGenerator.CreateRandomString(10, 20),
          DbAliasName = RandomValueGenerator.CreateRandomString(30, 60)
        };
    }

    private class FakeDataClass
    {
      public Guid Id { get; set; }
      public string Name { get; set; }
      public int Sequence { get; set; }
      public decimal Amount { get; set; }

      [FakeDbColumn(DbColumnName = "DbAlias")]
      public string DbAliasName { get; set; }
    }
  }
}
