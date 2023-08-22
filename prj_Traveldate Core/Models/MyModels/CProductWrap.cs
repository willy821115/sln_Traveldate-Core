namespace prj_Traveldate_Core.Models.MyModels
{
    public class CProductWrap
    {
        private ProductList _prod = null;
        private Company _company = null;

        public CProductWrap()
        {
            _prod = new ProductList();
            _company = new Company();
        }
        public ProductList ProductList { get { return _prod; } set { _prod = value; } }
        public Company Company { get { return _company; } set { _company = value; } }

        public string? CompanyName
        {
            get { return _company.CompanyName; }
            set { _company.CompanyName = value; }
        }
        public int? ProductId
        {
            get { return _prod.ProductId; }
            set { _prod.ProductId = (int)value; }
        }

        public int CompanyId
        {
            get { return _prod.CompanyId; }
            set { _prod.CompanyId = value; }
        }

        public string? ProductName
        {
            get { return _prod.ProductName; }
            set { _prod.ProductName = value; }
        }

        public int? CityId
        {
            get { return _prod.CityId; }
            set { _prod.CityId = value; }
        }

        public string? Description
        {
            get { return _prod.Description; }
            set { _prod.Description = value; }
        }

        public int? ProductTypeId
        {
            get { return _prod.ProductTypeId; }
            set { _prod.ProductTypeId = value; }
        }

        public int? StatusId
        {
            get { return _prod.StatusId; }
            set { _prod.StatusId = value; }
        }

        public string? PlanName
        {
            get { return _prod.PlanName; }
            set { _prod.PlanName = value; }
        }

        public string? PlanDescription
        {
            get { return _prod.PlanDescription; }
            set { _prod.PlanDescription = value; }
        }

        public bool? Discontinued
        {
            get { return _prod.Discontinued; }
            set { _prod.Discontinued = value; }
        }

        public string? Outline
        {
            get { return _prod.Outline; }
            set { _prod.Outline = value; }
        }

        public string? OutlineForSearch
        {
            get { return _prod.OutlineForSearch; }
            set { _prod.OutlineForSearch = value; }
        }

        public string? Address
        {
            get { return _prod.Address; }
            set { _prod.Address = value; }
        }

        public string? cityName { get; set; }

        public string? productType { get; set; }

        public string? productStatus { get; set;}

        public string? ImagePath { get; set; }

        public List<CCategoryAndTags> categoryAndTags { get; set;}

        public  List<string>? countries { get; set; }

        public List<CityList>? cities { get; set; }

        public List<ProductTypeList>? types { get; set; }

        public List<TripDetail>? CtripDetail { get; set; }
        public List<int?>? Tags { get; set; }

        public List< IFormFile> photos { get; set; }

        public List<ProductPhotoList> productPhotos { get; set; }


        public List<TripDetailText>triptest { get; set; }


}
}
