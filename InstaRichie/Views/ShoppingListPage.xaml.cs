using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using StartFinance.Models;
using SQLite.Net;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShoppingListPage : Page
    {

        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");



        public ShoppingListPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

            // Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            Results();
        }

        public void Results()
        {
            //create table
            conn.CreateTable<ShoppingDetails>();

            //Refresh Data
            var query = conn.Table<ShoppingDetails>();
            ShoppingList.ItemsSource = query.ToList();

        }

        public void clearFields()
        {
            tbShoppingItemID.Text = "";
            tbShopName.Text = "";
            tbItemName.Text = "";
            dpShoppingDate.Date = DateTime.Now;
            tbPriceQuoted.Text = "";
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var shoppingID = tbShoppingItemID.Text;
                var shopName = tbShopName.Text;
                var itemName = tbItemName.Text;
                var quotedPrice = tbPriceQuoted.Text;


                var dateNow = DateTime.Now;
                var selectedDate = dpShoppingDate.Date;

                String cDay = selectedDate.Day.ToString();
                String cMonth = selectedDate.Month.ToString();
                String cYear = selectedDate.Year.ToString();
                String cDate = cDay + "-" + cMonth + "-" + cYear;


                if (shoppingID == "")
                {
                    MessageDialog missingData = new MessageDialog("Some fields are missing. Please fill out correct information", "Something is wrong.");
                    await missingData.ShowAsync();
                    tbShoppingItemID.Focus(FocusState.Programmatic);
                    return;
                }
                else if (shopName == "")
                {
                    MessageDialog missingData = new MessageDialog("Some fields are missing. Please fill out correct information", "Something is wrong.");
                    await missingData.ShowAsync();
                    tbShopName.Focus(FocusState.Programmatic);
                    return;
                }
                else if (itemName == "")
                {
                    MessageDialog missingData = new MessageDialog("Some fields are missing. Please fill out correct information", "Something is wrong.");
                    await missingData.ShowAsync();
                    tbItemName.Focus(FocusState.Programmatic);
                    return;
                }
                else if (quotedPrice == "")
                {
                    MessageDialog missingData = new MessageDialog("Some fields are missing. Please fill out correct information", "Something is wrong.");
                    await missingData.ShowAsync();
                    tbPriceQuoted.Focus(FocusState.Programmatic);
                    return;
                }
                else if (selectedDate > dateNow)
                {
                    MessageDialog missingData = new MessageDialog("You've Selected a date that hasnt happened yet. Please fix", "Something is wrong");
                    await missingData.ShowAsync();
                    dpShoppingDate.Date = dateNow;
                    dpShoppingDate.Focus(FocusState.Programmatic);
                    return;

                }
                else
                {
                    conn.Insert(new ShoppingDetails()
                    {
                        ShoppingItemID = shoppingID,
                        ShopName = shopName,
                        ItemName = itemName,
                        ShoppingDate = cDate,
                        QuotedPrice = Convert.ToDouble(tbPriceQuoted.Text) * 1.00

                    });

                    MessageDialog successDialog = new MessageDialog("Item: " + tbItemName.Text + " has been added");
                    await successDialog.ShowAsync();
                    Results();

                    clearFields();
                }

            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog wrongData = new MessageDialog("Wrong format. Please enter number", "Something is wrong!");
                    await wrongData.ShowAsync();
                    tblPriceQuoted.Focus(FocusState.Programmatic);
                    return;
                }
                else if (ex is SQLiteException)
                {
                    MessageDialog duplicate = new MessageDialog("Duplicate Entry.", "Duplicate");
                    await duplicate.ShowAsync();

                }
            }

        }
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {


            MessageDialog ShowConf = new MessageDialog("Are you sure you want to delete this Item from your shopping list?", "IMPORTANT");
            ShowConf.Commands.Add(new UICommand("Yes, Delete")
            {
                Id = 0
            });
            ShowConf.Commands.Add(new UICommand("Cancel")
            {
                Id = 1
            });
            ShowConf.DefaultCommandIndex = 0;
            ShowConf.CancelCommandIndex = 1;

            var result = await ShowConf.ShowAsync();
            if ((int)result.Id == 0)
            {
                //checks if data is null else inserts
                try
                {
                    string ShoppingItem = ((ShoppingDetails)ShoppingList.SelectedItem).ItemName;
                    var querydel = conn.Query<ShoppingDetails>("DELETE FROM ShoppingDetails WHERE ItemName='" + ShoppingItem + "'");
                    Results();
                }
                catch (NullReferenceException)
                {
                    MessageDialog CleaDialog = new MessageDialog("Please select the item to Delete", "Sorry!");
                    await CleaDialog.ShowAsync();
                }
            }
            else
            {
                //
            }

        }



    }

            
}
