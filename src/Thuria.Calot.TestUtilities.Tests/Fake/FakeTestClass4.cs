using System;

namespace Thuria.Calot.TestUtilities.Tests.Fake
{
  public class FakeTestClass4
  {

    public FakeTestClass4(int testPositiveInt)
    {
      if (testPositiveInt <= 0)
      {
        throw new ArgumentException($"{nameof(testPositiveInt)} cannot be zero or negative", nameof(testPositiveInt));
      }

      TestPositiveInt = testPositiveInt;
    }

    public int TestPositiveInt { get; }
  }
}
