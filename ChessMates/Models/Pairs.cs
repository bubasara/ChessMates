using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMates.Models
{
    public class Pairs
    {
        public int ID { get; set; }
        public int Round { get; set; }
        public int Player1 { get; set; }
        public int Player2 { get; set; }
        public string Pieces1 { get; set; }
        public string Pieces2 { get; set; }

        public Pairs(/*int id, */int round, int player1, int player2, string pieces1, string pieces2)
        {
            /*ID = id;*/
            Round = round;
            Player1 = player1;
            Player2 = player2;
            Pieces1 = pieces1;
            Pieces2 = pieces2;
        }
    }
}
