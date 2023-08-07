using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Global;
using Global.ConfigTemplate;
using Nakama;
using Nakama.TinyJson;
using Newtonsoft.Json;
using Server.Services;

namespace Server {
    public class GlobalMessageListener {
        private readonly NakamaService _nakamaService;
        private readonly GlobalScope _globalScope;
        private readonly AppConfig _appConfig;
        private readonly MessageService _messageService;

        public GlobalMessageListener(NakamaService nakamaService,
                                     GlobalScope globalScope,
                                     AppConfig appConfig,
                                     MessageService messageService) {
            _nakamaService = nakamaService;
            _globalScope = globalScope;
            _appConfig = appConfig;
            _messageService = messageService;
        }
        
        public void Initialize() {
            _nakamaService.SubscribeToMessages(OnMessageListener);
        }

        private async void OnMessageListener(IApiChannelMessage m) {
            var content = m.Content.FromJson<Dictionary<string, string>>();

            var profile = _nakamaService.GetMe();
            
            if (content.TryGetValue("senderUserId", out var senderUserId)) {
                if (profile.User.Id == senderUserId) return;
            }

            if (content.TryGetValue("targetUserId", out var targetUser)) {
                if (profile.User.Id != targetUser) return;
            }
            
            if (content.TryGetValue("declineInviteSended", out var userDeclinedSendedId)) {
                if (!_globalScope.SendedInvites.ContainsKey(userDeclinedSendedId)) {
                    _globalScope.SendedInvites.Remove(userDeclinedSendedId);
                }
            }

            if (content.TryGetValue("declineInviteReceived", out var userDeclinedReceivedId)) {
                if (!_globalScope.ReceivedInvites.ContainsKey(userDeclinedReceivedId)) {
                    _globalScope.ReceivedInvites.Remove(userDeclinedReceivedId);
                }
            }

            await ChekIncomingInvites(content, senderUserId);

            // Check for approved matches
            if (content.TryGetValue("approveMatchInvite", out var matchAndPartyId)) {
                _globalScope.ApprovedMatchAndNeedLoad = true;
                _appConfig.OpponentUserId = senderUserId;
            }

            if (content.TryGetValue("leave", out var leave)) {
                _appConfig.Leave = true;
            }
        }

        private async UniTask ChekIncomingInvites(Dictionary<string, string> content, string senderUserId) {
            // Check for incoming invites
            if (content.TryGetValue("newInvite", out var value)) {
                var inviteData = JsonConvert.DeserializeObject<InviteData>(value);
                if (!_appConfig.InMatch && !_appConfig.InSearch) {
                    _globalScope.ReceivedInvites.Add(senderUserId, inviteData);
                    return;
                }

                await _messageService.SendDeclineInviteSended(inviteData.UserId);
            }
        }
    }
}