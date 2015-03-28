
namespace WebServer {
    partial class Server {
        protected override void CheckLang(){
        }

        public override string GetMsg(int messageNo) {
            switch (messageNo) {
                case 0:  return Kernel.IsJp()?"���N�G�X�g�̉��߂Ɏ��s���܂����i�s���ȃ��N�G�X�g�̉\�������邽�ߐؒf���܂���)":"Failed in interpretation of a request (I cut it off so that there was possibility of an unjust request in it)";
                case 1:  return Kernel.IsJp()?"�T�|�[�g�O�̃��\�b�h�ł��i�������p���ł��܂���j":"It is a method out of a support (Cannot continue processing)";
                case 2:  return Kernel.IsJp()?"�T�|�[�g�O�̃o�[�W�����ł��i�������p���ł��܂���j":"It is a version out of a support (Cannot continue processing)";
                case 3:  return "request";//�ڍ׃��O�p
                case 4:  return "response";//�ڍ׃��O�p
                case 5: return Kernel.IsJp() ? "URI�̉��߂Ɏ��s���܂����i�s���ȃ��N�G�X�g�̉\�������邽�ߐؒf���܂���)" : "failed in interpretation of URI (I cut it off so that there was possibility of an unjust request in it)";
                case 6:  return Kernel.IsJp()?"�F�؃G���[�i�F�؃��X�g�ɒ�`����Ă��Ȃ����[�U����̃A�N�Z�X�ł��j":"A certification error (it is access from the user who is not defined by a certification list)";
                case 7:  return Kernel.IsJp()?"�F�؃G���[�i���[�U���X�g�ɓ��Y���[�U�̏�񂪂���܂���j":"A certification error (a user list does not include information of the user concerned)";
                case 8:  return Kernel.IsJp()?"�F�ؐ���":"Certification success";
                case 9:  return Kernel.IsJp()?"�F�؃G���[�i�p�X���[�h���Ⴂ�܂��j":"";
                case 10: return Kernel.IsJp()?"���̃A�h���X����̃��N�G�X�g�͋�����Ă��܂���":"";
                case 11: return Kernel.IsJp()?"���̗��p�҂̃A�N�Z�X�͋�����Ă��܂���":"A certification error (a password is different)";
                //case 12: return "";
                case 13: return Kernel.IsJp()?".. ���܂܂�郊�N�G�X�g�͋�����Ă��܂���":"The request that .. is included in is not admitted";
                case 14: return Kernel.IsJp()?"�h�L�������g���[�g�Ŏw�肳�ꂽ�t�H���_�����݂��܂���i�������p���ł��܂���j":"There is not a folder appointed by a DocumentRoot (Cannot continue processing)";
                case 15: return Kernel.IsJp()?"SSI #include �������g���C���N���[�h���邱�Ƃ͂ł��܂���":"SSI #include Cannot do include of oneself";
                case 16: return Kernel.IsJp()?"CGI ���s�G���[":"CGI execution error";
                case 17: return "exec SSI";
                case 18: return "execute";
                case 20: return Kernel.IsJp()?"�p�����[�^�̉��߂Ɏ��s���܂���":"Failed in interpretation of a parameter";
                case 21: return Kernel.IsJp() ? "SSI�̏����Ɏ��s���܂���" : "Failed in processing of small scale integration";
                case 22: return Kernel.IsJp()?"SSI ���s�G���[":"SSI execution error";
                case 23: return Kernel.IsJp()?"���N�G�X�g[HTTPS]":"Request[HTTPS]";//�m�[�}�����O�p
                case 24: return Kernel.IsJp()?"���N�G�X�g":"Request";//�m�[�}�����O�p
                case 25: return Kernel.IsJp()?"���^(�G���[�h�L�������g)�����͂���Ă��܂���":"A model is not appointed(Error Document)";
                case 26: return Kernel.IsJp()?"���^(�C���f�b�N�X�h�L�������g)�����͂���Ă��܂���":"A model is not appointed(Index Document)";
                case 27: return Kernel.IsJp()?"CGI�ȊO�̃t�@�C�����w�肳��Ă��܂�" : "An appointed file is not CGI";
                case 28: return Kernel.IsJp() ? "#exec �́Acmd�y��cgi�����w��ł��܂���" : "\"#exec\" can appoint only \"cgi\" and \"cmd\"";
                case 29: return Kernel.IsJp() ? "�f�B���N�g�����폜�ł��܂���ł���" : "Failed in elimination of a directory";
                case 30: return Kernel.IsJp() ? "�t�@�C�����폜�ł��܂���ł���" : "Failed in elimination of a file";
                case 31: return Kernel.IsJp() ? "�t�@�C���̍쐬�Ɏ��s���܂���" : "Failed in making of a file";
                case 32: return Kernel.IsJp() ? "�f�B���N�g�����폜�ł��܂���ł���" : "Failed in elimination of a directory";
                case 33: return Kernel.IsJp() ? "�t�@�C�����폜�ł��܂���ł���" : "Failed in elimination of a file";
                case 34: return Kernel.IsJp() ? "�f�B���N�g���̈ړ�(�R�s�[)�Ɏ��s���܂���" : "Failed in movement(copy) of a directory";
                case 35: return Kernel.IsJp() ? "�t�@�C���̈ړ�(�R�s�[)�Ɏ��s���܂���" : "Failed in movement(copy) of a file";
                case 36: return Kernel.IsJp() ? "�f�B���N�g���̍쐬�Ɏ��s���܂���" : "Failed in making of a directory";
                case 37: return Kernel.IsJp() ? "URL�̉��߂Ɏ��s���܂���" : "Failed in interpretation of URL";
                case 38: return "POST data recved";
                case 39: return "POST data recved";
                case 40: return "faild POST data recve.";
                case 41: return "faild POST data recve.";
            }
            return "unknown";
        }

    }
}
