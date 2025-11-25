using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using System.Text.Json;
using teniacoSample.Models;

namespace teniacoSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, IMemoryCache cache)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _cache = cache;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            string cacheKey = "AllBreedsWithImages";
            int pageSize = 20;

            try
            {
                if (!_cache.TryGetValue(cacheKey, out List<Breed> allBreeds))
                {
                    var httpClient = _httpClientFactory.CreateClient();
                    string url = "https://dog.ceo/api/breeds/list/all";

                    var rawResponse = await httpClient.GetStringAsync(url);
                    var raw = JsonSerializer.Deserialize<RawDogApiResponse>(rawResponse);

                    allBreeds = raw.Message.Select(kvp => new Breed
                    {
                        Name = kvp.Key,
                        SubBreeds = kvp.Value,
                        Picture = null
                    }).ToList();

                    var sw = Stopwatch.StartNew();

                    await Parallel.ForEachAsync(allBreeds, async (breed, ct) =>
                    {
                        breed.Picture = await GetBreedRandomPicUrl(breed.Name);
                    });

                    sw.Stop();
                    var elapsedMs = sw.ElapsedMilliseconds;
                    var elapsedSec = sw.Elapsed.TotalSeconds;

                    _cache.Set(cacheKey, allBreeds, TimeSpan.FromHours(1));
                }

                var pagedBreeds = allBreeds
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();

                var model = new DogBreedsResponse
                {
                    Breeds = pagedBreeds,
                    PageNumber = page,
                    TotalPages = (int)Math.Ceiling(allBreeds.Count / (double)pageSize)
                };

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(null);
            }
        }

        private async Task<string> GetBreedRandomPicUrl(string breedName)
        {
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
