using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameFestWebApp.Models
{
    //dto to avoid passing the vm around
    public class InputModel
    {
        //from user input form
        public int NumRounds { get; set; }
        public int NumPlayers { get; set; }
        public int ChildCount { get; set; }
        //simple DTO for the front end
        public List<TableModel> Tables { get; set; }//store name/location and size and whether it's a kids' table

        public InputModel()
        {
            NumRounds = 3;
            NumPlayers = 20;
            ChildCount = 0;
            Tables = new List<TableModel>();
            Tables.Add(new TableModel("Table 1", 4, false));
        }
    }
}