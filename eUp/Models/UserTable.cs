using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace eUp.Models
{
    public class UserTable
    {
        public int UserTableId { get; set; }
        public int UserId { get; set; }
        public string TableName { get; set; }
        [DisplayName("Field")]
        public virtual ICollection<Field> Fields { get; set; }
        public virtual User User { get; set; }
    }
}