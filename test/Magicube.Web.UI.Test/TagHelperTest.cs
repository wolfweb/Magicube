using Magicube.TestBase;
using Magicube.Web.UI.TagHelpers;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.WebEncoders.Testing;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Magicube.Web.UI.Test {
    public class TagHelperTest {
        private readonly IServiceProvider _serviceProvider;

        public static TheoryData<TagHelperAttributeList> EntityFormAttributes {
            get {
                return new TheoryData<TagHelperAttributeList> {
                    new TagHelperAttributeList {
                        {
                            "asp-model", new FooViewModel( new FooEntity{ 
                                Id = 1,
                                Name = "wolfweb",
                                Born = DateTime.UtcNow,
                                Attribute = JObject.Parse("{\"Editable\":true}")
                            } )
                        }
                    }
                };
            }
        }

        public TagHelperTest() {
            var services = new ServiceCollection()
                .AddLogging();

            services.AddMvcCore().AddViews();
            var webServiceBuilder = new WebServiceBuilder(services);

            webServiceBuilder.AddWebUICore();


            _serviceProvider = services.BuildServiceProvider();
        }

        [Theory]
        [MemberData(nameof(EntityFormAttributes))]
        public async void Func_Model_To_Html_Test(TagHelperAttributeList attr) {
            var tagHelper = GetTagHelper(_serviceProvider.GetService<IHtmlGenerator>(), _serviceProvider.GetService<IModelMetadataProvider>());
            var ctx = new TagHelperContext(attr, new Dictionary<object, object>(), Guid.NewGuid().ToString("N"));

            var output = new TagHelperOutput("entity-form", new TagHelperAttributeList(), (useCachedResult, htmlEncoder) => {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(string.Empty);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

            await tagHelper.ProcessAsync(ctx, output);

            Assert.True(output.TagName == "form");
        }

        private EntityFormTagHelper GetTagHelper(IHtmlGenerator htmlGenerator, IModelMetadataProvider metadataProvider) {
            return GetTagHelper(
                htmlGenerator,
                container: new FooViewModel(),
                containerType: typeof(FooViewModel),
                "entity-form",
                metadataProvider: metadataProvider);
        }

        private EntityFormTagHelper GetTagHelper(
            IHtmlGenerator htmlGenerator, 
            object container, 
            Type containerType, 
            string expressionName, 
            IModelMetadataProvider metadataProvider 
            ) {
            var containerExplorer = metadataProvider.GetModelExplorerForType(containerType, container);
            var modelExpression = new ModelExpression(expressionName, containerExplorer);
            var viewContext     = TestableHtmlGenerator.GetViewContext(container, htmlGenerator, metadataProvider);
            var inputTagHelper  = new EntityFormTagHelper(_serviceProvider) {
                Model = modelExpression,
                ViewContext = viewContext,
            };

            return inputTagHelper;
        }
    }


    public class TestableHtmlGenerator : DefaultHtmlGenerator {
        private readonly IDictionary<string, object> _validationAttributes;

        public TestableHtmlGenerator(IModelMetadataProvider metadataProvider)
            : this(metadataProvider, Mock.Of<IUrlHelper>()) {
        }

        public TestableHtmlGenerator(IModelMetadataProvider metadataProvider, IUrlHelper urlHelper)
            : this(
                  metadataProvider,
                  GetOptions(),
                  urlHelper,
                  validationAttributes: new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)) {
        }

        public TestableHtmlGenerator(
            IModelMetadataProvider metadataProvider,
            IOptions<MvcViewOptions> options,
            IUrlHelper urlHelper,
            IDictionary<string, object> validationAttributes)
            : base(
                  Mock.Of<IAntiforgery>(),
                  options,
                  metadataProvider,
                  CreateUrlHelperFactory(urlHelper),
                  new HtmlTestEncoder(),
                  new DefaultValidationHtmlAttributeProvider(options, metadataProvider, new ClientValidatorCache())) {
            _validationAttributes = validationAttributes;
        }

        public IDictionary<string, object> ValidationAttributes {
            get { return _validationAttributes; }
        }

        public static ViewContext GetViewContext(
            object model,
            IHtmlGenerator htmlGenerator,
            IModelMetadataProvider metadataProvider) {
            return GetViewContext(model, htmlGenerator, metadataProvider, modelState: new ModelStateDictionary());
        }

        public static ViewContext GetViewContext(
            object model,
            IHtmlGenerator htmlGenerator,
            IModelMetadataProvider metadataProvider,
            ModelStateDictionary modelState) {
            var actionContext = new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new ActionDescriptor(),
                modelState);
            var viewData = new ViewDataDictionary(metadataProvider, modelState) {
                Model = model,
            };
            var viewContext = new ViewContext(
                actionContext,
                Mock.Of<IView>(),
                viewData,
                Mock.Of<ITempDataDictionary>(),
                TextWriter.Null,
                new HtmlHelperOptions());

            return viewContext;
        }

        public override IHtmlContent GenerateAntiforgery(ViewContext viewContext) {
            var tagBuilder = new TagBuilder("input") {
                Attributes =
                    {
                    { "name", "__RequestVerificationToken" },
                    { "type", "hidden" },
                    { "value", "olJlUDjrouRNWLen4tQJhauj1Z1rrvnb3QD65cmQU1Ykqi6S4" }, // 50 chars of a token.
                },
            };

            tagBuilder.TagRenderMode = TagRenderMode.SelfClosing;
            return tagBuilder;
        }

        protected override void AddValidationAttributes(
            ViewContext viewContext,
            TagBuilder tagBuilder,
            ModelExplorer modelExplorer,
            string expression) {
            tagBuilder.MergeAttributes(ValidationAttributes);
        }

        private static IOptions<MvcViewOptions> GetOptions() {
            var mockOptions = new Mock<IOptions<MvcViewOptions>>();
            mockOptions
                .SetupGet(options => options.Value)
                .Returns(new MvcViewOptions());

            return mockOptions.Object;
        }

        private static IUrlHelperFactory CreateUrlHelperFactory(IUrlHelper urlHelper) {
            var factory = new Mock<IUrlHelperFactory>();
            factory
                .Setup(f => f.GetUrlHelper(It.IsAny<ActionContext>()))
                .Returns(urlHelper);

            return factory.Object;
        }
    }
}
