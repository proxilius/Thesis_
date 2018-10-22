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
    /// Interaction logic for addItemWindow.xaml
    /// </summary>
    public partial class addItemWindow : Window
    {
        MainWindow m;
        public addItemWindow()
        {
            InitializeComponent();
            m = new MainWindow(); 

        }

        private void addDataToDatabase(object sender, RoutedEventArgs e)
        {
            SzamlaEntities uj = new SzamlaEntities();
            Szamlak temp = new Szamlak();
            temp.Megnevezes = textBoxMegnevezes.Text;
            temp.Osszeg = Convert.ToInt32(textBoxOsszeg.Text);
            temp.Datum = textBoxDatum.Text;
            uj.Szamlak.Add(temp);
            uj.SaveChanges();
            //MainWindow.OC.Add(temp);
            //MainWindow.dgrid.ItemsSource=uj.Szamlak.ToList();

            m.SzamlaDatagrid.ItemsSource = uj.Szamlak.ToList();
            m.szamol2();
            
            MessageBox.Show("Sikeres hozzáadás");
            this.Close();
            m.Show();
        }

        private void refresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }
    }
}
