using Microsoft.EntityFrameworkCore;

namespace Magicube.Data.Abstractions.Mapping {
    public interface IMappingConfiguration {
        void ApplyConfiguration(ModelBuilder modelBuilder);
    }
}
