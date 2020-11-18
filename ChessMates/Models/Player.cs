using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMates.Models
{
    public class Player
    {
        public int ID_Player;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int FideRank { get; set; }
        public int BirthYear { get; set; }
        public string Country { get; set; }

        public Player()
        {
        }

        public Player(/*int id_player,*/ string firstName, string lastName, int fideRank,
            int birthYear, string country)
        {
            /*ID_Player = id_player;*/
            FirstName = firstName;
            LastName = lastName;
            FideRank = fideRank;
            BirthYear = birthYear;
            Country = country;
        }
    }
}
