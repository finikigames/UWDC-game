﻿using Global;
using Zenject;

namespace Preloader.DI {
    public class PreloaderInstaller : MonoInstaller {
        public override void InstallBindings() {
            Container
                .BindInterfacesAndSelfTo<EntryPoint>()
                .AsSingle();
        }
    }
}