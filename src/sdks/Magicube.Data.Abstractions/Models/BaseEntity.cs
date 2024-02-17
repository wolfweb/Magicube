using Magicube.Data.Abstractions.Attributes;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace Magicube.Data.Abstractions {
    public interface IEntity {
        JObject Parts { get; set; }
    }
    public interface IEntity<T> : IEntity {
        T Id { get; set; }
    }

    public abstract class Entity<T> : IEntity<T> {
        public const string IdKey = nameof(Id);
        
        [NotMapped]
        [NoUIRender]
        public   JObject Parts { get; set; } = new JObject();

        public   T       Id    { get; set; }
    }

    public abstract class Entity : Entity<long> { }
}
