using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Models.Interfaces;

namespace Test.Models.Models
{
    public class User : IUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("Name")]
        public string? Name { get; set; }
        public int? Age { get; set; }
        public DateTime? MdfOn { get; set; }
        public string? Role { get; set; }
        public string? EmailId { get; set; }
        public string? RefreshToken { get; set;}
        public DateTime? RefreshTokenExpiryTime { get; set;}

    }
}
