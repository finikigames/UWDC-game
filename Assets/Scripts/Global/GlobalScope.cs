using System.Collections.Generic;

namespace Global {
    public class GlobalScope {
        public Dictionary<string, InviteData> SendedInvites = new Dictionary<string, InviteData>();
        public Dictionary<string, InviteData> ReceivedInvites = new Dictionary<string, InviteData>();

        public bool ApprovedMatchAndNeedLoad;
        public string ApproveSenderId;
    }
}