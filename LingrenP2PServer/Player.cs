using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingrenP2PServer
{
    class Player //The Player class and instant constructor
    {
        public string ID;
        public Vector2 pozition;
        public int timeOut; //This disconnects the client, even if no message from him within a certain period of time and not been reset value.
        public static int InactivePeriod = 1800;
        public static List<Player> players = new List<Player>();

        public Player(string name,
                      Vector2 pozition,
                      int timeOut)
        {
            this.ID = name;
            this.pozition = pozition;
            this.timeOut = timeOut;
        }

        public static void Update()
        {
            if (Network.Server.ConnectionsCount == players.Count) //If the number of the player object actually corresponds to the number of connected clients.
            {
                for (int i = 0; i < players.Count; i++)
                {
                    players[i].timeOut++; //This data member continuously counts up with every frame/tick.

                    //The server simply always sends data to the all players current position of all clients.
                    Network.outmsg = Network.Server.CreateMessage();

                    Network.outmsg.Write("playerMove");
                    Network.outmsg.Write(players[i].ID);
                    Network.outmsg.Write(players[i].pozition.X);
                    Network.outmsg.Write(players[i].pozition.Y);

                    Network.Server.SendMessage(Network.outmsg, Network.Server.Connections, NetDeliveryMethod.Unreliable, 0);

                    if (players[i].timeOut > InactivePeriod) //If this is true, so that is the player not sent information with himself
                    {
                        //The procedure will be the same as the above when "disconnect" message
                        Network.Server.Connections[i].Disconnect("bye");
                        Console.WriteLine(players[i].ID + " is timed out." + "\r\n");
                        System.Threading.Thread.Sleep(100);

                        if (Network.Server.ConnectionsCount != 0)
                        {
                            Network.outmsg = Network.Server.CreateMessage();

                            Network.outmsg.Write("disconnect");
                            Network.outmsg.Write(players[i].ID);

                            Network.Server.SendMessage(Network.outmsg, Network.Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                        }

                        players.RemoveAt(i);
                        i--;
                        Console.WriteLine("Players: " + players.Count.ToString());
                        break;
                    }
                }
            }
        }
    }
}
