using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicube.Pay.Wechatpay.ViewModels {
    public class WeChatPayMicroPayBaseViewModel {
        [Required]
        [Display(Name = "out_trade_no")]
        public string OutTradeNo { get; set; }

        [Required]
        [Display(Name = "body")]
        public string Body { get; set; }

        [Required]
        [Display(Name = "total_fee")]
        public int TotalFee { get; set; }

        [Required]
        [Display(Name = "spbill_create_ip")]
        public string SpBillCreateIp { get; set; }
    }

    public class WeChatPayMicroPayViewModel : WeChatPayMicroPayBaseViewModel {
        [Required]
        [Display(Name = "auth_code")]
        public string AuthCode { get; set; }
    }

    public class WeChatPayPubPayViewModel : WeChatPayMicroPayBaseViewModel {
        [Required]
        [Display(Name = "notify_url")]
        public string NotifyUrl { get; set; }

        [Required]
        [Display(Name = "trade_type")]
        public string TradeType { get; set; }

        [Required]
        [Display(Name = "openid")]
        public string OpenId { get; set; }
    }

    public class WeChatPayQrCodePayViewModel : WeChatPayMicroPayBaseViewModel {
        [Required]
        [Display(Name = "notify_url")]
        public string NotifyUrl { get; set; }

        [Required]
        [Display(Name = "trade_type")]
        public string TradeType { get; set; }

        [Display(Name = "profit_sharing")]
        public string ProfitSharing { get; set; }
    }

    public class WeChatPayH5PayViewModel : WeChatPayMicroPayBaseViewModel{
        [Required]
        [Display(Name = "notify_url")]
        public string NotifyUrl { get; set; }

        [Required]
        [Display(Name = "trade_type")]
        public string TradeType { get; set; }
    }

    public class WeChatPayMiniProgramPayViewModel : WeChatPayMicroPayBaseViewModel{
        [Required]
        [Display(Name = "notify_url")]
        public string NotifyUrl { get; set; }

        [Required]
        [Display(Name = "trade_type")]
        public string TradeType { get; set; }

        [Required]
        [Display(Name = "openid")]
        public string OpenId { get; set; }
    }

    public class WeChatPayOrderQueryViewModel {
        [Display(Name = "transaction_id")]
        public string TransactionId { get; set; }

        [Display(Name = "out_trade_no")]
        public string OutTradeNo { get; set; }
    }

    public class WeChatPayReverseViewModel {
        [Display(Name = "transaction_id")]
        public string TransactionId { get; set; }

        [Display(Name = "out_trade_no")]
        public string OutTradeNo { get; set; }
    }

    public class WeChatPayCloseOrderViewModel {
        [Required]
        [Display(Name = "out_trade_no")]
        public string OutTradeNo { get; set; }
    }

    public class WeChatPayRefundViewModel {
        [Required]
        [Display(Name = "out_refund_no")]
        public string OutRefundNo { get; set; }

        [Display(Name = "transaction_id")]
        public string TransactionId { get; set; }

        [Display(Name = "out_trade_no")]
        public string OutTradeNo { get; set; }

        [Required]
        [Display(Name = "total_fee")]
        public int TotalFee { get; set; }

        [Required]
        [Display(Name = "refund_fee")]
        public int RefundFee { get; set; }

        [Display(Name = "refund_desc")]
        public string RefundDesc { get; set; }

        [Display(Name = "notify_url")]
        public string NotifyUrl { get; set; }
    }

    public class WeChatPayRefundQueryViewModel {
        [Display(Name = "refund_id")]
        public string RefundId { get; set; }

        [Display(Name = "out_refund_no")]
        public string OutRefundNo { get; set; }

        [Display(Name = "transaction_id")]
        public string TransactionId { get; set; }

        [Display(Name = "out_trade_no")]
        public string OutTradeNo { get; set; }
    }

    public class WeChatPayDownloadBillViewModel {
        [Required]
        [Display(Name = "bill_date")]
        public string BillDate { get; set; }

        [Required]
        [Display(Name = "bill_type")]
        public string BillType { get; set; }

        [Display(Name = "tar_type")]
        public string TarType { get; set; }
    }

    public class WeChatPayDownloadFundFlowViewModel {
        [Required]
        [Display(Name = "bill_date")]
        public string BillDate { get; set; }

        [Required]
        [Display(Name = "account_type")]
        public string AccountType { get; set; }

        [Display(Name = "tar_type")]
        public string TarType { get; set; }
    }

    public class WeChatPayTransfersViewModel {
        [Required]
        [Display(Name = "partner_trade_no")]
        public string PartnerTradeNo { get; set; }

        [Required]
        [Display(Name = "openid")]
        public string OpenId { get; set; }

        [Required]
        [Display(Name = "check_name")]
        public string CheckName { get; set; }

        [Display(Name = "re_user_name")]
        public string ReUserName { get; set; }

        [Required]
        [Display(Name = "amount")]
        public int Amount { get; set; }

        [Required]
        [Display(Name = "desc")]
        public string Desc { get; set; }

        [Required]
        [Display(Name = "spbill_create_ip")]
        public string SpBillCreateIp { get; set; }
    }

    public class WeChatPayGetTransferInfoViewModel {
        [Required]
        [Display(Name = "partner_trade_no")]
        public string PartnerTradeNo { get; set; }
    }

    public class WeChatPayPayBankViewModel {
        [Required]
        [Display(Name = "partner_trade_no")]
        public string PartnerTradeNo { get; set; }

        [Required]
        [Display(Name = "bank_no")]
        public string BankNo { get; set; }

        [Required]
        [Display(Name = "true_name")]
        public string TrueName { get; set; }

        [Required]
        [Display(Name = "bank_code")]
        public string BankCode { get; set; }

        [Required]
        [Display(Name = "amount")]
        public int Amount { get; set; }

        [Display(Name = "desc")]
        public string Desc { get; set; }
    }

    public class WeChatPayQueryBankViewModel {
        [Required]
        [Display(Name = "partner_trade_no")]
        public string PartnerTradeNo { get; set; }
    }

    public class WeChatPayProfitSharingAddReceiverViewModel {
        [Required]
        [Display(Name = "receiver")]
        public string Receiver { get; set; }
    }

    public class WeChatPayProfitSharingViewModel {
        [Required]
        [Display(Name = "transaction_id")]
        public string TransactionId { get; set; }

        [Required]
        [Display(Name = "out_order_no")]
        public string OutOrderNo { get; set; }

        [Required]
        [Display(Name = "receivers")]
        public string Receivers { get; set; }
    }

    public class WeChatPayV3RefundViewModel {
        [Required]
        [Display(Name = "out_refund_no")]
        public string OutRefundNo { get; set; }

        [Display(Name = "transaction_id")]
        public string TransactionId { get; set; }

        [Display(Name = "out_trade_no")]
        public string OutTradeNo { get; set; }

        [Display(Name = "notify_url")]
        public string NotifyUrl { get; set; }

        [Required]
        [Display(Name = "amount.refund")]
        public int RefundAmount { get; set; }

        [Required]
        [Display(Name = "amount.total")]
        public int TotalAmount { get; set; }

        [Required]
        [Display(Name = "currency")]
        public string Currency { get; set; }
    }

    public class WeChatPayV3RefundQueryViewModel {
        [Display(Name = "refund_id")]
        public string RefundId { get; set; }

        [Display(Name = "out_refund_no")]
        public string OutRefundNo { get; set; }

        [Display(Name = "transaction_id")]
        public string TransactionId { get; set; }

        [Display(Name = "out_trade_no")]
        public string OutTradeNo { get; set; }
    }


    public class WeChatPayAppPayV3ViewModel {
        [Required]
        [Display(Name = "out_trade_no")]
        public string OutTradeNo { get; set; }

        [Required]
        [Display(Name = "description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "total")]
        public int Total { get; set; }

        [Required]
        [Display(Name = "notify_url")]
        public string NotifyUrl { get; set; }
    }

    public class WeChatPayPubPayV3ViewModel : WeChatPayAppPayV3ViewModel {
        [Required]
        [Display(Name = "openid")]
        public string OpenId { get; set; }
    }

    public class WeChatPayQrCodePayV3ViewModel : WeChatPayAppPayV3ViewModel {
        
    }

    public class WeChatPayH5PayV3ViewModel : WeChatPayAppPayV3ViewModel {
        
    }

    public class WeChatPayMiniProgramPayV3ViewModel : WeChatPayAppPayV3ViewModel {
        [Required]
        [Display(Name = "openid")]
        public string OpenId { get; set; }
    }

    public class WeChatPayQueryByTransactionIdViewModel {
        [Required]
        [Display(Name = "transaction_id")]
        public string TransactionId { get; set; }
    }

    public class WeChatPayQueryByOutTradeNoViewModel {
        [Required]
        [Display(Name = "out_trade_no")]
        public string OutTradeNo { get; set; }
    }

    public class WeChatPayOutTradeNoCloseViewModel {
        [Required]
        [Display(Name = "out_trade_no")]
        public string OutTradeNo { get; set; }
    }

    public class WeChatPayTradeBillViewModel {
        [Required]
        [Display(Name = "bill_date")]
        public string BillDate { get; set; }
    }

    public class WeChatPayFundflowBillViewModel {
        [Required]
        [Display(Name = "bill_date")]
        public string BillDate { get; set; }
    }

    public class WeChatPayBillDownloadViewModel {
        [Required]
        [Display(Name = "download_url")]
        public string DownloadUrl { get; set; }
    }

    #region 微信支付分

    public class WeChatPayScoreServiceOrderViewModel {
        [Required]
        [Display(Name = "service_id")]
        public string ServiceId { get; set; }

        [Required]
        [Display(Name = "out_order_no")]
        public string OutOrderNo { get; set; }

        [Required]
        [Display(Name = "service_introduction")]
        public string ServiceIntroduction { get; set; }

        [Required]
        [Display(Name = "start_time")]
        public string StartTime { get; set; }

        [Required]
        [Display(Name = "end_time")]
        public string EndTime { get; set; }

        [Required]
        [Display(Name = "risk_fund_name")]
        public string RiskFundName { get; set; }

        [Required]
        [Display(Name = "risk_fund_amount")]
        public long RiskFundAmount { get; set; }

        [Required]
        [Display(Name = "notify_url")]
        public string NotifyUrl { get; set; }

        [Display(Name = "openid")]
        public string OpenId { get; set; }
    }

    public class WeChatPayScoreServiceOrderQueryViewModel {
        [Required]
        [Display(Name = "service_id")]
        public string ServiceId { get; set; }

        [Display(Name = "out_order_no")]
        public string OutOrderNo { get; set; }

        [Display(Name = "query_id")]
        public string QueryId { get; set; }

    }

    public class WeChatPayScoreServiceOrderCancelViewModel {
        [Required]
        [Display(Name = "service_id")]
        public string ServiceId { get; set; }

        [Required]
        [Display(Name = "out_order_no")]
        public string OutOrderNo { get; set; }

        [Required]
        [Display(Name = "reason")]
        public string Reason { get; set; }
    }

    public class WeChatPayScoreServiceOrderModifyViewModel {
        [Required]
        [Display(Name = "service_id")]
        public string ServiceId { get; set; }

        [Required]
        [Display(Name = "out_order_no")]
        public string OutOrderNo { get; set; }

        [Required]
        [Display(Name = "post_payment_name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "post_payment_amount")]
        public long Amount { get; set; }

        [Required]
        [Display(Name = "post_payment_count")]
        public uint Count { get; set; }

        [Required]
        [Display(Name = "total_amount")]
        public long TotalAmount { get; set; }

        [Required]
        [Display(Name = "reason")]
        public string Reason { get; set; }
    }

    public class WeChatPayScoreServiceOrderCompleteViewModel {
        [Required]
        [Display(Name = "service_id")]
        public string ServiceId { get; set; }

        [Required]
        [Display(Name = "out_order_no")]
        public string OutOrderNo { get; set; }

        [Required]
        [Display(Name = "post_payment_name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "post_payment_amount")]
        public long Amount { get; set; }

        [Required]
        [Display(Name = "post_payment_count")]
        public uint Count { get; set; }

        [Required]
        [Display(Name = "total_amount")]
        public long TotalAmount { get; set; }
    }

    public class WeChatPayScoreServiceOrderPayViewModel {
        [Required]
        [Display(Name = "service_id")]
        public string ServiceId { get; set; }

        [Required]
        [Display(Name = "out_order_no")]
        public string OutOrderNo { get; set; }
    }

    public class WeChatPayScoreServiceOrderSyncViewModel {
        [Required]
        [Display(Name = "service_id")]
        public string ServiceId { get; set; }

        [Required]
        [Display(Name = "out_order_no")]
        public string OutOrderNo { get; set; }

        [Required]
        [Display(Name = "type")]
        public string Type { get; set; }

        [Required]
        [Display(Name = "paid_time")]
        public string PaidTime { get; set; }
    }


    public class WeChatPayScoreServiceOrderDirectCompleteViewModel {
        [Required]
        [Display(Name = "service_id")]
        public string ServiceId { get; set; }

        [Required]
        [Display(Name = "out_order_no")]
        public string OutOrderNo { get; set; }

        [Required]
        [Display(Name = "service_introduction")]
        public string ServiceIntroduction { get; set; }

        [Required]
        [Display(Name = "post_payment_name")]
        public string PostPaymentName { get; set; }

        [Required]
        [Display(Name = "post_payment_amount")]
        public long PostPaymentAmount { get; set; }

        [Required]
        [Display(Name = "post_payment_count")]
        public uint PostPaymentCount { get; set; }

        [Required]
        [Display(Name = "post_payment_description")]
        public string PostPaymentDescription { get; set; }


        [Required]
        [Display(Name = "start_time")]
        public string StartTime { get; set; }

        [Required]
        [Display(Name = "end_time")]
        public string EndTime { get; set; }

        [Required]
        [Display(Name = "total_amount")]
        public long TotalAmount { get; set; }

        [Display(Name = "notify_url")]
        public string NotifyUrl { get; set; }

        [Display(Name = "openid")]
        public string OpenId { get; set; }
    }

    public class PermissionsViewModel {
        [Required]
        [Display(Name = "service_id")]
        public string ServiceId { get; set; }

        [Required]
        [Display(Name = "authorization_code")]
        public string AuthorizationCode { get; set; }

        [Display(Name = "notify_url")]
        public string NotifyUrl { get; set; }
    }

    public class PermissionsQueryForAuthCodeViewModel {
        [Required]
        [Display(Name = "service_id")]
        public string ServiceId { get; set; }

        [Required]
        [Display(Name = "authorization_code")]
        public string AuthorizationCode { get; set; }
    }

    public class PermissionsTerminateForAuthCodeViewModel {
        [Required]
        [Display(Name = "service_id")]
        public string ServiceId { get; set; }

        [Required]
        [Display(Name = "authorization_code")]
        public string AuthorizationCode { get; set; }

        [Required]
        [Display(Name = "reason")]
        public string Reason { get; set; }
    }

    public class PermissionsQueryForOpenIdViewModel {
        [Required]
        [Display(Name = "service_id")]
        public string ServiceId { get; set; }

        [Required]
        [Display(Name = "openid")]
        public string OpenId { get; set; }
    }

    public class PermissionsTerminateForOpenIdViewModel {
        [Required]
        [Display(Name = "service_id")]
        public string ServiceId { get; set; }

        [Required]
        [Display(Name = "openid")]
        public string OpenId { get; set; }

        [Required]
        [Display(Name = "reason")]
        public string Reason { get; set; }
    }

    #endregion
}
