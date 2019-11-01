using System;

namespace Thuria.Calot.TestUtilities.Tests
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class FakeDbColumnAttribute : Attribute
  {
    public string DbColumnName { get; set; }
  }
}
