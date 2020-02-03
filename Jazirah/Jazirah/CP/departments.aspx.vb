Public Class departments
    Inherits System.Web.UI.Page
    Dim dcl As New DCL.Conn.DataClassLayer
    Dim UserLanguage As Integer
    Dim DataLang As String
    Dim Cash, Insurance, View As String

    Public colID, colNameAr, colNameEn, colAdmission, colMedicalCenter, colCenter As String

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
        ds = dcl.GetDS("SELECT * FROM Hw_Departments")
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
                Title = "لوحة التحكم | الأقسام"
                PageHeader.InnerText = "الاقسام"
                'Variables
                
                'Columns
                colID = "الرقم"
                colNameAr = "الاسم عربي"
                colNameEn = "الاسم إنجليزي"
                colAdmission = "القبول"
                colMedicalCenter = "المركز الطبي"
                colCenter = "المركز"
            Case Else
                DataLang = "En"
                Title = "Control Panel | Departments"
                PageHeader.InnerText = "Departments"
                'Variables
                
                'Columns
                colID = "No"
                colNameAr = "Name Arabic"
                colNameEn = "Name English"
                colAdmission = "Admission"
                colMedicalCenter = "Medical Center"
                colCenter = "Center"
        End Select
    End Sub

End Class