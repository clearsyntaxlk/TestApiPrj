using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Models.Interfaces;

namespace Test.Models.Models
{
    /// <summary>
    /// Base model for entities
    /// </summary>
    public class BaseModel : IBaseModel
    {
        #region Public Properties

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("InsertedBy")]
        public string InsertedBy { get; set; }

        [Column("InsertedOn")]
        public DateTime InsertedOn { get; set; }

        [Column("UpdatedBy")]
        public string UpdatedBy { get; set; }

        [Column("UpdatedOn")]
        public DateTime? UpdatedOn { get; set; }

        [Column("DeletedBy")]
        public string DeletedBy { get; set; }

        [Column("DeletedOn")]
        public DateTime? DeletedOn { get; set; }

        [Column("Deleted")]
        public bool Deleted { get; set; }

        #endregion Public Properties
    }
}
