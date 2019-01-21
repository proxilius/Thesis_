using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;




namespace SimaSzamlaAdatbazissal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public static SzamlaEntities DB=new SzamlaEntities();
        public static DataGrid dgrid;
        public static ObservableCollection<Szamlak> OC = new ObservableCollection<Szamlak>();
        List<Subtotal> SubtotalsList = new List<Subtotal>();
        public event PropertyChangedEventHandler PropertyChanged;
        Microsoft.Office.Interop.Excel.Range chartRange;
        private ViewModel vm;
        
        public class Subtotal
        {
            public int sub { get; set; }
            
        }
        List<Subtotal> subtotals = new List<Subtotal>();
        
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        public void makeSubtotal(List<Szamlak>a)
        {
            SubtotalsList.Clear();
            int subb = 0;
            foreach (var i in a)
            {
                subb += i.Osszeg;
                SubtotalsList.Add(new Subtotal() { sub = subb });
            }
            ReszosszegDatagrid.ItemsSource = SubtotalsList;
        }

        public MainWindow()
        {
            
            
            SubtotalsList.Clear();
            List<Szamlak> a = DB.Szamlak.ToList();
            InitializeComponent();
            foreach (var i in a) 
            {
                OC.Add(i);
            }
            SzamlaDatagrid.ItemsSource = DB.Szamlak.ToList();
            makeSubtotal(a);
            dgrid = SzamlaDatagrid;
            szamol2();
            OnPropertyChanged("szamol2");
            
        }

        
        public  void szamol2()
        {
            int osszeg = DB.Szamlak.Sum(f => f.Osszeg);
            Osszesen.Text = osszeg.ToString();
        }

        private void HozzaAd(object sender, RoutedEventArgs e)
        {
            addItemWindow newItem = new addItemWindow();
            newItem.Show();
            Close();
        }

        private void DeleteRow(object sender, RoutedEventArgs e)
        {
            int id=0;
            try
            {
                id = (SzamlaDatagrid.SelectedItem as Szamlak).Id;
                Szamlak torlendo = new Szamlak();
                torlendo = DB.Szamlak.Where(d => d.Id == id).First();
                DB.Szamlak.Remove(torlendo);
                DB.SaveChanges();
                OC.Remove(torlendo);
                SzamlaDatagrid.ItemsSource = DB.Szamlak.ToList();
                //SzamlaDatagrid.ItemsSource = OC;
                szamol2();
                makeSubtotal(DB.Szamlak.ToList());
            }
            catch
            {
                MessageBox.Show("Hiba, jelölje ki a torolni kivánt sort.");
            }
            

        }

        private void TesztFuggveny(object sender, RoutedEventArgs e)
        {
            //var ctx = sz.Szamlak.SqlQuery("Select * from Szamlak where ").ToList();
            //string listastring="";
            //foreach (var i in ctx)
            //{
            //    listastring += i.Datum + "\n";
            //}
            ////MessageBox.Show(listastring);

            //var osszeg = sz.Szamlak.SqlQuery("Select sum(Osszeg) from Szamlak");
            //int o = 0;
            //foreach (var i in ctx)
            //{
            //    o = i.Osszeg;
            //}
            //MessageBox.Show(o.ToString());
            int osszeg = DB.Szamlak.Sum(f => f.Osszeg);
            Console.WriteLine(osszeg);
        }

        private void tesztSzamol(object sender, RoutedEventArgs e)
        {
            //int osszeg = sz.Szamlak.Sum(f => f.Osszeg);
            //MessageBox.Show(osszeg.ToString());
            addItemWindow newItem = new addItemWindow();
            newItem.ShowDialog();
            
        }

        private void Osszesen_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }

        private void osszFriss(object sender, RoutedEventArgs e)
        {
           szamol2();
        }

        private void exportDataToExcel(object sender, RoutedEventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application excel= new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = excel.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = workbook.ActiveSheet;

            worksheet.Columns["A:E"].ColumnWidth = 17.57;

            worksheet.Cells[1, 1] = "Id";
            worksheet.Cells[1, 2] = "Megnevezés";
            worksheet.Cells[1, 3] = "Összeg";
            worksheet.Cells[1, 4] = "Dátum";
            worksheet.Cells[1, 5] = "Részösszeg";

            int index = 2;
            foreach (var i in DB.Szamlak)
            {
                worksheet.Cells[index, 1] = i.Id;
                worksheet.Cells[index, 2] = i.Megnevezes;
                worksheet.Cells[index, 3] = i.Osszeg;
                worksheet.Cells[index, 4] = i.Datum;
                index++;
            }
            index = 2;
            foreach (var j in SubtotalsList)
            {
                worksheet.Cells[index, 5] = j.sub;
                index++;
            }

            workbook.SaveAs("SimaSzámla");
            excel.Visible = true;

            MessageBox.Show("Az excel fájl sikeresen létrejött!");

        }

        private void GraphWindow(object sender, RoutedEventArgs e)
        {
            GrafikonWindow gw = new GrafikonWindow();
            gw.Show();
        }

        private void PieChartWindow(object sender, RoutedEventArgs e)
        {
            PieChartWindow pie = new PieChartWindow();
            pie.Show();
        }
    }
}
