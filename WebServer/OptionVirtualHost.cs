using System.Collections.Generic;
using Bjd;
using Bjd.ctrl;
using Bjd.option;

namespace WebServer {
    public class OptionVirtualHost : OneOption {
        public override string JpMenu { get { return "Web�̒ǉ��ƍ폜"; } }
        public override string EnMenu { get { return "Add or Remove VirtualHost"; } }
        public override char Mnemonic { get { return 'A'; } }



        public OptionVirtualHost(Kernel kernel, string path, string nameTag)
            : base(kernel.IsJp(), path, nameTag){

            var pageList = new List<OnePage>();
            pageList.Add(Page1("VirtualHost", IsJp() ? "���z�z�X�g" : "Virtual Host", kernel));
            Add(new OneVal("tab", null, Crlf.Nextline, new CtrlTabPage("tabPage", pageList)));

            Read(kernel.IniDb); //�@���W�X�g������̓ǂݍ���
        }

        private OnePage Page1(string name, string title, Kernel kernel){
            var onePage = new OnePage(name, title);
            var list1 = new ListVal();
            list1.Add(new OneVal("protocol", 0, Crlf.Nextline,
                                 new CtrlComboBox(IsJp() ? "�v���g�R��" : "Protocol", new[]{"HTTP", "HTTPS"}, 100)));
            list1.Add(new OneVal("host", "", Crlf.Contonie, new CtrlTextBox(IsJp() ? "�z�X�g��" : "Host Name", 30)));
            list1.Add(new OneVal("port", 80, Crlf.Nextline, new CtrlInt(IsJp() ? "�|�[�g�ԍ�" : "Port", 5)));
            onePage.Add(new OneVal("hostList", null, Crlf.Nextline, new CtrlOrgDat("", list1, 600, 270, kernel.IsJp())));
            var list2 = new ListVal();
            list2.Add(new OneVal("certificate", "", Crlf.Nextline,
                                 new CtrlFile(IsJp() ? "�T�C�g�ؖ���(.ptx)" : "site certificate(.ptx)", 50,kernel)));
            list2.Add(new OneVal("privateKeyPassword", "", Crlf.Nextline,
                                 new CtrlHidden(IsJp() ? "�閧���̃p�X���[�h" : "A password of private key", 20)));
            onePage.Add(new OneVal("groupHttps", null, Crlf.Nextline,
                                   new CtrlGroup(
                                       IsJp()
                                           ? "HTTPS���g�p����ꍇ�́A�ؖ���(pfx�`��)���K�v�ł�"
                                           : "When they use HTTPS, a certificate is necessary", list2)));

            return onePage;
        }
        //�R���g���[���̕ω�
        override public void OnChange() {
        }
    }
}
