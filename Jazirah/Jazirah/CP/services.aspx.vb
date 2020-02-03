Public Class services
    Inherits System.Web.UI.Page
    Dim dcl As New DCL.Conn.DataClassLayer
    Dim UserLanguage As Integer
    Dim DataLang As String
    Dim Cash, Insurance, View As String

    Public intService, strServiceEn, strServiceAr, intGroup As String
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Remove this line after complete user login and settings
        '=>
        'Session("UserID") = 1
        'Session("UserLanguage") = 2
        '<=

        'TODO: check authorization
        'TODO: load system settings
        'TODO: load user settings
        UserLanguage = Session("UserLanguage")
        'TODO: get data

        loadLanguage()

        Dim ds As DataSet
        ds = DCL.GetDS("SELECT * FROM Hw_Services")
        repServices.DataSource = ds
        repServices.DataBind()
    End Sub


    Function showStatus(ByVal TransNo As Long, ByVal Status As Integer) As String
        If Status = 1 Then
            Return ""
        Else
            Return "<button type=""button"" onclick=""javascript:showOrder(" & TransNo & ");"" class=""btn btn-sm btn-primary""> " & View & " </button>"
        End If
    End Function

    Sub loadLanguage()
        Select Case UserLanguage
            Case 2
                DataLang = "Ar"
                Title = "لوحة التحكم | الخدمات"
                PageHeader.InnerText = "الخدمات"
                'Variables

                'Columns
                intService = "الرقم"
                strServiceEn = "الخدمه عربي"
                strServiceAr = "الخدمه إنجليزي"
                intGroup = "مجموعه "
                
            Case Else
                DataLang = "En"
                Title = "Control Panel | Services"
                PageHeader.InnerText = "Services"
                'Variables

                'Columns
                intService = "No"
                strServiceEn = "Service Arabic"
                strServiceAr = "Service English"
                intGroup = " Group "
               
        End Select
    End Sub
End Class