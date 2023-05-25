using System;
using System.Collections.Generic;

namespace Core.Observable {
    public class Observable<T> {
        private T _value;
        private readonly List<Action<Observable<T>, T>> _subscribersWithSender = new List<Action<Observable<T>, T>>();
        private readonly List<Action<T>> _subscribersWithoutSender = new List<Action<T>>();

        public Observable() {}

        public Observable(T value) {
            _value = value;
        }

        public T Value {
            get => _value;
            set => SetData(value);
        }

        public static Observable<T> operator +(Observable<T> observable, Action<T> handler) {
            if (observable == null) {
                observable = new Observable<T>();
                observable.Subscribe(handler);
                
                return observable;
            }
            
            observable.Subscribe(handler);

            return observable;
        }
        
        public static Observable<T> operator -(Observable<T> observable, Action<T> handler) {
            observable.Unsubscribe(handler);

            return observable;
        }

        public void Subscribe(Action<Observable<T>, T> handler, bool immediateCall = true) {
            if(_subscribersWithSender.Contains(handler)) return;
            _subscribersWithSender.Add(handler);

            if(!immediateCall) return;
            handler(this, _value);
        }

        public void Subscribe(Action<T> handler, bool immediateCall = true) {
            if(_subscribersWithoutSender.Contains(handler)) return;
            _subscribersWithoutSender.Add(handler);

            if(!immediateCall) return;
            if(_value == null) return;
            handler(_value);
        }

        public void Unsubscribe(Action<Observable<T>, T> handler) {
            if(!_subscribersWithSender.Contains(handler)) return;
            _subscribersWithSender.Remove(handler);
        }

        public void Unsubscribe(Action<T> handler) {
            if(!_subscribersWithoutSender.Contains(handler)) return;
            _subscribersWithoutSender.Remove(handler);
        }

        public void UnsubscribeAll() {
            _subscribersWithSender.Clear();
            _subscribersWithoutSender.Clear();
        }

        public void ForceNotify() {
            Notify(_value);
        }

        private void Notify(T value) {
            if(value == null) return;

            foreach (var subscriber in _subscribersWithSender) {
                subscriber(this, value);
            }

            foreach (var subscriber in _subscribersWithoutSender) {
                subscriber(value);
            }
        }

        protected void SetData(T setValue) {
            if (_value.Equals(setValue)) return;
            _value = setValue;
            Notify(_value);
        }
    }
}