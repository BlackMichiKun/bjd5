using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using Bjd.log;
using Bjd.net;
using Bjd.option;
using Bjd.util;

//MD5

namespace Bjd.mail{

    public class MailBox{
        private readonly List<OneMailBox> _ar = new List<OneMailBox>();
        private readonly Logger _logger; //�e�X�g�̍ۂ�null�ł����v

        public string Dir { get; private set; }
        public bool Status { get; private set; } //���������ۂ̊m�F
        //���[�U�ꗗ
        public List<string> UserList {
            get {
                return _ar.Select(o => o.User).ToList();
            }
        }

        public MailBox(Logger logger,Dat datUser,String dir){
            Status = true; //��������� false�̏ꍇ�́A�������Ɏ��s���Ă���̂Ŏg�p�ł��Ȃ�

            _logger = logger;

            //MailBox��z�u����t�H���_
            Dir = dir;
            //Dir = kernel.ReplaceOptionEnv(Dir);

            try{
                Directory.CreateDirectory(Dir);
            } catch{

            }

            if (!Directory.Exists(Dir)){
                if (_logger != null){
                    _logger.Set(LogKind.Error, null, 9000029, string.Format("dir="));
                }
                Status = false;
                Dir = null;
                return; //�ȍ~�̏��������������Ȃ�
            }

            //���[�U���X�g�̏�����
            Init(datUser);

        }


        //���[�U���X�g�̏�����
        private void Init(Dat datUser){
            _ar.Clear();
            if (datUser != null){
                foreach (var o in datUser) {
                    if (!o.Enable)
                        continue; //�L���ȃf�[�^������Ώۂɂ���
                    var name = o.StrList[0];
                    var pass = Crypt.Decrypt(o.StrList[1]);
                    _ar.Add(new OneMailBox(name, pass));
                    var folder = string.Format("{0}\\{1}", Dir, name);
                    if (!Directory.Exists(folder)){
                        Directory.CreateDirectory(folder);
                    }
                }
            }
        }

        protected string CreateName(){
            lock (this){
                //Ver5.0.4 �X���b�h�Z�[�t�̊m��
                while (true){
                    var str = string.Format("{0:D20}", DateTime.Now.Ticks);
                    Thread.Sleep(1); //Ver5.0.4 �E�G�C�g��DateTIme.Now�̏d���������
                    var fileName = string.Format("{0}\\MF_{1}", Dir, str);
                    if (!File.Exists(fileName)){
                        return str;
                    }
                }
            }
        }


        public bool Save(string user, Mail mail, MailInfo mailInfo){
            //Ver_Ml
            if (!IsUser(user)){
                if (_logger != null){
                    _logger.Set(LogKind.Error, null, 9000047, string.Format("[{0}] {1}", user, mailInfo));
                }
                return false;
            }

            var folder = string.Format("{0}\\{1}", Dir, user);
            if (!Directory.Exists(folder)){
                Directory.CreateDirectory(folder);
            }

            var name = CreateName();

            //logger.Set(LogKind.Debug,null,7777,name);

            string fileName = string.Format("{0}\\MF_{1}", folder, name);
            if (mail.Save(fileName)){
                fileName = string.Format("{0}\\DF_{1}", folder, name);
                mailInfo.Save(fileName);
                return true;
            }
            return false;
        }

        //���[�U�����݂��邩�ǂ���
        public bool IsUser(string user){
            return _ar.Any(o => o.User == user);
        }

        //�Ō�Ƀ��O�C���ɐ������������̎擾 (PopBeforeSMTP�p�j
        public DateTime LastLogin(Ip addr){
            foreach (OneMailBox oneMailBox in _ar.Where(oneMailBox => oneMailBox.Addr == addr.ToString())){
                return oneMailBox.Dt;
            }
            return new DateTime(0);
        }

        //�p�X���[�h�ύX
        public bool Chps(string user, string pass,Conf conf){
            if (pass == null){
                //�����ȃp�X���[�h�̎w��͎��s����
                return false;
            }
            if (_ar.Any(oneUser => oneUser.User == user)){
                var dat = (Dat) conf.Get("user");
                foreach (var o in dat.Where(o => o.StrList[0] == user)){
                    o.StrList[1] = Crypt.Encrypt(pass);
                    break;
                }
                conf.Set("user", dat); //�f�[�^�ύX
                Init(dat); //���[�U���X�g�̏������i�ēǍ��j
                return true;
            }
            return false;
        }

        //�F�؁i�p�X���[�h�m�F) ���p�X���[�h�̖������[�U�����݂���?
        public bool Auth(string user, string pass){
            foreach (var o in _ar){
                if (o.User == user){
                    return o.Pass == pass;
                }
            }
            return false;
        }

        //�p�X���[�h�擾
        public string GetPass(string user){
            return (from oneUser in _ar where oneUser.User == user select oneUser.Pass).FirstOrDefault();
        }

        //�F�؁i�p�X���[�h�m�F) APOP�Ή�
        public bool Auth(string user, string authStr, string recvStr){
            foreach (OneMailBox o in _ar){
                if (o.User != user)
                    continue;
                if (o.Pass == null) //�p�X���[�h������
                    return false;

                var data = Encoding.ASCII.GetBytes(authStr + o.Pass);
                var md5 = new MD5CryptoServiceProvider();
                var result = md5.ComputeHash(data);
                var sb = new StringBuilder();
                for (int i = 0; i < 16; i++){
                    sb.Append(string.Format("{0:x2}", result[i]));
                }
                if (sb.ToString() == recvStr)
                    return true;
                return false;
            }
            return false;
        }

        public string Login(string user, Ip addr){
            foreach (var oneUser in _ar){
                if (oneUser.User != user)
                    continue;
                if (oneUser.Login(addr.ToString())){
                    return string.Format("{0}\\{1}", Dir, user);
                }
            }
            return null;
        }

        public void Logout(string user){
            foreach (var oneUser in _ar){
                if (oneUser.User == user){
                    oneUser.Logout();
                    return;
                }
            }
        }
    }
}
