using dot_net_qtec.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace dot_net_qtec.Pages.Admin.Users
{
    [Authorize(Roles = "admin")]
    public class CreateModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public CreateModel(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public class CreateUserDto : User
        {
            public string Password { get; set; } = String.Empty;
        }

        [BindProperty]
        public CreateUserDto _CreateUserDto { get; set; } = new CreateUserDto();
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {


            User user = new User
            {
                UserName = _CreateUserDto.UserName,
                Email = _CreateUserDto.Email,
                FirstName = _CreateUserDto.FirstName,
                LastName = _CreateUserDto.LastName,
            };

            IdentityResult result = await _userManager.CreateAsync(user, _CreateUserDto.Password);

            if (result.Succeeded)
            {
                Console.WriteLine("REGISTERED!");

                await _userManager.AddToRoleAsync(user, "VIEWER");
            }

            return RedirectToPage("Index");
        }
    }
}
