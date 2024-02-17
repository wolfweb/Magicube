using Magicube.Pay.Abstractions.Entities;

namespace Magicube.Pay.Alipay.Entities {
    public class AlipayEntity : PayEntity {
        public AlipayEntity() {
            AppId = "2017030105970078";

            RsaPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAnvQly6ShtK1Yo6VQB79vvaCa/hStLddKBwv8UKTDpukk0+TwvoSmolx7b6TFVWshPQjNWL5Pc58RUIGqi3LsGCS0fb3+rs/o43UFsRSh2u6wsLZzyhw2SMpXmVUhZIGC22qENWwf15uZn/UJ6ySvzTOE9vlR9Xu7rlUb8SoZaeuK+X1OGucB0r1z2FERl/AyG1PWW3e/OljwE07dpc2xZ/yVHDK8K8CkRHFGJZo3l+EfgJ6/EowacE2O5dw09itkar3BOTuj1McchQx62hIUjvtQ3hkJBI/78uENxwWWiqUmBngTyukEYUqbkCMqKq4YBJSdAS/zJ1yOiWm3aRWbPwIDAQAB";

            RsaPrivateKey = "MIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQCNhARXCUA/gLoNuz5Hbrzn+dkrzJsIvklQIbPk6ElW3ND4floi8BuXZxHydWIxTNeIhbcQfGLeMqNnBsSEeZgBKyiVtmiynvq+JlC057+kl4xftDQ34IRzSVLGe7XACNJ6XvGrSDRauQAdLxk+70A+8ClHhnnccv8dCEgQWm9KAdaQF+NDi2h35xQImNo16BmjsTV26rvnsFmqg8y0pXIypGyJcL7O/qqhYRPRaOHutZZg5UMnKIEGtCyhSWvDYTW+vXRkdVgM+Am7et4Au+M9/DjTYfmkKBjlHSu/aa97FbXUF4FqhqfQAlrFdIS7krR8iMmKJw+1WFsTiLllXG03AgMBAAECggEAdZV53a+sGJem//3Ss5vJjp5E2mW5c7xYQiKezNZNCcWURMUHs/v5kQVtHh52piwI8kDYaPRRWfqNVu+CnehCasYnhK35tGDwSsqfSJE+5U2er26V7M+6fegwxQbJb3RlHpSBDSaDidYpyBX4ThiRMzptsiaEXq2WV9MnvZ7G1aAQNC6XM1Ie8TBJUA1PjSmx/qHMzgZQoaL+DwI+0VhxwBPxkoT0a9Ctw8RhW8HrGpqOCZtXWWzhfkYP8YXLClIq0IdGFXcrSl6sYhMP/K/QunzvPhy63IP3aHylLXYocEHpwj/eVwPBHv7bDjJnq3UOzwemYvanAizYsZuaaYMtwQKBgQDM5z7AfPw5ZdgbRuhndn3Ejv7oOkDjMlnkhY87Zym+BO9JVE212bMeaQ4tOl62r0RcDVMPHxcS8dBz97ZrPnOvJJYy9Xl/GokpbX4pjK26z6VVRS2D4nygKJ0yqDQlwPO006rkRvZnpxZWdkgdECgOcysI743ugtIWVT6G8Cr/cQKBgQCwzjDfcfptQIdntCWV6Rv5+EFOUa6Z30PSyuW9oxYfy9ufkkP1OAdE7b45ZYVAirhCm/ad4Wd6C6O/BTiRGZMY5pXVsAYe6aRaiskt5XV+T7efU5cdeKMhzJ/zxxzp2wfAk0+RIyPSNrS9XOwfBIUf5EV/MDRrnRxzESSFfqwzJwKBgErgkkjrI4yiD/ff0JvGbC1BCBu69e1QgBMHT5EooNNkoEDOwtsaY99QCrMyalwd7kApSlnzRY5RuZg3RH0qREITf6O9mpl9C/SMo5bxZqcmrEdd0UUppdstHEzftLa7LRO+aeujlvXH1FziOnYMamblZSuNxvMK8VTh78iFyWCxAoGAaJzHrB4rd6M7uu+LYaT5CbGLKoUIE0FYRwwIXWl2uL+NJmCL1zccjftBl4JrEqJBnh/cDtSEvmDOtUKokqoYMB6XP0WUiYvi1DPUD1T6bQ9L7XivLB/qHiCN79a7U99mOdqvtIefNU1rKbQmfb/V6OXO/Q+PpLDkWW0mN9qYlvcCgYAKjtxEvdLkcrKr5vy21OPbVtPzMxSFuMrU9HFx6TaRVcnPOWprAfQonrvXoy3uxaAqqgPjzGT3RN6K8pF0tWO7TytDuKXMI3MfOOqdAqwHtUUFjXQBTZE3S2xcbPq48l/YUMRVyaRC3dBw+HOUSouMLsbkHd8v7iDIvFUsG2Z4RA==";
        }

        /// <summary>
        /// 服务网关地址
        /// </summary>
        public string ServerHost     { get; set; } = "https://openapi.alipay.com/gateway.do";

        /// <summary>
        /// 数据格式
        /// </summary>
        public string Format         { get; set; } = "json";

        /// <summary>
        /// 接口版本
        /// </summary>
        public string Version        { get; set; } = "1.0";

        /// <summary>
        /// 编码格式
        /// </summary>
        public string Charset        { get; set; } = "utf-8";

        /// <summary>
        /// 签名方式
        /// </summary>
        public string SignType       { get; set; } = "RSA2";

        /// <summary>
        /// 加密方式
        /// </summary>
        public string EncryptType    { get; set; } = "AES";

        /// <summary>
        /// 加密秘钥
        /// </summary>
        public string EncryptKey     { get; set; } = "fmNaIb1iO9jkFJfqWImwgg==";

        /// <summary>
        /// 支付宝根证书
        /// </summary>
        public string AlipayRootCert { get; set; }
    }
}
