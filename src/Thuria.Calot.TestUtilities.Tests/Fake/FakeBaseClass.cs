using System;

namespace Thuria.Calot.TestUtilities.Tests.Fake
{
  public class FakeBaseClass
  {
    private readonly Guid _guidValue;

    public FakeBaseClass(Guid guidValue, FakeComplex fakeComplex)
    {
      _guidValue = guidValue;
      FakeComplex = fakeComplex ?? throw new ArgumentNullException(nameof(fakeComplex));
    }

    public FakeComplex FakeComplex { get; set; }
  }
}
