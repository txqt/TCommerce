# TCommerce EN | [VI](./README.md)

- An e-commerce project built with ASP.NET Core MVC.

## Overview
- This is a small e-commerce application designed to improve programming skills.

## Requirements

- [.NET 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Visual Studio 2022 (or later)](https://visualstudio.microsoft.com/vs/)
- [Microsoft SQL Server 2019 or 2022](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

## Installation Guide
1. Clone the project from GitHub:

    ```bash
    git clone https://github.com/txqt/TCommerce.git
    ```

2. Open the solution.
3. Run the following command to install NuGet packages:

    ```bash
    dotnet restore
    ```
4. Build the project.
5. Run the project (TCommerce.Web).
6. Fill in the basic database information.
![Admin account setup and sample data creation](setup-images/store-info.png)
![Enter database information (MSSQL)](setup-images/db-info.png)

## Key Features

- **Product Management**: Add, edit, delete products, manage product attributes, add product images, and link related products.
- **Attribute Management**: Add, edit, delete product attributes.
- **Category Management**: Add, edit, delete categories and assign products to categories.
- **Manufacturer Management**: Add, edit, delete manufacturers and assign products to manufacturers.
- **User Management**: Add, edit, delete users, and manage user roles.
- **Role and Permission Management**: Add or remove permissions for one or multiple roles.
- **Order Management**: Process orders, track their status, and manage order history.
- **Online Payment Integration**: Supports VNPAY payment (sandbox).
