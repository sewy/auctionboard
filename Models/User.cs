using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
namespace auctionboard.Models
{
public abstract class BaseEntity {}
 public class User : BaseEntity
 {
    public int id { get; set; }
    [Required]
    [MinLength(4)]
    [MaxLength(19)]
    public string username { get; set; }
    [MinLength(3)]
    [Required]
    public string first_name { get; set; }
    [MinLength(2)]
    [Required]
    public string last_name { get; set; }
    [MinLength(8)]
    [RegularExpressionAttribute("^(?=.*[A-Za-z])(?=.*\\d)(?=.*[$@$!%*#?&])[A-Za-z\\d$@$!%*#?&]{8,}$", ErrorMessage="PassWord must be at least 8 characters long, contain a special character and at least one number.")]
    [Required]
    public string password { get; set; } 
    [Compare("password")]
    public string c_password { get; set; }
    public decimal money { get; set; }
 }

 public class Auction : BaseEntity
 {
     public int id { get; set; }
     public string first_name { get; set; }
     public string last_name { get; set; }
     
     public int user_id { get; set; }
     public int bidder_id { get; set; }
     [Required]
     [MinLength(4)]
     public string name { get; set; }
     [Required]
     [MinLength(10)]
     public string description { get; set; }
     [Range(1.00, 1000.00)]
     public double bid { get; set; }
     
     public DateTime end_date { get; set; }

     public string converted { get; set; }
     public decimal money { get; set; }
 }

}