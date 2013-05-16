using System.Collections.Generic;

using Bjd;
using Bjd.ctrl;
using Bjd.option;

namespace TunnelServer {
    internal class OptionTunnel : OneOption {

        public override string JpMenu { get { return "�g���l���̒ǉ��ƍ폜"; } }
        public override string EnMenu { get { return "Add or Remove Tunnel"; } }
        public override char Mnemonic { get { return 'A'; } }

        public OptionTunnel(Kernel kernel, string path, string nameTag)
            : base(kernel.IsJp(), path, nameTag){

            var pageList = new List<OnePage>();
            pageList.Add(Page1("Basic", IsJp() ? "��{�ݒ�" : "Basic",kernel));
            pageList.Add(PageAcl());
            Add(new OneVal("tab", null, Crlf.Nextline, new CtrlTabPage("tabPage", pageList)));

            Read(kernel.IniDb); //�@���W�X�g������̓ǂݍ���
        }
        
        private OnePage Page1(string name, string title, Kernel kernel) {
            var onePage = new OnePage(name, title);

            var l = new ListVal();
            l.Add(new OneVal("protocol", 0, Crlf.Nextline, new CtrlComboBox(IsJp() ? "�v���g�R��" : "Protocol", new[] { "TCP", "UDP" }, 100)));
            l.Add(new OneVal("srcPort", 0, Crlf.Nextline, new CtrlInt(IsJp() ? "�N���C�A���g���猩���|�[�g" : "Port (from client side)", 5)));
            l.Add(new OneVal("server", "", Crlf.Nextline, new CtrlTextBox(IsJp() ? "�ڑ���T�[�o��" : "Connection ahead server", 30)));
            l.Add(new OneVal("dstPort", 0, Crlf.Nextline, new CtrlInt(IsJp() ? "�ڑ���|�[�g" : "Port (to server side)", 5)));
            onePage.Add(new OneVal("tunnelList", null, Crlf.Nextline, new CtrlDat("", l, 380, IsJp())));

            return onePage;
        }

        //�R���g���[���̕ω�
        override public void OnChange() {

        }
    }
}
