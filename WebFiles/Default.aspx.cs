using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
//Email:jetaimefj@163.com
//该源码下载自www.51aspx.com(５１ａｓｐｘ．ｃｏｍ)
using System.IO;
public partial class _Default : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            //初始化文件夹信息
            InitFolderInfo();
            //初始化上传限制信息
            InitUploadLimit();
            //初始化列表框控件文件列表信息
            InitFileList();
        }
    }
    #region 初始化文件夹信息
    private void InitFolderInfo()
    {
        //从config中读取文件上传路径
        string strFileUpladPath = ConfigurationManager.AppSettings["FileUplodePath"].ToString();
        //如果上传文件夹不存在,则根据config创建一个
        if(!Directory.Exists(Server.MapPath(strFileUpladPath)))
        {
            Directory.CreateDirectory(Server.MapPath(strFileUpladPath));
        }
        //将虚拟路径转换为物理路径
        string strFilePath = Server.MapPath(strFileUpladPath);
        //从config里读取文件夹容量限制
        double iFolderSizeLimit = Convert.ToInt32(ConfigurationManager.AppSettings["FolderSizeLimit"]);
        //声明文件夹已经使用的容量
        double iFolderCurrentSize = 0;
        //获取文件夹中的所有文件
        FileInfo[] arrFiles = new DirectoryInfo(strFilePath).GetFiles();
        //循环文件获已经使用的容量
        foreach (FileInfo fi in arrFiles)
        {
            iFolderCurrentSize += Convert.ToInt32(fi.Length / 1024);
        }
        #region 第二种获得文件夹使用大小的方法
        //DirectoryInfo dir = new DirectoryInfo(strFilePath);
        //foreach (FileSystemInfo fi in dir.GetFileSystemInfos())
        //{
        //    FileInfo finf = new FileInfo(fi.FullName);
        //    iFolderCurrentSize += Convert.ToInt32(finf.Length / 1024);
        //}
        #endregion
        //把文件夹容量和以用文件夹容量赋值给标签
        lbl_FolderInfo.Text = string.Format("文件夹容量限制：{0}M，已用容量：{1}KB", iFolderSizeLimit / 1024, iFolderCurrentSize);
    }
    #endregion
    #region 初始化上传限制信息
    private void InitUploadLimit()
    {
        //从config中读取上传文件夹类型限制并根据逗号分割成字符串数组
        string[] arrFileTypeLimit = ConfigurationManager.AppSettings["FileTypeLimit"].ToString().Split(',');
        //从config中读取上传文件大小限制
        double iFileSizeLimit = Convert.ToInt32(ConfigurationManager.AppSettings["FileSizeLimit"]);
        //遍历字符串数组把所有项加入项目编号控件
        for (int i = 0; i < arrFileTypeLimit.Length; i++)
        {
            bl_TileTypeLimit.Items.Add(arrFileTypeLimit[i].ToString());
        }
        //把文件大小限制赋值给标签
        lab_FileSizeLimit.Text = string.Format("{0:f2}M", iFileSizeLimit / 1024);
    }
    #endregion
    #region 初始化列表框控件文件列表信息
    private void InitFileList()
    {
        //从config中获取文件上传路径
        string strFileUpladPath = ConfigurationManager.AppSettings["FileUplodePath"].ToString();
        //将虚拟路径转换为物理路径
        string strFilePath = Server.MapPath(strFileUpladPath);
        //读取上传文件夹下所有文件
        FileInfo[] arrFile = new DirectoryInfo(strFilePath).GetFiles();
        //把文件名逐一添加到列表框控件
        foreach(FileInfo fi in arrFile)
        {
            lb_FileList.Items.Add(fi.Name);
        }
    }
    #endregion
    #region 判断文件大小限制
    private bool IsAllowableFileSize()
    {
        //从web.config读取判断文件大小的限制
        double iFileSizeLimit = Convert.ToInt32(ConfigurationManager.AppSettings["FileSizeLimit"]) * 1024;
        //判断文件是否超出了限制
        if (iFileSizeLimit > FileUpload.PostedFile.ContentLength)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion
    #region 判断文件类型限制
    protected bool IsAllowableFileType()
    {
        //从web.config读取判断文件类型限制
        string strFileTypeLimit = ConfigurationManager.AppSettings["FileTypeLimit"].ToString();
        //当前文件扩展名是否包含在这个字符串中
        if(strFileTypeLimit.IndexOf(Path.GetExtension(FileUpload.FileName).ToLower()) >0)
            return true;
        else
            return false;
    }
    #endregion
    #region 弹出警告消息
    protected void ShowMessageBox(string strMessage)
    {
        Response.Write(string.Format("<script>alert('{0}')</script>",strMessage));
    }
    #endregion
    #region 上传文件按钮事件
    protected void btn_Upload_Click(object sender, EventArgs e)
    {
        //判断用户是否选择了文件
        if (FileUpload.HasFile)
        {
            //调用自定义方法判断文件类型否符合
            if (IsAllowableFileType())
            {
                //判断文件大小是否符合
                if (IsAllowableFileSize())
                {
                    //从web.config中读取上传路径
                    string strFileUploadPath = ConfigurationManager.AppSettings["FileUplodePath"].ToString();
                    //从UploadFile控件中读取文件名
                    string strFileName = FileUpload.FileName;
                    //组合成物理路径
                    string strFilePhysicalPath = Server.MapPath(strFileUploadPath + "/") + strFileName;
                    //判断文件是否存在
                    if(!File.Exists(strFilePhysicalPath))
                    {
                        //保存文件
                        FileUpload.SaveAs(strFilePhysicalPath);
                        //更新列表框
                        lb_FileList.Items.Add(strFileName);
                        //更新文件夹信息
                        InitFolderInfo();
                        ShowMessageBox("上传成功!");
                    }
                    else
                    {
                        ShowMessageBox("文件已经存在!");
                    }
                }
                else
                {
                    ShowMessageBox("文件大小不符合要求!");
                }
            }
            else
            {
                ShowMessageBox("类型不匹配");
            }
        }
    }
    #endregion
    #region 列表框事件
    protected void lb_FileList_SelectedIndexChanged(object sender, EventArgs e)
    {
        //从config中读取文件上传路径
        string strFileUpladPath = ConfigurationManager.AppSettings["FileUplodePath"].ToString();
        //从列表框中读取选择的文件名
        string strFileName = lb_FileList.SelectedValue;
        //组合成物理路径
        string strFilePhysicalPath = Server.MapPath(strFileUpladPath + "/") + strFileName;
        //根据物理路径实例化文件信息类
        FileInfo fi = new FileInfo(strFilePhysicalPath);
        //或得文件大小和创建日期赋值给标签
        lbl_FileDescription.Text = string.Format("文件大小：{0}字节<br><br>上传时间：{1}<br>", fi.Length, fi.CreationTime);
        //把文件名赋值给重命名文件框
        tb_FileNewName.Text = strFileName;
    }
    #endregion
    #region 下载文件按钮事件
    protected void btn_DownLoad_Click(object sender, EventArgs e)
    {
        //从web.config读取文件上传路径
        string strFileUploadPath = ConfigurationManager.AppSettings["FileUplodePath"].ToLower();
        //从列表框中读取选择的文件
        string strFileName = lb_FileList.SelectedValue;
        //组合成物理路径
        string FullFileName = Server.MapPath(strFileUploadPath + "/") + strFileName;

        FileInfo DownloadFile = new FileInfo(FullFileName);
        Response.Clear();
        Response.ClearHeaders();
        Response.Buffer = false;
        Response.ContentType = "application/octet-stream ";
        Response.AppendHeader("Content-Disposition ", "attachment;filename= " 
            + HttpUtility.UrlEncode(DownloadFile.FullName, System.Text.Encoding.UTF8));
        Response.AppendHeader("Content-Length ", DownloadFile.Length.ToString());
        Response.WriteFile(DownloadFile.FullName);
        Response.Flush();
        Response.End(); 
    }
    #endregion
    #region 删除文件
    protected void btn_Delete_Click(object sender, EventArgs e)
    {
        //从config中读取文件上传路径
        string strFileUpladPath = ConfigurationManager.AppSettings["FileUplodePath"].ToString();
        //从列表框中读取选择的文件名
        string strFileName = lb_FileList.SelectedValue;
        //组合成物理路径
        string strFilePhysicalPath = Server.MapPath(strFileUpladPath + "/") + strFileName;
        //删除文件
        System.IO.File.Delete(strFilePhysicalPath);
        //更新文件列表框控件
        lb_FileList.Items.Remove(lb_FileList.Items.FindByText(strFileName));
        //更新文件夹信息
        InitFolderInfo();
        //更新文件描述信息
        tb_FileNewName.Text = "";
        //更新重命名文本框
        lbl_FileDescription.Text = "";
        //调用自定义消息提示
        ShowMessageBox("删除成功!");
    }
    #endregion
    #region 重命名文件
    protected void btn_Rename_Click(object sender, EventArgs e)
    {
        //从web.config中读取文件上传路径
        string strFileUpladPath = ConfigurationManager.AppSettings["FileUplodePath"].ToString();
        //从列表框中控件中读取选择的文件名
        string strFileName = lb_FileList.SelectedValue;
        //重命名文本框或得选择的文件名
        string strFileNewName = tb_FileNewName.Text;
        //组合成物理路径
        string strFilePhysicalPath = Server.MapPath(strFileUpladPath + "/") + strFileName;
        //组合成新物理路径
        string strFileNewPhysicalPath = Server.MapPath(strFileUpladPath + "/") + strFileNewName;
        //文件重命名,即获取新地址覆盖旧地址的过程
        System.IO.File.Move(strFilePhysicalPath, strFileNewPhysicalPath);
        //找到文件列表的匹配项
        ListItem li = lb_FileList.Items.FindByText(strFileName);
        //修改文字
        li.Text = strFileNewName;
        //修改值
        li.Value = strFileNewName;
        //调用自定义方法现实
        ShowMessageBox("文件覆盖成功!");
    }
    #endregion
    #region 下拉列表事件
    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = Convert.ToInt32(DropDownList1.SelectedValue);
    }
    #endregion
}
