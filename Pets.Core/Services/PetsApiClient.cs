using System;
using System.Net.Http;
using Pets.Core.Models;
using System.Threading.Tasks;
using Pets.Core.Infrastructure;

namespace Pets.Core.Services
{
    public sealed class PetsApiClient : IPetsApiClient
    {
        public async Task<PetsApiResponse> GetAllPetsOwners(string requestUri)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(requestUri).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        return new PetsApiResponse
                        {
                            Success = true,
                            Result = result
                        };
                    }
                    else
                    {
                        return new PetsApiResponse
                        {
                            Success = false,
                            Result = $"Request to '{requestUri}' returned with : {response.StatusCode}"
                        };
                    }
                }
                catch(Exception ex)
                {
                    return new PetsApiResponse
                    {
                        Success = false,
                        Result = $"Error : {ex.Message}"
                    };
                }
            }
        }
    }
}
