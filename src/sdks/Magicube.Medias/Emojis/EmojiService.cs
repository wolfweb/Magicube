using AngleSharp;
using Magicube.Media.Images;
using Magicube.Net;
using Magicube.Core;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Magicube.Core.Reflection;
using Configuration = AngleSharp.Configuration;

namespace Magicube.Media.Emojis {
    public class EmojiService {
        private readonly IEnumerable<PropertyInfo> EmojiRawProperties = TypeAccessor.Get<EmojiRawItem>().Context.Properties.Select(x=>x.Member);
        private readonly IEnumerable<PropertyInfo> EmojiProperties    = TypeAccessor.Get<EmojiItem>().Context.Properties.Select(x=>x.Member);
        private readonly List<string> EmojiImageIdx      = new List<string> { "Appl", "Goog", "FB", "Wind", "Twtr", "Joy", "Sams" };
        private readonly EmojiOptions _options;
        private readonly Curl _curl;
        public EmojiService(IOptions<EmojiOptions> options, Curl curl) {
            _options = options.Value;
            _curl    = curl;
        }

        public async Task<List<EmojiItem>> GetEmojis(bool update=false) {
            if (update) {
                return await UpdateEmojis();
            }
            var folder = _options.CachePath;
            var result = new List<EmojiItem>();
            foreach (var file in Directory.EnumerateFiles(folder)) {
                var name = Path.GetFileNameWithoutExtension(file);
                var arrs = name.Split('$');
                var item = result.Find(x => x.Key == arrs[0]);

                if (item == null) {
                    item = new EmojiItem {
                        Key = arrs[0],
                        Code = arrs[0].Replace("_","")
                    };
                    result.Add(item);
                }

                EmojiProperties.First(x => x.Name == arrs[1]).SetValue(item, file);
            }
            return await Task.FromResult(result);
        }

        public async Task<List<EmojiItem>> UpdateEmojis() {
            var html       = await GetLatestEmoji();
            var emojiItems = await ParseEmoji(html);
            return await SaveEmojis(emojiItems);
        }

        private async Task<string> GetLatestEmoji() {
            byte[] bytes = new byte[1024];
            int i = 0;

            if(_options.EmojiHost.StartsWith("http")) {
                Trace.WriteLine($"begin download from {_options.EmojiHost}");
                return await _curl.Get(_options.EmojiHost).ReadAsStream(stream => {
                    using (var mem = new MemoryStream()) {
                        while ((i = stream.Read(bytes, 0, bytes.Length)) > 0) {
                            mem.Write(bytes, 0, i);            
                        }
                        Trace.WriteLine("download complete!");
                        return Encoding.UTF8.GetString(mem.ToArray());
                    }
                });
            } else if(_options.EmojiHost.StartsWith("file:///")){
                return await File.ReadAllTextAsync(_options.EmojiHost.Replace("file:///", ""));
            } else {
                throw new InvalidDataException("EmojiHost not a valid address!");
            }
        }

        private async Task<List<EmojiRawItem>> ParseEmoji(string html) {
            var result = new List<EmojiRawItem>();
            
            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(html));

            var trs = document.QuerySelectorAll("tr");
            Trace.WriteLine($"begin parse emoji data from html!");
            foreach(var tr in trs) {
                var acode = tr.QuerySelector(".code a");
                if (acode != null) {
                    EmojiRawItem item = new EmojiRawItem();
                    var code = acode.Attributes["href"].Value.Trim('#');
                    item.Key = code;
                    item.Code = code.Replace("_", "");

                    Trace.WriteLine($"find emoji {item.Code} ...");

                    var images = tr.QuerySelectorAll(".andr");
                    var j = 0;
                    foreach(var img in images) {
                        if (j > EmojiImageIdx.Count - 1) break;
                        if (images.Length == 1) {
                            var imgE = images.FirstOrDefault(x=>x.TagName == "img");
                            if (imgE != null) {
                                var imgD = imgE.GetAttribute("src");
                                foreach (var it in EmojiImageIdx) {
                                    EmojiRawProperties.First(x => x.Name == it).SetValue(item, imgD);
                                }
                            }
                        } else {
                            var imgE = img.QuerySelector("img");
                            if (imgE != null) {
                                var imgD = imgE.GetAttribute("src");
                                EmojiRawProperties.First(x => x.Name == EmojiImageIdx[j]).SetValue(item, imgD);
                            }
                        }
                        j++;
                    }
                    result.Add(item);
                }
            }
            return await Task.FromResult(result);
        }

        private async Task<List<EmojiItem>> SaveEmojis(List<EmojiRawItem> datas) {
            var folder = _options.CachePath;
            var result = new List<EmojiItem>();
            foreach (var item in datas) {
                var emoji = new EmojiItem {
                    Key  = item.Key,
                    Code = item.Code,
                };
                foreach (var p in EmojiRawProperties) {
                    if (!EmojiImageIdx.Contains(p.Name)) continue;
                    var file = Path.Combine(folder,$"{item.Key}${p.Name}.png");
                    if (!File.Exists(file)) {
                        Trace.WriteLine($"save emoji {item.Key} to {file}");
                        var base64 = p.GetValue(item, null)?.ToString();
                        if (base64.IsNullOrEmpty()) continue;
                        using (var img = ImageService.FromBase64(base64)) {
                            img.Save(file);
                        }
                    }
                    EmojiProperties.First(x => x.Name == p.Name).SetValue(emoji, file);
                }
                result.Add(emoji);
            }
            return await Task.FromResult(result);
        }
    }
}
