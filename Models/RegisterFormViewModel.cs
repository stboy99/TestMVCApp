using TestMVCApp.Models.Global;

namespace TestMVCApp.Models
{
    public class RegisterFormViewModel : FormViewModel
    {
        public RegisterFormViewModel() {
            ButtonText = "Register";
        }
        public Register Register { get; set; } = new Register();
    }
}