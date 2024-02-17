using Magicube.Core;
using Magicube.Core.Reflection;
using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.ViewModel;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Magicube.Web.UI.TagHelpers {
    public class TableTagHelperContext : MagicubeTagHelperContext {
        public TableTagHelperContext(ModelExpression model) {
            if (typeof(PageResult<,>).IsAssignableFrom(model.ModelExplorer.ModelType.GetGenericTypeDefinition())) {
                Model     = model.Model.GetValue("Items");
                ModelType = model.ModelExplorer.ModelType.GetGenericArguments().First();
            } else if (typeof(IEnumerable).IsAssignableFrom(model.ModelExplorer.ModelType)) {
                Model     = model;
                ModelType = model.ModelExplorer.ModelType.GetGenericEnumerableType().GenericTypeArguments.First();
            }

            if (typeof(IEntityViewModel).IsAssignableFrom(ModelType)) {
                var viewModel = (IEntityViewModel)TypeAccessor.Get(ModelType, null).Context.Constructors.First().Creator.DynamicInvoke(new object[] { null });
                Properties = viewModel.ExportProperties;
            } else {
                Properties = TypeAccessor.Get(ModelType, null).Context.Properties.Select(x => new PropertyComponentContext(Model, x.Member)).ToArray();
            }
        }
        public string         Id                 { get; set; }        
        public string         Class              { get; set; }
        public string         Style              { get; set; }        
        public string         AspAction          { get; set; }
        public string         AspController      { get; set; }
        public string         ReturnUrl          { get; set; }

        public string         ButtonHeaderTitle  { get; set; }
        public string         ButtonHeaderClass  { get; set; }
        public string         ButtonHeaderStyle  { get; set; }
        public string         ButtonColumnClass  { get; set; }
        public string         ButtonColumnStyle  { get; set; }

        public ICollection<TableColumnTagHelper> TableColumns       { get; set; } = new List<TableColumnTagHelper>();
        public ICollection<TableButtonTagHelper> TableButtons       { get; set; } = new List<TableButtonTagHelper>();
        public RowsSettingsTagHelper             RowsSettings       { get; set; } = new RowsSettingsTagHelper();
        public PaginationSettingsTagHelper       PaginationSettings { get; set; } = new PaginationSettingsTagHelper { AllowPagination = false };
        public RenderContainerTagHelper          ContainerSettings  { get; set; } = new RenderContainerTagHelper();
    }
}