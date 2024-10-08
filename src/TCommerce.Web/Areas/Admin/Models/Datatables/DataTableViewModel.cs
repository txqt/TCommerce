﻿using System.Drawing.Printing;

namespace TCommerce.Web.Areas.Admin.Models.Datatables
{
    public class DataTableViewModel
    {
        public DataTableViewModel()
        {
            TableId = Guid.NewGuid().ToString().Replace("-", "");
            DataSource = "";
        }

        public string TableId { get; set; }
        public string TableTitle { get; set; }
        public string CreateUrl { get; set; }
        public string SearchButtonId { get; set; }
        public string CreateButtonName { get; set; } = null;
        public List<ColumnDefinition> Columns { get; set; }
        public string GetDataUrl { get; set; }
        public bool PopupWindow { get; set; } = false;
        public List<int> LengthMenu { get; set; } = new List<int>() { 10, 25, 50, 100, 200 };
        public int PageLength { get; set; } = 10;
        public bool ServerSide { get; set; } = false;
        public bool Processing { get; set; } = true;
        public List<Filter> Filters { get; set; }
        public string EditUrl { get; set; }
        public string DeleteUrl { get; set; }
        public DefaultSort DefaultSort { get; set; }
        public string DataSource { get; set; }
    }
    public class DefaultSort
    {
        public DefaultSort()
        {
        }

        public DefaultSort(int columnIndex, string order)
        {
            ColumnIndex = columnIndex;
            Order = order;
        }

        public int ColumnIndex { get; set; }
        public string Order { get; set; }
    }
    public class ColumnDefinition
    {
        public ColumnDefinition(string data)
        {
            RenderType = RenderType.NotRender;
            Width = "100px";
            Searchable = true;
            Orderable = true;
            AutoWidth = true;
            Visible = true;
            Title = " ";
            CheckBoxName = "selectedIds";
            if (!string.IsNullOrEmpty(data))
            {
                Data = data;
            }
        }

        public ColumnDefinition()
        {
            RenderType = RenderType.NotRender;
            Width = "100px";
            Searchable = true;
            Orderable = true;
            AutoWidth = true;
            Visible = true;
            Title = " ";
            CheckBoxName = "selectedIds";
        }
        public string Data { get; set; }
        public string Title { get; set; } = "";
        public bool IsMasterCheckBox { get; set; }
        public string Width { get; set; }
        public bool Visible { get; set; }
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public bool AutoWidth { get; set; }
        public string ClassName { get; set; }
        public RenderType RenderType { get; set; }
        public string CheckBoxName { get; set; }
        public bool Editable { get; set; }
        public string FunctionName { get; set; }
    }

    public enum RenderType
    {
        NotRender,
        RenderBoolean,
        RenderCheckBox,
        RenderPicture,
        RenderButtonRemove,
        RenderButtonEdit,
        RenderInlineEdit,
        Custom
    }

    public class Filter
    {
        public Filter(string name)
        {
            Name = name;
            Type = typeof(string);
        }

        public Filter(string name, string modelName)
        {
            Name = name;
            Type = typeof(string);
            ModelName = modelName;
        }

        public string Name { get; set; }
        public Type Type { get; set; }
        public string ParentName { get; set; }
        public string ModelName { get; set; }
        public object Value { get; set; }
    }
}
