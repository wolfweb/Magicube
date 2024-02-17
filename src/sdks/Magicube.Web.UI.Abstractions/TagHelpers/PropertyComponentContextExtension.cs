using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.ViewModel;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;
using System.Text.Encodings.Web;

namespace Magicube.Web.UI.TagHelpers {
    public static class PropertyComponentContextExtension {
        public static (string,DisplayAttribute) GetDisplay(this PropertyComponentContext propertyContext) {
            DisplayAttribute displayAttr = null;

            if (propertyContext.Override != null) {
                displayAttr = propertyContext.Override.Property.GetAttribute<DisplayAttribute>();
            } 
            if(displayAttr == null) {
                displayAttr = propertyContext.Property.GetCustomAttribute<DisplayAttribute>();
            }            

            if (displayAttr == null) return (propertyContext.Property.Name, null);

            return (displayAttr.Name, displayAttr);
        }

        public static object GetValue(this PropertyComponentContext propertyContext, object model) {
            if (model is IEntityViewModel viewModel) {
                if (propertyContext.Override != null) {
                    return propertyContext.Override.Property.Member.Value(viewModel);
                }
                return propertyContext.Property.Value(viewModel);
            } else { 
                if (propertyContext.Override != null) {
                    return propertyContext.Override.Property.Member.GetValue(model);
                }
                return propertyContext.Property.GetValue(model);
            }
        }

        public static bool HasAttribute<T>(this PropertyComponentContext propertyContext, out T attribute) where T : Attribute {
            attribute = null;
            if (propertyContext.Override != null) {
                attribute = propertyContext.Override.Property.GetAttribute<T>();
            } 
            
            if(attribute == null){
                attribute = propertyContext.Property.GetCustomAttribute<T>();
            }

            return attribute != null;
        }

        public static bool HasAttributes<T>(this PropertyComponentContext propertyContext, out IEnumerable<T> attribute) where T : Attribute {
            attribute = null;
            if (propertyContext.Override != null) {
                attribute = propertyContext.Override.Property.GetAttributes<T>();
            }

            if (attribute == null) {
                attribute = propertyContext.Property.GetCustomAttributes<T>();
            }

            return attribute != null;
        }

        public static bool Readonly(this PropertyComponentContext propertyContext) {
            var result = false;
            if (propertyContext.Override != null) {
                result = !propertyContext.Override.Property.Member.CanWrite || propertyContext.Override.Property.GetAttribute<ReadOnlyAttribute>() != null;
            }

            if (!result) {
                result = !propertyContext.Property.CanWrite || propertyContext.Property.GetCustomAttribute<ReadOnlyAttribute>() != null;
            }

            return result;
        }

        public static string RenderTag(this IHtmlContent output, HtmlEncoder encoder) {
            using (var writer = new StringWriter()) {
                output.WriteTo(writer, encoder);
                return writer.ToString();
            }
        }
    }
}
