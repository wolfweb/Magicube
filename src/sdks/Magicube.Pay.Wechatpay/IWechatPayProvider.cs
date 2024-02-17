using Essensoft.Paylink.WeChatPay;
using Essensoft.Paylink.WeChatPay.V3;
using Essensoft.Paylink.WeChatPay.V3.Domain;
using Essensoft.Paylink.WeChatPay.V3.Request;
using Essensoft.Paylink.WeChatPay.V3.Response;
using Magicube.Pay.Wechatpay.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Magicube.Pay.Wechatpay {
    public interface IWechatPayProvider {
        /// <summary>
        /// APP支付-App下单API
        /// </summary>
        /// <param name="option"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        Task<WeChatPayDictionary> AppPay(WeChatPayOptions option, WeChatPayAppPayV3ViewModel viewModel);

        /// <summary>
        /// 公众号支付-JSAPI下单
        /// </summary>
        /// <param name="option"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        Task<WeChatPayDictionary> PubPay(WeChatPayOptions option, WeChatPayPubPayV3ViewModel viewModel);

        /// <summary>
        /// 扫码支付-Native下单API
        /// </summary>
        /// <param name="option"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        Task<WeChatPayTransactionsNativeResponse> QrCodePay(WeChatPayOptions option, WeChatPayQrCodePayV3ViewModel viewModel);

        /// <summary>
        /// H5支付-H5下单API
        /// </summary>
        /// <param name="option"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        Task<WeChatPayTransactionsH5Response> H5Pay(WeChatPayOptions option, WeChatPayH5PayV3ViewModel viewModel);

        /// <summary>
        /// 小程序支付-JSAPI下单
        /// </summary>
        /// <param name="option"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        Task<WeChatPayDictionary> MiniProgramPay(WeChatPayOptions option, WeChatPayPubPayV3ViewModel viewModel);

        /// <summary>
        /// 微信支付订单号查询
        /// </summary>
        /// <param name="option"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        Task<WeChatPayTransactionsIdResponse> QueryByTransactionId(WeChatPayOptions option, WeChatPayQueryByTransactionIdViewModel viewModel);

        /// <summary>
        /// 商户订单号查询
        /// </summary>
        /// <param name="option"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        Task<WeChatPayTransactionsOutTradeNoResponse> QueryByOutTradeNo(WeChatPayOptions option, WeChatPayQueryByOutTradeNoViewModel viewModel);

        /// <summary>
        /// 关闭订单
        /// </summary>
        /// <param name="option"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        Task<WeChatPayTransactionsOutTradeNoCloseResponse> OutTradeNoClose(WeChatPayOptions option, WeChatPayOutTradeNoCloseViewModel viewModel);

        /// <summary>
        /// 申请交易账单
        /// </summary>
        /// <param name="option"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        Task<WeChatPayBillTradeBillResponse> TradeBill(WeChatPayOptions option, WeChatPayTradeBillViewModel viewModel);

        /// <summary>
        /// 申请资金账单
        /// </summary>
        /// <param name="option"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        Task<WeChatPayBillFundflowBillResponse> FundflowBill(WeChatPayOptions option, WeChatPayFundflowBillViewModel viewModel);

        /// <summary>
        /// 下载账单
        /// </summary>
        /// <param name="option"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        Task<WeChatPayBillDownloadResponse> BillDownload(WeChatPayOptions option, WeChatPayBillDownloadViewModel viewModel);

        /// <summary>
        /// 退款申请
        /// </summary>
        /// <param name="option"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        Task<WeChatPayRefundDomesticRefundsResponse> Refund(WeChatPayOptions option, WeChatPayV3RefundViewModel viewModel);

        /// <summary>
        /// 查询单笔退款
        /// </summary>
        /// <param name="option"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        Task<WeChatPayRefundDomesticRefundsOutRefundNoResponse> RefundQuery(WeChatPayOptions option, WeChatPayV3RefundQueryViewModel viewModel);

        /// <summary>
        /// 支付分-创建支付分订单
        /// </summary>
        /// <param name="option"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        Task<WeChatPayScoreServiceOrderResponse> ServiceOrder(WeChatPayOptions option, WeChatPayScoreServiceOrderViewModel viewModel);

        /// <summary>
        /// 支付分-查询支付分订单
        /// </summary>        
        Task<WeChatPayScoreServiceOrderQueryResponse> ServiceOrderQuery(WeChatPayOptions option, WeChatPayScoreServiceOrderQueryViewModel viewModel);

        /// <summary>
        /// 支付分-取消支付分订单
        /// </summary>        
        Task<WeChatPayScoreServiceOrderOutOrderNoCancelResponse> ServiceOrderCancel(WeChatPayOptions option, WeChatPayScoreServiceOrderCancelViewModel viewModel);

        /// <summary>
        /// 支付分-修改支付分订单金额
        /// </summary>        
        Task<WeChatPayScoreServiceOrderOutOrderNoModifyResponse> ServiceOrderModify(WeChatPayOptions option, WeChatPayScoreServiceOrderModifyViewModel viewModel);


        /// <summary>
        /// 支付分-完结支付分订单
        /// </summary>        
        Task<WeChatPayScoreServiceOrderOutOrderNoCompleteResponse> ServiceOrderComplete(WeChatPayOptions option, WeChatPayScoreServiceOrderCompleteViewModel viewModel);


        /// <summary>
        /// 支付分-商户发起催收扣款
        /// </summary>        
        Task<WeChatPayScoreServiceOrderPayResponse> ServiceOrderPay(WeChatPayOptions option, WeChatPayScoreServiceOrderPayViewModel viewModel);


        /// <summary>
        /// 支付分-同步服务订单信息
        /// </summary>        
        Task<WeChatPayScoreServiceOrderSyncResponse> ServiceOrderSync(WeChatPayOptions option, WeChatPayScoreServiceOrderSyncViewModel viewModel);

        /// <summary>
        /// 支付分-创单结单合并
        /// </summary>        
        Task<WeChatPayScoreServiceOrderDirectCompleteResponse> ServiceOrderDirectComplete(WeChatPayOptions option, WeChatPayScoreServiceOrderDirectCompleteViewModel viewModel);

        /// <summary>
        /// 支付分-商户预授权
        /// </summary>
        Task<WeChatPayScorePermissionsResponse> Permissions(WeChatPayOptions option, PermissionsViewModel viewModel);

        /// <summary>
        /// 支付分-查询用户授权记录（授权协议号）
        /// </summary>
        Task<WeChatPayScorePermissionsQueryForAuthCodeResponse> PermissionsQueryForAuthCode(WeChatPayOptions option, PermissionsQueryForAuthCodeViewModel viewModel);

        /// <summary>
        /// 支付分-解除用户授权关系（授权协议号）
        /// </summary>
        Task<WeChatPayScorePermissionsTerminateForAuthCodeResponse> PermissionsTerminateForAuthCode(WeChatPayOptions option, PermissionsTerminateForAuthCodeViewModel viewModel);


        /// <summary>
        /// 支付分-查询用户授权记录（openid）
        /// </summary>
        Task<WeChatPayScorePermissionsQueryForOpenIdResponse> PermissionsQueryForOpenId(WeChatPayOptions option, PermissionsQueryForOpenIdViewModel viewModel);

        /// <summary>
        /// 支付分-解除用户授权关系（OpenId）
        /// </summary>
        Task<WeChatPayScorePermissionsTerminateForOpenIdResponse> PermissionsTerminateForOpenId(WeChatPayOptions option, PermissionsTerminateForOpenIdViewModel viewModel);
    }

    public class WechatPayProvider : IWechatPayProvider {
        private readonly IWeChatPayClient _client;

        public WechatPayProvider(IWeChatPayClient client) {
            _client = client;
        }

        /// <summary>
        /// APP支付-App下单API
        /// </summary>
        /// <param name="viewModel"></param>        
        public async Task<WeChatPayDictionary> AppPay(WeChatPayOptions option, WeChatPayAppPayV3ViewModel viewModel) {
            var model = new WeChatPayTransactionsAppBodyModel {
                AppId       = option.AppId,
                MchId       = option.MchId,
                Amount      = new Amount { Total = viewModel.Total, Currency = "CNY" },
                Description = viewModel.Description,
                NotifyUrl   = viewModel.NotifyUrl,
                OutTradeNo  = viewModel.OutTradeNo,
            };

            var request = new WeChatPayTransactionsAppRequest();
            request.SetBodyModel(model);

            var response = await _client.ExecuteAsync(request, option);

            if (!response.IsError) {
                var req = new WeChatPayAppSdkRequest {
                    PrepayId = response.PrepayId
                };

                var parameter = await _client.ExecuteAsync(req, option);

                // 将参数(parameter)给 ios/android端 
                // https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter3_2_4.shtml
                return parameter;
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// 公众号支付-JSAPI下单
        /// </summary>
        /// <param name="viewModel"></param>        
        public async Task<WeChatPayDictionary> PubPay(WeChatPayOptions option, WeChatPayPubPayV3ViewModel viewModel) {
            var model = new WeChatPayTransactionsJsApiBodyModel {
                AppId = option.AppId,
                MchId = option.MchId,
                Amount = new Amount { Total = viewModel.Total, Currency = "CNY" },
                Description = viewModel.Description,
                NotifyUrl = viewModel.NotifyUrl,
                OutTradeNo = viewModel.OutTradeNo,
                Payer = new PayerInfo { OpenId = viewModel.OpenId }
            };

            var request = new WeChatPayTransactionsJsApiRequest();
            request.SetBodyModel(model);

            var response = await _client.ExecuteAsync(request, option);

            if (!response.IsError) {
                var req = new WeChatPayJsApiSdkRequest {
                    Package = "prepay_id=" + response.PrepayId
                };

                var parameter = await _client.ExecuteAsync(req, option);

                // 将参数(parameter)给 公众号前端
                // https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter3_1_4.shtml
                return parameter;
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// 扫码支付-Native下单API
        /// </summary>
        /// <param name="viewModel"></param>        
        public async Task<WeChatPayTransactionsNativeResponse> QrCodePay(WeChatPayOptions option, WeChatPayQrCodePayV3ViewModel viewModel) {
            var model = new WeChatPayTransactionsNativeBodyModel {
                AppId = option.AppId,
                MchId = option.MchId,
                Amount = new Amount { Total = viewModel.Total, Currency = "CNY" },
                Description = viewModel.Description,
                NotifyUrl = viewModel.NotifyUrl,
                OutTradeNo = viewModel.OutTradeNo,
            };

            var request = new WeChatPayTransactionsNativeRequest();
            request.SetBodyModel(model);

            var response = await _client.ExecuteAsync(request, option);
            if (!response.IsError) {
                // response.CodeUrl 给前端生成二维码
                return response;
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// H5支付-H5下单API
        /// </summary>
        /// <param name="viewModel"></param>        
        public async Task<WeChatPayTransactionsH5Response> H5Pay(WeChatPayOptions option, WeChatPayH5PayV3ViewModel viewModel) {
            var model = new WeChatPayTransactionsH5BodyModel {
                AppId = option.AppId,
                MchId = option.MchId,
                Amount = new Amount { Total = viewModel.Total, Currency = "CNY" },
                Description = viewModel.Description,
                NotifyUrl = viewModel.NotifyUrl,
                OutTradeNo = viewModel.OutTradeNo,
                SceneInfo = new SceneInfo { PayerClientIp = "127.0.0.1", H5Info = new H5Info { Type = "Wap" } }
            };

            var request = new WeChatPayTransactionsH5Request();
            request.SetBodyModel(model);

            var response = await _client.ExecuteAsync(request, option);

            // h5_url为拉起微信支付收银台的中间页面，可通过访问该url来拉起微信客户端，完成支付,h5_url的有效期为5分钟。
            // https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter3_3_4.shtml
            return response;
        }


        /// <summary>
        /// 小程序支付-JSAPI下单
        /// </summary>
        /// <param name="viewModel"></param>        
        public async Task<WeChatPayDictionary> MiniProgramPay(WeChatPayOptions option, WeChatPayPubPayV3ViewModel viewModel) {
            var model = new WeChatPayTransactionsJsApiBodyModel {
                AppId = option.AppId,
                MchId = option.MchId,
                Amount = new Amount { Total = viewModel.Total, Currency = "CNY" },
                Description = viewModel.Description,
                NotifyUrl = viewModel.NotifyUrl,
                OutTradeNo = viewModel.OutTradeNo,
                Payer = new PayerInfo { OpenId = viewModel.OpenId }
            };

            var request = new WeChatPayTransactionsJsApiRequest();
            request.SetBodyModel(model);

            var response = await _client.ExecuteAsync(request, option);

            if (!response.IsError) {
                var req = new WeChatPayMiniProgramSdkRequest {
                    Package = "prepay_id=" + response.PrepayId
                };

                var parameter = await _client.ExecuteAsync(req, option);

                // 将参数(parameter)给 小程序端
                // https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter3_5_4.shtml
                return parameter;
            }
            throw new NotImplementedException();
        }


        /// <summary>
        /// 微信支付订单号查询
        /// </summary>
        /// <param name="viewModel"></param>        
        public async Task<WeChatPayTransactionsIdResponse> QueryByTransactionId(WeChatPayOptions option, WeChatPayQueryByTransactionIdViewModel viewModel) {
            var model = new WeChatPayTransactionsIdQueryModel {
                MchId = option.MchId,
            };

            var request = new WeChatPayTransactionsIdRequest {
                TransactionId = viewModel.TransactionId
            };

            request.SetQueryModel(model);

            var response = await _client.ExecuteAsync(request, option);

            throw new NotImplementedException();
        }

        /// <summary>
        /// 商户订单号查询
        /// </summary>
        /// <param name="viewModel"></param>        
        public async Task<WeChatPayTransactionsOutTradeNoResponse> QueryByOutTradeNo(WeChatPayOptions option, WeChatPayQueryByOutTradeNoViewModel viewModel) {
            var model = new WeChatPayTransactionsOutTradeNoQueryModel {
                MchId = option.MchId,
            };

            var request = new WeChatPayTransactionsOutTradeNoRequest {
                OutTradeNo = viewModel.OutTradeNo,
            };

            request.SetQueryModel(model);

            var response = await _client.ExecuteAsync(request, option);

            return response;
        }


        /// <summary>
        /// 关闭订单
        /// </summary>
        /// <param name="viewModel"></param>        
        public async Task<WeChatPayTransactionsOutTradeNoCloseResponse> OutTradeNoClose(WeChatPayOptions option, WeChatPayOutTradeNoCloseViewModel viewModel) {
            var model = new WeChatPayTransactionsOutTradeNoCloseBodyModel {
                MchId = option.MchId,
            };

            var request = new WeChatPayTransactionsOutTradeNoCloseRequest {
                OutTradeNo = viewModel.OutTradeNo,
            };

            request.SetBodyModel(model);

            var response = await _client.ExecuteAsync(request, option);
            return response;
        }


        /// <summary>
        /// 申请交易账单
        /// </summary>
        /// <param name="viewModel"></param>        
        public async Task<WeChatPayBillTradeBillResponse> TradeBill(WeChatPayOptions option, WeChatPayTradeBillViewModel viewModel) {
            var model = new WeChatPayBillTradeBillQueryModel {
                BillDate = viewModel.BillDate
            };

            var request = new WeChatPayBillTradeBillRequest();

            request.SetQueryModel(model);

            var response = await _client.ExecuteAsync(request, option);

            return response;
        }


        /// <summary>
        /// 申请资金账单
        /// </summary>
        /// <param name="viewModel"></param>        
        public async Task<WeChatPayBillFundflowBillResponse> FundflowBill(WeChatPayOptions option, WeChatPayFundflowBillViewModel viewModel) {
            var model = new WeChatPayBillFundflowBillQueryModel {
                BillDate = viewModel.BillDate
            };

            var request = new WeChatPayBillFundflowBillRequest();
            request.SetQueryModel(model);

            var response = await _client.ExecuteAsync(request, option);

            return response;
        }

        /// <summary>
        /// 下载账单
        /// </summary>
        /// <param name="viewModel"></param>        
        public async Task<WeChatPayBillDownloadResponse> BillDownload(WeChatPayOptions option, WeChatPayBillDownloadViewModel viewModel) {
            var request = new WeChatPayBillDownloadRequest();
            request.SetRequestUrl(viewModel.DownloadUrl);

            var response = await _client.ExecuteAsync(request, option);
            return response;
        }


        /// <summary>
        /// 退款申请
        /// </summary>
        /// <param name="viewModel"></param>        
        public async Task<WeChatPayRefundDomesticRefundsResponse> Refund(WeChatPayOptions option, WeChatPayV3RefundViewModel viewModel) {
            var model = new WeChatPayRefundDomesticRefundsBodyModel() {
                TransactionId = viewModel.TransactionId,
                OutTradeNo    = viewModel.OutTradeNo,
                OutRefundNo   = viewModel.OutRefundNo,
                NotifyUrl     = viewModel.NotifyUrl,
                Amount        = new RefundAmount { Refund = viewModel.RefundAmount, Total = viewModel.TotalAmount, Currency = viewModel.Currency }
            };

            var request = new WeChatPayRefundDomesticRefundsRequest();
            request.SetBodyModel(model);

            var response = await _client.ExecuteAsync(request, option);
            return response;
        }


        /// <summary>
        /// 查询单笔退款
        /// </summary>
        /// <param name="viewModel"></param>        
        public async Task<WeChatPayRefundDomesticRefundsOutRefundNoResponse> RefundQuery(WeChatPayOptions option, WeChatPayV3RefundQueryViewModel viewModel) {
            var request = new WeChatPayRefundDomesticRefundsOutRefundNoRequest {
                OutRefundNo = viewModel.OutRefundNo
            };

            var response = await _client.ExecuteAsync(request, option);
            return response;
        }

        #region 微信支付分


        /// <summary>
        /// 支付分-创建支付分订单
        /// </summary>        
        public async Task<WeChatPayScoreServiceOrderResponse> ServiceOrder(WeChatPayOptions option, WeChatPayScoreServiceOrderViewModel viewModel) {
            var model = new WeChatPayScoreServiceOrderBodyModel {
                AppId               = option.AppId,
                ServiceId           = viewModel.ServiceId,
                OutOrderNo          = viewModel.OutOrderNo,
                ServiceIntroduction = viewModel.ServiceIntroduction,
                TimeRange           = new TimeRange {
                    StartTime       = viewModel.StartTime,
                    EndTime         = viewModel.EndTime
                },
                RiskFund            = new RiskFund {
                    Name            = viewModel.RiskFundName,
                    Amount          = viewModel.RiskFundAmount
                },
                NotifyUrl           = viewModel.NotifyUrl,
                OpenId              = viewModel.OpenId
            };

            var request = new WeChatPayScoreServiceOrderRequest();
            request.SetBodyModel(model);

            var response = await _client.ExecuteAsync(request, option);
            return response;
        }

        /// <summary>
        /// 支付分-查询支付分订单
        /// </summary>        
        public async Task<WeChatPayScoreServiceOrderQueryResponse> ServiceOrderQuery(WeChatPayOptions option, WeChatPayScoreServiceOrderQueryViewModel viewModel) {
            var model = new WeChatPayScoreServiceOrderQueryModel {
                AppId      = option.AppId,
                ServiceId  = viewModel.ServiceId,
                OutOrderNo = viewModel.OutOrderNo,
                QueryId    = viewModel.QueryId
            };

            var request = new WeChatPayScoreServiceOrderQueryRequest();
            request.SetQueryModel(model);

            var response = await _client.ExecuteAsync(request, option);
            return response;
        }

        /// <summary>
        /// 支付分-取消支付分订单
        /// </summary>        
        public async Task<WeChatPayScoreServiceOrderOutOrderNoCancelResponse> ServiceOrderCancel(WeChatPayOptions option, WeChatPayScoreServiceOrderCancelViewModel viewModel) {
            var model = new WeChatPayScoreServiceOrderOutOrderNoCancelBodyModel {
                AppId     = option.AppId,
                ServiceId = viewModel.ServiceId,
                Reason    = viewModel.Reason
            };

            var request = new WeChatPayScoreServiceOrderOutOrderNoCancelRequest {
                OutOrderNo = viewModel.OutOrderNo
            };
            request.SetBodyModel(model);

            var response = await _client.ExecuteAsync(request, option);
            return response;
        }

        /// <summary>
        /// 支付分-修改支付分订单金额
        /// </summary>        
        public async Task<WeChatPayScoreServiceOrderOutOrderNoModifyResponse> ServiceOrderModify(WeChatPayOptions option, WeChatPayScoreServiceOrderModifyViewModel viewModel) {
            var model = new WeChatPayScoreServiceOrderOutOrderNoModifyBodyModel {
                AppId        = option.AppId,
                ServiceId    = viewModel.ServiceId,
                PostPayments = new List<PostPayment> {
                   new PostPayment{
                       Name   = viewModel.Name,
                       Amount = viewModel.Amount,
                       Count  = viewModel.Count
                   }
                },
                TotalAmount = viewModel.TotalAmount,
                Reason      = viewModel.Reason
            };

            var request = new WeChatPayScoreServiceOrderOutOrderNoModifyRequest {
                OutOrderNo = viewModel.OutOrderNo
            };
            request.SetBodyModel(model);

            var response = await _client.ExecuteAsync(request, option);
            return response;
        }


        /// <summary>
        /// 支付分-完结支付分订单
        /// </summary>        
        public async Task<WeChatPayScoreServiceOrderOutOrderNoCompleteResponse> ServiceOrderComplete(WeChatPayOptions option, WeChatPayScoreServiceOrderCompleteViewModel viewModel) {
            var model = new WeChatPayScoreServiceOrderOutOrderNoCompleteBodyModel {
                AppId = option.AppId,
                ServiceId = viewModel.ServiceId,
                PostPayments = new List<PostPayment>
                {
                   new PostPayment
                   {
                       Name = viewModel.Name,
                       Amount = viewModel.Amount,
                       Count = viewModel.Count
                   }
                },
                TotalAmount = viewModel.TotalAmount
            };

            var request = new WeChatPayScoreServiceOrderOutOrderNoCompleteRequest {
                OutOrderNo = viewModel.OutOrderNo
            };
            request.SetBodyModel(model);

            var response = await _client.ExecuteAsync(request, option);
            return response;
        }


        /// <summary>
        /// 支付分-商户发起催收扣款
        /// </summary>        
        public async Task<WeChatPayScoreServiceOrderPayResponse> ServiceOrderPay(WeChatPayOptions option, WeChatPayScoreServiceOrderPayViewModel viewModel) {
            var model = new WeChatPayScoreServiceOrderPayBodyModel {
                AppId = option.AppId,
                ServiceId = viewModel.ServiceId,
            };

            var request = new WeChatPayScoreServiceOrderPayRequest {
                OutOrderNo = viewModel.OutOrderNo
            };
            request.SetBodyModel(model);

            var response = await _client.ExecuteAsync(request, option);
            return response;
        }


        /// <summary>
        /// 支付分-同步服务订单信息
        /// </summary>        
        public async Task<WeChatPayScoreServiceOrderSyncResponse> ServiceOrderSync(WeChatPayOptions option, WeChatPayScoreServiceOrderSyncViewModel viewModel) {
            var model = new WeChatPayScoreServiceOrderSyncBodyModel {
                AppId = option.AppId,
                ServiceId = viewModel.ServiceId,
                Type = viewModel.Type,
                Detail = new SyncDetail {
                    PaidTime = viewModel.PaidTime
                }
            };

            var request = new WeChatPayScoreServiceOrderSyncRequest {
                OutOrderNo = viewModel.OutOrderNo
            };
            request.SetBodyModel(model);

            var response = await _client.ExecuteAsync(request, option);
            return response;
        }

        /// <summary>
        /// 支付分-创单结单合并
        /// </summary>        
        public async Task<WeChatPayScoreServiceOrderDirectCompleteResponse> ServiceOrderDirectComplete(WeChatPayOptions option, WeChatPayScoreServiceOrderDirectCompleteViewModel viewModel) {
            var model = new WeChatPayScoreServiceOrderDirectCompleteBodyModel {
                AppId = option.AppId,
                ServiceId = viewModel.ServiceId,
                OutOrderNo = viewModel.OutOrderNo,
                ServiceIntroduction = viewModel.ServiceIntroduction,
                PostPayments = new List<PostPayment> {
                   new PostPayment{
                       Name = viewModel.PostPaymentName,
                       Amount = viewModel.PostPaymentAmount,
                       Description = viewModel.PostPaymentDescription,
                       Count = viewModel.PostPaymentCount
                   }
                },
                TimeRange = new TimeRange {
                    StartTime = viewModel.StartTime,
                    EndTime = viewModel.EndTime
                },
                TotalAmount = viewModel.TotalAmount,
                NotifyUrl = viewModel.NotifyUrl,
                OpenId = viewModel.OpenId
            };

            var request = new WeChatPayScoreServiceOrderDirectCompleteRequest();
            request.SetBodyModel(model);

            var response = await _client.ExecuteAsync(request, option);
            return response;
        }

        /// <summary>
        /// 支付分-商户预授权
        /// </summary>
        public async Task<WeChatPayScorePermissionsResponse> Permissions(WeChatPayOptions option, PermissionsViewModel viewModel) {
            var model = new WeChatPayScorePermissionsBodyModel {
                AppId = option.AppId,
                ServiceId = viewModel.ServiceId,
                AuthorizationCode = viewModel.AuthorizationCode,
                NotifyUrl = viewModel.NotifyUrl
            };

            var request = new WeChatPayScorePermissionsRequest();
            request.SetBodyModel(model);

            var response = await _client.ExecuteAsync(request, option);
            return response;
        }

        /// <summary>
        /// 支付分-查询用户授权记录（授权协议号）
        /// </summary>
        public async Task<WeChatPayScorePermissionsQueryForAuthCodeResponse> PermissionsQueryForAuthCode(WeChatPayOptions option, PermissionsQueryForAuthCodeViewModel viewModel) {
            var model = new WeChatPayScorePermissionsQueryForAuthCodeQueryModel {
                ServiceId = viewModel.ServiceId,
            };

            var request = new WeChatPayScorePermissionsQueryForAuthCodeRequest {
                AuthorizationCode = viewModel.AuthorizationCode
            };
            request.SetQueryModel(model);

            var response = await _client.ExecuteAsync(request, option);
            return response;
        }

        /// <summary>
        /// 支付分-解除用户授权关系（授权协议号）
        /// </summary>
        public async Task<WeChatPayScorePermissionsTerminateForAuthCodeResponse> PermissionsTerminateForAuthCode(WeChatPayOptions option, PermissionsTerminateForAuthCodeViewModel viewModel) {
            var model = new WeChatPayScorePermissionsTerminateForAuthCodeBodyModel {
                ServiceId = viewModel.ServiceId,
                Reason = viewModel.Reason
            };

            var request = new WeChatPayScorePermissionsTerminateForAuthCodeRequest {
                AuthorizationCode = viewModel.AuthorizationCode
            };
            request.SetBodyModel(model);

            var response = await _client.ExecuteAsync(request, option);
            return response;
        }


        /// <summary>
        /// 支付分-查询用户授权记录（openid）
        /// </summary>
        public async Task<WeChatPayScorePermissionsQueryForOpenIdResponse> PermissionsQueryForOpenId(WeChatPayOptions option, PermissionsQueryForOpenIdViewModel viewModel) {
            var model = new WeChatPayScorePermissionsQueryForOpenIdQueryModel {
                AppId = option.AppId,
                ServiceId = viewModel.ServiceId,
            };

            var request = new WeChatPayScorePermissionsQueryForOpenIdRequest {
                OpenId = viewModel.OpenId
            };
            request.SetQueryModel(model);

            var response = await _client.ExecuteAsync(request, option);
            return response;
        }

        /// <summary>
        /// 支付分-解除用户授权关系（OpenId）
        /// </summary>
        public async Task<WeChatPayScorePermissionsTerminateForOpenIdResponse> PermissionsTerminateForOpenId(WeChatPayOptions option, PermissionsTerminateForOpenIdViewModel viewModel) {
            var model = new WeChatPayScorePermissionsTerminateForOpenIdBodyModel {
                AppId     = option.AppId,
                ServiceId = viewModel.ServiceId,
                Reason    = viewModel.Reason
            };

            var request = new WeChatPayScorePermissionsTerminateForOpenIdRequest {
                OpenId = viewModel.OpenId
            };
            request.SetBodyModel(model);

            var response = await _client.ExecuteAsync(request, option);
            return response;
        }

        #endregion
    }
}