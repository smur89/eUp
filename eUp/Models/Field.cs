using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eUp.Models
{
    public class Field
    {
        public int FieldId { get; set; }
        public int UserTableId { get; set; }
        public string FieldName { get; set; }
        public virtual UserTable Table { get; set; }
    }
}