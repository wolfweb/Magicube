using Fluid;
using Fluid.Ast;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.Web.UI.Liquid.Statements {
    public class RenderWidgetStatement : Statement {
		private readonly FluidParser _parser;
		private IFluidTemplate _template;

		public string Widget { get; }
		public string Entity { get; }

		public RenderWidgetStatement(FluidParser parser, string widget, string value) {
			Widget = widget;
			Entity = value;
			_parser = parser;
		}

		public override async ValueTask<Completion> WriteToAsync(TextWriter writer, TextEncoder encoder, TemplateContext context) {
			context.IncrementSteps();
			var ctx = context as MagicubeLiquidTemplateContext;
			if (ctx == null) throw new WebUIException($"render need MagicubeFluidTemplateContext");

			var widgetDataProvider = ctx.Services.GetRequiredService<IWidgetService>();
			var widget = await widgetDataProvider.GetWidget(Widget);
			var template = widget.Content;
			// get data by url & widget & entity
			object model = null;

			if (_template == null) {
				if (!_parser.TryParse(template, out _template, out var errors)) {
					throw new ParseException(errors);
				}
            }

            try {
				context.EnterChildScope();
				await _template.RenderAsync(writer, encoder, new TemplateContext(model, context.Options));
            } finally {
				context.ReleaseScope();
			}

			return Completion.Normal;
		}
	}
}
