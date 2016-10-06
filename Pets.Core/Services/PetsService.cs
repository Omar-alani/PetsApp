using Newtonsoft.Json;
using Pets.Core.Helpers;
using Pets.Core.Infrastructure;
using Pets.Core.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Pets.Core.Services
{
    public class PetsService
    {
        private readonly string _apiEndPoint;

        private readonly IOutputer _outputer;
        private readonly IPetsApiClient _petsApiClient;

        public PetsService(string apiEndPoint) : this(apiEndPoint, new ConsoleOutputer(), new PetsApiClient())
        {
        }

        public PetsService(string apiEndPoint, IOutputer outputer, IPetsApiClient petsApiClient)
        {
            // This can be injected through a config reader interface for example, 
            // but for now I am using the app.config
            _apiEndPoint = apiEndPoint;

            _outputer = outputer;
            _petsApiClient = petsApiClient;
        }

        public async Task CatsByGender()
        {
            try
            {
                var response = await _petsApiClient.GetAllPetsOwners(_apiEndPoint);
                if (response.Success)
                {
                    var petsOwners = JsonConvert.DeserializeObject<List<PetOwner>>(response.Result);

                    if (petsOwners != null)
                    {
                        var catsByGender = petsOwners
                            .Where(o => !string.IsNullOrEmpty(o.Gender) && !string.IsNullOrWhiteSpace(o.Gender) && o.Pets != null && o.Pets.Any(p => p.Type == Constants.PetTypes.Cat))
                            .GroupBy(o => o.Gender, o => o.Pets.Where(p => p.Type == Constants.PetTypes.Cat).Select(p => p.Name))
                            .Select(g => new { Gender = g.Key, Cats = g.SelectMany(n => n).OrderBy(n => n).ToList() })
                            .ToDictionary(r => r.Gender, r => r.Cats);


                        foreach (var gender in catsByGender.Keys)
                        {
                            _outputer.Output(gender);

                            foreach (var name in catsByGender[gender])
                            {
                                _outputer.Output($"{name}");
                            }
                        }
                    }
                }
                else
                {
                    _outputer.Output(response.Result);
                }
            }
            catch(Exception ex)
            {
                _outputer.Output($"Error : {ex.Message}");
            }
        }
    }
}
