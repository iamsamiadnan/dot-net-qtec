using dot_net_qtec.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace dot_net_qtec.Pages
{
    public class LoginModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public LoginModel(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public class LoginDto
        {
            public string? UserName { get; set; }
            public string? Password { get; set; }
        }

        [BindProperty]
        public LoginDto _LoginDto { get; set; } = new LoginDto();
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {

            var result = await _signInManager.PasswordSignInAsync(_LoginDto.UserName!, _LoginDto.Password!, true, false);

            if (result.Succeeded)
            {
                return RedirectToPage('/');
            }

            return RedirectToPage("/Login");
        }
    }
}
