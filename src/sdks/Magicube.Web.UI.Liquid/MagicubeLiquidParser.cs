using Fluid;
using Fluid.Ast;
using Fluid.ViewEngine;
using Magicube.Web.UI.Liquid.Statements;
using Parlot.Fluent;

namespace Magicube.Web.UI.Liquid {
    public class MagicubeLiquidParser : FluidViewParser {
        public const string RenderKey = "render";

        /// <summary>
        /// {% render {widgetType}:{entity} %}
        /// </summary>
        public MagicubeLiquidParser() {
            var widgetRenderTag = Identifier.ElseError($"An identifier was expected after the '{RenderKey}' tag")
                .AndSkip(Colon.ElseError($"':' was expected after the identifier of '{RenderKey}'"))
                .And(Identifier)
                .AndSkip(TagEnd.ElseError("'%}' was expected"))
                .Then<Statement>(x => {
                    return new RenderWidgetStatement(this, x.Item1, x.Item2);
                });
            RegisteredTags[RenderKey] = widgetRenderTag;
        }
    }
}
