using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace GameFestWebApp.Objects
{
    public class Person
    {
        //possibly this can be developed at some point to allow people to enter preferences such as more card games or no spoons, preferences would be applied to break ties or near-ties when choosing tables
        public int PersonID { get; set; }//I guess these could be letters but this is easier for looping
        public Dictionary<int, int> PlayedWith { get; set; }//people they have played with already (id) and counts for number of times together
        public List<Table> Tables { get; set; }//visited tables, in order
        public bool IsChild { get; set; }//to restrict them to the kids table if necessary
        public int PlayedWithCount { get; set; }

        public Person(int id, bool isChild)
        {
            this.PersonID = id;//might need to change to 1-indexed or translate for printing at the end
            this.IsChild = isChild;
            this.PlayedWithCount = 0;
            this.Tables = new List<Table>();
            //this.PlayedWith = new Dictionary<Person, int>();
            this.PlayedWith = new Dictionary<int, int>();//person ID and play count
        }

        public void AssignChildTables(List<Table> childTables, int round)
        {
            if (childTables.Count == 1)
            {
                //'this' is the child being assigned to the table
                Table table = childTables.ElementAt(0);
                table.AllPlayers.Add(this);
                table.UpdatePlayedCounts(this, round);
                table.Rounds.ElementAt(round).Add(this);
                this.Tables.Add(table);
            }
            else
            {
                //TODO replace with a front-end error and break, or actually handle this logic
                //remember to reset IsFull each round if it's used
                Debug.WriteLine("Please designate only one children's table");
            }
        }

        public Table ChooseTable(List<Table> tables, int round)
        {
            //default values
            double minTableRank = 100;
            Table tableChoice = new Table(-1, "blank", 0, false);
            //Sort by how full the tables are (choose empty tables first to leave more tables open for the later people to choose from
            tables = tables.OrderBy(t => (t.Size - (t.Rounds.ElementAt(round).Count()))).ToList();//TODO not sure this is quite what I wanted (size-people for empty spots) (maybe break ties by choosing the bigger table??)
            foreach (Table t in tables)
            {
                //only check tables they haven't been to yet and aren't full
                if (!this.Tables.Contains(t) && !t.IsFull)
                {
                    //playcount = the sum of the playedwith entries for then new player with the people at the table
                    double playCount = t.Rounds.ElementAt(round).Sum(person => this.PlayedWith[person.PersonID]);
                    //if (t.Size == tables.Max(table => table.Size))
                    //{
                    //    playCount = 0;
                    //}
                    //table rank is the average playedwithcount for the new player with the people at the table, to account for different table sizes
                    double tableRank = (playCount / t.Size);
                    if (tableRank <= minTableRank)//TODO figure out a way to make sure the one person doesn't have to stay at the fireplace table
                    {
                        tableChoice = t;
                        minTableRank = tableRank;
                    }
                }
            }
            if (tableChoice.TableID == -1)
            {
                tableChoice = tables.First(t => t.IsFull == false);
                Debug.WriteLine("Repeat Table Choice: " + tableChoice.Location + ", player " + this.PersonID);
                //TODO let them choose the lower playcount here too?
            }
            return tableChoice;
        }
    }
}