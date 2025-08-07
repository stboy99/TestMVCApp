using TestMVCApp.Models.Global;

namespace TestMVCApp.Models
{
    public class LoginFormViewModel : FormViewModel
    {
        public LoginFormViewModel()
        {
           ButtonText = "Login";
           LinkText = "Register";
           LinkUrl = "/Account/Register";
        }
    }
}