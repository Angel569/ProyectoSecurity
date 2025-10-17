using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy.Security.Domain.Dpo
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string[] Errors { get; set; }

        private OperationResult(bool success, IEnumerable<string>? errors = null)
        {
            Success = success;
            Errors = errors?.ToArray() ?? Array.Empty<string>();
        }
        public static OperationResult Ok() => new(true);
        public static OperationResult Fail(IEnumerable<string> errors) => new(false, errors);
    }
}
