using AxibugEmuOnline.Client.ClientCore;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AxibugEmuOnline.Client
{
    public class CommandDispatcher : MonoBehaviour
    {
        public static CommandDispatcher Instance { get; private set; }

        /// <summary> ƽ��ע�����,������Ӧָ�� </summary>
        List<CommandExecuter> m_register = new List<CommandExecuter>();
        /// <summary> ��ռע�����,ָ��ᱻ�б������һ�������ռ </summary>
        List<CommandExecuter> m_registerHigh = new List<CommandExecuter>();

        ICommandListener m_listener;
        /// <summary> ��׼UI���� </summary>
        public IKeyMapperChanger Normal { get; private set; }
        /// <summary> ��Ϸ��UI���� </summary>
        public IKeyMapperChanger Gaming { get; private set; }

        private IKeyMapperChanger m_current;
        public IKeyMapperChanger Current
        {
            get => m_current;
            set
            {
                m_current = value;

                SetKeyMapper(m_current);
            }
        }

        private void Awake()
        {
            Instance = this;

            //��ʼ��command������
            m_listener = new CommandListener();

            //��ʼ����λ�޸���
            Normal = new NormalChanger();
            Gaming = new GamingChanger();
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public bool IsRegisted(CommandExecuter commandExecuter)
        {
            return m_register.Contains(commandExecuter) || m_registerHigh.Contains(commandExecuter);
        }

        public void RegistController(CommandExecuter controller)
        {
            if (!controller.AloneMode)
            {
                if (m_register.Contains(controller)) { return; }

                m_register.Add(controller);
            }
            else
            {
                if (m_registerHigh.Contains(controller)) { return; }

                m_registerHigh.Add(controller);
            }
        }

        public void UnRegistController(CommandExecuter menuItemController)
        {
            if (!menuItemController.AloneMode)
                m_register.Remove(menuItemController);
            else
                m_registerHigh.Remove(menuItemController);
        }

        readonly List<CommandExecuter> oneFrameRegister = new List<CommandExecuter>();
        private void Update()
        {
            if (!InputUI.IsInputing)
            {
                peekRegister(oneFrameRegister);
                m_listener.Update(oneFrameRegister);
            }

            //��λӳ���ڰ�����Ӧ�Ķ�ջ��������,��ֹ�������޸�����
            if (m_waitMapperSetting != null)
            {
                m_listener.ApplyKeyMapper(m_waitMapperSetting);
                m_waitMapperSetting = null;
            }
        }

        IKeyMapperChanger m_waitMapperSetting = null;
        void SetKeyMapper(IKeyMapperChanger keyMapChanger)
        {
            m_waitMapperSetting = keyMapChanger;
        }

        private List<CommandExecuter> peekRegister(List<CommandExecuter> results)
        {
            results.Clear();

            if (m_registerHigh.Count > 0)
            {
                for (int i = m_registerHigh.Count - 1; i >= 0; i--)
                {
                    var controller = m_registerHigh[i];
                    if (controller.Enable)
                    {
                        results.Add(controller);
                        return results;
                    }
                }
            }

            foreach (var controller in m_register)
            {
                if (!controller.Enable) continue;

                results.Add(controller);
            }

            return results;
        }

#if UNITY_EDITOR
        public void GetRegisters(out IReadOnlyList<CommandExecuter> normal, out IReadOnlyList<CommandExecuter> alone)
        {
            normal = m_register;
            alone = m_registerHigh;
        }
#endif
    }
}
