using CoreAppStructure.Features.Users.Models;
using Newtonsoft.Json;

namespace CoreAppStructure.Core.WebSocket
{
    public static class WebSocketHandler
    {
        // Một danh sách để theo dõi các kết nối WebSocket
        private static List<System.Net.WebSockets.WebSocket> _connections = new List<System.Net.WebSockets.WebSocket>();
        private static List<string> _userIds = new List<string>(); // Để theo dõi các userId của người dùng đã kết nối

        // Truyền thông tin về số người dùng online cho tất cả các kết nối WebSocket
        public static async Task NotifyUserCountAsync()
        {
            var onlineUserCount = _connections.Count;
            var message = JsonConvert.SerializeObject(new { onlineUserCount });

            foreach (var socket in _connections)
            {
                if (socket.State == WebSocketState.Open)
                {
                    var buffer = System.Text.Encoding.UTF8.GetBytes(message);
                    await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        // Theo dõi các kết nối và cập nhật thông tin khi có người dùng mới hoặc ngắt kết nối
        public static async Task TrackUserActivity(HttpContext context, System.Net.WebSockets.WebSocket webSocket)
        {
            _connections.Add(webSocket); // Thêm người dùng mới vào danh sách kết nối
            _userIds.Add(context.Request.Query["userId"].ToString());

            await NotifyUserCountAsync(); // Thông báo số người dùng online cho tất cả

            // Chờ đợi dữ liệu từ client
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = null;

            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        break;
                    }
                }
            }
            catch (Exception)
            {
                // Nếu có lỗi, đóng kết nối và xóa người dùng khỏi danh sách
                _connections.Remove(webSocket);
                _userIds.Remove(context.Request.Query["userId"]);
                await NotifyUserCountAsync();
            }

            _connections.Remove(webSocket); // Xóa kết nối khi ngắt kết nối
            _userIds.Remove(context.Request.Query["userId"]);
            await NotifyUserCountAsync(); // Cập nhật số người dùng online sau khi một kết nối ngắt
        }

        // Lấy số người dùng online
        public static int GetOnlineUserCount()
        {
            return _connections.Count;
        }

    }
}
