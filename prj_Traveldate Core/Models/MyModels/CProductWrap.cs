namespace prj_Traveldate_Core.Models.MyModels
{
    public class CProductWrap
    {
        private ProductList _prod = null;

        public CProductWrap()
        {
            _prod = new ProductList();
        }
        public ProductList ProductList { get { return _prod; } set { _prod = value; } }

        public int ProductId
        {
            get { return _prod.ProductId; }
            set { _prod.ProductId = value; }
        }

        public int CompanyId
        {
            get { return _prod.ProductId; }
            set { _prod.ProductId = value; }
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

        //public string? Place
        //{
        //    get { return _prod.Place; }
        //    set { _prod.Place = value; }
        //}
        public DateTime date { get; set; }

        public string orderMember { get; set; }

        public int stock { get; set; }
    }
}
