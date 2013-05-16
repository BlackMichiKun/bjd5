using Bjd;
using Bjd.ctrl;
using Bjd.net;
using Bjd.option;
using System.Collections.Generic;

namespace ProxyTelnetServer {
    class Option : OneOption {

        public override string JpMenu { get { return "Telnet"; } }
        public override string EnMenu { get { return "Telnet"; } }
        public override char Mnemonic { get { return 'T'; } }


        public Option(Kernel kernel, string path, string nameTag)
            : base(kernel.IsJp(), path, nameTag) {

            Add(new OneVal("useServer", false, Crlf.Nextline, new CtrlCheckBox(IsJp() ? "�v���L�V�T�[�o[Telnet]���g�p����" : "Use Proxy Server [Telnet]")));

            var pageList = new List<OnePage>();
            pageList.Add(Page1("Basic", IsJp() ? "��{�ݒ�" : "Basic", kernel));
            pageList.Add(PageAcl());
            Add(new OneVal("tab", null, Crlf.Nextline, new CtrlTabPage("tabPage", pageList)));

            Read(kernel.IniDb); //�@���W�X�g������̓ǂݍ���
        }
        
        private OnePage Page1(string name, string title, Kernel kernel) {
            var onePage = new OnePage(name, title);

            onePage.Add(CreateServerOption(ProtocolKind.Tcp, 8023, 60, 10)); //�T�[�o��{�ݒ�

            onePage.Add(new OneVal("idleTime", 1, Crlf.Contonie, new CtrlInt(IsJp() ? "�A�C�h���^�C��(m)" : "Idle Timeout(sec)", 5)));


            return onePage;
        }

        //�R���g���[���̕ω�
        override public void OnChange() {
            var b = (bool)GetCtrl("useServer").Read();
            GetCtrl("tab").SetEnable(b);

        }
    }
}
