namespace CoreAppStructure.Core.Exceptions
{
    public class CustomException : Exception
    {
        public int StatusCode { get; set; }
        public string Detail { get; set; }

        public CustomException(string message, int statusCode = 500, string detail = null)
            : base(message)
        {
            StatusCode = statusCode;
            Detail = detail;
        }
    }
}
