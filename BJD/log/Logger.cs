using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Bjd.sock;
using Bjd.util;

namespace Bjd.log{
    public delegate string GetMsgDelegate(int no);

    //���O�o�͗p�̃N���X<br>
    //�t�@�C���ƃf�B�X�v���C�̗����𓝊�����
    //�e�X�g�p�ɁALogger.create()�Ń��O�o�͂���������؍s��Ȃ��C���X�^���X���쐬�����
    public class Logger{
        private Kernel _kernel;
        private readonly LogLimit _logLimit;
        private readonly LogFile _logFile;
        private readonly LogView _logView;
        private readonly bool _isJp;
        private readonly String _nameTag;
        private readonly bool _useDetailsLog;
        private readonly bool _useLimitString;
        private readonly ILogger _logger;

        [DllImport("kernel32.dll")]
        static extern int GetCurrentThreadId();

        //�R���X�g���N�^
        //kernel�̒���CreateLogger()���`���Ďg�p����
        public Logger(Kernel kernel, LogLimit logLimit, LogFile logFile, LogView logView, bool isJp, String nameTag,
                      bool useDetailsLog, bool useLimitString, ILogger logger){
            _kernel = kernel;
            _logLimit = logLimit;
            _logFile = logFile;
            _logView = logView;
            _isJp = isJp;
            _nameTag = nameTag;
            _useDetailsLog = useDetailsLog;
            _useLimitString = useLimitString;
            _logger = logger;
        }

        //�e�X�g�p
        public Logger(){
            _logLimit = null;
            _logFile = null;
            _logView = null;
            _isJp = true;
            _nameTag = "";
            _useDetailsLog = false;
            _useLimitString = false;
            _logger = null;
        }

        //���O�o��
        //Override�\�i�e�X�g�Ŏg�p�j
        public void Set(LogKind logKind, SockObj sockBase, int messageNo, String detailInfomation){
            //�f�o�b�O����kernel������������Ă��Ȃ��ꍇ�A�����Ȃ�
            if (_logFile == null && _logView == null){
                return;
            }
            //�ڍ׃��O���ΏۊO�̏ꍇ�A�����Ȃ�
            if (logKind == LogKind.Detail){
                if (!_useDetailsLog){
                    return;
                }
            }
            int threadId = GetCurrentThreadId();
            //long threadId = Thread.currentThread().getId(); 
            var message = _isJp ? "��`����Ă��܂���" : "Message is not defined";
            if (messageNo < 9000000){
                if (_logger != null){
                    message = _logger.GetMsg(messageNo); //�f���Q�[�g���g�p�����p���ɂ�郁�b�Z�[�W�擾
                }
            }
            else{
                //(9000000�ȏ�)���ʔԍ��̏ꍇ�̏���
                switch (messageNo){
                    case 9000000:
                        message = _isJp ? "�T�[�o�J�n" : "Server started it";
                        break;
                    case 9000001:
                        message = _isJp ? "�T�[�o��~" : "Server stopped";
                        break;
                    case 9000002:
                        message = "_subThread() started.";
                        break;
                    case 9000003:
                        message = "_subThread() stopped.";
                        break;
                    case 9000004:
                        message = _isJp
                                      ? "�����ڑ����𒴂����̂Ń��N�G�X�g���L�����Z�����܂�"
                                      : "Because the number of connection exceeded it at the same time, the request was canceled.";
                        break;
                    case 9000005:
                        message = _isJp
                                      ? "��M�����񂪒������܂��i�s���ȃ��N�G�X�g�̉\�������邽�ߐؒf���܂���)"
                                      : "Reception character string is too long (cut off so that there was possibility of an unjust request in it)";
                        break;
                    case 9000006:
                        message = _isJp ? "���̃|�[�g�́A���ɑ��̃v���O�������g�p���Ă��邽�ߎg�p�ł��܂���" : "Cannot use this port so that other programs already use it";
                        break;
                    case 9000007:
                        message = _isJp ? "callBack�֐����w�肳��Ă��܂���[UDP]" : "It is not appointed in callback function [UDP]";
                        break;
                    case 9000008:
                        message = _isJp ? "�v���O�C�����C���X�g�[�����܂���" : "setup initialize plugin";
                        break;
                    //case 9000009:
                    //    message = _isJp ? "Socket.Bind()�ŃG���[���������܂����B[TCP]" : "An error occurred in Socket.Bind() [TCP]";
                    //    break;
                    //case 9000010:
                    //    message = _isJp
                    //                  ? "Socket.Listen()�ŃG���[���������܂����B[TCP]"
                    //                  : "An error occurred in Socket..Listen() [TCP]";
                    //    break;
                    case 9000011:
                        message = "tcpQueue().Dequeue()=null";
                        break;
                    case 9000012:
                        message = "tcpQueue().Dequeue() SocektObjState != SOCKET_OBJ_STATE.CONNECT break";
                        break;
                    case 9000013:
                        message = "tcpQueue().Dequeue()";
                        break;
                        //			case 9000014:
                        //				message = "SendBinaryFile(string fileName) socket.Send()";
                        //				break;
                        //			case 9000015:
                        //				message = "SendBinaryFile(string fileName,long rangeFrom,long rangeTo) socket.Send()";
                        //				break;
                    case 9000016:
                        message = _isJp
                                      ? "���̃A�h���X����̐ڑ��͋�����Ă��܂���(ACL)"
                                      : "Connection from this address is not admitted.(ACL)";
                        break;
                    case 9000017:
                        message = _isJp
                                      ? "���̃A�h���X����̐ڑ��͋�����Ă��܂���(ACL)"
                                      : "Connection from this address is not admitted.(ACL)";
                        break;
                    case 9000018:
                        message = _isJp ? "���̗��p�҂̃A�N�Z�X�͋�����Ă��܂���(ACL)" : "Access of this user is not admitted (ACL)";
                        break;
                    case 9000019:
                        message = _isJp ? "�A�C�h���^�C���A�E�g" : "Timeout of an idle";
                        break;
                    case 9000020:
                        message = _isJp ? "���M�Ɏ��s���܂���" : "Transmission of a message failure";
                        break;
                    case 9000021:
                        message = _isJp ? "ThreadBase::loop()�ŗ�O���������܂���" : "An exception occurred in ThreadBase::Loop()";
                        break;
                    case 9000022:
                        message = _isJp
                                      ? "�E�C���h�E���ۑ��t�@�C����IO�G���[���������܂���"
                                      : "An IO error occurred in a window information save file";
                        break;
                    case 9000023:
                        message = _isJp ? "�ؖ����̓ǂݍ��݂Ɏ��s���܂���" : "Reading of a certificate made a blunder";
                        break;
                        //case 9000024: message = isJp ? "SSL�l�S�V�G�[�V�����Ɏ��s���܂���" : "SSL connection procedure makes a blunder"; break;
                        //case 9000025: message = isJp ? "�t�@�C���i�閧���j��������܂���" : "Private key is not found"; break;
                    case 9000026:
                        message = _isJp ? "�t�@�C���i�ؖ����j��������܂���" : "A certificate is not found";
                        break;
                        //case 9000027: message = isJp ? "OpenSSL�̃��C�u����(ssleay32.dll,libeay32.dll)��������܂���" : "OpenSSL library (ssleay32.dll,libeay32.dll) is not found"; break;
                    case 9000028:
                        message = _isJp ? "SSL�̏������Ɏ��s���Ă��܂�" : "Initialization of SSL made a blunder";
                        break;
                    case 9000029:
                        message = _isJp ? "�w�肳�ꂽ��ƃf�B���N�g�������݂��܂���" : "A work directory is not found";
                        break;
                    case 9000030:
                        message = _isJp ? "�N������T�[�o��������܂���" : "A starting server is not found";
                        break;
                    case 9000031:
                        message = _isJp ? "���O�t�@�C���̏������Ɏ��s���܂���" : "Failed in initialization of logfile";
                        break;
                    case 9000032:
                        message = _isJp ? "���O�ۑ��ꏊ" : "a save place of LogFile";
                        break;
                    case 9000033:
                        message = _isJp ? "�t�@�C���ۑ����ɃG���[���������܂���" : "An error occurred in a File save";
                        break;
                    case 9000034:
                        message = _isJp ? "ACL�w��ɖ�肪����܂�" : "ACL configuration failure";
                        break;
                    case 9000035:
                        message = _isJp ? "Socket()�ŃG���[���������܂����B[TCP]" : "An error occurred in Socket() [TCP]";
                        break;
                    //case 9000036:
                    //    message = _isJp ? "Socket()�ŃG���[���������܂����B[UDP]" : "An error occurred in Socket() [UDP]";
                    //    break;
                    case 9000037:
                        message = _isJp ? "_subThread()�ŗ�O���������܂���" : "An exception occurred in _subThread()";
                        break;
                    case 9000038:
                        message = _isJp ? "�y��O�z" : "[Exception]";
                        break;
                    case 9000039:
                        message = _isJp ? "�ySTDOUT�z" : "[STDOUT]";
                        break;
                    case 9000040:
                        message = _isJp ? "�g��SMTP�K�p�͈͂̎w��ɖ�肪����܂�" : "ESMTP range configuration failure";
                        break;
                    case 9000041:
                        message = _isJp ? "disp2()�ŗ�O���������܂���" : "An exception occurred in disp2()";
                        break;
                    case 9000042:
                        message = _isJp
                                      ? "�������Ɏ��s���Ă��邽�߃T�[�o���J�n�ł��܂���"
                                      : "Can't start a server in order to fail in initialization";
                        break;
                    case 9000043:
                        message = _isJp ? "�N���C�A���g�����ؒf����܂���" : "The client side was cut off";
                        break;
                    case 9000044:
                        message = _isJp ? "�T�[�o�����ؒf����܂���" : "The server side was cut off";
                        break;
                    case 9000045:
                        message = _isJp
                                      ? "�u�I�v�V����(O)-���O�\��(L)-��{�ݒ�-���O�̕ۑ��ꏊ�v���w�肳��Ă��܂���"
                                      : "\"log save place\" is not appointed";
                        break;
                    case 9000046:
                        message = _isJp ? "socket.send()�ŃG���[���������܂���" : "socket.send()";
                        break;
                    case 9000047:
                        message = _isJp ? "���[�U���������ł�" : "A user name is null and void";
                        break;
                    case 9000048:
                        message = _isJp ? "ThreadBase::Loop()�ŗ�O���������܂���" : "An exception occurred in ThreadBase::Loop()";
                        break;
                    case 9000049:
                        message = _isJp ? "�y��O�z" : "[Exception]";
                        break;
                    case 9000050:
                        message = _isJp ? "�t�@�C���ɃA�N�Z�X�ł��܂���ł���" : "Can't open a file";
                        break;
                    case 9000051:
                        message = _isJp ? "�C���X�^���X�̐����Ɏ��s���܂���" : "Can't create instance";
                        break;
                    case 9000052:
                        message = _isJp ? "���O�����Ɏ��s���܂���" : "Non-existent domain";
                        break;
                    case 9000053:
                        message = _isJp ? "�y��O�zSockObj.Resolve()" : "[Exception] SockObj.Resolve()";
                        break;
                    case 9000054:
                        message = _isJp
                                      ? "Apache Killer�ɂ��U���̉\��������܂�"
                                      : "There is possibility of attack by Apache Killer in it";
                        break;
                    case 9000055:
                        message = _isJp ? "�y�������ہz�uACL�v�̋֎~���闘�p�ҁi�A�h���X�j�ɒǉ����܂���" : "Add it to a deny list automatically";
                        break;
                    case 9000056:
                        message = _isJp
                                      ? "�s���A�N�Z�X�����o���܂������AACL�u���ہv���X�g�͒ǉ�����܂���ł���"
                                      : "I detected possibility of Attack, but the ACL [Deny] list was not added";
                        break;
                    case 9000057:
                        message = _isJp ? "�y��O�z" : "[Exception]";
                        break;
                    case 9000058:
                        message = _isJp ? "���[���̑��M�Ɏ��s���܂���" : "Failed in the transmission of a message of an email";
                        break;
                    case 9000059:
                        message = _isJp ? "���[���̕ۑ��Ɏ��s���܂���" : "Failed in a save of an email";
                        break;
                    case 9000060:
                        message = _isJp ? "�y��O�z" : "[Exception]";
                        break;
                    case 9000061:
                        message = _isJp ? "�y��O�z" : "[Exception]";
                        break;
                    //case 9000061:
                        //	message = isJp ? "�t�@�C���̍쐬�Ɏ��s���܂���" : "Failed in making of a file";
                        //	break;
                }
            }
            var remoteHostname = (sockBase == null) ? "-" : sockBase.RemoteHostname;
            var oneLog = new OneLog(DateTime.Now, logKind, _nameTag, threadId, remoteHostname, messageNo, message,
                                       detailInfomation);

            // �\�������Ƀq�b�g���邩�ǂ����̊m�F
            var isDisplay = true;
            if (!oneLog.IsSecure()){
                //�Z�L�����e�B���O�͕\�������̑ΏۊO
                if (_logLimit != null){
                    isDisplay = _logLimit.IsDisplay(oneLog.ToString());
                }
            }
            if (_logView != null && isDisplay){
                //isDisplay�̌��ʂɏ]��
                _logView.Append(oneLog);
            }

            //Ver5.8.8 Java fix
            //LogView�̒��Ŏ��s���Ă��������[�g�N���C�A���g�ւ̑��M��������Ɉړ�����
            //�T�[�r�X�N���̍ۂɁAListView��null�ŁA��������Ȃ�����
            //�����[�g�N���C�A���g�ւ̃��O���M
            if (_kernel != null && _kernel.RemoteConnect != null && _kernel.ListServer != null) {
                //�N���C�A���g����ڑ�����Ă���ꍇ
                var sv = _kernel.ListServer.Get("Remote");
                if (sv != null)
                    sv.Append(oneLog);
            }


            if (_logFile != null){
                if (_useLimitString){
                    //�\���������L���ȏꍇ
                    if (isDisplay){
                        //isDisplay�̌��ʂɏ]��
                        _logFile.Append(oneLog);
                    }
                }
                else{
                    //�\�������������ȏꍇ�́A���ׂĕۑ������
                    _logFile.Append(oneLog);
                }
            }
        }


        //Ver5.3.2
        public void Exception(Exception ex, SockObj sockObj, int messageNo) {
            Set(LogKind.Error, sockObj, messageNo, ex.Message);
            string[] tmp = ex.StackTrace.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in tmp) {
                var lines = new List<string>();
                var l = Util.SwapStr("\r\n", "", s);
                while (true) {
                    if (l.Length < 80) {
                        lines.Add(l);
                        break;
                    }
                    lines.Add(l.Substring(0, 80));
                    l = l.Substring(80);
                }
                for (int i = 0; i < lines.Count; i++) {
                    if (i == 0) {
                        Set(LogKind.Error, sockObj, messageNo, lines[i]);
                    } else {
                        Set(LogKind.Error, sockObj, messageNo, "   -" + lines[i]);
                    }
                }
            }
        }
    }
    //public void exception(Exception ex) {
    //	int messageNo = 9000060;
    //	set(LogKind.ERROR, null, messageNo, ex.getMessage());
    //TODO Logger ��O���b�Z�[�W�������񂳂ꂽ���̃X�^�b�N�g���[�X�@������
    //        string[] tmp = ex.StackTrace.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
    //        for (String s : tmp) {
    //            var lines = new List<string>();
    //            var l = Util.SwapStr("\r\n", "", s);
    //            while (true) {
    //                if (l.Length < 80) {
    //                    lines.Add(l);
    //                    break;
    //                }
    //                lines.Add(l.Substring(0, 80));
    //                l = l.Substring(80);
    //            }
    //            for (int i = 0; i < lines.Count; i++) {
    //                if (i == 0) {
    //                    Set(LogKind.ERROR, sockObj, messageNo, lines[i]);
    //                } else {
    //                    Set(LogKind.ERROR, sockObj, messageNo, "   -" + lines[i]);
    //                }
    //            }
    //        }
    //}
}


/*

public class Logger {
        readonly Kernel _kernel;
        readonly string _nameTag;
        readonly bool _useDetailsLog;
        readonly GetMsgDelegate _getMsg;

        [DllImport("kernel32.dll")]
        static extern int GetCurrentThreadId();

        public Logger(Kernel kernel,string nameTag,bool useDetailsLog, GetMsgDelegate getMsg) {
            _kernel = kernel;
            _nameTag = nameTag;
            _useDetailsLog = useDetailsLog;
            _getMsg = getMsg;
        }

        public Logger(){
            _kernel = null;
            _nameTag = "TEST";
            _useDetailsLog = false;
            _getMsg = null;
        }

        public void Set(LogKind logKind,SockObj sockObj,int messageNo,string detailInfomation) {
            if (logKind == LogKind.Detail) {
                if (!_useDetailsLog)
                    return;
            }
            int threadId = GetCurrentThreadId();

            string message = (_kernel.IsJp()) ? "��`����Ă��܂���" : "Message is not defined";
            
            if (messageNo < 9000000) {
                if(_getMsg!=null)
                    message = _getMsg(messageNo);//�f���Q�[�g���g�p�����p���ɂ�郁�b�Z�[�W�擾
            } else {
                //(9000000�ȏ�)���ʔԍ��̏ꍇ�̏���
                switch (messageNo) {
                    case 9000000: message = (_kernel.IsJp())?"�T�[�o�J�n":"Server started it";break;
                    case 9000001: message = (_kernel.IsJp()) ? "�T�[�o��~" : "Server stopped"; break;
                    case 9000002: message = "_subThread() started.";break;
                    case 9000003: message = "_subThread() stopped."; break;
                    case 9000004: message = (_kernel.IsJp()) ? "�����ڑ����𒴂����̂Ń��N�G�X�g���L�����Z�����܂�" : "Because the number of connection exceeded it at the same time, the request was canceled."; break;
                    case 9000005: message = (_kernel.IsJp()) ? "��M�����񂪒������܂��i�s���ȃ��N�G�X�g�̉\�������邽�ߐؒf���܂���)" : "Reception character string is too long (cut off so that there was possibility of an unjust request in it)"; break;
                    case 9000006: message = (_kernel.IsJp()) ? "Socket.Bind()�ŃG���[���������܂����B[UDP]" : "An error occurred in Socket.Bind() [UDP]"; break;
                    case 9000007: message = (_kernel.IsJp()) ? "callBack�֐����w�肳��Ă��܂���[UDP]" : "It is not appointed in callback function [UDP]"; break;
                    case 9000008: message = (_kernel.IsJp()) ? "BeginReceiveFrom()�ŃG���[���������܂���[UDP]" : "An error occurred in BeginReceiveFrom() [UDP]"; break;
                    case 9000009: message = (_kernel.IsJp()) ? "Socket.Bind()�ŃG���[���������܂����B[TCP]" : "An error occurred in Socket.Bind() [TCP]"; break;
                    case 9000010: message = (_kernel.IsJp()) ? "Socket.Listen()�ŃG���[���������܂����B[TCP]" : "An error occurred in Socket..Listen() [TCP]"; break;
                    case 9000011: message = "tcpQueue().Dequeue()=null";break;
                    case 9000012: message = "tcpQueue().Dequeue() SocektObjState != SOCKET_OBJ_STATE.CONNECT break";break;
                    case 9000013: message = "tcpQueue().Dequeue()";break;
                    case 9000014: message = "SendBinaryFile(string fileName) socket.Send()";break;
                    case 9000015: message = "SendBinaryFile(string fileName,long rangeFrom,long rangeTo) socket.Send()";break;
                    case 9000016: message = (_kernel.IsJp()) ? "���̃A�h���X����̐ڑ��͋�����Ă��܂���(ACL)" : "Connection from this address is not admitted.(ACL)"; break;
                    case 9000017: message = (_kernel.IsJp()) ? "���̃A�h���X����̐ڑ��͋�����Ă��܂���(ACL)" : "Connection from this address is not admitted.(ACL)"; break;
                    case 9000018: message = (_kernel.IsJp()) ? "���̗��p�҂̃A�N�Z�X�͋�����Ă��܂���(ACL)" : "Access of this user is not admitted (ACL)"; break;
                    case 9000019: message = (_kernel.IsJp()) ? "�A�C�h���^�C���A�E�g" : "Timeout of an idle"; break;
                    case 9000020: message = (_kernel.IsJp()) ? "���M�Ɏ��s���܂���" : "Transmission of a message failure"; break;
                    //case 9000021: message = (kernel.IsJp()) ? "OpenSSL�̏������Ɏ��s���܂���" : "Initialization of OpenSSL made a blunder"; break;
                    //case 9000022: message = (kernel.IsJp()) ? "�閧���̓ǂݍ��݂Ɏ��s���܂���" : "Reading of private key made a blunder"; break;
                    case 9000023: message = (_kernel.IsJp()) ? "�ؖ����̓ǂݍ��݂Ɏ��s���܂���" : "Reading of a certificate made a blunder"; break;
                    //case 9000024: message = (kernel.IsJp()) ? "SSL�l�S�V�G�[�V�����Ɏ��s���܂���" : "SSL connection procedure makes a blunder"; break;
                    //case 9000025: message = (kernel.IsJp()) ? "�t�@�C���i�閧���j��������܂���" : "Private key is not found"; break;
                    case 9000026: message = (_kernel.IsJp()) ? "�t�@�C���i�ؖ����j��������܂���" : "A certificate is not found"; break;
                    //case 9000027: message = (kernel.IsJp()) ? "OpenSSL�̃��C�u����(ssleay32.dll,libeay32.dll)��������܂���" : "OpenSSL library (ssleay32.dll,libeay32.dll) is not found"; break;
                    case 9000028: message = (_kernel.IsJp()) ? "SSL�̏������Ɏ��s���Ă��܂�" : "Initialization of SSL made a blunder"; break;
                    case 9000029: message = (_kernel.IsJp()) ? "�w�肳�ꂽ��ƃf�B���N�g�������݂��܂���" : "A work directory is not found"; break;
                    case 9000030: message = (_kernel.IsJp()) ? "�N������T�[�o��������܂���" : "A starting server is not found"; break;
                    case 9000031: message = (_kernel.IsJp()) ? "�u���O�̕ۑ��ꏊ�v��������܂���" : "Log Save place(directory) is not found"; break;
                    case 9000032: message = (_kernel.IsJp()) ? "���O�폜" : "Delete LogFile"; break;
                    //case 9000033: message = (kernel.IsJp())?"���[���{�b�N�X�Ƀ��[�����i�[����܂���" : "An email was stored to a mailbox";break;
                    case 9000034: message = (_kernel.IsJp()) ? "ACL�w��ɖ�肪����܂�" : "ACL configuration failure"; break;
                    case 9000035: message = (_kernel.IsJp()) ? "Socket()�ŃG���[���������܂����B[TCP]" : "An error occurred in Socket() [TCP]"; break;
                    case 9000036: message = (_kernel.IsJp()) ? "Socket()�ŃG���[���������܂����B[UDP]" : "An error occurred in Socket() [UDP]"; break;
                    case 9000037: message = (_kernel.IsJp()) ? "_subThread()�ŗ�O���������܂���" : "An exception occurred in _subThread()"; break;
                    case 9000038: message = (_kernel.IsJp()) ? "�y��O�z" : "[Exception]"; break;
                    case 9000039: message = (_kernel.IsJp()) ? "�ySTDOUT�z" : "[STDOUT]"; break;
                    case 9000040: message = (_kernel.IsJp()) ? "�g��SMTP�K�p�͈͂̎w��ɖ�肪����܂�" : "ESMTP range configuration failure"; break;
                    case 9000041: message = (_kernel.IsJp()) ? "disp2()�ŗ�O���������܂���" : "An exception occurred in disp2()"; break;
                    case 9000042: message = (_kernel.IsJp()) ? "�������Ɏ��s���Ă��邽�߃T�[�o���J�n�ł��܂���" : "Can't start a server in order to fail in initialization"; break;
                    case 9000043: message = (_kernel.IsJp()) ? "�N���C�A���g�����ؒf����܂���" : "The client side was cut off"; break;
                    case 9000044: message = (_kernel.IsJp()) ? "�T�[�o�����ؒf����܂���" : "The server side was cut off"; break;
                    case 9000045: message = (_kernel.IsJp()) ? "���O�폜�����œ��t�ϊ��ɗ�O���������܂���" : "An exception occurred in date conversion by log elimination processing"; break;
                    case 9000046: message = (_kernel.IsJp()) ? "socket.send()�ŃG���[���������܂���" : "socket.send()"; break;
                    case 9000047: message = (_kernel.IsJp()) ? "���[�U���������ł�" : "A user name is null and void"; break;
                    case 9000048: message = (_kernel.IsJp()) ? "ThreadBase::Loop()�ŗ�O���������܂���" : "An exception occurred in ThreadBase::Loop()"; break;
                    case 9000049: message = (_kernel.IsJp()) ? "�y��O�z" : "[Exception]"; break;
                    case 9000050: message = (_kernel.IsJp()) ? "�t�@�C���ɃA�N�Z�X�ł��܂���ł���" : "Can't open a file"; break; 
                    case 9000051: message = (_kernel.IsJp()) ? "�C���X�^���X�̐����Ɏ��s���܂���" : "Can't create instance"; break; 
                    case 9000052: message = (_kernel.IsJp()) ? "���O�����Ɏ��s���܂���" : "Non-existent domain"; break; 
                    case 9000053: message = (_kernel.IsJp()) ? "�y��O�zSockObj.Resolve()":"[Exception] SockObj.Resolve()"; break; 
                    case 9000054: message = (_kernel.IsJp()) ? "Apache Killer�ɂ��U���̉\��������܂�":"There is possibility of attack by Apache Killer in it"; break; 
                    case 9000055: message = (_kernel.IsJp()) ? "�y�������ہz�uACL�v�̋֎~���闘�p�ҁi�A�h���X�j�ɒǉ����܂���" : "Add it to a deny list automatically"; break;
                    case 9000056: message = (_kernel.IsJp()) ? "�s���A�N�Z�X�����o���܂������AACL�u���ہv���X�g�͒ǉ�����܂���ł���" : "I detected possibility of Attack, but the ACL [Deny] list was not added";break;
                    case 9000057: message = (_kernel.IsJp()) ? "�y��O�z" : "[Exception]"; break;
                    case 9000058: message = (_kernel.IsJp()) ? "���[���̑��M�Ɏ��s���܂���" : "Failed in the transmission of a message of an email"; break;
                    case 9000059: message = (_kernel.IsJp()) ? "���[���̕ۑ��Ɏ��s���܂���" : "Failed in a save of an email"; break;

                }
            }
            string dateStr = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            string messageNoStr = string.Format("{0:D7}",messageNo);
            string remoteAddrStr = (sockObj == null) ? "-" : sockObj.RemoteHost;

            var oneLog = new OneLog(dateStr,logKind.ToString(),_nameTag,threadId.ToString(),remoteAddrStr,messageNoStr,message,detailInfomation);


            //���O�t�@�C���ւ̒ǉ�
            if(_kernel.LogFile!=null)
                _kernel.LogFile.Append(oneLog);
           
        }

        //Ver5.3.2
        public void Exception(Exception ex,SockObj sockObj,int messageNo) {
            Set(LogKind.Error, sockObj, messageNo, ex.Message);
            string[] tmp = ex.StackTrace.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in tmp) {
                var lines = new List<string>();
                var l = Util.SwapStr("\r\n", "", s);
                while (true) {
                    if (l.Length < 80) {
                        lines.Add(l);
                        break;
                    }
                    lines.Add(l.Substring(0, 80));
                    l = l.Substring(80);
                }
                for (int i = 0; i < lines.Count; i++) {
                    if (i == 0) {
                        Set(LogKind.Error, sockObj, messageNo, lines[i]);
                    } else {
                        Set(LogKind.Error, sockObj, messageNo, "   -" + lines[i]);
                    }
                }
            }
        }
    }

}
*/