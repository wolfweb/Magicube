using OpenCvSharp;
using SixLabors.ImageSharp;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tesseract;


namespace Magicube.Medias.Ocr {
    public enum Languages : byte {
        English = 0,
        Russian = 1,
        Chinese = 2,
        Japanese = 3,
        Korean = 4,

        //Translation only
        Italian = 5,
        French = 6,
        German = 7,
        Spanish = 8,
        Portuguese = 9,
        Vietnamese = 10,
        Turkish = 11,
        Thai = 12,
        Arabic = 13
    }

    public class TesseractOptions {
        public string    Path     { get; set; }
        public Languages Language { get; set; }
    }

    public class TesseractMediaOcr : IMagicubeMediaOcr {
        private TesseractEngine _engine;
        public TesseractMediaOcr(TesseractOptions options) {
            var descriptor = LanguageDescriptors.FirstOrDefault(x => x.Language == options.Language);
            _engine = new TesseractEngine(options.Path, descriptor.TesseractCode, EngineMode.LstmOnly);
            _engine.DefaultPageSegMode = PageSegMode.SingleBlock;

            if (options.Language == Languages.Chinese || options.Language == Languages.Japanese) {
                _engine.SetVariable("preserve_interword_spaces", 1);
                _engine.SetVariable("chop_enable", true);
                _engine.SetVariable("use_new_state_cost", false);
                _engine.SetVariable("segment_segcost_rating", false);
                _engine.SetVariable("enable_new_segsearch", 0);
                _engine.SetVariable("language_model_ngram_on", 0);
                _engine.SetVariable("textord_force_make_prop_words", false);
                _engine.SetVariable("edges_max_children_per_outline", 40);
            }
        }

        public OcrResult Detect(Stream stream) {
            var result = new OcrResult();
            using (var src = Mat.FromStream(stream, ImreadModes.Unchanged)) {
                if (src.Channels() != 1) {
                    Cv2.CvtColor(src, src, ColorConversionCodes.BGR2GRAY);
                }
                var datas = src.ToBytes(".tif");
                using (var img = Pix.LoadTiffFromMemory(datas)) {
                    using (var page = _engine.Process(img)) {
                        using (var iter = page.GetIterator()) {
                            iter.Begin();
                            var list = new List<OcrResultRegion>();
                            do {
                                if (iter.TryGetBoundingBox(PageIteratorLevel.Word, out var bbox)) {
                                    var curText = iter.GetText(PageIteratorLevel.Word);
                                    result.Text += curText;

                                    list.Add(new OcrResultRegion {
                                        Text = curText,
                                        Rect = new RotatedRect(new Point2f(bbox.X1 + bbox.Width / 2, bbox.Y1 + bbox.Height / 2), new Size2f(bbox.Width, bbox.Height), 0)
                                    });
                                }
                            } while (iter.Next(PageIteratorLevel.Word));

                            result.Regions = list;
                        }
                        return result;
                    }
                }
            }
        }

        public OcrResult Detect(byte[] bytes) {
            var result = new OcrResult();
            using (var src = Mat.FromImageData(bytes, ImreadModes.Unchanged)) {
                if (src.Channels() != 1) {
                    Cv2.CvtColor(src, src, ColorConversionCodes.BGR2GRAY);
                }
                var datas = src.ToBytes(".tif");
                using (var img = Pix.LoadTiffFromMemory(datas)) {
                    using (var page = _engine.Process(img)) {
                        using (var iter = page.GetIterator()) {
                            iter.Begin();
                            var list = new List<OcrResultRegion>();
                            do {
                                if (iter.TryGetBoundingBox(PageIteratorLevel.Word, out var bbox)) {
                                    var curText = iter.GetText(PageIteratorLevel.Word);
                                    result.Text += curText;

                                    list.Add(new OcrResultRegion {
                                        Text = curText,
                                        Rect = new RotatedRect(new Point2f(bbox.X1 + bbox.Width / 2, bbox.Y1 + bbox.Height / 2), new Size2f(bbox.Width, bbox.Height), 0)
                                    });
                                }
                            } while (iter.Next(PageIteratorLevel.Word));

                            result.Regions = list;
                        }
                        return result;
                    }
                }
            }
        }

        public void Dispose() {
            _engine?.Dispose();
            _engine = null;
        }

        sealed class LanguageDescriptor {
            public Languages Language                { get; set; }
            public string    Code                    { get; set; }
            public string    TesseractCode           { get; set; }
            public string    EasyOcrCode             { get; set; }
            public string    IsoCode                 { get; set; }
            public string    EasyOcrModel            { get; set; }
            public string    TextScorePredictorModel { get; set; }
            public string[]  SupportedNamedBlocks    { get; set; }
            public bool      UseEndPunctuation       { get; set; }
            public bool      UseWordTokenizer        { get; set; }
            public bool      UseSpaceRemover         { get; set; }
            public bool      TranslationOnly         { get; set; } = false;
            public bool      Asian                   { get; set; } = false;
            public override bool Equals(object obj) {
                var langDesc = obj as LanguageDescriptor;
                if (langDesc is null) {
                    return false;
                }
                return langDesc.Language == this.Language;
            }
            public override int GetHashCode() {
                return Language.GetHashCode();
            }
        }

        static IEnumerable<LanguageDescriptor> LanguageDescriptors = new[] {
            new LanguageDescriptor()
            {
                Language = Languages.English, Code = "en-US", EasyOcrCode = "en", EasyOcrModel = "english_g2",
                TesseractCode = "eng", IsoCode = "en", TextScorePredictorModel = "eng",
                SupportedNamedBlocks = new[] { "IsBasicLatin" }, UseEndPunctuation = true, UseWordTokenizer = false, UseSpaceRemover = false
            },
            new LanguageDescriptor()
            {
                Language = Languages.Russian, Code = "ru-RU", EasyOcrCode = "ru", EasyOcrModel = "cyrillic_g2",
                TesseractCode = "rus", IsoCode = "ru", TextScorePredictorModel = "rus",
                SupportedNamedBlocks = new[] { "IsCyrillic" }, UseEndPunctuation = true, UseWordTokenizer = false, UseSpaceRemover = false
            },
            new LanguageDescriptor()
            {
                Language = Languages.Chinese, Code = "zh-CN", EasyOcrCode = "ch_sim", EasyOcrModel = "zh_sim_g2",
                TesseractCode = "chi_sim", IsoCode = "zh", TextScorePredictorModel = "chi",
                SupportedNamedBlocks = new[] { "IsCJKUnifiedIdeographs" }, UseEndPunctuation = false,
                UseWordTokenizer = true, UseSpaceRemover = true, Asian = true
            },
            new LanguageDescriptor()
            {
                Language = Languages.Japanese, Code = "ja-JP", EasyOcrCode = "ja", EasyOcrModel = "japanese_g2",
                TesseractCode = "jpn", IsoCode = "ja", TextScorePredictorModel = "jap",
                SupportedNamedBlocks = new[] { "IsHiragana", "IsKatakana" }, UseEndPunctuation = false,
                UseWordTokenizer = true, UseSpaceRemover = true, Asian = true
            },
            new LanguageDescriptor()
            {
                Language = Languages.Korean, Code = "ko-KR", EasyOcrCode = "ko", EasyOcrModel = "korean_g2",
                TesseractCode = "kor", IsoCode = "ko", TextScorePredictorModel = "kor",
                SupportedNamedBlocks = new[] { "IsHangulJamo", "IsHangulSyllables" }, UseEndPunctuation = false,
                UseWordTokenizer = true, UseSpaceRemover = false, Asian = true
            },
            new LanguageDescriptor() { Language = Languages.Italian, Code = "it-IT", IsoCode = "it", TranslationOnly = true },
            new LanguageDescriptor() { Language = Languages.French, Code = "fr-FR", IsoCode = "fr", TranslationOnly = true },
            new LanguageDescriptor() { Language = Languages.German, Code = "de-DE", IsoCode = "de", TranslationOnly = true },
            new LanguageDescriptor() { Language = Languages.Spanish, Code = "es-ES", IsoCode = "es", TranslationOnly = true },
            new LanguageDescriptor() { Language = Languages.Portuguese, Code = "pt-PT", IsoCode = "pt", TranslationOnly = true },
            new LanguageDescriptor() { Language = Languages.Vietnamese, Code = "vi-VN", IsoCode = "vi", TranslationOnly = true },
            new LanguageDescriptor() { Language = Languages.Turkish, Code = "tr-TR", IsoCode = "tr", TranslationOnly = true },
            new LanguageDescriptor() { Language = Languages.Thai, Code = "th-TH", IsoCode = "th", TranslationOnly = true },
            new LanguageDescriptor() { Language = Languages.Arabic, Code = "ar-SA", IsoCode = "ar", TranslationOnly = true },
        };
    }
}