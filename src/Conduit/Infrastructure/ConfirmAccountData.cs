using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Infrastructure
{
    public static class ConfirmAccountData
    {
        public class ConfirmAccountDataModel
        {
            public string Email { get; set; }

            public string Code { get; set; }
        }

        public static List<ConfirmAccountDataModel> ConfirmAccountListData = new List<ConfirmAccountDataModel>();
    }
}
