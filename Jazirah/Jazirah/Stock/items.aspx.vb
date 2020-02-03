Imports System.Xml

Public Class items
    Inherits System.Web.UI.Page
    Dim dcl As New DCL.Conn.DataClassLayer
    Public DataLang As String
    Public btnSearch, btnAdvanced, plcItem As String

    Public ByteLanguage As Byte
    Public strUserName, strDateFormat, strTimeFormat As String

    Public strWait As String

    Dim All, Enabled, Disabled As String

    'Dim WarehouseString As String = ""
    Public GroupList As String = ""
    Public AvailableList As String = ""

    Public FromDate, ToDate As String

    Private Sub balance_Init(sender As Object, e As EventArgs) Handles Me.Init
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
        loadLanguage()
        LoadPermissions()

        AvailableList = AvailableList & "<option value=""0"">" & All & "</option>"
        AvailableList = AvailableList & "<option value=""1"" selected=""selected"">" & Enabled & "</option>"
        AvailableList = AvailableList & "<option value=""2"">" & Disabled & "</option>"

        GroupList = "<option value=""0"" selected=""selected"">" & All & "</option>"
        Dim dsGroups As DataSet = dcl.GetDS("SELECT * FROM Stock_Groups WHERE bPharmacy=1 ")
        For I = 0 To dsGroups.Tables(0).Rows.Count - 1
            GroupList = GroupList & "<option value=""" & dsGroups.Tables(0).Rows(I).Item("intGroup") & """>" & dsGroups.Tables(0).Rows(I).Item("strGroup" & DataLang) & "</option>"
        Next
    End Sub

    Private Sub LoadPermissions()
        'Dim doc As New XmlDocument
        'doc.Load(HttpContext.Current.Server.MapPath("../data/xml/permissions.xml"))
        'Dim warehouse As XmlNodeList = doc.SelectNodes("Permissions/Warehouses/User[@ID='" & strUserName & "']/Transfer")
        'For Each w As XmlNode In warehouse
        '    WarehouseString = WarehouseString & w.InnerText & ","
        'Next
        'If WarehouseString <> "" Then
        '    WarehouseString = Left(WarehouseString, WarehouseString.Length - 1)
        'Else
        '    Throw New Exception("Wharehouse not assigned")
        'End If


    End Sub

    Sub loadLanguage()
        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                Title = "المستودع | الأصناف"
                'Variables
                strWait = "فضلا انتظر..."
                'Buttons
                btnSearch = "بحث"
                btnAdvanced = "متقدم"
                'Others
                plcItem = "اكتب رقم الصنف أو اسمه.."
            Case Else
                DataLang = "En"
                Title = "Warehouse | Items"
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