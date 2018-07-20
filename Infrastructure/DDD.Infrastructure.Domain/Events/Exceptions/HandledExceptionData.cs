using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Infrastructure.Domain.Events.Exceptions
{
    public class HandledExceptionData : ExceptionData
    {
       
        public HandledExceptionData(Exception exception)
            : base(exception)
        {

        }
    }
}
