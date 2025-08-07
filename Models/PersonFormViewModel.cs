using TestMVCApp.Models;
using TestMVCApp.Models.Global;

namespace TestMVCApp.Models
{
    public class PersonFormViewModel : FormViewModel
    {
        public PersonFormViewModel() {
            LinkText = "Cancel";
            LinkUrl = "/Person/Index";
            ButtonText = "Save";
        }
        public Person Person { get; set; } = new Person();
    }
}