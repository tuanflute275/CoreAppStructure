using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;

namespace CoreAppStructure.Infrastructure.Caching
{
    public class RedisCacheService
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        private readonly string _connectionString;
        private readonly IMemoryCache _memoryCache;

        public RedisCacheService(string connectionString, IMemoryCache memoryCache)
        {
            _connectionString = connectionString;
            _redis = ConnectToRedis(connectionString);
            _database = _redis?.GetDatabase();
            _memoryCache = memoryCache;
        }

        private ConnectionMultiplexer ConnectToRedis(string connectionString)
        {
            try
            {
                var connection = ConnectionMultiplexer.Connect(connectionString);
                Console.WriteLine("Kết nối Redis thành công.");
                return connection;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Không thể kết nối Redis: {ex.Message}");
                return null; // Nếu không kết nối được, trả về null
            }
        }

        // Lưu trữ vào Redis hoặc MemoryCache
        public async Task SetCacheAsync(string key, string value, TimeSpan? expiration = null)
        {
            if (_database != null) // Sử dụng Redis nếu có kết nối
            {
                await _database.StringSetAsync(key, value, expiration);
            }
            else // Nếu không có kết nối Redis, sử dụng MemoryCache
            {
                _memoryCache.Set(key, value, expiration ?? TimeSpan.FromMinutes(30));
            }
        }

        // Lấy dữ liệu từ Redis hoặc MemoryCache
        public async Task<string> GetCacheAsync(string key)
        {
            if (_database != null) // Sử dụng Redis nếu có kết nối
            {
                return await _database.StringGetAsync(key);
            }
            else // Nếu không có kết nối Redis, lấy từ MemoryCache
            {
                _memoryCache.TryGetValue(key, out string value);
                return value;
            }
        }

        // Xóa dữ liệu khỏi Redis hoặc MemoryCache
        public async Task RemoveCacheAsync(string key)
        {
            if (_database != null)
            {
                await _database.KeyDeleteAsync(key);
            }
            else
            {
                _memoryCache.Remove(key);
            }
        }

        // Kiểm tra sự tồn tại của cache trong Redis hoặc MemoryCache
        public async Task<bool> ExistsCacheAsync(string key)
        {
            if (_database != null)
            {
                return await _database.KeyExistsAsync(key);
            }
            else
            {
                return _memoryCache.TryGetValue(key, out _);
            }
        }
    }

}
