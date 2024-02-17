using Magicube.Data.Abstractions.ViewModel;
using System.Reflection;

namespace Magicube.Data.Abstractions {
    public static class ReflectionExtension {
        public static object Value(this PropertyInfo property, IEntityViewModel model)  {
            return model[property.Name];
        }

        public static object Value(this PropertyInfo property, IEntityViewModel model, object value) {
            return model[property.Name] = value;
        }
    }
}
