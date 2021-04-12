using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToeSignalRWebApp.Models
{
    public class GameMapping
    {
        private readonly Dictionary<string, Game> groups = new();

        public bool GameExist(string gameName)
        {
            if (this.groups.ContainsKey(gameName))
            {
                return true;
            }
            return false;
        }

        public void Add(string name, Game game)
        {
            lock (groups)
            {
                this.groups.Add(name, game);

            }
        }

        public GameInfo AddGame(string connectionId, string playerName)
        {
            lock (groups)
            {
                if (groups.ContainsKey(connectionId))
                {
                    return null;
                }
                else
                {
                    var game = new Game();
                    var player = new Player()
                    {
                        ConnectionId = connectionId,
                        Symbol = "x",
                        Name = playerName
                    };
                    game.Players.Add(player);
                    this.groups.Add(connectionId, game);
                    return new GameInfo(game);
                }

            }
        }

        public void Delete(string gameName)
        {
            lock (groups)
            {
                this.groups.Remove(gameName);
            }
        }

        public void AddPlayerToGame(string gameName, Player player)
        {
            lock (groups)
            {
                var game = groups.First(x => x.Key == gameName);
                if (game.Value.Players.Count == 1)
                {
                    lock (game.Value.Players)
                    {
                        game.Value.Players.Add(player);
                    }
                }
            }
        }

        public GameInfo AddPlayerToGame(string gameName, string connectionId, string playerName)
        {
            lock (groups)
            {
                var game = groups.First(x => x.Key == gameName).Value;
                if (game.Players.Count == 1)
                {
                    lock (game.Players)
                    {
                        var player = new Player()
                        {
                            ConnectionId = connectionId,
                            Symbol = "o",
                            Name = playerName
                        };
                        game.Players.Add(player);
                        return new GameInfo(game);
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public List<string> GetGamesWithOnePlayer()
        {
            return groups.Where(x => x.Value.Players.Count == 1).Select(x => x.Key).ToList();
        }

        public GameInfo Move(string connectionId, int x, int y)
        {
            var gameName = this.GetGameNameByConnectionId(connectionId);
            if (gameName==null)
            {
                return null;
            }
            var player = this.GetPlayerByConnectionId(connectionId);
            if (player==null)
            {
                return null;
            }
            var game = groups[gameName];
            var playerIsOnTurn = player.Symbol == game.Turn;
            if (playerIsOnTurn)
            {
                var gameBoard = game.GameBoard;
                lock (gameBoard)
                {
                    gameBoard[x, y] = player.Symbol;
                }
                this.ToggleTurn(game);
                this.TryForWinner(gameName);
                this.TryForFinish(gameName);
            }
            return new GameInfo(game);
        }

        public string GetGameNameByConnectionId(string connectionId)
        {
            return groups.FirstOrDefault(x => x.Value.Players.FirstOrDefault(x => x.ConnectionId == connectionId) != null).Key;
        }

        private void TryForWinner(string gameName)
        {
            var game = groups[gameName];
            var gameBoard = game.GameBoard;
            string winner = string.Empty;
            //Try horizontals
            for (int i = 0; i < 3; i++)
            {
                if (gameBoard[i, 0] != string.Empty && gameBoard[i, 0] == gameBoard[i, 1] && gameBoard[i, 0] == gameBoard[i, 2])
                {
                    winner = gameBoard[i, 0];
                }
            }
            //Try verticals
            for (int i = 0; i < 3; i++)
            {
                if (gameBoard[0, i] != string.Empty && gameBoard[0, i] == gameBoard[1, i] && gameBoard[0, i] == gameBoard[2, i])
                {
                    winner = gameBoard[0, i];
                }
            }
            //Try diagonals
            if (gameBoard[0, 0] != string.Empty && gameBoard[0, 0] == gameBoard[1, 1] && gameBoard[0, 0] == gameBoard[2, 2])
            {
                winner = gameBoard[0, 0];
            }
            if (gameBoard[2, 0] != string.Empty && gameBoard[2, 0] == gameBoard[1, 1] && gameBoard[2, 0] == gameBoard[0, 2])
            {
                winner = gameBoard[2, 0];
            }

            if (winner != string.Empty)
            {
                var player = game.Players.FirstOrDefault(x => x.Symbol == winner);
                game.Winner = player;
            }
        }

        private void TryForFinish(string gameName)
        {
            var game = groups[gameName];
            var gameBoard = game.GameBoard;
            //if have winner game is finished
            if (game.Winner != null)
            {
                game.IsFinished = true;
                return;
            }
            //If have free space on the board game is not finished
            bool tryGameIsFinished = true;
            for (int i = 0; i < gameBoard.GetLength(0); i++)
            {
                for (int k = 0; k < gameBoard.GetLength(1); k++)
                {
                    if (gameBoard[i, k] == string.Empty)
                    {
                        return;
                    }
                }
            }
            if (tryGameIsFinished)
            {
                game.IsFinished = true;
            }
        }

        private Player GetPlayerByConnectionId(string connectionId)
        {
            var game = groups.FirstOrDefault(x => x.Value.Players.FirstOrDefault(x => x.ConnectionId == connectionId) != null).Value;
            var player = game.Players.FirstOrDefault(x => x.ConnectionId == connectionId);
            return player;
        }

        private void ToggleTurn(Game game)
        {
            if (game.Turn == "x")
            {
                game.Turn = "o";
            }
            else if (game.Turn == "o")
            {
                game.Turn = "x";
            }
        }
    }
}
