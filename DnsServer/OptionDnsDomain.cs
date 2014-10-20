using System.Collections.Generic;
using Bjd;
using Bjd.ctrl;
using Bjd.option;

namespace DnsServer {
    public class OptionDnsDomain : OneOption{

        public override string JpMenu { get { return "�h���C���̒ǉ��ƍ폜"; } }
        public override string EnMenu { get { return "Add or Remove Domains"; } }
        public override char Mnemonic { get { return 'A'; } }

        public OptionDnsDomain(Kernel kernel, string path, string nameTag)
            : base(kernel.IsJp(), path, nameTag){

            var pageList = new List<OnePage>();
            pageList.Add(Page1("Basic", IsJp() ? "�h���C��" : "Domain",kernel));
            Add(new OneVal("tab", null, Crlf.Nextline, new CtrlTabPage("tabPage", pageList)));

            Read(kernel.IniDb); //�@���W�X�g������̓ǂݍ���
        }

        private OnePage Page1(string name, string title,Kernel kernel) {
            var onePage = new OnePage(name, title);
            var list = new ListVal();
            list.Add(new OneVal("name", "", Crlf.Nextline, new CtrlTextBox(IsJp() ? "�h���C����" : "Domain Name", 80)));
            list.Add(new OneVal("authority", true, Crlf.Nextline, new CtrlCheckBox(IsJp() ? "�I�[�\���e�B ( ���̃`�F�b�N�������ꍇ�A������Ȃ����\�[�X���ċA�������܂�)" : "Authority")));
            onePage.Add(new OneVal("domainList", null, Crlf.Nextline, new CtrlDat("", list, 400, IsJp())));
            return onePage;
        }
    }
}
