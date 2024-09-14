using AxibugEmuOnline.Client.ClientCore;
using AxibugEmuOnline.Client.Manager;
using System;
using System.Collections.Generic;

namespace AxibugEmuOnline.Client
{
    public class InGameUI : CommandExecuter
    {
        public static InGameUI Instance { get; private set; }

        public RomFile RomFile => m_rom;
        public override bool Enable => gameObject.activeInHierarchy;

        /// <summary> ָʾ����Ϸʵ���Ƿ�������ģʽ </summary>
        public bool IsOnline => App.roomMgr.RoomState <= AxibugProtobuf.RoomGameState.OnlyHost;

        private RomFile m_rom;
        private object m_core;
        private object m_state;

        private List<OptionMenu> menus = new List<OptionMenu>();


        protected override void Awake()
        {
            Instance = this;
            gameObject.SetActiveEx(false);
            menus.Add(new InGameUI_SaveState(this));
            menus.Add(new InGameUI_LoadState(this));
            menus.Add(new InGameUI_QuitGame(this));
            base.Awake();
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        /// <summary>
        /// ��ȡģ�������Ķ���
        /// </summary>
        /// <typeparam name="T">ģ�������Ķ�������</typeparam>
        public T GetCore<T>() => (T)m_core;
        /// <summary> ������ٿ��� </summary>
        public void SaveQuickState(object state)
        {
            m_state = state;
        }
        /// <summary>
        /// ��ȡ���ٿ���
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool GetQuickState<T>(out T state)
        {
            state = default(T);

            if (m_state is T)
            {
                state = (T)m_state;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Show(RomFile currentRom, object core)
        {
            CommandDispatcher.Instance.RegistController(this);

            m_rom = currentRom;
            m_core = core;

            if (App.user.IsLoggedIn)
            {
                App.roomMgr.SendCreateRoom(m_rom.ID, 0, m_rom.Hash);
            }

            gameObject.SetActiveEx(true);
        }

        public void Hide()
        {
            CommandDispatcher.Instance.UnRegistController(this);

            gameObject.SetActiveEx(false);
        }

        protected override void OnCmdOptionMenu()
        {
            OptionUI.Instance.Pop(menus);
        }

        public void QuitGame()
        {
            App.roomMgr.SendLeavnRoom();
            App.emu.StopGame();
        }
    }
}
