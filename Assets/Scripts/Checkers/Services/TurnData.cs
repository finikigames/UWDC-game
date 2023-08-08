using Core.Primitives;

namespace Checkers.Services {
    public struct TurnData {
        public Coords To;
        public Coords From;
        public bool Capture;
    }
}