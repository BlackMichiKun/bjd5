using System.Collections.Generic;
using Bjd;
using Bjd.ctrl;
using Bjd.net;
using Bjd.option;

namespace RemoteServer {
    class Option : OneOption {

        public override string JpMenu { get { return "�����[�g����"; } }
        public override string EnMenu { get { return "Remote Server"; } }
        public override char Mnemonic { get { return 'R'; } }

        public Option(Kernel kernel, string path, string nameTag)
            : base(kernel.IsJp(), path, nameTag) {

            Add(new OneVal("useServer", false, Crlf.Nextline, new CtrlCheckBox(IsJp() ? "�����[�g������g�p����" : "Use Remote Server")));

            var pageList = new List<OnePage>();
            pageList.Add(Page1("Basic", IsJp() ? "��{�ݒ�" : "Basic", kernel));
            pageList.Add(PageAcl());
            Add(new OneVal("tab", null, Crlf.Nextline, new CtrlTabPage("tabPage", pageList)));

            Read(kernel.IniDb); //�@���W�X�g������̓ǂݍ���
        }

        private OnePage Page1(string name, string title, Kernel kernel) {
            var onePage = new OnePage(name, title);

            onePage.Add(CreateServerOption(ProtocolKind.Tcp, 10001, 60, 1)); //�T�[�o��{�ݒ�

            onePage.Add(new OneVal("password", "", Crlf.Nextline, new CtrlHidden(IsJp() ? "�p�X���[�h" : "Password", 20)));
            return onePage;
        }

        //�R���g���[���̕ω�
        override public void OnChange() {

            var b = (bool)GetCtrl("useServer").Read();
            GetCtrl("tab").SetEnable(b);

            GetCtrl("multiple").SetEnable(false);// �����ڑ��� �ύX�s��
        }
    }
}
