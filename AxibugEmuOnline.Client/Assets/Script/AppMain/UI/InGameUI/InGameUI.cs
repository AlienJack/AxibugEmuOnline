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

            menus.Add(new InGameUI_FilterSetting(this));
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

            gameObject.SetActiveEx(true);

            var filterSetting = App.filter.GetFilterSetting(currentRom);
            if (filterSetting.filter != null)
            {
                var filter = filterSetting.filter;
                var preset = filterSetting.preset ?? filter.DefaultPreset;

                filter.ApplyPreset(preset);
                App.filter.EnableFilter(filter);
            }
        }

        private void OnServerStepUpdate(int step)
        {
            m_stepPerformer.Perform(step);
        }

        public void Hide()
        {
            CommandDispatcher.Instance.UnRegistController(this);
            gameObject.SetActiveEx(false);

            App.filter.ShutDownFilter();
        }

        protected override void OnCmdOptionMenu()
        {
            OverlayManager.PopSideBar(menus, 0, PopMenu_OnHide);

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
        }
    }
}
