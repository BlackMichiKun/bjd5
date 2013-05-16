using System.Collections.Generic;

using Bjd;
using Bjd.ctrl;
using Bjd.net;
using Bjd.option;

namespace Pop3Server {
    public class Option : OneOption{

        public override string JpMenu { get { return "POP�T�[�o"; } }
        public override string EnMenu { get { return "POP Server"; } }
        public override char Mnemonic { get { return 'P'; } }

        public Option(Kernel kernel, string path, string nameTag)
            : base(kernel.IsJp(), path, nameTag) {

            Add(new OneVal("useServer", false, Crlf.Nextline, new CtrlCheckBox(IsJp() ? "POP3�T�[�o���g�p����" : "Use POP Server")));

            var pageList = new List<OnePage>();
            pageList.Add(Page1("Basic", IsJp() ? "��{�ݒ�" : "Basic"));
            pageList.Add(Page2("Cange Password", IsJp() ? "�p�X���[�h�ύX" : "Cange Password"));
            pageList.Add(Page3("AutoDeny", IsJp() ? "��������" : "AutoDeny"));
            pageList.Add(PageAcl());
            Add(new OneVal("tab", null, Crlf.Nextline, new CtrlTabPage("tabPage", pageList)));

            Read(kernel.IniDb); //�@���W�X�g������̓ǂݍ���
        }
        
        private OnePage Page1(string name, string title) {
            var onePage = new OnePage(name, title);
            onePage.Add(CreateServerOption(ProtocolKind.Tcp, 110, 30, 10)); //�T�[�o��{�ݒ�
            onePage.Add(new OneVal("bannerMessage", "$p (Version $v) ready", Crlf.Nextline, new CtrlTextBox(IsJp() ? "�o�i�[���b�Z�[�W" : "BannerMessage",80)));
            onePage.Add(new OneVal("authType", 0, Crlf.Nextline, new CtrlRadio(IsJp() ? "�F�ؕ���" : "Authorization ", new [] { IsJp() ? "USER/PASS�F��" : "Only USER/PASS", IsJp() ? "APOP�F��" : "Only APOP", IsJp() ? "USER/PASS�y��APOP�F��" : "Bath" },600, 2)));
            onePage.Add(new OneVal("authTimeout", 30, Crlf.Nextline, new CtrlInt(IsJp() ? "�F�؎��s���̃^�C���A�E�g(�b)" : "Timeout in certification failure(sec)",5)));
            return onePage;            
        }

        private OnePage Page2(string name, string title) {
            var onePage = new OnePage(name, title);
                onePage.Add(new OneVal("useChps", false,Crlf.Nextline, new CtrlCheckBox(IsJp() ? "�p�X���[�h�ύX��������" : "Use CHPS")));
                onePage.Add(new OneVal("minimumLength", 8,Crlf.Nextline, new CtrlInt(IsJp() ? "�Œᕶ����" : "admit only a password more then this sharacters", 5)));
                onePage.Add(new OneVal("disableJoe", true,Crlf.Nextline, new CtrlCheckBox(IsJp() ? "���[�U���Ɠ���̃p�X���[�h�������Ȃ�" : "Don't admit password same as a user name")));

                var list = new ListVal();
                list.Add(new OneVal("useNum", true, Crlf.Contonie, new CtrlCheckBox(IsJp() ? "����" : "Number")));
                list.Add(new OneVal("useSmall", true, Crlf.Contonie, new CtrlCheckBox(IsJp() ? "�p������" : "Small")));
                list.Add(new OneVal("useLarge", true, Crlf.Contonie, new CtrlCheckBox(IsJp() ? "�p�啶��" : "Large")));
                list.Add(new OneVal("useSign", true, Crlf.Nextline, new CtrlCheckBox(IsJp() ? "�L��" : "Sign")));
                onePage.Add(new OneVal("groupNeed", null, Crlf.Nextline, new CtrlGroup(IsJp() ? "�K���܂܂Ȃ���΂Ȃ�Ȃ�����" : "A required letter",list)));
            return onePage;            
        }

        private OnePage Page3(string name, string title) {
            var onePage = new OnePage(name, title);
                onePage.Add(new OneVal("useAutoAcl", false, Crlf.Nextline, new CtrlCheckBox(IsJp() ? "�������ۂ��g�p����" : "use automatic deny")));
                onePage.Add(new OneVal("autoAclLabel", IsJp() ? "�uACL�v�ݒ�Łu�w�肷��A�h���X����̃A�N�Z�X�݂̂��v-�u�֎~����v�Ƀ`�F�b�N����Ă���K�v������܂�" : "It is necessary for it to be checked if I [Deny] by [ACL] setting", Crlf.Nextline, new CtrlLabel(IsJp() ? "�uACL�v�ݒ�Łu�w�肷��A�h���X����̃A�N�Z�X�݂̂��v-�u�֎~����v�Ƀ`�F�b�N����Ă���K�v������܂�" : "It is necessary for it to be checked if I [Deny] by [ACL] setting")));
                onePage.Add(new OneVal("autoAclMax", 5, Crlf.Contonie, new CtrlInt(IsJp() ? "�F�؎��s���i��j" : "Continuation failure frequency", 5)));
                onePage.Add(new OneVal("autoAclSec", 60, Crlf.Nextline, new CtrlInt(IsJp() ? "�Ώۊ���(�b)" : "confirmation period(sec)", 5)));
            return onePage;            
        }

        //�R���g���[���̕ω�
        override public void OnChange() {
            var b = (bool)GetCtrl("useServer").Read();
            GetCtrl("tab").SetEnable(b);

            b = (bool)GetCtrl("useChps").Read();
            GetCtrl("minimumLength").SetEnable(b);
            GetCtrl("disableJoe").SetEnable(b);
            GetCtrl("groupNeed").SetEnable(b);

            b = (bool)GetCtrl("useAutoAcl").Read();
            GetCtrl("autoAclLabel").SetEnable(b);
            GetCtrl("autoAclMax").SetEnable(b);
            GetCtrl("autoAclSec").SetEnable(b);
        }

    }
}
