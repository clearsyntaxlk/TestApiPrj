using Microsoft.AspNetCore.Authorization;
using System.ServiceModel;

namespace TestApiSOAP.BusinessLogic.Services.Interface
{
    [ServiceContract]
    public interface ISoapService
    {
        [OperationContract]
        string Sum(int num1, int num2);
    }
    [Authorize]
    public class SoapService : ISoapService
    {
        public string Sum(int num1, int num2)
        {
            return $"Sum of two number is: {num1 + num2}";
        }

    }
}
