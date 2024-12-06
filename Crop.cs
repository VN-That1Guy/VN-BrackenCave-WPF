using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace VN_BrackenCave_WPF
{
    public class Crop
    {
        private int densitylevel = 0;
        private int maxdensity = 5;

        public int Densitylevel
        {
            get { return densitylevel; }
            set 
            {
                if (densitylevel != value)
                {
                    if (value > maxdensity)
                        value = maxdensity;
                    else if (value <= 0)
                    {
                        bGrown = false;
                        bPlanted = false;
                        value = 0;
                    }
                    densitylevel = value;
                }
            }
        }
        public string Name { get; set; }
        public string GiveMsg{ get; set; }
        public bool bTaken = false;
        public bool bFertilized = false;
        public bool bGrown = false;
        public bool bPlanted = false;
        public Item ?ItemToGive;

        public int MaxDensity() => maxdensity;
        public virtual Item Give()
        {
            Console.WriteLine($"Got {ItemToGive.Name}!");
            GiveMsg = $"Got {ItemToGive.Name}!";
            return ItemToGive;
        }
    }

    public class Corn : Crop
    {
        public Corn() 
        {
            this.Name = "Corn";
            this.ItemToGive = new Cornseed();
        }
    }

    public class Cotton : Crop
    {
        public Cotton()
        {
            this.Name = "Cotton";
            this.ItemToGive = new Cottonseed();
        }
    }
}
