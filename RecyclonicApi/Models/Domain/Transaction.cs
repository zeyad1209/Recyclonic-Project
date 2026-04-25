using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RecyclonicApi.Models;

public class Transaction
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; } 
    public string PaymentMethod { get; set; } = "Cash";
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime DeliveredAt {  get; set; }
    public Guid OrderId { get; set; } 
    public Order order { get; set; }
    public Delivery delivery { get; set; }
}
