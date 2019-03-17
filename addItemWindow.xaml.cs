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
            try
            {
                temp.Megnevezes = textBoxMegnevezes.Text;
                temp.Osszeg = Convert.ToInt32(textBoxOsszeg.Text);
                temp.Datum =datepicker.Text;
                temp.Idopont = textBlock.Text;

                uj.Szamlak.Add(temp);
                uj.SaveChanges();

                m.SzamlaDatagrid.ItemsSource = uj.Szamlak.ToList();
                m.dataGridCommercialFix.ItemsSource = uj.CommercialPaperFix.ToList();
                m.makeSubtotal(uj.Szamlak.ToList());
                m.sumOfCommercialPaper();

                m.szamol2();

                MessageBox.Show("Sikeres hozzáadás");
                this.Close();
                m.Show();
            }
            catch (Exception)
            {
                MessageBox.Show("Hibásan adtad meg valamely paramétert");
            }
        }

        private void refresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }

        private void getNowTime(object sender, RoutedEventArgs e)
        {
            textBlock.Text = DateTime.Now.ToString("HH: mm:ss");
        }
    }
}
