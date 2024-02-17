using Magicube.Data.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicube.Pay.Abstractions.Entities {
    public class PayEntity : Entity {
        /// <summary>
        /// 支付提供者
        /// </summary>
        public string Provider      { get; set; }

        /// <summary>
        /// 应用id
        /// </summary>
        public string AppId         { get; set; }
        
        /// <summary>
        /// 公钥
        /// </summary>
        public string RsaPublicKey  { get; set; }

        /// <summary>
        /// 密钥
        /// </summary>
        public string RsaPrivateKey { get; set; }        
    }
}
