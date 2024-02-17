namespace Magicube.Web.Configuration {
    public class SiteAdminSetting {
        public string AdminUrlPrefix { get; set; } = "Admin";
    }

    public class AuthenticationSecrityOption {
        public double ExpiredTimes { get; set; } = 15;
    }
}
