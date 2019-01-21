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
    /// Interaction logic for GrafikonWindow.xaml
    /// </summary>
    public partial class GrafikonWindow : Window
    {
        public static SzamlaEntities DB = new SzamlaEntities();
        public GrafikonWindow()
        {
            InitializeComponent();
            
            List<KeyValuePair<string, int>> MyValue = new List<KeyValuePair<string, int>>();
            foreach (var i in DB.Szamlak.ToList())
            {
                MyValue.Add(new KeyValuePair<string, int>(i.Datum, i.Osszeg));
            }
            //MyValue.Add(new KeyValuePair<string, int>("Administration", 20));
            //MyValue.Add(new KeyValuePair<string, int>("Management", 36));
            //MyValue.Add(new KeyValuePair<string, int>("Development", 89));
            //MyValue.Add(new KeyValuePair<string, int>("Support", 270));
            //MyValue.Add(new KeyValuePair<string, int>("Sales", 140));

            ColumnChart1.DataContext = MyValue;
        }
    }
}
