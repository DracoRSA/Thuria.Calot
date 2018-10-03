using System;
using System.Collections.Generic;

namespace Thuria.Calot.TestUtilities.Tests
{
  public enum FakeTestEnum
  {
    Option1,
    Option2,
    Option3
  }

  public class FakeTestClass
  {
    public FakeTestClass(Guid fakeId, string testName, int testIntValue, bool isActive, 
                         // ReSharper disable once UnusedParameter.Local
                         string notSetParameter,
                         DateTime testDateTime,
                         FakeComplex complexObjectNotTested, FakeComplex complexObject, IFakeComplex complexInterface, 
                         FakeTestEnum testEnum,
                         IEnumerable<IFakeComplex> allFakes = null, Dictionary<string, string> testDictionary = null)
    {
      Id             = fakeId;
      Name           = testName ?? throw new ArgumentNullException(nameof(testName));
      IntValue       = testIntValue;
      IsActive       = isActive;
      TestDateTime   = testDateTime;
      ComplexObject1 = complexObjectNotTested;
      ComplexObject2 = complexObject ?? throw new ArgumentNullException(nameof(complexObject));
      ComplexObject3 = complexInterface ?? throw new ArgumentNullException(nameof(complexInterface));
      TestEnum       = testEnum;
      FakeList       = allFakes;
      TestDictionary = testDictionary;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public int IntValue { get; private set; }
    public bool IsActive { get; private set; }

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    public string NotSetProperty { get; }

    public DateTime TestDateTime { get; private set; }

    public FakeComplex ComplexObject1 { get; private set; }
    public FakeComplex ComplexObject2 { get; private set; }
    public IFakeComplex ComplexObject3 { get; private set; }
    public FakeTestEnum TestEnum { get; private set; }
    public IEnumerable<IFakeComplex> FakeList { get; private set; }
    public Dictionary<string, string> TestDictionary { get; private set; }
  }
}
