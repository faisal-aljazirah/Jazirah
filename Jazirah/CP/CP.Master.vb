Public Class CP
    Inherits System.Web.UI.MasterPage
    Public PageDir As String
    Dim UserLanguage As Integer
    Dim DataLang As String
    Dim English, Arabic As String
    Dim ReqNew As String
    Public currentFile As String

    Public mnuDashboard, mnuApplicationSettings, mnuMasterFiles, mnuHeathware, mnuStock As String
    Public mnuPharmacy, mnuDepartments, mnuServices, mnuUnits, mnuWarehouse As String

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
                'Menus
                mnuProfile = "الملف الشخصي"
                mnuSettings = "الإعدادات"
                mnuHelp = "المساعدة"
                mnuLogout = "تسجيل خروج"
                '
                mnuDashboard = "الإحصائيات"
                mnuApplicationSettings = "إعدادات البرامج"
                mnuMasterFiles = "الملفات الرئيسية"
                '
                mnuPharmacy = "الصيدلية"
                mnuHeathware = "النظام الصحي"
                mnuStock = "المخزون"
                mnuDepartments = "الأقسام"
                mnuServices = "الخدمات"
                mnuUnits = "الوحدات"
                mnuWarehouse = "المستودعات"
            Case Else
                DataLang = "En"
                PageDir = "ltr"
                '
                English = "English"
                Arabic = "Arabic"
                'Menus
                mnuProfile = "Profile"
                mnuSettings = "Settings"
                mnuHelp = "Help"
                mnuLogout = "Logout"
                '
                mnuDashboard = "Dashborad"
                mnuApplicationSettings = "Applications Settings"
                mnuMasterFiles = "Master Files"
                '
                mnuPharmacy = "Pharmacy"
                mnuHeathware = "Healthware"
                mnuStock = "Stock"
                mnuDepartments = "Departments"
                mnuServices = "Services"
                mnuUnits = "Units"
                mnuWarehouse = "Warehouses"
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

End Class