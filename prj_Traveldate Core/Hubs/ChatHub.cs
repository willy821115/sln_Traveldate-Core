using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using prj_Traveldate_Core.Controllers;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using System.Threading.Tasks;
using System.Linq;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;

namespace prj_Traveldate_Core.Hubs
{
    public class ChatHub : Hub
    {
        private  IHttpContextAccessor _httpContextAccessor;

        public  List<string> ConnIDList = new List<string>();
        private  Dictionary<string, string> _userConnectionMap = new Dictionary<string, string>();

        /// <summary>
        /// 連線事件
        /// </summary>
        /// <returns></returns>
        /// 

        public override async Task OnConnectedAsync()
        {
            Random RAM = new Random();
            int min = 11;
            int max = 5999;
            string ConnectionID = RAM.Next(min, max + 1).ToString();

            if (ConnIDList.Where(p => p == Context.ConnectionId).FirstOrDefault() == null)
            {
                ConnIDList.Add(ConnectionID);
            }
            Context.Items["ConnectionID"] = ConnectionID;
            if (ConnIDList.Contains(ConnectionID) == false)
            {
                ConnIDList.Add(ConnectionID);
            }
            // 更新連線 ID 列表
            string jsonString = JsonConvert.SerializeObject(ConnIDList);
            await Clients.All.SendAsync("UpdList", jsonString);

            // 更新個人 ID
            await Clients.Client(Context.ConnectionId).SendAsync("UpdSelfID", ConnectionID);

            // 更新聊天內容
            await Clients.All.SendAsync("UpdContent", "旅人 "+ ConnectionID + " 進來了~ 來聊聊吧 "); /*+ Context.ConnectionId*/

            await base.OnConnectedAsync();
        }
        /// <summary>
        /// 離線事件
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception ex)
        {
            //string id = ConnIDList.Where(p => p == Context.ConnectionId).FirstOrDefault();
            //if (id != null)
            //{
            //    ConnIDList.Remove(id);
            //}
            string ConnectionID = Context.Items["ConnectionID"] as string;
            if(!string.IsNullOrEmpty(ConnectionID))
            {
                ConnIDList.Remove(ConnectionID);    
            }
            // 更新連線 ID 列表
            string jsonString = JsonConvert.SerializeObject(ConnIDList);
            await Clients.All.SendAsync("UpdList", jsonString);

            // 更新聊天內容
            await Clients.All.SendAsync("UpdContent", "旅人 " + ConnectionID + " 已離線");

            await base.OnDisconnectedAsync(ex);
        }
        /// <summary>
        /// 傳遞訊息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task SendMessage( string message, string sendToID)
        {
            string ConnectionID = Context.Items["ConnectionID"] as string;
            if (!string.IsNullOrEmpty(ConnectionID))
            {
                if (string.IsNullOrEmpty(sendToID))
                {
                    await Clients.All.SendAsync("UpdContent", "旅人 " + ConnectionID + " 說：" + message + "    " + DateTime.Now);
                }
                else
                {
                    // 如果有接收人 ID，则发送私信
                    await Clients.Client(sendToID).SendAsync("UpdContent", ConnectionID + " 私訊向你說: " + message + " " + DateTime.Now);

                    // 同时向自己发送一份私信的副本
                    await Clients.Client(ConnectionID).SendAsync("UpdContent", "你向 " + sendToID + " 私訊說: " + message + " " + DateTime.Now);
                }
            }
        }
    }
}

