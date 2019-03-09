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

        public void makediagram()
        {
            lineChartCopy.Visibility = Visibility.Hidden;
            lineChart.Visibility = Visibility.Visible;
            List<KeyValuePair<string, int>> otplist = new List<KeyValuePair<string, int>>();
            List<KeyValuePair<string, int>> mollist = new List<KeyValuePair<string, int>>();
            List<KeyValuePair<string, int>> erstelist = new List<KeyValuePair<string, int>>();
            List<KeyValuePair<string, int>> datalist = new List<KeyValuePair<string, int>>();
            List<RateTable> data = new List<RateTable>();
            data = DB.RateTable.ToList();
            var dataSourceList = new List<List<KeyValuePair<string, int>>>();
            
            string s = "";
            foreach (var i in data)
            {
                s = i.NameOfpaper;
                if (s.Contains("OTP"))
                {
                    otplist.Add(new KeyValuePair<string, int>(i.DateOf + "\n" + i.TimeOf, i.Price));
                }
                else if (s.Contains("MOL"))
                {
                    mollist.Add(new KeyValuePair<string, int>(i.DateOf + "\n" + i.TimeOf, i.Price));
                }
                else if (s.Contains("ERSTE"))
                {
                    erstelist.Add(new KeyValuePair<string, int>(i.DateOf + "\n" + i.TimeOf, i.Price));
                }
            }
            
            dataSourceList.Add(otplist);
            dataSourceList.Add(mollist);
            dataSourceList.Add(erstelist);
            lineChart.DataContext = dataSourceList;
        }

        private void makediagramSeperate()
        {
            lineChartCopy.Visibility = Visibility.Visible;
            lineChart.Visibility = Visibility.Hidden;
            List<KeyValuePair<string, int>> datalist = new List<KeyValuePair<string, int>>();
            List<RateTable> data = new List<RateTable>();
            data = DB.RateTable.ToList();
            var dataSourceList2 = new List<List<KeyValuePair<string, int>>>();
            string nameofPaper = comboBox1.Text.ToString();
            foreach (var i in data)
            {
                if (i.NameOfpaper.Contains(nameofPaper))
                {
                    datalist.Add(new KeyValuePair<string, int>(i.DateOf + "\n" + i.TimeOf, i.Price));
                }
            }
            dataSourceList2.Add(datalist);
            lineChartCopy.DataContext = dataSourceList2;
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

        public MainWindow()
        {
           
            
            SubtotalsList.Clear();
            List<Szamlak> a = DB.Szamlak.ToList();
            
            InitializeComponent();

            makediagramSeperate();
            makediagram();
            //actualDBS();
            grafikonReszvenyDarabszam();



            CenterWindowOnScreen();

            foreach (var i in a) 
            {
                OC.Add(i);
            }
            SzamlaDatagrid.ItemsSource = DB.Szamlak.ToList();
            List<CommercialPaperFix> b = DB.CommercialPaperFix.ToList();
            foreach (var i in b)
            {
                i.sumcom = i.cp_value * i.cp_amount;
            }
            dataGridCommercialFix.ItemsSource = b;//DB.CommercialPapers.ToList();
            dataGridCommercialsells.ItemsSource = DB.CommercialPaperSells.ToList();
            dataGridRate.ItemsSource = DB.RateTable.ToList();
            CommercialPapersDataGrid.ItemsSource = DB.CommercialPapers.ToList();
            dataGridActual.ItemsSource=DB.ActualTable.ToList();
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

        //private void GraphWindow(object sender, RoutedEventArgs e)
        //{
        //    GrafikonWindow gw = new GrafikonWindow();
        //    gw.Show();
        //}

        //private void PieChartWindow(object sender, RoutedEventArgs e)
        //{
        //    PieChartWindow pie = new PieChartWindow();
        //    pie.Show();
        //}

        private void addCommercialPaper(object sender, RoutedEventArgs e)
        {
            addCommercialPaperWindow commercialwindow = new addCommercialPaperWindow();
            commercialwindow.Show();
            Close();
        }

        private void sellCommercialPaper(object sender, RoutedEventArgs e)
        {
            int id = 0;
            int actualPriceOfPaper=0;//Az adott ertekpapir arfolyama, utolso ertek
            int winning = 0;
            
            try
            {
                id = (CommercialPapersDataGrid.SelectedItem as CommercialPapers).cp_id;
                CommercialPapers selling = new CommercialPapers();
                CommercialPaperSells selled = new CommercialPaperSells();
                selling = DB.CommercialPapers.Where(d => d.cp_id == id).First();
                int cpamount = selling.cp_amount;
                if (cpamount - Convert.ToInt32(amountBox.Text) >= 0)
                {
                    foreach (var i in DB.RateTable.ToList())
                    {
                        if (i.NameOfpaper.Contains(selling.cp_name))
                        {
                            actualPriceOfPaper = i.Price;
                        }
                    }
                    selling.cp_amount = selling.cp_amount - Convert.ToInt32(amountBox.Text);
                    winning = (actualPriceOfPaper * Convert.ToInt32(amountBox.Text)) - (Convert.ToInt32(amountBox.Text) * selling.cp_value);
                    selled.cpAmount = Convert.ToInt32(amountBox.Text);
                    selled.cpDate = DateTime.Today.ToString("yyyy.MM.dd");
                    selled.cpTime =DateTime.Now.ToString("HH:mm:ss");
                    selled.cpName = selling.cp_name;
                    selled.cpValue = actualPriceOfPaper;
                    selled.cpSumcom = actualPriceOfPaper * Convert.ToInt32(amountBox.Text);
                    selled.cpWinning = winning;
                    MessageBox.Show(selled.cpName+", "+selled.cpAmount+","+selled.cpValue+", "+selled.cpDate);
                    DB.CommercialPaperSells.Add(selled);
                    DB.SaveChanges();
                    dataGridCommercialsells.ItemsSource = DB.CommercialPaperSells.ToList();

                    
                    if (selling.cp_amount == 0)
                    {
                        
                        DB.CommercialPapers.Remove(selling);
                       
                        DB.SaveChanges();
                        List<CommercialPapers> b = DB.CommercialPapers.ToList();
                        foreach (var i in b)
                        {
                            i.sumcom = i.cp_value * i.cp_amount;
                        }
                        CommercialPapersDataGrid.ItemsSource = b;
                       

                    }
                    if (selling.cp_amount > 0)
                    {
                        DB.SaveChanges();
                        List<CommercialPapers> b = DB.CommercialPapers.ToList();
                        foreach (var i in b)
                        {
                            i.sumcom = i.cp_value * i.cp_amount;
                        }
                        CommercialPapersDataGrid.ItemsSource = b;
                        

                    }
                    ActualTable actualdbs = new ActualTable();
                    int firstvalueforactual = 0;//amikor meg nincs ertekpapir 
                    foreach (var i in DB.ActualTable)
                    {
                        if (i.Name.Contains(selling.cp_name))
                            firstvalueforactual = i.AmountAfterChange;
                    }
                    actualdbs.Name = selling.cp_name;
                    actualdbs.DateOf = DateTime.Today.ToString("yyyy.MM.dd");
                    actualdbs.TimeOf = DateTime.Now.ToString("HH:mm:ss");
                    actualdbs.Change = 0-Convert.ToInt32(amountBox.Text);
                    actualdbs.actualRate = actualPriceOfPaper;
                    actualdbs.AmountAfterChange = firstvalueforactual- Convert.ToInt32(amountBox.Text);
                    actualdbs.Sum = actualdbs.actualRate * actualdbs.AmountAfterChange;
                    DB.ActualTable.Add(actualdbs);
                    DB.SaveChanges();
                    dataGridActual.ItemsSource = DB.ActualTable.ToList();
                    actualDBS();
                    MessageBox.Show("A nyereseg: "+winning+"\n"+"Az adózás után: "+winning*0.85+"\nAz értékpapír árfolyama: "+actualPriceOfPaper);
                    
                }   
                else
                {
                    MessageBox.Show("Nincs annyi darab, mint amennyit el szeretne adni.");
                }
                

            }
            catch(Exception ed)
            {
                MessageBox.Show("Hiba, jelölje ki az eladni kivánt értékpapírt.");
                MessageBox.Show(ed.ToString());
            }
        }

        private void sellFifo(object sender, RoutedEventArgs e)
        {
            int actualPriceOfPaper = 0;
            int winningFifo = 0;
            string name = fifoname.Text.ToString();
            int amount = Convert.ToInt32(fifoamount.Text);//ennyit akarunk eladni
            int actualPaperAmount = 0;
            List<CommercialPapers> cplist = new List<CommercialPapers>();
            List<CommercialPapers> sortedcplist = new List<CommercialPapers>();
            cplist =DB.CommercialPapers.Where(d => d.cp_name == name).ToList();
            string nevek="";
            sortedcplist =cplist.OrderBy(d => d.cp_date).ThenBy(d =>d.cp_time).ToList();
            foreach (var i in sortedcplist)
            {
                nevek += i.cp_name+","+i.cp_date+"\n";
                if (i.cp_name.Contains(name)) actualPaperAmount+=i.cp_amount;//ennyi van
            }
            if (actualPaperAmount >= amount)
            {
                foreach (var i in DB.RateTable.ToList())
                {
                    if (i.NameOfpaper.Contains(name))
                    {
                        actualPriceOfPaper = i.Price;//set actual price
                    }
                }
                MessageBox.Show("Név\n" + nevek + "\n" + actualPaperAmount);

                ActualTable actualdbs = new ActualTable();
                int firstvalueforactual = 0;//amikor meg nincs ertekpapir 
                foreach (var l in DB.ActualTable)
                {
                    if (l.Name.Contains(name))
                        firstvalueforactual = l.AmountAfterChange;
                }
                actualdbs.Name = name;
                actualdbs.DateOf = DateTime.Today.ToString("yyyy.MM.dd");
                actualdbs.TimeOf = DateTime.Now.ToString("HH:mm:ss");
                actualdbs.Change = 0 - amount;
                actualdbs.actualRate = actualPriceOfPaper;
                actualdbs.AmountAfterChange = firstvalueforactual - amount;
                actualdbs.Sum = actualdbs.actualRate * actualdbs.AmountAfterChange;
                DB.ActualTable.Add(actualdbs);
                DB.SaveChanges();
                dataGridActual.ItemsSource = DB.ActualTable.ToList();

                foreach (var i in sortedcplist)
                {
                    CommercialPaperSells selled = new CommercialPaperSells();
                    int actualdb = i.cp_amount - amount;//10-15
                    if (actualdb <= 0)
                    {
                        winningFifo = (actualPriceOfPaper * i.cp_amount) - (i.cp_amount * i.cp_value);

                        selled.cpName = i.cp_name;
                        selled.cpAmount = i.cp_amount;
                        selled.cpDate = DateTime.Today.ToString("yyyy.MM.dd");
                        selled.cpTime = DateTime.Now.ToString("HH:mm:ss");
                        selled.cpValue = actualPriceOfPaper;
                        selled.cpSumcom = actualPriceOfPaper * i.cp_amount;
                        selled.cpWinning = winningFifo;
                        DB.CommercialPaperSells.Add(selled);


                        DB.CommercialPapers.Remove(i);
                        DB.SaveChanges();
                        amount = amount - i.cp_amount;
                        dataGridCommercialsells.ItemsSource = DB.CommercialPaperSells.ToList();
                    }
                    else
                    {
                        winningFifo += (actualPriceOfPaper * amount) - (amount * i.cp_value);

                        selled.cpName = i.cp_name;
                        selled.cpAmount = amount;
                        selled.cpDate = DateTime.Today.ToString("yyyy.MM.dd");
                        selled.cpTime = DateTime.Now.ToString("HH:mm:ss");
                        selled.cpValue = actualPriceOfPaper;
                        selled.cpSumcom = actualPriceOfPaper * amount;
                        selled.cpWinning = winningFifo;
                        DB.CommercialPaperSells.Add(selled);

                        i.cp_amount = i.cp_amount - amount;
                        DB.SaveChanges();
                        List<CommercialPapers> b = DB.CommercialPapers.ToList();
                        foreach (var k in b)
                        {
                            k.sumcom = k.cp_value * k.cp_amount;
                        }
                        CommercialPapersDataGrid.ItemsSource = b;
                        dataGridCommercialsells.ItemsSource = DB.CommercialPaperSells.ToList();
                        break;
                    }
                   
                }


                actualDBS();
                MessageBox.Show("Árfolyam: "+actualPriceOfPaper+"\nNyereseg: "+winningFifo+"\n neve: "+name);
            }
            else
            {
                MessageBox.Show("Nincs elég értékpapír!");
            }
            

           
             

        }

        public void actualDBS()
        {
            List<CommercialPapers> cplist = new List<CommercialPapers>();
            List<string> cpNames = new List<string>();
            List<string> cpNamesDistinct = new List<string>();
            //string mai = "";
            //mai = datum.Text;


            Dictionary<string, int> my = new Dictionary<string, int>();

            string names = "";
            cplist = DB.CommercialPapers.ToList();
            foreach (var i in cplist)
            {
                cpNames.Add(i.cp_name);
            }

            cpNamesDistinct = cpNames.Distinct().ToList();
            foreach (var i in cpNamesDistinct)
            {
                my.Add(i, 0);
            }

            foreach (var i in cplist)
            {
                //if (DateTime.Parse(i.cp_date) <= DateTime.Parse(mai))
                    my[i.cp_name] += i.cp_amount;
            }

            foreach (var i in my)
            {
                names += i.Key + ": " + i.Value + "\n";
            }

           
            int actualPriceOfPaper = 0;
            foreach (var i in my)
            {
                foreach (var k in DB.RateTable.ToList())
                {
                    if (k.NameOfpaper.Contains(i.Key))
                    {
                        actualPriceOfPaper = k.Price;//set actual price
                    }
                }
                ActualDBTable actualdb = new ActualDBTable();
                actualdb.cpName = i.Key;
                actualdb.cpDB = i.Value;
                actualdb.cpFulValue = actualPriceOfPaper * i.Value;
                actualdb.cpDate =DateTime.Now.ToString();
                DB.ActualDBTable.Add(actualdb);
                DB.SaveChanges();

            }

            dataGridActualAmounts.ItemsSource = DB.ActualDBTable.ToList();

        }

        private void getActualAmount(object sender, RoutedEventArgs e)
        { 
            List<CommercialPapers> cplist = new List<CommercialPapers>();
            List<string> cpNames = new List<string>();
            List<string> cpNamesDistinct = new List<string>();
            //string mai = "";
            //mai = datum.Text;
            

            Dictionary<string, int> my = new Dictionary<string, int>();

            string names="";
            cplist = DB.CommercialPapers.ToList();
            foreach (var i in cplist)
            {
                cpNames.Add(i.cp_name);
            }

            cpNamesDistinct = cpNames.Distinct().ToList();
            foreach (var i in cpNamesDistinct)
            {
                my.Add(i,0);
            }
            
            foreach(var i in cplist)
            {
                //if(DateTime.Parse(i.cp_date) <= DateTime.Parse(mai))
                my[i.cp_name] += i.cp_amount;
            }
            
            foreach (var i in my)
            {
                names += i.Key + ": " + i.Value + "\n";
            }
            
            //MessageBox.Show(names+"\n"+mai);
            var all = from c in DB.ActualDBTable select c;
            DB.ActualDBTable.RemoveRange(all);
            int actualPriceOfPaper = 0;
            foreach (var i in my)
            {
                foreach (var k in DB.RateTable.ToList())
                {
                    if (k.NameOfpaper.Contains(i.Key))
                    {
                        actualPriceOfPaper = k.Price;//set actual price
                    }
                }
                ActualDBTable actualdb = new ActualDBTable();
                actualdb.cpName = i.Key;
                actualdb.cpDB = i.Value;
                actualdb.cpFulValue = actualPriceOfPaper*i.Value;
                actualdb.cpDate =DateTime.Today.ToString() + ","+DateTime.Now.ToString();
                DB.ActualDBTable.Add(actualdb);
                DB.SaveChanges();
                
            }

            dataGridActualAmounts.ItemsSource = DB.ActualDBTable.ToList();

        }

        private void addPriceToDatabase(object sender, RoutedEventArgs e)
        {
            string nameofpaper = comboBox.Text.ToString();
            int priceofpaper = Convert.ToInt32(priceOfTable.Text);
            RateTable actual = new RateTable();
            actual.NameOfpaper = nameofpaper;
            actual.Price = priceofpaper;
            actual.DateOf =getDate.Text;
            actual.TimeOf = timeOf.Text;
            DB.RateTable.Add(actual);
            DB.SaveChanges();
            dataGridRate.ItemsSource = DB.RateTable.ToList();
            makediagram();
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

        private void sellLifo(object sender, RoutedEventArgs e)
        {
            int actualPriceOfPaper = 0;
            int winningFifo = 0;
            string name = fifoname.Text.ToString();
            int amount = Convert.ToInt32(fifoamount.Text);//ennyit akarunk eladni
            int actualPaperAmount = 0;
            List<CommercialPapers> cplist = new List<CommercialPapers>();
            List<CommercialPapers> sortedcplist = new List<CommercialPapers>();
            cplist = DB.CommercialPapers.Where(d => d.cp_name == name).ToList();
            string nevek = "";
            sortedcplist = cplist.OrderBy(d => d.cp_date).ThenBy(d => d.cp_time.ToString()).ToList();
            sortedcplist.Reverse();
            foreach (var i in sortedcplist)
            {
                nevek += i.cp_name + "," + i.cp_date + "\n";
                if (i.cp_name.Contains(name)) actualPaperAmount += i.cp_amount;//ennyi van
            }
            if (actualPaperAmount >= amount)
            {
                foreach (var i in DB.RateTable.ToList())
                {
                    if (i.NameOfpaper.Contains(name))
                    {
                        actualPriceOfPaper = i.Price;//set actual price
                    }
                }
                MessageBox.Show("Név\n" + nevek + "\n" + actualPaperAmount);

                ActualTable actualdbs = new ActualTable();
                int firstvalueforactual = 0;//amikor meg nincs ertekpapir 
                foreach (var l in DB.ActualTable)
                {
                    if (l.Name.Contains(name))
                        firstvalueforactual = l.AmountAfterChange;
                }
                actualdbs.Name = name;
                actualdbs.DateOf = DateTime.Today.ToString("yyyy.MM.dd");
                actualdbs.TimeOf = DateTime.Now.ToString("HH:mm:ss");
                actualdbs.Change = 0 - amount;
                actualdbs.actualRate = actualPriceOfPaper;
                actualdbs.AmountAfterChange = firstvalueforactual - amount;
                actualdbs.Sum = actualdbs.actualRate * actualdbs.AmountAfterChange;
                DB.ActualTable.Add(actualdbs);
                DB.SaveChanges();
                dataGridActual.ItemsSource = DB.ActualTable.ToList();

                foreach (var i in sortedcplist)
                {
                    CommercialPaperSells selled = new CommercialPaperSells();
                    int actualdb = i.cp_amount - amount;//10-15
                    if (actualdb <= 0)
                    {
                        winningFifo = (actualPriceOfPaper * i.cp_amount) - (i.cp_amount * i.cp_value);

                        selled.cpName = i.cp_name;
                        selled.cpAmount = i.cp_amount;
                        selled.cpDate = DateTime.Today.ToString("yyyy.MM.dd");
                        selled.cpTime = DateTime.Now.ToString("HH:mm:ss");
                        selled.cpValue = actualPriceOfPaper;
                        selled.cpSumcom = actualPriceOfPaper * i.cp_amount;
                        selled.cpWinning = winningFifo;
                        DB.CommercialPaperSells.Add(selled);


                        DB.CommercialPapers.Remove(i);
                        DB.SaveChanges();
                        amount = amount - i.cp_amount;
                        dataGridCommercialsells.ItemsSource = DB.CommercialPaperSells.ToList();
                    }
                    else
                    {
                        winningFifo += (actualPriceOfPaper * amount) - (amount * i.cp_value);

                        selled.cpName = i.cp_name;
                        selled.cpAmount = amount;
                        selled.cpDate = DateTime.Today.ToString("yyyy.MM.dd");
                        selled.cpTime = DateTime.Now.ToString("HH:mm:ss");
                        selled.cpValue = actualPriceOfPaper;
                        selled.cpSumcom = actualPriceOfPaper * amount;
                        selled.cpWinning = winningFifo;
                        DB.CommercialPaperSells.Add(selled);

                        i.cp_amount = i.cp_amount - amount;
                        DB.SaveChanges();
                        List<CommercialPapers> b = DB.CommercialPapers.ToList();
                        foreach (var k in b)
                        {
                            k.sumcom = k.cp_value * k.cp_amount;
                        }
                        CommercialPapersDataGrid.ItemsSource = b;
                        dataGridCommercialsells.ItemsSource = DB.CommercialPaperSells.ToList();
                        break;
                    }

                    
                }

               

                MessageBox.Show("Árfolyam: " + actualPriceOfPaper + "\nNyereseg: " + winningFifo + "\n neve: " + name);
            }
            else
            {
                MessageBox.Show("Nincs elég értékpapír!");
            }
        }

        private void makeDiagram(object sender, RoutedEventArgs e)
        {
            //makediagramSeperate();
            lineChartCopy.Visibility = Visibility.Visible;
            lineChart.Visibility = Visibility.Hidden;
            List<KeyValuePair<string, int>> datalist = new List<KeyValuePair<string, int>>();
            List<RateTable> data = new List<RateTable>();
            data = DB.RateTable.ToList();
            var dataSourceList2 = new List<List<KeyValuePair<string, int>>>();
            string nameofPaper = comboBox1.Text.ToString();
            foreach (var i in data)
            {
                if (i.NameOfpaper.Contains(nameofPaper))
                {
                    datalist.Add(new KeyValuePair<string, int>(i.DateOf + "\n" + i.TimeOf, i.Price));
                }
            }
            dataSourceList2.Add(datalist);
            lineChartCopy.DataContext = dataSourceList2;
        }

        private void ListPapersByDate(object sender, RoutedEventArgs e)
        {
            var listofAllCommercialPaper = DB.CommercialPapers.ToList();
            List<CommercialPapers> filterredList = new List< CommercialPapers > ();
            string kezdo = BeginDate.ToString();
            string vegso = EndDate.ToString();
            foreach (var i in listofAllCommercialPaper)
            {
                if (DateTime.Parse(i.cp_date) > DateTime.Parse(kezdo) && DateTime.Parse(i.cp_date) < DateTime.Parse(vegso))
                {
                    filterredList.Add(i);
                }
            }
            dataGridFilterByDate.ItemsSource = filterredList;

        }

        private void grafikonReszvenyDarabszam()
        {
            List<KeyValuePair<string, int>> datalist = new List<KeyValuePair<string, int>>();
            List<ActualDBTable> data = new List<ActualDBTable>();
            data = DB.ActualDBTable.ToList();
            var dataSourceList2 = new List<List<KeyValuePair<string, int>>>();
            string nameofPaper = comboBoxcpname.Text.ToString();
            foreach (var i in data)
            {
                if (i.cpName.Contains(nameofPaper))
                {
                    datalist.Add(new KeyValuePair<string, int>(i.cpDate, i.cpDB));
                }
            }
            dataSourceList2.Add(datalist);
            lineChartForPaperDB.DataContext = dataSourceList2;
        }

        private void GrafikonDarabszam(object sender, RoutedEventArgs e)
        {
            grafikonReszvenyDarabszam();
        }
    }
}
