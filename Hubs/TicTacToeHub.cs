using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TicTacToeSignalRWebApp.Models;

namespace TicTacToeSignalRWebApp.Hubs
{
    [Authorize]
    public class TicTacToeHub : Hub
    {
       private readonly static GameMapping groups = new ();

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = this.Context.ConnectionId;
            var name = this.Context.User.Identity.Name;
            var gameName = groups.GetGameNameByConnectionId(connectionId);
            this.Clients.GroupExcept(gameName, new List<string>() { this.Context.ConnectionId }).SendAsync("Message", $"Player {name} leave the game. Congratulations you WIN!!!");
            this.Clients.Group(gameName).SendAsync("Finish");
            groups.Delete(gameName);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task Play()
        {
            var gamesWithOnePlayer = groups.GetGamesWithOnePlayer();
            if (gamesWithOnePlayer.Count==0)
            {
                var gameInfo = groups.AddGame(this.Context.ConnectionId, this.Context.User.Identity.Name);
                if (gameInfo!=null)
                {
                    await Groups.AddToGroupAsync(this.Context.ConnectionId, this.Context.ConnectionId);
                    await this.Clients.Caller.SendAsync("ChangeGameBoard", gameInfo.GameBoard);
                    await this.Clients.Caller.SendAsync("Message", "Waiting for partner ...");
                }                
            }
            else if(gamesWithOnePlayer.Count > 0)
            {
                var gameName = gamesWithOnePlayer.FirstOrDefault();
                var userName = this.Context.User.Identity.Name;
                var gameInfo = groups.AddPlayerToGame(gameName, this.Context.ConnectionId, userName);
                await Groups.AddToGroupAsync(this.Context.ConnectionId, gameName);
                var playerOnTurn = gameInfo.Players.First();
                await this.Clients.Caller.SendAsync("ChangeGameBoard", gameInfo.GameBoard);
                await this.Clients.Group(gameName).SendAsync("GameStart");
                await this.Clients.Group(gameName).SendAsync("Message", $"Player {playerOnTurn.Name} starts the game");
            }
        }
        
        public async Task Move(int x, int y)
        {
            var gameName = groups.GetGameNameByConnectionId(this.Context.ConnectionId);            
            var connectionId = this.Context.ConnectionId;
            var gameInfo = groups.Move(connectionId, x, y);
            if (gameInfo==null)
            {
                return;
            }
            await this.Clients.Group(gameName).SendAsync("ChangeGameBoard", gameInfo.GameBoard);
            await this.Clients.Caller.SendAsync("Message", "Waiting oponent to play ...");
            await this.Clients.GroupExcept(gameName, new List<string>() { this.Context.ConnectionId }).SendAsync("Message", "It's your turn to play.");
            if (gameInfo.Winner!=null)
            {
                await this.Clients.Caller.SendAsync("Message", "Congratulations you WIN !!!");
                await this.Clients.GroupExcept(gameName, new List<string>() { this.Context.ConnectionId }).SendAsync("Message", $"Sorry you lose. Winner is {gameInfo.Winner.Name}");
                
            }
            if (gameInfo.IsFinished)
            {
                await this.Clients.Group(gameName).SendAsync("Finish");
                groups.Delete(gameName);
            }         
        }                
    }
}
