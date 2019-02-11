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
    }
}
