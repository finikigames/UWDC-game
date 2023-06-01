using System.Collections.Generic;
using Global;
using Global.ConfigTemplate;
using Nakama;
using Nakama.TinyJson;
using Server.Services;

namespace Server {
    public class GlobalMessageListener {
        private readonly NakamaService _nakamaService;
        private readonly GlobalScope _globalScope;
        private readonly AppConfig _appConfig;

        public GlobalMessageListener(NakamaService nakamaService,
                                     GlobalScope globalScope,
                                     AppConfig appConfig) {
            _nakamaService = nakamaService;
            _globalScope = globalScope;
            _appConfig = appConfig;
        }
        
        public void Initialize() {
            _nakamaService.SubscribeToMessages(OnMessageListener);
        }

        private void OnMessageListener(IApiChannelMessage m) {
            var content = m.Content.FromJson<Dictionary<string, string>>();
            
            var profile = _nakamaService.GetMe();
            if (content.TryGetValue("targetUserId", out var targetUser)) {
                if (profile.User.Id != targetUser) return;
            }
            
            if (content.TryGetValue("declineInviteSended", out var userDeclinedSendedId)) {
                _globalScope.SendedInvites.Remove(userDeclinedSendedId);
            }

            if (content.TryGetValue("declineInviteReceived", out var userDeclinedReceivedId)) {
                _globalScope.ReceivedInvites.Remove(userDeclinedReceivedId);
            }
        }
    }
}