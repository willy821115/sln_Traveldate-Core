using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace prj_Traveldate_Core.Models;

public partial class Company
{
    public int CompanyId { get; set; }

    [DisplayName("公司統一編號")]
    [Required(ErrorMessage = "{0}為必填")]
    public string? TaxIdNumber { get; set; }

    [DisplayName("法定公司名稱")]
    [Required(ErrorMessage = "{0}為必填")]
    public string? CompanyName { get; set; }

    [DisplayName("國家")]
    public string? Country { get; set; }

    [DisplayName("縣市")]
    public string? City { get; set; }

    [DisplayName("郵遞區號")]
    [Required(ErrorMessage = "{0}為必填")]
    public string? PostalCode { get; set; }

    [DisplayName("公司地址")]
    [Required(ErrorMessage = "{0}為必填")]
    public string? Address { get; set; }

    [DisplayName("聯絡電話")]
    [Required(ErrorMessage = "{0}為必填")]
    public string? Phone { get; set; }

    [DisplayName("官方網站")]
    public string? Url { get; set; }

    [DisplayName("負責人姓名")]
    [Required(ErrorMessage = "{0}為必填")]
    public string? Principal { get; set; }

    [DisplayName("聯絡人姓名")]
    [Required(ErrorMessage = "{0}為必填")]
    public string? Contact { get; set; }

    [DisplayName("聯絡人職稱")]
    public string? Title { get; set; }

    [DisplayName("聯絡E-mail")]
    [Required(ErrorMessage = "{0}為必填")]
    [EmailAddress(ErrorMessage = "請填寫正確E-mail")]
    public string? Email { get; set; }

    [DisplayName("密碼")]
    [Required(ErrorMessage = "{0}為必填")]
    public string? Password { get; set; }

    [DisplayName("服務描述")]
    [Required(ErrorMessage = "{0}為必填")]
    public string? ServerDescription { get; set; }

    public bool? Enable { get; set; }

    public virtual ICollection<ProductList> ProductLists { get; set; } = new List<ProductList>();
}
