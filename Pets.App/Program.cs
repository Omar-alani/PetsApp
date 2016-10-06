using Pets.Core.Services;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace Pets.App
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().Wait();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadLine();
        }
        
        static async Task RunAsync()
        {
            var apiEndPoint = ConfigurationManager.AppSettings["PetsApiUrl"];
            var service = new PetsService(apiEndPoint);

            await service.CatsByGender();
        }
    }
}
