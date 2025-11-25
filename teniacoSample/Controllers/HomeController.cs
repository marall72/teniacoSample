using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using teniacoSample.Models;

namespace teniacoSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var httpClient = _httpClientFactory.CreateClient();
            string url = "https://dog.ceo/api/breeds/list/all";

            try
            {
                var rawResponse = await httpClient.GetStringAsync(url);

                var raw = JsonSerializer.Deserialize<RawDogApiResponse>(rawResponse);

                var response = new DogBreedsResponse
                {
                    Status = raw.Status,
                    Breeds = raw.Message.Select(kvp => new Breed
                    {
                        Name = kvp.Key,
                        SubBreeds = kvp.Value
                    }).ToList()
                };

                for(int i = 0; i < response.Breeds.Count; i++)
                {
                    var breed = response.Breeds[i];
                    breed.Picture = await GetBreedRandomPicUrl(breed.Name);
                }

                return View(response);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(null);
            }
        }

        private async Task<string> GetBreedRandomPicUrl(string breedName)
        {
            //naffenpinscher
            var picsUrl = $"https://dog.ceo/api/breed/{breedName}/images/random";
            var httpClient = _httpClientFactory.CreateClient();
            var rawResponse = await httpClient.GetStringAsync(picsUrl);

            var image = JsonSerializer.Deserialize<DogImageResponse>(rawResponse);
            return image.ImageUrl;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
