namespace Magicube.Core {
    public interface IMagicubeConfigProvider<T> where T : class {
        T GetSetting();

        void SetSetting(T setting);
    }
}