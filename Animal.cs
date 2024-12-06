using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VN_BrackenCave_WPF
{
    public class Animal
    {
        int populationlevel;
        int maxpopulationlevel = 5;
        public string Status = "";
        public string GiveMsg = "";
        public int Populationlevel
        {
            get { return populationlevel; }
            set
            {
                if (populationlevel == value) return;
                int oldvalue = populationlevel;
                // Check if the value if above maximum or below minimum
                if (value < 0)
                    value = 0;
                else if (value > maxpopulationlevel)
                    value = maxpopulationlevel;
                populationlevel = value;
                OnPopulationChange(new PopulationChangedEventArgs(oldvalue, populationlevel));
                
            }
        }
        public event EventHandler<PopulationChangedEventArgs> PopulationChanged;
        protected virtual void OnPopulationChange(PopulationChangedEventArgs e)
        {
            PopulationChanged?.Invoke(this, e);
        }

        public string Name;
        public Item ?ItemToGive;

        public int GetMaxPopLevel()
        {
            return maxpopulationlevel;
        }
        
        public Animal() 
        {
            Name = "Mysterious Species";
            Populationlevel = 0;
        }
        public virtual Item Give()
        {
            //Nothing to give
            if (ItemToGive != null)
            {
                GiveMsg = $"Got {ItemToGive.Name}!";
                return ItemToGive;
            }
            return null;
        }

        public virtual void Animal_PopulationChanged(object sender, PopulationChangedEventArgs e)
        {
            if (e.LastLevel == e.NewLevel)
            {
                Status = "";
                return;
            }
            if (e.LastLevel < e.NewLevel)
            {
                //Population has decreased!
                Console.WriteLine($"Alert: {Name} population level has increased!");
                Status = $"Alert: {Name} population level has increased!";
            }
            else if (e.LastLevel > e.NewLevel)
            {
                //Population has increased!
                Console.WriteLine($"Alert: {Name} population level has decreased!");
                Status = $"Alert: {Name} population level has decreased!";
            }
            else
            {
                Status = "";
            }
        }
    }

    public class PopulationChangedEventArgs : EventArgs
    {
        public readonly int LastLevel;
        public readonly int NewLevel;

        public PopulationChangedEventArgs(int level, int newLevel)
        {
            LastLevel = level;
            NewLevel = newLevel;
        }
    }

    public class Bats : Animal
    {
        public Bats() 
        {
            this.Name = "Bat";
            this.Populationlevel = 5;
            this.ItemToGive = new Guano();
        }

        public override Item Give()
        {
            // Give the Guano
            Console.WriteLine($"Got {ItemToGive.Name}!");
            return base.Give();
        }
    }

    public class Hawks : Animal
    {
        public Hawks()
        {
            this.Name = "Hawk";
            this.Populationlevel = 0;

        }

        public override Item Give()
        {
            Console.WriteLine("I am a hawk, I do not give!!!");
            return base.Give();
        }

        public override void Animal_PopulationChanged(object sender, PopulationChangedEventArgs e)
        {
            //base.Animal_PopulationChanged(sender, e);
            Status = "";
            if (e.LastLevel < e.NewLevel)
            {
                Console.WriteLine("Notice: The Hawks will continue to grow until you repell all of them away");
                Status = "Alert: The Hawks will continue to grow until you repell all of them away";
            }
        }
    }
}
