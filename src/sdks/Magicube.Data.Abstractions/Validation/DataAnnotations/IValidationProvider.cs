using Magicube.Core.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicube.Data.Abstractions.Validation {
    public class ValidationOptions {

    }

    public interface IValidationProvider {

    }

    public class ValidationProvider : IValidationProvider {
        public ValidationProvider() {
            var validations = TypeAccessor.Get<RequiredAttribute>().ScanTypes<ValidationAttribute>();
        }
    }
}
