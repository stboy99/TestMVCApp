namespace TestMVCApp.Models.Global
{
    // Models/FormField.cs
    public class FormField
    {
        public string Name { get; set; } = "";
        public string Label { get; set; } = "";
        public string Type { get; set; } = "text"; // "text", "email", "number", "select", "textarea", "checkbox", etc.
        public bool IsRequired { get; set; } = false;
        public string? Value { get; set; } = null;

        // For select, radio, or checkbox lists
        public List<SelectOption>? Options { get; set; } = null;
    }
}