using Magicube.Data.Abstractions.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;
using Magicube.Data.Abstractions;

namespace Magicube.Data.ProviderTest {
    public class ValidationTest {
        [Fact]
        public void Func_DynamicEntityValidationFactory_Test() {
            IEnumerable<DbFieldValidator> Validators = new[] {
                new DbFieldValidator {
                    Provider = "Required"
                },
                new DbFieldValidator {
                    Provider = "EmailAddress"
                }
            };
            Assert.Throws<ValidationException>(() => DynamicEntityValidationFactory.Validator("email", null, Validators));

            Assert.Throws<ValidationException>(() => DynamicEntityValidationFactory.Validator("email", "abcdefg", Validators));

            DynamicEntityValidationFactory.Validator("email", "empty@empty.com", Validators);
        }
    }
}
