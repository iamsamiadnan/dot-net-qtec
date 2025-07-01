using dot_net_qtec.Models;
using dot_net_qtec.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace dot_net_qtec.Pages.Admin.Users
{
    public class EditModel : PageModel
    {
        private readonly SqlManager _sqlManager;
        public EditModel(SqlManager sqlManager)
        {
            _sqlManager = sqlManager;
        }
        public class EditUserDto
        {

            public string? FirstName { get; set; }
            public string? LastName { get; set; }
        }

        [BindProperty]
        public EditUserDto _EditUserDto { get; set; } = new EditUserDto();
        public IActionResult OnGet(string? id)
        {
            if (id == null) return RedirectToPage("Index");


            var user = _sqlManager.ExecuteReader<EditUserDto>(
                $"SELECT * FROM ASPNETUSERS WHERE ID = @ID;",
                reader => new EditUserDto
                {

                    FirstName = SqlManager.GetValue<string>(reader, "FirstName"),
                    LastName = SqlManager.GetValue<string>(reader, "LastName"),
                },
                new Dictionary<string, object>()
                {
                    { "@ID", id }
                }
            )[0];

            if (user != null)
                _EditUserDto = user;

            return Page();

        }

        public IActionResult OnPost(string? id)
        {
            if (id == null) return RedirectToPage("Index");

            _sqlManager.ExecuteNonQuery(
                $"UPDATE ASPNETUSERS SET FIRSTNAME = @FIRSTNAME, LASTNAME = @LASTNAME WHERE ID = @ID;",
                   new Dictionary<string, object>()
                {
                    { "@ID", id},
                    { "@FIRSTNAME", _EditUserDto.FirstName! },
                    { "@LASTNAME", _EditUserDto.LastName! }
                }
            );


            return RedirectToPage("Index");
        }
    }
}
