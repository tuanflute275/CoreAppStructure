namespace CoreAppStructure.Infrastructure.Logging
{
    public static class LogHelper
    {
        // Log lỗi khi có ngoại lệ
        public static void LogError(ILogger logger, Exception ex, string method, string endpoint, object requestData = null)
        {
            if (ex == null) throw new ArgumentNullException(nameof(ex));

            // Kiểm tra null cho requestData
            var requestLogData = requestData ?? "No request data provided";

            // Ghi log lỗi chi tiết
            logger.LogError(ex,
                "Error in {Method} {Endpoint}. Request Data: {RequestData}. Exception: {Message}",
                method, endpoint, requestLogData, ex.Message);
        }

        // Log thông tin thành công của phương thức
        public static void LogInformation(ILogger logger, string method, string endpoint, object requestData = null, object responseData = null)
        {
            // Kiểm tra null cho requestData và responseData
            var requestLogData = requestData ?? "No request data provided";
            var responseLogData = responseData ?? "No response data provided";

            // Ghi log thành công
            logger.LogInformation(
                "Processed {Method} {Endpoint}. Request Data: {RequestData}. Response Data: {ResponseData}",
                method, endpoint, requestLogData, responseLogData);
        }
    }
}
