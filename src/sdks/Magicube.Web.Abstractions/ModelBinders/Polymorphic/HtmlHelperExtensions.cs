using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Expressions;
using System;
using System.Collections;

namespace Magicube.Web.ModelBinders.Polymorphic {
    public static class HtmlHelperExtensions {
        public static IHtmlContent PolymorphicEditorFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression) {
            if (typeof(IEnumerable).IsAssignableFrom(typeof(TProperty)))
                throw new InvalidOperationException($"There is currently no support for using an {nameof(IEnumerable)}. Iterate through the list in the view and call the ${nameof(PolymorphicEditorFor)} method for each item.");

            if (htmlHelper.ViewData.Model == null) return new HtmlString(string.Empty);

            var templateName = expression.Compile()(htmlHelper.ViewData.Model)?.GetType().Name;

            return htmlHelper.EditorFor(expression, templateName);
        }

        public static IHtmlContent PolymorphicTypeValue(this IHtmlHelper htmlHelper) {
            if (htmlHelper.ViewData.Model == null)
                throw new ArgumentNullException(nameof(htmlHelper.ViewData.Model));

            return htmlHelper.Hidden(TypeInValuePolymorphicBindable.FieldName,htmlHelper.ViewData.Model.GetType().AssemblyQualifiedName);
        }

        public static IHtmlContent PolymorphicTypeValue<T>(this IHtmlHelper htmlHelper) {
            return htmlHelper.Hidden(TypeInValuePolymorphicBindable.FieldName, typeof(T).AssemblyQualifiedName);
        }

        public static IHtmlContent PolymorphicTypeValue(this IHtmlHelper htmlHelper, Type type) {
            return htmlHelper.Hidden(TypeInValuePolymorphicBindable.FieldName, type.AssemblyQualifiedName);
        }

        public static IHtmlContent PolymorphicTypeValueFor<T, TProp>(this IHtmlHelper<T> htmlHelper, Expression<Func<T, TProp>> expression) {
            var mvcHtmlString = htmlHelper.NameFor(expression);
            var model = expression.Compile()(htmlHelper.ViewData.Model);
            if (model == null) throw new ArgumentNullException(nameof(model));

            return htmlHelper.Hidden($"{mvcHtmlString}.{TypeInValuePolymorphicBindable.FieldName}",
                    model.GetType().AssemblyQualifiedName);
        }
    }
}
