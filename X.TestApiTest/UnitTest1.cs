using TestSDK;

namespace X.TestApiTest
{
    public class UnitTest1
    {
        private HttpClient BuildHttpClent(string prefix = "https://localhost:7223/")
        => new HttpClient
        {
            BaseAddress = new Uri(prefix)
        };

        [Fact]
        public async void Test1()
        {
            string _token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiQ2hhbWluZGEiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJhZG1pbiIsImp0aSI6IjBlZmU4MGY5LTRjYTQtNGE5ZS1hYjhkLWY2NGY4YTJlNWM2NyIsImV4cCI6MTcyMjkzMTI0NCwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo3MjIzIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo3MjIzIn0.OJjY2Z3waK6v2gHtmiiMd9ElvoAMvDcZbzEBdx1LNQE";
            ITestInterface testSdk= new TestClient(BuildHttpClent());
            var _resul = testSdk.GetUsers(_token);
            Console.WriteLine(_resul.ToString());   
            Assert.Equal("Chaminda", _resul.Result.users.FirstOrDefault().Name.ToString());

        }
    }
}