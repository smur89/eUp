using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace eUp.Models
{
    public class Field
    {
        public int FieldId { get; set; }
        public int UserTableId { get; set; }
        [DisplayName("Field Name")]
        public string FieldName { get; set; }
        [DisplayName("Type of Field")]
        public string FieldType { get; set; }
        public virtual UserTable Table { get; set; }
    }
}