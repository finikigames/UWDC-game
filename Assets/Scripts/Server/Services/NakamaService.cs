﻿using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Global.Services;
using Nakama;
using Nakama.TinyJson;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Server.Services {
    public class NakamaService : IDisposable {
        private Client _client;
        private Profile _profile;
        private ISession _session;
        private ISocket _socket;
        private ISocketAdapter _adapter;
        private IMatch _match;
        private IParty _party;
        private IApiAccount _me;
        private IChannel _globalChannel;
        private IApiGroup _apiGroup;

        private Dictionary<string, IParty> _createdParties;

        public NakamaService() {
            _createdParties = new Dictionary<string, IParty>();
        }
        
        public void ProvideData(Profile userProfile) {
            _profile = userProfile;
        }

        public async UniTask CommonInitialize() {
            await CreateClient();
            await DeviceAuth();
            await CreateSocket();
            await ConnectSocket();
            _me = await GetUserInfo();
            
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
#endif
        }

        public IApiAccount GetMe() {
            return _me;
        }

        public async UniTask<IMatchmakerTicket> AddMatchmaker() {
            return await _socket.AddMatchmakerAsync(minCount: 2, maxCount: 2);
        }

        public async UniTask RemoveMatchmaker(IMatchmakerTicket ticket) {
            await _socket.RemoveMatchmakerAsync(ticket);
        }

        public void SubscribeToMatchmakerMatched(Action<IMatchmakerMatched> callback) {
            _socket.ReceivedMatchmakerMatched += callback;
        }

        public void UnsubscribeMatchmakerMatched(Action<IMatchmakerMatched> callback) {
            _socket.ReceivedMatchmakerMatched -= callback;
        }

        public async UniTask RemoveAllPartiesExcept(string userId) {
            foreach (var partyPair in _createdParties) {
                if (partyPair.Key == userId) continue;
                await LeaveParty(partyPair.Value.Id);
            }

            _createdParties = _createdParties.Where(a => a.Key == userId)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public async UniTask LeaveCurrentMatch() {
            await _socket.LeaveMatchAsync(_match.Id);
        }
        
        public async UniTask RemoveAllParties() {
            foreach (var partyPair in _createdParties) {
                await LeaveParty(partyPair.Value.Id);
            }
            
            _createdParties.Clear();
        }
        
        public async UniTask<IParty> CreateParty() {
            return await _socket.CreatePartyAsync(false, 2);
        }

        public async UniTask LeaveParty(string partyId) {
            await _socket.LeavePartyAsync(partyId);
        }
        
        public async UniTask JoinParty(string partyId) {
            await _socket.JoinPartyAsync(partyId);
        }

        public async UniTask SendUserConfirmation(string partyId, string userId) {
            var senderUserId = _me.User.Id;
            
            var content = new Dictionary<string, string>() {
                {"senderUserId", senderUserId},
                {"approveMatchInvite", partyId},
                {"targetUserId", userId}
            };
            
            await _socket.WriteChatMessageAsync(_globalChannel, content.ToJson());
        }
        
        public async UniTask SendPartyToUser(string userId, IParty party) {
            var content = new Dictionary<string, string>() {
                {"senderUserId", _me.User.Id},
                {"partyId", party.Id},
                {"senderDisplayName", _me.User.DisplayName},
                {"targetUserId", userId}
            };
            
            await _socket.WriteChatMessageAsync(_globalChannel, content.ToJson());

            if (_createdParties.ContainsKey(userId)) return;
            _createdParties.Add(userId, party);
        }

        public async UniTask<IChannel> JoinChat(string groupId) {
            _globalChannel = await _socket.JoinChatAsync(groupId, ChannelType.Group, true);
            return _globalChannel;
        }
        
        public async UniTask JoinTournament(string id) {
            await _client.JoinTournamentAsync(_session, id);
        }

        public async UniTask SubmitTournamentScore(string title, Dictionary<string, string> metadata, int score, int subScore) {
            await _client.WriteTournamentRecordAsync(_session, title, score, subScore, JsonWriter.ToJson(metadata));
        }

        public void SubscribeToPartyPresence(Action<IPartyPresenceEvent> callback) {
            _socket.ReceivedPartyPresence += callback;
        }

        public void UnsubscribeFromPartyPresence(Action<IPartyPresenceEvent> callback) {
            _socket.ReceivedPartyPresence -= callback;
        }
        
        public void SubscribeToMessages(Action<IApiChannelMessage> onParty) {
            _socket.ReceivedChannelMessage += onParty;
        }

        public void UnsubscribeFromMessages(Action<IApiChannelMessage> onParty) {
            _socket.ReceivedChannelMessage -= onParty;
        }
        
        public async UniTask<IApiAccount> GetUserInfo() {
            return await _client.GetAccountAsync(_session);
        }
        
        public async UniTask<IApiGroup> CreateGroup(string groupName) {
            var groups = await _client.ListGroupsAsync(_session, groupName);
            foreach (var group in groups.Groups) {
                if (group.Name == groupName) {
                    return group;
                }
            }
            _apiGroup = await _client.CreateGroupAsync(_session, groupName);
            return _apiGroup;
        }
        
        public async UniTask JoinGroup(string groupId) {
            var resultsMember = await _client.ListUserGroupsAsync(_session, 2, 1, "");
            
            foreach (var group in resultsMember.UserGroups) {
                if (group.Group.Id == groupId) return;
            }
            var resultsSuperAdmin = await _client.ListUserGroupsAsync(_session, 0, 1, "");

            foreach (var group in resultsSuperAdmin.UserGroups) {
                if (group.Group.Id == groupId) return;
            }
            
            await _client.JoinGroupAsync(_session, groupId);
        }

        public async UniTask<IApiGroup> GetGroupInfo(string groupName) {
            var groups = await _client.ListGroupsAsync(_session, groupName);

            foreach (var group in groups.Groups) {
                if (group.Name == groupName) return group;
            }

            return null;
        }

        public async UniTask<IApiGroupUserList> GetGroupUsers(string groupName, int limit, string cursor = "") {
            return await _client.ListGroupUsersAsync(_session, groupName, 2, limit, cursor);
        }

        public async UniTask UpdateUserStatus() {
        }
        
        public async UniTask<List<IGroupUserListGroupUser>> GetGroupUsersWithoutMe(string groupName, int limit, string cursor = "") {
            var usersList = await _client.ListGroupUsersAsync(_session, groupName, 2, limit, cursor);

            var value = usersList.GroupUsers.Where(u => u.User.Id != _me.User.Id).ToList();

            return value;
        }
        
        public string GetCurrentMatchId() {
            return _match.Id;
        }

#if UNITY_EDITOR
        private void OnPlayModeChanged(PlayModeStateChange change) {
            if (change == PlayModeStateChange.ExitingPlayMode) {
                LeaveMatches();
            }
        }
#endif

        public async UniTask CreateClient() {
            _client = new Client("https", "main.arcanecrystalsnakama.ru", 7350, "defaultkey", UnityWebRequestAdapter.Instance);
            
#if UNITY_WEBGL && !UNITY_EDITOR
            _adapter = new JsWebSocketAdapter();
#else
            _adapter = new WebSocketAdapter();
#endif
            Debug.Log(_adapter.GetType());
        }

        public List<IUserPresence> GetCurrentMatchPlayers() {
            return _match.Presences.ToList();
        }
        
        public int GetCurrentMatchPlayersCount() {
            return _match.Presences.Count();
        }


        public IUserPresence GetOpponent() {
            var presences = GetCurrentMatchPlayers();

            IUserPresence presence = null;
            foreach (var local in presences) {
                if (local.UserId != _me.User.Id) {
                    presence = local;
                    break;
                }
            }

            return presence;
        }

        public async UniTask DeviceAuth() {
            _session = await _client.AuthenticateDeviceAsync(_profile.UserId, _profile.Username);

            Debug.Log(_session);

            await _client.UpdateAccountAsync(_session, null, $"{_profile.FirstName} {_profile.LastName}");
        }

        public async UniTask CreateSocket() {
            _socket = Socket.From(_client, _adapter);
        }

        public async UniTask ConnectSocket() {
            bool appearOnline = true;
            int connectionTimeout = 30;
            await _socket.ConnectAsync(_session, appearOnline, connectionTimeout);
        }

        public async UniTask JoinMatch(string matchId) {
            await CheckSocketState();
            
            _match = await _socket.JoinMatchAsync(matchId);

            if (_match.Presences.Count() > 2) {
                await _socket.LeaveMatchAsync(_match);
            }
        }

        public async UniTask<IMatch> CreateMatch(string matchId) {
            await CheckSocketState();
            
            _match = await _socket.CreateMatchAsync(matchId);
            return _match;
        }

        public async UniTask<IApiMatchList> GetMatchesList() {
            return await _client.ListMatchesAsync(_session, 0, 1, 10, false, null, null);
        }

        public async UniTask SendMatchStateAsync(string matchId, long opCode, string data) {
            await _socket.SendMatchStateAsync(matchId, opCode, data);
        }

        public void SubscribeToMatchPresence(Action<IMatchPresenceEvent> callback) {
            _socket.ReceivedMatchPresence += callback;
        }

        public void UnsubscribeFromMatchPresence(Action<IMatchPresenceEvent> callback) {
            _socket.ReceivedMatchPresence -= callback;
        }

        public void SubscribeToMatchState(Action<IMatchState> callback) {
            _socket.ReceivedMatchState += callback;
        }

        public void UnsubscribeFromMatchState(Action<IMatchState> callback) {
            _socket.ReceivedMatchState -= callback;
        }

        public async UniTask AddFriend(string friendId) {
            await _client.AddFriendsAsync(_session, new [] {friendId});
        }

        public async UniTask LeaveMatch(string matchId) {
            await _socket.LeaveMatchAsync(matchId);
        }

        private async UniTask CheckSocketState() {
            if (_socket.IsConnected) return;

            await ConnectSocket();
        }

        public void Dispose() {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
#endif
        }
        private void LeaveMatches() {
            if (_match == null) return;
            _socket.LeaveMatchAsync(_match);
            _match = null;
        }
    }
}