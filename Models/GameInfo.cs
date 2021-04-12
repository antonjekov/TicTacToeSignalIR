using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToeSignalRWebApp.Models
{
    public class GameInfo
    {
        public GameInfo(Game game)
        {
            //this.GameName = game.GameName;
            this.Players = game.Players.ToList();
            this.Winner = game.Winner;
            this.Turn = game.Turn;
            this.IsFinished = game.IsFinished;
            this.GameBoard = this.ConvertTwoDimentionalArrayToDictionary(game.GameBoard);
        }

        //public string GameName { get; }
        public List<Player> Players { get; }
        public Player Winner { get; }
        public string Turn { get; }
        public bool IsFinished { get; }
        public Dictionary<string,string> GameBoard { get; }

        private Dictionary<string,string> ConvertTwoDimentionalArrayToDictionary(string[,] gameBoard)
        {
            Dictionary<string, string> info = new();
            for (int i = 0; i < gameBoard.GetLength(0); i++)
            {
                for (int j = 0; j < gameBoard.GetLength(1); j++)
                {
                    info.Add($"{i}{j}", gameBoard[i, j]);
                }
            }

            return info;
        }
    }
}
