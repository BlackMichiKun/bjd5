
namespace ProxyHttpServer {
    partial class Server {
        protected override void CheckLang()
        {
        }

        public override string GetMsg(int messageNo) {
            switch (messageNo) {
                case 0:  return Kernel.IsJp()?"���N�G�X�g":"Request";
                case 1:  return Kernel.IsJp()?"�T�|�[�g�O�̃��\�b�h�ł��i�������p���ł��܂���j":"It is a method out of a support (Cannot continue processing)";
                case 2:  return Kernel.IsJp()?"�T�|�[�g�O�̃o�[�W�����ł��i�������p���ł��܂���j":"It is a version out of a support(Cannot continue processing)";
                case 3:  return Kernel.IsJp()?"�^�C���A�E�g":"Timeout";
                case 4:  return Kernel.IsJp()?"�L���b�V���i�������j�֕ۑ����܂���":"Saved it to cash (MEMORY)";
                case 5:  return Kernel.IsJp()?"�L���b�V���i�f�B�X�N�j�֕ۑ����܂���":"Saved it to cash (DISK)";
                case 6:  return Kernel.IsJp()?"���X�|���X��M�Ɏ��s���܂���":"Failed in the reception of a response";
                case 7:  return Kernel.IsJp()?"�w�b�_��M�Ɏ��s���܂���":"Failed in the reception of a header";
                case 8:  return "BREAK";
                case 9:  return Kernel.IsJp()?"���M�Ɏ��s���܂���":"Transmission of a message failure";
                case 10: return Kernel.IsJp()?"URL�����Ƀq�b�g���܂���":"An URL limit";
                case 11: return Kernel.IsJp()?"���O�����Ɏ��s���܂����B�T�[�o�֐ڑ��ł��܂���":"Name solution failure(Cannot be connected to a server";
                case 12:  return Kernel.IsJp()?"�L���b�V���Ώۂł͂���܂���i�z�X�g�j":"It is not a cash object (Host)";
                case 13:  return Kernel.IsJp()?"�L���b�V���Ώۂł͂���܂���i�g���q�j":"It is not a cash object (Extension)";
                case 14:  return Kernel.IsJp()?"�L���b�V���Ƀq�b�g���܂���":"Hit in cache";
                case 15:  return Kernel.IsJp()?"�u�L���b�V���ۑ��f�B���N�g���v�̎w�肪�����ł��B�f�B�X�N�L���b�V���͋@�\�ł��܂���":"Appointment of \"save directory\" is null and void(A disk cache cannot function)";
                case 16:  return Kernel.IsJp()?"�uno cache�v�̂��߃L���b�V�����܂���":"Don't do cash for \"no cache\"";
                case 17:  return Kernel.IsJp()?"�L���b�V���ɕۑ����܂���":"saved it in cache";
                case 18:  return Kernel.IsJp()?"�L���b�V���ւ̕ۑ����L�����Z�����܂���":"I did not save it in cache";
                case 19:  return Kernel.IsJp()?"�L���b�V���i�������j�ֈړ����܂���":"I moved to cache (memory)";
                case 20:  return Kernel.IsJp()?"�ő�T�C�Y�𒴂��Ă���̂ŃL���b�V���ւ̕ۑ����L�����Z�����܂�":"Because I exceed maximum size, I cancel a save to cache";
                case 21:  return Kernel.IsJp()?"�R���e���c�����Ƀq�b�g���܂����i�ڑ��͒��f����܂����j":"I made a hit in contents limit (the connection was stopped)";
                case 22:  return Kernel.IsJp()?"POST���N�G�X�g�̏����Ɏ��s���܂���" : "Failed in processing of a POST request";
                case 23:  return Kernel.IsJp()?"�f�B�X�N�L���b�V���̍œK�����J�n���܂�":"Start optimization of a disk cache";
                case 24:  return Kernel.IsJp()?"�f�B�X�N�L���b�V���̍œK�����I�����܂�":"Stop optimization of a disk cache";
                case 25:  return Kernel.IsJp()?"�Â��L���b�V�����폜���܂�":"Remove old cash";
                case 26:  return Kernel.IsJp()?"�ڑ��Ɏ��s���܂���":"Failed in connection";
                case 27: return Kernel.IsJp() ? "�f�B�X�N�L���b�V���̍œK���ŃG���[���������܂���" : "An error occurred by optimization of disk cache";
                case 28: return Kernel.IsJp() ? "URL�����ŉ��߂ł��Ȃ����K�\�����ݒ肳��܂���" : "The regular expression that I cannot interpret by an URL limit was set";
                case 29: return Kernel.IsJp() ? "�݌v�G���[�@Request.Recv()" : "Request.Recv()";
                //case 27:  return kernel.IsJp() ? "HTTP/1.1 �͎g�p�ł��܂���" : "Cannot use HTTP /1.1";
            }
            return "unknown";
        }
    }
   
}
