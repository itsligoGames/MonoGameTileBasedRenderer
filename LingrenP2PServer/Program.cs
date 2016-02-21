using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace LingrenP2PServer
{
    class Program
    {
        public static Timer ServerTimer;

        static void Main(string[] args)
        {
            //creating a new network config, and starting the server (with "Network" class, created below)
            Network.Config = new NetPeerConfiguration("TankWarServer"); // The server and the client program must also use this name, so that can communicate with each other.
            Network.Config.Port = 14242; //one port, if your PC it not using yet
            Network.Server = new NetServer(Network.Config);
            Network.Server.Start();
            ServerTimer = new Timer();
            ServerTimer.Interval = 100;
            ServerTimer.Elapsed += ServerTimer_Elapsed;
            ServerTimer.Start();
            Console.ReadKey();
        }

        private static void ServerTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Network.Update();
            Player.Update();
        }
    }

    class Network // A Basics Network class
    {
        public static NetServer Server; //the Server
        public static NetPeerConfiguration Config; //the Server config
                                                   /*public*/
        static NetIncomingMessage incmsg; //the incoming messages that server can read from clients
        public static NetOutgoingMessage outmsg; //the outgoing messages that clients can receive and read
                                                 /*public*/

        public static void Update()
        {
            while ((incmsg = Server.ReadMessage()) != null) //while the message is received, and is not equal to null...
            {
                switch (incmsg.MessageType) //There are several types of messages (see the Lidgren Basics tutorial), but it is easier to just use it the most important thing the "Data".
                {
                    case NetIncomingMessageType.Data:
                        {
                            //////////////////////////////////////////////////////////////
                            // You must create your own custom protocol with the        //
                            // server-client communication, and data transmission.      //
                            //////////////////////////////////////////////////////////////


                            // 1. step: The first data/message (string or int) tells the program to what is going on, that is what comes to doing.
                            // 2. step: The second tells by name (string) or id (int) which joined client(player) or object(bullets, or other dynamic items) to work.
                            // 3. step: The other data is the any some parameters you want to use, and this is setting and refhreshing the object old (player or item) state.

                            // Now this example I'm use to string (yes you can saving the bandwidth with all messages, if you use integer)
                            string headStringMessage = incmsg.ReadString(); //the first data (1. step)

                            switch (headStringMessage) //and I'm think this is can easyli check what comes to doing
                            {
                                case "connect": //if the firs message/data is "connect"
                                    {
                                        string name = incmsg.ReadString(); //Reading the 2. message who included the name (you can use integer, if you want to store the players in little data)
                                        float x = incmsg.ReadFloat(); //Reading the x position
                                        float y = incmsg.ReadFloat(); // -||- y postion
                                            System.Threading.Thread.Sleep(100); //A little pause to make sure you connect the client before performing further operations
                                            Player.players.Add(new Player(name, new Vector2(x, y), 0)); //Add to player messages received as a parameter
                                            Console.WriteLine(name + " connected." + "\r\n");

                                            for (int i = 0; i < Player.players.Count; i++)
                                            {
                                                // Write a new message with incoming parameters, and send the all connected clients.
                                                outmsg = Server.CreateMessage();

                                                outmsg.Write("connect");
                                                outmsg.Write(Player.players[i].ID);
                                                outmsg.Write(Player.players[i].pozition.X);
                                                outmsg.Write(Player.players[i].pozition.Y);
                                                Server.SendMessage(Network.outmsg, Network.Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                                            }
                                        Console.WriteLine("Players: " + Player.players.Count.ToString());
                                        
                                    }
                                    break;

                                case "movePlayer": //The moving messages
                                    {
                                        //This message is treated as plain UDP (NetDeliveryMethod.Unreliable)
                                        //The motion is not required to get clients in every FPS.
                                        //The exception handling is required if the message can not be delivered in full, 
                                        //just piece, so this time the program does not freeze.
                                        try
                                        {
                                            string name = incmsg.ReadString();
                                            float x = incmsg.ReadFloat();
                                            float y = incmsg.ReadFloat();

                                            Console.WriteLine("Player " + name + " Moved to " + x.ToString() + "," + y.ToString() );

                                            for (int i = 0; i < Player.players.Count; i++)
                                            {
                                                if (Player.players[i].ID.Equals(name))
                                                {
                                                    Player.players[i].pozition = new Vector2(x, y);
                                                    Player.players[i].timeOut = 0; //below for explanation (Player class)...
                                                    break;
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            continue;
                                        }
                                    }
                                    break;

                                case "disconnect": //If the client want to disconnect from server at manually
                                    {
                                        string name = incmsg.ReadString();

                                        for (int i = 0; i < Player.players.Count; i++)
                                        {
                                            if (Player.players[i].ID.Equals(name)) //If the [index].name equaled the incoming message name...
                                            {
                                                Server.Connections[i].Disconnect("bye"); //The server disconnect the correct client with index
                                                System.Threading.Thread.Sleep(100); //Again a small pause, the server disconnects the client actually
                                                Console.WriteLine(name + " disconnected." + "\r\n");

                                                if (Server.ConnectionsCount != 0) //After if clients count not 0
                                                {
                                                    //Sending the disconnected client name to all online clients
                                                    outmsg = Server.CreateMessage();
                                                    outmsg.Write("disconnect");
                                                    outmsg.Write(name);
                                                    Server.SendMessage(Network.outmsg, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                                                }

                                                Player.players.RemoveAt(i); //And remove the player object
                                                i--;
                                                break;
                                            }
                                        }

                                        Console.WriteLine("Players: " + Player.players.Count.ToString());
                                    }
                                    break;
                            }
                        }
                        break;
                }
                Server.Recycle(incmsg); //All messages processed at the end of the case, delete the contents.
            }
        }
    }

   

}
