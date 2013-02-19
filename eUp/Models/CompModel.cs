using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eUp.Models
{
    public class CompModel
    {
        public int CompModelId { get; set; }
        public UserTable Table {get;set;}
        public ICollection<Field> Fields { get; set; } 
    }
}