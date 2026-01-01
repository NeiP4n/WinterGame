using DG.Tweening;
using Sources.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Sources.Code.UI
{
    public class GameScreen : BaseScreen
    {
        [SerializeField] private Image _image;
        [SerializeField] private UIInteract _uiInteract;

        private Tween _fadeTween;

        public void Init()
        {
            _fadeTween?.Kill();

            var c = _image.color;
            c.a = 100f;
            _image.color = c;

            FadeOut(3f);
        }

        public void SetBlackInstant()
        {
            _fadeTween?.Kill();

            var c = _image.color;
            c.a = 1f;
            _image.color = c;
        }

        public Tween FadeOut(float duration, Ease ease = Ease.OutCubic)
        {
            _fadeTween?.Kill();
            return _fadeTween = _image.DOFade(0f, duration).SetEase(ease);
        }

        public Tween FadeIn(float duration, Ease ease = Ease.InCubic)
        {
            _fadeTween?.Kill();
            return _fadeTween = _image.DOFade(1f, duration).SetEase(ease);
        }

        public UIInteract GetUIInteract()
        {
            return _uiInteract;
        }
    }
}
