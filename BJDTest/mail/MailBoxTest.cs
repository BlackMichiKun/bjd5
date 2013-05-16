﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bjd.mail;
using Bjd.net;
using Bjd.option;
using BjdTest.test;
using NUnit.Framework;
using Bjd;
using System.Security.Cryptography;
using System.Threading;

namespace BjdTest.mail {
    [TestFixture]
    internal class MailBoxTest{
        
        private MailBox sut;
        private TmpOption _op;


        [SetUp]
        public void SetUp(){
            _op = new TmpOption("BjdTest","MailBoxTest.ini");
            var kernel = new Kernel();
            var  oneOption = kernel.ListOption.Get("MailBox");
            var conf = new Conf(oneOption);
            sut = new MailBox(kernel, conf);
        }

        [TearDown]
        public void TearDown(){
            Thread.Sleep(100);
            //後始末で、MainBoxフォルダごと削除する
            if(Directory.Exists(sut.Dir)){
                try{
                    Directory.Delete(sut.Dir);
                }catch{
                    Directory.Delete(sut.Dir, true);
                }
            }
            _op.Dispose();
        }

        [TestCase]
        public void ステータス確認(){
            //setUp
            var expected = true;
            //exercise
            var actual = sut.Status;
            //verify
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("user1",true)]
        [TestCase("user2", true)]
        [TestCase("user3", true)]
        [TestCase("", false)]
        [TestCase("xxx", false)]
        [TestCase(null, false)]
        public void IsUserによるユーザの存在確認(string user, bool expected) {
            //exercise
            var actual = sut.IsUser(user);
            //verify
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("user1", "user1")]
        [TestCase("user2", "user2")]
        [TestCase("user3", null)]//user3は、無効なパスワードで初期化されている
        [TestCase("xxx", null)]//存在しないユーザの問い合わせ
        public void GetPassによるパスワードの取得(string user, string expected) {
            //exercise
            var actual = sut.GetPass(user);
            //verify
            Assert.That(actual, Is.EqualTo(expected));

        }

        [TestCase("user1", "user1", true)]
        [TestCase("user2", "user2",true)]
        [TestCase("user1", "xxx", false)]//パスワード誤り
        [TestCase("user3", "user3", false)]//パスワードが無効
        [TestCase("xxx", "xxx", false)]//登録外のユーザ
        [TestCase(null, "xxx", false)]//ユーザ名が無効（不正）
        [TestCase("user1", null, false)]//パスワードが無効（不正）
        public void Authによる認証(string user, string pass, bool expected) {
            //exercise
            var actual = sut.Auth(user,pass);
            //verify
            Assert.That(actual, Is.EqualTo(expected));
        }


        [TestCase("user1", "user1", true)]
        [TestCase("user1", "xxx", false)]//パスワードが間違えた場合失敗する
        [TestCase("user4", "", false)]//登録されていないユーザは失敗する
        [TestCase("user3", "", false)]//パスワードが無効のユーザは、失敗する
        public void Authによる認証_チャレンジ文字列対応(string user, string pass, bool expected) {
            //setUp
            const string challengeStr = "solt";
            byte[] data = Encoding.ASCII.GetBytes(challengeStr + pass);
            MD5 md5 = new MD5CryptoServiceProvider();

            
            byte[] result = md5.ComputeHash(data);
            var sb = new StringBuilder();
            for (int i = 0; i < 16; i++) {
                sb.Append(string.Format("{0:x2}", result[i]));
            }

            //exercise
            var actual = sut.Auth(user, challengeStr, sb.ToString());
            //verify
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("user1", "192.168.0.1")]
        [TestCase("user1", "10.0.0.1")]
        [TestCase("user2", "10.0.0.1")]
        [TestCase("user3", "10.0.0.1")]
        public void Loginによるログイン処理_成功(string user, string ip){
            //setUp
            //成功した場合、排他用ファイルのパスが返される
            var expected = string.Format("{0}\\{1}", sut.Dir, user);
            //exercise
            var actual = sut.Login(user, new Ip(ip));
            //verify
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("xxx", "10.0.0.1")]//無効ユーザではログインできない
        [TestCase(null, "10.0.0.1")]//無効(不正)ユーザではログインできない
        public void Loginによるログイン処理_失敗(string user, string ip) {
            //setUp
            string expected = null; //失敗した場合はnullが返される
            //exercise
            var actual = sut.Login(user, new Ip(ip));
            //verify
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Login_二重ログインでnullが返る() {
            //setUp
            var ip = new Ip("10.0.0.1");
            const string user = "user1";
            sut.Login(user, ip); //1回目のログイン
            string expected = null;//失敗した場合nullが返される
            //exercise
            var actual = sut.Login(user, ip); //２回目のログイン
            //verify
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Login_二重ログイン後にログアウトすればログインは成功する() {
            //setUp
            const string user = "user1";
            var ip = new Ip("10.0.0.1");
            var expected = string.Format("{0}\\{1}", sut.Dir, user);//成功した場合、排他用ファイルのパスが返される
            sut.Login(user, ip); //1回目のログイン
            sut.Login(user, ip); //2回目のログイン
            sut.Logout(user); //ログアウト
            //exercise
            var actual = sut.Login(user, ip); //２回目のログイン
            //verify
            Assert.That(actual, Is.EqualTo(expected));
        }



        [TestCase("user1", 0)]
        [TestCase("user1", 1000)]//１秒経過
        public void LastLoginによる最終ログイン時間の取得(string user, int waitMsec) {
            //Ticksは100ナノ秒単位
            //10で１マイクロ秒
            //10000で１ミリ秒
            //10000000で１秒
            //setUp
            var ip = new Ip("10.0.0.1");
            var now = DateTime.Now;//ログイン直前の時間計測
            sut.Login(user, ip);//ログイン
            Thread.Sleep(waitMsec);//経過時間
            var expected = true;

            //exercise
            var dt = sut.LastLogin(ip);//ログイン後の時間計測
            var actual = (dt.Ticks - now.Ticks) < 100000; //１ミリ秒以下の誤差
            //verify
            Assert.That(actual, Is.EqualTo(expected));

        }
        [TestCase("xxx")]//無効(不正)ユーザではログインできない
        [TestCase(null)]//無効(不正)ユーザではログインできない
        public void LastLoginによる最終ログイン時間の取得_無効ユーザの場合0が返る(string user) {
            //setUp
            var ip = new Ip("10.0.0.1");
            sut.Login(user, ip);//ログイン
            var expected = 0;
            //exercise
            var actual = sut.LastLogin(ip).Ticks;
            //verify
            Assert.That(actual, Is.EqualTo(expected));

        }


        [TestCase("user1",false, true)]
        [TestCase("user1", true, true)]//ログアウトしても経過時間の取得は成功する
        [TestCase("xxx", false, false)]//無効ユーザ
        public void LogoutTest(string user, bool logout, bool success) {
            var ip = new Ip("10.0.0.1");
            sut.Login(user, ip);//ログイン
            if(logout){
                sut.Logout(user);
            }
            var dt = sut.LastLogin(ip);//ログイン後の時間計測
            if(success){
                Assert.AreNotEqual(dt.Ticks,0);//過去にログインした記録があれば0以外が返る
            }else{
                Assert.AreEqual(dt.Ticks, 0);//過去にログイン形跡なし
            }
            sut.Logout(user);
        }

        [TestCase("user1", "123")]//user1のパスワードを123に変更する
        [TestCase("user3", "123")]//user3のパスワードを123に変更する
        public void Chpsによるパスワード変更(string user, string pass) {
            //setUp
            bool expected = true;

            //exercise
            var actual = sut.Chps(user, pass);
            //verify
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("user1", "123")]//user1のパスワードを123に変更する
        [TestCase("user3", "123")]//user3のパスワードを123に変更する
        public void Chpsによるパスワード変更_変更が成功しているかどうかの確認(string user, string pass) {
            //setUp
            var expected = pass;
            sut.Chps(user, pass);
            //exercise
            var actual = sut.GetPass(user);
            //verify
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("user1", null)]//無効パスワードの指定は失敗する
        [TestCase("xxx", "123")]//無効ユーザのパスワード変更は失敗する
        [TestCase(null, "123")]//無効ユーザのパスワード変更は失敗する
        public void Chpsによるパスワード変更_失敗するとfalseが返る(string user, string pass) {
            //setUp
            bool expected = false;

            //exercise
            var actual = sut.Chps(user, pass);
            //verify
            Assert.That(actual, Is.EqualTo(expected));
        }




        
        [Test]
        public void UserListによるユーザ一覧取得() {
            //exercise
            var actual = sut.UserList;
            //verify
            Assert.That(actual.Count, Is.EqualTo(3));
            Assert.That(actual[0], Is.EqualTo("user1"));
            Assert.That(actual[1], Is.EqualTo("user2"));
            Assert.That(actual[2], Is.EqualTo("user3"));
        }


        //保存件数（ファイル数)
        [TestCase(1)]
        [TestCase(3)]
        public void SaveCountTest(int n) {
            var mail = new Mail(null);
            const string uid = "XXX123";
            const int size = 100;
            const string host = "hostname";
            const string user = "user1";
            var ip = new Ip("10.0.0.1");
            var date = DateTime.Now.ToString();
            var from = new MailAddress("1@1");
            var to = new MailAddress("2@2");
            var mailInfo = new MailInfo(uid, size, host, ip, date, from, to);

            //同一内容でn回送信
            for (int i = 0; i < n; i++){
                sut.Save(user, mail, mailInfo);
            }

            //メールボックス内に蓄積されたファイル数を検証する
            var path = string.Format("{0}\\{1}", sut.Dir, user);
            var di = new DirectoryInfo(path);
         
            //DF_*がn個存在する
            var files = di.GetFiles("DF_*");
            Assert.AreEqual(files.Count(), n);
            //MF_*がn個存在する
            files = di.GetFiles("MF_*");
            Assert.AreEqual(files.Count(), n);

        }

        //保存（DF内容)
        [TestCase("user1",true,"UID",100,"hostname","1@1","2@2")]
        [TestCase("zzzz", false, "", 0, "", "", "")]//無効ユーザで保存失敗
        public void SaveDfTest(string user, bool status, string uid, int size, string hostname, string from, string to) {
            var mail = new Mail(null);
            var ip = new Ip("10.0.0.1");
            var date = DateTime.Now.ToString();
            var mailInfo = new MailInfo(uid, size, hostname, ip, date,new MailAddress(from),new MailAddress(to));

            var b = sut.Save(user, mail, mailInfo);
            //メールボックス内に蓄積されたファイル数を検証する
            var path = string.Format("{0}\\{1}", sut.Dir, user);
            var di = new DirectoryInfo(path);
            
            if (status){
                Assert.AreEqual(b,true);//保存成功
            
                var files = di.GetFiles("DF_*");
                
                //メールボックス内に蓄積されたファイル数を検証する
                var lines = File.ReadAllLines(files[0].FullName);
                Assert.AreEqual(lines[0], uid); //１行目 uid
                Assert.AreEqual(lines[1], size.ToString()); //２行目 size
                Assert.AreEqual(lines[2], hostname); //３行目 hostname
                Assert.AreEqual(lines[3], ip.ToString()); //４行目 ip
                Assert.AreEqual(lines[7], from); //８行目 from
                Assert.AreEqual(lines[8], to); //９行目 to
            }else{
                Assert.AreEqual(b, false);//保存失敗
                Assert.AreEqual(Directory.Exists(di.FullName),false);
            }
        }


    }
}
