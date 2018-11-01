<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebSelf1.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server" aria-orientation="vertical">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>在线网盘设计</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:DropDownList ID="selectList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="selectList_SelectedIndexChanged" Height="50" Width="500" Font-Size="Large">
                <asp:ListItem Value="0" Selected="True">功能选择</asp:ListItem>
                <asp:ListItem Value="1" >网盘信息</asp:ListItem>
                <asp:ListItem Value="2">上传文件</asp:ListItem>
                <asp:ListItem Value="3">文件列表</asp:ListItem>
            </asp:DropDownList>
        </div>
        <br />
        <div>
            <asp:MultiView ID="multiView1" runat="server">
                <asp:View ID="view_configure" runat="server">
                    <!--上传文件格式:
                    <asp:BulletedList ID="fileType_list" runat="server">
                    </asp:BulletedList>
                    <br />-->
                    单个文件上传最大为:
                    <asp:Label ID="fileMaxsize" runat="server"></asp:Label>
                    网盘使用信息:
                    <asp:Label ID="disk_useInfo" runat="server"></asp:Label>
                </asp:View>

                <asp:View ID="view_Upload" runat="server" >
                    <asp:FileUpload ID="fileUpload" runat="server" Height="100" Width="200" />
                    <asp:Button ID="Upload_button" runat="server" Text="开始上传" OnClick="Upload_button_Click" />
                </asp:View>

                <asp:View ID="disk_manage" runat="server">
                    <table cellpadding="5" cellspacing="0" border="0">
                        <tr>
                            <td>
                                <asp:ListBox ID="fileList" runat="server" AutoPostBack="true" Height="300" Width="500" OnSelectedIndexChanged="fileList_SelectedIndexChanged"></asp:ListBox></td>
                            <td valign="top">
                                <asp:Label ID="fileInfo" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <asp:Button ID="fileDownload" runat="server" Text="文件下载" OnClick="fileDownload_Click" />
                    <asp:Button ID="fileDelete" runat="server" Text="文件删除" OnClientClick="return confirm('确定删除文件')" OnClick="fileDelete_Click" />
                    <asp:TextBox ID="file_newName" runat="server" Width="300px"></asp:TextBox>
                    <asp:Button ID="file_rename" Text="重命名" runat="server" OnClick="file_rename_Click" />
                 </asp:View>
            </asp:MultiView>
        </div>

        <div >
            <asp:Label ID="navigation"
        </div>
    </form>
</body>
</html>
