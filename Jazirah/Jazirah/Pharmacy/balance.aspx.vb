Public Class balance
    Inherits System.Web.UI.Page
    Dim dcl As New DCL.Conn.DataClassLayer
    Public DataLang As String
    Public btnSearch, btnAdvanced, plcItem As String

    Public ByteLanguage As Byte
    Public strUserName, strDateFormat, strTimeFormat As String

    Public strWait As String

    Private Sub balance_Init(sender As Object, e As EventArgs) Handles Me.Init
        ' User Options
        If (HttpContext.Current.Session("UserName") Is Nothing) Or (HttpContext.Current.Session("UserName") = "") Then
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
        loadLanguage()
    End Sub

    Sub loadLanguage()
        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                Title = "المخزون | الرصيد"
                'Variables
                strWait = "فضلا انتظر..."
                'Buttons
                btnSearch = "بحث"
                btnAdvanced = "متقدم"
                'Others
                plcItem = "اكتب رقم الصنف أو اسمه.."
            Case Else
                DataLang = "En"
                Title = "Stock | Balance"
                'Variables
                strWait = "Please wait..."
                'Buttons
                btnSearch = "Search"
                btnAdvanced = "Advanced"
                'Others
                plcItem = "Type item number or name.."
        End Select
    End Sub
End Class