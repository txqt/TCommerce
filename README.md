# TCommerce [EN](./README(EN).md) | VI

- Dự án thương mại điện tử sử dụng ASP.NET Core MVC.

## Tổng quan
- Đây là một ứng dụng thương mại điện tử nhỏ được xây dựng để nâng cao kỹ năng lập trình.

## Yêu cầu

- [.NET 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Visual Studio 2022 (v17.12 hoặc cao hơn)](https://visualstudio.microsoft.com/vs/)
- [Microsoft SQL Server 2019 hoặc 2022](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

## Hướng dẫn cài đặt
1. Clone dự án từ GitHub:

    ```bash
    git clone https://github.com/txqt/TCommerce.git
    ```

2. Mở file solution `TCommerce.sln` bằng Visual Studio 2022.
3. Chạy lệnh sau để cài đặt các gói NuGet:

    ```bash
    dotnet restore
    ```

4. Build dự án trong Visual Studio.
5. Đảm bảo `TCommerce.Web` được thiết lập làm project khởi động (startup project).
6. Chạy dự án.
7. Điền các thông tin cơ bản về cơ sở dữ liệu khi được yêu cầu:
    
    ![Tài khoản admin và tạo dữ liệu mẫu](setup-images/store-info.png)
    ![Điền thông tin của datatable (MSSQL)](setup-images/db-info.png)

## Một vài chức năng

- **Quản lý sản phẩm**: Thêm, sửa, xóa sản phẩm, chỉnh sửa thuộc tính, thêm hình ảnh và liên kết sản phẩm liên quan.
- **Quản lý thuộc tính**: Thêm, sửa, xóa thuộc tính sản phẩm.
- **Quản lý danh mục**: Thêm, sửa, xóa danh mục và gắn sản phẩm vào danh mục.
- **Quản lý nhà sản xuất**: Thêm, sửa, xóa nhà sản xuất và gắn sản phẩm vào nhà sản xuất.
- **Quản lý người dùng**: Thêm, sửa, xóa người dùng, gán hoặc sửa vai trò.
- **Quản lý vai trò và quyền hạn**: Thêm hoặc xóa quyền của một hoặc nhiều vai trò.
- **Quản lý đơn hàng**: Xử lý, theo dõi trạng thái và quản lý lịch sử đơn hàng.
- **Thanh toán trực tuyến**: Momo (test).
