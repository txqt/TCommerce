using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using T.Library.Model.Common;

namespace T.Web.Models
{
    public class AddressModel : BaseEntity
    {
        [Required, Display(Name = "Tên")]
        public string FirstName { get; set; } = null!;

        [Required, Display(Name = "Họ")]
        public string LastName { get; set; } = null!;

        [Display(Name = "Công ty")]
        public string Company { get; set; }

        [Required, Display(Name = "Địa chỉ")]
        public string AddressDetails { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "Phải chọn Tỉnh/Thành phố")]
        public int ProvinceId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Phải chọn Quận/Huyện")]
        public int DistrictId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Phải chọn Phường/Xã")]
        public int CommuneId { get; set; }

        [Required, Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; } = null!;

        public int AddressTypeId { get; set; }

        [Required, Display(Name = "Loại địa chỉ")]
        public AddressType AddressType
        {
            get => (AddressType)AddressTypeId;
            set => AddressTypeId = (int)value;
        }

        [Required]
        public List<SelectListItem> AvaiableProvinces { get; set; }

        [Required]
        public List<SelectListItem> AvaiableDistricts { get; set; }

        [Required]
        public List<SelectListItem> AvaiableCommunes { get; set; }

        public AddressModel()
        {
            AvaiableProvinces = new List<SelectListItem>();
            AvaiableDistricts = new List<SelectListItem>();
            AvaiableCommunes = new List<SelectListItem>();
        }

        public bool IsDefault { get; set; }
    }
}
