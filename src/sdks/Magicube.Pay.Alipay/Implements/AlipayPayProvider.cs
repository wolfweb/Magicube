using Essensoft.Paylink.Alipay;
using Essensoft.Paylink.Alipay.Domain;
using Essensoft.Paylink.Alipay.Request;
using Essensoft.Paylink.Alipay.Response;
using Magicube.Pay.Alipay.ViewModels;
using System;
using System.Threading.Tasks;

namespace Magicube.Pay.Alipay.Implements {
    public class AlipayPayProvider : IAlipayPayProvider {
        private readonly IAlipayClient _client;

        public AlipayPayProvider(IAlipayClient client) {
            _client = client;
        }

        /// <summary>
        /// 当面付-扫码支付
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public async Task<AlipayTradePrecreateResponse> FaceToFacePayByQrcode(AlipayOptions option, AlipayTradePreCreateViewModel viewModel) {
            var model = new AlipayTradePrecreateModel {
                OutTradeNo  = viewModel.OutTradeNo,
                Subject     = viewModel.Subject,
                TotalAmount = viewModel.TotalAmount,
                Body        = viewModel.Body
            };
            var req = new AlipayTradePrecreateRequest();
            req.SetBizModel(model);
            req.SetNotifyUrl(viewModel.NotifyUrl);

            var response = await _client.CertificateExecuteAsync(req, option);
            return response;
        }

        /// <summary>
        /// 当面付-二维码/条码/声波支付
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<AlipayTradePayResponse> FaceToFacePay(AlipayOptions option, AlipayTradePayViewModel viewModel) {
            var model = new AlipayTradePayModel {
                OutTradeNo  = viewModel.OutTradeNo,
                Subject     = viewModel.Subject,
                Scene       = viewModel.Scene,
                AuthCode    = viewModel.AuthCode,
                TotalAmount = viewModel.TotalAmount,
                Body        = viewModel.Body
            };
            var req = new AlipayTradePayRequest();
            req.SetBizModel(model);

            var response = await _client.CertificateExecuteAsync(req, option);
            return response;
        }

        /// <summary>
        /// App支付
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<AlipayTradeAppPayResponse> AppPay(AlipayOptions option, AlipayTradeAppPayViewModel viewModel) {
            var model = new AlipayTradeAppPayModel {
                OutTradeNo  = viewModel.OutTradeNo,
                Subject     = viewModel.Subject,
                ProductCode = viewModel.ProductCode,
                TotalAmount = viewModel.TotalAmount,
                Body        = viewModel.Body
            };
            var req = new AlipayTradeAppPayRequest();
            req.SetBizModel(model);
            req.SetNotifyUrl(viewModel.NotifyUrl);

            var response = await _client.SdkExecuteAsync(req, option);
            // 将response.Body给 ios、android端，由其去完成调起支付宝APP。
            // 客户端 Android 集成流程: https://opendocs.alipay.com/open/204/105296/
            // 客户端 iOS 集成流程: https://opendocs.alipay.com/open/204/105295/
            return response;
        }

        /// <summary>
        /// web支付
        /// </summary>
        /// <returns></returns>
        public async Task<AlipayTradePagePayResponse> WebPay(AlipayOptions option, AlipayTradePagePayViewModel viewModel) {
            var model = new AlipayTradePagePayModel {
                Body        = viewModel.Body,
                Subject     = viewModel.Subject,
                TotalAmount = viewModel.TotalAmount,
                OutTradeNo  = viewModel.OutTradeNo,
                ProductCode = viewModel.ProductCode
            };
            var req = new AlipayTradePagePayRequest();
            req.SetBizModel(model);
            req.SetNotifyUrl(viewModel.NotifyUrl);
            req.SetReturnUrl(viewModel.ReturnUrl);

            var response = await _client.PageExecuteAsync(req, option);

            return response;
        }

        /// <summary>
        /// wap支付
        /// </summary>
        /// <returns></returns>
        public async Task<AlipayTradeWapPayResponse> WapPay(AlipayOptions option, AlipayTradeWapPayViewModel viewModel) {
            var model = new AlipayTradeWapPayModel {
                Body        = viewModel.Body,
                Subject     = viewModel.Subject,
                TotalAmount = viewModel.TotalAmount,
                OutTradeNo  = viewModel.OutTradeNo,
                ProductCode = viewModel.ProductCode
            };
            var req = new AlipayTradeWapPayRequest();
            req.SetBizModel(model);
            req.SetNotifyUrl(viewModel.NotifyUrl);
            req.SetReturnUrl(viewModel.ReturnUrl);

            var response = await _client.PageExecuteAsync(req, option);
            return response;
        }

        /// <summary>
        /// 交易查询
        /// </summary>
        /// <param name="viewMode"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<AlipayTradeQueryResponse> Query(AlipayOptions option, AlipayTradeQueryViewModel viewMode) {
            var model = new AlipayTradeQueryModel {
                OutTradeNo = viewMode.OutTradeNo,
                TradeNo    = viewMode.TradeNo
            };

            var req = new AlipayTradeQueryRequest();
            req.SetBizModel(model);

            var response = await _client.CertificateExecuteAsync(req, option);
            return response;
        }

        /// <summary>
        /// 交易退款
        /// </summary>
        /// <param name="viewMode"></param>
        /// <returns></returns>
        public async Task<AlipayTradeRefundResponse> Refund(AlipayOptions option, AlipayTradeRefundViewModel viewMode) {
            var model = new AlipayTradeRefundModel {
                OutTradeNo   = viewMode.OutTradeNo,
                TradeNo      = viewMode.TradeNo,
                RefundAmount = viewMode.RefundAmount,
                OutRequestNo = viewMode.OutRequestNo,
                RefundReason = viewMode.RefundReason
            };

            var req = new AlipayTradeRefundRequest();
            req.SetBizModel(model);

            var response = await _client.CertificateExecuteAsync(req, option);
            return response;
        }

        /// <summary>
        /// 退款查询
        /// </summary>
        /// <param name="viewMode"></param>
        /// <returns></returns>
        public async Task<AlipayTradeFastpayRefundQueryResponse> RefundQuery(AlipayOptions option, AlipayTradeRefundQueryViewModel viewMode) {
            var model = new AlipayTradeFastpayRefundQueryModel {
                OutTradeNo   = viewMode.OutTradeNo,
                TradeNo      = viewMode.TradeNo,
                OutRequestNo = viewMode.OutRequestNo
            };

            var req = new AlipayTradeFastpayRefundQueryRequest();
            req.SetBizModel(model);

            var response = await _client.CertificateExecuteAsync(req, option);
            return response;
        }

        /// <summary>
        /// 交易关闭
        /// </summary>
        /// <param name="viewMode"></param>
        /// <returns></returns>
        public async Task<AlipayTradeCloseResponse> Close(AlipayOptions option, AlipayTradeCloseViewModel viewMode) {
            var model = new AlipayTradeCloseModel {
                OutTradeNo = viewMode.OutTradeNo,
                TradeNo    = viewMode.TradeNo,
            };

            var req = new AlipayTradeCloseRequest();
            req.SetBizModel(model);
            req.SetNotifyUrl(viewMode.NotifyUrl);

            var response = await _client.CertificateExecuteAsync(req, option);
            return response;
        }

        /// <summary>
        /// 统一转账
        /// </summary>
        public async Task<AlipayFundTransUniTransferResponse> Transfer(AlipayOptions option, AlipayTransferViewModel viewMode) {
            var model = new AlipayFundTransUniTransferModel {
                OutBizNo    = viewMode.OutBizNo,
                TransAmount = viewMode.TransAmount,
                ProductCode = viewMode.ProductCode,
                BizScene    = viewMode.BizScene,
                PayeeInfo   = new Participant { Identity = viewMode.PayeeIdentity, IdentityType = viewMode.PayeeIdentityType, Name = viewMode.PayeeName },
                Remark      = viewMode.Remark
            };
            var req = new AlipayFundTransUniTransferRequest();
            req.SetBizModel(model);
            var response = await _client.CertificateExecuteAsync(req, option);
            return response;
        }

        /// <summary>
        /// 查询统一转账订单
        /// </summary>
        public async Task<AlipayFundTransCommonQueryResponse> TransQuery(AlipayOptions option, AlipayTransQueryViewModel viewMode) {
            var model = new AlipayFundTransCommonQueryModel {
                OutBizNo = viewMode.OutBizNo,
                OrderId  = viewMode.OrderId
            };

            var req = new AlipayFundTransCommonQueryRequest();
            req.SetBizModel(model);
            var response = await _client.CertificateExecuteAsync(req, option);
            return response;
        }

        /// <summary>
        /// 余额查询
        /// </summary>
        public async Task<AlipayFundAccountQueryResponse> AccountQuery(AlipayOptions option, AlipayAccountQueryViewModel viewModel) {
            var model = new AlipayFundAccountQueryModel {
                AlipayUserId = viewModel.AlipayUserId,
                AccountType  = viewModel.AccountType
            };

            var req = new AlipayFundAccountQueryRequest();
            req.SetBizModel(model);
            var response = await _client.CertificateExecuteAsync(req, option);
            return response;
        }
    }
}