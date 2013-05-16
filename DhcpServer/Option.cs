
using Bjd;
using Bjd.ctrl;
using Bjd.net;
using Bjd.option;
using System.Collections.Generic;

namespace DhcpServer {
    class Option : OneOption {
        public override string JpMenu { get { return "DHCP�T�[�o"; } }
        public override string EnMenu { get { return "DHCP Server"; } }

        public override char Mnemonic{ get { return  'H'; }}
       
        public Option(Kernel kernel, string path, string nameTag)
            : base(kernel.IsJp(), path, nameTag) {

            Add(new OneVal("useServer", false, Crlf.Nextline, new CtrlCheckBox((IsJp()) ? "DHCP�T�[�o���g�p����" : "Use DHCP Server")));

            var pageList = new List<OnePage>();
            pageList.Add(Page1("Basic", IsJp() ? "��{�ݒ�" : "Basic", kernel));
            pageList.Add(Page2("Acl","ACL(MAC)", kernel));
            Add(new OneVal("tab", null, Crlf.Nextline, new CtrlTabPage("tabPage", pageList)));

            Read(kernel.IniDb); //�@���W�X�g������̓ǂݍ���
        }

        private OnePage Page1(string name, string title, Kernel kernel){
            var onePage = new OnePage(name, title);

            onePage.Add(CreateServerOption(ProtocolKind.Udp, 67, 10, 10)); //�T�[�o��{�ݒ�


            onePage.Add(new OneVal("leaseTime", 18000, Crlf.Nextline,new CtrlInt(IsJp() ? "���[�X����(�b)" : "Lease Time(sec)", 8)));
            onePage.Add(new OneVal("startIp", new Ip(IpKind.V4_0), Crlf.Nextline,new CtrlAddress(IsJp() ? "�J�n�A�h���X  �@ " : "Start Address")));
            onePage.Add(new OneVal("endIp", new Ip(IpKind.V4_0), Crlf.Nextline,new CtrlAddress(IsJp() ? "�I���A�h���X �@  " : "End Address")));
            onePage.Add(new OneVal("maskIp", new Ip("255.255.255.0"), Crlf.Nextline,new CtrlAddress(IsJp() ? "�T�u�l�b�g�}�X�N  " : "Subnet Mask")));
            onePage.Add(new OneVal("gwIp", new Ip(IpKind.V4_0), Crlf.Nextline,new CtrlAddress(IsJp() ? "�Q�[�g�E�G�C    " : "Gateway")));
            onePage.Add(new OneVal("dnsIp0", new Ip(IpKind.V4_0), Crlf.Nextline,new CtrlAddress(IsJp() ? "DNS�i�v���C�}��)" : "DNS(Primary)")));
            onePage.Add(new OneVal("dnsIp1", new Ip(IpKind.V4_0), Crlf.Nextline,new CtrlAddress(IsJp() ? "DNS�i�Z�J���_��)" : "DNS(Secondary)")));
            onePage.Add(new OneVal("useWpad", false, Crlf.Contonie,new CtrlCheckBox(IsJp()
                                                       ? "WPAD(Web Proxy Auto-Discovery Protocol)���g�p����"
                                                       : "use WPAD(Web Proxy Auto-Discovery Protocol)")));
            onePage.Add(new OneVal("wpadUrl", "http://", Crlf.Nextline, new CtrlTextBox("URL", 37)));

            return onePage;
        }

        private OnePage Page2(string name, string title, Kernel kernel) {
            var onePage = new OnePage(name, title);

                onePage.Add(new OneVal("useMacAcl", false, Crlf.Nextline, new CtrlCheckBox(IsJp() ? "MAC�A�h���X�ɂ�鐧���i�L���ɂ���Ɠo�^����MAC�A�h���X�̂ݎg�p�\�ɂȂ�܂��j" : "limit by a MAC address (it is OK only the person who registered itself)")));

                var l = new ListVal();
                l.Add(new OneVal("macAddress", "", Crlf.Nextline, new CtrlTextBox(IsJp() ? "MAC�A�h���X(99-99-99-99-99-99)" : "MAC Address(99-99-99-99-99-99)", 50)));
                l.Add(new OneVal("v4Address", new Ip(IpKind.V4_0), Crlf.Nextline, new CtrlAddress(IsJp() ? "IP�A�h���X" : "IP Address")));
                l.Add(new OneVal("macName", "", Crlf.Nextline, new CtrlTextBox(IsJp() ? "���O�i�\�����j" : "Name(Display)", 50)));
                onePage.Add(new OneVal("macAcl", null, Crlf.Nextline, new CtrlDat(IsJp() ? "IP�A�h���X�Ɂu255.255.255.255�v�w�肵���ꍇ�A��{�ݒ�Ŏw�肵���͈͂��烉���_���Ɋ��蓖�Ă��܂�" : "When appointed 255.255.255.255 to IP Address, basic setting is used", l, 250, IsJp())));

            return onePage;
        }

        //�R���g���[���̕ω�
        override public void OnChange() {
            // �|�[�g�ԍ��ύX�֎~
            GetCtrl("port").SetEnable(false);

            var b = (bool)GetCtrl("useServer").Read();
            GetCtrl("tab").SetEnable(b);
            b = (bool)GetCtrl("useWpad").Read();
            GetCtrl("wpadUrl").SetEnable(b);

        }
    }

}
