using Magicube.Data.Abstractions.Attributes;
using Magicube.Net;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;

namespace Magicube.Quartz.Web.Models {
    public class HttpRequestModel {
        [Display(Name = "请求地址")]
        public string                     Url         { get; set; }

        [Display(Name = "请求方式")]
        [DataItems]
        public HttpMethod                 Method      { get; set; }

        [Display(Name = "自定义请求头")]
        public Dictionary<string, string> Headers     { get; set; }

        [Display(Name = "请求参数")]
        public string                     Parameters  { get; set; }

        [Display(Name = "请求体数据类型")]
        public string                     ContentType { get; set; } = Curl.JSON;
    }
}
