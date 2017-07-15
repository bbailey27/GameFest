using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameFestWebApp.Models
{
    public class TableModel
    {
        public string Name { get; set; }
        public int Size { get; set; }
        public bool IsKidTable { get; set; }

        public TableModel ()
        {
            this.Name = "";
            this.Size = 4;
            this.IsKidTable = false;
        }
        public TableModel(string name, int size, bool kid)
        {
            this.Name = name;
            this.Size = size;
            this.IsKidTable = kid;
        }
    }
}