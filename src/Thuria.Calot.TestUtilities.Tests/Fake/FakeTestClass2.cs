using System;

namespace Thuria.Calot.TestUtilities.Tests.Fake
{
  public class FakeTestClass2 : FakeBaseClass
  {
    private readonly FakeComplex _fakeComplex2;

    public FakeTestClass2(Guid guidValue, FakeComplex fakeComplex, FakeComplex fakeComplex2) 
      : base(guidValue, fakeComplex)
    {
      _fakeComplex2 = fakeComplex2 ?? throw new ArgumentNullException(nameof(fakeComplex2));
    }
  }
}
