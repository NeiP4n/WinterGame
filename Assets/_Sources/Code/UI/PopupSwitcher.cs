using System;
using System.Collections.Generic;
using Sources.Code.Core.Singletones;
using UnityEngine;

namespace Sources.Code.UI
{
    public class PopupSwitcher : SingletonBehaviour<PopupSwitcher>
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private List<BasePopup> _popupsPrefabs = new List<BasePopup>();
        
        private readonly Dictionary<Type, BasePopup> _popup = new Dictionary<Type, BasePopup>();
        public void Init()
        {
            if (_transform.childCount <= 0)
                return;
            
            for (int i = 0; i < _transform.childCount; i++)
                Destroy(_transform.GetChild(i).gameObject);
        }
        
        public TPopup GetPopup<TPopup>() where TPopup : BasePopup
        {
            Type popupType = typeof(TPopup);
            if (_popup.TryGetValue(popupType, out var foundPopup))
            {
                return (TPopup)foundPopup;
            }
            else
            {
                BasePopup prefab = _popupsPrefabs.Find(x => x.GetType().FullName == popupType.FullName);
                if (prefab != null)
                {
                    BasePopup newPopup = Instantiate(prefab, _transform);
                    newPopup.Init();
                    newPopup.Closed += PopupOnClosed;

                    return (TPopup)newPopup;
                }
                else
                {
                    Debug.LogError($"Popup prefab not found for type {popupType}", this);
                }
            }

            return null;

            void PopupOnClosed(BasePopup popup)
            {
                popup.Closed -= PopupOnClosed;
                Destroy(popup.gameObject);
                Debug.Log(" Destroy(popup.gameObject);");
            }
        }
    }
}