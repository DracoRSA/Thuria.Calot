using System;

namespace Thuria.Calot.TestUtilities.Tests
{
  public interface IFakeComplex
  {
    Guid Id { get; set; }
    string Name { get; set; }
  }

  public class FakeComplex : IFakeComplex
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
  }
}
