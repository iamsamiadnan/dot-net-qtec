using dot_net_qtec.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace dot_net_qtec.Pages.Accountant.Accounts
{
    [Authorize(Roles = "Accountant")]
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

        public class ParentAccountDto
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }
        public List<ParentAccountDto> _ParentAccounts { get; set; } = new List<ParentAccountDto>();

        [BindProperty]
        public CreateAccountDto _CreateAccountDto { get; set; } = new CreateAccountDto();
        public void OnGet()
        {
            // If ParentId is NULL then it is parent category
            _ParentAccounts = _sqlManager.ExecuteReader<ParentAccountDto>($@"
                SELECT ID, NAME FROM CHARTOFACCOUNTS WHERE PARENTID IS NULL;",
                reader => new ParentAccountDto
                {
                    Id = SqlManager.GetValue<int>(reader, "Id"),
                    Name = SqlManager.GetValue<string>(reader, "Name"),
                }
            );
        }

        public IActionResult OnPost()
        {
            _sqlManager.ExecuteNonQuery($@"sp_ManageChartOfAccounts",
             new Dictionary<string, object>()
                {
                    { "@ACTION", "CREATE" },
                    { "@NAME", _CreateAccountDto.Name!},
                    { "@PARENTID", (_CreateAccountDto.ParentId.ToString() == "#" ? null : _CreateAccountDto.ParentId)!}
                },
                true
            );

            return RedirectToPage("Index");
        }
    }
}
