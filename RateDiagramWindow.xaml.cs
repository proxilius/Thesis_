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
    /// Interaction logic for RateDiagramWindow.xaml
    /// </summary>
    public partial class RateDiagramWindow : Window
    {
        private SzamlaEntities DB =new SzamlaEntities();
        public RateDiagramWindow()
        {
            InitializeComponent();
            List<KeyValuePair<string, int>> otplist = new List<KeyValuePair<string, int>>();
            List<KeyValuePair<string, int>> mollist = new List<KeyValuePair<string, int>>();
            List<KeyValuePair<string, int>> erstelist = new List<KeyValuePair<string, int>>();
            List<RateTable> data = new List<RateTable>();
            data = DB.RateTable.ToList();
            var dataSourceList = new List<List<KeyValuePair<string, int>>>();
            string s = "";
            foreach (var i in data)
            {
                s= i.NameOfpaper;
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
    }
}
