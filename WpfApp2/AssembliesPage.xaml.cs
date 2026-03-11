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


namespace WpfApp2
{
    public partial class AssembliesPage : Page
    {
        public AssembliesPage()
        {
            InitializeComponent();
            AssembliesGrid.ItemsSource = Core.DB.assembly_.ToList();
        }

        private void AssembliesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AssembliesGrid.SelectedItem is assembly_ selectedAssembly)
            {
                var parts = Core.DB.partassembly_
                    .Where(pa => pa.assemblyid == selectedAssembly.id)
                    .Select(pa => pa.basepart_)
                    .ToList();

                AssemblyPartsGrid.ItemsSource = parts;
            }
        }
    }
}
