using AxibugEmuOnline.Client.ClientCore;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AxibugEmuOnline.Client
{
    public class RomListMenuItem : VirtualSubMenuItem
    {
        [SerializeField]
        protected EnumPlatform Platform;

        private RomLib RomLib
        {
            get
            {
                switch (Platform)
                {
                    case EnumPlatform.NES:
                        return App.nesRomLib;
                    default:
                        throw new System.NotImplementedException($"δʵ�ֵ�ƽ̨ {Platform}");
                }
            }
        }

        private List<OptionMenu> m_options;

        protected override void Awake()
        {
            base.Awake();

            m_options = new List<OptionMenu>()
            {
                new OptMenu_Search(this),
                new OptMenu_ShowAll(this),
            };
        }

        public string SearchKey;
        protected override void GetVirtualListDatas(Action<object> datas)
        {
            RomLib.FetchRomCount((roms) => datas.Invoke(roms), SearchKey);
        }

        public override bool OnEnterItem()
        {
            var res = base.OnEnterItem();
            if (res) CommandDispatcher.Instance.RegistController(this);

            return true;
        }

        public override bool OnExitItem()
        {
            var res = base.OnExitItem();
            if (res) CommandDispatcher.Instance.UnRegistController(this);

            return false;
        }


        protected override void OnCmdOptionMenu()
        {
            OptionUI.Instance.Pop(m_options);
        }

        public class OptMenu_Search : ExecuteMenu
        {
            private RomListMenuItem m_romListUI;

            public OptMenu_Search(RomListMenuItem romListUI, Sprite icon = null) : base("����", icon)
            {
                m_romListUI = romListUI;
            }

            public override void OnExcute(OptionUI optionUI, ref bool cancelHide)
            {
                OverlayManager.Input(OnSearchCommit, "����Rom����", m_romListUI.SearchKey);
            }

            private void OnSearchCommit(string text)
            {
                m_romListUI.SearchKey = text;
                m_romListUI.RefreshUI();
            }
        }

        public class OptMenu_ShowAll : ExecuteMenu
        {
            private RomListMenuItem m_ui;

            public override bool Visible => !string.IsNullOrWhiteSpace(m_ui.SearchKey);

            public OptMenu_ShowAll(RomListMenuItem romListUI, Sprite icon = null) : base("��ʾȫ��", icon)
            {
                m_ui = romListUI;
            }

            public override void OnExcute(OptionUI optionUI, ref bool cancelHide)
            {
                m_ui.SearchKey = null;
                m_ui.RefreshUI();
            }
        }
    }
}
