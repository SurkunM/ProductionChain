using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductionChain.Contracts.Dto.Requests;

public class EmployeeStateRequest
{
    public int Id { get; set; }

    public int StatusType { get; set; }
}
