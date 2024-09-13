using AxibugEmuOnline.Client.ClientCore;
using System.Diagnostics;

namespace AxibugEmuOnline.Client
{
    public class InGameUI_SaveState : ExecuteMenu
    {
        private InGameUI m_gameUI;

        public InGameUI_SaveState(InGameUI gameUI) : base("�������", null)
        {
            m_gameUI = gameUI;
        }

        public override void OnExcute()
        {
            Stopwatch sw = Stopwatch.StartNew();
            switch (m_gameUI.RomFile.Platform)
            {
                case EnumPlatform.NES:
                    var state = m_gameUI.GetCore<NesEmulator>().NesCore.GetState();
                    App.log.Info($"{m_gameUI.RomFile.Platform}===>���մ�С{state.ToBytes().Length}");
                    break;
            }
            sw.Stop();
            App.log.Info($"{m_gameUI.RomFile.Platform}====>��ȡ���պ�ʱ:{sw.Elapsed.TotalMilliseconds}ms");
        }
    }
}
