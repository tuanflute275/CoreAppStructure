namespace CoreAppStructure.Core.Helpers
{
    public class ResponseObject
    {
        public int status { get; set; }
        public string message { get; set; }
        public object? data { get; set; }

        public ResponseObject(int code, string message)
        {
            this.status = code;
            this.message = message;
        }

        public ResponseObject(int code, string message, object? data)
        {
            this.status = code;
            this.message = message;
            this.data = data;
        }
    }
}
