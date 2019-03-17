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
            int firstAMountAfterChange = 0;//amikor meg nincs ertekpapir 
            int actualPriceOfPaper = 0;
            int firstHuf = 0;
            SzamlaEntities uj = new SzamlaEntities();
            CommercialPapers cp = new CommercialPapers();
            CommercialPaperFix cpfix = new CommercialPaperFix();
            ActualTable actualdbs = new ActualTable();
            try
            {
                foreach (var i in uj.RateTable.ToList())
                {
                    if (i.NameOfpaper.Contains(comboBox.Text.ToString()))
                    {
                        actualPriceOfPaper = i.Price;
                    }
                }
                foreach(var i in uj.ActualTable)
                {
                    if(i.Name.Contains(comboBox.Text.ToString()))
                    firstAMountAfterChange = i.AmountAfterChange;
                }
                foreach (var i in uj.ActualTable.ToList())
                {
                    if (i.Name.Contains("HUF"))
                    {
                        firstHuf = i.huf;
                    }
                }


                cp.cp_name = comboBox.Text.ToString();
                cp.cp_date = tbDate.Text;
                cp.cp_time =TimeSpan.Parse( tbTime.Text);
                cp.cp_value = Convert.ToInt32(tbValue.Text);
                cp.cp_amount= Convert.ToInt32(tbAmount.Text);
                cp.sumcom = cp.cp_amount * cp.cp_value;
                uj.CommercialPapers.Add(cp);
                uj.SaveChanges();

                cpfix.cp_name = comboBox.Text.ToString();
                cpfix.cp_date = tbDate.Text;
                cpfix.cp_time =tbTime.Text;
                cpfix.cp_value = Convert.ToInt32(tbValue.Text);
                cpfix.cp_amount = Convert.ToInt32(tbAmount.Text);
                cpfix.sumcom = cpfix.cp_value * cpfix.cp_amount;
                uj.CommercialPaperFix.Add(cpfix);
                uj.SaveChanges();

                actualdbs.Name= comboBox.Text.ToString();
                actualdbs.DateOf= tbDate.Text;
                actualdbs.TimeOf= tbTime.Text;
                actualdbs.Change= Convert.ToInt32(tbAmount.Text);
                actualdbs.actualRate = actualPriceOfPaper;
                actualdbs.AmountAfterChange = Convert.ToInt32(tbAmount.Text) + firstAMountAfterChange;
                actualdbs.Sum = actualdbs.actualRate * actualdbs.AmountAfterChange;
                actualdbs.huf = firstHuf - cpfix.sumcom;
                uj.ActualTable.Add(actualdbs);
                uj.SaveChanges();


                m.SzamlaDatagrid.ItemsSource = uj.Szamlak.ToList();
                List<CommercialPaperFix> b = uj.CommercialPaperFix.ToList();
                foreach (var i in b)
                {
                    i.sumcom = i.cp_value * i.cp_amount;
                }
                m.dataGridCommercialFix.ItemsSource = b;
                m.CommercialPapersDataGrid.ItemsSource = uj.CommercialPapers.ToList();
                m.dataGridActual.ItemsSource = uj.ActualTable.ToList();


                m.actualDBS();
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
