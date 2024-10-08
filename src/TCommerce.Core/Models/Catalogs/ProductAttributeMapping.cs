﻿

using System.ComponentModel.DataAnnotations.Schema;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Common;

namespace TCommerce.Core.Models.Catalogs
{
    /// <summary>
    /// Represents a product attribute mapping
    /// </summary>
    public partial class ProductAttributeMapping : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the product attribute identifier
        /// </summary>
        public int ProductAttributeId { get; set; }

        /// <summary>
        /// Gets or sets a value a text prompt
        /// </summary>
        public string? TextPrompt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is required
        /// </summary>
        public bool IsRequired { get; set; } = true;

        /// <summary>
        /// Gets or sets the attribute control type identifier
        /// </summary>
        public int AttributeControlTypeId { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        //validation fields

        /// <summary>
        /// Gets or sets the validation rule for minimum length (for textbox and multiline textbox)
        /// </summary>
        public int? ValidationMinLength { get; set; }

        /// <summary>
        /// Gets or sets the validation rule for maximum length (for textbox and multiline textbox)
        /// </summary>
        public int? ValidationMaxLength { get; set; }

        /// <summary>
        /// Gets or sets the validation rule for file allowed extensions (for file upload)
        /// </summary>
        public string? ValidationFileAllowedExtensions { get; set; }

        /// <summary>
        /// Gets or sets the validation rule for file maximum size in kilobytes (for file upload)
        /// </summary>
        public int? ValidationFileMaximumSize { get; set; }

        /// <summary>
        /// Gets or sets the default value (for textbox and multiline textbox)
        /// </summary>
        public string? DefaultValue { get; set; }

        public int ConditionAttributeSelected { get; set; }

        [NotMapped]
        public AttributeControlType AttributeControlType
        {
            get => (AttributeControlType)AttributeControlTypeId;
            set => AttributeControlTypeId = (int)value;
        }

        public List<ProductAttributeValue>? ProductAttributeValue { get; set; }
        public Product? Product { get; set; }
        public ProductAttribute? ProductAttribute { get; set; }
    }
}