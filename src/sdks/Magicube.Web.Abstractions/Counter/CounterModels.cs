using System;

namespace Magicube.Web.Counter {
	public interface ICounterObject {
		string Path { get; }
		string Name { get; }
	}

	public class CounterObject : ICounterObject {
		public CounterObject(string name) {
			Name = name;
		}
		public         string Name  { get; }
		public virtual string Path  => Name;
		public         int    Count { get; set; } = 1;
	}

	public class CounterObject<T> : CounterObject {
		public CounterObject(T key, string name) : base(name) {
			Key = key;
		}
		public          T      Key  { get; }
		public override string Path => $"{Name}:{Key}";
	}

	public class CounterProperty {
		public CounterProperty(string name, ICounterObject t) {
			Name          = name;
			CounterObject = t;
		}
		public         ICounterObject CounterObject { get; }
		public         string         Name          { get; }
										            
		public virtual string         Path          => $"{CounterObject.Name}:{Name}";
							          			    
		public virtual string         RankingPath   => $"{CounterObject.Name}:{Name}:{DateTime.UtcNow.DayOfYear}";
	}
}
