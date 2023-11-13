namespace T.Web.Areas.Admin.Models
{
    public class CKEditor
    {
        public CKEditor(string idEditor, bool loadLibrary = true)
        {
            IdEditor = idEditor;
            LoadLibrary = loadLibrary;
        }
        public string IdEditor { get; set; }
        public bool LoadLibrary { get; set; }
        public int height { get; set; } = 200;
        public string toolbar { get; set; } = @"
    [
        { name: 'clipboard', items: [ 'Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo' ] },
        { name: 'editing', items: [ 'Find', 'Replace', '-', 'SelectAll', '-', 'Scayt' ] },
        // Add more toolbar items as needed
    ]
";
    }
}
