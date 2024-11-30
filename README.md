# TCommerce [EN](./README(EN).md) | VI

- Dự án thương mại điện tử sử dụng ASP.NET Core MVC.

## Tổng quan
- Dự án này là một ứng dụng thương mại điện tử nhỏ được xây dựng để nâng cao kỹ năng lập trình.
## Yêu Cầu

- [.NET 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Visual Studio 2022 (phiên bản v17.12 hoặc hơn)](https://visualstudio.microsoft.com/vs/)
- [Microsoft SQL Server 2019 hoặc 2022](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

## Hướng dẫn cài đặt
1. Clone dự án từ GitHub:

    ```bash
    git clone https://github.com/txqt/TCommerce.git
    ```

2. Mở solution
3. Chạy lệnh sau để cài đặt các gói NuGet:

    ```bash
    dotnet restore
    ```
4. Build dự án
5. Run dự án (TCommerce.Web)
6. Điền thông tin cơ bản về database
![Tài khoản admin và tạo dữ liệu mẫu](setup-images/store-info.png)
![Điền thông tin của datatable(MSSQL)](setup-images/db-info.png)

## Một vài chức năng

- **Quản lý sản phẩm**: Có thể thêm, sửa, xóa, chỉnh sửa thuộc tính, thêm hình ảnh sản phẩm và liên kết các sản phẩm liên quan với nhau (related product).
- **Quản lí thuộc tính**: Có thể thêm, sửa, xóa thuộc tính sản phẩm.
- **Quản lí danh mục**: Có thể thêm, sửa, xóa danh mục và thêm các sản phẩm vào danh mục.
- **Quản lí nhà sản xuất**: Có thể thêm, sửa, xóa danh mục và thêm các sản phẩm nhà chế tạo.
- **Quản lý người dùng**: Có thể thêm, sửa, xóa, thêm hoặc chỉnh sửa vai trò của người dùng.
- **Quản lý vai trò và quyền hạn**: Có thể thêm hoặc xóa quyền của một hoặc nhiều vai trò.
- **Quản lý đơn hàng**: Xử lý các đơn hàng, theo dõi trạng thái và quản lý lịch sử đơn hàng.
- **Tích hợp thanh toán trực tuyến**: Hỗ trợ thanh toán VNPAY (sandbox).
