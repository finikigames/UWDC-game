using System.Collections.Generic;
using Global;
using Nakama;
using Nakama.TinyJson;
using Server.Services;
using Zenject;

namespace Server {
    public class GlobalMessageListener : IInitializable {
        private readonly NakamaService _nakamaservice;
        private readonly GlobalScope _globalScope;

        public GlobalMessageListener(NakamaService nakamaservice,
                                     GlobalScope globalScope)
        {
            _nakamaservice = nakamaservice;
            _globalScope = globalScope;
        }
        
        public void Initialize() {
            _nakamaservice.SubscribeToMessages(OnMessageListener);
        }

        private void OnMessageListener(IApiChannelMessage m) {
            var content = m.Content.FromJson<Dictionary<string, string>>();

            if (content.TryGetValue("declineInvite", out var userDeclinedId)) {
                _globalScope.SendedInvites.Remove(userDeclinedId);
            }
        }
    }
}