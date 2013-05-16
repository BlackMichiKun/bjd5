using System.Collections.Generic;
using Bjd;
using Bjd.log;
using Bjd.net;
using Bjd.option;
using Bjd.server;
using Bjd.sock;
using Bjd.util;

namespace ProxyFtpServer {

    public partial class Server : OneServer {
        private const int DataPortMin = 10000;
        private const int DataPortMax = 11000;
        int _dataPort;

        public Server(Kernel kernel, Conf conf, OneBind oneBind)
            : base(kernel, conf, oneBind) {

            _dataPort = DataPortMin;

        }

        override protected bool OnStartServer() { return true; }
        override protected void OnStopServer() { }
        //接続単位の処理
        override protected void OnSubThread(SockObj sockObj) {

            var timeout = (int)Conf.Get("timeOut");

            var client = (SockTcp)sockObj;
            SockTcp server = null;

            var user = "";//ユーザ名
            string pass;//パスワード
            var hostName = "";//ホスト名

            //***************************************************************
            //前処理（接続先・ユーザ名・パスワードの取得)
            //***************************************************************
            {
                var str = string.Format("220 {0} {1}", Define.ApplicationName(), Define.Copyright());
                client.AsciiSend(str);

                var cmdStr = "";
                var paramStr = "";
                //wait USER user@hostName
                if (!WaitLine(client, ref cmdStr, ref paramStr)) {
                    goto end;
                }
                if (cmdStr.ToUpper() != "USER")
                    goto end;

                //paramStr = "user@hostName"
                if (paramStr != null) {
                    //string[] tmp = paramStr.Split('@');
                    //if(tmp.Length == 2) {
                    //    user = tmp[0];//ユーザ名取得
                    //    hostName = tmp[1];//ホスト名取得
                    //}
                    var i = paramStr.LastIndexOf('@');
                    if (i != -1) {
                        user = paramStr.Substring(0, i);//ユーザ名取得
                        hostName = paramStr.Substring(i + 1);//ホスト名取得
                    }
                }
                if (hostName == "") {
                    Logger.Set(LogKind.Error, sockObj, 8, "");
                    goto end;
                }

                client.AsciiSend("331 USER OK enter password");

                //wait PASS password
                if (!WaitLine(client, ref cmdStr, ref paramStr)) {
                    goto end;
                }
                if (cmdStr.ToUpper() != "PASS")
                    goto end;
                //paramStr = "password"
                pass = paramStr;//パスワード取得
            }
            //***************************************************************
            // サーバとの接続
            //***************************************************************
            {
                const int port = 21;

                //var ipList = new List<Ip>{new Ip(hostName)};
                //if (ipList[0].ToString() == "0.0.0.0") {
                //    ipList = Kernel.DnsCache.Get(hostName);
                //    if (ipList.Count == 0) {
                //        goto end;
                //    }
                //}
                var ipList = Kernel.GetIpList(hostName);
                if (ipList.Count == 0) {
                    goto end;
                }

                Ssl ssl = null;

                foreach (var ip in ipList) {
                    server = Inet.Connect(Kernel,ip, port,Timeout, ssl);
                    if (server != null)
                        break;
                }
                if (server == null)
                    goto end;
            }
            //***************************************************************
            //後処理（ユーザ名・パスワードの送信)
            //***************************************************************
            {
                var cmdStr = "";
                var paramStr = "";
                //wait 220 welcome
                while (cmdStr != "220") {
                    if (!WaitLine(server, ref cmdStr, ref paramStr)) {
                        goto end;
                    }
                }
                server.AsciiSend(string.Format("USER {0}", user));
                //wait 331 USER OK enter password
                while (cmdStr != "331") {
                    if (!WaitLine(server, ref cmdStr, ref paramStr)) {
                        goto end;
                    }
                }
                server.AsciiSend(string.Format("PASS {0}", pass));
                if (!WaitLine(server, ref cmdStr, ref paramStr)) {
                    goto end;
                }
                client.AsciiSend(string.Format("{0} {1}", cmdStr, paramStr));
            }

            //***************************************************************
            // パイプ
            //***************************************************************
            var ftpTunnel = new FtpTunnel(Kernel, Logger, (int)Conf.Get("idleTime"), _dataPort, timeout);
            //Ver5.0.5
            //ftpTunnel.BytePipe(ref life, server,client);
            ftpTunnel.Pipe(server, client,this);
            _dataPort = ftpTunnel.Dispose();
            if (_dataPort > DataPortMax)
                _dataPort = DataPortMin;
        end:
            client.Close();
            if (server != null)
                server.Close();
        }
        //RemoteServerでのみ使用される
        public override void Append(OneLog oneLog) {

        }

    }

}
