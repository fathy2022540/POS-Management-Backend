using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRM.Application.Common.DTOs.ParametersConfiguration
{
    public class paramterConfigRespone
    {
        public long Id { get; set; } 
        public string Keyword { get; set; }
        public string? Parent { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? ContentEn { get; set; }
        public string? ContentAr { get; set; }
        public string? URL { get; set; }
    }
}
