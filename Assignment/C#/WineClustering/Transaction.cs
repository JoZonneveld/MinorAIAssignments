using System;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;

namespace WineClustering
{
    public class Transaction
    {
        private string name;
        private int transactionId;

        public Transaction(string name, int transactionId)
        {
            this.name = name;
            this.transactionId = transactionId;
        }

        public Transaction(string[] values)
        {
            name = values[0];
            transactionId = Convert.ToInt32(values[1]);
        }

        public string getName()
        {
            return this.name;
        }

        public int getTransactionId()
        {
            return this.transactionId;
        }
    }
}