using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimaSzamlaAdatbazissal
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public SzamlaEntities DB = new SzamlaEntities();
        public ObservableCollection<Subtotals> Subtotals
        {
            get;
            set;
        }

        public void makeSubtoal()
        {
            ObservableCollection<Subtotals> subtotal = new ObservableCollection<Subtotals>();
            subtotal.Add(new Subtotals(){ sub = DB.Szamlak.Sum(f => f.Osszeg)});

            Subtotals = subtotal;
    }

    }
}
