using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToeSignalRWebApp.Models
{
    public class Player
    {
        public string ConnectionId { get; set; }

        public string Symbol { get; set; }

        public string Name { get; set; }
    }
}
