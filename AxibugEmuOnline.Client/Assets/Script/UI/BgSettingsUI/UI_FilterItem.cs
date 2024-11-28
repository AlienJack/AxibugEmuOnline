using AxibugEmuOnline.Client.ClientCore;
using AxibugEmuOnline.Client.UI;

namespace AxibugEmuOnline.Client
{
    /// <summary>
    /// ������ɫ����UI
    /// </summary>
    public class UI_FilterItem : MenuItem, IVirtualItem
    {
        public int Index { get; set; }
        public FilterManager.Filter Datacontext { get; private set; }


        public void SetData(object data)
        {
            Datacontext = data as FilterManager.Filter;

            UpdateView();
        }

        private void UpdateView()
        {
            SetBaseInfo(Datacontext.Name, $"��������:{Datacontext.Paramerters.Count}", null);
        }

        public void SetDependencyProperty(object data)
        {
            SetSelectState(data is ThirdMenuRoot tr && tr.SelectIndex == Index);

            if (m_select)
            {
                App.filter.EnableFilterPreview();
                App.filter.EnableFilter(Datacontext);
            }
        }

        public void Release()
        {
        }

        public override bool OnEnterItem()
        {
            return false;
        }
    }
}
