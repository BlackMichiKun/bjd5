
namespace SmtpServer {
    public partial class Server {
        protected override void CheckLang(){
        }
        public override string GetMsg(int messageNo)
        {
            switch (messageNo) {
                case 0: return "MESSAE";
                case 1: return Kernel.IsJp()?"�ڑ����܂���":"Connected";
                case 2: return Kernel.IsJp()?"�f�B���N�g���w��i���[���{�b�N�X�j�ɖ�肪����܂��B�i�T�[�o�͋@�\���܂���j":"Directory appointment (a mailbox) includes a problem(Server start failure)";
                case 3: return Kernel.IsJp()?"�h���C�������w�肳��Ă��܂���i�T�[�o�͋@�\���܂���j":"A domain name is not appointed(Server start failure)";
                case 4:return Kernel.IsJp()?"���[���{�b�N�X�̏������Ɏ��s���܂���(�T�[�o�͋N���ł��܂���)":" failed in initialization of a mailbox(Server start failure)";
                case 5: return Kernel.IsJp()?"POP bfore SMTP�ɂ�郊���[���p�������܂���":"admitted relay broadcast by POP before SMTP";
                case 6: return Kernel.IsJp()?"���[�U�����݂��܂���":"There is not a user";
                case 7: return Kernel.IsJp()?"��M�T�C�Y�̐������z���܂���":"Exceeded a limit of reception size";
                case 8: return Kernel.IsJp()?"���[���{�b�N�X�֊i�[���܂���":"Housed it to a mailbox";
                case 9: return Kernel.IsJp()?"���[���L���[�֊i�[���܂���":"Housed it to a mailqueue";
                case 10: return Kernel.IsJp()?"�L���[����(�J�n)":"Queue processing (start)";
                case 11: return Kernel.IsJp()?"�L���[����(����)":"Queue processing (success)";
                case 12: return Kernel.IsJp()?"�L���[����(���s) �T�[�o�i�A�h���X�j�������s":"Queue processing (Server search failure)";
                case 13: return Kernel.IsJp()?"�L���[����(���s) �����s��":"Queue processing (faild) A cause is unknown";
                case 14: return Kernel.IsJp()?"�L���[����(���s) �G���[�R�[�h����M���܂���":"Queue processing (faild) Received an error cord";
                case 15: return Kernel.IsJp()?"�G���[���[�����쐬���܂���":"Made an error email";
                case 16: return Kernel.IsJp()?"�w�b�_��u�������܂���":"Moved a header";
                case 17: return Kernel.IsJp()?"�w�b�_��ǉ����܂���":"Added a header";
                //case 18: return Kernel.IsJp()?"UUCP�A�h���X�ɂ͑Ή����Ă��܂���":"Not equivalent to an UUCP address";
                case 19: return Kernel.IsJp()?"�G���A�X�w�肪�����ł��i���[�U�����݂��܂���j":"Elias appointment is invalidity (there is not a user)";
                case 20: return Kernel.IsJp()?"�u��{�I�v�V�����v�|�u�T�[�o���v���w�肳��Ă��܂���(�T�[�o�ɂ���ẮA���M�Ɏ��s����\��������܂�)":"Option [Basic Option]-[Saerver Name]  is not appointed(With a server, I may fail in the transmission of a message)";
                case 21: return Kernel.IsJp()?"�t�@�C���Ƀ��[�����ǉ�����܂���" : "An email was added to a file";
                case 22: return Kernel.IsJp()?"�t�@�C���ւ̃��[���ǉ��Ɏ��s���܂���" : "I failed in email addition to a file";
                case 23: return Kernel.IsJp()?"������M" : "The automatic reception";
                //case 24: return Kernel.IsJp()?"�T�[�o�ւ̐ڑ��Ɏ��s���܂���(������M)" : "Connection failure to a server(The automatic reception)";
                case 25: return Kernel.IsJp()?"���p���̎w��ɖ�肪����܂�" : "Relay configuration failure";
                case 26: return Kernel.IsJp()?"���̐ڑ��ɂ͊g��SMTP���K�p����܂���" : "ESMP is not applied in this connection";
                case 27: return Kernel.IsJp() ? "�G���A�X�ϊ�" : "Alias";
                case 28: return Kernel.IsJp() ? "���[���A�h���X�����[�J���h���C���ł͂���܂���iFrom�U���������Ȃ��j" : "There is not an email address in a local domain (From: Check)";
                case 29: return Kernel.IsJp() ? "���[���A�h���X�����[�J�����[�U�ł͂���܂���iFrom�U���������Ȃ��j" : "There is not an email address in a local user (From: Check)";
                case 30: return Kernel.IsJp() ? "�P���[�U�̃G���A�X�𕡐��s�Ŏw�肷�邱�Ƃ͂ł��܂���B�ʖ��̓J���}�ŋ�؂��ĕ����w��ł��܂��B" : "Can't appoint plural Elias of a 1 user in a line, and another name divides it in a comma and can appoint a plural number.";
                case 31: return Kernel.IsJp() ? "�u�Ǘ��̈�i�t�H���_�j�v�̎w�肪�����ł��BML�͋@�\�ł��܂���" : "Appointment of \"management directory\" is null and void(A ML cannot function)";
                case 32: return Kernel.IsJp() ? "(ML)���[���̕ۑ��Ɏ��s���܂���" : "(ML)Failed in a save of an email";
                case 33: return Kernel.IsJp() ? "���e���󂯕t���܂���" : "Accepted a contribution";
                case 34: return Kernel.IsJp() ? "�����o�[�ȊO����̓��e�ł�" : "It is a contribution from not member";
               // case 35: return kernel.IsJp() ? "�����o�[�ւ̔z�M�ɐ������܂���" : "Success delivery it";
               // case 36: return kernel.IsJp() ? "�����o�[�ւ̔z�M�Ɏ��s���܂���" : "Error delivery it";
                case 37: return Kernel.IsJp() ? "������Ȃ����[�U����̐��䃁�[���ł�" : "It is the control demand that is not admitted";
                case 38: return Kernel.IsJp() ? "(ML)�z�M�ɐ������܂���" : "(ML)Success delivery it";
                case 39: return Kernel.IsJp() ? "(ML)�z�M�Ɏ��s���܂���" : "(ML)Error delivery it";
                case 40: return Kernel.IsJp() ? "����R�}���h�̉��߂Ɏ��s���܂���" : "Failed in interpretation of a control command";
                case 41: return Kernel.IsJp() ? "����R�}���h�����s���܂�" : "Execute a control command";
                case 42: return Kernel.IsJp() ? "Guide�𑗐M���܂���" : "Transmitted a guide";
                case 43: return Kernel.IsJp() ? "���e��������Ă��Ȃ������o�[����̓��e�ł�" : "It is POST from the member that it is not admitted a contribution";
                case 44: return Kernel.IsJp() ? "(ML)���[�����O���X�g���L���ɂȂ�܂���" : "(ML)A mailing list became effective";
                case 45: return Kernel.IsJp() ? "�G���A�X�w�肪�����ł�" : "Elias appointment is invalidity";
                case 46: return Kernel.IsJp() ? "�����o�[��ǉ����܂���" : "Added a member";
                case 47: return Kernel.IsJp() ? "���̃R�}���h�𔭍s���錠��������܂���" : "There is not authority to execute this command";
                case 48: return Kernel.IsJp() ? "�����o�[�̒ǉ��Ɏ��s���܂���" : "Failed in addition of a member";
                case 49: return Kernel.IsJp() ? "�R�}���h���s�ŃG���[���������܂���" : "An error occurred by a command";
                case 50: return Kernel.IsJp() ? "�Ǘ��҃��O�C�����K�v�ł�" : "Manager login is necessary";
                case 51: return Kernel.IsJp() ? "�p�����[�^�ɖ�肪����܂�" : "A parameter includes a problem";
                case 52: return Kernel.IsJp() ? "���[���A�h���X�����[�J�����[�U�ł͂���܂���iFrom�U���������Ȃ��j" : "There is not an email address in a local user (From: Check)";
                case 53: return Kernel.IsJp() ? "(ML)�����o�i�Ǘ��ҁj�̃��[���A�h���X�ɖ�肪����܂�":"(ML)There is a problem to an email address of a member (a manager)";
                case 54: return Kernel.IsJp() ? "�����R�}���h������񐔂𒴂��܂����B�����������ؒf���܂��B" : "Unknown command exceeded established frequency";//Ver5.4.7
                case 55: return Kernel.IsJp() ? "���[���{�b�N�X�ւ̊i�[�Ɏ��s���܂���" : "Failed in housing to a mailbox";
                case 56: return Kernel.IsJp() ? "�T�[�o�Ƃ̐ڑ��Ɏ��s���܂���" : "Failed in connection with a server";
                case 57: return Kernel.IsJp() ? "(ML)�����o�����݂��܂���" : "(ML)There is not a member��";
            }
            return "unknown";
        }

    }
}
