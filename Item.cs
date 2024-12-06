using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VN_BrackenCave_WPF
{
    public enum ItemType
    {
        None,
        Fertilizer,
        Seed,
        Spray
    }
    public class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        private int amount;
        public int Amount // Quantity: Used in inventory
        {
            get {  return amount; }
            set
            {
                if (amount != value)
                {
                    if (value < 0)
                        value = 0;
                    amount = value;
                }
            }
        }
        
        public int Value { get; set; } // How much an item costs/sells for
        public ItemType Type;
        public BitmapImage Picture {  get; set; }
        public string path = "media/blank.bmp";

        public void SetPicture()
        {
            Picture = new BitmapImage(new Uri(path, UriKind.Relative));
        }
        public Item()
        {
            SetPicture();
        }
    }

    class Guano : Item
    {
        public Guano()
        {
            Name = "Guano";
            Description = "Substance produced by Bats that was once used to make gunpowder. Can be used as fertilizer to grow more crops. Crops grown from nothing will take a day to grow...";
            Value = 5;
            Type = ItemType.Fertilizer;
            path = "media/item_guano.bmp";
            SetPicture();
        }
    }

    class Cornseed : Item
    {
        public Cornseed()
        {
            this.Name = "Corn Seed";
            this.Description = "Seed that can be used to grow Corn. Maintains the worm population";
            this.Value = 2;
            this.Type = ItemType.Seed;
            this.path = "media/item_cornseed.bmp";
            SetPicture();
        }
    }

    class Cottonseed : Item
    {
        public Cottonseed()
        {
            this.Name = "Cotton Seed";
            this.Description = "Seed that can be used to grow Cotton. Maintains the worm population.";
            this.Value = 2;
            this.Type = ItemType.Seed;
            this.path = "media/item_cottonseed.bmp";
            SetPicture();
        }
    }

    class HawkRepellent : Item
    {
        public HawkRepellent()
        {
            this.Name = "Anti-Hawk Spray";
            this.Description = "A spray that repell hawks! Hawks are very disruptive to this ecosystem.";
            this.Value = 30;
            this.Type = ItemType.Spray;
            this.path = "media/item_repellant.bmp";
            SetPicture();
        }
    }
}
