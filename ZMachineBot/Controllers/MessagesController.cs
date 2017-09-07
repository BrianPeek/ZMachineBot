using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using Microsoft.Bot.Connector;
using ZMachineLib;

namespace ZMachineBot.Controllers
{
	[BotAuthentication]
	public class MessagesController : ApiController
	{
		/// <summary>
		/// POST: api/Messages
		/// Receive a message from a user and reply to it
		/// </summary>
		public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
		{
			switch(activity.Text)
			{
				case "/reset":
					activity.GetStateClient().BotState.DeleteStateForUser(activity.ChannelId, activity.From.Id);
					break;
			}

			BotIO io = new BotIO();
			ZMachine zMachine = new ZMachine(io);

			string path = HostingEnvironment.MapPath("~/Games/cutthroa.dat");
			FileStream fs = File.OpenRead(path);
			zMachine.LoadFile(fs);

			BotData data = activity.GetStateClient().BotState.GetUserData(activity.ChannelId, activity.From.Id);
			byte[] state = data.GetProperty<byte[]>("ZState");
			try
			{
				if(state != null)
				{
					using (MemoryStream ms = new MemoryStream(state))
						zMachine.RestoreState(ms);
					zMachine.FinishRead(activity.Text);
				}
			}
			catch(Exception)
			{
				activity.GetStateClient().BotState.DeleteStateForUser(activity.ChannelId, activity.From.Id);
			}

			zMachine.Run(true);
			Stream s = zMachine.SaveState();
			using (MemoryStream ms = new MemoryStream())
			{
				s.CopyTo(ms);
				data.SetProperty("ZState", ms.ToArray());
			}

			activity.GetStateClient().BotState.SetUserData(activity.ChannelId, activity.From.Id, data);

			if (activity.Type == ActivityTypes.Message)
			{
				ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

				// return our reply to the user
				Activity reply = activity.CreateReply(io.Text);
				await connector.Conversations.ReplyToActivityAsync(reply);
			}
			else
			{
				HandleSystemMessage(activity);
			}
			var response = Request.CreateResponse(HttpStatusCode.OK);
			return response;
		}

		private Activity HandleSystemMessage(Activity activity)
		{
			if (activity.Type == ActivityTypes.DeleteUserData)
			{
				// Implement user deletion here
				// If we handle user deletion, return a real message
				activity.GetStateClient().BotState.DeleteStateForUser(activity.ChannelId, activity.From.Id);
			}
			else if (activity.Type == ActivityTypes.ConversationUpdate)
			{
				// Handle conversation state changes, like members being added and removed
				// Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
				// Not available in all channels
			}
			else if (activity.Type == ActivityTypes.ContactRelationUpdate)
			{
				// Handle add/remove from contact lists
				// Activity.From + Activity.Action represent what happened
			}
			else if (activity.Type == ActivityTypes.Typing)
			{
				// Handle knowing tha the user is typing
			}
			else if (activity.Type == ActivityTypes.Ping)
			{
			}

			return null;
		}
	}
}