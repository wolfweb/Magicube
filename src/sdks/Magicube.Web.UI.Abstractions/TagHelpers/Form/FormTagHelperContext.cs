using Magicube.Core.Reflection;
using Magicube.Data.Abstractions.ViewModel;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Linq;

namespace Magicube.Web.UI.TagHelpers {
    public class FormTagHelperContext : MagicubeTagHelperContext {
        public FormTagHelperContext(ModelExpression model) {
            Model     = model.Model;
            ModelType = model.ModelExplorer.ModelType;
            if (typeof(IEntityViewModel).IsAssignableFrom(ModelType)) {
                if (Model == null) {
                    var viewModel = (IEntityViewModel)TypeAccessor.Get(ModelType, null).Context.Constructors.First().Creator.DynamicInvoke(new object[] { null });
                    Properties = viewModel.ExportProperties;
                } else {
                    Properties = ((IEntityViewModel)Model).ExportProperties;
                }
            } else {
                Properties = TypeAccessor.Get(ModelType, null).Context.Properties.Select(x => new PropertyComponentContext(Model, x.Member)).ToArray();
            }
        }
    }
}
