using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Reflection;

namespace Magicube.Identity.Web {
    public class IdentityPageModelConvention : IPageApplicationModelConvention {
        public void Apply(PageApplicationModel model) {
            var defaultUIAttribute = model.ModelType.GetCustomAttribute<IdentityDefaultUIAttribute>();
            if (defaultUIAttribute == null) {
                return;
            }

            var templateInstance = defaultUIAttribute.Template.MakeGenericType(typeof(User));
            model.ModelType = templateInstance.GetTypeInfo();
        }
    }
}
