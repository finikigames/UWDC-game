﻿using Checkers.UI.Data;
using Checkers.UI.Views.Base;
using Cysharp.Threading.Tasks;
using Global.Context;
using Global.Window.Base;
using Global.Window.Enums;
using Global.Window.Signals;
using UnityEngine.Scripting;

namespace Checkers.UI.Presenters {
    [Preserve]
    public class MatchWindowPresenter : BaseWindowPresenter<IMatchWindow, MatchWindowData> {
        public MatchWindowPresenter(ContextService service) : base(service) {
        }

        protected override async UniTask LoadContent() {
            View.SubscribeToHowToPlayButton(OnHowToPlayClick);
            View.SubscribeToFleeButton(OnFleeClick);
            
            View.ProvideCamera(UnityEngine.Camera.main);
        }

        private void OnFleeClick() {
            FireSignal(new OpenWindowSignal(WindowKey.FleeWindow, new FleeWindowData()));
        }

        private void OnHowToPlayClick() {
            FireSignal(new OpenWindowSignal(WindowKey.RulesWindow, new RulesWindowData()));
        }
    }
}