namespace T.Library.Model
{
    /// <summary>
    /// Represents the base class for entities
    /// </summary>
    public abstract partial class BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the entity is deleted or not
        /// </summary>
        public bool Deleted { get; set; }
    }
}