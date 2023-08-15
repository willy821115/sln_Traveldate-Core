﻿using prj_Traveldate_Core.Models;

namespace prj_Traveldate_Core.ViewModels
{
    public class CcommentListViewModel
    {
        private Member _member = null;
        private CommentList _commentList = null;
        private ProductList _productList = null;

        public Member member
        {
            get { return _member; }
            set { _member = value; }
        }
        public CommentList commentList
        {
            get { return _commentList; }
            set { _commentList = value; }
        }
        public ProductList productList
        {
            get { return _productList; }
            set { _productList = value; }
        }
        public CcommentListViewModel()
        {
            _member = new Member();
            _commentList = new CommentList();
            _productList = new ProductList();
        }


        public string? Title
        {
            get { return _commentList.Title; }
            set { _commentList.Title = value; }
        }
        public string? Content
        {
            get { return _commentList.Content; }
            set { _commentList.Content = value; }
        }
        public int? CommentScore
        {
            get { return _commentList.CommentScore; }
            set { _commentList.CommentScore = value; }
        }
        public DateTime? Date
        {
            get { return _commentList.Date; }
            set { _commentList.Date = value; }
        }
        public string? ProductName
        {
            get { return _productList.ProductName; }
            set { _productList.ProductName = value; }
        }
        public int ProductId
        {
            get { return _productList.ProductId; }
            set { _productList.ProductId = value; }
        }
    }
}
