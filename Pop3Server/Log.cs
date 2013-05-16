
namespace Pop3Server {
    partial class Server {
        public override string GetMsg(int messageNo) {
            switch (messageNo) {
                case 1:return IsJp ? "�Q�d���O�C���̗v�����������܂���":"A demand of double login occurred";
                case 2:return IsJp ? "���O�C�����܂���":"Login";
                case 3:return IsJp ? "�F�؂Ɏ��s���܂���":"Certification failure";
                case 4:return IsJp ? "���[���{�b�N�X�̏������Ɏ��s���܂����B�T�[�o�͋N���ł��܂���":"Failed in initialization of a mailbox (A server cannot start)";
                case 5:return IsJp ? "���[������M���܂��� [RETR]" : "Received an email [RETR]";
            }
            return "unknown";
        }
    }
}
