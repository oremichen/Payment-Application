using System;

namespace ContentServiceManagementAPI.ContentServiceLogger
{
    public interface ICServiceLogger
    {
        void LogError(string message, Exception ex);
    }
}
