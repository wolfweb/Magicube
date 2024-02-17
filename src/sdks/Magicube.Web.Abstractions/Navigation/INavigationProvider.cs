using System.Collections.Generic;
using System.Threading.Tasks;

namespace Magicube.Web.Navigation {
    public interface INavigationProvider {
        Task BuildNavigationAsync(string name, NavigationBuilder builder);
    }
}
