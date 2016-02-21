///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  For a more detailed comments found in the server program (ServerAplication), so if you have not done it, check out.  //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Lidgren.Network;
using NetworkClient.Network;
using AnimatedSprite;

namespace NetworkClient.Network.Player
{
    public class GamePlayer //The Player class and instant constructor,
                 //+ 2 Rectangle, because it is already in the game, and here we must draw,
                 //but in this case, however, no need for the timeout.
    {
        public static string name;
        public static Vector2 position;

        public static List<GamePlayer> otherPlayers = new List<GamePlayer>();

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public Vector2 Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
            }
        }

        public GamePlayer(string Gname, 
                      Vector2 pozition)
        {
            Name = Gname;
            Position = pozition;
        }
        
        public static void Update()
        {
                    Network.outmsg = Network.Client.CreateMessage();
                    Network.outmsg.Write("movePlayer");
                    Network.outmsg.Write(name);
                    Network.outmsg.Write(position.X);
                    Network.outmsg.Write(position.Y);
                    Network.Client.SendMessage(Network.outmsg, NetDeliveryMethod.Unreliable);
            }


   }
}
