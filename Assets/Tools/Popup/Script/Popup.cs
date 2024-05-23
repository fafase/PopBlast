using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using State = Tools.IPopup.State;

namespace Tools
{
    [RequireComponent(typeof(Animation))]
    public abstract class Popup : MonoBehaviour, IPopup
    {
        [SerializeField] protected Button m_closeBtn;
        [SerializeField] private AnimationClip m_openAnimation;
        [SerializeField] private AnimationClip m_closeAnimation;

        private IPopupManager m_popupManager;
        private Animation m_animation;
        private State m_state = State.Idle;
        public State PopupState => m_state;

        public event Action<IPopup> OnClose;
        public event Action <IPopup> OnOpen;
        public bool IsOpen => m_state == State.Opening || m_state == State.Idle;
        public virtual void Init(IPopupManager popupManager)
        {
            m_popupManager = popupManager;
            transform.SetParent(m_popupManager.Container, false);
            if (m_closeBtn != null)
            {
                m_closeBtn.onClick.AddListener(() => Close());
            }
            m_animation = GetComponent<Animation>();
            m_animation.wrapMode = WrapMode.Once;
            m_animation.AddClip(m_openAnimation, m_openAnimation.name);
            m_animation.AddClip(m_closeAnimation, m_closeAnimation.name);

            m_state = State.Idle;
            StartCoroutine(OpenSequence());
        }

        private IEnumerator OpenSequence() 
        {
            yield return StartCoroutine(AnimationSequence(State.Opening));
            SetButtons(true);
            OnOpen?.Invoke(this);
        }
        private IEnumerator CloseSequence() 
        {
            yield return StartCoroutine(AnimationSequence(State.Closing));
            OnClose?.Invoke(this);
            Destroy(gameObject);
        }


        private IEnumerator AnimationSequence(State state) 
        {
            m_state = state;
            SetButtons(false);
            AnimationClip currentAnim = state == State.Opening ? m_openAnimation : m_closeAnimation;
            m_animation.clip = currentAnim;
            m_animation.Play();
            while (m_animation.IsPlaying(currentAnim.name))
            {
                yield return null;
            }
            m_state = State.Idle;
        }

        public void Close(bool closeImmediate = false) 
        {
            m_popupManager.Close(this);
            if (closeImmediate) 
            {
                Destroy(gameObject);
                return;
            }
            StartCoroutine(CloseSequence());
        }

        public void AddToClose(Action<IPopup> action) => OnClose += action;
        public void RemoveToClose(Action<IPopup> action) => OnClose -= action;

        private void SetButtons(bool value) => Array.ForEach(GetComponentsInChildren<Button>(), (btn => btn.enabled = value));
    }

    public interface IPopup 
    {
        void Close(bool closeImmediate = false);
        void Init(IPopupManager popupManager);
        void AddToClose(Action<IPopup> action);
        void RemoveToClose(Action<IPopup> action);
        State PopupState { get; }
        bool IsOpen { get; }
        public enum State
        {
            Idle, Opening, Closing
        }
    }
}
