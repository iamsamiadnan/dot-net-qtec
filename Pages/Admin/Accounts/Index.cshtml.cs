using System.Text.Json;
using dot_net_qtec.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace dot_net_qtec.Pages.Admin.Accounts
{
    public class IndexModel : PageModel
    {

        private readonly SqlManager _sqlManager;
        public IndexModel(SqlManager sqlManager)
        {
            _sqlManager = sqlManager;
        }

        public class AccountDto
        {
            public string? id { get; set; }
            public string? text { get; set; }
            public string? parent { get; set; }

        }
        public List<AccountDto> _Accounts { get; set; } = new List<AccountDto>();

        public string _RenderTreeJSON { get; set; } = String.Empty;
        public void OnGet()
        {

            _Accounts = _sqlManager.ExecuteReader<AccountDto>($@"
                SELECT ID, NAME, ParentId FROM CHARTOFACCOUNTS;",
               reader =>
               {
                   string ParentId = SqlManager.GetValue<int>(reader, "ParentId").ToString();
                   return new AccountDto
                   {
                       id = SqlManager.GetValue<int>(reader, "Id").ToString(),
                       text = SqlManager.GetValue<string>(reader, "Name"),
                       parent = ParentId == "0" ? "#" : SqlManager.GetValue<int>(reader, "ParentId").ToString()
                   };
               }
            );


            _RenderTreeJSON = JsonSerializer.Serialize(_Accounts);

        }
    }
}
