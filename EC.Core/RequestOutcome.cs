using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Core
{
    public class RequestOutcome<T>
    {
        public T Data { get; set; }
        public string RedirectUrl { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
        public string WarningMessage { get; set; }
    }
}
