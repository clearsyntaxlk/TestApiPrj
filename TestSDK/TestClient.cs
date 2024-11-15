
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using TestSDK.Object;

namespace TestSDK
{
    public class TestClient : ITestInterface
    {
        HttpClient _client;
        public TestClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<string> AuthenticateAsync()
        {

            return "";
        }

        public async Task<UsersDto> GetUsers(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var result = await _client.GetAsync($"mytest/2/Employee");
            //string t = await result.Content.ReadAsStringAsync(); 
            //var obj= JsonConvert.DeserializeObject<UsersDto>(await result.Content.ReadAsStringAsync());
            if (result.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<UsersDto>(await result.Content.ReadAsStringAsync());
            else
                throw new Exception(await result.Content.ReadAsStringAsync());

        }

    }
}