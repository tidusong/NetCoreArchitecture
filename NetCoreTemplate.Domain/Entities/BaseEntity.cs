using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreTemplate.Domain.Entities {
  public abstract class BaseEntity {
    public int Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime ModifiedAtUtc { get; set; }
  }
}
