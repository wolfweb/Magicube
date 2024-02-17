using System;
using System.Dynamic;

namespace Magicube.Data.Abstractions.Validation {
    internal class KeyGetMemberBinder : GetMemberBinder {
        public KeyGetMemberBinder(string key) : base(key, true) {

        }

        public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion) {
            if (!target.HasValue)
                return Defer(target);

            throw new NotImplementedException();
        }
    }
}
