
namespace RemoteServer {
    partial class Server {
        protected override void CheckLang()
        {
        }

        public override string GetMsg(int messageNo) {
            switch (messageNo) {
                case 1: return (Kernel.IsJp())?"�����[�g�N���C�A���g����ڑ�����܂���":"Connected by a RemoteClient";
                case 2: return (Kernel.IsJp())?"�����[�g�N���C�A���g����̐ڑ�����������܂���":"Disconnected by a RemoteClient";
                case 3: return (Kernel.IsJp())?"�����[�g�T�[�o�X���b�h������������Ă��܂���":"A RemoteServerThread is not initialized";
                case 4: return (Kernel.IsJp())?"�p�X���[�h���Ⴂ�܂�":"Password incorrect";
                case 5: return (Kernel.IsJp())?"�F�؃p�X���[�h�͐ݒ肳��Ă��܂���":"No password";
            }
            return "unknown";
        }

    }
}
