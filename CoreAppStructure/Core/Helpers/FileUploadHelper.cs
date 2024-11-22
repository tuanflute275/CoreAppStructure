namespace CoreAppStructure.Core.Helpers
{
    public static class FileUploadHelper
    {
        public static async Task<string> UploadImageAsync(IFormFile imageFile, string oldImage, string folderName)
        {
            string requestScheme = "http";
            string requestHost = "localhost:5095";
            if (imageFile != null && imageFile.Length > 0)
            {
                // Kiểm tra loại file (ví dụ: chỉ cho phép hình ảnh JPEG, PNG, GIF)
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    throw new ArgumentException("Invalid file type. Allowed types are: jpg, jpeg, png, gif.");
                }

                // Đường dẫn lưu file (dùng tên thư mục dynamic từ folderName)
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", folderName);

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Xóa hình ảnh cũ nếu tồn tại
                if (!string.IsNullOrEmpty(oldImage))
                {
                    var oldFileName = oldImage.Split($"{requestScheme}://{requestHost}/uploads/{folderName}/").LastOrDefault();
                    var oldFilePath = Path.Combine(uploadsFolder, oldFileName);

                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }
                }

                // Tạo tên file duy nhất để tránh trùng lặp
                var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Lưu file hình ảnh
                try
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    // Trả về đường dẫn URL của hình ảnh mới
                    var newImageUrl = $"{requestScheme}://{requestHost}/uploads/{folderName}/{uniqueFileName}";

                    return newImageUrl;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error uploading file: {ex.Message}");
                }
            }
            else
            {
                throw new ArgumentException("No image file provided.");
            }
        }
    }
}
