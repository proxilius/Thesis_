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
    /// Interaction logic for addCommercialPaperWindow.xaml
    /// </summary>
    public partial class addCommercialPaperWindow : Window
    {
        MainWindow m;
        public addCommercialPaperWindow()
        {
            InitializeComponent();
            m = new MainWindow();
        }

        private void addCommercialToDatabase(object sender, RoutedEventArgs e)
        {
            SzamlaEntities uj = new SzamlaEntities();
            CommercialPapers cp = new CommercialPapers();
            try
            {
                cp.cp_name = comboBox.Text.ToString();
                cp.cp_date = tbDate.Text;
                cp.cp_time = TimeSpan.Parse(tbTime.Text);
                cp.cp_value = Convert.ToInt32(tbValue.Text);
                cp.cp_amount= Convert.ToInt32(tbAmount.Text);
                uj.CommercialPapers.Add(cp);
                uj.SaveChanges();

                m.SzamlaDatagrid.ItemsSource = uj.Szamlak.ToList();
                List<CommercialPapers> b = uj.CommercialPapers.ToList();
                foreach (var i in b)
                {
                    i.sumcom = i.cp_value * i.cp_amount;
                }
                m.CommercialPapersDataGrid.ItemsSource = b;
                
               

                MessageBox.Show("Sikeres hozzáadás");
                this.Close();
                m.Show();
            }
            catch (Exception)
            {
                MessageBox.Show("Hibásan adtad meg valamely paramétert");
            }
        }

        private void setDateNow(object sender, RoutedEventArgs e)
        {
            tbDate.Text = DateTime.Today.ToString("yyyy.MM.dd");
        }

        private void setTimeNow(object sender, RoutedEventArgs e)
        {
             tbTime.Text=DateTime.Now.ToString("HH:mm:ss");
        }
    }
}
