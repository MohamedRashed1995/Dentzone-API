using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Domain.DTO
{
    public class InvoiceDto
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string InvoiceType { get; set; }
        public Guid OrderId { get; set; }
        public Guid PharmacyUserId { get; set; }
        public string PharmacyName { get; set; }
        public decimal CreditUsed { get; set; }
        public decimal CashPaid { get; set; }
        public string PaymentMethod { get; set; }
    }
}
