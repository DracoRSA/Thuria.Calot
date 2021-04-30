[![Build Status](https://dracorsa.visualstudio.com/Thuria%20-%20Calot/_apis/build/status/DracoRSA.Thuria.Calot?branchName=master)](https://dracorsa.visualstudio.com/Thuria%20-%20Calot/_build/latest?definitionId=1&branchName=master)

Thuria - Calot
===

Overview
---

A set of packages with a set of helper methods to assist with some of the repetitive tests that is done when we are unit testing our applications.


Features
---
* **ConstructorTestHelper** - A set of helper methods to help test the Construction of an object
* **PropertyTestHelper** - A set of helper methods to help test the Properties of an object
* **MethodTestHelper** - A set of helper methods to help test the Methods of an object  
* **RandomValueGenerator** - Generate Random Values based on the object type
* **TestHelper** - Generic helpers to facilitate some general functionality

---
ConstructorTestHelper
---

The Constructor Test Helper consists of the following helper methods:

  >
    ConstructObject\<T>(parameterName, parameterValue, 
                        bool allParametersMatch = false,
                        params (string parameterName, object parameterValue)[] constructorParams)

    ConstructObject(objectType, parameterName, parameterValue, 
                    bool allParametersMatch = false,
                    params (string parameterName, object parameterValue)[] constructorParams)

    ValidateExceptionIsThrownIfParameterIsNull\<T, TException>(parameterName, 
                                                                params (string parameterName, object parameterValue)[] constructorParams)

    ValidateArgumentNullExceptionIfParameterIsNull\<T>(parameterName,
                                                        bool allParametersMatch = false,
                                                        params (string parameterName, object parameterValue)[] constructorParams)

    ValidatePropertySetWithParameter\<T>(parameterName, propertyName,
                                          bool allParametersMatch = false,
                                          params (string parameterName, object parameterValue)[] constructorParams) 

Examples:

>
    [TestCase("columnName")]
    [TestCase("parameterName")]
    public void Constructor_GivenNullParameter_ShouldThrowException(string parameterName)
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      ConstructorTestHelper.ValidateArgumentNullExceptionIfParameterIsNull<ColumnModel>(parameterName);
      //---------------Test Result -----------------------
    }

    [TestCase("columnName")]
    [TestCase("parameterName")]
    public void Constructor_GivenNullParameter_ShouldThrowException(string parameterName)
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      ConstructorTestHelper.ValidateExceptionIsThrownIfParameterIsNull<ColumnModel, ArgumentNullException>(parameterName);
      //---------------Test Result -----------------------
    }
>
    [TestCase("connectionString", "ConnectionString")]
    [TestCase("dbConnection", "DbConnection")]
    public void Constructor_GivenParameter_ShouldSetExpectedProperty(string parameterName, string propertyName)
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      ConstructorTestHelper.ValidatePropertySetWithParameter<DatabaseContext>(parameterName, propertyName);
      //---------------Test Result -----------------------
    }

    [Test]
    public void ValidateDecoratedWithAttribute_ShouldSetAttributeWithExpectedValues()
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      Assert.DoesNotThrow(() => PropertyTestHelper.ValidateDecoratedWithAttribute<FakeTestClass>("TestDictionary",
                                                                                                 typeof(FakeTestAttribute),
                                                                                                 new List<(string propertyName, object propertyValue)>
                                                                                                   {
                                                                                                     ("Name", "TestDictionary"),
                                                                                                     ("Sequence", 23)
                                                                                                   }));
      //---------------Test Result -----------------------
    }
    
    [Test]
    public void ConstructObject_GivenParameterValues_ShouldConstructObjectWithParameterValues()
    {
      //---------------Set up test pack-------------------
      var testDateTime = DateTime.Now;
      var fakeComplex  = new FakeComplex();

      var parameterValues = new List<(string paramName, object paramValue)>
        {
          ("testDateTime", testDateTime), ("complexObject", fakeComplex)
        };
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var testClass = ConstructorTestHelper.ConstructObject<FakeTestClass>(constructorParams: parameterValues.ToArray());
      //---------------Test Result -----------------------
      testClass.TestDateTime.Should().BeSameDateAs(testDateTime);
      testClass.ComplexObject2.Should().Be(fakeComplex);
    }

    [Test]
    public void ValidateArgumentNullExceptionIfParameterIsNull_GivenParameterValuesAndExceptionNotThrown_ShouldFailTest()
    {
      //---------------Set up test pack-------------------
      var parameterName = "notSetParameter";
      var testDateTime  = DateTime.Now;
      var fakeComplex   = new FakeComplex();

      var parameterValues = new List<(string paramName, object paramValue)>
        {
          ("testDateTime", testDateTime), ("complexObject", fakeComplex)
        };
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var exception = Assert.Throws<AssertionException>(() => ConstructorTestHelper.ValidateArgumentNullExceptionIfParameterIsNull<FakeTestClass>(parameterName, 
                                                                                                                                                  parameterValues.ToArray()));
      //---------------Test Result -----------------------
      exception.Message.Should().Contain($"ArgumentNullException not throw for Constructor Parameter [{parameterName}] on {typeof(FakeTestClass).FullName}");
    }

    [Test]
    public void ValidateExceptionIsThrownIfParameterIsNull_GivenParameterValuesAndExceptionNotThrown_ShouldFailTest()
    {
      //---------------Set up test pack-------------------
      var parameterName = "notSetParameter";
      var testDateTime  = DateTime.Now;
      var fakeComplex   = new FakeComplex();

      var parameterValues = new List<(string paramName, object paramValue)>
        {
          ("testDateTime", testDateTime), ("complexObject", fakeComplex)
        };
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var exception = Assert.Throws<AssertionException>(() => ConstructorTestHelper.ValidateExceptionIsThrownIfParameterIsNull<FakeTestClass, ArgumentNullException>(parameterName, 
                                                                                                                                                                     parameterValues.ToArray()));
      //---------------Test Result -----------------------
      exception.Message.Should().Contain($"ArgumentNullException not throw for Constructor Parameter [{parameterName}] on {typeof(FakeTestClass).FullName}");
    }

    [TestCase("testName")]
    [TestCase("complexObject")]
    [TestCase("complexInterface")]
    public void ValidateArgumentNullExceptionIfParameterIsNull_GivenParameterWhereExceptionIsThrownAndParameterValues_ShouldPassTest(string parameterName)
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      Assert.DoesNotThrow(() => ConstructorTestHelper.ValidateArgumentNullExceptionIfParameterIsNull<FakeTestClass>(parameterName, ("testDateTime", DateTime.Now)));
      //---------------Test Result -----------------------
    }

---
PropertyTestHelper
---

The Property Test Helper consists of the following helper methods:

>
    ValidateGetAndSet\<T>(propertyName)

    ValidateGetAndSet(objectType, propertyName)

    ValidateDecoratedWithAttribute\<T>(propertyName, attributeType, \
                                      List<(string propertyName, object propertyValue)> attributePropertyValues = null)

Examples:

>    
    [TestCase("CommandTimeout")]
    public void Properties_GivenValue_ShouldSetPropertyValue(string propertyName)
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      PropertyTestHelper.ValidateGetAndSet<DatabaseContext>(propertyName);
      //---------------Test Result -----------------------
    }

>    
    [TestCase("TestProperty", typeof(SomeAttribute)]
    public void Properties_ShouldBeDecoratedWithAttribute(string propertyName, Type attributeType)
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      PropertyTestHelper.ValidateDecoratedWithAttribute<DatabaseContext>(propertyName, attributeType);
      //---------------Test Result -----------------------
    }

---
MethodTestHelper
---

The Method Test Helper consist of the following helper methods:

>
    ValidateArgumentNullExceptionIfParameterIsNull\<T>(methodName, parameterName, parameterValue)

    ValidateArgumentNullExceptionIfParameterIsNullAsync\<T>(methodName, parameterName, parameterValue)

    ValidateExceptionIsThrownIfParameterIsNull\<T, TException>(methodName, parameterName, parameterValue)

    ValidateExceptionIsThrownIfParameterIsNullAsync\<T, TException>(methodName, parameterName, parameterValue)

    ValidateDecoratedWithAttribute\<T>(propertyName, attributeType,
                                      List<(string propertyName, object propertyValue)> attributePropertyValues = null,
                                      bool supportMultipleMethods = false,
                                      List<string> matchingParameters = null) 

Example(s):

>    
    [TestCase("UnRegister", "connectionString")]
    [TestCase("Get", "connectionString")]
    public void Methods_GivenNullParameter_ShouldThrowException(string methodName, string parameterName)
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      MethodTestHelper.ValidateArgumentNullExceptionIfParameterIsNull<DatabaseContext>(methodName, parameterName);
      //---------------Test Result -----------------------
    }

    [TestCase("UnRegister", "connectionString")]
    [TestCase("Get", "connectionString")]
    public void Methods_GivenNullParameter_ShouldThrowException(string methodName, string parameterName)
    {
      //---------------Set up test pack-------------------
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      MethodTestHelper.ValidateExceptionIsThrownIfParameterIsNull<DatabaseContext, ArgumentNullException>(methodName, parameterName);
      //---------------Test Result -----------------------
    }
    [Test]
    public void Methods_MethodShouldBeDecoratedWithAttribute()
    {
      //---------------Set up test pack-------------------
      var matchingParameters = new List<string>
                   {
                     "someParameter"
                   };
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      MethodTestHelper.ValidateDecoratedWithAttribute<FakeTestClass2>("FakeTestMethod", typeof(FakeTestAttribute), 
                                                                      supportMulitpleMethods: true, 
                                                                      matchingParameters: matchingParameters);
      //---------------Test Result -----------------------
    }

---
RandomValueGenerator
---

The Random Value Generator can generate random values for the following data types:

* bool
* ushort _(Can specify range - min & max)_
* uint _(Can specify range - min & max)_
* ulong _(Can specify range - min & max)_
* ushort _(Can specify range - min & max)_
* short _(Can specify range - min & max)_
* int _(Can specify range - min & max)_
* long _(Can specify range - min & max)_
* double _(Can specify range - min & max)_
* string _(Can specify length range - min & max)_
* byte _(Can specify length range - min & max)_
* byte[] _(Can specify length range - min & max)_
* Guid
* object
* DateTime _(Can specify range - min & max)_

Other functionality:
 
* **CreateRandomFrom\<T>** - Select random value from a collection of values
* **CreateRandomCollection** - Create a collection of random values
* **CreateRandomArray** - Create an array of random values of the specified array type

**CreateRandomValue(Type objectType)**\
Create a random value of the specified type which will include all the types mentioned above\
CreateRandomValue also have support to create the following types:

* IEnumerable<>
* IList<>
* List<>
* IDictionary<,>
* Dictionary<,>

---
TestHelper
---

The Test Helper contains generic test helper methods that can be used in tests.

* **CreateParameterValues** - Generate random values for a list of parameters
* **CreateSubstituteDataReader<T>** - Create a Substitute Data Reader that will behave like a normal Data Reader

Example(s):

>    
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
    public void CreateSubstituteDataReader_GivenData_ShouldReturnDataReaderAbleToAutoMap()
    {
      //---------------Set up test pack-------------------
      var dataReaderData = new List<FakeDataClass>
        {
          CreateRandomFakeDataClass(), CreateRandomFakeDataClass(), CreateRandomFakeDataClass()
        };
      var dataReader = TestHelper.CreateSubstituteDataReader(dataReaderData, customColumnAttribute: typeof(FakeDbColumnAttribute));
      //---------------Assert Precondition----------------
      //---------------Execute Test ----------------------
      var allData = new List<FakeDataClass>();
      while (dataReader.Read())
      {
        var rowData = Enumerable.Range(0, dataReader.FieldCount)
                                .ToDictionary(i => dataReader.GetName(i), i => dataReader.GetValue(i));

        var fakeData = new FakeDataClass
                         {
                           Id          = (Guid) rowData["Id"],
                           Name        = (string) rowData["Name"],
                           DbAliasName = (string) rowData["DbAliasName"]
                         };
        allData.Add(fakeData);
      }
      //---------------Test Result -----------------------
      allData.Count.Should().Be(dataReaderData.Count);

      foreach (var currentReaderData in dataReaderData)
      {
        var foundData = allData.FirstOrDefault(data => data.Id == currentReaderData.Id);
        foundData.Should().NotBeNull();
        foundData.Should().BeEquivalentTo(currentReaderData);
      }
    }

---
