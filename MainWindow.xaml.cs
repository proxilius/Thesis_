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
        public static SzamlaEntities DB = new SzamlaEntities();
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
        public void makeSubtotal(List<Szamlak> a)
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

        public void sumhozam()
        {
            int sum=0;
            foreach (var i in DB.CommercialPaperSells.ToList())
            {
                sum += i.cpWinning;
            }
            textBlockSumHozam.Text = "Összes hozam: "+sum+" Ft";
        }

        public void makeDiagramOfSzamlak()
        {
            List<KeyValuePair<string, int>> MyValue = new List<KeyValuePair<string, int>>();
            foreach (var i in DB.Szamlak.ToList())
            {
                MyValue.Add(new KeyValuePair<string, int>(i.Datum+"\n"+i.Idopont, i.Osszeg));
            }
            ColumnChart1.DataContext = MyValue;
        }


        public MainWindow()
        {


            SubtotalsList.Clear();
            List<Szamlak> a = DB.Szamlak.ToList();

            InitializeComponent();
            allMoney();
            makediagramSeperate();
            makediagram();
            actualDBS();
            grafikonReszvenyDarabszam();
            grafikonReszvenyErtek();
            sumOfCommercialPaper();
            sumhozam();
            makeDiagramOfSzamlak();
            List<KeyValuePair<string, int>> Values = new List<KeyValuePair<string, int>>();
           
                Values.Add(new KeyValuePair<string, int>("valami",500));


            PieChart12.DataContext = Values;

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
            dataGridActual.ItemsSource = DB.ActualTable.ToList();
            makeSubtotal(a);
            dgrid = SzamlaDatagrid;
            szamol2();
            OnPropertyChanged("szamol2");

        }


        public void szamol2()
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
            int id = 0;
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
            Microsoft.Office.Interop.Excel.Application excel =
                new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = 
                excel.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = 
                workbook.ActiveSheet;
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
            int actualPriceOfPaper = 0;//Az adott ertekpapir arfolyama, utolso ertek
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
                    selled.cpTime = DateTime.Now.ToString("HH:mm:ss");
                    selled.cpName = selling.cp_name;
                    selled.cpValue = actualPriceOfPaper;
                    selled.cpSumcom = actualPriceOfPaper * Convert.ToInt32(amountBox.Text);
                    selled.cpWinning = winning;
                    MessageBox.Show(selled.cpName + ", " + selled.cpAmount + "," + selled.cpValue + ", " + selled.cpDate);
                    DB.CommercialPaperSells.Add(selled);
                    DB.SaveChanges();
                    dataGridCommercialsells.ItemsSource = DB.CommercialPaperSells.ToList();
                    sumhozam();

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
                    int preveousHuf = 0;
                    foreach (var i in DB.ActualTable)
                    {
                        if (i.Name.Contains(selling.cp_name))
                            firstvalueforactual = i.AmountAfterChange;
                        preveousHuf = i.huf;
                    }
                    actualdbs.Name = selling.cp_name;
                    actualdbs.DateOf = DateTime.Today.ToString("yyyy.MM.dd");
                    actualdbs.TimeOf = DateTime.Now.ToString("HH:mm:ss");
                    actualdbs.Change = 0 - Convert.ToInt32(amountBox.Text);
                    actualdbs.actualRate = actualPriceOfPaper;
                    actualdbs.AmountAfterChange = firstvalueforactual - Convert.ToInt32(amountBox.Text);
                    actualdbs.Sum = actualdbs.actualRate * actualdbs.AmountAfterChange;
                    if (winning > 0)
                        actualdbs.huf = preveousHuf + (actualPriceOfPaper * Convert.ToInt32(amountBox.Text)) + winning;
                    else
                        actualdbs.huf = preveousHuf + (actualPriceOfPaper * Convert.ToInt32(amountBox.Text));
                    DB.ActualTable.Add(actualdbs);
                    DB.SaveChanges();
                    dataGridActual.ItemsSource = DB.ActualTable.ToList();
                    actualDBS();
                    MessageBox.Show("A nyereseg: " + winning + "\n" + "Az adózás után: " + winning * 0.85 + "\nAz értékpapír árfolyama: " + actualPriceOfPaper);

                }
                else
                {
                    MessageBox.Show("Nincs annyi darab, mint amennyit el szeretne adni.");
                }


            }
            catch (Exception ed)
            {
                MessageBox.Show("Hiba, jelölje ki az eladni kivánt értékpapírt.");
                MessageBox.Show(ed.ToString());
            }
        }
        public void sellFifoWithoutParams()
        {
            int actualPriceOfPaper = 0;
            int winningFifo = 0;
            int allMoney = 0;//AZ az osszeg amit hozza kell adni a kezpenz allomanyhoz, nyereseg+amibe kerult a paper
            string name = fifoname.Text.ToString();
            int amount = Convert.ToInt32(fifoamount.Text);//ennyit akarunk eladni
            int amountfix = Convert.ToInt32(fifoamount.Text);
            int actualPaperAmount = 0;
            List<CommercialPapers> cplist = new List<CommercialPapers>();
            List<CommercialPapers> sortedcplist = new List<CommercialPapers>();
            cplist = DB.CommercialPapers.Where(d => d.cp_name == name).ToList();
            string nevek = "";
            sortedcplist = cplist.OrderBy(d => d.cp_date).ThenBy(d => d.cp_time).ToList();
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



                foreach (var i in sortedcplist)
                {
                    CommercialPaperSells selled = new CommercialPaperSells();
                    int actualdb = i.cp_amount - amount;//10-15
                    if (actualdb <= 0)
                    {
                        winningFifo = (actualPriceOfPaper * i.cp_amount) - (i.cp_amount * i.cp_value);
                        allMoney += (actualPriceOfPaper * i.cp_amount);
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
                        List<CommercialPapers> b = DB.CommercialPapers.ToList();
                        foreach (var k in b)
                        {
                            k.sumcom = k.cp_value * k.cp_amount;
                        }
                        CommercialPapersDataGrid.ItemsSource = b;
                        if (actualdb == 0) break;
                    }
                    else
                    {
                        winningFifo = (actualPriceOfPaper * amount) - (amount * i.cp_value);
                        allMoney += (actualPriceOfPaper * amount);
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
                sumhozam();
                ActualTable actualdbs = new ActualTable();
                int firstvalueforactual = 0;//amikor meg nincs ertekpapir 
                int preveousHuf = 0;
                foreach (var l in DB.ActualTable)
                {
                    if (l.Name.Contains(name))
                        firstvalueforactual = l.AmountAfterChange;
                    preveousHuf = l.huf;
                }
                actualdbs.Name = name;
                actualdbs.DateOf = DateTime.Today.ToString("yyyy.MM.dd");
                actualdbs.TimeOf = DateTime.Now.ToString("HH:mm:ss");
                actualdbs.Change = 0 - amountfix;
                actualdbs.actualRate = actualPriceOfPaper;
                actualdbs.AmountAfterChange = firstvalueforactual - amountfix;
                actualdbs.Sum = actualdbs.actualRate * actualdbs.AmountAfterChange;
                actualdbs.huf = preveousHuf + allMoney;
                DB.ActualTable.Add(actualdbs);
                DB.SaveChanges();
                dataGridActual.ItemsSource = DB.ActualTable.ToList();

                actualDBS();
                MessageBox.Show("Árfolyam: " + actualPriceOfPaper + "\nNyereseg: " + winningFifo + "\n neve: " + name);
            }
            else
            {
                MessageBox.Show("Nincs elég értékpapír!");
            }
        }
        private void sellFifo(object sender, RoutedEventArgs e)
        {
            int actualPriceOfPaper = 0;
            int winningFifo = 0;
            int allMoney = 0;//AZ az osszeg amit hozza kell adni a kezpenz allomanyhoz, nyereseg+amibe kerult a paper
            string name = fifoname.Text.ToString();
            int amount = Convert.ToInt32(fifoamount.Text);//ennyit akarunk eladni
            int amountfix = Convert.ToInt32(fifoamount.Text);
            int actualPaperAmount = 0;
            List<CommercialPapers> cplist = new List<CommercialPapers>();
            List<CommercialPapers> sortedcplist = new List<CommercialPapers>();
            cplist = DB.CommercialPapers.Where(d => d.cp_name == name).ToList();
            string nevek = "";
            sortedcplist = cplist.OrderBy(d => d.cp_date).ThenBy(d => d.cp_time).ToList();
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



                foreach (var i in sortedcplist)
                {
                    CommercialPaperSells selled = new CommercialPaperSells();
                    int actualdb = i.cp_amount - amount;//10-15
                    if (actualdb <= 0)
                    {
                        winningFifo = (actualPriceOfPaper * i.cp_amount) - (i.cp_amount * i.cp_value);
                        allMoney += (actualPriceOfPaper * i.cp_amount);
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
                        List<CommercialPapers> b = DB.CommercialPapers.ToList();
                        foreach (var k in b)
                        {
                            k.sumcom = k.cp_value * k.cp_amount;
                        }
                        CommercialPapersDataGrid.ItemsSource = b;
                        if (actualdb == 0) break;
                    }
                    else
                    {
                        winningFifo = (actualPriceOfPaper * amount) - (amount * i.cp_value);
                        allMoney += (actualPriceOfPaper * amount);
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
                sumhozam();
                ActualTable actualdbs = new ActualTable();
                int firstvalueforactual = 0;//amikor meg nincs ertekpapir 
                int preveousHuf = 0;
                foreach (var l in DB.ActualTable)
                {
                    if (l.Name.Contains(name))
                        firstvalueforactual = l.AmountAfterChange;
                    preveousHuf = l.huf;
                }
                actualdbs.Name = name;
                actualdbs.DateOf = DateTime.Today.ToString("yyyy.MM.dd");
                actualdbs.TimeOf = DateTime.Now.ToString("HH:mm:ss");
                actualdbs.Change = 0 - amountfix;
                actualdbs.actualRate = actualPriceOfPaper;
                actualdbs.AmountAfterChange = firstvalueforactual - amountfix;
                actualdbs.Sum = actualdbs.actualRate * actualdbs.AmountAfterChange;
                actualdbs.huf = preveousHuf + allMoney+winningFifo;
                DB.ActualTable.Add(actualdbs);
                DB.SaveChanges();
                dataGridActual.ItemsSource = DB.ActualTable.ToList();

                actualDBS();
                MessageBox.Show("Árfolyam: " + actualPriceOfPaper + "\nNyereseg: " + winningFifo + "\n neve: " + name);
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
                actualdb.cpDate = DateTime.Now.ToString();
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
                actualdb.cpFulValue = actualPriceOfPaper * i.Value;
                actualdb.cpDate = DateTime.Today.ToString() + "," + DateTime.Now.ToString();
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
            actual.DateOf = getDate.Text;
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
        public void sellLifoWithoutParam()
        {
            int actualPriceOfPaper = 0;
            int allMoney = 0;
            int winningFifo = 0;
            string name = fifoname.Text.ToString();
            int amount = Convert.ToInt32(fifoamount.Text);//ennyit akarunk eladni
            int amountfix = Convert.ToInt32(fifoamount.Text);
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

                

                foreach (var i in sortedcplist)
                {
                    CommercialPaperSells selled = new CommercialPaperSells();
                    int actualdb = i.cp_amount - amount;//10-15
                    if (actualdb <= 0)
                    {
                        winningFifo = (actualPriceOfPaper * i.cp_amount) - (i.cp_amount * i.cp_value);
                        allMoney += (actualPriceOfPaper * i.cp_amount);
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
                        List<CommercialPapers> b = DB.CommercialPapers.ToList();
                        foreach (var k in b)
                        {
                            k.sumcom = k.cp_value * k.cp_amount;
                        }
                        CommercialPapersDataGrid.ItemsSource = b;
                        dataGridCommercialsells.ItemsSource = DB.CommercialPaperSells.ToList();
                        if (actualdb == 0) break;
                    }
                    else
                    {
                        winningFifo = (actualPriceOfPaper * amount) - (amount * i.cp_value);
                        allMoney += (actualPriceOfPaper * amount);
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
                        break;//kell még actual datagrid frissites
                    }
                    sumhozam();
                    ActualTable actualdbs = new ActualTable();
                    int firstvalueforactual = 0;//amikor meg nincs ertekpapir 
                    int preveousHuf = 0;
                    foreach (var l in DB.ActualTable)
                    {
                        if (l.Name.Contains(name))
                            firstvalueforactual = l.AmountAfterChange;
                        preveousHuf = l.huf;
                    }
                    actualdbs.Name = name;
                    actualdbs.DateOf = DateTime.Today.ToString("yyyy.MM.dd");
                    actualdbs.TimeOf = DateTime.Now.ToString("HH:mm:ss");
                    actualdbs.Change = 0 - amountfix;
                    actualdbs.actualRate = actualPriceOfPaper;
                    actualdbs.AmountAfterChange = firstvalueforactual - amountfix;
                    actualdbs.Sum = actualdbs.actualRate * actualdbs.AmountAfterChange;
                    actualdbs.huf = preveousHuf + allMoney;
                    DB.ActualTable.Add(actualdbs);
                    DB.SaveChanges();
                    dataGridActual.ItemsSource = DB.ActualTable.ToList();


                }



                MessageBox.Show("Árfolyam: " + actualPriceOfPaper + "\nNyereseg: " + winningFifo + "\n neve: " + name);
            }
            else
            {
                MessageBox.Show("Nincs elég értékpapír!");
            }
        }
        private void sellLifo(object sender, RoutedEventArgs e)
        {
            int actualPriceOfPaper = 0;
            int allMoney = 0;
            int winningFifo = 0;
            string name = fifoname.Text.ToString();
            int amount = Convert.ToInt32(fifoamount.Text);//ennyit akarunk eladni
            int amountfix = Convert.ToInt32(fifoamount.Text);
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

                

                foreach (var i in sortedcplist)
                {
                    CommercialPaperSells selled = new CommercialPaperSells();
                    int actualdb = i.cp_amount - amount;//10-15
                    if (actualdb <= 0)
                    {
                        winningFifo = (actualPriceOfPaper * i.cp_amount) - (i.cp_amount * i.cp_value);
                        allMoney += (actualPriceOfPaper * i.cp_amount);
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
                        List<CommercialPapers> b = DB.CommercialPapers.ToList();
                        foreach (var k in b)
                        {
                            k.sumcom = k.cp_value * k.cp_amount;
                        }
                        CommercialPapersDataGrid.ItemsSource = b;
                        dataGridCommercialsells.ItemsSource = DB.CommercialPaperSells.ToList();
                        if (actualdb == 0) break;
                    }
                    else
                    {
                        winningFifo = (actualPriceOfPaper * amount) - (amount * i.cp_value);
                        allMoney += (actualPriceOfPaper * amount);
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
                        break;//kell még actual datagrid frissites
                    }


                }
                sumhozam();
                ActualTable actualdbs = new ActualTable();
                int firstvalueforactual = 0;//amikor meg nincs ertekpapir 
                int preveousHuf = 0;
                foreach (var l in DB.ActualTable)
                {
                    if (l.Name.Contains(name))
                        firstvalueforactual = l.AmountAfterChange;
                    preveousHuf = l.huf;
                }
                actualdbs.Name = name;
                actualdbs.DateOf = DateTime.Today.ToString("yyyy.MM.dd");
                actualdbs.TimeOf = DateTime.Now.ToString("HH:mm:ss");
                actualdbs.Change = 0 - amountfix;
                actualdbs.actualRate = actualPriceOfPaper;
                actualdbs.AmountAfterChange = firstvalueforactual - amountfix;
                actualdbs.Sum = actualdbs.actualRate * actualdbs.AmountAfterChange;
                actualdbs.huf = preveousHuf + allMoney;
                DB.ActualTable.Add(actualdbs);
                DB.SaveChanges();
                dataGridActual.ItemsSource = DB.ActualTable.ToList();



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

        //private void ListPapersByDate(object sender, RoutedEventArgs e)
        //{
        //    var listofAllCommercialPaper = DB.CommercialPapers.ToList();
        //    List<CommercialPapers> filterredList = new List<CommercialPapers>();
        //    string kezdo = BeginDate.ToString();
        //    string vegso = EndDate.ToString();
        //    int sumofOTP = 0, sumofErste = 0, sumofMol = 0;
        //    int allSum = 0;

        //    foreach (var i in listofAllCommercialPaper)
        //    {
        //        if (DateTime.Parse(i.cp_date) > DateTime.Parse(kezdo) && DateTime.Parse(i.cp_date) < DateTime.Parse(vegso))
        //        {
        //            filterredList.Add(i);
        //            if (i.cp_name.Contains("OTP"))
        //            {
        //                sumofOTP += i.cp_value;
        //            }
        //            if (i.cp_name.Contains("MOL"))
        //            {
        //                sumofMol += i.cp_value;
        //            }
        //            if (i.cp_name.Contains("ERSTE"))
        //            {
        //                sumofErste += i.cp_value;
        //            }
        //            allSum += i.cp_value;
        //        }
        //    }
        //    dataGridFilterByDate.ItemsSource = filterredList;
        //    textBlockSumForMonth.Text = "Összesen: " + allSum + " Ft\nEbből\nOTP: " + sumofOTP + " Ft\nMOL: " + sumofMol + " Ft\nERSTE: " + sumofErste;

        //}

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

        private void grafikonReszvenyErtek()
        {
            string nameofPaper = comboBox2.Text.ToString();
            int actualPriceOfPaper = 1;
            foreach (var i in DB.RateTable.ToList())
            {
                if (i.NameOfpaper.Contains(nameofPaper))
                {
                    actualPriceOfPaper = i.Price;
                }
            }
            List<KeyValuePair<string, int>> datalist3 = new List<KeyValuePair<string, int>>();
            List<ActualDBTable> data3 = new List<ActualDBTable>();
            data3 = DB.ActualDBTable.ToList();
            var dataSourceList3 = new List<List<KeyValuePair<string, int>>>();
            foreach (var i in data3)
            {
                if (i.cpName.Contains(nameofPaper))
                {
                    datalist3.Add(new KeyValuePair<string, int>(i.cpDate, i.cpDB * actualPriceOfPaper));
                }
            }
            dataSourceList3.Add(datalist3);
            lineChartForPaperValue.DataContext = dataSourceList3;

        }

        private void GrafikonReszvenyErtek(object sender, RoutedEventArgs e)
        {
            grafikonReszvenyErtek();

        }

        private void ChangeSize(object sender, RoutedEventArgs e)
        {
            int L = Convert.ToInt32(textBoxHeight.Text);
            int W = Convert.ToInt32(textBoxWidth.Text);
            graphicGrid.Width = W;
            graphicGrid.Height = L;
        }

        private void ChangeSize2(object sender, RoutedEventArgs e)
        {
            int L = Convert.ToInt32(textBoxHeight2.Text);
            int W = Convert.ToInt32(textBoxWidth2.Text);
            graphicGrid2.Width = W;
            graphicGrid2.Height = L;
        }

        private void ChangeSize3(object sender, RoutedEventArgs e)
        {
            int L = Convert.ToInt32(textBoxHeight1.Text);
            int W = Convert.ToInt32(textBoxWidth1.Text);
            graphicSumGrid.Width = W;
            graphicSumGrid.Height = L;
        }

        public void sumOfCommercialPaper()
        {
            int sum = 0;
            int szamlasum = 0;
            foreach (var i in DB.CommercialPaperFix.ToList())
            {
                sum += i.sumcom;
                if (i.cp_name.Contains("HUF"))
                {
                    szamlasum += i.cp_value;
                }
            }
            textBlockSum.Text = "Összesen: " + sum + " Ft\nEbből huf: " + szamlasum + " Ft";
        }

        private void addAsHuf(object sender, RoutedEventArgs e)
        {
            int id = 0;
            try
            {
                id = (SzamlaDatagrid.SelectedItem as Szamlak).Id;
                Szamlak trans = new Szamlak();
                Szamlak uj = new Szamlak();
                CommercialPaperFix huf = new CommercialPaperFix();
                ActualTable act = new ActualTable();
                trans = DB.Szamlak.Where(d => d.Id == id).First();
                uj.Datum = trans.Datum;
                uj.Idopont = DateTime.Now.ToString("HH:mm:ss");
                uj.Datum = DateTime.Today.ToString("yyyy.MM.dd");
                uj.Megnevezes = trans.Megnevezes;
                uj.Osszeg = 0 - trans.Osszeg;

                huf.cp_name = "HUF: " + trans.Megnevezes; ;
                huf.cp_value = trans.Osszeg;
                huf.cp_date = DateTime.Today.ToString("yyyy.MM.dd");
                huf.cp_time = DateTime.Now.ToString("HH:mm:ss");
                huf.cp_amount = 1;

                int prevHuf = 0;
                foreach (var i in DB.ActualTable)
                {
                    prevHuf = i.huf;
                }

                act.Name = "HUF";
                act.AmountAfterChange = 0;
                act.Change = 0;
                act.DateOf = DateTime.Now.ToString("yyyy.MM.dd");
                act.actualRate = 0;
                act.huf = prevHuf + trans.Osszeg;
                act.TimeOf = DateTime.Now.ToString("HH:mm:ss");

                DB.CommercialPaperFix.Add(huf);
                DB.ActualTable.Add(act);
                DB.Szamlak.Add(uj);
                DB.SaveChanges();
                dataGridCommercialFix.ItemsSource = DB.CommercialPaperFix.ToList();
                SzamlaDatagrid.ItemsSource = DB.Szamlak.ToList();

                szamol2();
                makeSubtotal(DB.Szamlak.ToList());
                dataGridActual.ItemsSource = DB.ActualTable.ToList();
            }
            catch
            {
                MessageBox.Show("Hiba, jelölje ki az átvinni kivánt sort.");
            }
        }
        public Dictionary<string, int> dateDict { get; set; }
        public void allMoney()
        {
            dateDict = new Dictionary<string, int>();
            List<ActualDBTable> darabok = new List<ActualDBTable>();
            List<string> dates = new List<string>();
            List<string> DatesDistinct = new List<string>();


            darabok = DB.ActualDBTable.ToList();
            foreach (var i in darabok)
            {
                dates.Add(i.cpDate);
            }

            DatesDistinct = dates.Distinct().ToList();
            foreach (var i in DatesDistinct)
            {
                dateDict.Add(i, 0);
            }

            foreach (var i in darabok)
            {

                dateDict[i.cpDate] += i.cpFulValue;
            }

            List<KeyValuePair<string, int>> datalist = new List<KeyValuePair<string, int>>();
            List<KeyValuePair<string, int>> datalistPlusHuf = new List<KeyValuePair<string, int>>();
            List<RateTable> data = new List<RateTable>();
            data = DB.RateTable.ToList();
            var dataSourceList2 = new List<List<KeyValuePair<string, int>>>();
            string nameofPaper = comboBox1.Text.ToString();
            foreach (var i in dateDict)
            {
                int huf = 0;
                foreach (var k in DB.ActualTable.ToList())
                {
                    if (DateTime.Parse(k.DateOf) < DateTime.Parse(i.Key))
                    {
                        huf = k.huf;
                    }
                }
                datalist.Add(new KeyValuePair<string, int>(i.Key, i.Value));
                datalistPlusHuf.Add(new KeyValuePair<string, int>(i.Key, i.Value + huf));
            }
            dataSourceList2.Add(datalist);
            dataSourceList2.Add(datalistPlusHuf);
            lineChartForPaperSum.DataContext = dataSourceList2;


        }
        /// <summary>
        /// /////////////
        /// </summary>

        //public void Shuffle<T>(this List<T> list)
        //{
        //    Random rng = new Random();
        //    int n = list.Count;
        //    while (n > 1)
        //    {
        //        n--;
        //        int k = rng.Next(n + 1);
        //        T value = list[k];
        //        list[k] = list[n];
        //        list[n] = value;
        //    }
        //}


        private void bestSell(object sender, RoutedEventArgs e)
        {
            int fifoWinning = countFifoWinning();
            int lifoWinning = countLifoWinning();
            int randomWinnig = randomWinning();
            int[] array = new int[3];
            array[0] = fifoWinning;
            array[1] = lifoWinning;
            array[2] = randomWinnig;
            int lowest = 0;

            MessageBox.Show("Fifo eladas eseten hozam: " + fifoWinning + "\nLifo eseten hozam: " + lifoWinning + ", random eseten: " + randomWinnig);

            if (fifoWinning > 0 || lifoWinning > 0 || randomWinnig > 0)//is there at least a sell that has positive income
            {
                lowest = array.Where(i => i > 0).Min();
            }
            MessageBox.Show(lowest.ToString());

            if (lowest == fifoWinning)
            {
                sellFifoWithoutParams();
                MessageBox.Show("Fifo");
            }
            else if (lowest == lifoWinning)
            {
                sellLifoWithoutParam();
                MessageBox.Show("Lifo");
            }
            else if (lowest == randomWinnig)
            {
                sellRandomly();
                MessageBox.Show("Random");
            }

        }

        public void sellRandomly()
        {
            int actualPriceOfPaper = 0;
            int randomWinnig = 0;
            int allMoney = 0;
            int randomWinningSum = 0;//kell sum winnning fifohoz lifohoz az aktualis tabla huf miatt
            string name = fifoname.Text.ToString();
            foreach (var i in DB.RateTable.ToList())
            {
                if (i.NameOfpaper.Contains(name))
                {
                    actualPriceOfPaper = i.Price;//set actual price
                }
            }
            List<CommercialPapers> cplist = new List<CommercialPapers>();
            cplist = DB.CommercialPapers.Where(d => d.cp_name == name).ToList();
            List<int> ids = new List<int>();
            foreach (var i in cplist)
            {
                ids.Add(i.cp_id);
            }
            var shuffids = ids.OrderBy(a => Guid.NewGuid()).ToList();//randomize elements order
            int amount = Convert.ToInt32(fifoamount.Text);//ennyit akarunk eladni
            
            foreach (var index in shuffids)
            {
                var actualBatch = DB.CommercialPapers.Where(d => d.cp_id == index).First(); ;
                int actualdb = actualBatch.cp_amount - amount;//10-15
                CommercialPaperSells selled = new CommercialPaperSells();
                if (actualdb <= 0)
                {
                    randomWinningSum += (actualPriceOfPaper * actualBatch.cp_amount) - (actualBatch.cp_amount * actualBatch.cp_value);
                    randomWinnig = (actualPriceOfPaper * actualBatch.cp_amount) - (actualBatch.cp_amount * actualBatch.cp_value);
                    allMoney += (actualPriceOfPaper * actualBatch.cp_amount);
                    amount = amount - actualBatch.cp_amount;
                    selled.cpName = actualBatch.cp_name;
                    selled.cpAmount = actualBatch.cp_amount;
                    selled.cpDate = DateTime.Today.ToString("yyyy.MM.dd");
                    selled.cpTime = DateTime.Now.ToString("HH:mm:ss");
                    selled.cpValue = actualPriceOfPaper;
                    selled.cpSumcom = actualPriceOfPaper * actualBatch.cp_amount;
                    selled.cpWinning = randomWinnig;
                    DB.CommercialPaperSells.Add(selled);


                    DB.CommercialPapers.Remove(actualBatch);
                    DB.SaveChanges();
                    dataGridCommercialsells.ItemsSource = DB.CommercialPaperSells.ToList();
                    List<CommercialPapers> b = DB.CommercialPapers.ToList();
                    foreach (var k in b)
                    {
                        k.sumcom = k.cp_value * k.cp_amount;
                    }
                    CommercialPapersDataGrid.ItemsSource = b;
                    if (actualdb == 0) break;
                }
                else
                {
                    randomWinningSum += (actualPriceOfPaper * actualBatch.cp_amount) - (actualBatch.cp_amount * actualBatch.cp_value);
                    randomWinnig = (actualPriceOfPaper * amount) - (amount * actualBatch.cp_value);
                    allMoney += (actualPriceOfPaper * actualBatch.cp_amount);
                    actualBatch.cp_amount = actualBatch.cp_amount - amount;
                    selled.cpName = actualBatch.cp_name;
                    selled.cpAmount = actualBatch.cp_amount;
                    selled.cpDate = DateTime.Today.ToString("yyyy.MM.dd");
                    selled.cpTime = DateTime.Now.ToString("HH:mm:ss");
                    selled.cpValue = actualPriceOfPaper;
                    selled.cpSumcom = actualPriceOfPaper * actualBatch.cp_amount;
                    selled.cpWinning = randomWinnig;
                    DB.CommercialPaperSells.Add(selled);
                    dataGridCommercialsells.ItemsSource = DB.CommercialPaperSells.ToList();
                    List<CommercialPapers> b = DB.CommercialPapers.ToList();
                    foreach (var k in b)
                    {
                        k.sumcom = k.cp_value * k.cp_amount;
                    }
                    CommercialPapersDataGrid.ItemsSource = b;
                    DB.SaveChanges();
                    break;
                }
                sumhozam();
                ActualTable actualdbs = new ActualTable();
                int firstvalueforactual = 0;//amikor meg nincs ertekpapir 
                int preveousHuf = 0;
                foreach (var l in DB.ActualTable)
                {
                    if (l.Name.Contains(name))
                        firstvalueforactual = l.AmountAfterChange;
                    preveousHuf = l.huf;
                }
                actualdbs.Name = name;
                actualdbs.DateOf = DateTime.Today.ToString("yyyy.MM.dd");
                actualdbs.TimeOf = DateTime.Now.ToString("HH:mm:ss");
                actualdbs.Change = 0 - amount;
                actualdbs.actualRate = actualPriceOfPaper;
                actualdbs.AmountAfterChange = firstvalueforactual - amount;
                actualdbs.Sum = actualdbs.actualRate * actualdbs.AmountAfterChange;
                actualdbs.huf = preveousHuf + allMoney;
                DB.ActualTable.Add(actualdbs);
                DB.SaveChanges();
                dataGridActual.ItemsSource = DB.ActualTable.ToList();
            }
        }

        public int randomWinning()
        {
            int actualPriceOfPaper = 0;
            int randomWinnig = 0;
            string name = fifoname.Text.ToString();
            foreach (var i in DB.RateTable.ToList())
            {
                if (i.NameOfpaper.Contains(name))
                {
                    actualPriceOfPaper = i.Price;//set actual price
                }
            }
            List<CommercialPapers> cplist = new List<CommercialPapers>();
            cplist = DB.CommercialPapers.Where(d => d.cp_name == name).ToList();
            List<int> ids = new List<int>();
            foreach (var i in cplist)
            {
                ids.Add(i.cp_id);
            }
            var shuffids = ids.OrderBy(a => Guid.NewGuid()).ToList();//randomize elements order
            string s = "";
            foreach (var i in shuffids)
            {
                s += i+", ";
            }
            
            int amount = Convert.ToInt32(fifoamount.Text);//ennyit akarunk eladni
            foreach (var index in shuffids)
            {
                var actualBatch = DB.CommercialPapers.Where(d => d.cp_id == index).First(); ;
                int actualdb = actualBatch.cp_amount - amount;//10-15
                if (actualdb <= 0)
                {
                    randomWinnig += (actualPriceOfPaper * actualBatch.cp_amount) - (actualBatch.cp_amount * actualBatch.cp_value);
                    amount = amount - actualBatch.cp_amount;
                    s += ". ";
                    if (actualdb == 0) break;
                }
                else
                {
                    randomWinnig += (actualPriceOfPaper * amount) - (amount * actualBatch.cp_value);
                    actualBatch.cp_amount = actualBatch.cp_amount - amount;
                    s += "-";
                    break;
                }
            }
            //MessageBox.Show(s);
            return randomWinnig;
        }

        public int countFifoWinning()
        {
            int actualPriceOfPaper = 0;
            int fifoWinning = 0;
            string name = fifoname.Text.ToString();
            int amount = Convert.ToInt32(fifoamount.Text);//ennyit akarunk eladni
            int actualPaperAmount = 0;
            List<CommercialPapers> cplist = new List<CommercialPapers>();
            List<CommercialPapers> sortedcplist = new List<CommercialPapers>();
            cplist = DB.CommercialPapers.Where(d => d.cp_name == name).ToList();
            string nevek = "";
            sortedcplist = cplist.OrderBy(d => d.cp_date).ThenBy(d => d.cp_time).ToList();
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

                foreach (var i in sortedcplist)
                {
                    CommercialPaperSells selled = new CommercialPaperSells();
                    int actualdb = i.cp_amount - amount;//10-15
                    if (actualdb <= 0)
                    {
                        fifoWinning += (actualPriceOfPaper * i.cp_amount) - (i.cp_amount * i.cp_value);
                        amount = amount - i.cp_amount;
                        if (actualdb == 0) break;
                    }
                    else
                    {
                        fifoWinning += (actualPriceOfPaper * amount) - (amount * i.cp_value);
                        i.cp_amount = i.cp_amount - amount;

                        break;
                    }

                }

            }
            return fifoWinning;
        }
        public int countLifoWinning()
        {
            int actualPriceOfPaper = 0;
            int lifoWinning = 0;
            string name = fifoname.Text.ToString();
            int amount = Convert.ToInt32(fifoamount.Text);//ennyit akarunk eladni
            int actualPaperAmount = 0;
            List<CommercialPapers> cplist = new List<CommercialPapers>();
            List<CommercialPapers> sortedcplist = new List<CommercialPapers>();
            cplist = DB.CommercialPapers.Where(d => d.cp_name == name).ToList();
            string nevek = "";
            sortedcplist = cplist.OrderBy(d => d.cp_date).ThenBy(d => d.cp_time).ToList();
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

                foreach (var i in sortedcplist)
                {
                    CommercialPaperSells selled = new CommercialPaperSells();
                    int actualdb = i.cp_amount - amount;//10-15
                    if (actualdb <= 0)
                    {
                        lifoWinning += (actualPriceOfPaper * i.cp_amount) - (i.cp_amount * i.cp_value);
                        amount = amount - i.cp_amount;
                        if (actualdb == 0) break;
                    }
                    else
                    {
                        lifoWinning += (actualPriceOfPaper * amount) - (amount * i.cp_value);
                        i.cp_amount = i.cp_amount - amount;

                        break;
                    }

                }

            }
            return lifoWinning;
        }

        private void listTransByDate(object sender, RoutedEventArgs e)
        {
            var listofAllTrans = DB.ActualTable.ToList();
            List<ActualTable> filterredList = new List<ActualTable>();
            string kezdo = BeginDate.ToString();
            string vegso = EndDate.ToString();
            int sumofOTP = 0, sumofErste = 0, sumofMol = 0;
            int allSum = 0;

            foreach (var i in listofAllTrans)
            {
                if (DateTime.Parse(i.DateOf) > DateTime.Parse(kezdo) && DateTime.Parse(i.DateOf) < DateTime.Parse(vegso))
                {
                    filterredList.Add(i);
                }
            }
            dataGridActualFiltered.ItemsSource = filterredList;

        }

        private void CommercialPapersOnDate(object sender, RoutedEventArgs e)
        {
            List<ActualDBTable> all = new List<ActualDBTable>();
            List<ActualDBTable> otplist = new List<ActualDBTable>();
            List<ActualDBTable> erstelist = new List<ActualDBTable>();
            List<ActualDBTable> mollist = new List<ActualDBTable>();
            List<ActualDBTable> neededValues = new List<ActualDBTable>();
            //otplist.Add(new ActualDBTable());
            //mollist.Add(new ActualDBTable());
            //erstelist.Add(new ActualDBTable());
            all = DB.ActualDBTable.ToList();
            foreach (var i in all)
            {
                if (i.cpDate.Substring(0, 11) == dateOftheday.ToString().Substring(0, 11))
                    if (DateTime.Parse(i.cpDate) < DateTime.Parse(dateOftheday.Text.ToString() + " " + textBoxforTime.Text.ToString()))
                    {
                        if (i.cpName.Contains("OTP"))
                            otplist.Add(i);
                        if (i.cpName.Contains("MOL"))
                            mollist.Add(i);
                        if (i.cpName.Contains("ERSTE"))
                            erstelist.Add(i);
                    }
            }
            //MessageBox.Show(dateOftheday.Text.ToString() + " " + textBoxforTime.Text.ToString());
            List<ActualDBTable> helpList = new List<ActualDBTable>();
            helpList.AddRange(otplist);
            helpList.AddRange(mollist);
            helpList.AddRange(erstelist);
            DateTime maxDate= DateTime.MinValue;
            foreach (var i in helpList)
            {
                DateTime date = DateTime.Parse(i.cpDate);
                if (date > maxDate)
                    maxDate = date;
            }
            if (otplist.Last().cpDate == maxDate.ToString())
                neededValues.Add(otplist.Last());
            if (erstelist.Last().cpDate == maxDate.ToString())
                neededValues.Add(erstelist.Last());
            if (mollist.Last().cpDate == maxDate.ToString())
                neededValues.Add(mollist.Last());

            dataGridSumOnDate.ItemsSource = neededValues;
            List<KeyValuePair<string, int>> Values = new List<KeyValuePair<string, int>>();
            foreach (var i in neededValues)
            {
                Values.Add(new KeyValuePair<string, int>(i.cpName, i.cpFulValue));
            }

            PieChart12.DataContext = Values;
        }

    }
}
