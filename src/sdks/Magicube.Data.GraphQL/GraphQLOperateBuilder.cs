using GraphQL;
using Magicube.Data.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Magicube.Data.GraphQL {
	public enum GraphQLOperateType {
		Create,
		Query,
		AddOrUpdate,
		Delete,
		Update,
	}

	public abstract class GraphQLFilter {
		public int Group { get; set; }
		protected abstract string FilterOperate { get; }
	}

	public abstract class JunctionGraphQLFilter : GraphQLFilter {
		public JunctionGraphQLFilter() {
			Children = new List<GraphQLFilter>();
		}
		public List<GraphQLFilter> Children { get; set; }

		public override string ToString() {
			var stringBuilder = new StringBuilder(FilterOperate).Append(" : {");

			stringBuilder.Append(string.Join(", ", Children.Select(x => x.ToString())));

			return stringBuilder.Append(" }").ToString();
		}
	}

	public abstract class FieldValueGraphQLFilter : GraphQLFilter {
		public string Field { get; }
		public object Value { get; }
		public FieldValueGraphQLFilter(string field, object value) {
			Field = string.IsNullOrEmpty(FilterOperate) ? field.ToCamelCase() : $"{field}_{FilterOperate}".ToCamelCase();
			Value = value;
		}

		public override string ToString() {
			return $" {Field} : {Value}";
		}
	}

	public class EqualGraphQLFilter : FieldValueGraphQLFilter {
		public EqualGraphQLFilter(string field, object value) : base(field, value) {
		}

		protected override string FilterOperate => "";
	}

	public class NotEqualGraphQLFilter : FieldValueGraphQLFilter {
		public NotEqualGraphQLFilter(string field, object value) : base(field, value) {
		}

		protected override string FilterOperate => "not";
	}

	public class GtGraphQLFilter : FieldValueGraphQLFilter {
		public GtGraphQLFilter(string field, object value) : base(field, value) { }

		protected override string FilterOperate => "gt";
	}

	public class GteGraphQLFilter : FieldValueGraphQLFilter {
		public GteGraphQLFilter(string field, object value) : base(field, value) { }

		protected override string FilterOperate => "gte";
	}

	public class LtGraphQLFilter : FieldValueGraphQLFilter {
		public LtGraphQLFilter(string field, object value) : base(field, value) { }

		protected override string FilterOperate => "lt";
	}

	public class LteGraphQLFilter : FieldValueGraphQLFilter {
		public LteGraphQLFilter(string field, object value) : base(field, value) { }

		protected override string FilterOperate => "lte";
	}

	public class NotGraphQLFilter : FieldValueGraphQLFilter {
		public NotGraphQLFilter(string field, object value) : base(field, value) {
		}

		protected override string FilterOperate => "not";
	}

	public class InGraphQLFilter : FieldValueGraphQLFilter {
		public InGraphQLFilter(string field, object value) : base(field, value) {
		}

		protected override string FilterOperate => "in";
	}

	public class NotInGraphQLFilter : FieldValueGraphQLFilter {
		public NotInGraphQLFilter(string field, object value) : base(field, value) {
		}

		protected override string FilterOperate => "not_in";
	}

	public class AndGraphQLFilter : JunctionGraphQLFilter {
		protected override string FilterOperate => "and";
	}

	public class OrGraphQLFilter : JunctionGraphQLFilter {
		protected override string FilterOperate => "or";
	}

	public class OrderByGraphQLFilter : JunctionGraphQLFilter {
		protected override string FilterOperate => "orderBy";
	}

	public class GraphQLOperateBuilder {
		private readonly string _entityType;
		private readonly GraphQLOperateType _type;

		private JunctionGraphQLFilter _junctionFilter;
		private OrderByGraphQLFilter _orderByFilter;

		private int _group;
		private int _first;
		private int _skip;

		private readonly List<string> _fields = new List<string>();
		private readonly List<GraphQLFilter> _filters = new List<GraphQLFilter>();

		public GraphQLOperateBuilder(string entityType, GraphQLOperateType type = GraphQLOperateType.Query) {
			_type       = type;
			_entityType = entityType;
		}

		public string EntityType => _entityType.ToCamelCase();

		public GraphQLOperateBuilder AddField(string field) {
			_fields.Add(field.ToCamelCase());
			return this;
		}

		public GraphQLOperateBuilder AddFilter(GraphQLFilter filter) {
			if (_junctionFilter != null) {
				_junctionFilter.Children.Add(filter);
			} else {
				_filters.Add(filter);
			}
			return this;
		}

		public GraphQLOperateBuilder And {
			get {
				Interlocked.Increment(ref _group);
				_junctionFilter = new AndGraphQLFilter() {
					Group = _group
				};
				_filters.Add(_junctionFilter);
				return this;
			}
		}

		public GraphQLOperateBuilder Or {
			get {
				Interlocked.Increment(ref _group);
				_junctionFilter = new OrGraphQLFilter() {
					Group = _group
				};
				_filters.Add(_junctionFilter);
				return this;
			}
		}

		public GraphQLOperateBuilder AddOrder(string field, OrderBy orderBy = OrderBy.Descending) {
			if(_orderByFilter == null) _orderByFilter = new OrderByGraphQLFilter();
			_orderByFilter.Children.Add(new EqualGraphQLFilter(field, orderBy == OrderBy.Ascending ? "\"asc\"" : "\"desc\""));
			return this;
		}

		public GraphQLOperateBuilder AddSkip(int skip) {
			_skip = skip;
			return this;
		}

		public GraphQLOperateBuilder AddTake(int take) {
			_first = take;
			return this;
		}

		public string Build() {
			var stringBuilder = new StringBuilder();

			switch (_type) {
				case GraphQLOperateType.Create:
					stringBuilder.Append($"mutation (${EntityType}:{_entityType}Input)").Append("{ create(").Append(EntityType).Append(":").Append("$").Append(EntityType);
					break;
				case GraphQLOperateType.Query:
					stringBuilder.Append("query { ").Append(EntityType);
					break;
				case GraphQLOperateType.AddOrUpdate:
					stringBuilder.Append($"mutation (${EntityType}:{_entityType}Input)").Append("{ addOrUpdate(").Append(EntityType).Append(":").Append("$").Append(EntityType);
					break;
				case GraphQLOperateType.Delete:
					stringBuilder.Append($"mutation (${EntityType}:{_entityType}Input)").Append("{ delete(").Append(EntityType).Append(":").Append("$").Append(EntityType);
					break;
				case GraphQLOperateType.Update:
					stringBuilder.Append($"mutation (${EntityType}:{_entityType}Input)").Append("{ update(").Append(EntityType).Append(":").Append("$").Append(EntityType);
					break;
				default:
					stringBuilder.Append("query {").Append(EntityType).Append("(");
					break;
			}

			if (_filters.Count > 0) {
				if (_type == GraphQLOperateType.Query) {
					stringBuilder.Append(" (");
				} else {
					stringBuilder.Append(",");
				}
				stringBuilder.Append(" where : {");

				stringBuilder.Append(string.Join(", ", _filters.Select(x => x.ToString())));

				stringBuilder.Append(" }");

				if (_type == GraphQLOperateType.Query) {
					if (_orderByFilter != null) {
						stringBuilder.Append($" {_orderByFilter}");
					}
					if (_first > 0) {
						stringBuilder.Append($" first : {_first}");
					}
					if (_skip > 0) {
						stringBuilder.Append($" skip : {_skip}");
					}
				}
				stringBuilder.Append(" )");
            } else {
				if(_type != GraphQLOperateType.Query) {
					stringBuilder.Append(")");
				}
            }

			stringBuilder.Append(" {");
			foreach (var field in _fields) {
				stringBuilder.Append($" {field.ToCamelCase()}");
			}
			stringBuilder.Append(" }");

			stringBuilder.Append(" }");

			return stringBuilder.ToString();
		}
	}

	public class GraphQLArguementBuilder {
		private readonly string _entityType;
		private readonly Dictionary<string, object> _args = new Dictionary<string, object>();
		public GraphQLArguementBuilder(string entityType) {
			_entityType = entityType;
        }

		public string EntityType => _entityType.ToCamelCase();

		public GraphQLArguementBuilder Add(string key, object value) {
			if (_args.ContainsKey(key)) throw new DataException($"GraphQL 输入参数已包含 {key}");
			_args.Add(key, value);
			return this;
        }

		public static implicit operator Inputs(GraphQLArguementBuilder builder) {
			return new Inputs (new Dictionary<string, object> {
				[builder.EntityType] = builder._args
			});
        }
	}
}
