using System;

namespace Bjd.tool {
    public class Tool : OneTool {
        public Tool(Kernel kernel, string nameTag)
            : base(kernel, nameTag) {
        }
        public override string JpMenu { get { return "�X�e�[�^�X�\��"; } }
        public override string EnMenu { get { return "Status"; } }
        public override char Mnemonic { get { return 'U'; } }

        override public ToolDlg CreateDlg(Object obj) {
            return new Dlg(Kernel, NameTag, obj, (Kernel.IsJp()) ? "�X�e�[�^�X�\��" : "Status");
        }
    }
}

