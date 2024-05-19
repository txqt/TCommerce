using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Common;

namespace T.Library.Model.ViewsModel
{
    public class ShoppingCartItemModel : BaseEntity
    {
        public List<SelectedAttribute>? Attributes { get; set; }

        //public decimal? CustomerEnteredPrice { get; set; }

        public int Quantity { get; set; }

        //public DateTime? RentalStartDateUtc { get; set; }

        //public DateTime? RentalEndDateUtc { get; set; }

        public DateTime? CreatedOnUtc { get; set; }

        public DateTime? UpdatedOnUtc { get; set; }

        public ShoppingCartType ShoppingCartType { get; set; }

        public int ProductId { get; set; }

        //public ProductModel? ProductModel { get; set; }

        public Guid UserId { get; set; }
        public class SelectedAttribute
        {
            public int ProductAttributeMappingId { get; set; }
            public List<int>? ProductAttributeValueIds { get; set; }
        }
        public class ShoppingCartValidationResult
        {
            public bool IsValid { get; set; }
            public List<string>? Warnings { get; set; }
        }
    }
}
