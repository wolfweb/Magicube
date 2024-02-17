using System.Reflection;

namespace Magicube.Core.Reflection {
    public interface IModuleBuilder {
        ITypeBuilder NewType(string typeName);

        IMethodBuilder NewGlobalMethod(string methodName);

        IModuleBuilder CreateGlobalFunctions();

        MethodInfo GetMethod(string methodName);
    }
}