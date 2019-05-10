using System;

namespace Thuria.Calot.TestUtilities.Tests
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
  public class FakeTestAttribute : Attribute
  {
    public FakeTestAttribute(string propertyName)
    {
      PropertyName = propertyName;
    }

    public string PropertyName { get; private set; }
    public int Sequence { get; set; }
  }
}
