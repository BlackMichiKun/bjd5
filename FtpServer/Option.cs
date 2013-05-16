using System.Collections.Generic;

using Bjd;
using Bjd.ctrl;
using Bjd.net;
using Bjd.option;

namespace FtpServer {
    public class Option : OneOption {

        public override string JpMenu { get { return "FTP�T�[�o"; } }
        public override string EnMenu { get { return "FTP Server"; } }
        public override char Mnemonic { get { return 'F'; } }


        public Option(Kernel kernel, string path, string nameTag)
            : base(kernel.IsJp(), path, nameTag){

            Add(new OneVal("useServer", false, Crlf.Nextline, new CtrlCheckBox((IsJp()) ? "FTP�T�[�o���g�p����" : "Use FTP Server")));

            var pageList = new List<OnePage>();
            pageList.Add(Page1("Basic", IsJp() ? "��{�ݒ�" : "Basic",kernel));
            pageList.Add(Page2("VirtualFolder", IsJp() ? "���z�t�H���_" : "VirtualFolder",kernel));
            pageList.Add(Page3("User", IsJp() ? "���p��" : "User", kernel));
            pageList.Add(PageAcl());
            Add(new OneVal("tab", null, Crlf.Nextline, new CtrlTabPage("tabPage", pageList)));

            Read(kernel.IniDb); //�@���W�X�g������̓ǂݍ���
        }

        private OnePage Page1(string name, string title,Kernel kernel) {
            var onePage = new OnePage(name, title);

            onePage.Add(CreateServerOption(ProtocolKind.Tcp, 21, 30, 50)); //�T�[�o��{�ݒ�

            onePage.Add(new OneVal("bannerMessage", "FTP ( $p Version $v ) ready", Crlf.Nextline, new CtrlTextBox((IsJp()) ? "�o�i�[���b�Z�[�W" : "Banner Message", 80)));
            //���C�u�h�A���ʎd�l
            //onePage.Add(new OneVal(new ValType(CRLF.NEXTLINE, VTYPE.FILE, (IsJp()) ? "�t�@�C����M���ɋN������X�N���v�g" : "auto run acript", 250,kernel), "autoRunScript","c:\\test.bat"));
            onePage.Add(new OneVal("useSyst", false, Crlf.Nextline, new CtrlCheckBox(IsJp() ? "SYST�R�}���h��L���ɂ��� ( �Z�L�����e�B���X�N�̍����I�v�V�����ł��B�K�v�̂Ȃ�����`�F�b�N���Ȃ��ł��������B)" : "Validate a SYST command")));
            onePage.Add(new OneVal("reservationTime", 5000, Crlf.Nextline, new CtrlInt(IsJp() ? "�F�؎��s���ۗ̕�����(�~���b)" : "Reservation time in certification failure (msec)", 6)));
            return onePage;            
        }
        private OnePage Page2(string name, string title, Kernel kernel) {
            var onePage = new OnePage(name, title);
            var listVal = new ListVal();
            listVal.Add(new OneVal("fromFolder", "", Crlf.Nextline, new CtrlFolder(IsJp() ? "���t�H���_" : "Real Folder", 70, kernel)));
            listVal.Add(new OneVal("toFolder", "", Crlf.Nextline, new CtrlFolder(IsJp() ? "�}�E���g��" : "Mount Folder", 70, kernel)));
            onePage.Add(new OneVal("mountList", null, Crlf.Nextline, new CtrlDat(IsJp() ? "�}�E���g�̎w��" : "Mount List", listVal, 360, IsJp())));
            return onePage;
        }
        private OnePage Page3(string name, string title, Kernel kernel) {
            var onePage = new OnePage(name, title);
            var listVal = new ListVal();
            listVal.Add(new OneVal("accessControl", 0, Crlf.Nextline, new CtrlComboBox(IsJp() ? "�A�N�Z�X����" : "Access Control", new []{ "FULL", "DOWN", "UP" },100)));
            listVal.Add(new OneVal("homeDirectory", "", Crlf.Nextline, new CtrlFolder(IsJp() ? "�z�[���f�B���N�g��" : "Home Derectory", 60, kernel)));
            listVal.Add(new OneVal("userName", "", Crlf.Nextline, new CtrlTextBox(IsJp() ? "���[�U��" : "User Name", 20)));
            listVal.Add(new OneVal("password", "", Crlf.Nextline, new CtrlHidden(IsJp() ? "�p�X���[�h" : "Password", 20)));
            onePage.Add(new OneVal("user", null, Crlf.Nextline, new CtrlDat(IsJp() ? "���p�ҁi�A�N�Z�X���j�̎w��" : "User List", listVal,360, IsJp())));
            return onePage;
        }

        //�R���g���[���̕ω�
        override public void OnChange() {

            // �|�[�g�ԍ��ύX�֎~
            GetCtrl("port").SetEnable(false);

            var b = (bool)GetCtrl("useServer").Read();
            GetCtrl("tab").SetEnable(b);
        }
    }
}
