using Magicube.Core.Reflection;
using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.ViewModel;
using Magicube.Web.Sites;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Magicube.Web.ModelBinders {
    public class ViewModelBinderProvider : IModelBinderProvider {
        public IModelBinder GetBinder(ModelBinderProviderContext context) {
            var site = context.Services.GetService<IOptionsMonitor<DefaultSite>>().CurrentValue;
            if (site?.Status == SiteStatus.Running) {
                if (typeof(IEntityViewModel).IsAssignableFrom(context.Metadata.ModelType)) {
                    return new ViewModelBinder();
                }
            }

            return null;
        }
    }

    public class ViewModelBinder : IModelBinder {
        public Task BindModelAsync(ModelBindingContext bindingContext) {
            if (bindingContext == null) {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var model = (IEntityViewModel)TypeAccessor.Get(bindingContext.ModelType, null).Context.Constructors.First().Creator.DynamicInvoke(new object[] { null });
            bindingContext.Model = model;
            foreach (var property in model.ExportProperties) {
                if (!bindingContext.HttpContext.Request.Form.ContainsKey(property.Property.Name)) continue;
                try {
                    property.Property.Value(model, bindingContext.HttpContext.Request.Form[property.Property.Name].ToString());
                } catch (ValidationException ve) {
                    bindingContext.ModelState.AddModelError(property.Property.Name, ve.Message);
                    break;
                } catch (Exception ex) {
                    bindingContext.ModelState.AddModelError(property.Property.Name, ex.Message);
                    break;
                }
            }

            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);

            return Task.CompletedTask;
        }
    }
}
