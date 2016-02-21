///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  For a more detailed comments found in the server program (ServerAplication), so if you have not done it, check out.  //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using Lidgren.Network;
using NetworkClient.Network.Player;

namespace NetworkClient.Network
{
    class Network // Roughly the same, except that here NetClient was used.
    {
        public static NetClient Client;
        public static NetPeerConfiguration Config;
        /*public*/ static NetIncomingMessage incmsg;
        public static NetOutgoingMessage outmsg;
        public static string ClientID;
        public static void Update()
        {
            //The biggest difference is that the client side of things easier, 
            //since we will only consider the amount of player object is created, 
            //so there is no keeping track of separate "Server.Connections" as the server side.
            while ((incmsg = Client.ReadMessage()) != null)
            {
                switch (incmsg.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        {
                            string headStringMessage = incmsg.ReadString();

                            switch (headStringMessage)
                            {
                                case "connect":
                                    {
                                        string name = incmsg.ReadString();
                                        float x = incmsg.ReadFloat();
                                        float y = incmsg.ReadFloat();
                                        if(name != ClientID)
                                            GamePlayer.otherPlayers.Add(new GamePlayer(name, new Vector2(x, y)));
                                    }
                                    break;

                                case "movePlayer":
                                    {
                                        try
                                        {
                                            string pname = incmsg.ReadString();
                                            float x = incmsg.ReadFloat();
                                            float y = incmsg.ReadFloat();
                                            GamePlayer moved = GamePlayer.otherPlayers.Where(p => p.Name == pname).FirstOrDefault();
                                            if(moved != null)
                                                moved.Position = new Vector2(x, y);
                                        }
                                        catch
                                        {
                                            continue;
                                        }
                                    }
                                    break;

                                case "disconnect": //Clear enough :)
                                    {
                                        string name = incmsg.ReadString();

                                        GamePlayer left = GamePlayer.otherPlayers.Where(p => p.Name == name).FirstOrDefault();
                                        if (left != null)
                                            GamePlayer.otherPlayers.Remove(left);
                                    }
                                    break;

                                case "deny": //If the name on the message is the same as ours
                                    {
                                        
                                    }
                                    break;
                            }
                        }
                        break;
                }
                Client.Recycle(incmsg);
            }
        }
    }
}
