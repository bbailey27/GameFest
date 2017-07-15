using GameFestWebApp.Components;
using GameFestWebApp.Models;
using GameFestWebApp.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace GameFestWebApp.Components
{
    public class Organizer
    {
        public IO FileIO;

        //from user input form
        public int NumRounds { get; set; }
        public int NumPlayers { get; set; }
        public int ChildCount { get; set; }
        //generated
        public List<Table> Tables { get; set; }
        public List<Table> ChildTables { get; set; }
        public List<Person> Players { get; set; }
        public List<Person> AdultPlayers { get; set; }
        public List<Person> Children { get; set; }

        //TODO make a method to translate the user input DTOs into the necessary objects/properties and create the lists
        public Organizer(InputModel model)//possibly this can go in a controller?
        {
            this.FileIO = new IO();
            this.NumRounds = model.NumRounds;
            this.NumPlayers = model.NumPlayers;
            this.ChildCount = model.ChildCount;
            this.Players = new List<Person>();
            this.AdultPlayers = new List<Person>();
            this.Children = new List<Person>();
            this.Tables = new List<Table>();
            this.ChildTables = new List<Table>();
            //populate the lists
            GeneratePlayerList();
            GenerateTableLists(model.Tables);
        }


        public void GeneratePlayerList()
        {
            for (int id = 0; id < NumPlayers; id++)
            {
                if (ChildCount > id)
                {
                    Players.Add(new Person(id, true));
                }
                else
                {
                    Players.Add(new Person(id, false));
                }
            }
            //shuffle so families aren't together on the first round
            Players.Shuffle();
            //add blank entries for each person to avoid checks later
            foreach (Person person in Players)
            {
                foreach (Person otherPlayer in Players)
                {
                    if (!person.Equals(otherPlayer))
                    {//skip themselves
                        person.PlayedWith.Add(otherPlayer.PersonID, 0);
                    }
                }
            }
        }

        public void GenerateTableLists(List<TableModel> tableInfo)
        {
            for (int id = 0; id < tableInfo.Count; id++)
            {
                Table newTable = new Table(id, tableInfo.ElementAt(id).Name, tableInfo.ElementAt(id).Size, tableInfo.ElementAt(id).IsKidTable);
                Tables.Add(newTable);
                if (newTable.IsKidTable)
                {
                    ChildTables.Add(newTable);
                }
                for (int round = 0; round < NumRounds; round++)//change to 1 index at end?
                {
                    //fill with blank lists
                    newTable.Rounds.Add(new List<Person>());
                }
            }
            //sort so the biggest tables are last so ties and first picks will default to filling the bigger table first
            //because the big tables are hardest to fill and have the highest playedwith counts in general
            Tables.OrderBy(table => table.Size);
        }

        public void RunOptimizer()
        {
            /*
            Assign any kids to the kids table first (or rotate between multiple kids tables)
            Randomly shuffle the person list before and between each round so the choosing order gets shuffled and families standing together don't get put at the same table first
            //Possibly find a way to let the most restricted players choose first (high playedwith counts or lots of big tables?)
            For each round, each player needs to pick a table to sit at (maybe change loop order or do random so the last player doesn't get stuck).
                A ChooseTable method (in the person class?) can check the tables they haven't visited to see who else is currently there and compare it to who they have played with already.
                    Choose the table with the lowest total PlayedWith count (sum playcounts for all players currently sitting there)
                    The first round could fill the tables in order since the list will be randomized before then
                When a player chooses a table:
                    Add the person to the list of all players for that table
                    Add them to that round for that table
                    Add new PlayedWith entries or increase play counts in existing entries
                    ^UPDATE BOTH SIDES  - so call an update method to add the new person to everyone else at the table's list
                    Increase their PlayedWithCount
                    Add the table to their list
            */
            //Check that the input was valid (players = table spots)
            if (Tables.Sum(t => t.Size) != NumPlayers)
            {
                //TODO getting this for too many rounds
                throw new Exception("The number of players must match the number of table places");
            }

            foreach (Person player in Players)
            {
                if (player.IsChild)
                {
                    Children.Add(player);
                }
                else {
                    AdultPlayers.Add(player);
                }
            }

            for (int round = 0; round < NumRounds; round++)
            {
                foreach (Table t in Tables)
                {
                    //reset each round
                    t.IsFull = false;
                }
                //method for children to be assigned their tables and not added to the adultPlayers list
                foreach (Person player in Children)
                {
                    player.AssignChildTables(ChildTables, round);
                }

                //sort so the players with the most limited options get first pick each round (better than just a random sort)
                this.AdultPlayers.Shuffle();
                this.AdultPlayers = AdultPlayers.OrderByDescending(person => person.PlayedWithCount).ToList<Person>();

                foreach (Person player in AdultPlayers)
                {
                    Table choice = player.ChooseTable(Tables, round);
                    choice.AllPlayers.Add(player);//do I really need this variable?
                    if (choice.Rounds.Count == 0)
                    {
                        Debug.WriteLine("Error Choosing Table");
                        //TODO getting this error for everything after the 2nd round
                        throw new Exception("Error Choosing Table. Please make sure there are as many tables as rounds.");
                    }
                    else
                    {
                        //update counts before adding the player to the round, or they'll try and count themselves
                        choice.UpdatePlayedCounts(player, round);
                        choice.Rounds.ElementAt(round).Add(player);
                        //check if the table is full now
                        if (choice.Rounds.ElementAt(round).Count >= choice.Size)
                        {
                            choice.IsFull = true;
                        }
                        player.Tables.Add(choice);
                    }
                }
            }
            //TODO add an IO method to display the result and/or generate files
            PrintResults();
        }

        private void PrintResults()
        {
            List<Person> SortedPlayers = Players.OrderBy(p => p.PersonID).ToList();
            //write to an output model to display or generate files
            //foreach (Table table in Tables)
            //{
            //    Debug.WriteLine("Table: " + table.Location);
            //    for (int round = 0; round < table.Rounds.Count; round++)
            //    {
            //        int r = round + 1;
            //        Debug.WriteLine("Round: " + r);
            //        foreach (Person player in table.Rounds.ElementAt(round))
            //        {
            //            Debug.WriteLine(player.PersonID);
            //        }
            //    }
            //    Debug.WriteLine("");
            //}
            foreach (Person p in SortedPlayers)
            {
                KeyValuePair<int,int> maxPlays = p.PlayedWith.FirstOrDefault(x => x.Value == p.PlayedWith.Values.Max());
                Debug.WriteLine("Person " + p.PersonID + " max plays " + maxPlays.Value + " with person " + maxPlays.Key);
            }

            //File Output
            List<string> PlayerAssignmentOutput = new List<string>();
            
            foreach (Person p in SortedPlayers)
            {
                PlayerAssignmentOutput.Add("Person " + (p.PersonID+1));
                for(int i=0; i < p.Tables.Count; i++)
                {
                    Table t = p.Tables.ElementAt(i);
                    int round = i + 1;
                    PlayerAssignmentOutput.Add("Round " + round + ": " + t.Location);
                }
                PlayerAssignmentOutput.Add("-----------------------------");
            }
            FileIO.WriteFile(PlayerAssignmentOutput);
        }
    }

    public static class Randomizer
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}