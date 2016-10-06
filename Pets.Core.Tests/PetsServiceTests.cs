using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Pets.Core.Infrastructure;
using Pets.Core.Models;
using Pets.Core.Services;
using System.Threading.Tasks;

namespace Pets.Core.Tests
{
    [TestClass]
    public class PetsServiceTests
    {
        const string apiendpoint = "apiendpoint";

        [TestMethod]
        public void when_petapiclient_returns_error_service_outputed()
        {
            // arrange
            var petApiClient = Substitute.For<IPetsApiClient>();
            var outputer = Substitute.For<IOutputer>();

            var servire = new PetsService(apiendpoint, outputer, petApiClient);

            petApiClient.GetAllPetsOwners(apiendpoint).Returns(Task.FromResult(new PetsApiResponse { Success = false, Result = "Error 1" }));

            // act
            servire.CatsByGender().Wait();

            // assert
            outputer.Received().Output("Error 1");
        }

        [TestMethod]
        public void when_petapiclient_returns_differnt_dataobject_service_should_catch_the_exception_and_output_the_error()
        {
            // arrange
            var petApiClient = Substitute.For<IPetsApiClient>();
            var outputer = Substitute.For<IOutputer>();

            var servire = new PetsService(apiendpoint, outputer, petApiClient);

            petApiClient.GetAllPetsOwners(apiendpoint).Returns(Task.FromResult(new PetsApiResponse { Success = true, Result = "somewrongdata" }));

            // act
            servire.CatsByGender().Wait();

            // assert
            outputer.Received().Output(Arg.Is<string>(x => x.StartsWith("Error : ")));
        }

        [TestMethod]
        public void when_petapiclient_returns_empty_list_service_should_output_nothing()
        {
            // arrange
            var petApiClient = Substitute.For<IPetsApiClient>();
            var outputer = Substitute.For<IOutputer>();

            var servire = new PetsService(apiendpoint, outputer, petApiClient);

            petApiClient.GetAllPetsOwners(apiendpoint).Returns(Task.FromResult(new PetsApiResponse { Success = true, Result = "[{\"name\":\"Bob\",\"gender\":\"Male\",\"age\":23,\"pets\":[{\"name\":\"Boxer\",\"type\":\"Dog\"}]}]" }));

            // act
            servire.CatsByGender().Wait();

            // assert
            outputer.DidNotReceive().Output(Arg.Any<string>());
        }

        [TestMethod]
        public void when_petapiclient_returns_null_pets_array_service_should_hanle_it_without_any_error()
        {
            // arrange
            var petApiClient = Substitute.For<IPetsApiClient>();
            var outputer = Substitute.For<IOutputer>();

            var servire = new PetsService(apiendpoint, outputer, petApiClient);

            petApiClient.GetAllPetsOwners(apiendpoint).Returns(Task.FromResult(new PetsApiResponse { Success = true, Result = "[{\"name\":\"Bob\",\"gender\":\"Male\",\"age\":23,\"pets\":null}]" }));

            // act
            servire.CatsByGender().Wait();

            // assert
            outputer.DidNotReceive().Output(Arg.Is<string>(x => x.StartsWith("Error")));
        }

        [TestMethod]
        public void when_petapiclient_returns_valid_petsowners_array_CatsByGender_should_groupit_by_gender_and_ordered_cats()
        {
            // arrange
            var petApiClient = Substitute.For<IPetsApiClient>();
            var outputer = Substitute.For<IOutputer>();

            var servire = new PetsService(apiendpoint, outputer, petApiClient);

            petApiClient.GetAllPetsOwners(apiendpoint).Returns(Task.FromResult(new PetsApiResponse { Success = true, Result = "[{\"name\":\"Fred\",\"gender\":\"Male\",\"age\":40,\"pets\":[{\"name\":\"Tom\",\"type\":\"Cat\"},{\"name\":\"Max\",\"type\":\"Cat\"},{\"name\":\"Sam\",\"type\":\"Dog\"},{\"name\":\"Jim\",\"type\":\"Cat\"}]}, {\"name\":\"Samantha\",\"gender\":\"Female\",\"age\":40,\"pets\":[{\"name\":\"Tabby\",\"type\":\"Cat\"}]}, {\"name\":\"Jennifer\",\"gender\":\"Female\",\"age\":18,\"pets\":[{\"name\":\"Garfield\",\"type\":\"Cat\"}]}]" }));

            /* passed cats data are: 
                Male -> Tom, Max, Jim
                Female -> Tabby
                Female -> Garfield*/

            // act
            servire.CatsByGender().Wait();

            // assert
            // check cats names order
            /* order should be : 
             *     Male -> Jim, Max, Tom
             *     Female -> Garfield, Tabby
            */

            Received.InOrder(() =>
                {
                    outputer.Output("Male");
                    outputer.Output("Jim");
                    outputer.Output("Max");
                    outputer.Output("Tom");
                    outputer.Output("Female");
                    outputer.Output("Garfield");
                    outputer.Output("Tabby");
                });
        }

        [TestMethod]
        public void when_petapiclient_returns_valid_petsowners_array_with_casesensitive_names_catsbygender_should_order_cats_with_case_insensitive()
        {
            // arrange
            var petApiClient = Substitute.For<IPetsApiClient>();
            var outputer = Substitute.For<IOutputer>();

            var servire = new PetsService(apiendpoint, outputer, petApiClient);

            petApiClient.GetAllPetsOwners(apiendpoint).Returns(Task.FromResult(new PetsApiResponse { Success = true, Result = "[{\"name\":\"Fred\",\"gender\":\"Female\",\"age\":40,\"pets\":[{\"name\":\"Pineapple\",\"type\":\"Cat\"},{\"name\":\"orange\",\"type\":\"Cat\"},{\"name\":\"Sam\",\"type\":\"Dog\"},{\"name\":\"Grape\",\"type\":\"Cat\"}]}, {\"name\":\"Samantha\",\"gender\":\"Female\",\"age\":40,\"pets\":[{\"name\":\"Banana\",\"type\":\"Cat\"}]}, {\"name\":\"Jennifer\",\"gender\":\"Female\",\"age\":18,\"pets\":[{\"name\":\"apple\",\"type\":\"Cat\"}]}]" }));

            /* passed cats data are: 
                Female -> Pineapple, orange, Grape, Banana, apple
            */

            // act
            servire.CatsByGender().Wait();

            // assert
            // check cats names order
            /* order should be : 
             *     Female -> apple, Banana, Grape, orange, Pineapple,  
            */

            Received.InOrder(() =>
            {
                outputer.Output("Female"); 
                outputer.Output("apple");
                outputer.Output("Banana");
                outputer.Output("Grape");
                outputer.Output("orange");
                outputer.Output("Pineapple");
            });
        }
    }
}
