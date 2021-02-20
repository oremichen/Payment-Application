using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;

namespace ContentServiceManagementAPI.ContentServiceLogger
{
    public class CServiceLogger:ICServiceLogger
    {
        private readonly ILogger<CServiceLogger> logger;
        private readonly IHttpContextAccessor _httpAccessor;

        public CServiceLogger(ILogger<CServiceLogger>Logger, IHttpContextAccessor httpAccessor)
        {
            this.logger = Logger;
            _httpAccessor = httpAccessor;
        }

        public void LogError(string message, Exception ex)
        {

            var scheme = _httpAccessor.HttpContext.Request.Scheme;
            var protocol = _httpAccessor.HttpContext.Request.Protocol;
            var IPAddress = _httpAccessor.HttpContext.Request.Host;
            var pathBase = _httpAccessor.HttpContext.Request.PathBase;

            var IpAddress = ($"{scheme}://{protocol}{IPAddress}/{pathBase}");

           var LogModel= new CServiceLoggerClass()
           {

               AppName = "ANQ",
               ServiceName = "ContentManagementAPIService",
               ErrorMessage = message,
               DateTime = DateTime.Now,
               IpAddress= IpAddress
           };

            logger.LogError("{ServiceName} { ApplicationName} {IPaddress} {DateTime} {errormessage}", LogModel.ServiceName, LogModel.AppName,
                     LogModel.IpAddress, LogModel.DateTime.ToString(), LogModel.ErrorMessage, LogModel);
        }       
    }
}
