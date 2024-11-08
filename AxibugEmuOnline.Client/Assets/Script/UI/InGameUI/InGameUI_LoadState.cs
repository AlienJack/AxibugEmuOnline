using AxibugEmuOnline.Client.ClientCore;
using System.Diagnostics;

namespace AxibugEmuOnline.Client
{
    public class InGameUI_LoadState : ExecuteMenu
    {
        private InGameUI m_gameUI;
        public override bool Visible => !m_gameUI.IsNetPlay;

        public InGameUI_LoadState(InGameUI gameUI) : base("��ȡ����", null)
        {
            m_gameUI = gameUI;
        }

        public override void OnExcute()
        {
            if (m_gameUI.IsNetPlay) return;

            object state = m_gameUI.GetQuickState();
            Stopwatch sw = Stopwatch.StartNew();
            if (state != null)
            {
                m_gameUI.Core.LoadState(state);
                sw.Stop();
                App.log.Info($"{m_gameUI.RomFile.Platform}====>���ռ��غ�ʱ:{sw.Elapsed.TotalMilliseconds}ms");
            }
        }
    }
}
