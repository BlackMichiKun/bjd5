var fso = new ActiveXObject("Scripting.FileSystemObject");

//�o�[�W����
var version = "6.0.1";
//�\�[�X�R�[�h�̃t�H���_
//var srcDir = fso.GetFolder("C:\\tmp2\\bjd5");
var currentDir = WScript.CreateObject("WScript.Shell").CurrentDirectory;
var srcDir = fso.GetFolder(currentDir);


//var files = srcDir.Files;//�K�w���̃t�@�C���̈ꗗ
var dirs = srcDir.SubFolders;//�K�w���̃t�H���_�̈ꗗ
var ar = new Enumerator(dirs);
for (; !ar.atEnd(); ar.moveNext()){
	var file =  ar.item()+"\\Properties\\AssemblyInfo.cs";
	if(fso.FileExists(file)==1){//���݊m�F
		EditFile(file);//�ҏW
	}
}
WScript.Echo("�o�[�W������"+version+"�ɏC�����܂���")

//�t�@�C�����I�[�v�����āu�v�̍s��ҏW����
function EditFile(path){

	var charset="utf-8";
	var str = adoLoadText(path,charset);

	var tmp = "";
	var ar = new Enumerator(str.split("\r\n"));//�s�P�ʂŏ�������
	for (; !ar.atEnd(); ar.moveNext()){
		//�Y���s�͕ҏW���ĂP�s�ۑ�
		if(ar.item().indexOf("[assembly: AssemblyVersion(")==0){
			tmp = tmp + "[assembly: AssemblyVersion(\"" + version + "\")]"+"\r\n"
		}else if(ar.item().indexOf("[assembly: AssemblyFileVersion(")==0){
			tmp = tmp + "[assembly: AssemblyFileVersion(\"" + version + "\")]"+"\r\n"
		}else{//���̑��͂��̂܂܂P�s�ۑ�
			tmp = tmp + ar.item() +"\r\n";
		}
	}
	adoSaveText(path,tmp, charset);
}


function adoLoadText(filename, charset) {
	var stream, text;
	stream = new ActiveXObject("ADODB.Stream");
	stream.type = 2; //2:TypeText
	stream.charset = charset;
	stream.open();
	stream.loadFromFile(filename);
	text = stream.readText(-1);//-1:ReadAll
	stream.close();
	return text;
}

function adoLoadLinesOfText(filename, charset) {
  var stream;
  var lines = new Array();
  stream = new ActiveXObject("ADODB.Stream");
  stream.type = adTypeText;
  stream.charset = charset;
  stream.open();
  stream.loadFromFile(filename);
  while (!stream.EOS) {
    lines.push(stream.readText(adReadLine));
  }
  stream.close();
  return lines;
}

function adoSaveText(filename, text, charset) {
  var stream;
  stream = new ActiveXObject("ADODB.Stream");
  stream.type = 2;//2:TypeText;
  stream.charset = charset;
  stream.open();
  stream.writeText(text);
  stream.saveToFile(filename, 2);//2:adSaveCreateOverWrite
  stream.close();
}


