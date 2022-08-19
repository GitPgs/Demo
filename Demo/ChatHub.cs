using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Demo.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;


namespace Demo
{
    public class ChatHub : Hub
    {
        DBEntities db = new DBEntities();
        private static List<object> list = new List<object>();
        private static List<object> unduplicated = new List<object>();

        public void SendText(string connectionid, string name, string text)
        {
            var h = new History
            {
                message = text,
                status = "unread",
                date = DateTime.Today,
                sender = name,
                receiver = connectionid
            };
            db.Histories.Add(h);
            db.SaveChanges();

            //  list.Add(new {name,connectionid, text});
            //  while (list.Count > 10) list.RemoveAt(0);
            //unduplicated = list.Distinct().ToList();
            //  Clients.User(connectionid).Initialize(unduplicated);



            // Clients.All.ReceiveText(name, text);
            Clients.Caller.ReceiveText(connectionid, name, text, "me");
            Clients.User(connectionid).ReceiveText(connectionid, name, text);


            //Clients.Others.ReceiveText(connectionid, text);
            //Clients.User(connectionid).ReceiveText(name, text,connectionid);
            //Clients.All.addNewMessageToPage(name, text, connectionid);


        }

        //public void SendText(string sender, string receiver, string message,string senderName)
        //{



        //    //if (receiver == "public")
        //    //{
        //    //    await Clients.Caller.SendAsync("ReceivePublicText", sender, receiver, message, "caller", "");
        //    //    await Clients.Others.SendAsync("ReceivePublicText", sender, receiver, message, "others", "public");
        //    //    messageHistory.Add(new Message(senderId, "public", message, "text", "public", sender));
        //    //}

        //      Clients.Client(senderName).SendAsync("ReceiveText", sender, receiver, message, "caller", "", false);
        //        Clients.Client(receiver).SendAsync("ReceiveText", sender, receiver, message, "others", senderName, false);
        //        //messageHistory.Add(new Message(senderId, receiver, message, "text", "private", sender));


        //}

        public override Task OnConnected()
        {
            //Clients.Caller.Initialize(list);
            return base.OnConnected();
        }

        public void GetHistory(string connectionid, string name)
        {
           
            if (name == "")
            {
           var his1 = db.Histories.Where(s => s.sender == connectionid || s.receiver == connectionid);

                foreach (var item in his1)
                {
                    if (item.sender.StartsWith("S"))
                    {
                        //Clients.User(connectionid).ReceiveHistory(connectionid, name,item.message, "owner");
                        Clients.Caller.ReceiveHistory(item.receiver, item.sender, item.message, "student");
                    }
                    else
                    {
                        //Clients.User(name).ReceiveHistory(connectionid, name, item.message, "student");
                        Clients.Caller.ReceiveHistory(item.receiver, item.sender, item.message, "owner");
                    }
                }
            }
      var his = db.Histories.Where(s => s.sender == name && s.receiver == connectionid || s.sender == connectionid && s.receiver == name);
            foreach (var item in his)
            {
                if (item.sender.StartsWith("S"))
                {
                    //Clients.User(connectionid).ReceiveHistory(connectionid, name,item.message, "owner");
                    Clients.Caller.ReceiveHistory(item.receiver, item.sender, item.message, "student","his");
                }
                else
                {
                    //Clients.User(name).ReceiveHistory(connectionid, name, item.message, "student");
                    Clients.Caller.ReceiveHistory(item.receiver, item.sender, item.message, "owner","his");
                }
            }
        }






        public void GetAllHistory(string connectionid)
        {


            var his = db.Histories.Where(s => s.receiver == connectionid || s.sender == connectionid);
            foreach (var item in his)
            {

                if (item.sender.StartsWith("S"))
                {
                    //Clients.User(connectionid).ReceiveHistory(connectionid, name,item.message, "owner");
                    Clients.Caller.ReceiveAllHistory(item.receiver, item.sender, item.message, "student");
                }
                else
                {
                    //Clients.User(name).ReceiveHistory(connectionid, name, item.message, "student");
                    Clients.Caller.ReceiveAllHistory(item.receiver, item.sender, item.message, "owner");
                }
            }





        }
    }
}