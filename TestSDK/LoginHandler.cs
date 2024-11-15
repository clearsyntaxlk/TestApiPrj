using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TestSDK
{
    public class LoginHandler : DelegatingHandler
    {
        private readonly ITestInterface _loginApiRepository;
        public LoginHandler(ITestInterface loginApiRepository)
        {
            _loginApiRepository = loginApiRepository;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
        {
            var token = await _loginApiRepository.AuthenticateAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
            return await base.SendAsync(request, cancellationToken);
        }

    }
}
