using System.Threading.Tasks;

namespace Magicube.LightApp.Abstractions {
    public interface ILightAppUserService {
        string Key { get; }
        Task<LightAppUserViewModel> GetOrAddUser(string code);
        Task Profile(long userId, LigthAppUserDataViewModel viewModel);
    }
}
