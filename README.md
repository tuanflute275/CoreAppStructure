# CLI VS code

> Add-Migration v1 -OutputDir Data\Migrations
> Update-Database

# INSERT SQL Parameters

	INSERT INTO Parameters(ParaScope, ParaName, ParaShortValue, ParaLobValue, ParaDesc, ParaType, SystemId, UserAccessibleFlag, AdminAccessibleFlag)
	values 
	('WORKFLOW','NORMAL_RETRY_INTERVAL', '15',null, N'Khoảng thời gian thử lại sau vài giây cho luồng công việc khi gặp phải lỗi', 'INTEGER', 'SYSTEM', 'Y', 'Y'),
	('SYSTEM','NATIONALITY', 'NAT',null, N'Mã quốc gia', 'CODE', 'SYSTEM', 'Y', 'Y'),
	('SYSTEM','PORT_SERVER', 'http://192.168.10.10:8280',null, N'Url port server', 'STRING', 'SYSTEM', 'Y', 'Y'),
	('SYSTEM','URL_API_TTDH', 'http://192.168.10.10:8380',null, N'Url api TTDH CLS', 'STRING', 'SYSTEM', 'Y', 'Y'),
	('API','WS_LPPP_GET', 'false','/listPendingDocumentByHandover/{code}', N'Lấy danh sách bàn giao hộ chiếu chờ in', 'BOOLEAN', 'SYSTEM', 'Y', 'Y');
	
	
# INSERT SQL Role

	INSERT INTO Roles(RoleName, RoleDescription, CreateBy, CreateDate, UpdateBy, UpdateDate) values
  ('Admin', N'Vai trò có toàn quyền quản lý hệ thống, bao gồm người dùng, sản phẩm, đơn hàng và cấu hình hệ thống.', 'Admin', '2024-11-21 17:19:52.007', 'Admin', '2024-11-21 17:25:15.663'),
  ('User', N'Vai trò cho phép duyệt sản phẩm, mua sắm và quản lý đơn hàng của cá nhân.', 'Admin', '2024-11-21 17:19:52.007', 'Admin', '2024-11-21 17:25:15.663'),
  ('ContentManager', N'Vai trò quản lý và chỉnh sửa nội dung trên website, bao gồm banner, bài viết blog và trang khuyến mãi.', 'Admin', '2024-11-21 17:19:52.007', 'Admin', '2024-11-21 17:25:15.663'),
  ('Support', N'Vai trò hỗ trợ khách hàng qua các kênh hỗ trợ và xử lý yêu cầu hoàn tiền hoặc đổi trả.', 'Admin', '2024-11-21 17:19:52.007', 'Admin', '2024-11-21 17:25:15.663'),
  ('DeliveryStaff', N'Vai trò xem danh sách đơn hàng được giao và cập nhật trạng thái giao hàng.', 'Admin', '2024-11-21 17:19:52.007', 'Admin', '2024-11-21 17:25:15.663');

	
	
