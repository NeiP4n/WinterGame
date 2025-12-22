using System;
using UnityEngine;

namespace Sources.Code.UI
{
    public abstract class BaseScreen : MonoBehaviour
    {
        public virtual void Enable()
        {
            gameObject.SetActive(true);
        }
    
        public virtual void Disable()
        {
            gameObject.SetActive(false);
        }
    }

    public abstract class BasePopup : MonoBehaviour
    {
        public event Action<BasePopup> Closed;

        public abstract void Init();

        public void Destroy()
        {
            Closed?.Invoke(this);
        }
    }
}