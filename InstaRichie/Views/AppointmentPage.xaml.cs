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

namespace StartFinance.Views
{
    public sealed partial class AppointmentPage : Page
    {
        SQLiteConnection conn; //add sqlite conn
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public AppointmentPage()
        {
            this.InitializeComponent();

            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            //Init DB
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            //Create Table
            conn.CreateTable<Appointments>();
            EvntDateStamp.Date = DateTime.Now; //Gets time and date

            Results();
        }

        public void Results()
        {
            conn.CreateTable<Appointments>();
            var query = conn.Table<Appointments>();
            AppointmentList.ItemsSource = query.ToList();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            //Collect Date and time data
            string CDay = EvntDateStamp.Date.Value.Day.ToString();
            string CMonth = EvntDateStamp.Date.Value.Month.ToString();
            string CYear = EvntDateStamp.Date.Value.Year.ToString();
            string FinalDate = "" + CMonth + "/" + CDay + "/" + CYear;


            TimeSpan startTimeSpan = StartTimeStamp.Time;
            TimeSpan endTimeSpan = EndTimeStamp.Time;


            string StartTimeStr = startTimeSpan.ToString();
            string EndTimeStr = endTimeSpan.ToString();

            

            //TimeSpan startTimeT = StartTimeStamp.Time;
            //string eeee = startTimeT.ToString();

            //string StartTimeStr = StartTimeStamp.
            //string EndTimeStr = DateTime.Today.Add(EndTimeStamp.Time).ToString(timePicker.Format);
 

            try
            {//Checks if app name is the same as the db name or empty
                if (EvntNameBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Appointment Name not entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (EvntNameBox.Text.ToString() == "EventName" || EvntNameBox.Text.ToString() == "e.g Dentist App")
                {
                    MessageDialog variableError = new MessageDialog("You cannot use this Appointment name", "Oops..!");
                }
                else
                {//inserts data if OK
                    conn.Insert(new Appointments()
                    {
                        EventName = EvntNameBox.Text,
                        Location = LocationBox.Text,
                        EventDate = FinalDate,
                        StartTime = StartTimeStr,
                        EndTime = EndTimeStr
                    });
                    Results();
                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("Invalid data inputted");
                    await dialog.ShowAsync();
                }
                else if(ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Appointment already exists");
                    await dialog.ShowAsync();
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        private async void DelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string AppSelection = ((Appointments)AppointmentList.SelectedItem).EventName;
                if(AppSelection == "")
                {
                    MessageDialog dialog = new MessageDialog("Item hasnt been selected to delete", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<Appointments>();
                    var query1 = conn.Table<Appointments>();
                    var query3 = conn.Query<Appointments>("DELETE FROM Appointments WHERE EventName ='" + AppSelection + "'");
                    AppointmentList.ItemsSource = query1.ToList();
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Null Item", "Oops..!");
                await dialog.ShowAsync();
            }
        }
    }
}
