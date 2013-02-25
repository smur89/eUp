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
        public ICollection<string> FieldType = new System.Collections.ObjectModel.Collection<string>{"text","number","yes/no"};
        public virtual UserTable Table { get; set; }
    }
}