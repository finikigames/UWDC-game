using System.Collections.Generic;
using System.Linq;
using Core.MVP.ShowStates.Interfaces;

namespace Core.MVP.ShowStates {
    public abstract class BaseVisibilityService : IVisibilityService {
        private readonly List<IHide> _hides;
        private readonly List<IShow> _shows;

        protected BaseVisibilityService() {
            _hides = new List<IHide>(10);
            _shows = new List<IShow>(10);
        }

        public void SubscribeToShow(IShow show) {
            _shows.Add(show);
        }

        public void SubscribeToHide(IHide hide) {
            _hides.Add(hide);
        }

        public void Show() {
            foreach (var show in _shows.ToList()) show.Show();
        }

        public void Hide() {
            foreach (var hide in _hides.ToList()) hide.Hide();
        }
    }
}