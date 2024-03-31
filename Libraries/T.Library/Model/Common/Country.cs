using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T.Library.Model.Common
{
    public class Country : BaseEntity
    {
        public string? Name { get; set; }
        public string? Iso3 { get; set; }
        public string? Iso2 { get; set; }
        public string? NumericCode { get; set; }
        public string? PhoneCode { get; set; }
        public string? Capital { get; set; }
        public string? Currency { get; set; }
        public string? CurrencyName { get; set; }
        public string? CurrencySymbol { get; set; }
        public string? Tld { get; set; }
        public string? Native { get; set; }
        public string? Region { get; set; }
        public string? RegionId { get; set; }
        public string? Subregion { get; set; }
        public string? SubregionId { get; set; }
        public string? Nationality { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Emoji { get; set; }
        public string? EmojiU { get; set; }
        public bool AllowBilling { get; set; }
        public bool AllowShipping { get; set; }
        public bool Hidden { get; set; }
    }
}
