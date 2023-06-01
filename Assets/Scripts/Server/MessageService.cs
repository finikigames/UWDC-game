using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Global;
using Global.ConfigTemplate;
using Nakama;
using Nakama.TinyJson;
using Server.Services;
using Zenject;

namespace Server {
    public class MessageService {
        private readonly NakamaService _nakamaService;
        private readonly AppConfig _appConfig;
        private readonly SignalBus _signalBus;
        private readonly GlobalScope _globalScope;
        private IChannel _globalChannel;

        public MessageService(NakamaService nakamaService,
                              AppConfig appConfig,
                              SignalBus signalBus,
                              GlobalScope globalScope) {
            _nakamaService = nakamaService;
            _appConfig = appConfig;
            _signalBus = signalBus;
            _globalScope = globalScope;
        }

        public void InitializeGlobalChannel(IChannel channel) {
            _globalChannel = channel;
        }
        
        public async UniTask SendPartyToUser(string userId, IParty party) {
            var me = _nakamaService.GetMe();
            
            var inviteData = new InviteData {
                UserId = me.User.Id,
                MatchId = party.Id,
                DisplayName = me.User.DisplayName
            };

            var content = new Dictionary<string, string>() {
                {"senderUserId", me.User.Id},
                {"newInvite", inviteData.ToJson()},
                {"targetUserId", userId}
            };
            
            await _nakamaService.SendMessage(_globalChannel, content);

            if (_nakamaService.HasParty(userId)) return;
            _nakamaService.AddParty(userId, party);
        }
        
        public async UniTask SendUserConfirmation(string partyId, string userId) {
            var me = _nakamaService.GetMe();
            
            var content = new Dictionary<string, string>() {
                {"senderUserId", me.User.Id},
                {"approveMatchInvite", partyId},
                {"targetUserId", userId}
            };
            
            await _nakamaService.SendMessage(_globalChannel, content);
        }
        
        public async UniTask SendMatchmakingInfo(string targetUserId, string value) {
            var me = _nakamaService.GetMe();
            
            var content = new Dictionary<string, string>() {
                {"senderUserId", me.User.Id},
                {"valueDropped", value},
                {"targetUserId", targetUserId}
            };

            await _nakamaService.SendMessage(_globalChannel, content);
        }
        
        public async UniTask SendDeclineInviteReceived(string inviteSenderUserId) {
            var me = _nakamaService.GetMe();
            
            var content = new Dictionary<string, string>() {
                {"senderUserId", me.User.Id},
                {"targetUserId", inviteSenderUserId},
                {"declineInviteReceived", me.User.Id}
            };

            await _nakamaService.SendMessage(_globalChannel, content);
        }
        
        public async UniTask SendPauseInfo(string opponent, string value) {
            var senderUser = _nakamaService.GetMe();
            
            var content = new Dictionary<string, string>() {
                {"senderUserId", senderUser.User.Id},
                {"Pause", value},
                {"TargetUser", opponent}
            };

            await _nakamaService.SendMessage(_globalChannel, content);
        }
        
        public async UniTask SendDeclineInviteSended(string inviteSenderUserId) {
            var me = _nakamaService.GetMe();
            
            var content = new Dictionary<string, string>() {
                {"senderUserId", me.User.Id},
                {"targetUserId", inviteSenderUserId},
                {"declineInviteSended", me.User.Id}
            };

            await _nakamaService.SendMessage(_globalChannel, content);
        }
    }
}