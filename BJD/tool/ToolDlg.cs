using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Bjd.net;
using Bjd.remote;
using Bjd.server;
using Bjd.sock;

namespace Bjd {
    public abstract partial class ToolDlg : Form {
        protected Kernel Kernel;
        readonly string _caption;
        protected string NameTag;
        protected Control MainControl;//���C���R���g���[��

        new abstract public void Closed();//�_�C�A���O������ۂɌĂяo�����
        abstract public void Clear();//���C���R���g���[���̃N���A
        abstract public void AddItem(string line);//���C���R���g���[���ւ̃f�[�^�ǉ�
        abstract public void Recv(string cmdStr, string buffer);//�R�}���h�ւ̉���

        //�ʏ�̏ꍇ�AServer������������
        //�����[�g�N���C�A���g�̏ꍇ�ATcpObj�������������
        protected OneServer Server;
        protected SockTcp sockTcp;

        public delegate void MenuFunc();
        ContextMenuStrip _popupMenu;

        protected ToolDlg(Kernel kernel,string nameTag,Object obj,string caption) {
            InitializeComponent();

            Kernel = kernel;
            NameTag = nameTag;
            _caption = caption;
        
            if (kernel.RunMode == RunMode.Remote) {
                sockTcp = (SockTcp)obj;
            } else {
                Server = (OneServer)obj;
            }

            Text = caption;

            //�E�C���h�E�T�C�Y�̕���
            kernel.WindowSize.Read(this);

            //MainMenuFile.Text = (kernel.IsJp()) ? "�t�@�C��(&F)" : "&File";
            //MainMenuClose.Text = (kernel.IsJp()) ? "����(&C)" : "&Close";
        }

        public override sealed string Text{
            get { return base.Text; }
            set { base.Text = value; }
        }

        //���C�����j���[�̒ǉ�
        protected ToolStripMenuItem AddMenu(ToolStripMenuItem parent, MenuFunc menuFunc, string title,Keys keys) {
            ToolStripItem item = null;
            if (parent == null) {
                item = menuStrip.Items.Add(title);
            } else {
                if(title=="-")
                    parent.DropDownItems.Add(new ToolStripSeparator());
                else
                    item = parent.DropDownItems.Add(title);
            }

            if (item != null) {
                item.Click += MenuClick;
                item.Tag = menuFunc;
                if (keys != Keys.None)
                    ((ToolStripMenuItem)item).ShortcutKeys = keys;

            }
            return (ToolStripMenuItem)item;
        }
        //�|�b�v�A�b�v���j���[�̒ǉ�
        protected void AddPopup(MenuFunc menuFunc, string title) {
            if (_popupMenu != null) {
                var item = _popupMenu.Items.Add(title);
                item.Click += MenuClick;
                item.Tag = menuFunc;
            }
        }
        //���C�����j���[�ƃ|�b�v�A�b�v���j���[�̗����ւ̒ǉ�
        protected ToolStripMenuItem Add2(ToolStripMenuItem parent, MenuFunc menuFunc, string title,Keys keys) {
            //�|�b�v�A�b�v���j���[�̒ǉ�
            AddPopup(menuFunc, title);
            //���C�����j���[�̒ǉ�
            return AddMenu(parent, menuFunc, title, keys);

        }

        private void MenuClick(object sender, EventArgs e) {
            try{
                var item = (ToolStripMenuItem)sender;
                if (item.Tag != null)
                    ((MenuFunc)item.Tag)();
            }catch{
            }
        }
        protected void FuncClose() {
            Close();
        }

        //���C���R���g���[���̒ǉ�
        protected void AddControl(Control control){
            MainControl = control;
            
            //�e���|����
            var list = new List<Control>();

            SuspendLayout();

            for (var i = 0; i < Controls.Count;i++ ) {
                list.Add(Controls[i]);
                if (i == 0) {
                    list.Add(MainControl);
                }
            }
            Controls.Clear();
            foreach (var t in list){
                Controls.Add(t);
            }
            ResumeLayout();

            _popupMenu = new ContextMenuStrip();
            MainControl.ContextMenuStrip = _popupMenu;
        }

        //�_�C�A���O�N���[�Y���̃C�x���g����
        private void ToolDlgFormClosed(object sender, FormClosedEventArgs e) {
            Closed();
            //�E�C���h�E�T�C�Y�̕ۑ�
            Kernel.WindowSize.Save(this);
            Kernel.View.Activated();
        }
        //�X�e�[�^�X�o�[�ւ̃e�L�X�g�\��
        protected void SetStatusText(string text) {
            StatusLabel.Text = text;
        }


        protected void Cmd(string cmdStr) {
            if (MainControl.InvokeRequired) {
                MainControl.Invoke(new MethodInvoker(()=>Cmd(cmdStr)));
            } else { // ���C���X���b�h����Ăяo���ꂽ�ꍇ(�R���g���[���ւ̕`��)
                if (cmdStr.IndexOf("Refresh") == 0) {
                    //���C���R���g���[���̃N���A
                    Clear();

                    //�f�[�^�擾�̂��ߕ\���ҋ@
                    //�X�e�[�^�X�o�[�ւ̃e�L�X�g�\��
                    SetStatusText("");
                    MainControl.BackColor = SystemColors.ButtonFace;
                    MainControl.Update();
                    Text = "���擾���ł��B���΂炭���҂����������B";
                }

                if (Kernel.RunMode == RunMode.Remote) {
                    //�iToolDlg�p�j�f�[�^�v��(C->S)
                    RemoteData.Send(sockTcp, RemoteDataKind.CmdTool, string.Format("{0}-{1}", NameTag, cmdStr));
                } else {

                    if (Server != null) {
                        var buffer = Server.Cmd(cmdStr);//�����[�g����i�f�[�^�擾�j
                        CmdRecv(cmdStr, buffer);
                    } else {
                        CmdRecv(cmdStr, "");
                    }
                }
            }
        }

        public void CmdRecv(string cmdStr,string buffer) {
            if (MainControl.InvokeRequired) {
                MainControl.Invoke(new MethodInvoker(()=>CmdRecv(cmdStr,buffer)));
            } else { // ���C���X���b�h����Ăяo���ꂽ�ꍇ(�R���g���[���ւ̕`��)
                if (cmdStr.IndexOf("Refresh-")==0) {
                    string[] lines = buffer.Split(new char[] { '\b' }, StringSplitOptions.RemoveEmptyEntries);

                    //�f�[�^�擾�̂��ߕ\���ҋ@�i�����j
                    MainControl.BackColor = SystemColors.Window;
                    MainControl.Update();
                    Text = _caption;

                    Kernel.Wait.Max = 100;
                    Kernel.Wait.Start("���΂炭���҂����������B");


                    var max = lines.Length;
                    Kernel.Wait.Max = max;
                    for (var i = 0; i < max && Kernel.Wait.Life; i++) {

                        Kernel.Wait.Val = i;
                        Thread.Sleep(1);
                        AddItem(lines[i]);
                    }

                    //�X�e�[�^�X�o�[�ւ̃e�L�X�g�\��
                    Kernel.Wait.Stop();
                } else if (cmdStr.IndexOf("Cmd-") == 0) {
                    Recv(cmdStr,buffer);//�R�}���h�ւ̉���(�q�N���X�Ŏ��������)
                }
            }
        }

    }
}