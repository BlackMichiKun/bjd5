
namespace DnsServer {
    partial class Server {
        public override string GetMsg(int messageNo) {
            switch (messageNo) {
                case 0: return (Kernel.Jp) ? "�W���⍇(OPCODE=0)�ȊO�̃��N�G�X�g�ɂ͑Ή��ł��܂���" : "Because I am different from 0 in OPCODE,can't process it.";
                case 1: return (Kernel.Jp) ? "����G���g���[���P�łȂ��p�P�b�g�͏����ł��܂���":"Because I am different from 1 a question entry,can't process it.";
                case 2: return (Kernel.Jp) ? "�p�P�b�g�̃T�C�Y�ɖ�肪���邽�߁A�������p���ł��܂���":"So that size includes a problem,can't process it.";
                case 3: return (Kernel.Jp) ? "�p�P�b�g�̃T�C�Y�ɖ�肪���邽�߁A�������p���ł��܂���":"So that size includes a problem,can't process it.";
                case 4: return (Kernel.Jp) ? "�p�P�b�g�̃T�C�Y�ɖ�肪���邽�߁A�������p���ł��܂���":"So that size includes a problem,can't process it.";
                case 5: return (Kernel.Jp) ? "Lookup() �p�P�b�g��M�Ń^�C���A�E�g���������܂����B":"Timeout occurred in Lookup()";
                case 6: return (Kernel.Jp) ? "���[�g�L���b�V����ǂݍ��݂܂���":"root cache database initialised.";
                case 7: return "zone database initialised.";
                case 8: return "Query";
                case 9: return "request to a domain under auto (localhost)";
                case 10: return "request to a domain under management";
                case 11: return "request to a domain under auto (localhost)";
                case 12: return "request to a domain under management";
                case 13: return "Search LocalCache";
                case 14: return  "Answer";
                case 15: return "Search LocalCache";
                case 16: return "Answer CNAME";
                case 17: return "Lookup";
                case 18: return "Lookup";
                case 19: return (Kernel.Jp) ? "A(PTR)���R�[�h��IPv6�A�h���X���w��ł��܂���" : "IPv6 cannot address it in an A(PTR) record";
                case 20: return (Kernel.Jp) ? "AAAA���R�[�h��IPv4�A�h���X���w��ł��܂���" : "IPv4 cannot address it in an AAAA record";
                case 21:  return (Kernel.Jp) ? "���[�g�L���b�V����������܂���" : "Root chace is not found";

            }
            return "unknown";
        }
    }
}
