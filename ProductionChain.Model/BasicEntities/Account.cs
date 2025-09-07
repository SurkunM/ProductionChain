using Microsoft.AspNetCore.Identity;

namespace ProductionChain.Model.BasicEntities;

public class Account : IdentityUser<int>
{
    public int EmployeeId { get; set; }

    public required virtual Employee Employee { get; set; }
}
