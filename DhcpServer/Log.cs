
using Bjd;
using Bjd.option;

namespace DhcpServer {
    partial class Server {
        private readonly Dat _macAcl;

        public override string GetMsg(int messageNo) {
            switch (messageNo) {
                case 1:return IsJp ? "MAC�A�h���X�ɂ�鐧���@���p�҂ɓo�^����Ă��Ȃ�MAC�A�h���X����̗v����j�����܂�":"Access deny by a MAC address";
                case 3:return IsJp ?"���N�G�X�g ->":"request ->";
                case 4:return IsJp ?"<- ���X�|���X":"<- response";
                case 5:return IsJp ?"���[�X���܂���":"complete a Lease";
                case 6:return IsJp ?"�J�����܂���":"complete a Release";
            }
            return "unknown";
        }

    }
}
