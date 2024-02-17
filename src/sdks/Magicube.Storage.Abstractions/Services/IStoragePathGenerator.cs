using Magicube.Core.Environment.Variable;

namespace Magicube.Storage.Abstractions.Services {
    /// <summary>
    /// 存储路径生成器
    /// </summary>
    public interface IStoragePathGenerator {
        string Generate(string template, string fileName);
    }

    public class StoragePathGenerator : IStoragePathGenerator {
        private readonly VariableFactroy _envVariableFactroy;

        public StoragePathGenerator(VariableFactroy envVariableFactroy) {
            _envVariableFactroy = envVariableFactroy;
        }

        public string Generate(string template, string fileName) {
            return _envVariableFactroy.Parse(template, fileName);
        }
    }
}
