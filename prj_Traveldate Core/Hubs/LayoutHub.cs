using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;

namespace prj_Traveldate_Core.Hubs
{

    public class LayoutHub:Hub
    {

        public static List<ChatUserInfo> MemberConnection = new List<ChatUserInfo>();

               
        public async Task UpdateUserInfo(string memberID)
        {
            string currentMemberID = memberID;
            if (!string.IsNullOrEmpty(currentMemberID))
            {
                var memberinfo = MemberConnection.Where(p => p.MemberID == currentMemberID).FirstOrDefault();
                if (memberinfo != null) 
                {
                    MemberConnection.Remove(memberinfo);
                }
                TraveldateContext db = new TraveldateContext();
                var CustomerName = db.Members.Where(m => m.MemberId == Convert.ToInt32(memberID)).Select(m => m.FirstName).FirstOrDefault();
                ChatUserInfo x = new ChatUserInfo();
                x.ConnectID = Context.ConnectionId;
                x.MemberID = memberID;
                x.FirstName = CustomerName;
                MemberConnection.Add(x);
                 }
                   
            // 更新連線 ID 列表
            //List<string> list = new List<string>();
            //foreach (ChatUserInfo x in MemberConnection)
            //{
            //    list.Add(x.ConnectID);
            //}
            //string jsonString = JsonConvert.SerializeObject(MemberConnection);
            //await Clients.All.SendAsync("UpdList", jsonString);

            // 更新個人 ID
            //await Clients.Client(Context.ConnectionId).SendAsync("UpdSelfID", Context.ConnectionId);

            // 更新聊天內容
            //await Clients.All.SendAsync("UpdContent", "新連線 ID: " + Context.ConnectionId);
         
        }

        public async Task SelectSupplier(string id)
        {
            var SelectSupplierID = MemberConnection.FirstOrDefault(m => m.MemberID == id);
            if (SelectSupplierID != null)
            {
                var ReceiveName = MemberConnection.Where(c => c.ConnectID == SelectSupplierID.ConnectID).Select(c => c.FirstName).FirstOrDefault();
                var SendName = MemberConnection.Where(c => c.ConnectID == Context.ConnectionId).Select(c => c.FirstName).FirstOrDefault();
                //傳送訊息給主揪，並把傳送者的ConnectionID傳過去
                await Clients.Client(SelectSupplierID.ConnectID).SendAsync("UpdateSupplierID", SendName + "私訊您", Context.ConnectionId, SelectSupplierID.ConnectID);
                //傳送訊息給自己，並把主揪的ConnectionID傳過去
                await Clients.Client(Context.ConnectionId).SendAsync("UpdateCustomerID", "已連絡上"+ ReceiveName, SelectSupplierID.ConnectID, Context.ConnectionId);
            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync("UpdateCustomerID", "此人不再線上", SelectSupplierID.ConnectID, Context.ConnectionId);
            }


        }

        public async Task SendMessage(string myID, string message, string sendToID)
        {
            if (string.IsNullOrEmpty(sendToID))
            {
                await Clients.All.SendAsync("UpdSendContent",   " 說: " + message);
            }
            else
            {
                var ReceiveName = MemberConnection.Where(c=>c.ConnectID== sendToID).Select(c=>c.FirstName).FirstOrDefault();
                var SendName = MemberConnection.Where(c => c.ConnectID == myID).Select(c => c.FirstName).FirstOrDefault();
                // 接收人
                await Clients.Client(sendToID).SendAsync("UpdReceiveContent", SendName + " 私訊向你說: " + message);

                // 發送人
                await Clients.Client(Context.ConnectionId).SendAsync("UpdSendContent", "你向 " + ReceiveName + " 私訊說: " + message);
            }
        }
        //public override async Task OnDisconnectedAsync(Exception ex)
        //{
        //    var memberinfo = MemberConnection.Where(p => p.ConnectID == Context.ConnectionId).FirstOrDefault();
        //    if (memberinfo != null)
        //    {
        //        MemberConnection.Remove(memberinfo);
        //    }
        //}


    }
}
