using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace Magicube.Web.ModelBinders.Polymorphic {
    internal interface IPolymorphicBindableCollection {
        bool IsMatch(ModelBinderProviderContext context);
        ICollection<IPolymorphicBindable> GetTypes();
    }

    class PolymorphicBindableCollection<TAbstract> : IPolymorphicBindableCollection {
        private readonly ICollection<IPolymorphicBindable> _typeEntries;

        public PolymorphicBindableCollection() {
            _typeEntries = new List<IPolymorphicBindable>();
        }

        public bool IsMatch(ModelBinderProviderContext context) {
            return context.Metadata.ModelType == typeof(TAbstract);
        }

        public void AddEntryType(IPolymorphicBindable bindable) => _typeEntries.Add(bindable);

        public ICollection<IPolymorphicBindable> GetTypes() => _typeEntries.ToList();
    }
}
