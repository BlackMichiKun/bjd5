
namespace FtpServer {
    partial class Server {
        public override string GetMsg(int messageNo) {
            switch (messageNo) {
                case 1: return Kernel.Jp?"�p�����[�^���������܂��i�s���ȃ��N�G�X�g�̉\�������邽�ߐؒf���܂���)":"A parameter is too long (I cut it off so that there was possibility of an unjust request in it)";
                case 2: return Kernel.Jp?"�z�[���f�B���N�g�������݂��܂���i�������p���ł��Ȃ����ߐؒf���܂���)":"There is not a home directory (because I cannot continue processing, I cut it off)";
                case 3: return Kernel.Jp?"�R�}���h�����ŃG���[���������܂���":"An error occurred by command processing";
                case 5: return "login";
                case 6: return "login";
                case 7: return "success";
                case 8: return "RENAME";
                case 9: return "UP start";
                case 10: return "UP end";
                case 11: return "DOWN start";
                case 12: return "DOWN end";
                case 13: return "logout";
                case 14: return Kernel.Jp?"���[�U���������ł�":"A user name is null and void";
                case 15: return Kernel.Jp?"�p�X���[�h���Ⴂ�܂�":"password is different";
            }
            return "unknown";
        }
    }
}
