using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Magicube.Data.Abstractions.EfDbContext {
    public interface IEntityBuilder {
        void Build(ModelBuilder modelBuilder);
    }
}
