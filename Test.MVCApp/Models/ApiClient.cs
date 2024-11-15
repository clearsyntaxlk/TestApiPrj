using Test.MVCApp.Models.Interface;
using Test.MVCApp.Models.Type;

namespace Test.MVCApp.Models
{
    public class ApiClient : IApiClient
    {
        private readonly string _baseAddress;
        public ApiClient() {
            //_baseAddress = configuration["Api:BaseAddress"];
        }

        public async Task<EndPointsDto> GetEndPointsAsync() {
            List<EndPoint> l = new List<EndPoint>();
            int i = 0;
            int parentId = 0;
            i++;
            parentId=i;
            l.Add(new EndPoint
            {
                Id = i,
                ParentId= parentId,
                RequestType = "GET",
                EndPointName = "get_authonticate",
                Parameters = new List<Parameter> { new Parameter
                {
                    Name = "userName",Type="Text",SrNo=1
                }, new Parameter
                {
                    Name="password",Type="Text",SrNo=2
                }
            }
            });
            i++;
            l.Add(new EndPoint
            {
                Id = i,
                ParentId = parentId,
                RequestType = "GET",
                EndPointName = "chaild-get_authonticate",
                Parameters = new List<Parameter> { new Parameter
                {
                    Name = "userName",Type="Text",SrNo=1
                }, new Parameter
                {
                    Name="password",Type="Text",SrNo=2
                }
                }
            });
            i++;
            l.Add(new EndPoint
            {
                Id = i,
                ParentId = parentId,
                RequestType = "GET",
                EndPointName = "chaild-get_authonticate=>"+i.ToString(),
                Parameters = new List<Parameter> { new Parameter
                {
                    Name = "userName",Type="Text",SrNo=1
                }, new Parameter
                {
                    Name="password",Type="Text",SrNo=2
                }
                }
            });

            //========== Endpoint 2
            i++;
            parentId = i;
            l.Add(new EndPoint
            {
                Id = i,
                ParentId = parentId,
                RequestType = "POST",
                EndPointName = "get_authorization",
                RequestBodyJson = "AccesToken"
            });
            i++;
            l.Add(new EndPoint
            {
                Id = i,
                ParentId = parentId,
                RequestType = "POST",
                EndPointName = "child-get_authorization",
                RequestBodyJson = "AccesToken"
            });
            i++;
            l.Add(new EndPoint
            {
                Id = i,
                ParentId = parentId,
                SubParentId = i-1,
                RequestType = "POST",
                EndPointName = "child-child-get_authorization",
                RequestBodyJson = "AccesToken"
            });


            EndPointsDto endPoints = new EndPointsDto {EndPoints=l };    
            return endPoints;
        }
    }
}
