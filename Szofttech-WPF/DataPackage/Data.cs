﻿namespace Szofttech_WPF.DataPackage
{
    public abstract class Data
    {
        public int clientID;
        public int recipientID;     

        public Data(int clientID)
        {
            this.clientID = clientID;
            this.recipientID = -1;
        }

        public int getClientID()
        {
            return clientID;
        }

        public int getRecipientID()
        {
            return recipientID;
        }

        public void setRecipientID(int recipientID)
        {
            this.recipientID = recipientID;
        }
    }
}
