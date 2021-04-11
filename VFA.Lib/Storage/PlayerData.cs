using System;
using System.Collections.Generic;
using System.Text;
using VFA.Lib.Support;

namespace VFA.Lib.Storage
{
    public class PlayerData
    {
        public List<Player> GetAllPlayers()
        {
            var player1 = new Player { Name = "Ronaldo" , };
            var player2 = new Player { Name = "Messi" };
            var player3 = new Player { Name = "Pele" };

            return new List<Player>()
            {
                player1, player2, player3
            };
        }
    }
}
