

using TCommerce.Core.Models.Common;

namespace TCommerce.Core.Models.Catalogs
{
    /// <summary>
    /// Represents a product attribute
    /// </summary>
    public partial class ProductAttribute : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string? Description { get; set; }

        public List<ProductAttributeMapping>? ProductAttributeMappings { get; set; }
    }
}
