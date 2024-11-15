using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSDK.Type
{
    public class User
    {
        public string Name { get; set; }    
        public int Age { get; set; }
        public DateTime MdfOn { get; set; }
        public string? Role { get; set; }
        public string? EmailId { get; set; }
    }
}
