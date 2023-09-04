using prj_Traveldate_Core.Models;

namespace prj_Traveldate_Core.ViewModels
{
    public class CforumListViewModel2
    {
     private Member _member = null;
     private ForumList _forumList = null;
    
    public ForumList forumList
    {
        get { return _forumList; }
        set { _forumList = value; }
    }
    public Member member
    {
        get { return _member; }
        set { _member = value; }
    }

    public CforumListViewModel2()
    {
        _member = new Member();
        _forumList = new ForumList();
    }


    public int ForumListId
    {
        get { return forumList.ForumListId; }
        set { forumList.ForumListId = value; }
    }
    public string? Title
    {
        get { return forumList.Title; }
        set { forumList.Title = value; }
    }
    public DateTime? DueDate
    {
        get { return forumList.DueDate; }
        set { forumList.DueDate = value; }
    }
    public DateTime? ReleaseDatetime
    {
        get { return forumList.ReleaseDatetime; }
        set { forumList.ReleaseDatetime = value; }
    }
        public int? Likes
        {
            get { return forumList.Likes; }
            set { forumList.Likes = value; }
        }
        public int? Watches
    {
        get { return forumList.Watches; }
        set { forumList.Watches = value; }
    }
        public bool? IsPublish { get; set; }
    }
}
