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

namespace WpfApp2
{
    public partial class SelectPartWindow : Window
    {
        private int _typeId;
        private List<BuildItem> _currentItems;
        public basepart_ SelectedPart { get; private set; }

        public SelectPartWindow(int typeId, List<BuildItem> currentItems)
        {
            InitializeComponent();
            _typeId = typeId;
            _currentItems = currentItems;

            var manufacturers = Core.DB.manufacturer_.ToList();
            manufacturers.Insert(0, new manufacturer_ { name = "Все производители" });
            ManufacturerCb.ItemsSource = manufacturers;
            ManufacturerCb.SelectedIndex = 0;

            UpdateList();
        }

        private void Filter_Changed(object sender, RoutedEventArgs e)
        {
            UpdateList();
        }

        private void UpdateList()
        {
            var query = Core.DB.basepart_.Where(p => p.parttypeid == _typeId).ToList();

            if (!string.IsNullOrWhiteSpace(SearchTb.Text))
                query = query.Where(p => p.name.ToLower().Contains(SearchTb.Text.ToLower())).ToList();

            if (ManufacturerCb.SelectedIndex > 0)
            {
                var selectedMan = ManufacturerCb.SelectedItem as manufacturer_;
                query = query.Where(p => p.manufacturerid == selectedMan.id).ToList();
            }

            PartsGrid.ItemsSource = query;
        }

        private void PartsGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (PartsGrid.SelectedItem is basepart_ part)
            {
                if (CheckCompatibility(part))
                {
                    SelectedPart = part;
                    DialogResult = true;
                }
            }
        }

        private bool CheckCompatibility(basepart_ part)
        {
            var cpuItem = _currentItems.FirstOrDefault(i => i.TypeId == 1)?.Part;
            var moboItem = _currentItems.FirstOrDefault(i => i.TypeId == 4)?.Part;
            var ramItem = _currentItems.FirstOrDefault(i => i.TypeId == 3)?.Part;
            var coolerItem = _currentItems.FirstOrDefault(i => i.TypeId == 7)?.Part;
            var caseItem = _currentItems.FirstOrDefault(i => i.TypeId == 5)?.Part;
            var gpuItem = _currentItems.FirstOrDefault(i => i.TypeId == 2)?.Part;
            var psuItem = _currentItems.FirstOrDefault(i => i.TypeId == 6)?.Part;

            if (_typeId == 1)
            {
                var cpuInfo = Core.DB.cpu_.FirstOrDefault(c => c.id == part.id);
                if (moboItem != null)
                {
                    var moboInfo = Core.DB.motherboard_.FirstOrDefault(m => m.id == moboItem.id);
                    if (cpuInfo.socketid != moboInfo.socketid) { MessageBox.Show("Несовместимый сокет с материнской платой."); return false; }
                }
                if (coolerItem != null)
                {
                    if (!Core.DB.socketprocessorcooler_.Any(s => s.socketid == cpuInfo.socketid && s.processorcoolerid == coolerItem.id)) { MessageBox.Show("Несовместимый сокет с кулером."); return false; }
                }
            }

            if (_typeId == 4)
            {
                var moboInfo = Core.DB.motherboard_.FirstOrDefault(m => m.id == part.id);
                if (cpuItem != null)
                {
                    var cpuInfo = Core.DB.cpu_.FirstOrDefault(c => c.id == cpuItem.id);
                    if (moboInfo.socketid != cpuInfo.socketid) { MessageBox.Show("Несовместимый сокет с процессором."); return false; }
                }
                if (ramItem != null)
                {
                    var ramInfo = Core.DB.ram_.FirstOrDefault(r => r.id == ramItem.id);
                    if (moboInfo.memorytypeid != ramInfo.memorytypeid) { MessageBox.Show("Несовместимый тип оперативной памяти."); return false; }
                }
                if (caseItem != null)
                {
                    if (!Core.DB.boardformfactorcase_.Any(b => b.caseid == caseItem.id && b.formfactorid == moboInfo.formfactorid)) { MessageBox.Show("Форм-фактор платы не подходит к корпусу."); return false; }
                }
            }

            if (_typeId == 3 && moboItem != null)
            {
                var ramInfo = Core.DB.ram_.FirstOrDefault(r => r.id == part.id);
                var moboInfo = Core.DB.motherboard_.FirstOrDefault(m => m.id == moboItem.id);
                if (moboInfo.memorytypeid != ramInfo.memorytypeid) { MessageBox.Show("Несовместимый тип памяти с материнской платой."); return false; }
            }

            if (_typeId == 5 && moboItem != null)
            {
                var moboInfo = Core.DB.motherboard_.FirstOrDefault(m => m.id == moboItem.id);
                if (!Core.DB.boardformfactorcase_.Any(b => b.caseid == part.id && b.formfactorid == moboInfo.formfactorid)) { MessageBox.Show("Корпус не поддерживает форм-фактор выбранной платы."); return false; }
            }

            if (_typeId == 7 && cpuItem != null)
            {
                var cpuInfo = Core.DB.cpu_.FirstOrDefault(c => c.id == cpuItem.id);
                if (!Core.DB.socketprocessorcooler_.Any(s => s.socketid == cpuInfo.socketid && s.processorcoolerid == part.id)) { MessageBox.Show("Кулер не подходит под сокет процессора."); return false; }
            }

            if (_typeId == 2 && psuItem != null)
            {
                var gpuInfo = Core.DB.gpu_.FirstOrDefault(g => g.id == part.id);
                var psuInfo = Core.DB.powersupply_.FirstOrDefault(p => p.id == psuItem.id);
                if (gpuInfo.recommendpower != null && psuInfo.power < gpuInfo.recommendpower) { MessageBox.Show("Мощность блока питания недостаточна для видеокарты."); return false; }
            }

            if (_typeId == 6 && gpuItem != null)
            {
                var psuInfo = Core.DB.powersupply_.FirstOrDefault(p => p.id == part.id);
                var gpuInfo = Core.DB.gpu_.FirstOrDefault(g => g.id == gpuItem.id);
                if (gpuInfo.recommendpower != null && psuInfo.power < gpuInfo.recommendpower) { MessageBox.Show("Мощность блока питания недостаточна для выбранной видеокарты."); return false; }
            }

            return true;
        }
    }
}