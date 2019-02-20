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
using System.Windows.Shapes;

namespace SimaSzamlaAdatbazissal
{
    /// <summary>
    /// Interaction logic for FreezePriceWindow.xaml
    /// </summary>
    public partial class FreezePriceWindow : Window
    {
        public string todayDate;
        public string todayTime;
        private SzamlaEntities DB = new SzamlaEntities();
        public FreezePriceWindow()
        {
            InitializeComponent();
            dataGridRate.ItemsSource = DB.RateTable.ToList();
            CenterWindowOnScreen();
            

        }
        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }

        private void putPriceToDatabase(object sender, RoutedEventArgs e)
        {
            string nameofpaper=comboBox.Text.ToString();
            int priceofpaper=Convert.ToInt32(textBoxcompaperPrice.Text);
            MessageBox.Show(nameofpaper + "\n" + priceofpaper + "\n" + todayDate + "\n" + todayTime);
            
            RateTable actual = new RateTable();
            actual.NameOfpaper = nameofpaper;
            actual.Price = priceofpaper;
            actual.DateOf = todayDate;
            actual.TimeOf = todayTime;
            DB.RateTable.Add(actual);
            DB.SaveChanges();
            dataGridRate.ItemsSource = DB.RateTable.ToList();


        }

        private void getNowTime(object sender, RoutedEventArgs e)
        {
            todayDate = DateTime.Today.ToString("yyyy.MM.dd");
            todayTime= DateTime.Now.ToString("HH:mm:ss");
            dateTimeTextbox.Text = todayDate + ", " + todayTime;
        }

        private void showDiagram(object sender, RoutedEventArgs e)
        {
            List<string> nevek = new List<string>();
            List<RateTable> data = new List<RateTable>();
            string s="";
            string ss = "";
            data = DB.RateTable.ToList();
            var result = data.GroupBy(test => test.Id)
                   .Select(grp => grp.First())
                   .ToList();
            foreach (var i in data)
            {
                if (i.NameOfpaper.Contains("OTP"))
                {
                    nevek.Add(i.NameOfpaper);
                    s += i.NameOfpaper + "\n";
                }
                else if (i.NameOfpaper.Contains("MOL"))
                {
                    nevek.Add(i.NameOfpaper);
                    ss += i.NameOfpaper + "\n";
                }

            }

            MessageBox.Show(s+"\n"+ss);
            RateDiagramWindow rd = new RateDiagramWindow();
            rd.Show();
        }

        private void deletePrice(object sender, RoutedEventArgs e)
        {
            int id = 0;
            try
            {
                id = (dataGridRate.SelectedItem as RateTable).Id;
                RateTable torlendo = new RateTable();
                torlendo = DB.RateTable.Where(d => d.Id == id).First();
                DB.RateTable.Remove(torlendo);
                DB.SaveChanges();
                dataGridRate.ItemsSource = DB.RateTable.ToList();
                
            }
            catch
            {
                MessageBox.Show("Hiba, jelölje ki a torolni kivánt sort.");
            }
        }
    }
}
