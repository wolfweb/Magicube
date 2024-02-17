using Magicube.Core.Reflection;
using System.Collections.Generic;
using System.Reflection;

namespace Magicube.Data.Abstractions.ViewModel {
    public sealed class ViewModelPropertyComponent {
        public object               ViewModel { get; set; }
        public PropertyInfoExplorer Property  { get; set; }
    }

    public sealed class PropertyComponentContext {
        public PropertyComponentContext(object entity, PropertyInfo property) {
            Entity   = entity;
            Property = property;
        }
        public object                     Entity   { get; }
        public PropertyInfo               Property { get; }
        
        public int                        Order    { get; set; }
        public ViewModelPropertyComponent Override { get; set; }
    }

    public interface IEntityContext<TEntity> where TEntity : IEntity {
        bool TrySetValue(string name, object value);
        bool TryGetValue(string name, out object result);
        
        IList<PropertyComponentContext> Components { get; }

        TEntity Build();
    }
}
