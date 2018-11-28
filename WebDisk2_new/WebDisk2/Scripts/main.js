//初始化
var nd = new TreeNode();
nd.container = $("tree");
nd.text = "在线文件管理";
nd.show();
currentNode = nd;
currentNode.Refersh();

//加载对话框
var dialog = new Dialog();
dialog.ImgZIndex = 110;
dialog.DialogZIndex = 111;

//下载操作
clickFile = function (fName) {
    window.onbeforeunload = function () { }
    window.location = defaultURL + "?action=DOWNLOAD&value1=" + encodeURIComponent(currentNode.path + fName);
    window.onbeforeunload = function () {
        return "无法保存当前状态，请退出！"
    };
}

//保存文件
function saveFile(action, fileName) {
    getEditorContent();
    var url = defaultURL + "?action=" + action + "&value1=" + encodeURIComponent(currentNode.path + fileName);
    return executeHttpRequest("POST", url, "content=" + encodeURIComponent(currentContent));
}

//返回上级目录
function gotoParentDirectory() {
    if (currentNode != null) {
        currentNode.GotoParentNode();
    }
}

//根目录
function goRoot() {
    currentNode = nd;
    currentNode.Refersh();
}

//刷新
function refersh() {
    if (currentNode != null) {
        currentNode.Refersh();
    }
}

//全选
function selectAll() {
    var checkBoxes = $("fileList").getElementsByTagName("input");

    for (var i = 0; i < checkBoxes.length; i++) {
        if (checkBoxes[i].type == "checkbox") {
            checkBoxes[i].checked = $("checkAll").checked;
        }
    }
}


var iframeOnload = function () { }
//上传文件
function uploadFile() {
    iframeOnload = function () { }
    dialog.Content = "<iframe id='uploadFrm' frameborder='no' border='0' scrolling='no' allowtransparency='yes' onload='iframeOnload()' name='uploadFrm' style='width:0px; height:0px; display:none;'></iframe><form name='actionForm' id='actionForm' action='" + defaultURL + "?action=UPLOAD&value1=" + currentNode.path + "' method='POST' target='uploadFrm' enctype='multipart/form-data'><input name='selectFile' width='150' type='file' /></form><div id='uploadStatus' style='display:none;'><img src='images/process.gif' /><div style='color:#ccc;'>正在上传，如果长时间不响应，可能是上传文件太大导致出错！</div></div>";
    dialog.Text = "上传文件";
    dialog.Show();

    dialog.OK = function () {
        iframeOnload = function () {
            dialog.Text = "提示";
            dialog.Content = "文件上传成功！";
            dialog.OK = dialog.Close;
            dialog.Show(1);
            currentNode.Refersh();
        }

        $("actionForm").submit();
        $("actionForm").style.display = "none";
        $("uploadStatus").style.display = "";
    }
}