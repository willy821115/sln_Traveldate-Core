using prj_Traveldate_Core.Models;
using System.Collections.Generic;

namespace prj_Traveldate_Core.Controllers
{
    //金使用的 2023.09.04
    internal class CForumListViewModel2
    {
        internal bool? isLike;
        private ForumList _forumList = null;
        private LikeList _likeList = null;

        public ForumList forumList
        {
            get { return _forumList; }
            set { _forumList = value; }
        }
        public LikeList likeList
        {
            get { return _likeList; }
            set { _likeList = value; }
        }

        public int ForumListId { get; set; }
        public string Title { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ReleaseDatetime { get; set; }
        public int? Likes { get; set; }
        public int? Watches { get; set; }
        public string? Content { get; set; }
        public bool? IsPublish { get; internal set; }

        //Models- LikeList
        public int LikeId { get; set; }
        public int? MemberId { get; set; }
        public int? ForumId { get; set; }
        public int? IsLike { get; set; }
        public virtual ForumList? Forum { get; set; }
        public virtual Member? Member { get; set; }
    }
}