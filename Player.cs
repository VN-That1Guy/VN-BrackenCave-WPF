using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VN_BrackenCave_WPF
{
    public class Player
    {
        public string Name = "Player";
        public int Currency = 0;
        public List<Item> Inventory = new List<Item>();

        public Player()
        {
            this.Inventory.Add(new Guano() { Amount = 0});
            this.Inventory.Add(new Cornseed() { Amount = 1 });
            this.Inventory.Add(new Cottonseed() { Amount = 1 });
            this.Inventory.Add(new HawkRepellent() { Amount = 1 });
        }

        public void Use(Item item)
        {
            Item CurrentItem = Inventory.Find(x => x.Name == item.Name);
            if (CurrentItem == null)
            {
                Console.WriteLine($"You do not have {item.Name}!");
                return;
            }
            if (CurrentItem.Amount > 0)
                CurrentItem.Amount--;
            else
                Console.WriteLine($"You do not have any more of {CurrentItem.Name}!");
        }

        public string ShowInventory()
        {
            string output = "";
            foreach (Item item in Inventory)
            {
                output += $"* {item.Name} [{item.Type}] - {item.Value.ToString("c")} (x{item.Amount})\n";
            }
            return output;
        }
    }
}
