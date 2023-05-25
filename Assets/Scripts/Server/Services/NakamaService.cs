using System;
using System.Linq;
using System.Threading.Tasks;
using Global;
using Nakama;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Server.Services {
    public class NakamaService : IDisposable {
        private Client _client;
        private ISession _session;
        private ISocket _socket;
        private ISocketAdapter _adapter;
        private IMatch _match;
        private Profile _profile;

        public void ProvideData(Profile userProfile) {
            _profile = userProfile;
        }

        public async Task CommonInitialize() {
            await CreateClient();
            await DeviceAuth();
            await CreateSocket();
            await ConnectSocket();
            
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
#endif
        }
        
        public async Task JoinGroup(string groupName) {
            await _client.JoinGroupAsync(_session, groupName);
        }

        public async Task<IApiGroupUserList> GetGroupUsers(string groupName, int limit, string cursor = "") {
            return await _client.ListGroupUsersAsync(_session, groupName, 2, limit, cursor);
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

        public async Task CreateClient() {
            _client = new Client("https", "main.arcanecrystalsnakama.ru", 7350, "defaultkey", UnityWebRequestAdapter.Instance);
            
#if UNITY_WEBGL && !UNITY_EDITOR
            _adapter = new JsWebSocketAdapter();
#else
            _adapter = new WebSocketAdapter();
#endif
            Debug.Log(_adapter.GetType());
        }

        public int GetCurrentMatchPlayers() {
            return _match.Presences.Count();
        }

        public async Task DeviceAuth() {
            _session = await _client.AuthenticateDeviceAsync(_profile.UserId);

            Debug.Log(_session);

            var account = await _client.GetAccountAsync(_session);

            var userName = account.User.Username == _profile.Username ? null : _profile.UserId;

            await _client.UpdateAccountAsync(_session, userName, $"{_profile.FirstName} {_profile.LastName}");
        }

        public async Task CreateSocket() {
            _socket = Socket.From(_client, _adapter);
        }

        public async Task ConnectSocket() {
            bool appearOnline = true;
            int connectionTimeout = 30;
            await _socket.ConnectAsync(_session, appearOnline, connectionTimeout);
        }

        public async Task JoinMatch(string matchId) {
            await CheckSocketState();
            
            _match = await _socket.JoinMatchAsync(matchId);

            if (_match.Presences.Count() > 2) {
                await _socket.LeaveMatchAsync(_match);
            }
        }

        public async Task<IMatch> CreateMatch(string matchId) {
            await CheckSocketState();
            
            _match = await _socket.CreateMatchAsync(matchId);
            return _match;
        }

        public async Task<IApiMatchList> GetMatchesList() {
            return await _client.ListMatchesAsync(_session, 0, 1, 10, false, null, null);
        }

        public async Task SendMatchStateAsync(string matchId, long opCode, string data) {
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

        public async Task AddFriend(string friendId) {
            await _client.AddFriendsAsync(_session, new [] {friendId});
        }

        public async Task LeaveMatch(string matchId) {
            await _socket.LeaveMatchAsync(matchId);
        }

        private async Task CheckSocketState() {
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