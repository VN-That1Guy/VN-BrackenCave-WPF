using System;
using System.Collections.Generic;
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

namespace VN_BrackenCave_WPF
{
    /// <summary>
    /// Interaction logic for Inventory.xaml
    /// </summary>
    public partial class ShopMenu : Page
    {
        MainWindow window = (MainWindow)Application.Current.MainWindow;
        Random rand = new Random();

        private List<String> Greetings = new List<String>()
        {
            "\"Hoot! Hoot?\" The Owl hoots.",
            "An Owl? Judging by the apparel, this is the seller.",
            "As you see the Owl, you're unsure if this is the seller. Regardless, you continue shopping.",
            "\"Hoot! Hoot!\" You are confused by the Owl.",
            "\"Hoot?\" You do believe that is the Owl greeting you."
        };
        private List<string> BuyDialogue = new List<string>()
        {
            "\"Hoot! Hoot!\" The Owl Exclaims. You're not sure what that means",
            "The Owl pecks at the money. Not sure what is an owl going to do with currency.",
            "It is not clear how you would give your money to the owl, so you just left it on the counter infront of it",
            "As you pass the money over to the owl, it tilts is head at what you just passed over.",
            "\"Hoot! Hoot!\" The Owl Exclaims."
        };
        private List<string> SellDialogue = new List<string>()
        {
            "\"Hoot? Hoot?\" The Owl hoots, as if confused.",
            "As you hand over the item, the Owl looks at it as if it was analyzing it.",
            "\"Hoot! Hoot!\" The Owl pecks at the item a little bit. You'd think it'd recognize it's own products... if that is the real seller any way.",
            "The Owl tilts it's head at you when you hand over the item, unsure what to do with it."
        };
        public ShopMenu()
        {
            InitializeComponent();
            PlayerWallet.Text = "Wallet: " + window.game.Client.Currency.ToString("c");
        }

        private void UpdatePlayerInventory()
        {
            InventoryTextBlock.Text = $"{window.game.Client.Name}\nItems:\n{window.game.Client.ShowInventory()}";
        }

        private void BuyList_Initialized(object sender, EventArgs e)
        {
            BuyList.ItemsSource = window.game.Seller.Items;
            BuyList.DisplayMemberPath = "Name";
        }

        private void SellList_Initialized(object sender, EventArgs e)
        {
            SellList.ItemsSource = window.game.Client.Inventory;
            SellList.DisplayMemberPath = "Name";
        }

        private void SellButton_Click(object sender, RoutedEventArgs e)
        {
            Item item = (Item)SellList.SelectedItem;
            if (item.Amount > 0)
            {
                window.game.Client.Inventory.Find(x => x.Name.Equals(item.Name)).Amount--;
                window.game.Client.Currency += item.Value;
                PlayerWallet.Text = "Wallet: " + window.game.Client.Currency.ToString("c");
                UpdatePlayerInventory();
                Vendor_Dialogue.Text = SellDialogue[rand.Next(SellDialogue.Count)] + " All of a sudden money falls upon you.";
            }
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            Item item = (Item) BuyList.SelectedItem;
            if (window.game.Client.Currency >= item.Value)
            {
                window.game.Client.Inventory.Find(x => x.Name.Equals(item.Name)).Amount++;
                window.game.Client.Currency -= item.Value;
                PlayerWallet.Text = "Wallet: "+ window.game.Client.Currency.ToString("c");
                UpdatePlayerInventory();
                Vendor_Dialogue.Text = BuyDialogue[rand.Next(BuyDialogue.Count)];
            }
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void SellList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Item item = (Item)SellList.SelectedItem;
            if (item != null)
            {
                ItemDescriptionBox.DataContext = item;
                ItemPreview.DataContext = item;
                SellItemCounter.Text = $"{item.Value.ToString("c")}";
                SellButton.IsEnabled = window.game.Client.Currency >= item.Value;
            }
        }

        private void BuyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Item item = (Item)BuyList.SelectedItem;
            if (item != null)
            {
                ItemDescriptionBox.DataContext = item;
                ItemPreview.DataContext = item;
                BuyItemCounter.Text = $"{item.Value.ToString("c")}";
                BuyButton.IsEnabled = window.game.Client.Currency >= item.Value;
            }
        }

        private void Grid_Initialized(object sender, EventArgs e)
        {
            UpdatePlayerInventory();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Vendor_Dialogue.Text = Greetings[rand.Next(Greetings.Count)];
        }
    }
}
