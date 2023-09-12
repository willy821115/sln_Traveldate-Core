namespace ECPAYTEST.Models
{
    public class PaymentRequestDto
    {
        public int Amount { get; set; }
        public string Currency { get; set; }
        public string OrderId { get; set; }
        public List<PackageDto> Packages { get; set; }
        public RedirectUrlsDto RedirectUrls { get; set; }
        public RequestOptionDto? Options { get; set; }
    }
    public class PackageDto
    {
        public string Id { get; set; }
        public int Amount { get; set; }
        public string Name { get; set; }
        public List<LinePayProductDto> Products { get; set; }
        public int? UserFee { get; set; }

    }
    public class LinePayProductDto
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public string? Id { get; set; }
        public string? ImageUrl { get; set; }
        public int? OriginalPrice { get; set; }
    }

    public class RedirectUrlsDto
    {
        public string ConfirmUrl { get; set; }
        public string CancelUrl { get; set; }
        public string? AppPackageName { get; set; }
        public string? ConfirmUrlType { get; set; }
    }

    public class RequestOptionDto
    {
        public PaymentOptionDto? Payment { get; set; }
        public DisplpyOptionDto? Displpy { get; set; }
        public ShippingOptionDto? Shipping { get; set; }
        public ExtraOptionsDto? Extra { get; set; }
    }
    public class PaymentOptionDto
    {
        public bool? Capture { get; set; }
        public string? PayType { get; set; }
    }
    public class DisplpyOptionDto
    {
        public string? Local { get; set; }
        public bool? CheckConfirmUrlBrowser { get; set; }
    }
    public class ShippingOptionDto
    {
        public string? Type { get; set; }
        public int FeeAmount { get; set; }
        public string? FeeInquiryUrl { get; set; }
        public string? FeeInquiryType { get; set; }
        public ShippingAddressDto? Address { get; set; }
    }

    public class ShippingAddressDto
    {
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? Detail { get; set; }
        public string? Optional { get; set; }
        public ShippingAddressRecipientDto Recipient { get; set; }
    }

    public class ShippingAddressRecipientDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FirstNameOptional { get; set; }
        public string? LastNameOptional { get; set; }
        public string? Email { get; set; }
        public string? PhoneNo { get; set; }
        public string? Type { get; set; }
    }

    public class ExtraOptionsDto
    {
        public string? BranchName { get; set; }
        public string? BranchId { get; set; }
    }
}
