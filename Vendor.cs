using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VN_BrackenCave_WPF
{
    public class Vendor
    {
        public string Name = "Trader";
        public List<Item> Items = new List<Item>();

        public Vendor()
        {
            Items.Add(new Guano());
            Items.Add(new Cornseed());
            Items.Add(new Cottonseed());
            Items.Add(new HawkRepellent());
        }

        public Item Give(Item item)
        {
            return Items.Find(x=>x.Equals(item));
        }
    }
}
