﻿using Nest;

namespace CoreAppStructure.Infrastructure.Logging
{
    public static class LogHelper
    {
        private static IElasticClient _elasticClient;
        static LogHelper()
        {
            var elasticsearchConfig = new ElasticsearchLogConfig();
            _elasticClient = elasticsearchConfig.GetElasticClient();
        }

        // Log lỗi khi có ngoại lệ
        public static async Task LogError(Microsoft.Extensions.Logging.ILogger logger, Exception ex, string method, string endpoint, object requestData = null)
        {
            if (ex == null) throw new ArgumentNullException(nameof(ex));

            // Kiểm tra null cho requestData
            var requestLogData = requestData ?? "No request data provided";

            // Ghi log lỗi chi tiết
            logger.LogError(ex,
                "Error in {Method} {Endpoint}. Request Data: {RequestData}. Exception: {Message}",
                method, endpoint, requestLogData, ex.Message);

            // Tạo log cho Elasticsearch
            var logDocument = new
            {
                timestamp = DateTime.UtcNow,
                level = "ERROR",
                method = method,
                endpoint = endpoint,
                requestData = requestLogData,
                exception = ex.Message,
                stackTrace = ex.StackTrace
            };

            // Gửi log vào Elasticsearch
            await _elasticClient.IndexDocumentAsync(logDocument);
        }

        // Log thông tin thành công của phương thức
        public static async Task LogInformation(Microsoft.Extensions.Logging.ILogger logger, string method, string endpoint, object requestData = null, object responseData = null)
        {
            // Kiểm tra null cho requestData và responseData
            var requestLogData   = requestData ?? "No request data provided";
            var responseLogData  = responseData ?? "No response data provided";

            // Ghi log thành công
            logger.LogInformation(
                "Processed {Method} {Endpoint}. Request Data: {RequestData}. Response Data: {ResponseData}",
                method, endpoint, requestLogData, responseLogData);

            // Tạo log cho Elasticsearch
            var logDocument = new
            {
                timestamp = DateTime.UtcNow,
                level = "INFO",
                method = method,
                endpoint = endpoint,
                requestData = requestLogData,
                responseData = responseLogData
            };

            // Gửi log vào Elasticsearch
            await _elasticClient.IndexDocumentAsync(logDocument);
        }
    }
}
