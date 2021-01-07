namespace Thuria.Calot.TestUtilities.Tests.Fake
{
  public class FakeTestClass2
  {
    public FakeTestClass2(int someTestValue)
    {
      SomeTestValue = someTestValue;
    }

    public FakeTestClass2(int someTestValue, FakeComplex[] allFakes)
    {
      SomeTestValue = someTestValue;
      AllFakes = allFakes;
    }

    public int SomeTestValue { get; }
    public FakeComplex[] AllFakes { get; set; }
  }
}
