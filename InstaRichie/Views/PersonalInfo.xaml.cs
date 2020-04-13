using SQLite.Net;
using StartFinance.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PersonalInfo : Page
    {

        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");


        public PersonalInfo()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

            // Creating table
            Results();
        }

        public void Results()
        {
            // Creating table
            conn.CreateTable<Personal>();
            var query = conn.Table<Personal>();
            Personalinfo.ItemsSource = query.ToList();

                   
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }



        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            string CDay = DOB.Date.Value.Day.ToString();
            string CMonth = DOB.Date.Value.Month.ToString();
            string CYear = DOB.Date.Value.Year.ToString();
            string FinalDate = "" + CDay + "/" + CMonth + "/" + CYear;



            try
            {
                // checks if account name is null
                if (FirstName.Text.ToString() == "" || LastName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Please Enter All Fields !!");
                    await dialog.ShowAsync();
                }
                else if (Gender.Text.ToString() == "" )
                {
                    MessageDialog dialog = new MessageDialog("Gender Should be Male or Female !!");
                    await dialog.ShowAsync();
                }
               
                else if (EmailAddress.Text.ToString() == "" || PhoneNumber.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Please ENter All Fields !!");
                    await dialog.ShowAsync();
                }
                else
                {   // Inserts the data
                    conn.Insert(new Personal()
                    {
                        FirstName = FirstName.Text,
                        LastName = LastName.Text,
                        DOB = FinalDate,
                        Gender = Gender.Text,
                        EmailAddress = EmailAddress.Text,
                        MobilePhone = PhoneNumber.Text

                    });
                    Results();
                }
            }
            catch (Exception ex)
            {   // Exception to display when amount is invalid or not numbers
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("Please Instert all the data !");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Already Exist, Try Different Name", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }

            }

           

        }

       

        private async void Delete_Click(object sender, RoutedEventArgs e)
           {
            MessageDialog ShowConf = new MessageDialog("Deleting this Account will delete all transactions of this account", "Important");
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
                // checks if data is null else inserts
                try
                {
                    string Personal1 = ((Personal)Personalinfo.SelectedItem).FirstName;
                    var querydel = conn.Query<Personal>("DELETE FROM Personal WHERE FirstName='" + Personal1 + "'");
                    Results();
                    conn.CreateTable<Personal>();
                    var querytable = conn.Query<Personal>("DELETE FROM Personal WHERE FirstName='" + Personal1 + "'");

                }
                catch (NullReferenceException)
                {
                    MessageDialog ClearDialog = new MessageDialog("Please select the item to Delete", "Oops..!");
                    await ClearDialog.ShowAsync();
                }
            }
            else
            {
                //
            }
        }


        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            string CDay = DOB.Date.Value.Day.ToString();
            string CMonth = DOB.Date.Value.Month.ToString();
            string CYear = DOB.Date.Value.Year.ToString();
            string FinalDate = "" + CDay + "/" + CMonth + "/" + CYear;

            try
            {
                string Personal1 = ((Personal)Personalinfo.SelectedItem).FirstName;
                if (Personal1 == "")
                {
                    MessageDialog dialog = new MessageDialog("Please Select which one should be edited", "Damnnn...!!");
                    await dialog.ShowAsync();
                }
                else
                {

                    string FirsttName = FirstName.Text;
                    string LasttName = LastName.Text;
                    string birthdate = FinalDate;
                    string Genderr = Gender.Text;
                    string EmailAddresss = EmailAddress.Text;
                    string MobilePhone = PhoneNumber.Text;

                    conn.CreateTable<Personal>();
                    var query = conn.Table<Personal>();
                    var queryEdit = conn.Query<Personal>("UPDATE Personal SET FirstName ='" + FirsttName + "', LastName ='" + LasttName + "', DOB ='" + birthdate + "', Gender ='" + Genderr + "', EmailAddress ='" + EmailAddresss + "',  MobilePhone ='" + MobilePhone + "' WHERE FirstName ='" + Personal1 + "'");
                    Results();
                }

            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Please select record to edit ", "Error !!");
                await dialog.ShowAsync();

            }

        }

        private void Personalinfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
  



            try
            {
                string selectFirstName = ((Personal)Personalinfo.SelectedItem).FirstName;
                string selectLastName = ((Personal)Personalinfo.SelectedItem).LastName;

                string selectGender = ((Personal)Personalinfo.SelectedItem).Gender;
                string selectEmailAddress = ((Personal)Personalinfo.SelectedItem).EmailAddress;
                string selectMobilePhone = ((Personal)Personalinfo.SelectedItem).MobilePhone;


                FirstName.Text = selectFirstName;
                LastName.Text = selectFirstName;

                Gender.Text = selectGender;
                EmailAddress.Text = selectEmailAddress;
                PhoneNumber.Text = selectMobilePhone;

            }
            catch (NullReferenceException)
            {
                NoFunction();
            }
        }

        private void NoFunction()
        {
            throw new NotImplementedException();
        }
    }
}
