using AxibugEmuOnline.Client.ClientCore;
using AxibugEmuOnline.Client.Event;
using System.Collections.Generic;

namespace AxibugEmuOnline.Client
{
    public class InGameUI : CommandExecuter
    {
        public static InGameUI Instance { get; private set; }

        public RomFile RomFile => m_rom;
        public override bool Enable => gameObject.activeInHierarchy;

        /// <summary> ָʾ����Ϸʵ���Ƿ�������ģʽ </summary>
        public bool IsNetPlay
        {
            get
            {
                if (!App.user.IsLoggedIn) return false;
                if (App.roomMgr.mineRoomMiniInfo == null) return false;
                if (App.roomMgr.RoomState <= AxibugProtobuf.RoomGameState.OnlyHost) return false;

                return true;
            }
        }

        private RomFile m_rom;
        public IEmuCore Core { get; private set; }
        private object m_state;

        private List<OptionMenu> menus = new List<OptionMenu>();
        private StepPerformer m_stepPerformer;

        protected override void Awake()
        {
            Instance = this;
            gameObject.SetActiveEx(false);

            m_stepPerformer = new StepPerformer(this);

            menus.Add(new InGameUI_Reset(this));
            menus.Add(new InGameUI_SaveState(this));
            menus.Add(new InGameUI_LoadState(this));
            menus.Add(new InGameUI_QuitGame(this));

            base.Awake();
        }
        
        protected override void OnDestroy()
        {
            Instance = null;
            base.OnDestroy();
        }

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
        public object GetQuickState()
        {
            return m_state;
        }

        public void Show(RomFile currentRom, IEmuCore core)
        {
            m_state = null;//�����Ϸ����
            CommandDispatcher.Instance.RegistController(this);

            m_rom = currentRom;
            Core = core;
            m_stepPerformer.Reset();

            if (App.user.IsLoggedIn && !App.roomMgr.InRoom)
            {
                App.roomMgr.SendCreateRoom(m_rom.ID, 0, m_rom.Hash);
            }

            Eventer.Instance.RegisterEvent<int>(EEvent.OnRoomWaitStepChange, OnServerStepUpdate);
            OptionUI.Instance.OnHide += PopMenu_OnHide;

            gameObject.SetActiveEx(true);
        }

        private void OnServerStepUpdate(int step)
        {
            m_stepPerformer.Perform(step);
        }

        public void Hide()
        {
            CommandDispatcher.Instance.UnRegistController(this);

            OptionUI.Instance.OnHide -= PopMenu_OnHide;
            gameObject.SetActiveEx(false);
        }

        protected override void OnCmdOptionMenu()
        {
            OptionUI.Instance.Pop(menus);

            if (!IsNetPlay)//����ģʽ��ͣģ����
            {
                Core.Pause();
            }
        }

        //�˵��ر�ʱ��
        private void PopMenu_OnHide()
        {
            if (!IsNetPlay)//����ģʽ�ָ�ģ��������ͣ
                Core.Resume();
        }


        public void QuitGame()
        {
            Eventer.Instance.UnregisterEvent<int>(EEvent.OnRoomWaitStepChange, OnServerStepUpdate);
            App.roomMgr.SendLeavnRoom();
            App.emu.StopGame();

            CommandDispatcher.Instance.Current = CommandDispatcher.Instance.Normal;
        }
    }
}
