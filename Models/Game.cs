using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToeSignalRWebApp.Models
{
    public class Game
    {
        public Game()
        {
            //this.GameName = gameName;
            this.Players = new HashSet<Player>(new PlayerComparer());
            this.GameBoard = this.CreateEmptyGameBoard();
            this.Turn = "x";
            this.IsFinished = false;
        }

        //public string GameName { get; }

        public HashSet<Player> Players { get; set; }

        public string Turn { get; set; }

        public string[,] GameBoard { get; set; }

        public Player Winner { get; set; }

        public bool IsFinished { get; set; }

        private string[,] CreateEmptyGameBoard()
        {
            var board = new string[3, 3];
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int k = 0; k < board.GetLength(1); k++)
                {
                    board[i, k] = string.Empty;
                }
            }
            return board;
        }
    }
}
