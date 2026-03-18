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
using System.Collections.ObjectModel;


namespace WpfApp2
{
    public partial class BuilderPage : Page
    {
        private ObservableCollection<BuildItem> _buildItems;

        public BuilderPage()
        {
            InitializeComponent();
            LoadSlots();
        }

        private void LoadSlots()
        {
            _buildItems = new ObservableCollection<BuildItem>
            {
                new BuildItem { TypeId = 1, TypeName = "Процессор" },
                new BuildItem { TypeId = 4, TypeName = "Материнская плата" },
                new BuildItem { TypeId = 7, TypeName = "Охлаждение процессора" },
                new BuildItem { TypeId = 3, TypeName = "Оперативная память" },
                new BuildItem { TypeId = 2, TypeName = "Видеокарта" },
                new BuildItem { TypeId = 8, TypeName = "Накопитель" },
                new BuildItem { TypeId = 6, TypeName = "Блок питания" },
                new BuildItem { TypeId = 5, TypeName = "Корпус" }
            };
            PartsGrid.ItemsSource = _buildItems;
            UpdatePrice();
        }

        private void SelectPart_Click(object sender, RoutedEventArgs e)
        {
            int typeId = (int)((Button)sender).Tag;
            SelectPartWindow window = new SelectPartWindow(typeId, _buildItems.ToList());
            if (window.ShowDialog() == true)
            {
                var item = _buildItems.First(i => i.TypeId == typeId);
                item.Part = window.SelectedPart;
                PartsGrid.Items.Refresh();
                UpdatePrice();
            }
        }

        private void UpdatePrice()
        {
            decimal total = _buildItems.Where(i => i.Part != null).Sum(i => i.Part.price);
            TotalPriceTb.Text = $"{total} ₽";
        }

        private void SaveBuild_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BuildNameTb.Text) || string.IsNullOrWhiteSpace(AuthorTb.Text))
            {
                MessageBox.Show("Введите название и автора сборки.");
                return;
            }

            if (!_buildItems.Any(i => i.Part != null))
            {
                MessageBox.Show("Сборка пуста.");
                return;
            }

            assembly_ newAssembly = new assembly_
            {
                name = BuildNameTb.Text,
                author = AuthorTb.Text
            };
            Core.DB.assembly_.Add(newAssembly);
            Core.DB.SaveChanges();

            foreach (var item in _buildItems.Where(i => i.Part != null))
            {
                partassembly_ pa = new partassembly_
                {
                    assemblyid = newAssembly.id,
                    partid = item.Part.id
                };
                Core.DB.partassembly_.Add(pa);
            }
            Core.DB.SaveChanges();

            MessageBox.Show("Сборка успешно сохранена!");
            LoadSlots();
            BuildNameTb.Clear();
            AuthorTb.Clear();
        }

    }
}
