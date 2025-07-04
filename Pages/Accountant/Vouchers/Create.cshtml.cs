using System.Data;
using System.Text.Json;
using dot_net_qtec.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace dot_net_qtec.Pages.Accountant.Vouchers
{
    public class CreateModel : PageModel
    {


        private readonly SqlManager _sqlManager;
        public CreateModel(SqlManager sqlManager)
        {
            _sqlManager = sqlManager;
        }


        public class CreateVoucherDto
        {
            public string? Category { get; set; }
            public DateTime Date { get; set; } = DateTime.Now;
            public string? Ref { get; set; }
            public string? Entries { get; set; }
        }

        public class CreateEntryDto
        {
            public int AccountId { get; set; }
            public double Debit { get; set; }
            public double Credit { get; set; }
        }

        [BindProperty]
        public CreateVoucherDto _CreateVoucherDto { get; set; } = new CreateVoucherDto();
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            var t = JsonSerializer.Deserialize<Dictionary<string, CreateEntryDto>>(_CreateVoucherDto.Entries!);
            List<CreateEntryDto> entryList = t!.Values.ToList();

            var entriesTable = new DataTable();
            entriesTable.Columns.Add("AccountId", typeof(int));
            entriesTable.Columns.Add("Debit", typeof(decimal));
            entriesTable.Columns.Add("Credit", typeof(decimal));

            foreach (var entry in entryList)
            {
                entriesTable.Rows.Add(entry.AccountId, entry.Debit, entry.Credit);
            }

            _sqlManager.ExecuteNonQuery($@"sp_SaveVoucher",
            new Dictionary<string, object>()
               {
                    { "@CATEGORY", _CreateVoucherDto.Category! },
                    { "@DATE", _CreateVoucherDto.Date!},
                    { "@REF", _CreateVoucherDto.Ref!},
                    { "@ENTRIES", entriesTable},
               },
               true
           );

            return RedirectToPage("Create");

        }
    }
}
