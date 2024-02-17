using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Magicube.Core;
using System.Collections;

namespace Magicube.Web.ModelBinders {
    public class ArrayModelBinderProvider : IModelBinderProvider {
        public IModelBinder GetBinder(ModelBinderProviderContext context) {
            if (typeof(IEnumerable).IsAssignableFrom(context.Metadata.ModelType) && context.Metadata.ModelType != typeof(string)) {
                return new ArrayModelBinder();
            }

            return null;
        }
    }

    /// <summary>
    /// usage: ModelBinderAttribute(typeof(ArrayModelBinder))
    /// </summary>
    public class ArrayModelBinder : IModelBinder {
        readonly char[] ArraySplitChars = new char[] { ',', ' ', ';', '|' };
        public Task BindModelAsync(ModelBindingContext bindingContext) {
            if (!bindingContext.ModelType.IsArray) return Task.CompletedTask;

            var modelName = bindingContext.ModelName;
            var value = bindingContext.ValueProvider.GetValue(modelName);

            if (value == ValueProviderResult.None) return Task.CompletedTask;

            var valueString = value.FirstValue;

            if (valueString.IsNullOrEmpty()) return Task.CompletedTask;

            var mapperProvider = bindingContext.HttpContext.RequestServices.GetService<IMapperProvider>();

            var array = mapperProvider.Map(valueString.IsJson() ? Json.Parse<string[]>(valueString) : valueString.Split(',').ToArray(), typeof(string[]), bindingContext.ModelType);
            bindingContext.Result = ModelBindingResult.Success(array);

            return Task.CompletedTask;
        }
    }

}
