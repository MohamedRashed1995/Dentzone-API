using System;
using System.Collections.Generic;

namespace Dragza.Domain.Models;

public partial class PharmacyDetail
{
    public Guid Id { get; set; }

    public string? ArabicName { get; set; }

    public string? EnglishName { get; set; }

    public Guid? PurchasingManager { get; set; }

    public string? DemoAgentCode { get; set; }

    public string? PhoneNumber { get; set; }

    public string? LandLineNumber { get; set; }

    public string? CommercialRegisteryNumber { get; set; }

    public string? CommercialRegisteryAttach { get; set; }

    public string? NationalId { get; set; }

    public string? NationalIdAttach { get; set; }

    public string? PharmacyLicenseNo { get; set; }

    public string? PharmacyLicenseAttach { get; set; }

    public string? OwnersgipAttach { get; set; }

    public string? TaxationCardNo { get; set; }

    public string? TaxationCardAttach { get; set; }

    public Guid? UserId { get; set; }

    public virtual User? PurchasingManagerNavigation { get; set; }

    public virtual User? User { get; set; }
}
