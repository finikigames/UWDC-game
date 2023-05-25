using System.Threading.Tasks;

namespace Core.MVP.Base.Interfaces {
    public interface IView {
        Task ShowView();
        Task Hide();
        void HideImmediate();
        void Dispose();
    }
}