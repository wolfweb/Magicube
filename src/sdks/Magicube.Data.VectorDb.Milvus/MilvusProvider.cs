using Magicube.Data.Abstractions;
using Microsoft.Extensions.Options;
using Milvus.Client;
using System;

namespace Magicube.Data.VectorDb.Milvus {
    public class MilvusProvider {
        public MilvusProvider(IOptionsMonitor<DatabaseOptions> options) {
            var uri = new Uri(options.CurrentValue.Value);
            var userInfo = uri.UserInfo?.Split(":");
            string user = string.Empty, password = string.Empty;
            if(userInfo.Length > 0 ) {
                user = userInfo[0];
            }
            if (userInfo.Length > 1) {
                password = userInfo[1];
            }

            Database = new MilvusClient(uri.Host, user, password, uri.Port);
        }

        public MilvusClient Database { get; }
    }
}
