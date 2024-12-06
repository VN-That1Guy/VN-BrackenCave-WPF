using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace VN_BrackenCave_WPF
{
    public class Game
    {
        private int Day;

        public Player Client = new Player();
        public Vendor Seller = new Vendor();

        private Random RNG = new Random();
        private float RandomEventChance = 0.25f;
        private int DailySalary = 30;

        private int RNGValue;

        private List<Crop> Crops = new List<Crop>();
        private List<Animal> Animals = new List<Animal>();
        private Hawks? Hawk;
        private Bats? Bat;

        private int TotalCropDensity = 0;

        public string RandomEventMsg = "";

        private string path = "../../../data/";
        public Game()
        {
            Setup();
        }

        public List<Crop> GetCrops() => Crops;

        public List<Animal> GetAnimals() => Animals;

        public int GetTotalCropDensity()
        {
            UpdateTotalCropDensity();
            return TotalCropDensity;
        }

        private void Setup()
        {
            Day = 0;
            LoadEcosystemXML("Ecosystem.xml");
            UpdateTotalCropDensity();
            Hawk = (Hawks)Animals.Find(x => x.Name.Equals("Hawk"));
            Bat = (Bats)Animals.Find(x => x.Name.Equals("Bat"));
            //Start();
        }

        public Hawks GetHawk() => Hawk;

        public Bats GetBat() => Bat;

        public void UpdateTotalCropDensity()
        {
            TotalCropDensity = Crops.Sum(crop => crop.Densitylevel);
            if (TotalCropDensity <= 0)
            {
                Console.WriteLine("Alert! There are no more crops to for the worms to feast upon.\nThis will affect the bat population.");
            }
        }
        public void GetItemFrom(object actor)
        {
            if (actor is Animal)
            {
                Animal tempanim = (Animal)actor;
                if (tempanim.ItemToGive != null)
                        Client.Inventory.Find(x => x.Name.Equals(tempanim.ItemToGive.Name)).Amount++;
            }
            else if (actor is Crop)
            {
                Crop tempcrop = (Crop)actor;
                if (tempcrop.ItemToGive != null)
                        Client.Inventory.Find(x => x.Name.Equals(tempcrop.ItemToGive.Name)).Amount++;
            }
            else
                Console.WriteLine("Error! GetItem is being called but actor is invalid...");
        }
        // Debug/Test function from the ConsoleApp version
        private void Start()
        {
            Console.WriteLine("Day " + GetDay() + $"\nYour Wallet: {Client.Currency.ToString("c")}\nTotal Crop Density {TotalCropDensity}");
            foreach (var c in Crops)
            {
                Console.WriteLine(c.Name + $"\nDensity:{c.Densitylevel}");
                c.Give();
            }
            foreach (var anim in Animals)
            {
                Console.WriteLine(anim.Name + $"\nPopulation:{anim.Populationlevel}");
                anim.Give();
            }
            Console.WriteLine($"\nI am seller man, seller of things, take a look at my goodies:\n");
            Console.WriteLine(DisplayGoodies());
            for (int i = 0; i < 7; i++)
            {
                NextDay();
                foreach (var c in Crops)
                {
                    Console.WriteLine(c.Name + $"\nDensity:{c.Densitylevel}");
                    GetItemFrom(c);
                }
                foreach (var anim in Animals)
                {
                    Console.WriteLine(anim.Name + $"\nPopulation:{anim.Populationlevel}");
                    GetItemFrom(anim);
                }
                Console.WriteLine();
            }
            Console.WriteLine($"Okay, bye bye!\n{Client.ShowInventory()}");
        }

        public string DisplayGoodies()
        {
            string goodslist = "";
            foreach (var inv in Seller.Items)
            {
                goodslist += $"* {inv.Name} - {inv.Value} - {inv.Type}\n{inv.Description}\n";
            }
            return goodslist;
        }

        public void NextDay()
        {
            Day++;
            Client.Currency += DailySalary;
            RandomEventMsg = "";
            Console.WriteLine($"You have been paid {DailySalary.ToString("c")}");
            RandomEventMsg = $"You have been paid {DailySalary.ToString("c")}\n";
            double ChanceValue = RNG.NextDouble();
            Console.WriteLine("Random Event chance value: " + ChanceValue.ToString());
            if (ChanceValue < RandomEventChance)
                RandomEvent();

            foreach (var c in Crops)
            {
                if (c.Densitylevel > 0 && c.bGrown)
                    c.Densitylevel--;
                else if (c.Densitylevel <= 0) // Crop Density is 0, trying to grow a new crop
                {
                    if (!c.bGrown)
                    {
                        if (c.bPlanted)
                        {
                            if (!c.bFertilized)
                            {
                                RandomEventMsg = "Notice: Crops did not grow, did you fertilze them yesterday?\n";
                            }
                            else
                            {
                                c.Densitylevel = 0; // In any case that the Density level is somehow less than 0 still
                                c.Densitylevel++;
                                c.bPlanted = false;
                                c.bGrown = true;
                                c.bFertilized = false;
                            }
                        }
                        else
                        {
                            RandomEventMsg = "Notice: Crops have not grown, please plant the seeds\n";
                        }    
                    }
                }
            }
            UpdateTotalCropDensity();
            foreach (var anim in Animals)
            {
                if (anim is not Hawks)
                {   
                    if (((anim.Populationlevel > 0) && (TotalCropDensity <= 0)) || Hawk.Populationlevel > 0)
                        anim.Populationlevel--;
                    else
                        anim.Populationlevel++;
                }
                else if (anim is Hawks && anim.Populationlevel > 1) // Random Event already increases the level by 1, let's not make that increase the population level by 2 when they are first spotted
                {
                    anim.Populationlevel++;
                }
            }
            Console.WriteLine("Day " + GetDay() + $"\nYour Wallet: {Client.Currency.ToString("c")}\nTotal Crop Density {TotalCropDensity}");
        }

        private void RandomEvent()
        {
            Console.WriteLine("Something has happened!");
            RandomEventMsg += $"Something has happened\n";
            switch(RNG.Next(2))
            {
                case 0:
                    Console.WriteLine("Rain! Crops are growing");
                    RandomEventMsg += $"Day {GetDay()}:Rain! the Crops are growing!\n";
                    foreach (var c in Crops)
                    {
                        if (c.bGrown)
                            c.Densitylevel++;
                        if (c.bPlanted && c.bFertilized)
                            c.Densitylevel++;
                    }
                    break;
                case 1: 
                    Console.WriteLine("Hawks have been spotted! This will trouble the Bat population...");
                    RandomEventMsg += $"Day {GetDay()}: Hawks have been spotted! This will trouble the Bat population...\n";
                    foreach (var anim in Animals)
                    {
                        if (anim is Hawks && anim.Populationlevel >= 0)
                            anim.Populationlevel++;
                    }
                    break;
                case 2: 
                    Console.WriteLine("Someone has snuck in and has disturbed the Ecosystem!");
                    RandomEventMsg += $"Day {GetDay()}: Someone has snuck in and has disturbed the Ecosystem!\n";
                    foreach (var anim in Animals)
                    {
                        if (anim is not Hawks && anim.Populationlevel > 0)
                            anim.Populationlevel--;
                    }
                    foreach (var c in Crops)
                    {
                        c.Densitylevel--;
                    }
                    break;
            }
        }

        public int GetDay()
        {
            return Day;
        }

        public void LoadEcosystemXML(string fileName)
        {
            XmlDocument doc = new XmlDocument();
            if (File.Exists(path + fileName))
            {
                Console.WriteLine("File loaded!");
                doc.Load(path + fileName);
                XmlNode root = doc.DocumentElement;
                XmlNodeList EcoList = root.SelectNodes("/Ecosystem/Animals");
                XmlNodeList tempnodelist;

                foreach (XmlElement animal in EcoList)
                {
                    tempnodelist = animal.ChildNodes;
                    Animal tempanimal = new Animal();
                    foreach (XmlElement tempnode in tempnodelist)
                    {
                        switch (tempnode.GetAttribute("Name"))
                        {
                            case "Bats":
                                tempanimal = new Bats();
                                break;
                            case "Hawks":
                                tempanimal = new Hawks();
                                break;
                            default:
                                Console.WriteLine("(Animal) Root not found!");
                                break;
                        }
                        int.TryParse(tempnode.GetAttribute("Populationlevel"), out int poplevel);
                            tempanimal.Populationlevel = poplevel;
                        tempanimal.PopulationChanged += tempanimal.Animal_PopulationChanged;
                        Animals.Add(tempanimal);
                    }
                }

                EcoList = root.SelectNodes("/Ecosystem/Crops");

                foreach (XmlElement crop in EcoList)
                {
                    Crop tempcrop = new();
                    tempnodelist = crop.ChildNodes;
                    foreach (XmlElement tempnode in tempnodelist)
                    {
                        switch (tempnode.GetAttribute("Name"))
                        {
                            case "Corn":
                                tempcrop = new Corn();
                                break;
                            case "Cotton":
                                tempcrop = new Cotton();
                                break;
                            default:
                                Console.WriteLine("(Crop) Root not found!");
                                break;
                        }
                        int.TryParse(tempnode.GetAttribute("Densitylevel"), out int denselevel);
                        tempcrop.Densitylevel = denselevel;
                        tempcrop.bGrown = tempnode.GetAttribute("bGrown") == "true";
                        tempcrop.bTaken = tempnode.GetAttribute("bTaken") == "true";

                        Crops.Add(tempcrop);
                    }
                }
            }
            else
                Console.WriteLine("File not loaded!");
        }
    }
}
