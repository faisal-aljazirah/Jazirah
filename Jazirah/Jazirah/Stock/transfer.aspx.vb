Imports System.Xml

Public Class transfer
    Inherits System.Web.UI.Page
    Dim dcl As New DCL.Conn.DataClassLayer
    Public DataLang As String
    Public btnSearch, btnAdvanced, plcItem As String

    Public ByteLanguage As Byte
    Public strUserName, strDateFormat, strTimeFormat As String

    Public strWait As String

    Dim WarehouseString As String = ""
    Public WarehouseList As String = ""

    Public FromDate, ToDate As String

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
        LoadPermissions()

        Dim Where As String
        Dim AllValue As String = ""
        If WarehouseString = "*" Then
            Where = ""
        Else
            Where = " AND byteWarehouse IN (" & WarehouseString & ")"
        End If

        Dim isSelected As Boolean = False
        Dim selected As String
        Dim dsWarehouse As DataSet = dcl.GetDS("SELECT * FROM Stock_Warehouses WHERE bActive=1 " & Where)
        For I = 0 To dsWarehouse.Tables(0).Rows.Count - 1
            selected = ""
            If isSelected = False Then
                selected = " selected=""selected"""
                isSelected = True
            End If
            WarehouseList = WarehouseList & "<option value=""" & dsWarehouse.Tables(0).Rows(I).Item("byteWarehouse") & """ " & selected & ">" & dsWarehouse.Tables(0).Rows(I).Item("strWarehouse" & DataLang) & "</option>"
        Next
        If Where = "" Then WarehouseList = "<option value=""0"">All</option>" & WarehouseList

        FromDate = DateAdd(DateInterval.Day, -1 * 7, Today).ToString("yyyy-MM-dd")
        ToDate = Today.ToString("yyyy-MM-dd")
    End Sub

    Private Sub LoadPermissions()
        Dim doc As New XmlDocument
        doc.Load(HttpContext.Current.Server.MapPath("../data/xml/permissions.xml"))
        Dim warehouse As XmlNodeList = doc.SelectNodes("Permissions/Warehouses/User[@ID='" & strUserName & "']/Transfer")
        For Each w As XmlNode In warehouse
            WarehouseString = WarehouseString & w.InnerText & ","
        Next
        If WarehouseString <> "" Then
            WarehouseString = Left(WarehouseString, WarehouseString.Length - 1)
        Else
            Throw New Exception("Wharehouse not assigned")
        End If
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