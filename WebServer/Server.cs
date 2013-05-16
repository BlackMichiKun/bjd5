using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Globalization;

using Bjd;
using Bjd.acl;
using Bjd.log;
using Bjd.net;
using Bjd.option;
using Bjd.server;
using Bjd.sock;
using Bjd.util;

namespace WebServer {
    partial class Server : OneServer {
        readonly AttackDb _attackDb;//��������
        private Ssl _ssl = null;


        //�ʏ�͊e�|�[�g���ƂP��ނ̃T�[�o���N������̂�ServerTread.option ���g�p���邪�A
        //�o�[�`�F���z�X�g�̏ꍇ�A�P�̃|�[�g�ŕ����̃T�[�o���N������̂ŃI�v�V�������X�g�iwebOptionList�j
        //����K�؂Ȃ��̂�I�����AopBase�ɃR�s�[���Ďg�p����
        //_subThread���Ăяo�����܂ł́A�|�[�g�ԍ��̑�\�ł��� ServerThread.option �iwebOptionList[0]�Ɠ��� �j���g�p����Ă���
        //Ver5.1.4
        readonly List<WebDavDb> _webDavDbList = new List<WebDavDb>();
        WebDavDb _webDavDb;//WevDAV��Deth�v���p�e�C���Ǘ�����N���X
        
        protected List<OneOption> WebOptionList = null;

        //�ʏ��ServerThread�̎q�N���X�ƈႢ�A�I�v�V�����̓��X�g�Ŏ󂯎��
        //�e�N���X�́A���̃��X�g��0�Ԗڂ̃I�u�W�F�N�g�ŏ���������

        //�R���X�g���N�^
        public Server(Kernel kernel,Conf conf,OneBind oneBind)
            : base(kernel, conf,oneBind) {

            //����|�[�g�ő҂��󂯂Ă��鉼�z�T�[�o�̃I�v�V���������ׂă��X�g����
            WebOptionList = new List<OneOption>();
            foreach (var o in kernel.ListOption) {
                if (o.NameTag.IndexOf("Web-") == 0) {
                    if((int)o.GetValue("port") == (int)Conf.Get("port")){
                        WebOptionList.Add(o);
                    }
                }
            }
            //WebDAV���X�g�̏�����
            foreach (var o in WebOptionList) {
                if(o.UseServer) {
                    _webDavDbList.Add(new WebDavDb(kernel,NameTag));
                }
            }
            _webDavDb = _webDavDbList[0];
            
            //Ver5.1.2�uCgi�p�X�v�uWebDAV�p�X�v�u�ʖ��v�̃I�v�V�����̏C��
            var tagList = new List<string> { "cgiPath", "webDavPath", "aliaseList" };
            foreach(string tag in tagList) {
                var dat = (Dat)Conf.Get(tag);
                var changed = false;
                foreach(var o in dat) {
                    var str = o.StrList[0];
                    if(str[0] != '/') {
                        changed = true;
                        str = '/' + str;
                    }
                    if(str.Length > 1 && str[str.Length - 1] != '/') {
                        changed = true;
                        str = str + '/';
                    }
                    o.StrList[0] = str;
                }
                if(changed)
                    Conf.Set(tag, dat);
            }


            //�����AopBase�y��logger�́AweboptionList[0]�Ŏb��I�ɏ���������� 
            var protocol = (int)Conf.Get("protocol");
            if (protocol==1) {//HTTPS
                var op = kernel.ListOption.Get("VirtualHost");
                var privateKeyPassword = (string)op.GetValue("privateKeyPassword");
                var certificate = (string)op.GetValue("certificate");

                //�T�[�o�pSSL�̏�����
                _ssl = new Ssl(Logger, certificate, privateKeyPassword);
            }

            var useAutoAcl = (bool)Conf.Get("useAutoAcl");// ACL���ۃ��X�g�֎����ǉ�����
            if (useAutoAcl) {
                const int max = 1; //������
                const int sec = 120; // �Ώۊ���(�b)
                _attackDb = new AttackDb(sec, max);
            }

        }
        //�I������
        new public void Dispose() {
            foreach(var db in _webDavDbList) {
                db.Dispose();
            }
            base.Dispose();
        }
        //�X���b�h�J�n����
        override protected bool OnStartServer() {
            return true;
        }
        //�X���b�h��~����
        override protected void OnStopServer() {

        }


        //�ڑ��P�ʂ̏���
        override protected void OnSubThread(SockObj sockObj) {

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var sockTcp = (SockTcp)sockObj;

            var remoteIp = sockTcp.RemoteIp;

            //opBase �y�� logger�̓o�[�`�����z�X�g�ŕύX�����̂ŁA
            //���̃|�C���^���������Ɏg�p�ł��Ȃ�

            bool keepAlive = true;//���X�|���X���I�������Ƃ��ڑ���ؒf���Ȃ��Ōp������ 

            //1��ڂ̒ʐM�Ńo�[�`�����z�X�g�̌��������{����
            var checkVirtual = true;

            var request = new Request(Logger,sockTcp);//���N�G�X�g���C�������N���X
            
            //��M�w�b�_
            var recvHeader = new Header();

            //Ver5.1.x
            string urlStr = null;//http://example.com

            //�ڑ����p�����Ă���Ԃ́A���̃��[�v�̒��ɂ���(�p�����ۂ���keepAlive�ŕێ�����)
            //�ucontinue�v�́A���̃��N�G�X�g��҂@�ubreak�v�́A�ڑ���ؒf���鎖���Ӗ�����

            WebStream inputStream = null;
            var outputStream = new WebStream(-1);




            while (keepAlive && IsLife()) {
                int responseCode;

                //***************************************************************
                // �h�L�������g�����N���X�̏�����
                //***************************************************************
                var contentType = new ContentType(Conf);
                var document = new Document(Kernel, Logger, Conf, sockTcp, contentType);

                var authrization = new Authorization(Conf, Logger);
                var authName = "";
                

                //***************************************************************
                //�f�[�^�擾
                //***************************************************************
                //���N�G�X�g�擾
                //�����̃^�C���A�E�g�l�́A�傫������ƃu���E�U�̐ؒf���擾�ł��Ȃ��Ńu���b�N���Ă��܂�
                var requestStr = sockTcp.AsciiRecv(Timeout, this);
                if (requestStr == null)
                    break;
                //\r\n�̍폜
                requestStr = Inet.TrimCrlf(requestStr);
                //Ver5.8.8 ���N�G�X�g�̉��߂Ɏ��s�����ꍇ�ɁA�����𒆒f����
                //request.Init(requestStr);
                if (!request.Init(requestStr)){
                    break;
                }

                //�w�b�_�擾�i�����f�[�^�͏����������j
                if (!recvHeader.Recv(sockTcp,(int)Conf.Get("timeOut"),this))
�@                   break;

                {
                    //Ver5.1.x
                    var hostStr = recvHeader.GetVal("host");
                    urlStr = hostStr==null ? null : string.Format("{0}://{1}",(_ssl != null)?"https":"http",hostStr);
                }

                //���͎擾�iPOST�y��PUT�̏ꍇ�j
                var contentLengthStr = recvHeader.GetVal("Content-Length");
                if(contentLengthStr != null) {
                    try{
                        //max,len�͂Ƃ���long
                        var max = Convert.ToInt64(contentLengthStr);
                        if(max!=0){//���M�f�[�^����
                            inputStream = new WebStream((256000<max)?-1:(int)max);
                            var errorCount = 0;
                            while(inputStream.Length<max && IsLife()){

                                var len = max - inputStream.Length;
                                if (len > 51200000) {
                                    len = 51200000;
                                }
                                var b = sockTcp.Recv((int)len, (int)Conf.Get("timeOut"),this);
                                if (!inputStream.Add(b)) {
                                    errorCount++;//�G���[�~��
                                    Logger.Set(LogKind.Error, null, 41, string.Format("content-Length={0} Recv={1}", max, inputStream.Length));
                                } else {
                                    errorCount = 0;//������
                                }
                                Logger.Set(LogKind.Detail, null,38, string.Format("Content-Length={0} {1}bytes Received.", max, inputStream.Length));
                                if (errorCount > 5){//�T��A�����Ď�M�����������ꍇ�A�T�[�o�G���[
                                    responseCode = 500;
                                    goto SEND;//�T�[�o�G���[
                                }
                                Thread.Sleep(10);
                            }
                            Logger.Set(LogKind.Detail, null, 39, string.Format("Content-Length={0} {1}bytes", max, inputStream.Length));
                        }
                    }catch(Exception ex){
                        Logger.Set(LogKind.Error, null, 40, ex.Message);
                    }
                }

                // /�ɂ��p�����[�^�n���ɑΉ�
                //for (int i = 0;i < Option->CgiExt->Count;i++) {
                //    wsprintf(TmpBuf,".%s/",Option->CgiExt->Strings[i]);
                //    strupr(TmpBuf);
                //    strcpy(Buf,Headers->Uri);
                //    strupr(Buf);
                //    if (NULL != (p = strstr(Buf,TmpBuf))) {
                //        i = p - Buf;
                //        i += strlen(TmpBuf) - 1;
                //        p = &Headers->Uri[i];
                //        *p = '\0';
                //        p = &Headers->UriNoConversion[i];
                //        *p = '\0';
                //        wsprintf(TmpBuf,"/%s",p + 1);
                //        Headers->PathInfo = new char[strlen(TmpBuf) + 1];
                //        strcpy(Headers->PathInfo,TmpBuf);
                //        break;
                //    }
                //}



                //***************************************************************
                //�o�[�`�����z�X�g�̌��������{���AopBase�Alogger�y�� webDavDb ��u��������
                //***************************************************************
                if (checkVirtual) {//����̂�
                    ReplaceVirtualHost(recvHeader.GetVal("host"),sockTcp.LocalAddress.Address,sockTcp.LocalAddress.Port);
                    checkVirtual = false;
                }
                //***************************************************************
                //�ڑ����p�����邩�ǂ����̔��f keepAlive�̏�����
                //***************************************************************
                if (_ssl != null) {
                    keepAlive = false;//SSL�ʐM�ł́A�P��ÂR�l�N�V�������K�v
                }else{
                    if (request.Ver == "HTTP/1.1") {//HTTP1.1�̓f�t�H���g�� keepAlive=true
                        keepAlive = true;
                    } else { // HTTP/1.1�ȊO�̏ꍇ�A�p���ڑ��́AConnection: Keep-Alive�̗L���ɏ]��
                        keepAlive = recvHeader.GetVal("Connection") == "Keep-Alive";
                    }
                }

                //***************************************************************
                // �h�L�������g�����N���X�̏�����
                //***************************************************************
                //var contentType = new ContentType(OneOption);
                //var document = new Document(kernel,Logger,OneOption,sockTcp,contentType);


                //***************************************************************
                // ���O
                //***************************************************************
                Logger.Set(LogKind.Normal, sockTcp, _ssl != null ? 23 : 24, request.LogStr);

                //***************************************************************
                // �F��
                //***************************************************************
                //var authrization = new Authorization(OneOption,Logger);
                //string authName = "";
                if (!authrization.Check(request.Uri, recvHeader.GetVal("authorization"), ref authName)) {
                    responseCode = 401;
                    keepAlive = false;//�ؒf
                    goto SEND;
                }
                //***************************************************************
                // �s����URI�ɑ΂���G���[����
                //***************************************************************
                //URI��_�����ĕs���ȏꍇ�̓G���[�R�[�h��Ԃ�
                responseCode = CheckUri(sockTcp, request, recvHeader);
                if (responseCode != 200) {
                    keepAlive = false;//�ؒf
                    goto SEND;
                }
                
                //***************************************************************
                //�^�[�Q�b�g�I�u�W�F�N�g�̏�����
                //***************************************************************
                var target = new Target(Conf,Logger);
                if (target.DocumentRoot == null) {
                    Logger.Set(LogKind.Error,sockTcp,14,string.Format("documentRoot={0}",Conf.Get("documentRoot")));//�h�L�������g���[�g�Ŏw�肳�ꂽ�t�H���_�����݂��܂���i�������p���ł��܂���j
                    break;//�h�L�������g���[�g�������ȏꍇ�́A�������p���ł��Ȃ�
                }
                target.InitFromUri(request.Uri);

                //***************************************************************
                // ���M�w�b�_�̒ǉ�
                //***************************************************************
                // ���ʊg�� BlackJumboDog�o�R�̃��N�G�X�g�̏ꍇ ���M�w�b�_��RemoteHost��ǉ�����
                if ((bool)Conf.Get("useExpansion")) {
                    if (recvHeader.GetVal("Host") != null) {
                        document.AddHeader("RemoteHost",sockTcp.RemoteAddress.Address.ToString());
                    }
                }
                //��M�w�b�_�ɁuPathInfo:�v���ݒ肳��Ă���ꍇ�A���M�w�b�_�ɁuPathTranslated�v��ǉ�����
                var pathInfo = recvHeader.GetVal("PathInfo");
                if (pathInfo != null) {
                    pathInfo = target.DocumentRoot + pathInfo;
                    document.AddHeader("PathTranslated",Util.SwapChar('/','\\',pathInfo));
                }
                //***************************************************************
                //���\�b�h�ɉ��������� OPTIONS �Ή� Ver5.1.x
                //***************************************************************
                if(WebDav.IsTarget(request.Method)){
                    var webDav = new WebDav(Logger, _webDavDb, target, document, urlStr, recvHeader.GetVal("Depth"), contentType,(bool)Conf.Get("useEtag"));

                    var inputBuf = new byte[0];
                    if(inputStream!=null){
                        inputBuf = inputStream.GetBytes();
                    }

                    switch(request.Method) {
                        case HttpMethod.Options:
                            responseCode = webDav.Option();
                            break;
                        case HttpMethod.Delete:
                            responseCode = webDav.Delete();
                            break;
                        case HttpMethod.Put:
                            responseCode = webDav.Put(inputBuf);
                            break;
                        case HttpMethod.Proppatch:
                            responseCode = webDav.PropPatch(inputBuf);
                            break;
                        case HttpMethod.Propfind:
                            responseCode = webDav.PropFind();
                            break;
                        case HttpMethod.Mkcol:
                            responseCode = webDav.MkCol();
                            break;
                        case HttpMethod.Copy:
                        case HttpMethod.Move:
                            responseCode = 405;
                            //Destnation�Ŏw�肳�ꂽ�t�@�C���͏������݋�����Ă��邩�H
                            var dstTarget = new Target(Conf,Logger);
                            string destinationStr = recvHeader.GetVal("Destination");
                            if(destinationStr != null) {
                                if(destinationStr.IndexOf("://") == -1) {
                                    destinationStr = urlStr + destinationStr;
                                }
                                var uri = new Uri(destinationStr);
                                dstTarget.InitFromUri(uri.LocalPath);


                                if(dstTarget.WebDavKind == WebDavKind.Write) {
                                    var overwrite = false;
                                    var overwriteStr = recvHeader.GetVal("Overwrite");
                                    if(overwriteStr != null) {
                                        if(overwriteStr == "F") {
                                            overwrite = true;
                                        }
                                    }
                                    responseCode = webDav.MoveCopy(dstTarget,overwrite,request.Method);
                                    document.AddHeader("Location",destinationStr);
                                }
                            }
                            break;
                    }
                    //WebDAV�ɑ΂��郊�N�G�X�g�́A�����ŏ�������
                    goto SEND;
                }
                //�ȉ� label SEND�܂ł̊Ԃ́AGET/POST�Ɋւ��鏈��

                //***************************************************************
                //�^�[�Q�b�g�̎�ނɉ���������
                //***************************************************************

                if (target.TargetKind == TargetKind.Non) { //������Ȃ��ꍇ
                    responseCode = 404;
                    goto SEND;
                }
                if (target.TargetKind == TargetKind.Move) { //�^�[�Q�b�g�̓f�B���N�g���̏ꍇ
                    responseCode = 301;
                    goto SEND;
                }
                if (target.TargetKind == TargetKind.Dir) { //�f�B���N�g���ꗗ�\���̏ꍇ
                    //�C���f�b�N�X�h�L�������g�𐶐�����
                    if (!document.CreateFromIndex(request, target.FullPath)) 
                        break;
                    goto SEND;
                }

                //***************************************************************
                //  �B�������̃t�@�C���ւ̃A�N�Z�X����
                //***************************************************************
                if (!(bool)Conf.Get("useHidden")) {
                    if ((target.Attr & FileAttributes.Hidden) == FileAttributes.Hidden) {
                        //�G���[�L�������g�𐶐�����
                        responseCode = 404;
                        keepAlive = false;//�ؒf
                        goto SEND;
                    }
                }

                if (target.TargetKind == TargetKind.Cgi || target.TargetKind == TargetKind.Ssi) {
                    keepAlive = false;//�f�t�H���g�Őؒf
                    
                    //���ϐ��쐬
                    var env = new Env(Kernel,Conf,request, recvHeader,sockTcp, target.FullPath);
                    
                    // �ڍ׃��O
                    Logger.Set(LogKind.Detail,sockTcp,18,string.Format("{0} {1}",target.CgiCmd,Path.GetFileName(target.FullPath)));

                    if (target.TargetKind == TargetKind.Cgi) {

                        var cgi = new Cgi();
                        var cgiTimeout = (int)Conf.Get("cgiTimeout");
                        if (!cgi.Exec(target,request.Param,env,inputStream,out outputStream,cgiTimeout)) {
                            // �G���[�o��
                            var errStr = Encoding.ASCII.GetString(outputStream.GetBytes()); 

                            Logger.Set(LogKind.Error,sockTcp,16,errStr);
                            responseCode = 500;
                            goto SEND;
                        }

                        //***************************************************
                        // NPH (Non-Parsed Header CGI)�X�N���v�g  nph-�Ŏn�܂�ꍇ�A�T�[�o�����i���X�|���X�R�[�h��w�b�_�̒ǉ��j���o�R���Ȃ�
                        //***************************************************
                        if (Path.GetFileName(target.FullPath).IndexOf("nph-") == 0) {
                            sockTcp.SendUseEncode(outputStream.GetBytes());//CGI�o�͂����̂܂ܑ��M����
                            break;
                        }
                        // CGI�œ���ꂽ�o�͂���A�{�̂ƃw�b�_�𕪗�����
                        if(!document.CreateFromCgi(outputStream.GetBytes()))
                            break;
                        // cgi�o�͂ŁALocation:���܂܂��ꍇ�A���X�|���X�R�[�h��302�ɂ���
                        if (document.SearchLocation())//Location:�w�b�_���܂ނ��ǂ���
                            responseCode = 302;
                        goto SEND;
                    } 
                    //SSI
                    var ssi = new Ssi(Kernel, Logger,Conf, sockTcp, request, recvHeader);
                    if (!ssi.Exec(target,env,outputStream)) {
                        // �G���[�o��
                        Logger.Set(LogKind.Error,sockTcp,22,MLang.GetString(outputStream.GetBytes()));
                        responseCode = 500;
                        goto SEND;
                    }
                    document.CreateFromSsi(outputStream.GetBytes(),target.FullPath);
                    goto SEND;
                }

                //�ȉ��́A�ʏ�t�@�C���̏��� TARGET_KIND.FILE

                //********************************************************************
                //Modified����
                //********************************************************************
                if (recvHeader.GetVal("If_Modified_Since") != null) {
                    var dt = Util.Str2Time(recvHeader.GetVal("If-Modified-Since"));
                    if (target.FileInfo.LastWriteTimeUtc.Ticks / 10000000 <= dt.Ticks / 10000000) {
                        
                        responseCode = 304;
                        goto SEND;
                    }
                }
                if (recvHeader.GetVal("If_Unmodified_Since") != null) {
                    var dt = Util.Str2Time(recvHeader.GetVal("If_Unmodified_Since"));
                    if (target.FileInfo.LastWriteTimeUtc.Ticks / 10000000 > dt.Ticks / 10000000) {
                        responseCode = 412;
                        goto SEND;
                    }
                }
                document.AddHeader("Last-Modified",Util.UtcTime2Str(target.FileInfo.LastWriteTimeUtc));
                //********************************************************************
                //ETag����
                //********************************************************************
                // (1) useEtag��true�̏ꍇ�́A���M����ETag��t������
                // (2) If-None-Match �Ⴕ����If-Match�w�b�_���w�肳��Ă���ꍇ�́A�r���Ώۂ��ǂ����̔��f���K�v�ɂȂ�
                if ((bool)Conf.Get("useEtag") || recvHeader.GetVal("If-Match") != null || recvHeader.GetVal("If-None-Match") != null) {
                    //Ver5.1.5
                    //string etagStr = string.Format("\"{0:x}-{1:x}\"", target.FileInfo.Length, (target.FileInfo.LastWriteTimeUtc.Ticks / 10000000));
                    var etagStr = WebServerUtil.Etag(target.FileInfo);
                    string str;
                    if (null != (str = recvHeader.GetVal("If-Match"))) {
                        if (str != "*" && str != etagStr) {
                            responseCode = 412;
                            goto SEND;
                        }

                    }
                    if (null != (str = recvHeader.GetVal("If-None-Match"))) {
                        if (str != "*" && str == etagStr) {
                            responseCode = 304;
                            goto SEND;
                        }
                    }
                    if ((bool)Conf.Get("useEtag"))
                        document.AddHeader("ETag",etagStr);
                }
                //********************************************************************
                //Range����
                //********************************************************************
                document.AddHeader("Accept-Range","bytes");
                var rangeFrom = 0L;//�f�t�H���g�͍ŏ�����
                var rangeTo = target.FileInfo.Length;//�f�t�H���g�͍Ō�܂Łi�t�@�C���T�C�Y�j
                if (recvHeader.GetVal("Range") != null) {//�����W�w��̂��郊�N�G�X�g�̏ꍇ
                    var range = recvHeader.GetVal("Range");
                    //�w��͈͂��擾����i�}���`�w��ɂ͖��Ή��j
                    if (range.IndexOf("bytes=") == 0) {
                        range = range.Substring(6);
                        var tmp = range.Split('-');


                        //Ver5.3.5 ApacheKiller�Ώ�
                        if (tmp.Length > 20) {
                            Logger.Set(LogKind.Secure, sockTcp,9000054, string.Format("[ Apache Killer ]Range:{0}", range));

                            AutoDeny(false, remoteIp);
                            responseCode = 503;
                            keepAlive = false;//�ؒf
                            goto SEND;
                        }
                        
                        if(tmp.Length == 2) {

                            //Ver5.3.6 �̃f�o�b�O�p
                            //tmp[1] = "499";

                            if(tmp[0] != "") {
                                if(tmp[1] != "") {// bytes=0-10 0�`10��11�o�C�g
                                    
                                    //Ver5.5.9
                                    rangeFrom = Convert.ToInt64(tmp[0]);
                                    if (tmp[1] != "") {
                                        //Ver5.5.9
                                        rangeTo = Convert.ToInt64(tmp[1]);
                                        if (target.FileInfo.Length <= rangeTo) {
                                            rangeTo = target.FileInfo.Length - 1;
                                        } else {
                                            document.SetRangeTo = true;//Ver5.4.0
                                        }
                                    }
                                } else {// bytes=3- 3�`�Ō�܂�
                                    rangeTo = target.FileInfo.Length - 1;
                                    rangeFrom = Convert.ToInt64(tmp[0]); 
                                }
                            } else {
                                if(tmp[1] != "") {// bytes=-3 �Ōォ��3�o�C�g
                                    var len = Convert.ToInt64(tmp[1]);
                                    rangeTo = target.FileInfo.Length - 1;
                                    rangeFrom = rangeTo-len+1;
                                    if(rangeFrom<0)
                                        rangeFrom=0;
                                    document.SetRangeTo = true;//Ver5.4.0
                                }

                            }
                            if(rangeFrom <= rangeTo) {
                                //����ɔ͈͂��擾�ł����ꍇ�A����Range���[�h�œ��삷��
                                document.AddHeader("Content-Range",string.Format("bytes {0}-{1}/{2}",rangeFrom,rangeTo,target.FileInfo.Length));
                                responseCode = 206;
                            }
                        }
                    }
                }
                //�ʏ�t�@�C���̃h�L�������g
                if (request.Method != HttpMethod.Head) {
                    if (!document.CreateFromFile(target.FullPath,rangeFrom,rangeTo))
                        break;
                }

            SEND:
                //���X�|���X�R�[�h��200�ȊO�̏ꍇ�́A�h�L�������g�i�y�ё��M�w�b�_�j���G���[�p�ɕύX����
                if(responseCode != 200 && responseCode != 302 && responseCode != 206 && responseCode != 207 && responseCode != 204 && responseCode != 201) {

                    //ResponceCode�̉����ăG���[�h�L�������g�𐶐�����
                    if (!document.CreateFromErrorCode(request,responseCode))
                        break;

                    if (responseCode == 301) {//�^�[�Q�b�g���t�@�C���ł͂Ȃ��f�B���N�g�̊ԈႢ�̏ꍇ
                        if(urlStr != null) {
                            var str = string.Format("{0}{1}/",urlStr,request.Uri);
                            document.AddHeader("Location",Encoding.UTF8.GetBytes(str));
                        }
                    }

                    if (responseCode == 304 || responseCode == 301) {//304 or 301 �̏ꍇ�́A�w�b�_�݂̂ɂȂ�
                        document.Clear();
                    } else {
                        if (responseCode == 401) {
                            document.AddHeader("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", authName));
                        }
                    }
                }
                
                //Ver5.6.2 request.Send()�p�~
                var responseStr = request.CreateResponse(responseCode);
                sockTcp.AsciiSend(responseStr);//���X�|���X���M
                Logger.Set(LogKind.Detail, sockTcp, 4, responseStr);//���O

                
                document.Send(keepAlive,this);//�h�L�������g�{�̑��M
            }

            if(inputStream!=null)
                inputStream.Dispose();
            if (outputStream != null)
                outputStream.Dispose();

            //end://���̃\�P�b�g�ڑ��̏I��
            if (sockTcp != null) {
                sockTcp.Close();
            }
        }
        
        //********************************************************
        // Host:�w�b�_�����āA�o�[�`�����z�X�g�̐ݒ�Ƀq�b�g�����ꍇ��
        // �I�v�V��������u��������
        //********************************************************
        void ReplaceVirtualHost(string host, IPAddress ip,int port) {
            
            //Ver5.0.0-b12
            if(host == null) {
                return;
            }

            //Ver5.0.0-a6 ���zWeb�̌������z�X�g���i�A�h���X�j�{�|�[�g�ԍ��ɏC��
            for (int n = 0; n < 2; n++) {
                if (n == 0) {//�P��ڂ̓z�X�g���Ō�������
                    //Ver5.0.0-a6 �u�z�X�g��:�|�[�g�ԍ��v�̌`���Ō�������
                    if (host.IndexOf(':') < 0) {
                        host = string.Format("{0}:{1}",host,port);
                    }
                    host = host.ToUpper();//�z�X�g���́A�啶���E����������ʂ��Ȃ�
                } else {//�Q��ڂ̓A�h���X�Ō�������
                    host = string.Format("{0}:{1}",ip,port);
                }

                //�o�[�`�����z�X�g�w��̏ꍇ�I�v�V������ύX����
                foreach (var op in WebOptionList) {
                    //�擪��Web-���폜����
                    string name = op.NameTag.Substring(4).ToUpper();
                    if (name == host) {
                        if (op.NameTag != Conf.NameTag) {
                            //Ver5.1.4 webDavDb��u��������
                            foreach(var db in _webDavDbList) {
                                if(db.NameTag == op.NameTag) {
                                    _webDavDb = db;
                                }
                            }
                            //�I�v�V�����y�у��K�[���ď���������
                            //OneOption = op;
                            Conf = new Conf(op);
                            Logger = Kernel.CreateLogger(op.NameTag, (bool)Conf.Get("useDetailsLog"), this);
                        }
                        return;
                    }
                }
            }
        }


        //********************************************************
        //URI��_�����ĕs���ȏꍇ�̓G���[�R�[�h��Ԃ�
        //return 200 �G���[�Ȃ�
        //********************************************************
        int CheckUri(SockTcp sockTcp, Request request, Header recvHeader) {
            var responseCode = 200;

            // v2.3.1 Uri �̂P�����ڂ�/�Ŗ����ꍇ
            if (request.Uri[0] != '/') {
                responseCode = 400;

                //Uri�̍Ō�ɋ󔒂������Ă���ꍇ
            } else if (request.Uri[request.Uri.Length - 1] == (' ') || request.Uri[request.Uri.Length - 1] == ('.')) {
                responseCode = 404;

                // ./�̊܂܂�郊�N�G�X�g��404�ŗ��Ƃ�
                // %20/�̊܂܂�郊�N�G�X�g��404�ŗ��Ƃ�
            } else if ((0 <= request.Uri.IndexOf("./")) || (0 <= request.Uri.IndexOf(" /"))) {
                responseCode = 404;

                // HTTP1.1��host�w�b�_�̂Ȃ����̂̓G���[
            } else if (request.Ver == "HTTP/1.1" && recvHeader.GetVal("Host") == null) {
                responseCode = 400;

                // ..���Q�Ƃ���p�X�̔r��
            } else if (!(bool)Conf.Get("useDot") && 0 <= request.Uri.IndexOf("..")) {
                Logger.Set(LogKind.Secure,sockTcp,13,"URI=" + request.Uri);//.. ���܂܂�郊�N�G�X�g�͋�����Ă��܂���B
                responseCode = 403;
            }
            return responseCode;
        }




        //bool CheckAuthList(string requestUri) {
        //    // �y���� �V���[�g�t�@�C�����ŃA�N�Z�X�����ꍇ�́A�F�؂̉�����l������K�v������z
        //    //AnsiString S = ExtractShortPathName(ShortNamePath);
        //    var authList = (Dat)this.Conf.Get("authList");
        //    foreach (var o in authList) {
        //        if (!o.Enable)
        //            continue;
        //        string uri = o.StrList[0];
                
        //        if (requestUri.IndexOf(uri) == 0) {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        void AutoDeny(bool success, Ip remoteIp) {
            if (_attackDb == null)
                return;
            //�f�[�^�x�[�X�ւ̓o�^
            if (!_attackDb.IsInjustice(success, remoteIp))
                return;

            //�u���[�g�t�H�[�X�A�^�b�N
            if (AclList.Append(remoteIp)) {//ACL�������ېݒ�(�u������v�ɐݒ肳��Ă���ꍇ�A�@�\���Ȃ�)
                //�ǉ��ɐ��������ꍇ�A�I�v�V����������������
                var d = (Dat)Conf.Get("acl");
                var name = string.Format("AutoDeny-{0}", DateTime.Now);
                var ipStr = remoteIp.ToString();
                d.Add(true, string.Format("{0}\t{1}", name, ipStr));
                Conf.Set("acl", d);
                Conf.Save(Kernel.IniDb);

                Logger.Set(LogKind.Secure, null, 9000055, string.Format("{0},{1}", name, ipStr));
            } else {
                Logger.Set(LogKind.Secure, null, 9000056, remoteIp.ToString());
            }
        }

        //�e�X�g�p
        public String DocumentRoot{
            get{
                return (string) Conf.Get("documentRoot");
            }
        }

        //RemoteServer�ł̂ݎg�p�����
        public override void Append(OneLog oneLog) {

        }


    }
}
