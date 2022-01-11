using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Event;
using MikroFramework.Managers;
using MikroFramework.Pool;
using UnityEngine;

namespace MikroFramework.BindableProperty
{
    [Serializable][ES3Serializable]
    public class BindableProperty<T>
    {
        public BindableProperty() { }
        public BindableProperty(T defaultValue = default) {
            this.value = defaultValue;
        }

        [SerializeField]
        [ES3Serializable]
        private T value = default(T);

        public T Value
        {
            get => value;
            set
            {
                if (value == null && this.value == null) {
                    return;
                }

                if (value != null && value.Equals(this.value)) {
                    return;
                }

                T temp = this.value;
                this.value = value;
                this.onValueChanged2?.Invoke(temp, value);
                this.onValueChanged?.Invoke(value);
            }
        }

        [ES3NonSerializable]
        private Action<T> onValueChanged = (v) => { };
        [ES3NonSerializable]
        private Action<T, T> onValueChanged2 = (v, w) => { };

        /// <summary>
        /// RegisterInstance listeners to the event that triggered when the value of the property changes.
        /// </summary>
        /// <param name="onValueChanged"></param>
        /// <returns>The returned IUnRegister allows you to call its UnRegisterWhenGameObjectDestroyed()
        /// function to unregister the event more convenient instead of calling UnRegisterOnValueChanged function</returns>
        [Obsolete("Use the one with two values (new and old) instead")]
        public IUnRegister RegisterOnValueChaned(Action<T> onValueChanged)
        {
            this.onValueChanged += onValueChanged;

            return new BindablePropertyUnRegister<T>(this, onValueChanged);

        }
        [Obsolete("Use the one with two values (new and old) instead")]
        public IUnRegister RegisterWithInitValue(Action<T> onValueChange) {
            onValueChange?.Invoke(value);
            return RegisterOnValueChaned(onValueChange);
        }


        public IUnRegister RegisterWithInitValue(Action<T,T> onValueChange)
        {
            onValueChange?.Invoke(default, value);
            return RegisterOnValueChaned(onValueChange);
        }


        //a == b
        public static implicit operator T(BindableProperty<T> property) {
            return property.value;
        }

        public override string ToString() {
            return value.ToString();
        }

        /// <summary>
        /// RegisterInstance listeners to the event that triggered when the value of the property changes.
        /// </summary>
        /// <param name="onValueChanged">old and new values</param>
        /// <returns>The returned IUnRegister allows you to call its UnRegisterWhenGameObjectDestroyed()
        /// function to unregister the event more convenient instead of calling UnRegisterOnValueChanged function</returns>
        public IUnRegister RegisterOnValueChaned(Action<T, T> onValueChanged)
        {
            this.onValueChanged2 += onValueChanged;

            return new BindablePropertyUnRegister2<T>(this, onValueChanged2);

        }

        /// <summary>
        /// Unregister listeners to the event that triggered when the value of the property changes
        /// </summary>
        /// <param name="onValueChanged"></param>
        [Obsolete("Use the one with two parameters (new and old) instead")]
        public void UnRegisterOnValueChanged(Action<T> onValueChanged)
        {
            this.onValueChanged -= onValueChanged;
        }

        public void UnRegisterOnValueChanged(Action<T, T> onValueChanged)
        {
            this.onValueChanged2 -= onValueChanged;
        }

        public void ClearListeners() {
            this.onValueChanged = null;
            this.onValueChanged2 = null;
        }
    }
}
