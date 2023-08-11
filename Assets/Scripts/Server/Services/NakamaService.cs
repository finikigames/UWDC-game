using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Global.Extensions;
using Global.Services;
using Nakama;
using Nakama.TinyJson;
using Newtonsoft.Json;
using UnityEngine;
using JsonWriter = Nakama.TinyJson.JsonWriter;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Server.Services {
    public class NakamaService : IDisposable {
        public Action OnSocketReconnect;
        
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
        private IApiTournament _tournament;

        private bool _isSocketConnecting;

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

        private async UniTask CheckForSessionExpired() {
            if (!_session.IsExpired) return;
            if (_session.IsRefreshExpired) {
                await CommonInitialize();
                return;
            }

            await _client.SessionRefreshAsync(_session);
        }
        
        public IApiAccount GetMe() {
            return _me;
        }

        public void AddParty(string id, IParty party) {
            _createdParties.Add(id, party);
        }

        public bool HasParty(string id) {
            return _createdParties.ContainsKey(id);
        }

        public void RemoveParty(string id) {
            _createdParties.Remove(id);
        }

        public async UniTask<IMatchmakerTicket> AddMatchmaker() {
            await CheckForSocketConnect();
            
            return await _socket.AddMatchmakerAsync(minCount: 2, maxCount: 2);
        }

        public async UniTask RemoveMatchmaker(IMatchmakerTicket ticket) {
            await CheckForSocketConnect();
            
            await _socket.RemoveMatchmakerAsync(ticket);
        }

        public async UniTask GoOffline() {
            await CheckForSocketConnect();
            
            await _socket.UpdateStatusAsync(null);
        }

        public async UniTask GoOnline() {
            await CheckForSocketConnect();
            
            await _socket.UpdateStatusAsync("online");
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

        public async UniTask<IApiTournamentRecordList> ListTournamentRecordsAroundOwner(string tournamentId, string cursor, int limit = 100) {
            return await _client.ListTournamentRecordsAroundOwnerAsync(_session, tournamentId, _me.User.Id, limit:limit, cursor:cursor);
        }

        public async UniTask LeaveCurrentMatch() {
            await CheckForSocketConnect();
            
            await _socket.LeaveMatchAsync(_match.Id);
        }

        public async UniTask RemoveAllParties() {
            foreach (var partyPair in _createdParties) {
                await LeaveParty(partyPair.Value.Id);
            }
            
            _createdParties.Clear();
        }

        public async UniTask<IParty> CreateParty() {
            await CheckForSocketConnect();
            
            return await _socket.CreatePartyAsync(false, 2);
        }

        public async UniTask LeaveParty(string partyId) {
            await CheckForSocketConnect();
            
            await _socket.LeavePartyAsync(partyId);
        }

        public async UniTask JoinParty(string partyId) {
            await CheckForSocketConnect();
            
            await _socket.JoinPartyAsync(partyId);
        }

        public async UniTask SendMessage(IChannel channel, Dictionary<string, string> data) {
            await CheckForSocketConnect();
            
            await _socket.WriteChatMessageAsync(channel, data.ToJson());
        }

        public async UniTask<IChannel> JoinChatByName(string chatName) {
            var group = await CreateGroup(chatName);
            await JoinGroup(group.Id);
            return await JoinChat(group.Id);
        }
        
        public async UniTask<IChannel> JoinChat(string groupId, ChannelType type = ChannelType.Group, bool persistence = true) {
            await CheckForSocketConnect();
            
            _globalChannel = await _socket.JoinChatAsync(groupId, type, persistence);
            return _globalChannel;
        }

        public async UniTask<IApiTournament> GetTournament(string id) {
            await CheckForSessionExpired();
            
            var list = await _client.ListTournamentsAsync(_session, 1, 2);

            foreach (var tournament in list.Tournaments) {
                if (tournament.Id != id) continue;

                _tournament = tournament;
                return tournament;
            }

            return null;
        }

        public async UniTask JoinTournament(string id) {
            if (!_tournament.IsActive()) return;
            await CheckForSessionExpired();
            await _client.JoinTournamentAsync(_session, id);
        }

        //Return first in leaderboard players 

        public async UniTask<IApiTournamentRecordList> ListTournamentRecords(string id, int limit = 100, string cursor = null) {
            if (!_tournament.IsActive()) return null;
            await CheckForSessionExpired();
            
            return await _client.ListTournamentRecordsAsync(_session, id, new []{ _session.UserId }, limit:limit, cursor:cursor);
        }

        //Return some count of players around player pos.

        public async UniTask<IApiTournamentRecordList> ListTournamentRecordsAround(string id, int limit = 100) {
            if (!_tournament.IsActive()) return null;
            await CheckForSessionExpired();
            
            return await _client.ListTournamentRecordsAroundOwnerAsync(_session, id, _session.UserId, limit:limit);
        }

        public async UniTask SubmitTournamentScore(string id, Dictionary<string, string> metadata, int score, int subScore) {
            if (!_tournament.IsActive()) return;
            await CheckForSessionExpired();
            
            await _client.WriteTournamentRecordAsync(_session, id, score, subScore, metadata == null ? null:  JsonWriter.ToJson(metadata));
        }

        public void SubscribeToPartyPresence(Action<IPartyPresenceEvent> callback) {
            _socket.ReceivedPartyPresence += callback;
        }

        public void UnsubscribeFromPartyPresence(Action<IPartyPresenceEvent> callback) {
            _socket.ReceivedPartyPresence -= callback;
        }

        public async UniTask WriteStorageObject<T>(string collectionId, string key, T value) where T : class, new() {
            await CheckForSessionExpired();
            
            var writeObject = new WriteStorageObject {
                Collection = collectionId,
                Key = key,
                Value = JsonConvert.SerializeObject(value),
                PermissionRead = 2,
                PermissionWrite = 1
            };

            await _client.WriteStorageObjectsAsync(_session, new [] { writeObject });
        }

        public async UniTask DeleteStorageObject<T>(string collectionId, string key, T value) where T : class, new() {
            await CheckForSessionExpired();

            var deleteObject = new StorageObjectId {
                Collection = collectionId,
                Key = key
            };

            await _client.DeleteStorageObjectsAsync(_session, new[] { deleteObject });
        }

        public async UniTask<IApiStorageObjectList> ListStorageObjects(string id, string userId = null) {
            if (string.IsNullOrEmpty(userId)) {
                userId = _session.UserId;
            }

            await CheckForSessionExpired();
            
            return await _client.ListUsersStorageObjectsAsync(_session, id, userId);
        }

        public async UniTask<T> ListStorageObjects<T>(string collectionId, string key, string userId = null) where T : class, new() {
            if (string.IsNullOrEmpty(userId)) {
                userId = _session.UserId;
            }

            await CheckForSessionExpired();
            var objects = await _client.ListUsersStorageObjectsAsync(_session, collectionId, userId, limit:15);

            foreach (var obj in objects.Objects) {
                if (obj.Key != key) continue;

                return obj.Value.FromJson<T>();
            }

            return new();
        }

        public async UniTask<string> GetStorageObject(string collectionId, string key, string userId = null) {
            if (string.IsNullOrEmpty(userId)) {
                userId = _session.UserId;
            }

            await CheckForSessionExpired();
            var objects = await _client.ListUsersStorageObjectsAsync(_session, collectionId, userId, limit: 15);

            foreach (var obj in objects.Objects) {
                if (obj.Key != key) continue;

                return obj.Value;
            }

            return "";
        }

        public void SubscribeToMessages(Action<IApiChannelMessage> onParty) {
            _socket.ReceivedChannelMessage += onParty;
        }

        public void UnsubscribeFromMessages(Action<IApiChannelMessage> onParty) {
            _socket.ReceivedChannelMessage -= onParty;
        }

        public async UniTask<IApiAccount> GetUserInfo() {
            await CheckForSessionExpired();
            return await _client.GetAccountAsync(_session);
        }

        public async UniTask<IApiUsers> GetUsersInfos(string[] userIds) {
            await CheckForSessionExpired();
            return  await _client.GetUsersAsync(_session, userIds);
        }

        public async UniTask<IApiUser> GetUserInfo(string userId) {
            await CheckForSessionExpired();
            var users =  await _client.GetUsersAsync(_session, new [] {userId});
            foreach (var user in users.Users) {
                return user;
            }

            return null;
        }

        public async UniTask<IApiGroup> CreateGroup(string groupName) {
            await CheckForSessionExpired();
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
            await CheckForSessionExpired();
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
            await CheckForSessionExpired();
            
            var groups = await _client.ListGroupsAsync(_session, groupName);

            foreach (var group in groups.Groups) {
                if (group.Name == groupName) return group;
            }

            return null;
        }

        public async UniTask<IApiGroupUserList> GetGroupUsers(string groupName, int limit, string cursor = "") {
            await CheckForSessionExpired();
            return await _client.ListGroupUsersAsync(_session, groupName, 2, limit, cursor);
        }

        public async UniTask<List<IGroupUserListGroupUser>> GetGroupUsersWithoutMe(string groupName, int limit, string cursor = "") {
            await CheckForSessionExpired();
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
                EditorApplication.playModeStateChanged -= OnPlayModeChanged;
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
            if (_socket != null) {
                await _socket.ConnectAsync(_session, true);
            }
            _socket = _client.NewSocket(true, _adapter);
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
            await CheckForSessionExpired();
            
            await _client.AddFriendsAsync(_session, new [] {friendId});
        }

        public async UniTask LeaveMatch(string matchId) {
            await _socket.LeaveMatchAsync(matchId);
        }

        private async UniTask CheckSocketState() {
            if (_socket.IsConnected) return;

            await ConnectSocket();
        }

        private async UniTask CheckForSocketConnect() {
            if (_isSocketConnecting || _socket.IsConnected || _socket.IsConnecting) return;

            _isSocketConnecting = true;
            await _socket.ConnectAsync(_session);
            OnSocketReconnect?.Invoke();

            _isSocketConnecting = false;
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