using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace VN_BrackenCave_WPF
{
    /// <summary>
    /// Interaction logic for GameMenu.xaml
    /// </summary>
    public partial class GameMenu : Page
    {
        MainWindow window = (MainWindow)Application.Current.MainWindow;
        Uri ShopMenu = new Uri("ShopMenu.xaml",UriKind.Relative);
        Bats ?GameBat;
        Hawks? GameHawk;
        DispatcherTimer timer;
        TimeSpan timeSpan;
        int TimeLimit = 45;
        bool Fertilized = false;

        public GameMenu()
        {
            GameBat = window.game.GetBat();
            GameHawk = window.game.GetHawk();
            window.Title = $"Project Bracken - {window.game.Client.Name}";
            InitializeComponent();
        }
        public void SetTimer()
        {
            //DispatchTimer example by kmatyaszek (https://stackoverflow.com/users/1410998/kmatyaszek)
            timeSpan = TimeSpan.FromSeconds(TimeLimit);

            if (timer != null)
                timer.Stop();
            if (timer == null)
                timer = new DispatcherTimer(
                    new TimeSpan(0, 0, 1),
                    DispatcherPriority.Normal,
                    delegate
                    {
                        Timer.Text = timeSpan.ToString("c");
                        if (timeSpan <= TimeSpan.Zero)
                        {
                            timer.Stop();
                            GotoNextDay();
                        }
                        timeSpan = timeSpan.Add(TimeSpan.FromSeconds(-1));
                    },
                    Application.Current.Dispatcher);

            timer.Start();
        }

        private void Crops_GetSeeds_Click(object sender, RoutedEventArgs e)
        {
            foreach (Crop crop in window.game.GetCrops())
            {
                window.game.GetItemFrom(crop);
                LogBox.Text += $"Day {window.game.GetDay()}: Got {crop.ItemToGive.Name}\n";
            }
            Crops_GetSeeds.IsEnabled = false;
            ActionPortrait.Source = new BitmapImage(new Uri("media/GetSeeds.bmp",UriKind.Relative));
            UpdatePlayerStats();
        }

        private void GotoShop_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(ShopMenu);
        }

        private void Bats_GetGuano_Click(object sender, RoutedEventArgs e)
        {
            Bats bat = (Bats)window.game.GetAnimals().Find(x => x.Name.Equals("Bat"));
            window.game.GetItemFrom(bat);
            Bats_GetGuano.IsEnabled = false;
            ActionPortrait.Source = new BitmapImage(new Uri("media/GetGuano.bmp", UriKind.Relative));
            LogBox.Text += $"Day {window.game.GetDay()}: Got {bat.ItemToGive.Name}\n";
            UpdateCropStats();
            UpdatePlayerStats();
        }

        private void UpdatePlayerStats()
        {
            Player_Stats.Text = $"Wallet: {window.game.Client.Currency.ToString("c")}\nItems:\n{window.game.Client.ShowInventory()}";
            /*if (window.game.Client.Inventory.Find(x => x.Name.Equals("Guano")).Amount >= 2 && window.game.GetTotalCropDensity() < window.game.GetCrops().Sum(x => x.MaxDensity()) && !Crops_PlantSeed.IsEnabled)
            {
                Crops_FertilizeButton.IsEnabled = true;
            }
            else if (window.game.Client.Inventory.Find(x => x.Name.Equals("Guano")).Amount < 2 && Crops_PlantSeed.IsEnabled)
            {
                Crops_FertilizeButton.IsEnabled = false;
            }*/
            /*if (window.game.GetAnimals().Find(x => x.Name.Equals("Hawk")).Populationlevel > 0 && window.game.Client.Inventory.Find(x => x.Name.Contains("Anti-Hawk Spray")).Amount > 0)
            {
                Hawk_ActionButton.IsEnabled = true;
            }*/
        }
        private void UpdateBatStats()
        {
            Bat_Statistics.Text = $"Bat Population level: {window.game.GetAnimals().Find(x => x.Name.Equals("Bat")).Populationlevel}";
            if (GameBat.Populationlevel < GameBat.GetMaxPopLevel() && Bat_Alert.Visibility != Visibility.Visible)
            {
                Bat_Alert.Visibility = Visibility.Visible;
            }
            else if (GameBat.Populationlevel >=  GameBat.GetMaxPopLevel() && Bat_Alert.Visibility != Visibility.Hidden)
            {
                Bat_Alert.Visibility = Visibility.Hidden;
            }

            if (GameBat.Populationlevel <= 0)
            {
                BatsPortrait.Source = new BitmapImage(new Uri("media/Bats-NoPopulation.bmp", UriKind.Relative));
            }
            else
            {
                BatsPortrait.Source = new BitmapImage(new Uri("media/Bats.bmp", UriKind.Relative));
            }
        }
        private void UpdateHawkStats()
        {
            Hawk_Statistics.Text = $"Hawk Population level: {window.game.GetAnimals().Find(x => x.Name.Equals("Hawk")).Populationlevel}";
            // Update Hawk Portrait if there are one hawks or more
            if ( window.game.GetAnimals().Find(x => x.Name.Equals("Hawk")).Populationlevel > 0 )
            {
                if ( window.game.Client.Inventory.Find(x => x.Name.Equals("Anti-Hawk Spray")).Amount > 0 )
                    Hawk_ActionButton.IsEnabled = true;
                HawksPortrait.Source = new BitmapImage(new Uri("media/Hawks.bmp", UriKind.Relative));
                Hawk_Alert.Visibility = Visibility.Visible;
            }
            else if ( GameHawk.Populationlevel <= 0 )
                Hawk_Alert.Visibility = Visibility.Hidden;
        }

        private void UpdateCropStats()
        {
            Crop_Statistic.Text = $"Crop Density level: {window.game.GetTotalCropDensity()}";
            if (window.game.Client.Inventory.Find(x => x.Name.Equals("Guano")).Amount >= 2 && window.game.GetTotalCropDensity() < window.game.GetCrops().Sum(x => x.MaxDensity()))
                Crops_FertilizeButton.IsEnabled = true;
            foreach (Crop crop in window.game.GetCrops())
            {
                if (crop.bPlanted)
                {
                    Crops_PlantSeed.IsEnabled = false;
                    if (crop.bFertilized)
                    {
                        Crops_FertilizeButton.IsEnabled = false;
                    }
                }
            }
        }
        private void GotoNextDay()
        {
            window.game.NextDay();
            DayAndName.Content = $"Day {window.game.GetDay()}\n{window.game.Client.Name}";
            LogBox.Text += $"--Day {window.game.GetDay()}--\n";
            ActionPortrait.Source = new BitmapImage(new Uri("media/blank.bmp", UriKind.Relative));
            UpdatePlayerStats();
            UpdateBatStats();
            UpdateHawkStats();
            foreach (Crop crop in window.game.GetCrops())
            {
                crop.bFertilized = false;
                crop.bPlanted = false;
                if (crop.bGrown && crop.Densitylevel <= 0)
                    crop.bGrown = false;
            }
            UpdateCropStats();

            

            foreach (var anim in window.game.GetAnimals())
            {
                if (anim.Status != "")
                    LogBox.Text += $"Day {window.game.GetDay()}: {anim.Status}\n";
            }
            if (window.game.GetTotalCropDensity() < 0)
            {
                LogBox.Text += $"Day {window.game.GetDay()}: Alert! There are no more crops! That means no more food for the Bats... Try growing them back.\nNote: It will take a day for it to grow back\n";
            }
            if (window.game.RandomEventMsg != "")
            {
                LogBox.Text += $"Day {window.game.GetDay()}: {window.game.RandomEventMsg}\n";
            }

            

            
            // Update Crops portrait if there are no crops left
            if (window.game.GetTotalCropDensity() == 0)
            {
                CropsPortrait.Source = new BitmapImage(new Uri("media/Decomposers.bmp", UriKind.Relative));
                Crops_PlantSeed.IsEnabled = true;
                Crops_GetSeeds.IsEnabled = false;
            }
            else if (window.game.GetTotalCropDensity() > 0)
            {
                CropsPortrait.Source = new BitmapImage(new Uri("media/Crops.bmp", UriKind.Relative));
                Crops_GetSeeds.IsEnabled = true;
                Crops_PlantSeed.IsEnabled = false;
            }

            /*if (!Crops_FertilizeButton.IsEnabled && !Crops_GetSeeds.IsEnabled && (window.game.Client.Inventory.Find(x => x.Name.Equals("Guano")).Amount >= 2 && window.game.GetTotalCropDensity() < window.game.GetCrops().Sum(x => x.MaxDensity())))
                Crops_FertilizeButton.IsEnabled = true;*/

            if (!Bats_GetGuano.IsEnabled && window.game.GetAnimals().Find(x => x.Name.Equals("Bat")).Populationlevel > 0)
                Bats_GetGuano.IsEnabled = true;
            

            SetTimer();
        }
        private void Grid_Initialized(object sender, EventArgs e)
        {
            GotoNextDay();
        }

        private void Hawk_ActionButton_Click(object sender, RoutedEventArgs e)
        {
            Hawks hawk = (Hawks)window.game.GetAnimals().Find(x => x.Name.Equals("Hawk"));
            HawkRepellent repellent = (HawkRepellent)window.game.Client.Inventory.Find(x => x.Name.Equals("Anti-Hawk Spray"));

            if (hawk != null && repellent != null)
            {
                if (hawk.Populationlevel > 0 && repellent.Amount > 0)
                {
                    hawk.Populationlevel--;
                    repellent.Amount--;
                    UpdatePlayerStats();
                    UpdateHawkStats();
                    LogBox.Text += $"Day {window.game.GetDay()}: Used Anti-Hawk Spray!\n";
                    ActionPortrait.Source = new BitmapImage(new Uri("media/RepellHawks.bmp", UriKind.Relative));
                }
                if (hawk.Populationlevel <= 0)
                    HawksPortrait.Source = new BitmapImage(new Uri("media/Hawks_none.bmp",UriKind.Relative));
                if (repellent.Amount <= 0 || hawk.Populationlevel <= 0)
                    Hawk_ActionButton.IsEnabled = false;
            }
        }

        private void Crops_PlantSeed_Click(object sender, RoutedEventArgs e)
        {
            foreach (Item seed in window.game.Client.Inventory)
            {
                if (seed.Type == ItemType.Seed && seed.Amount > 0)
                {
                    foreach (Crop crop in window.game.GetCrops())
                    {
                        if (!crop.bPlanted && seed.Name.Contains(crop.Name) && seed.Amount > 0)
                        {
                            crop.bPlanted = true;
                            seed.Amount--;
                            LogBox.Text += $"Day {window.game.GetDay()}: Used {seed.Name}\n";
                        }
                    }
                    UpdateCropStats();
                }
            }
            UpdatePlayerStats();
        }

        private void Crops_FertilizeButton_Click(object sender, RoutedEventArgs e)
        {
            Guano guano = (Guano)window.game.Client.Inventory.Find(x => x.Name.Contains("Guano"));

            if (guano != null) {
                if (guano.Amount >= 2)
                {
                    foreach (Crop crop in window.game.GetCrops())
                    {
                        if (crop.bPlanted && !crop.bFertilized)
                        {
                            crop.bFertilized = true;
                            guano.Amount--;
                        }
                        else if (window.game.GetTotalCropDensity() > 0)
                        {
                            crop.Densitylevel++;
                            guano.Amount--;
                        }
                        window.game.UpdateTotalCropDensity();
                    }
                    LogBox.Text += $"Day {window.game.GetDay()}: Used 2 Guanos\n";
                    UpdateCropStats();
                }
                if (guano.Amount < 2 || window.game.GetTotalCropDensity() == window.game.GetCrops().Sum(x=>x.MaxDensity()))
                {
                    Crops_FertilizeButton.IsEnabled = false;
                }
                UpdatePlayerStats();
            }
        }

        private void NextDay_Button_Click(object sender, RoutedEventArgs e)
        {
            GotoNextDay();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) // The player will be navigating from two pages, the game page and the trader page, it is likely that the player will be back with necessary items to interact (e.g, getting seeds, hawk repellant, or guano to maintain the ecosystem) if they do not have them or enough of them. Update the stats to see if the player has bought enough from the trader.
        {
            UpdatePlayerStats();
            UpdateHawkStats();
            UpdateCropStats();
        }

        private void LogBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScrollBox.PageDown();
        }
    }
}
