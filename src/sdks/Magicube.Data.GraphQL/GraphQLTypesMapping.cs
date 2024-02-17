using GraphQL.Types;
using System.Collections.Generic;
using System;

namespace Magicube.Data.GraphQL {
    public static class GraphQLTypesMapping {
        public static Dictionary<Type, Type> GraphTypeMappings = new Dictionary<Type, Type> {
            [typeof(int)]            = typeof(IntGraphType),
            [typeof(Uri)]            = typeof(UriGraphType),
            [typeof(Guid)]           = typeof(GuidGraphType),
            [typeof(uint)]           = typeof(UIntGraphType),
            [typeof(long)]           = typeof(LongGraphType),
            [typeof(byte)]           = typeof(ByteGraphType),
            [typeof(float)]          = typeof(FloatGraphType),
            [typeof(ulong)]          = typeof(ULongGraphType),
            [typeof(sbyte)]          = typeof(SByteGraphType),
            [typeof(short)]          = typeof(ShortGraphType),
            [typeof(string)]         = typeof(StringGraphType),
            [typeof(ushort)]         = typeof(UShortGraphType),
            [typeof(bool)]           = typeof(BooleanGraphType),
            [typeof(decimal)]        = typeof(DecimalGraphType),
            [typeof(DateTime)]       = typeof(DateTimeGraphType),
            [typeof(Enum)]           = typeof(EnumerationGraphType),
            [typeof(DateTimeOffset)] = typeof(DateTimeOffsetGraphType),
        };
    }
}
