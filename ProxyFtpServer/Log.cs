
namespace ProxyFtpServer {
    partial class Server {
        public override string GetMsg(int messageNo) {
            switch (messageNo) {
                case 1: return Kernel.IsJp()?"�F�؂Ɏ��s���܂���":"Failed in the certification";
                case 2: return Kernel.IsJp()?"�A�C�h���^�C���A�E�g":"An idle time out";
                case 3: return Kernel.IsJp()?"���M�Ɏ��s���܂���":"Transmission of a message failure";
                case 4: return Kernel.IsJp()?"PORT�R�}���h�̉��߂Ɏ��s���܂���":"Interpretation failure of a command(PORT)";
                case 5: return Kernel.IsJp()?"PASV�R�}���h�̉��߂Ɏ��s���܂���(1)":"Interpretation failure of a command(PASV)[1]";
                case 6: return Kernel.IsJp()?"PASV�R�}���h�̉��߂Ɏ��s���܂���(2)":"Interpretation failure of a command(PASV)[2]";
                case 7: return Kernel.IsJp()?"�R�}���h��M":"The command reception";
                case 8: return Kernel.IsJp() ? "�uUSER ���[�U��@�z�X�g���v�̐ڑ��`�������Ή��ł��܂���B" : "Only a connection form of [USER username@hostname] can support.";

            }
            return "unknown";
        }

    }
}
