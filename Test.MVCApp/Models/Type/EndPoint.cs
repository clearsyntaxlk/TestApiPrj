namespace Test.MVCApp.Models.Type
{
    public class EndPoint
    {
        public int Id { get; set; } 
        public int ParentId { get; set; }
        public int SubParentId { get; set; } = 0;
        public int Version { get; set; }
        public string EndPointName { get; set; }
        public string RequestType { get; set; }
        public string RequestBodyJson { get; set; }
        public List<Parameter> Parameters{ get; set; }
        public EndPoint() { 
            Parameters = new List<Parameter>();
        }
    }
}
