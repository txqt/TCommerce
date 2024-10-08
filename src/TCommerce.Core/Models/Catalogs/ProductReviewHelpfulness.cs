﻿using TCommerce.Core.Models.Common;

namespace TCommerce.Core.Models.Catalogs
{
    /// <summary>
    /// Represents a product review helpfulness
    /// </summary>
    public partial class ProductReviewHelpfulness : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product review identifier
        /// </summary>
        public int ProductReviewId { get; set; }

        /// <summary>
        /// A value indicating whether a review a helpful
        /// </summary>
        public bool WasHelpful { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        public ProductReview? ProductReview { get; set; }
    }
}
