﻿using System;
using Szofttech_WPF.DataPackage;

namespace Szofttech_WPF.EventArguments.Chat
{
    public class GameEndedArgs : EventArgs
    {
        public GameEndedStatus GameEndedStatus { get; set; }

        public GameEndedArgs(GameEndedStatus gameEndedStatus)
        {
            GameEndedStatus = gameEndedStatus;
        }
    }
}