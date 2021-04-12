using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToeSignalRWebApp.Models
{
    public class PlayerComparer : IEqualityComparer<Player>
    {
        public bool Equals(Player x, Player y)
        {
            return x.ConnectionId.Equals(y.ConnectionId, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(Player obj)
        {
            return obj.ConnectionId.GetHashCode();
        }
    }
}
