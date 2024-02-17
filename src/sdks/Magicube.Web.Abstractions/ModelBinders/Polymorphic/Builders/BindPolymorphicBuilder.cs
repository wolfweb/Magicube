using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace Magicube.Web.ModelBinders.Polymorphic {
    internal class BindPolymorphicBuilder<TAbstract> : IBindPolymorphicBuilder<TAbstract> {
        private readonly PolymorphicBindableCollection<TAbstract> _bindableCollection;

        public BindPolymorphicBuilder() {
            _bindableCollection = new PolymorphicBindableCollection<TAbstract>();
        }

        public IBindPolymorphicBuilder<TAbstract> AddFromType<TImplementation>() where TImplementation : TAbstract, new() {
            return Add(new TypeInValuePolymorphicBindable(typeof(TImplementation)));
        }

        public IBindPolymorphicBuilder<TAbstract> AddFromDiscriminator<TImplementation>() where TImplementation : TAbstract, new() {
            return AddFromDiscriminator<TImplementation>(
                "Discriminator");
        }

        public IBindPolymorphicBuilder<TAbstract> AddFromDiscriminator<TImplementation>(string fieldName) where TImplementation : TAbstract, new() {
            return AddFromDiscriminator<TImplementation>(
                fieldName,
                discriminatorValue => discriminatorValue == typeof(TImplementation).Name);
        }

        public IBindPolymorphicBuilder<TAbstract> AddFromDiscriminator<TImplementation>(Func<string, bool> discriminatorValueMatch) where TImplementation : TAbstract, new() {
            return AddFromDiscriminator<TImplementation>(
                "Discriminator",
                discriminatorValueMatch);
        }

        public IBindPolymorphicBuilder<TAbstract> AddFromDiscriminator<TImplementation>(string fieldName, Func<string, bool> discriminatorValueMatch) where TImplementation : TAbstract, new() {
            return Add(new DiscriminatorPolymorphicBindable(
                typeof(TImplementation),
                fieldName,
                discriminatorValueMatch));
        }

        public IBindPolymorphicBuilder<TAbstract> AddFromCustom<TImplementation>(Func<ModelBindingContext, bool> isMatchFunc) where TImplementation : TAbstract, new() {
            return Add(new CustomPolymorphicBindable(typeof(TImplementation), isMatchFunc));
        }

        public IBindPolymorphicBuilder<TAbstract> Add(IPolymorphicBindable bindable) {
            _bindableCollection.AddEntryType(bindable);
            return this;
        }

        public IPolymorphicBindableCollection Build() => _bindableCollection;
    }
}
