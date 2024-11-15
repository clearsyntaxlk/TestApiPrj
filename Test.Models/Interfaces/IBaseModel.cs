using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Models.Interfaces
{
    /// <summary>
    /// Base model for entities
    /// </summary>
    public interface IBaseModel
    {
        #region Public Properties

        int Id { get; set; }

        string InsertedBy { get; set; }
        DateTime InsertedOn { get; set; }
        string UpdatedBy { get; set; }
        DateTime? UpdatedOn { get; set; }
        string DeletedBy { get; set; }
        DateTime? DeletedOn { get; set; }
        bool Deleted { get; set; }

        #endregion Public Properties
    }
}
