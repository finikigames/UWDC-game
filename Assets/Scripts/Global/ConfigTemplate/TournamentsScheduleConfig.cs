using System;
using System.Collections.Generic;
using Zenject;

namespace Global.ConfigTemplate {
    [Serializable]
    public class TournamentsScheduleConfig : IInitializable {
        public List<TournamentsScheduleData> Datas = new();

        private Dictionary<string, TournamentsScheduleData> _profiles;

        public void Initialize() {
            _profiles = new Dictionary<string, TournamentsScheduleData>(Datas.Count);

            foreach (var data in Datas) {
                _profiles.Add(data.Id, data);
            }
        }
    }
}
