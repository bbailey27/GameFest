using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameFestWebApp.Models
{
    public class HomeViewModel
    {
        //from user input form
        public int NumRounds { get; set; }
        public int NumPlayers { get; set; }
        public int ChildCount { get; set; }
        //likely replace this tuple with a simple DTO for the front end
        public List<TableModel> Tables { get; set; }//store name/location and size and whether it's a kids' table

    }
}