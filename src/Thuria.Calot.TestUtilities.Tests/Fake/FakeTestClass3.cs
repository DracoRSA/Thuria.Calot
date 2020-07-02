using System;

namespace Thuria.Calot.TestUtilities.Tests.Fake
{
  public class FakeTestClass3
  {
    public FakeTestClass3(int testId, FakeComplex fakeComplex)
    {
      Id = testId;
      FakeComplex = fakeComplex ?? throw new ArgumentNullException(nameof(fakeComplex));
    }

    public FakeTestClass3(int id, FakeTestClass fakeClass)
    {
      Id = id;
      FakeClass = fakeClass ?? throw new ArgumentNullException(nameof(fakeClass)); 
    }

    public int Id { get; }
    public FakeComplex FakeComplex { get; }
    public FakeTestClass FakeClass { get; }
  }
}
