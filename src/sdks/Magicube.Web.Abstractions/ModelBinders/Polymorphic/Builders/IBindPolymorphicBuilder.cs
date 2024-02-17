using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace Magicube.Web.ModelBinders.Polymorphic {
    public interface IBindPolymorphicBuilder<in TAbstract> {
        IBindPolymorphicBuilder<TAbstract> AddFromType<TImplementation>() where TImplementation : TAbstract, new();

        IBindPolymorphicBuilder<TAbstract> AddFromDiscriminator<TImplementation>() where TImplementation : TAbstract, new();

        IBindPolymorphicBuilder<TAbstract> AddFromDiscriminator<TImplementation>(string fieldName) 
            where TImplementation : TAbstract, new();

        IBindPolymorphicBuilder<TAbstract> AddFromDiscriminator<TImplementation>(Func<string, bool> discriminatorValueMatch) 
            where TImplementation : TAbstract, new();

        IBindPolymorphicBuilder<TAbstract> AddFromDiscriminator<TImplementation>(string fieldName, Func<string, bool> discriminatorValueMatch)
            where TImplementation : TAbstract, new();

        IBindPolymorphicBuilder<TAbstract> AddFromCustom<TImplementation>(Func<ModelBindingContext, bool> isMatchFunc) 
            where TImplementation : TAbstract, new();

        IBindPolymorphicBuilder<TAbstract> Add(IPolymorphicBindable bindable);
    }
}
