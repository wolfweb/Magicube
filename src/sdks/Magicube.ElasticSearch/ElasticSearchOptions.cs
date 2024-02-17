using System.ComponentModel.DataAnnotations;

namespace Magicube.ElasticSearch {
    public class ElasticSearchOptions {
        [Display(Name = "搜索服务地址", Prompt = "例如：http://{user}:{pwd}@{domain}:{port|9200}/[带访问凭证]/http://{domain}:{port|9200}[不带验证]")]
        public string[] Hosts { get; set; }

        [Display(Name = "开启调试", Prompt = "启用调试后可以显示搜索查询日志")]
        public bool     Debug { get; set; }

        [Display(Name = "NLP服务地址")]
        public string   NlpHost { get; set; }
    }
}
