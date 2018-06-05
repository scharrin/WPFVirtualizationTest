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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VirtualizationTest
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // create the demo items provider according to specified parameters
            int numItems = int.Parse(tbNumItems.Text);
            int fetchDelay = int.Parse(tbFetchDelay.Text);
            ParticalCrewListMaker customerProvider = new ParticalCrewListMaker(numItems, fetchDelay);

            // create the collection according to specified parameters
            //int pageSize = int.Parse(tbPageSize.Text);
            int pageSize = 100;
            //int pageTimeout = int.Parse(tbPageTimeout.Text);
            int pageTimeoutInSec = 3;

            if (rbNormal.IsChecked.Value)
            {
                //DataContext = new List<Crew>(customerProvider.FetchRange(0, customerProvider.FetchCount()));
            }
            else if (rbVirtualizing.IsChecked.Value)
            {
                //DataContext = new VirtualizingCollection<Crew>(customerProvider, pageSize);
            }
            else if (rbAsync.IsChecked.Value)
            {
                DataContext = new ParticalList<Crew>(customerProvider, pageSize, pageTimeoutInSec * 1000);
            }
        }
    }
}
