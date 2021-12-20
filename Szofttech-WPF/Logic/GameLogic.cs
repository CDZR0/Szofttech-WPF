﻿using System;
using System.Collections.Generic;
using System.Threading;
using Szofttech_WPF.DataPackage;

namespace Szofttech_WPF.Logic
{
    public class GameLogic
    {
        public static readonly object queueLock = new object();
        //public LinkedList<string> messageQueueVili = new LinkedList<string>();
        public LinkedList<string> messageQueue = new LinkedList<string>();
        private Random rnd = new Random();
        private Player[] players;

        public GameLogic()
        {
            players = new Player[2];
            players[0] = new Player();
            players[1] = new Player();
        }

        private void addMessage(Data data)
        {
            lock (queueLock)
            {
                messageQueue.AddLast(DataConverter.encode(data));
            }
        }

        public void processMessage(Data data)
        {
            if (data == null)
            {
                throw new Exception("Data is null");
            }
            switch (data.GetType().Name)
            {
                case "ChatData":
                    data.RecipientID = 0;
                    addMessage((ChatData)data);
                    data.RecipientID = 1;
                    addMessage((ChatData)data);
                    break;
                case "PlaceShipsData":
                    setPlayerBoard((PlaceShipsData)data);
                    break;
                case "ConnectionData":
                    break;
                case "ShotData":
                    calcShot((ShotData)data);
                    break;
                case "DisconnectData":
                    data.RecipientID = data.ClientID == 1 ? 0 : 1;
                    addMessage((DisconnectData)data);
                    break;
                default:
                    Console.WriteLine("########## ISMERETLEN OSZTÁLY #########");
                    Console.WriteLine("Nincs implementálva a GameLogicban az alábbi osztály: " + data.GetType().Name);
                    throw new Exception("Not implemented");
            }
        }

        private void calcShot(ShotData data)
        {

            int egyik = data.ClientID;
            int masik = egyik == 1 ? 0 : 1;

            ShotData sd = new ShotData(data.ClientID, data.I, data.J);
            sd.RecipientID = masik;
            addMessage(sd);

            CellData cd = new CellData(-1, data.I, data.J, players[masik].Board.cellstatus[data.I, data.J]);
            cd.RecipientID = egyik;
            addMessage(cd);

            if (players[masik].Board.cellstatus[data.I, data.J] == CellStatus.Ship)
            {
                Console.WriteLine("Gamelogic Ship");
                players[masik].Board.cellstatus[data.I, data.J] = CellStatus.ShipHit;
                if (players[masik].Board.isSunk(data.I, data.J))
                {
                    hitNear(egyik, masik, data.I, data.J);
                }
                if (isWin(players[masik]))
                {
                    addMessage(new GameEndedData(GameEndedStatus.Win, egyik));
                    addMessage(new GameEndedData(GameEndedStatus.Defeat, masik));
                }
                else
                {
                    addMessage(new TurnData(egyik));
                }
            }
            else
            {
                addMessage(new TurnData(masik));
            }
        }

        private bool isWin(Player player)
        {
            if (player.Board.hasCellStatus(CellStatus.Ship))
                return false;

            return true;
        }

        private void hitNear(int egyik, int masik, int i, int j)
        {
            foreach (Coordinate nearShipPoint in players[masik].Board.nearShipPoints(i, j))
            {
                CellData cd = new CellData(-1, nearShipPoint.X, nearShipPoint.Y, players[masik].Board.cellstatus[nearShipPoint.X, nearShipPoint.Y]);
                cd.RecipientID = egyik;
                addMessage(cd);
                ShotData sd = new ShotData(egyik, nearShipPoint.X, nearShipPoint.Y);
                sd.RecipientID = masik;
                addMessage(sd);

                //int count;
                //do
                //{
                //    lock (queueLock)
                //    {
                //        count = messageQueue.Count;
                //    }
                //    if (count>0) Thread.Sleep(1);                    
                //} while (count > 0);
            }
        }

        private void setPlayerBoard(PlaceShipsData data)
        {
            Console.WriteLine(data.Board);
            if (data.ClientID == 0)
            {
                players[0].Identifier = data.ClientID;
                players[0].isReady = true;
                players[0].Board = data.Board;
            }
            else
            {
                players[1].Identifier = data.ClientID;
                players[1].isReady = true;
                players[1].Board = data.Board;
            }

            if (players[0].isReady == true && players[1].isReady == true)
            {
                addMessage(new TurnData(rnd.Next(2)));
            }
        }
    }
}
