﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Users;

namespace T.Library.Model.Common
{
    public partial class DeliveryAddress : BaseEntity
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? Company { get; set; }

        public string AddressDetails { get; set; } = null!;

        public int CommuneId { get; set; }

        public int DistrictId { get; set; }

        public int ProvinceId { get; set; }

        public string PhoneNumber { get; set; } = null!;

        public int DeliveryAddressTypeId { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public DeliveryAddressType DeliveryAddressType
        {
            get => (DeliveryAddressType)DeliveryAddressTypeId;
            set => DeliveryAddressTypeId = (int)value;
        }
    }
}
