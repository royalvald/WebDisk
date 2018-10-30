<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>网络文件夹</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:DropDownList ID="DropDownList1" runat="server" Width="200px" AutoPostBack="True" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
            <asp:ListItem Value="-1">选择功能</asp:ListItem>
            <asp:ListItem Value="0">上传限制</asp:ListItem>
            <asp:ListItem Value="1">上传文件</asp:ListItem>
            <asp:ListItem Value="2">管理文件</asp:ListItem>
        </asp:DropDownList>
        <asp:Label ID="lbl_FolderInfo" runat="server"></asp:Label><br />
        <asp:MultiView ID="MultiView1" runat="server">
            <!--上传限制界面开始-->
            <asp:View ID="view_Configure" runat="server">
                允许上传文件的类型：
                <asp:BulletedList ID="bl_TileTypeLimit" runat="server">
                </asp:BulletedList>
                允许上传单个文件的大小：
                <asp:Label ID="lab_FileSizeLimit" runat="server" Text=""></asp:Label>
             </asp:View>
             <asp:View ID="view_Upload" runat="server">
                 <asp:FileUpload ID="FileUpload" runat="server" Width="400"/><br />
                 <asp:Button ID="btn_Upload" runat="server" Text="上传文件" OnClick="btn_Upload_Click" />
             </asp:View>
             <!--管理文件开始-->
             <asp:View ID="view_Manage" runat="server">
                <table cellpadding="5" cellspacing="0" border="0">
                    <tr>
                        <td>
                            <!--启用了AutoPostBack-->
                            <asp:ListBox ID="lb_FileList" runat="server" AutoPostBack="True" Height="300px" Width="300px" OnSelectedIndexChanged="lb_FileList_SelectedIndexChanged"></asp:ListBox></td>
                        <td valign="top">
                            <asp:Label ID="lbl_FileDescription" runat="server"></asp:Label></td>
                    </tr>
                </table>
                <asp:Button ID="btn_DownLoad" runat="server" Text="下载文件" OnClick="btn_DownLoad_Click" />
                <!--在删除前给予确定-->
                <asp:Button ID="btn_Delete" runat="server" Text="删除文件" OnClientClick="return confirm('确定删除文件!')" OnClick="btn_Delete_Click" /><br />
                <asp:TextBox ID="tb_FileNewName" runat="server" Width="300px"></asp:TextBox>
                <asp:Button ID="btn_Rename" runat="server" Text="对文件重命名" OnClick="btn_Rename_Click" />
            </asp:View>
        </asp:MultiView>
    </div>
    </form>
    
</body>
</html>
