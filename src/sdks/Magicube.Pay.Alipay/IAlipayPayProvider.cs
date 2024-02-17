using Essensoft.Paylink.Alipay;
using Essensoft.Paylink.Alipay.Response;
using Magicube.Pay.Alipay.ViewModels;
using System.Threading.Tasks;

namespace Magicube.Pay.Alipay {
    public interface IAlipayPayProvider {
        /// <summary>
        /// 当面付-扫码支付
        /// </summary>
        /// <param name="option"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        Task<AlipayTradePrecreateResponse> FaceToFacePayByQrcode(AlipayOptions option, AlipayTradePreCreateViewModel viewModel);

        /// <summary>
        /// 当面付-二维码/条码/声波支付
        /// </summary>
        /// <param name="option"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        Task<AlipayTradePayResponse> FaceToFacePay(AlipayOptions option, AlipayTradePayViewModel viewModel);

        /// <summary>
        /// App支付
        /// </summary>
        /// <param name="option"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        Task<AlipayTradeAppPayResponse> AppPay(AlipayOptions option, AlipayTradeAppPayViewModel viewModel);

        /// <summary>
        /// web支付
        /// </summary>
        /// <param name="option"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        Task<AlipayTradePagePayResponse> WebPay(AlipayOptions option, AlipayTradePagePayViewModel viewModel);

        /// <summary>
        /// wap支付
        /// </summary>
        /// <param name="option"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        Task<AlipayTradeWapPayResponse> WapPay(AlipayOptions option, AlipayTradeWapPayViewModel viewModel);

        /// <summary>
        /// 交易查询
        /// </summary>
        /// <param name="option"></param>
        /// <param name="viewMode"></param>
        /// <returns></returns>
        Task<AlipayTradeQueryResponse> Query(AlipayOptions option, AlipayTradeQueryViewModel viewMode);

        /// <summary>
        /// 交易退款
        /// </summary>
        /// <param name="option"></param>
        /// <param name="viewMode"></param>
        /// <returns></returns>
        Task<AlipayTradeRefundResponse> Refund(AlipayOptions option, AlipayTradeRefundViewModel viewMode);

        /// <summary>
        /// 退款查询
        /// </summary>
        /// <param name="option"></param>
        /// <param name="viewMode"></param>
        /// <returns></returns>
        Task<AlipayTradeFastpayRefundQueryResponse> RefundQuery(AlipayOptions option, AlipayTradeRefundQueryViewModel viewMode);

        /// <summary>
        /// 交易关闭
        /// </summary>
        /// <param name="option"></param>
        /// <param name="viewMode"></param>
        /// <returns></returns>
        Task<AlipayTradeCloseResponse> Close(AlipayOptions option, AlipayTradeCloseViewModel viewMode);

        /// <summary>
        /// 统一转账
        /// </summary>
        /// <param name="option"></param>
        /// <param name="viewMode"></param>
        /// <returns></returns>
        Task<AlipayFundTransUniTransferResponse> Transfer(AlipayOptions option, AlipayTransferViewModel viewMode);

        /// <summary>
        /// 查询统一转账订单
        /// </summary>
        /// <param name="option"></param>
        /// <param name="viewMode"></param>
        /// <returns></returns>
        Task<AlipayFundTransCommonQueryResponse> TransQuery(AlipayOptions option, AlipayTransQueryViewModel viewMode);

        /// <summary>
        /// 余额查询
        /// </summary>
        /// <param name="option"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        Task<AlipayFundAccountQueryResponse> AccountQuery(AlipayOptions option, AlipayAccountQueryViewModel viewModel);
    }
}