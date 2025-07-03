using dot_net_qtec.Models;
using dot_net_qtec.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace dot_net_qtec.Pages.Admin.Users
{
    public class IndexModel : PageModel
    {
        private readonly SqlManager _sqlManager;

        public IndexModel(SqlManager sqlManager)
        {
            _sqlManager = sqlManager;
        }
        public class UserDto
        {
            public string? Id { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? Email { get; set; }
            public string? UserName { get; set; }
            public string? RoleName { get; set; }
        }

        public List<UserDto> Users { get; set; } = new List<UserDto>();
        public void OnGet()
        {
            Users = _sqlManager.ExecuteReader<UserDto>($@"
                SELECT U.ID, U.FIRSTNAME, U.LASTNAME, U.EMAIL, U.USERNAME, R.NAME AS ROLENAME 
                FROM ASPNETUSERS U 
                LEFT JOIN ASPNETUSERROLES UR ON U.ID = UR.USERID 
                LEFT JOIN ASPNETROLES R ON UR.ROLEID = R.ID;",
                reader => new UserDto
                {
                    Id = SqlManager.GetValue<string>(reader, "Id"),
                    FirstName = SqlManager.GetValue<string>(reader, "FirstName"),
                    LastName = SqlManager.GetValue<string>(reader, "LastName"),
                    Email = SqlManager.GetValue<string>(reader, "Email"),
                    UserName = SqlManager.GetValue<string>(reader, "UserName"),
                    RoleName = SqlManager.GetValue<string>(reader, "RoleName")
                }
            );


            Console.WriteLine("EOF");
        }


    }
}
