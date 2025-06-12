using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class PharmacyDetailsDto
    {
        public string? ArabicName { get; set; }
        public string? EnglishName { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile? CommercialRegisteryAttach { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile? NationalIdAttach { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile? PharmacyLicenseAttach { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile? OwnersgipAttach { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile? TaxationCardAttach { get; set; }

        public string? DemoAgentCode { get; set; }

        public string? PhoneNumber { get; set; }

        public string? LandLineNumber { get; set; }

        public string? CommercialRegisteryNumber { get; set; }
        public string? NationalId { get; set; }

        public string? PharmacyLicenseNo { get; set; }

        public string? TaxationCardNo { get; set; }

        public string? CommercialRegisteryAttachPath { get; set; }
        public string? NationalIdAttachPath { get; set; }
        public string? PharmacyLicenseAttachPath { get; set; }
        public string? OwnersgipAttachPath { get; set; }
        public string? TaxationCardAttachPath { get; set; }
    }
}
