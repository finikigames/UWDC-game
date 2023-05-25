using Core.Extensions;
using Global;
using Server.Services;
using Zenject;

namespace Preloader {
    public class EntryPoint : IInitializable {
        private readonly ProfileGetService _getService;
        private readonly NakamaService _nakamaService;

        public EntryPoint(ProfileGetService getService,
            NakamaService nakamaService) {
            _getService = getService;
            _nakamaService = nakamaService;
        }
        
        public async void Initialize() {
            _getService.Initialize();
            var profile = _getService.GetProfile();
            
            _nakamaService.ProvideData(profile);
            
            await _nakamaService.CommonInitialize();

            await UnityExtensions.LoadSceneAsync("Main");
        }
    }
}