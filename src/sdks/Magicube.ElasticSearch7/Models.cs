using Nest;
using System.Collections.Generic;
using Magicube.Core;

namespace Magicube.ElasticSearch7 {
    public class IndexDocMapping {
        public string                   Index            { get; set; }
        public int                      NumberOfShards   { get; set; } = 1;
        public int                      NumberOfReplicas { get; set; } = 1;
        public List<IIndexMappingField> Fields           { get; set; }
    }

    public interface IIndexMappingField {
        PropertiesDescriptor<object> Build(PropertiesDescriptor<object> properties);
    }

    public class BooleanMappingField : BooleanProperty, IIndexMappingField {
        public PropertiesDescriptor<object> Build(PropertiesDescriptor<object> properties) {
            return properties.Boolean(x => {
                x.Name(Name);
                return x;
            });
        }
    }

    public class DateMappingField : DateProperty, IIndexMappingField {
        public PropertiesDescriptor<object> Build(PropertiesDescriptor<object> properties) {
            return properties.Date(x => {
                x.Name(Name);
                return x;
            });
        }
    }

    public class DenseVectorMappingField : DenseVectorProperty, IIndexMappingField {
        public PropertiesDescriptor<object> Build(PropertiesDescriptor<object> properties) {
            return properties.DenseVector(x => { 
                x.Dimensions(Dimensions);
                return x;
            });
        }
    }

    public class KeywordMappingField : KeywordProperty, IIndexMappingField {
        public PropertiesDescriptor<object> Build(PropertiesDescriptor<object> properties) {
            return properties.Keyword(x => {
                x.Name(Name);

                if (Norms.HasValue) x.Norms(Norms);
                if (!Normalizer.IsNullOrEmpty()) x.Normalizer(Normalizer);
                return x;
            });
        }
    }

    public class NumberMappingField : NumberProperty, IIndexMappingField {
        public NumberType Type { get; set; } = NumberType.Float;
        public PropertiesDescriptor<object> Build(PropertiesDescriptor<object> properties) {
            return properties.Number(x => {
                x.Name(Name).Type(Type);
                return x;
            });
        }
    }

    public class TextMappingField : TextProperty, IIndexMappingField {
        public PropertiesDescriptor<object> Build(PropertiesDescriptor<object> properties) {
            return properties.Text(x => {
                x = x.Name(Name);

                if (!SearchAnalyzer.IsNullOrEmpty())
                    x = x.SearchAnalyzer(SearchAnalyzer);

                if (!Analyzer.IsNullOrEmpty())
                    x = x.Analyzer(Analyzer);

                if (!Similarity.IsNullOrEmpty())
                    x.Similarity(Similarity);

                if (Boost > 0)
                    x = x.Boost(Boost);

                return x;
            });
        }
    }
}
