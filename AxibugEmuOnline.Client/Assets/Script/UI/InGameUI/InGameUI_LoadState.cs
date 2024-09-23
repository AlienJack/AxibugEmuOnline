using AxibugEmuOnline.Client.ClientCore;
using System.Diagnostics;
using VirtualNes.Core;

namespace AxibugEmuOnline.Client
{
    public class InGameUI_LoadState : ExecuteMenu
    {
        private InGameUI m_gameUI;
        public override bool Visible => !m_gameUI.IsOnline;

        public InGameUI_LoadState(InGameUI gameUI) : base("��ȡ����", null)
        {
            m_gameUI = gameUI;
        }

        public override void OnExcute()
        {
            Stopwatch sw = Stopwatch.StartNew();
            m_gameUI.Core.LoadState(m_gameUI.GetQuickState());
            sw.Stop();
            App.log.Info($"{m_gameUI.RomFile.Platform}====>���ռ��غ�ʱ:{sw.Elapsed.TotalMilliseconds}ms");
        }
    }
}
