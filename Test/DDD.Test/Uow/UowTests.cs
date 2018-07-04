using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DDD.Test.Uow
{
    public class UowTests : SampleDbContext
    {


        [Fact]
        public void Should_Rollback_If_Uow_Is_Not_Completed()
        {

        }
    }
}
