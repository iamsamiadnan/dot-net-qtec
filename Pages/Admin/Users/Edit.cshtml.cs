using dot_net_qtec.Models;
using dot_net_qtec.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;


namespace dot_net_qtec.Pages.Admin.Users
{
    [Authorize(Roles = "admin")]
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
            public string? RoleId { get; set; }
            public string? RoleName { get; set; }
        }

        public class RoleDto
        {
            public string? Id { get; set; }
            public string? Name { get; set; }
        }

        public List<RoleDto> _Roles { get; set; } = new List<RoleDto>();

        [BindProperty]
        public EditUserDto _EditUserDto { get; set; } = new EditUserDto();

        // public List<string> _RoleList { get; set; }
        public IActionResult OnGet(string? id)
        {
            if (id == null) return RedirectToPage("Index");

            _Roles = _sqlManager.ExecuteReader<RoleDto>($@"
                SELECT ID, NAME FROM ASPNETROLES;",
                reader => new RoleDto
                {
                    Id = SqlManager.GetValue<string>(reader, "Id"),
                    Name = SqlManager.GetValue<string>(reader, "Name")
                }
            );

            var user = _sqlManager.ExecuteReader<EditUserDto>($@"
                SELECT U.ID, U.FIRSTNAME, U.LASTNAME, U.EMAIL, U.USERNAME, R.NAME AS ROLENAME, R.ID AS ROLEID
                FROM ASPNETUSERS U 
                LEFT JOIN ASPNETUSERROLES UR ON U.ID = UR.USERID 
                LEFT JOIN ASPNETROLES R ON UR.ROLEID = R.ID
                WHERE U.ID = @ID;",
                reader => new EditUserDto
                {

                    FirstName = SqlManager.GetValue<string>(reader, "FirstName"),
                    LastName = SqlManager.GetValue<string>(reader, "LastName"),
                    RoleId = SqlManager.GetValue<string>(reader, "RoleId"),
                    RoleName = SqlManager.GetValue<string>(reader, "RoleName")
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


            List<SqlCommand> sqlCommands = new List<SqlCommand>()
            {
                _sqlManager.CreateCommand($@"
                    UPDATE ASPNETUSERS 
                    SET FIRSTNAME = @FIRSTNAME, LASTNAME = @LASTNAME 
                    WHERE ID = @ID;",
                    new Dictionary<string, object>()
                    {
                        { "@ID", id},
                        { "@FIRSTNAME", _EditUserDto.FirstName! },
                        { "@LASTNAME", _EditUserDto.LastName! }
                    }
                )
            ,
                _sqlManager.CreateCommand($@"
                    UPDATE ASPNETUSERROLES 
                    SET ROLEID = @ROLEID 
                    WHERE USERID = @ID;",
                    new Dictionary<string, object>()
                    {

                        { "@ROLEID", _EditUserDto.RoleId! },
                        { "@ID", id }
                    }
                )
            };

            _sqlManager.ExecuteTransaction(sqlCommands);

            return RedirectToPage("Index");
        }


    }
}
