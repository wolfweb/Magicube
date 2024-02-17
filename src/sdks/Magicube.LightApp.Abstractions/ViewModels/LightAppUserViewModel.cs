using Magicube.Data.Abstractions.ViewModel;

namespace Magicube.LightApp.Abstractions {
    public class LightAppUserViewModel : ViewModel {
        public string           Token            { get; set; }
        public string           Avator           { get; set; }
        public string           UserName         { get; set; }
        public LightAppUserType LightAppUserType { get; set; }
    }

    public class LigthAppUserDataViewModel : ViewModel {
        public string Iv            { get; set; }
        public string Code          { get; set; }
        public string Provider      { get; set; }
        public string EncryptedData { get; set; }
    }
}
