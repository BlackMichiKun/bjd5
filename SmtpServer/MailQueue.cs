using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using Bjd;
using Bjd.mail;

namespace SmtpServer {
    class MailQueue {
        //Ver5.4.8
        readonly object _lockObj = new Object();

        public MailQueue(string currentDirectory) {

            Status = true;//初期化状態 falseの場合は、初期化に失敗しているので使用できない

            //基底クラスのstring dirの初期化
            Dir = string.Format("{0}\\MailQueue", currentDirectory);
            if (Directory.Exists(Dir))
                return;
            try {
                Directory.CreateDirectory(Dir);
            } catch {
                Status = false;//初期化失敗
                Dir = null;
            }
        }
        public bool Status { get; private set; }//初期化状態 falseの場合は、初期化に失敗しているので使用できない
        public string Dir { get; private set; }

        //重複しないファイル名を取得する
        protected string CreateName() {
            while (true) {
                var str = string.Format("{0:D20}", DateTime.Now.Ticks);
                Thread.Sleep(1);//Ver5.0.0-b18
                var fileName = string.Format("{0}\\MF_{1}", Dir, str);
                if (!Directory.Exists(fileName)) {
                    return str;
                }
            }
        }

        public List<OneQueue> GetList(int max, int threadSpan) {

            //DateTime now = DateTime.Now;

            var queueList = new List<OneQueue>();

            //Ver5.4.8
            //lock (this) {//排他制御
            lock (_lockObj) {//排他制御
                foreach (var fileName in Directory.GetFiles(Dir, "DF_*")) {
                    if (queueList.Count == max)
                        break;
                    var mailInfo = new MailInfo(fileName);

                    //処理対象かどうかの確認
                    if (mailInfo.IsProcess(threadSpan, fileName)) {
                        var fname = Path.GetFileName(fileName);
                        //          if(Sw || Df.State==1){
                        queueList.Add(new OneQueue(fname.Substring(3), mailInfo));
                    }
                }
                return queueList;
            }
        }
        public void Delete(string fname) {
            //Ver5.4.8
            //lock (this) {//排他制御
            lock (_lockObj) {//排他制御
                var fileName = string.Format("{0}\\MF_{1}", Dir, fname);
                File.Delete(fileName);
                fileName = string.Format("{0}\\DF_{1}", Dir, fname);
                File.Delete(fileName);
            }
        }
        //public bool Save(Mail mail,MailAddress from, MailAddress to, string host, string addr, string date, string uid) {
        public bool Save(Mail mail, MailInfo mailInfo) {

            //Ver5.4.8
            //lock (this) {//排他制御
            lock (_lockObj) {//排他制御
                var fname = CreateName();
                var fileName = string.Format("{0}\\MF_{1}", Dir, fname);
                if (mail.Save(fileName)) {
                    fileName = string.Format("{0}\\DF_{1}", Dir, fname);
                    mailInfo.Save(fileName);
                    return true;
                }
                return false;
            }
        }
        public bool Read(string fname, ref Mail mail) {
            //Ver5.4.8
            //lock (this) {//排他制御
            lock (_lockObj) {//排他制御
                var fileName = string.Format("{0}\\MF_{1}", Dir, fname);
                return mail.Read(fileName);
            }
        }
    }
}

