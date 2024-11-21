# TCommerce EN) | [VI](./README.md)

- An e-commerce project using ASP.NET Core MVC.

## Overview
- This project is a small e-commerce application built to enhance programming skills.

## Requirements

- [ASP.NET Core 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2019 or 2022](https://visualstudio.microsoft.com/vs/)
- [Microsoft SQL Server 2019 or 2022](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

## Installation Instructions
1. Clone the project from GitHub:

    ```bash
    git clone https://github.com/txqt/TCommerce.git
    ```

2. Open the solution in VS2019 or VS2022.
3. Run the following command to install NuGet packages:

    ```bash
    dotnet restore
    ```
4. Build the project (Ctrl + Shift + B).
5. Run the project (TCommerce.Web) with F5 or Ctrl + F5.
6. Fill in basic database information.
![Admin account and sample data creation](setup-images/store-info.png)
![Fill in datatable (MSSQL) information](setup-images/db-info.png)

## Features

- **Product Management**: You can add, edit, delete, modify attributes, add product images, and link related products.
- **Attribute Management**: You can add, edit, delete product attributes.
- **Category Management**: You can add, edit, delete categories, and assign products to categories.
- **Manufacturer Management**: You can add, edit, delete manufacturers and assign products to them.
- **User Management**: You can add, edit, delete users, and assign or modify user roles.
- **Role and Permission Management**: You can add or remove permissions for one or more roles.
- **Order Management**: Handle orders, track status, and manage order history.
- **Online Payment Integration**: Supports VNPAY payment (testing).
