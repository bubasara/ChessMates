using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMates.Models
{
    public interface IPairingAlgorithm
    {
        public void Pair(string connString);
    }
}
