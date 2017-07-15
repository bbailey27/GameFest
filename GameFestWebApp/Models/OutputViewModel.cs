using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameFestWebApp.Components;

namespace GameFestWebApp.Models
{
    public class OutputViewModel
    {
        //from user input form
        public OutputModel Result { get; set; }
        private Organizer organizer;

        public OutputViewModel(InputModel model)
        {
            organizer = new Organizer(model);
        }

        public void GetResults()
        {
            organizer.RunOptimizer();
        }
    }
}