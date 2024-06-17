using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T.Library.Model.Discounts
{
    public enum DiscountType
    {
        /// <summary>
        /// Assigned to order total 
        /// </summary>
        AssignedToOrderTotal = 1,

        /// <summary>
        /// Assigned to products (SKUs)
        /// </summary>
        AssignedToSkus = 2,

        /// <summary>
        /// Assigned to categories (all products in a category)
        /// </summary>
        AssignedToCategories = 5,

        /// <summary>
        /// Assigned to manufacturers (all products of a manufacturer)
        /// </summary>
        AssignedToManufacturers = 6,

        /// <summary>
        /// Assigned to order subtotal
        /// </summary>
        AssignedToOrderSubTotal = 20
    }
}
