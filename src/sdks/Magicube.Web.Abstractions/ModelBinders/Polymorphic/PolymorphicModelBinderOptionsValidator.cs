using Microsoft.Extensions.Options;

namespace Magicube.Web.ModelBinders.Polymorphic {
    class PolymorphicModelBinderOptionsValidator : IValidateOptions<PolymorphicModelBinderOptions> {
        public ValidateOptionsResult Validate(string name, PolymorphicModelBinderOptions options) {
            return ValidateOptionsResult.Success;
        }
    }
}
