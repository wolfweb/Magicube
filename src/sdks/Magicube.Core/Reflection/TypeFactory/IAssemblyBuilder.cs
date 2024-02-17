namespace Magicube.Core.Reflection {
    public interface IAssemblyBuilder {
        IModuleBuilder NewDynamicModule(string moduleName);
    }
}