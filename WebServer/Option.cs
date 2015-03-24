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
            var key = "useServer";
            Add(new OneVal(key, false, Crlf.Nextline, new CtrlCheckBox(Lang.Value(key))));

            var pageList = new List<OnePage>();
            key = "Basic";
            pageList.Add(Page1(key, Lang.Value(key), kernel, protocol));
            pageList.Add(Page2("CGI", "CGI", kernel));
            pageList.Add(Page3("SSI", "SSI", kernel));
            pageList.Add(Page4("WebDAV","WebDAV" , kernel));
            key = "Alias";
            pageList.Add(Page5(key, Lang.Value(key), kernel));
            key = "MimeType";
            pageList.Add(Page6(key, Lang.Value(key), kernel));
            key = "Certification";
            pageList.Add(Page7(key, Lang.Value(key), kernel));
            key = "CertUserList";
            pageList.Add(Page8(key, Lang.Value(key), kernel));
            key = "CertGroupList";
            pageList.Add(Page9(key, Lang.Value(key), kernel));
            key = "ModelSentence";
            pageList.Add(Page10(key, Lang.Value(key), kernel));
            key = "AutoACL";
            pageList.Add(Page11(key, Lang.Value(key), kernel));
            pageList.Add(PageAcl());
            Add(new OneVal("tab", null, Crlf.Nextline, new CtrlTabPage("tabPage", pageList)));

            Read(_kernel.IniDb); //�@���W�X�g������̓ǂݍ���
        }

        private OnePage Page1(string name, string title, Kernel kernel,int protocol) {
            var onePage = new OnePage(name, title);

            var key = "protocol";
            onePage.Add(new OneVal(key, protocol, Crlf.Nextline, new CtrlComboBox(Lang.Value(key), new[] { "HTTP", "HTTPS" }, 100)));
            
            var port = 80;
            //nameTag����|�[�g�ԍ����擾���Z�b�g����i�ύX�s�j
            var tmp = NameTag.Split(':');
            if (tmp.Length == 2) {
                port = Convert.ToInt32(tmp[1]);
            }
            onePage.Add(CreateServerOption(ProtocolKind.Tcp, port, 3, 10)); //�T�[�o��{�ݒ�

            key = "documentRoot";
            onePage.Add(new OneVal(key, "", Crlf.Nextline, new CtrlFolder(Lang.Value(key), 50, kernel)));
            key = "welcomeFileName";
            onePage.Add(new OneVal(key, "index.html", Crlf.Nextline, new CtrlTextBox(Lang.Value(key), 30)));
            key = "useHidden";
            onePage.Add(new OneVal(key, false, Crlf.Nextline, new CtrlCheckBox(Lang.Value(key))));
            key = "useDot";
            onePage.Add(new OneVal(key, false, Crlf.Nextline, new CtrlCheckBox(Lang.Value(key))));
            key = "useExpansion";
            onePage.Add(new OneVal(key, false, Crlf.Nextline, new CtrlCheckBox(Lang.Value(key))));
            key = "useDirectoryEnum";
            onePage.Add(new OneVal(key, false, Crlf.Nextline, new CtrlCheckBox(Lang.Value(key))));
            key = "serverHeader";
            onePage.Add(new OneVal(key, "BlackJumboDog Version $v", Crlf.Nextline, new CtrlTextBox(Lang.Value(key), 50)));
            key = "useEtag";
            onePage.Add(new OneVal(key, false, Crlf.Contonie, new CtrlCheckBox(Lang.Value(key))));
            key = "serverAdmin";
            onePage.Add(new OneVal(key, "", Crlf.Contonie, new CtrlTextBox(Lang.Value(key), 30)));

            return onePage;
        }

        private OnePage Page2(string name, string title, Kernel kernel) {
            var onePage = new OnePage(name, title);
            var key = "useCgi";
            onePage.Add(new OneVal(key, false, Crlf.Nextline, new CtrlCheckBox(Lang.Value(key))));
                {//DAT
                    var l = new ListVal();
                    key = "cgiExtension";
                    l.Add(new OneVal(key, "", Crlf.Contonie, new CtrlTextBox(Lang.Value(key), 10)));
                    key = "Program";
                    l.Add(new OneVal(key, "", Crlf.Nextline, new CtrlFile(Lang.Value(key), 50, kernel)));
                    onePage.Add(new OneVal("cgiCmd",null,Crlf.Nextline,new CtrlDat("",l,142,IsJp())));
                }//DAT
                key = "cgiTimeout";
                onePage.Add(new OneVal(key, 10, Crlf.Nextline, new CtrlInt(Lang.Value(key), 5)));
                {//DAT
                    var l = new ListVal();
                    key = "CgiPath";
                    l.Add(new OneVal(key, "", Crlf.Nextline, new CtrlTextBox(Lang.Value(key), 50)));
                    key = "cgiDirectory";
                    l.Add(new OneVal(key, "", Crlf.Nextline, new CtrlFolder(Lang.Value(key), 60, kernel)));
                    key = "cgiPath";
                    onePage.Add(new OneVal(key, null, Crlf.Nextline, new CtrlDat(Lang.Value(key), l, 155, IsJp())));
                }//DAT
            return onePage;
        }

        private OnePage Page3(string name, string title, Kernel kernel) {
            var onePage = new OnePage(name, title);
            var key = "useSsi";
            onePage.Add(new OneVal(key, false, Crlf.Nextline, new CtrlCheckBox(Lang.Value(key))));
                key = "ssiExt";
                onePage.Add(new OneVal(key, "html,htm",Crlf.Nextline,new CtrlTextBox(Lang.Value(key), 30)));
                key = "useExec";
                onePage.Add(new OneVal(key, false, Crlf.Nextline, new CtrlCheckBox(Lang.Value(key))));
            return onePage;
        }
        private OnePage Page4(string name, string title, Kernel kernel) {
            var onePage = new OnePage(name, title);
            var key = "useWebDav";
            onePage.Add(new OneVal(key, false, Crlf.Nextline, new CtrlCheckBox(Lang.Value(key))));
                var l = new ListVal();
                key = "WebDAV Path";
                l.Add(new OneVal(key, "", Crlf.Nextline, new CtrlTextBox(Lang.Value(key), 50)));
                key = "Writing permission";
                l.Add(new OneVal(key, false, Crlf.Nextline, new CtrlCheckBox(Lang.Value(key))));
                key = "webDAVDirectory";
                l.Add(new OneVal(key, "", Crlf.Nextline, new CtrlFolder(Lang.Value(key), 50, kernel)));
                key = "webDavPath";
                onePage.Add(new OneVal(key, null, Crlf.Nextline, new CtrlDat(Lang.Value(key), l, 280, IsJp())));
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
