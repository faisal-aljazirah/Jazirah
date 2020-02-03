Public Class Unit
    Inherits System.Web.UI.Page
    Dim dcl As New DCL.Conn.DataClassLayer
    Dim UserLanguage As Integer
    Dim DataLang As String
    Dim Cash, Insurance, View As String

    Public byteUnit, strUnitEn, strUnitAr, curFactor, strUnitGroup As String

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
        ds = dcl.GetDS("SELECT * FROM Stock_Units")
        repDepartments.DataSource = ds
        repDepartments.DataBind()
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
                Title = "لوحة التحكم | الوحدات"
                PageHeader.InnerText = "الوحدات"
                'Variables

                'Columns
                byteUnit = "الرقم"
                strUnitEn = "الوحده عربي"
                strUnitAr = "الوحده إنجليزي"
                curFactor = " المعامل"
                strUnitGroup = "المجموعه"

            Case Else
                DataLang = "En"
                Title = "Control Panel | Units "
                PageHeader.InnerText = "Units"
                'Variables

                'Columns
                byteUnit = "No"
                strUnitEn = "Service Arabic"
                strUnitAr = "Service English"
                curFactor = "Factor"
                strUnitGroup = "Group"

        End Select
    End Sub
End Class