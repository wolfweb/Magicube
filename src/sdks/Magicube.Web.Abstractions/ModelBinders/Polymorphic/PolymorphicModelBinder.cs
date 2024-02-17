using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Magicube.Web.ModelBinders.Polymorphic {
    class PolymorphicModelBinder : IModelBinder {
        private readonly ICollection<PolymorphicBindableModelBinder> _binders;

        public PolymorphicModelBinder(ICollection<PolymorphicBindableModelBinder> binders) {
            _binders = binders;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext) {
            var entryFound = _binders
                .Any(binder => binder.IsBindable(bindingContext));

            if (!entryFound) {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            var bindableModelBinder = _binders
                .First(binder => binder.IsBindable(bindingContext));

            var newBindingContext = DefaultModelBindingContext.CreateBindingContext(
                bindingContext.ActionContext,
                bindingContext.ValueProvider,
                bindableModelBinder.ModelMetadata,
                bindingInfo: null,
                bindingContext.ModelName);

            await bindableModelBinder.ModelBinder.BindModelAsync(newBindingContext).ConfigureAwait(false);
            bindingContext.Result = newBindingContext.Result;

            if (newBindingContext.Result.IsModelSet) {
                bindingContext.ValidationState[newBindingContext.Result.Model!] = new ValidationStateEntry {
                    Metadata = bindableModelBinder.ModelMetadata,
                };
            }
        }
    }
}
