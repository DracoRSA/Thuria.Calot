using System;

namespace Thuria.Calot.TestUtilities.Tests
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
  public class FakeTestAttribute : Attribute
  {
  }
}
