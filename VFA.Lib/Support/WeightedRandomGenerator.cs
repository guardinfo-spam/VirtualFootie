﻿using System;
using System.Collections.Generic;

namespace VFA.Lib.Support
{
    public sealed class WeightedRandomGenerator<T>
    {
        private struct Entry
        {
            public double accumulatedWeight;
            public double price;
            public T item;
        }

        private List<Entry> entries = new List<Entry>();
        private double accumulatedWeight;
        private Random rand = new Random();

        public void AddEntry(T item, double weight, double price)
        {
            accumulatedWeight += weight;
            entries.Add(new Entry { item = item, accumulatedWeight = accumulatedWeight, price = price });
        }

        public (T, double) GetRandom()
        {
            double r = rand.NextDouble() * accumulatedWeight;

            foreach (Entry entry in entries)
            {
                if (entry.accumulatedWeight >= r)
                {
                    return (entry.item, entry.price);
                }
            }

            throw new Exception();
        }
    }
}




