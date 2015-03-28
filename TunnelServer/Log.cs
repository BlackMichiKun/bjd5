
namespace TunnelServer {
    partial class Server {
        protected override void CheckLang()
        {
        }

        public override string GetMsg(int messageNo) {
            switch (messageNo) {
                case 1:  return Kernel.IsJp()?"�ڑ���T�[�o���w�肳��Ă��܂���":"Connection ahead server is not appointed";
                case 2:  return Kernel.IsJp()?"�ڑ���|�[�g���w�肳��Ă��܂���":"Connection ahead port is not appointed";
                case 4:  return Kernel.IsJp()?"�T�[�o�ւ̐ڑ��Ɏ��s���܂���(1)":"Failed in connection to a server (1)";
                case 5:  return Kernel.IsJp()?"�T�[�o�ւ̐ڑ��Ɏ��s���܂���(2)":"Failed in connection to a server (2)";
                case 6:  return Kernel.IsJp()?"TCP�X�g���[�����g���l�����܂���":"A tunnel(TCP stream)";
                case 7:  return Kernel.IsJp()?"UDP�p�P�b�g���g���l�����܂���":"A tunnel(UDP packet)";
            }
            return "unknown";
        }

    }
}
