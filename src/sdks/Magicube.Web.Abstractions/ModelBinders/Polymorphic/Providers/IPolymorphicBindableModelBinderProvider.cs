using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicube.Web.ModelBinders.Polymorphic {
    interface IPolymorphicBindableModelBinderProvider {
        ICollection<PolymorphicBindableModelBinder> Provide(ModelBinderProviderContext context, IPolymorphicBindableCollection collection);
    }

    class PolymorphicBindableModelBinderProvider : IPolymorphicBindableModelBinderProvider {
        public ICollection<PolymorphicBindableModelBinder> Provide(ModelBinderProviderContext context, IPolymorphicBindableCollection collection) {
            var binders = new List<PolymorphicBindableModelBinder>();

            foreach (var entryType in collection.GetTypes()) {
                var modelMetadata = context.MetadataProvider.GetMetadataForType(entryType.BindToType);
                var binder = context.CreateBinder(modelMetadata);
                binders.Add(new PolymorphicBindableModelBinder(entryType, modelMetadata, binder));
            }

            return binders;
        }
    }
}
