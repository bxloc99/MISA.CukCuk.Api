using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISA.CukCuk.Api.Model
{
    public class CustomerGroup: BaseEntity
    {
        #region Property

        public Guid CustomerGroupId { get; set; }

        public string CustomerGroupName { get; set; }

        public string Description { get; set; }

        #endregion
    }
}
