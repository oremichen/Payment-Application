using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PaymentCore
{
    public class ENUM
    {
        public enum Status
        {
            [Description("Pending")]
            Pending = 1,

            [Description("Failed")]
            Failed = 2,

            [Description("Processed")]
            Processed = 3,
        }

      
    }
}
