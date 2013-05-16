using System;

using Bjd;
using Bjd.ctrl;
using Bjd.net;
using Bjd.option;
using System.Collections.Generic;

namespace TunnelServer {
    class Option : OneOption {

        public override string JpMenu { get { return NameTag; } }
        public override string EnMenu { get { return NameTag; } }
        public override char Mnemonic { get { return '0'; } }

        public Option(Kernel kernel, string path, string nameTag)
            : base(kernel.IsJp(), path, nameTag) {

            Add(new OneVal("useServer", false, Crlf.Nextline, new CtrlCheckBox(IsJp() ? "���̒�`���g�p����" : "Use this configration")));

            var pageList = new List<OnePage>();
            pageList.Add(Page1("Basic", IsJp() ? "��{�ݒ�" : "Basic", kernel));
            pageList.Add(PageAcl());
            Add(new OneVal("tab", null, Crlf.Nextline, new CtrlTabPage("tabPage", pageList)));

            Read(kernel.IniDb); //�@���W�X�g������̓ǂݍ���
        }

        private OnePage Page1(string name, string title, Kernel kernel) {
            var onePage = new OnePage(name, title);
            
            //nameTag����|�[�g�ԍ����擾���Z�b�g����i�ύX�s�j
            var tmp = NameTag.Split(':');
            var protocolKind = ProtocolKind.Tcp;
            var port = 0;
            var targetServer = "";
            var targetPort = 0;
            if (tmp.Length == 4) {
                //�l�������I�ɐݒ�
                protocolKind = (tmp[0] == "Tunnel-TCP") ? ProtocolKind.Tcp : ProtocolKind.Udp;
                port = Convert.ToInt32(tmp[1]);
                targetServer = tmp[2];
                targetPort = Convert.ToInt32(tmp[3]);
            }
            onePage.Add(CreateServerOption(protocolKind, port, 60, 10)); //�T�[�o��{�ݒ�

            onePage.Add(new OneVal("targetPort", targetPort, Crlf.Nextline, new CtrlInt(IsJp() ? "�ڑ���|�[�g" : "Port", 5)));
            onePage.Add(new OneVal("targetServer", targetServer, Crlf.Nextline, new CtrlTextBox(IsJp() ? "�ڑ���T�[�o" : "Server", 50)));
            onePage.Add(new OneVal("idleTime", 1, Crlf.Nextline, new CtrlInt(IsJp() ? "�A�C�h���^�C��(m)" : "Idle time (m)", 5)));


            return onePage;
        }


        //�R���g���[���̕ω�
        override public void OnChange() {
            var b = (bool)GetCtrl("useServer").Read();
            GetCtrl("tab").SetEnable(b);

            GetCtrl("port").SetEnable(false);// �|�[�g�ԍ� �ύX�s��
            //GetCtrl("protocolKind").SetEnable(false);// �v���g�R�� �ύX�s��
            GetCtrl("targetServer").SetEnable(false);// �ڑ���T�[�o�� �ύX�s��
            GetCtrl("targetPort").SetEnable(false);// �ڑ���|�[�g�ԍ� �ύX�s��
        }
    }
}
