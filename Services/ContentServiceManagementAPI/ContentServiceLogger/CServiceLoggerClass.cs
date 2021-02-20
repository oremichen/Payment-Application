
using System;

namespace ContentServiceManagementAPI.ContentServiceLogger
{
    public class CServiceLoggerClass
    {
        public string ServiceName { get; set; }
        public string AppName { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime DateTime { get; set; }
        public string IpAddress { get; set; }
    }
}
