using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T.Library.Model.Table
{
    public class DatatableModel
    {
        public DatatableModel(List<ColumnProperty> columns)
        {
            Columns = columns;
        }
        public List<ColumnProperty> Columns { get; set; }
        public string GetListController { get; set; }
        public string GetListAction { get; set; }
        public object GetListParemeter { get; set; }
        public string CreateNewController { get; set; }
        public string CreateNewAction { get; set; }
        public object CreateNewParemeter { get; set; }
    }
}
