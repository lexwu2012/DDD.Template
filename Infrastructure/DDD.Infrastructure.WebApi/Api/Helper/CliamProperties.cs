using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Infrastructure.WebApi.Api.Helper
{
    public class CliamProperties
    {
        public string RoleName { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; }

        public List<int> DepartmentIdList { get; set; }
    }
}
