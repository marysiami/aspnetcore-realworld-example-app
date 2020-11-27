using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Infrastructure
{
    public class RemindPasswordData
    {
        public class RemindPasswordDataModel
        {
            public string Email { get; set; }

            public string Code { get; set; }
        }

        public static List<RemindPasswordDataModel> RemindPasswordListData = new List<RemindPasswordDataModel>();
    }
}
