using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameFestWebApp.Objects
{
    public class Table
    {
        public int TableID { get; set; }
        public string Location { get; set; }
        public int Size { get; set; }
        public bool IsKidTable { get; set; }
        public bool IsFull { get; set; }
        public List<Person> AllPlayers { get; set; }//all players at this table for checking if they've been with those people
        public List<List<Person>> Rounds { get; set; }//list of people at the table in each round
        //list of rounds which contain lists of people at the table then
        public Table(int id, string location, int size, bool kidTable)
        {
            this.TableID = id;
            this.Location = location;
            this.Size = size;
            this.IsKidTable = kidTable;
            this.IsFull = false;
            this.AllPlayers = new List<Person>();//do I need a play count here? I think the people can keep track of that themselves
            this.Rounds = new List<List<Person>>();
        }

        public void UpdatePlayedCounts(Person newPlayer, int round)//'this' is the table being updated
        {
            //for all the people at the table already, update their played counts with the new person
            foreach (Person p in Rounds.ElementAt(round))
            {
                //add them to the new player's playedwith list or increase counts
                newPlayer.PlayedWith[p.PersonID] += 1;
                newPlayer.PlayedWithCount++;
                //add the new player to each person's list and increase counts
                p.PlayedWith[newPlayer.PersonID] += 1;
                p.PlayedWithCount++;
            }
        }
    }
}