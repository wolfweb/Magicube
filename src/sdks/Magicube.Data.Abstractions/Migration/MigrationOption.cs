namespace Magicube.Data.Abstractions {
	public class MigrationOption {
		/// <summary>
		/// 用来解析实例
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 用来链接db
		/// </summary>
		public string ConnectionString { get; set; }
		/// <summary>
		/// 用来识别db
		/// </summary>
		public string ConnectionProvider { get; set; }
	}
}
