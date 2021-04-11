using System;
using System.Collections.Generic;
using System.Text;

namespace VFA.Lib.Support
{
    public sealed class CardTypeClaimChance
    {
        public Enums.CardCategory CardCategory { get; set; }

        public double Percentage { get; set; }

        public CardTypeClaimChance(Enums.CardCategory cardCategory, double percentage)
        {
            this.CardCategory = cardCategory;
            this.Percentage = percentage;
        }

    }
}
