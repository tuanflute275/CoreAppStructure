using CoreAppStructure.Data;
using CoreAppStructure.Features.Categories.Interfaces;
using CoreAppStructure.Features.Categories.Models;
using CoreAppStructure.Features.Categories.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CoreAppStructure.Tests.UnitTests
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<ILogger<CategoryService>> _mockLogger;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            // Tạo mock cho các phụ thuộc
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockLogger = new Mock<ILogger<CategoryService>>();

            // Khởi tạo CategoryService với các mock này
            _categoryService = new CategoryService(_mockCategoryRepository.Object, _mockLogger.Object);
        }

        // Test cho phương thức FindAllAsync
        [Fact]
        public async Task FindAllAsync_ShouldReturnPagedCategories_WhenNameAndSortAreProvided()
        {
            // Arrange: Tạo một danh sách các category giả
            var categories = new List<Category>
            {
                new Category { CategoryId = 1, CategoryName = "Category 1" },
                new Category { CategoryId = 2, CategoryName = "Category 2" },
                new Category { CategoryId = 3, CategoryName = "Category 3" },
                new Category { CategoryId = 4, CategoryName = "Category 4" },
                new Category { CategoryId = 5, CategoryName = "Category 5" }
            };

            // Mock phương thức FindAllAsync của repository trả về danh sách categories
            _mockCategoryRepository.Setup(r => r.FindAllAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(categories);

            // Act: Gọi phương thức FindAllAsync với các tham số (name, sort, page)
            var result = await _categoryService.FindAllAsync("Category", "Name-ASC", 1);

            // Assert: Kiểm tra mã trạng thái phản hồi
            Assert.Equal(200, result.status);

            // Kiểm tra rằng dữ liệu trả về là một danh sách các category đã phân trang
            var responseData = result.data as dynamic;
            var pagedCategories = responseData.Data as List<Category>;

            // Vì bạn chỉ trả về tối đa 10 items mỗi trang, chắc chắn dữ liệu đã phân trang đúng
            Assert.Equal(5, pagedCategories.Count); // 5 items trong 1 trang

            // Kiểm tra xem tổng số trang và tổng số bản ghi có chính xác không
            Assert.Equal(1, responseData.TotalPages); // Chỉ có 1 trang vì có 5 bản ghi
            Assert.Equal(5, responseData.TotalRecords); // Tổng số bản ghi là 5

            // Kiểm tra xem phương thức FindAllAsync đã được gọi chính xác 1 lần
            _mockCategoryRepository.Verify(r => r.FindAllAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        // Test cho phương thức FindByIdAsync
        [Fact]
        public async Task FindByIdAsync_ShouldReturnCategory_WhenCategoryExists()
        {
            // Arrange: Tạo đối tượng Category giả với CategoryId = 1 và CategoryName = "Category 1"
            var category = new Category { CategoryId = 1, CategoryName = "Category 1" };

            // Thiết lập mock repository trả về category khi gọi phương thức FindByIdAsync với bất kỳ int nào
            _mockCategoryRepository.Setup(r => r.FindByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(category);

            // Act: Gọi phương thức FindByIdAsync của CategoryService với ID 1
            var result = await _categoryService.FindByIdAsync(1);

            // Assert: Kiểm tra rằng kết quả trả về có status 200
            Assert.Equal(200, result.status);

            // Kiểm tra xem dữ liệu (data) trả về có phải là một Category không
            var responseData = result.data as Category;
            Assert.NotNull(responseData);
            Assert.Equal("Category 1", responseData.CategoryName);

            // Kiểm tra xem phương thức FindByIdAsync của repository đã được gọi đúng một lần
            _mockCategoryRepository.Verify(r => r.FindByIdAsync(It.IsAny<int>()), Times.Once);
        }

        // Test cho phương thức AddAsync
        /*[Fact]
        public async Task AddAsync_ShouldCallAddAsync_WhenCategoryIsValid()
        {
            // Arrange: Tạo đối tượng Category mới với CategoryName = "New Category"
            var newCategory = new Category { CategoryName = "New Category" };

            // Thiết lập mock repository để kiểm tra phương thức AddAsync và SaveChangesAsync
            _mockCategoryRepository.Setup(r => r.AddAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);
            _mockCategoryRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act: Gọi phương thức SaveAsync từ CategoryService
            await _categoryService.SaveAsync(newCategory);

            // Assert: Kiểm tra xem phương thức AddAsync của repository đã được gọi một lần với Category hợp lệ
            _mockCategoryRepository.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Once);

            // Kiểm tra xem phương thức SaveChangesAsync đã được gọi một lần để lưu thay đổi
            _mockCategoryRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }*/
    }
}
