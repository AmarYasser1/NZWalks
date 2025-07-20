using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using NZWalks.UI.Models.VMs;

namespace NZWalks.UI.Controllers
{
    public class RegionsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RegionsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = new List<RegionVM>();

            try
            {
                var client = _httpClientFactory.CreateClient();

                var httpResponseMessage = await client.GetAsync("https://localhost:7123/api/v1/Regions");

                httpResponseMessage.EnsureSuccessStatusCode();

                response.AddRange(await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<RegionVM>>());
            }
            catch (Exception ex)
            {

                // Log exceptions
            }
            return View(response);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddRegionVM regionVM)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Something went wrong while adding new region");

                var client = _httpClientFactory.CreateClient();

                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://localhost:7123/api/v1/Regions"),
                    Content = new StringContent(JsonSerializer.Serialize(regionVM), Encoding.UTF8, "application/json")
                };

                var httpResponseMessage = await client.SendAsync(httpRequestMessage);

                httpResponseMessage.EnsureSuccessStatusCode();

                var response = await httpResponseMessage.Content.ReadFromJsonAsync<RegionVM>();

                if (response is null)
                    return View();
            }
            catch (Exception ex)
            {

                // Log exception
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var client = _httpClientFactory.CreateClient();

            var response = await client.GetFromJsonAsync<RegionVM>($"https://localhost:7123/api/v1/Regions/{id.ToString()}");

            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RegionVM regionVM)
        {
            if (regionVM is null)
                return BadRequest("Region data is null.");

            if (!ModelState.IsValid)
                return View(regionVM);

            try
            {       
                var client = _httpClientFactory.CreateClient();

                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri($"https://localhost:7123/api/v1/Regions/{regionVM.Id}"),
                    Content = new StringContent(JsonSerializer.Serialize(regionVM), Encoding.UTF8, "application/json")
                };

                var httpResponseMessage = await client.SendAsync(httpRequestMessage);

                httpResponseMessage.EnsureSuccessStatusCode();

                var response = await httpResponseMessage.Content.ReadFromJsonAsync<RegionVM>();

                if (response is null)
                    return View(response);
            }
            catch (Exception ex)
            {

                return View(regionVM);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                var response = await client.DeleteFromJsonAsync<RegionVM>($"https://localhost:7123/api/v1/Regions/{id}");

                if (response is null)
                    return View(response);
            }
            catch (Exception)
            {

                // Log exception
            }

            return RedirectToAction("Index");
        }
    }
}
