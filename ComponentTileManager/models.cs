using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComponentTileManager
{
    public class player
    {
        public string playerId = Guid.NewGuid().ToString();
        public int XP { get; set; }
        public string GamerTag { get; set; }
        public string firstName { get; set; }

    }

    public class Achievement
    {
        public string ID = Guid.NewGuid().ToString();
        public string InGame { get; set; }
        public string Name { get; set; }
    }

    public class PlayerAchievement
    {
        public string AchievementID = Guid.NewGuid().ToString();
        public string PlayerID { get; set; }
    }

    public class GameData
    {
        public string GameID = Guid.NewGuid().ToString();
        public string GameName { get; set; }
    }

    public class GameScore
    {
        public string ScoreID = Guid.NewGuid().ToString();
        public string GameID { get; set; }
        public string PlayerID { get; set; }
        public int score { get; set; }
    }

}
