using System.Threading.Tasks;
using Magicube.Pay.Abstractions.ViewModels;

namespace Magicube.Pay.Abstractions
{
    public interface IPayService {
        Task<object> Pay(PayViewModel viewModel);
    }

    public class PayService {

    }
}