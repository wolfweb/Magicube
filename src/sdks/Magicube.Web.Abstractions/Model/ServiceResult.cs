namespace Magicube.Web {
    public class ServiceResult {
		public int    Code    { get; set; }
		public object Data    { get; set; }
		public string Message { get; set; }

        public ServiceResult(int code) : this(code, null) { }

        public ServiceResult(int code, string message) {
            Code    = code;
            Message = message;
        }
    }

    public class ServiceResult<T> : ServiceResult {
        public new T Data { get; set; }

        public ServiceResult(T data) : this(200, data, null) { }

        public ServiceResult(int code, T data) : this(code, data, null) { }

        public ServiceResult(int code, T data, string message) : base(code, message) {
            Data = data;
        }
    }
}
