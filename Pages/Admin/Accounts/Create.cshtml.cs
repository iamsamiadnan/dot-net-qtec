using dot_net_qtec.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace dot_net_qtec.Pages.Admin.Accounts
{
    public class CreateModel : PageModel
    {

        private readonly SqlManager _sqlManager;
        public CreateModel(SqlManager sqlManager)
        {
            _sqlManager = sqlManager;
        }

        public class CreateAccountDto
        {
            public string? Name { get; set; }
            public int? ParentId { get; set; }
        }

        public class ParentAccount
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }
        public List<ParentAccount> _ParentAccounts { get; set; } = new List<ParentAccount>();

        [BindProperty]
        public CreateAccountDto _CreateAccountDto { get; set; } = new CreateAccountDto();
        public void OnGet()
        {
            _ParentAccounts = _sqlManager.ExecuteReader<ParentAccount>($@"
                SELECT ID, NAME FROM CHARTOFACCOUNTS WHERE PARENTID IS NULL;",
                reader => new ParentAccount
                {
                    Id = SqlManager.GetValue<int>(reader, "Id"),
                    Name = SqlManager.GetValue<string>(reader, "Name"),
                }
            );
        }

        public IActionResult OnPost()
        {


            return RedirectToPage("Create");
        }
    }
}
