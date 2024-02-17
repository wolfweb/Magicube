using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Magicube.Data.Abstractions.EfDbContext {
    public interface IEntityBuilder {
        IEnumerable<Type> Entities { get; }
        void Build(ModelBuilder modelBuilder);
    }
}
