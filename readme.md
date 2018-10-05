Thuria - Calot
===

Overview
---

This package consists of a set of helper methods to help with some of the repetititive tests that is done 
when we are unit testing our applications.



Features
---
* **ConstructorTestHelper** - A set of helper methods to help test the Construction of an object
* **PropertyTestHelper** - A set of helper methods to help test the Properties of an object
* **MethodTestHelper** - A set of helper methods to help test the Methods of an object  
* **RandomValueGenerator** - Generate Random Values based on the object type

---
ConstructorTestHelper
---

The Constructor Test Helper consists of the following helper methods:

* ConstructObject\<T>(parameterName, parameterValue)
* ConstructObject(objectType, parameterName, parameterValue)
* ValidateArgumentNullExceptionIfParameterIsNull\<T>(parameterName)
* ValidatePropertySetWithParameter\<T>(parameterName, propertyName)

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

---
PropertyTestHelper
---

The Property Test Helper consists of the following helper methods:

* ValidateGetAndSet\<T>(propertyName)
* ValidateGetAndSet(object6Type, propertyName)

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

---
MethodTestHelper
---

The Method Test Helper consist of the following helper methods:

* ValidateArgumentNullExceptionIfParameterIsNull\<T>(methodName, parameterName, parameterValue)

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

---
RandomValueGenerator
---

The Random Value Generator can generate random values for the following data types:

* bool
* uint _(Can specify range - min & max)_
* int _(Can specify range - min & max)_
* long _(Can specify range - min & max)_
* double _(Can specify range - min & max)_
* string _(Can specify length range - min & max)_
* Guid
* object
* DateTime _(Can specify range - min & max)_

Other functionality:
 
* **CreateRandomFrom\<T>** - Select random value from a collection of values
* **CreateRandomCollection** - Create a collection of random values
