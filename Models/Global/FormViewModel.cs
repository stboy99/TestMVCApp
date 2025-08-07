namespace TestMVCApp.Models.Global
{
    // Models/MotherFormView.cs
    public class FormViewModel
    {
        public string FormAction { get; set; } = "";
        public string ButtonText { get; set; } = "Submit";
        public string? LinkText { get; set; } = "Back to Home";
        public string? LinkUrl { get; set; } = "/";
        public List<FormField> Fields { get; set; } = new();
        public string? ErrorMessage { get; set; } = null;
        public string? SuccessMessage { get; set; } = null;
    }
}