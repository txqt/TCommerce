using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T.Library.Model.Startup
{
    public class StartupFormModel
    {

        [Required]
        public string AdminEmail { get; set; } = string.Empty;

        [Required]
        public string AdminPassword { get; set; } = string.Empty;

        [Required,Compare("AdminPassword", ErrorMessage = "Mật khẩu phải khớp")]
        public string ConfirmPassword { get; set; } = string.Empty;
        public bool CreateSampleData { get; set; }
        public string DbType { get; set; } = string.Empty;
        public bool CreateDatabaseIfNotExist { get; set; } = true;
        public string ServerName { get; set; } = string.Empty;
        public string DbName { get; set; } = string.Empty;
        public bool UseWindowsAuth { get; set; } = true;

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SqlUsername { get; set; } = string.Empty;

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SqlPassword { get; set; } = string.Empty;

        //public string GetConnectionString()
        //{
        //    if (UseWindowsAuth)
        //    {
        //        return $"Data Source={ServerName};Initial Catalog={DbName};Integrated Security=True";
        //    }
        //    else
        //    {
        //        return $"Data Source={ServerName};Initial Catalog={DbName};User ID={SqlUsername};Password={SqlPassword}";
        //    }
        //}
    }
}
