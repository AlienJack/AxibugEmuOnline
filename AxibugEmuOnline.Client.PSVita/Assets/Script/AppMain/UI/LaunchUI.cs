using AxibugEmuOnline.Client.ClientCore;
using AxibugEmuOnline.Client.UI;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace AxibugEmuOnline.Client
{
    public class LaunchUI : MonoBehaviour
    {
        [SerializeField]
        RectTransform MainMenuRoot;
        [SerializeField]
        MainMenuController MainMenu;
        public Image BG;

        Vector2 m_mainLayoutPosition;
        [SerializeField]
        float m_detailLayoutPosition_x = 55;
        [SerializeField]
        float m_LayoutChangeSpeed = 10;

        public static LaunchUI Instance { get; private set; }

        TweenerCore<Vector2, Vector2, VectorOptions> m_layoutTween;

        private void Awake()
        {
            Instance = this;
            m_mainLayoutPosition = MainMenuRoot.anchoredPosition;
            MainMenu.ListenControlAction = true;
        }

        private void Start()
        {
            CommandDispatcher.Instance.Current = CommandDispatcher.Instance.Normal;
        }

        private void Update()
        {
            if (CommandDispatcher.Instance.Current == CommandDispatcher.Instance.Gaming && App.emu.Core.IsNull())
                CommandDispatcher.Instance.Current = CommandDispatcher.Instance.Normal;
        }

        public void HideMainMenu()
        {
            BG.gameObject.SetActiveEx(false);
            MainMenuRoot.gameObject.SetActiveEx(false);
        }

        public void ShowMainMenu()
        {
            BG.gameObject.SetActiveEx(true);
            MainMenuRoot.gameObject.SetActiveEx(true);
        }

        public void ToDetailMenuLayout()
        {
            if (m_layoutTween != null)
            {
                m_layoutTween.Kill();
                m_layoutTween = null;
            }
            m_layoutTween = DOTween
                .To(
                () => MainMenuRoot.anchoredPosition,
                (x) => MainMenuRoot.anchoredPosition = x,
                new Vector2(m_detailLayoutPosition_x, MainMenuRoot.anchoredPosition.y),
                m_LayoutChangeSpeed)
                .SetSpeedBased();
            MainMenu.ListenControlAction = false;
            MainMenu.EnterDetailState();
        }

        public void ToMainMenuLayout()
        {
            if (m_layoutTween != null)
            {
                m_layoutTween.Kill();
                m_layoutTween = null;
            }
            m_layoutTween = DOTween.To(
                () => MainMenuRoot.anchoredPosition,
                (x) => MainMenuRoot.anchoredPosition = x,
                m_mainLayoutPosition,
                m_LayoutChangeSpeed)
                .SetSpeedBased();
            MainMenu.ListenControlAction = true;
            MainMenu.ExitDetailState();
        }
    }
}
