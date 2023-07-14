using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace T.Test
{
    public class UnitTest1 : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<T.WebApi.Services.ProductServices.IProductService> _factory;

        public UnitTest1(WebApplicationFactory<T.WebApi.Services.ProductServices.IProductService> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_ProductsList_Returns_Success()
        {
            // Khởi tạo phiên bản ứng dụng Web API
            var client = _factory.CreateClient();

            // Gửi yêu cầu HTTP GET đến địa chỉ endpoint cần kiểm thử
            var response = await client.GetAsync("/weatherforecast");

            // Kiểm tra kết quả trả về từ Web API
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);
        }
    }
}