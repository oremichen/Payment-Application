using ContentServiceManagementAPI.HttpClientService.Dto;
using ContentServiceManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.HttpClientService
{
    public class AnqClientService : IAnqClientService
    {
        private HttpClient client = new HttpClient();
        private ILogger _logger;
        private readonly IOptions<ServiceSettings> _serviceSettings;

        public AnqClientService(IOptions<ServiceSettings> serviceSettings , ILogger<AnqClientService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _serviceSettings = serviceSettings;
            _logger = logger;
        }

        public async Task<List<AnqContentProviderDto>> GetAllContentProviders()
        {
            try
            {
                var response = await client.GetAsync(_serviceSettings.Value.AnqBaseUrl + "allcontentproviders");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<List<AnqContentProviderDto>>();
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception,$"An error occured while pulling contentProviders from ACQ");
                throw;
            }
        }

        public async Task<List<AnqPartnerDto>> GetAllServiceProviders()
        {
            try
            {
                var response = await client.GetAsync(_serviceSettings.Value.AnqBaseUrl + "allpatners");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<List<AnqPartnerDto>>();
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, $"An error occured while pulling serviceproviders from ACQ");
                throw;
            }
        }

        public async Task<List<AnqClientDto>> GetAllClients()
        {
            try
            {
                var request = await client.GetAsync(_serviceSettings.Value.AnqBaseUrl + "allclients");
                request.EnsureSuccessStatusCode();
                if (request.IsSuccessStatusCode)
                {
                   return await request.Content.ReadAsAsync<List<AnqClientDto>>();
                }
                return null;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, $"An error occured while pulling clients from ACQ");
                throw;
            }

        }

        public async Task<List<AnqCommandRecordDto>> GetAnqCommandRecords(int clientId)
        {
            try
            {
                var request = await client.GetAsync(_serviceSettings.Value.AnqBaseUrl + "commandrecords?providerId="+ clientId);
                request.EnsureSuccessStatusCode();
                if (request.IsSuccessStatusCode)
                {
                    return await request.Content.ReadAsAsync<List<AnqCommandRecordDto>>();
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured while pulling Command Records from ACQ");
                throw;
            }

        }
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {                 
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion


    }
}
