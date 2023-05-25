namespace T.Web.Areas.Admin.Models
{
    public class Summernote
    {
        public Summernote(string idEditor, bool loadLibrary = true)
        {
            IdEditor = idEditor;
            LoadLibrary = loadLibrary;
        }
        public string IdEditor { get; set; }
        public bool LoadLibrary { get; set; }
        public int height { get; set; } = 200;
        public string toolbar { get; set; } = @"
                [
                   ['style', ['style']],
                   ['font', ['bold', 'underline', 'clear']],
                   ['color', ['color']],
                   ['para', ['ul', 'ol', 'paragraph']],
                   ['table', ['table']],
                   ['insert', ['link', 'picture', 'video']],
                   ['view', ['fullscreen', 'codeview', 'help']]
                ]
        ";
    }
}
//$(document).ready(function() {
//            $('@Model.IdEditor').summernote({
//    height: 120,
//                toolbar: [
//                    ['style', ['style']],
//                    ['font', ['bold', 'underline', 'clear']],
//                    ['color', ['color']],
//                    ['para', ['ul', 'ol', 'paragraph']],
//                    ['table', ['table']],
//                    ['insert', ['link', 'picture', 'video']],
//                    ['view', ['fullscreen', 'codeview', 'help']]
//                ]
//            });
//});
