using System;
using System.Windows.Forms;
using System.ServiceProcess;

namespace Bjd.service {
    internal partial class SetupServiceDlg : Form {
        readonly SetupService _setupService;
        readonly Kernel _kernel;
        public SetupServiceDlg(Kernel kernel) {
            InitializeComponent();

            _setupService = new SetupService(kernel);

            _kernel = kernel;

            Text = (kernel.IsJp()) ?"�T�[�r�X�ݒ�_�C�A���O":"Setting Service";

            groupBoxInstall.Text = (kernel.IsJp()) ? "�T�[�r�X�ւ̃C���X�g�[��" : "Registration";
            buttonInstall.Text = (kernel.IsJp()) ? "�o�^" : "Install";
            buttonUninstall.Text = (kernel.IsJp()) ? "�폜" : "Uninstall";

            groupBoxStatus.Text = (kernel.IsJp()) ? "���" : "Service status";
            buttonStart.Text = (kernel.IsJp()) ? "�J�n" : "Start";
            buttonStop.Text = (kernel.IsJp()) ? "��~" : "Stop";
            buttonRestart.Text = (kernel.IsJp()) ? "�ċN��" : "Restart";

            groupBoxStartupType.Text = (kernel.IsJp()) ? "�X�^�[�g�A�b�v�̎��" : "Startup type";
            buttonAutomatic.Text = (kernel.IsJp()) ? "����" : "Auto";
            buttonManual.Text = (kernel.IsJp()) ? "�蓮" : "Manual";
            buttonDisable.Text = (kernel.IsJp()) ? "����" : "Disable";
            
            
            DispInit();
        }

        public override sealed string Text{
            get { return base.Text; }
            set { base.Text = value; }
        }

        void DispInit() {

            if (_setupService.IsRegist) {//�T�[�r�X���o�^�ς݂��ǂ���

                //�u�C���X�g�[���v
                textBoxInstall.Text = (_kernel.IsJp()) ? "�o�^" : "Registered";
                buttonInstall.Enabled = false;
                buttonUninstall.Enabled = true;

                //�u��ԁv
                groupBoxStatus.Visible = true;//�O���[�v�\��
                buttonStart.Enabled = true;
                buttonStop.Enabled = false;
                buttonRestart.Enabled = false;
                switch (_setupService.Status) {
                    case ServiceControllerStatus.ContinuePending:
                        textBoxStatus.Text = (_kernel.IsJp()) ? "�ۗ���" : "ContinuePending";
                        break;
                    case ServiceControllerStatus.Paused:
                        textBoxStatus.Text = (_kernel.IsJp()) ? "�ꎞ���f" : "Paused";
                        break;
                    case ServiceControllerStatus.PausePending:
                        textBoxStatus.Text = (_kernel.IsJp()) ? "�ꎞ���f�ۗ���" : "PausePending";
                        break;
                    case ServiceControllerStatus.Running:
                        textBoxStatus.Text = (_kernel.IsJp()) ? "���s��" : "Running";
                        buttonStart.Enabled = false;
                        buttonStop.Enabled = true;
                        buttonRestart.Enabled = true;
                        break;
                    case ServiceControllerStatus.StartPending:
                        textBoxStatus.Text = (_kernel.IsJp()) ? "�J�n��" : "StartPending";
                        break;
                    case ServiceControllerStatus.Stopped:
                        textBoxStatus.Text = (_kernel.IsJp()) ? "��~" : "Stopped";
                        break;
                    case ServiceControllerStatus.StopPending:
                        textBoxStatus.Text = (_kernel.IsJp()) ? "��~��" : "StopPending";
                        break;
                }
                //�X�^�[�g�A�b�v
                groupBoxStartupType.Visible = true;//�O���[�v�\��
                switch (_setupService.StartupType) {
                    case "Auto":
                        textBoxStartupType.Text = (_kernel.IsJp()) ? "����" : "Auto";
                        buttonAutomatic.Enabled = false;
                        buttonManual.Enabled = true;
                        buttonDisable.Enabled = true;
                        break;
                    case "Manual":
                        textBoxStartupType.Text = (_kernel.IsJp()) ? "�蓮" : "Manual";
                        buttonAutomatic.Enabled = true;
                        buttonManual.Enabled = false;
                        buttonDisable.Enabled = true;
                        break;
                    case "Disabled":
                        textBoxStartupType.Text = (_kernel.IsJp()) ? "����" : "Disabled";
                        buttonAutomatic.Enabled = true;
                        buttonManual.Enabled = true;
                        buttonDisable.Enabled = false;
                        break;
                }

                _kernel.RunMode = RunMode.NormalRegist;

            } else {
               
                textBoxInstall.Text = (_kernel.IsJp()) ? "���o�^" : "Not Regist";
                buttonInstall.Enabled = true;
                buttonUninstall.Enabled = false;

                //�u��ԁv�u�X�^�[�g�A�b�v�v�O���[�v��\��
                groupBoxStatus.Visible = false;
                groupBoxStartupType.Visible = false;

                _kernel.RunMode = RunMode.Normal;
            }
            //�u�N��/��~�v���j���[�̏�����
            //kernel.Menu2.InitStartStop(kernel.IsRunnig);
            _kernel.View.SetColor();//�E�C���h�F�̏�����
        }
        //�o�^
        private void ButtonInstallClick(object sender, EventArgs e) {
            Job(ServiceCmd.Install);

        }
        //�폜
        private void ButtonUninstallClick(object sender, EventArgs e) {
�@          Job(ServiceCmd.Uninstall);
        }
        //�J�n
        private void ButtonStartClick(object sender, EventArgs e) {
            Job(ServiceCmd.Start);

        }
        //��~
        private void ButtonStopClick(object sender, EventArgs e) {
            Job(ServiceCmd.Stop);
        }
        //�ċN��
        private void ButtonRestartClick(object sender, EventArgs e) {
            Job(ServiceCmd.Stop);
            Job(ServiceCmd.Start);
        }
        //����
        private void ButtonAutomaticClick(object sender, EventArgs e) {
            Job(ServiceCmd.Automatic);
        }
        //�蓮
        private void ButtonManualClick(object sender, EventArgs e) {
            Job(ServiceCmd.Manual);
        }
        //����
        private void ButtonDisableClick(object sender, EventArgs e) {
            Job(ServiceCmd.Disable);
        }

        void Job(ServiceCmd serviceCmd) {

            Enabled = false;
            Cursor.Current = Cursors.WaitCursor;

            _setupService.Job(serviceCmd);
            DispInit();

            Enabled = true;
            Cursor.Current = Cursors.Default;
        }

        private void ButtonOkClick(object sender, EventArgs e) {
            Close();
        }


    }
}