using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.OptionsConfig
{
    public class JwtOptions
    {        
        public string JwtIssuer { get; set; } = string.Empty;
        public string JwtAudience { get; set; } = string.Empty;
        public string JwtSecret { get; set; } = string.Empty;
        
    }
}
