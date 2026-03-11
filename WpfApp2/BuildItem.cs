using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    public class BuildItem
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public basepart_ Part { get; set; } // Здесь и далее basepart_ — имя сущности из вашей модели
    }
}
