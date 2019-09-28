Public Class MN
    Inherits System.Web.UI.MasterPage
    Public PageDir As String
    Dim UserLanguage As Integer
    Dim DataLang As String
    Dim English, Arabic As String
    Dim ReqNew As String
    Public currentFile As String

    Public mnuDashboard, mnuUser, mnuActions As String
    Public mnuApprovals As String

    Public lblNotification As String

    Public mnuProfile, mnuSettings, mnuHelp, mnuLogout As String

    Public ByteLanguage As Byte
    Public strUserName, strDateFormat, strTimeFormat As String

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        PageDir = "rtl"

        ' User Options
        If (HttpContext.Current.Session("UserName") Is Nothing) Or (HttpContext.Current.Session("UserName") = "") Then
            'Remove this line after complete user login and settings
            'HttpContext.Current.Session("UserName") = "SoftNet"
            Response.Redirect("../login.aspx")
        End If
        If HttpContext.Current.Session("UserLanguage") Is Nothing Then
            HttpContext.Current.Session("UserLanguage") = 1
        End If
        If HttpContext.Current.Session("UserDTFormat") Is Nothing Then
            HttpContext.Current.Session("UserDTFormat") = "yyyy-MM-dd"
        End If
        If HttpContext.Current.Session("UserTMFormat") Is Nothing Then
            HttpContext.Current.Session("UserTMFormat") = "HH:mm"
        End If

        strUserName = HttpContext.Current.Session("UserName")
        ByteLanguage = HttpContext.Current.Session("UserLanguage")
        strDateFormat = HttpContext.Current.Session("UserDTFormat")
        strTimeFormat = HttpContext.Current.Session("UserTMFormat")
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        'TODO: check authorization
        'TODO: load system settings
        'TODO: load user settings

        'TODO: get data
        currentFile = IO.Path.GetFileName(HttpContext.Current.Request.Url.AbsolutePath)

        loadLanguage()
        'divNotification.InnerHtml = lblNotification
        'getRequestsCount()
        'divNotificationList.InnerHtml = getNotifications()
        'TODO: UI design
        'lstLanguage.InnerHtml = showLanguageList()
    End Sub

    Sub loadLanguage()
        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                PageDir = "rtl"
                '
                English = "إنجليزي"
                Arabic = "عربي"
                ReqNew = "جديد"
                '
                lblNotification = "إشعارات"
                'Menus
                mnuProfile = "الملف الشخصي"
                mnuSettings = "الإعدادات"
                mnuHelp = "المساعدة"
                mnuLogout = "تسجيل خروج"
                mnuDashboard = "الإحصائيات"
                mnuUser = "المستخدم"
                mnuActions = "الإجراءات"
                mnuApprovals = "التعميدات"
            Case Else
                DataLang = "En"
                PageDir = "ltr"
                '
                English = "English"
                Arabic = "Arabic"
                ReqNew = "New"
                '
                lblNotification = "Notifications"
                'Menus
                mnuProfile = "Profile"
                mnuSettings = "Settings"
                mnuHelp = "Help"
                mnuLogout = "Logout"
                mnuDashboard = "Dashborad"
                mnuUser = "User"
                mnuActions = "Actions"
                mnuApprovals = "Approvals"
        End Select
    End Sub

    Function getApps() As String
        Dim Pharmacy, CP As String
        Select Case ByteLanguage
            Case 2
                Pharmacy = "الصيدلية"
                CP = "لوحة التحكم"
            Case Else
                Pharmacy = "Pharmacy"
                CP = "Control Panel"
        End Select
        Dim str As String = ""
        str = str & "<div aria-labelledby=""dropdown-flag"" class=""dropdown-menu"">"
        str = str & "<a href=""../Pharmacy/main.aspx"" class=""dropdown-item""><i class=""ficon icon-medkit""></i> " & Pharmacy & "</a>"
        If strUserName = "SoftNet" Then
            str = str & "<a href=""../CP/main.aspx"" class=""dropdown-item""><i class=""ficon icon-database2""></i> " & CP & "</a>"
        End If
        str = str & "</div>"
        Return str
    End Function

    Private Sub getRequestsCount()
        Dim dc As New DCL.Conn.XMLData
        Dim count As Integer = dc.GetDataCount(Server.MapPath("../data/xml/requests.xml"), "*[@status=0]", "")
        'If count > 0 Then
        '    divRequestCount.InnerHtml = count
        '    divRequestNew.InnerHtml = count & " " & ReqNew
        'Else
        '    divRequestCount.InnerHtml = ""
        '    divRequestNew.InnerHtml = ""
        'End If
    End Sub

    Private Function getNotifications() As String
        Dim str As String = ""

        Dim dc As New DCL.Conn.XMLData
        Dim count As Integer = dc.GetDataCount(Server.MapPath("../data/xml/requests.xml"), "Cancel_Invoice", "@status=0")
        If count > 0 Then
            str = str & "<a href=""javascript:void(0)"" class=""list-group-item""><div class=""media""><div class=""media-left valign-middle""><i class=""icon-medkit2 icon-bg-circle bg-red bg-darken-1""></i></div><div class=""media-body"">"
            str = str & "<h6 class=""media-heading red darken-1"">" & count & " Invoices need to be canceled." & "</h6><small><time datetime=""2015-06-11T18:29:20+08:00"" class=""media-meta text-muted"">30 minutes ago</time></small>"
            str = str & "</div></div></a>"
        End If
        Return str
    End Function
End Class