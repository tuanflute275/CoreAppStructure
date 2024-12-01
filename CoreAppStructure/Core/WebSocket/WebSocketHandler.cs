namespace CoreAppStructure.Core.WebSocket
{
    public static class WebSocketHandler
    {
        // Một danh sách để theo dõi các kết nối WebSocket
        private static List<System.Net.WebSockets.WebSocket> _connections = new List<System.Net.WebSockets.WebSocket>();
        private static List<string> _userIds = new List<string>(); // Để theo dõi các userId của người dùng đã kết nối
        private static decimal _totalUserAccessing = 0;
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
        public static async Task TrackUserActivity(HttpContext context, System.Net.WebSockets.WebSocket webSocket, ApplicationDbContext ctx)
        {
            // tăng tổng số lượng người dùng 
            _totalUserAccessing++;
                // Tìm tham số TOTAL_USER_ACCESSING trong cơ sở dữ liệu
                var parameter = await ctx.Parameters
                    .FirstOrDefaultAsync(p => p.ParaName == "TOTAL_USER_ACCESSING" && p.ParaScope == "SYSTEM");

                if (parameter != null)
                {
                    // Cập nhật tham số nếu đã tồn tại
                    parameter.ParaShortValue = (Decimal.Parse(parameter.ParaShortValue) + 1).ToString();
                    parameter.ParaLobValue = (Decimal.Parse(parameter.ParaLobValue) + 1).ToString();
                    parameter.UpdateDatetime = DateTime.Now; 
                }
                else
                {
                    // Nếu chưa tồn tại, tạo mới tham số
                    parameter = new Parameter
                    {
                        ParaScope = "SYSTEM",
                        ParaName = "TOTAL_USER_ACCESSING",
                        ParaType = ParaType.Number,
                        ParaDesc = "Tổng số người dùng đã truy cập trang web",
                        ParaShortValue = _totalUserAccessing.ToString(),
                        ParaLobValue = _totalUserAccessing.ToString(),
                        CreateBy = "SYSTEM",
                        CreateDatetime = DateTime.Now,
                    };

                    ctx.Parameters.Add(parameter); // Thêm tham số mới vào cơ sở dữ liệu
                

                }
                // Lưu thay đổi vào cơ sở dữ liệu
                await ctx.SaveChangesAsync();

            //
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
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        // Xử lý dữ liệu nhận được từ client (ví dụ: thông báo giảm giá)
                        string message = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine($"Nhận dữ liệu từ client: {message}");
                    }
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        //await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
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
        public static async Task SendPromoNotification(string promoMessage)
        {
            var promoMessageBytes = System.Text.Encoding.UTF8.GetBytes(promoMessage);
            foreach (var webSocket in _connections)
            {
                if (webSocket.State == WebSocketState.Open)
                {
                    await webSocket.SendAsync(new ArraySegment<byte>(promoMessageBytes, 0, promoMessageBytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}
