using System;

namespace WineClustering
{
    public class Wine
    {
        private int offerId;
        private string campaign;
        private string varietal;
        private int minimumQty;
        private int discout;
        private string origin;
        private bool pastPeak;

        public Wine(int offerId, string campaign, string varietal, int minimumQty, int discout, string origin, bool pastPeak)
        {
            this.offerId = offerId;
            this.campaign = campaign;
            this.varietal = varietal;
            this.minimumQty = minimumQty;
            this.discout = discout;
            this.origin = origin;
            this.pastPeak = pastPeak;
        }

        public Wine(string[] values)
        {
            offerId = Convert.ToInt32(values[0]);
            campaign = values[1];
            varietal = values[2];
            minimumQty = Convert.ToInt32(values[3]);
            discout = Convert.ToInt32(values[4]);
            origin = values[5];
            pastPeak = values[6] != "FALSE";
        }

        public int getOfferId()
        {
            return offerId;
        }
    }
}