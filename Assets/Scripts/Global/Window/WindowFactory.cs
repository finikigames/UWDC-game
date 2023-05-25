using System;
using System.Threading.Tasks;
using Core.AssetManager;
using Core.MVP.Base.Interfaces;
using Global.ConfigTemplate;
using Global.Services.Context.Base;
using Global.VisibilityMechanisms;
using Global.Window.Enums;
using UnityEngine;
using Zenject;

namespace Global.Window {
    public class WindowFactory {
        private readonly IContextService _contextService;
        private readonly IAssetService _service;

        [Inject]
        protected WindowFactory(IContextService contextService,
                                IAssetService assets) {
            _contextService = contextService;
            _service = assets;
        }

        public async Task<UIWindow> Create(WindowConfig data, Transform parent, WindowKey key,
            Vector3 position) {
            var uid = Guid.NewGuid().ToString();

            IBasePresenter<WindowKey> controller = CreateController(data);
            BaseView view = await CreateView(data);

            var window = new UIWindow(uid, data, key, controller, view);
            window.SetPlace(parent, position);

            view.gameObject.SetActive(true);

            return window;
        }

        private IBasePresenter<WindowKey> CreateController(WindowConfig data) {
            var controller = Activator.CreateInstance(data.Controller, _contextService);

            var container = _contextService.ResolveContainer(data.Context);
            container.Inject(controller);
            
            return controller as IBasePresenter<WindowKey>;
        }

        private async Task<BaseView> CreateView(WindowConfig data) {
            var prefab = await _service.Load<GameObject>(data.PrefabReference);

            var container = _contextService.ResolveContainer(data.Context);

            var view = container.InstantiatePrefabForComponent<BaseView>(prefab);
            return view;
        }
    }
}
