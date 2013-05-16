using System;
using System.Collections.Generic;

using Bjd;
using Bjd.ctrl;
using Bjd.net;
using Bjd.option;

namespace WebServer {
    public class Option : OneOption {

        public override string JpMenu { get { return NameTag; } }
        public override string EnMenu { get { return NameTag; } }
        public override char Mnemonic { get { return '0'; } }

        private Kernel _kernel; //����Web�̏d�������o���邽�ߕK�v�ƂȂ�



        public Option(Kernel kernel, string path, string nameTag)
            : base(kernel.IsJp(), path, nameTag) {

            _kernel = kernel;

            var protocol = 0;//HTTP
            //nameTag����|�[�g�ԍ����擾���Z�b�g����i�ύX�s�j
            var tmp = NameTag.Split(':');
            if (tmp.Length == 2) {
                int port = Convert.ToInt32(tmp[1]);
                protocol = (port == 443) ? 1:0;
            }

            Add(new OneVal("useServer", false, Crlf.Nextline, new CtrlCheckBox(IsJp() ? "Web�T�[�o���g�p����" : "Use Web Server")));

            var pageList = new List<OnePage>();
            pageList.Add(Page1("Basic", IsJp() ? "��{�ݒ�" : "Basic", kernel,protocol));
            pageList.Add(Page2("CGI", "CGI", kernel));
            pageList.Add(Page3("SSI", "SSI", kernel));
            pageList.Add(Page4("WebDAV","WebDAV" , kernel));
            pageList.Add(Page5("Alias", IsJp() ? "�ʖ��w��" : "Alias", kernel));
            pageList.Add(Page6("MimeType", IsJp() ? "MIME�^�C�v" : "MIME Type", kernel));
            pageList.Add(Page7("Certification", IsJp() ? "�F�؃��X�g" : "Certification", kernel));
            pageList.Add(Page8("CertUserList", IsJp() ? "�F�؁i���[�U���X�g�j" : "Certification(User List)", kernel));
            pageList.Add(Page9("CertGroupList", IsJp() ? "�F�؁i�O���[�v���X�g�j" : "Certification(Group List)", kernel));
            pageList.Add(Page10("ModelSentence", IsJp() ? "���^" : "Model Sentence", kernel));
            pageList.Add(Page11("AutoACL", IsJp() ? "��������" : "AutoDeny", kernel));
            pageList.Add(PageAcl());
            Add(new OneVal("tab", null, Crlf.Nextline, new CtrlTabPage("tabPage", pageList)));

            Read(_kernel.IniDb); //�@���W�X�g������̓ǂݍ���
        }

        private OnePage Page1(string name, string title, Kernel kernel,int protocol) {
            var onePage = new OnePage(name, title);

            onePage.Add(new OneVal("protocol", protocol, Crlf.Nextline, new CtrlComboBox(IsJp() ? "�v���g�R��" : "Protocol", new[]{ "HTTP", "HTTPS" },100)));
            
            var port = 80;
            //nameTag����|�[�g�ԍ����擾���Z�b�g����i�ύX�s�j
            var tmp = NameTag.Split(':');
            if (tmp.Length == 2) {
                port = Convert.ToInt32(tmp[1]);
            }
            onePage.Add(CreateServerOption(ProtocolKind.Tcp, port, 3, 10)); //�T�[�o��{�ݒ�

            onePage.Add(new OneVal("documentRoot", "", Crlf.Nextline, new CtrlFolder(IsJp() ? "�h�L�������g�̃��[�g�f�B���N�g��" : "DocumentRoot", 50,kernel)));
            onePage.Add(new OneVal("welcomeFileName", "index.html", Crlf.Nextline, new CtrlTextBox(IsJp() ? "Welcome�t�@�C���̎w��(�J���}�ŋ�؂��ĕ����w��\�ł�)" : "Welcome File", 30)));
            onePage.Add(new OneVal("useHidden", false, Crlf.Nextline, new CtrlCheckBox(IsJp() ? "�B�������t�@�C���փ��N�G�X�g��������" : "Cover it and prohibit a request to a file of attribute")));
            onePage.Add(new OneVal("useDot", false, Crlf.Nextline, new CtrlCheckBox(IsJp() ? "URL��..���܂܂�郊�N�G�X�g��������" : "Prohibit the request that .. is include in")));
            onePage.Add(new OneVal("useExpansion", false, Crlf.Nextline, new CtrlCheckBox(IsJp() ? "BJD���o�R�������N�G�X�g�̓��ʊg����L���ɂ���" : "Use special expansion")));
            onePage.Add(new OneVal("useDirectoryEnum", false, Crlf.Nextline, new CtrlCheckBox(IsJp() ? "�f�B���N�g���ꗗ��\������" : "Display Index")));
            onePage.Add(new OneVal("serverHeader", "BlackJumboDog Version $v", Crlf.Nextline, new CtrlTextBox(IsJp() ? "Server:�w�b�_�̎w��" : "Server Header", 50)));
            onePage.Add(new OneVal("useEtag", false, Crlf.Contonie, new CtrlCheckBox(IsJp() ? "ETag��ǉ�����" : "Use ETag")));
            onePage.Add(new OneVal("serverAdmin", "", Crlf.Contonie, new CtrlTextBox(IsJp() ? "�Ǘ��҃��[���A�h���X" : "server admin", 30)));

            return onePage;
        }

        private OnePage Page2(string name, string title, Kernel kernel) {
            var onePage = new OnePage(name, title);
                onePage.Add(new OneVal("useCgi", false,Crlf.Nextline,new CtrlCheckBox(IsJp() ? "CGI���g�p����" : "Use CGI")));
                {//DAT
                    var l = new ListVal();
                    l.Add(new OneVal("cgiExtension","",Crlf.Contonie,new CtrlTextBox(IsJp() ? "�g���q" : "Extension", 10)));
                    l.Add(new OneVal("Program","",Crlf.Nextline,new CtrlFile(IsJp() ? "�v���O����" : "Program",50,kernel)));
                    onePage.Add(new OneVal("cgiCmd",null,Crlf.Nextline,new CtrlDat("",l,142,IsJp())));
                }//DAT
                onePage.Add(new OneVal("cgiTimeout", 10,Crlf.Nextline,new CtrlInt(IsJp() ? "CGI�^�C���A�E�g(�b)" : "CGI Timeout(sec)", 5)));
                {//DAT
                    var l = new ListVal();
                    l.Add(new OneVal("CgiPath","",Crlf.Nextline,new CtrlTextBox(IsJp() ? "CGI�p�X" : "CGI Path", 50)));
                    l.Add(new OneVal("cgiDirectory","",Crlf.Nextline,new CtrlFolder(IsJp() ? "�Q�ƃf�B���N�g��" : "Directory",60,kernel)));
                    onePage.Add(new OneVal("cgiPath", null,Crlf.Nextline,new CtrlDat(IsJp() ? "CGI�p�X���w�肵���ꍇ�A�w�肵���p�X�̂�CGI��������܂�" : "When I appointed a CGI path It is admitted CGI only the path that I appointed",l,155,IsJp())));
                }//DAT
            return onePage;
        }

        private OnePage Page3(string name, string title, Kernel kernel) {
            var onePage = new OnePage(name, title);
                onePage.Add(new OneVal("useSsi", false,Crlf.Nextline,new CtrlCheckBox(IsJp() ? "SSI���g�p����" : "Use SSI")));
                onePage.Add(new OneVal("ssiExt", "html,htm",Crlf.Nextline,new CtrlTextBox(IsJp() ? "SSI�Ƃ��ĔF������g���q(�J���}�ŋ�؂��ĕ����w��ł��܂�)" : "Extension to recognize as SSI ( Separator , )", 30)));
                onePage.Add(new OneVal("useExec", false,Crlf.Nextline,new CtrlCheckBox(IsJp() ? "exec cmd (cgi) ��L���ɂ���" : "Use exec,cmd(cgi)")));
            return onePage;
        }
        private OnePage Page4(string name, string title, Kernel kernel) {
            var onePage = new OnePage(name, title);
                onePage.Add(new OneVal("useWebDav", false,Crlf.Nextline,new CtrlCheckBox(IsJp() ? "WebDAV���g�p����" : "Use WebDAV")));
                var l = new ListVal();
                    l.Add(new OneVal("WebDAV Path","",Crlf.Nextline,new CtrlTextBox(IsJp() ? "WebDAV�p�X" : "WebDAV Path", 50)));
                    l.Add(new OneVal("Writing permission",false,Crlf.Nextline,new CtrlCheckBox(IsJp() ? "�������݂�������" : "Writing permission")));
                    l.Add(new OneVal("webDAVDirectory", "", Crlf.Nextline, new CtrlFolder(IsJp() ? "�Q�ƃf�B���N�g��" : "Directory", 50, kernel)));
                    onePage.Add(new OneVal("webDavPath", null,Crlf.Nextline,new CtrlDat(IsJp() ? "�w�肵���p�X�ł̂�WevDAV���L���ɂȂ�܂�" : "WebDAV becomes effective only in the path that I appointed",l,280,IsJp())));
            return onePage;
        }
        private OnePage Page5(string name, string title, Kernel kernel) {
            var onePage = new OnePage(name, title);
                    var l = new ListVal();
                    l.Add(new OneVal("aliasName","",Crlf.Nextline,new CtrlTextBox(IsJp() ? "�ʖ�" : "Alias", 30)));
                    l.Add(new OneVal("aliasDirectory","",Crlf.Nextline,new CtrlFolder(IsJp() ? "�Q�ƃf�B���N�g��" : "Directory",50,kernel)));
                    onePage.Add(new OneVal("aliaseList", null,Crlf.Nextline,new CtrlDat(IsJp() ? "�w�肵�����O�i�ʖ��j�Ŏw�肵���f�B���N�g���𒼐ڃA�N�Z�X���܂�" : "Access the directory which I appointed by the name(alias) that I appointed directly",l,250,IsJp())));
            return onePage;
        }
        private OnePage Page6(string name, string title, Kernel kernel) {
            var onePage = new OnePage(name, title);
                    var l = new ListVal();
                    l.Add(new OneVal("mimeExtension","",Crlf.Nextline,new CtrlTextBox(IsJp() ? "�g���q" : "Extension", 10)));
                    l.Add(new OneVal("mimeType","",Crlf.Nextline,new CtrlTextBox(IsJp() ? "MIME�^�C�v" : "MIME Type", 50)));
                    onePage.Add(new OneVal("mime", null,Crlf.Nextline,new CtrlDat(IsJp() ? "�f�[�^�`�����w�肷�邽�߂́A�uMIME�^�C�v�v�̃��X�g��ݒ肵�܂�" : "Set a MIME Type list in order to appoint data form",l,350,IsJp())));
            return onePage;
        }
        private OnePage Page7(string name, string title, Kernel kernel) {
            var onePage = new OnePage(name, title);
                    var l = new ListVal();
                    l.Add(new OneVal("authDirectory","",Crlf.Nextline,new CtrlTextBox(IsJp() ? "URL (Directory)" : "Directory", 50)));
                    l.Add(new OneVal("AuthName","",Crlf.Nextline,new CtrlTextBox(IsJp() ? "���O (AuthName)" : "AuthName", 20)));
                    l.Add(new OneVal("Require","",Crlf.Nextline,new CtrlTextBox(IsJp() ? "���[�U/�O���[�v (Require)" : "Require", 30)));
                    onePage.Add(new OneVal("authList", null,Crlf.Nextline,new CtrlDat(IsJp() ? "���[�U/�O���[�v�́u;�v�ŋ�؂��ĕ����ݒ�ł��܂�" : "divide it in [;], and plural [Require] can appoint it",l,350,IsJp())));
            return onePage;
        }
        private OnePage Page8(string name, string title, Kernel kernel) {
            var onePage = new OnePage(name, title);
                    var l = new ListVal();
                    l.Add(new OneVal("user","",Crlf.Nextline,new CtrlTextBox(IsJp() ? "���[�U (user)" : "user", 20)));
                    l.Add(new OneVal("pass","",Crlf.Nextline,new CtrlHidden(IsJp() ? "�p�X���[�h (password)" : "password", 20)));
                    onePage.Add(new OneVal("userList", null,Crlf.Nextline,new CtrlDat(IsJp() ? "���[�U��`" : "User List",l,350,IsJp())));
            return onePage;
        }
        private OnePage Page9(string name, string title, Kernel kernel) {
            var onePage = new OnePage(name, title);
                    var l = new ListVal();
                    l.Add(new OneVal("group","",Crlf.Nextline,new CtrlTextBox(IsJp() ? "�O���[�v (group)" : "group", 20)));
                    l.Add(new OneVal("userName","",Crlf.Nextline, new CtrlTextBox(IsJp() ? "���[�U(user)" : "user", 40)));
                    onePage.Add(new OneVal("groupList", null,Crlf.Nextline,new CtrlDat(IsJp() ? "���[�U�́u;�v�ŋ�؂��ĕ����ݒ�ł��܂�" : "divide it in [;], and plural [user] can appoint it",l,350,IsJp())));
            return onePage;
        }
        private OnePage Page10(string name, string title, Kernel kernel) {
            var onePage = new OnePage(name, title);
                onePage.Add(new OneVal("encode", 0,Crlf.Nextline,new CtrlComboBox(IsJp() ? "�G���R�[�h" : "Encode",new []{"UTF-8", "SHIFT-JIS", "EUC"},100)));
                onePage.Add(new OneVal("indexDocument", "",Crlf.Nextline,new CtrlMemo(IsJp() ? "�C���f�b�N�X�h�L�������g" : "Index Document", OptionDlg.Width()-15, 145)));
                onePage.Add(new OneVal("errorDocument", "", Crlf.Nextline, new CtrlMemo(IsJp() ? "�G���[�h�L�������g" : "Error Document", OptionDlg.Width() - 15, 145)));
            return onePage;
        }
        private OnePage Page11(string name, string title, Kernel kernel){
            var onePage = new OnePage(name, title);
            onePage.Add(new OneVal("useAutoAcl", false, Crlf.Nextline,
                                   new CtrlCheckBox(IsJp() ? "�������ۂ��g�p����" : "use automatic deny")));
            onePage.Add(new OneVal("autoAclLabel",
                                   IsJp()
                                       ? "�uACL�v�ݒ�Łu�w�肷��A�h���X����̃A�N�Z�X�݂̂��v-�u�֎~����v�Ƀ`�F�b�N����Ă���K�v������܂�"
                                       : "It is necessary for it to be checked if I [Deny] by [ACL] setting",
                                   Crlf.Nextline,
                                   new CtrlLabel(IsJp()
                                                     ? "�uACL�v�ݒ�Łu�w�肷��A�h���X����̃A�N�Z�X�݂̂��v-�u�֎~����v�Ƀ`�F�b�N����Ă���K�v������܂�"
                                                     : "It is necessary for it to be checked if I [Deny] by [ACL] setting")));
            var l = new ListVal();
            l.Add(new OneVal("AutoAclApacheKiller", false, Crlf.Nextline,
                             new CtrlCheckBox(IsJp() ? "Apache Killer �̌��o" : "Search of Apache Killer")));
            onePage.Add(new OneVal("autoAclGroup", null, Crlf.Nextline,
                                   new CtrlGroup(IsJp() ? "���ۃ��X�g�ɒǉ�����C�x���g" : "Target Event", l)));
            return onePage;
        }

        //�R���g���[���̕ω�
        override public void OnChange(){


            var b = (bool) GetCtrl("useServer").Read();
            GetCtrl("tab").SetEnable(b);

            GetCtrl("protocol").SetEnable(false);
            GetCtrl("port").SetEnable(false);

            b = (bool) GetCtrl("useCgi").Read();
            GetCtrl("cgiCmd").SetEnable(b);
            GetCtrl("cgiTimeout").SetEnable(b);
            GetCtrl("cgiPath").SetEnable(b);

            b = (bool) GetCtrl("useSsi").Read();
            GetCtrl("ssiExt").SetEnable(b);
            GetCtrl("useExec").SetEnable(b);

            b = (bool) GetCtrl("useWebDav").Read();
            GetCtrl("webDavPath").SetEnable(b);

            ////����|�[�g�ő҂��󂯂鉼�z�T�[�o�̓����ڑ����́A�ŏ��̒�`�����̂܂܎g�p����
            //var port = (int)GetValue("port");
            //foreach (var o in Kernel.ListOption){
            //    if (o.NameTag.IndexOf("Web-") != 0)
            //        continue;
            //    if (port != (int) o.GetValue("port"))
            //        continue;
            //    if (o == this)
            //        continue;
            //    //���̃I�v�V�����ȊO�̍ŏ��̒�`�𔭌������ꍇ
            //    var multiple = (int)o.GetValue("multiple");
            //    SetVal("multiple", multiple);
            //    GetCtrl("multiple").SetEnable(false);
            //   break;
            //}
            //����|�[�g�̉��z�T�[�o�̃��X�g���쐬����
            var ar = new List<OneOption>();
            var port = (int) GetValue("port");
            foreach (var o in _kernel.ListOption){
                if (o.NameTag.IndexOf("Web-") != 0)
                    continue;
                if (port != (int) o.GetValue("port"))
                    continue;
                if (!o.UseServer){
                    //�g�p���Ă��Ȃ��T�[�o�͑ΏۊO
                    continue;
                }
                ar.Add(o);
            }
            //����|�[�g�̉��z�T�[�o����������ꍇ
            if (ar.Count > 1){
                //�ŏ��̒�`�ȊO�́A�����ڑ�����ݒ�ł��Ȃ�����
                if (ar[0] != this){
                    var multiple = (int) ar[0].GetValue("multiple");
                    SetVal(_kernel.IniDb,"multiple", multiple);
                    GetCtrl("multiple").SetEnable(false);
                }
            }

            b = (bool) GetCtrl("useAutoAcl").Read();
            GetCtrl("autoAclLabel").SetEnable(b);
            GetCtrl("autoAclGroup").SetEnable(b);

        }
    }
}
