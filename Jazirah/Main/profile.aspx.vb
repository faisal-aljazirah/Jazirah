Public Class profile
    Inherits System.Web.UI.Page
    Dim UserLanguage As Integer
    Dim DataLang As String

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
    End Sub

    Sub loadLanguage()
        Select Case UserLanguage
            Case 2
                DataLang = "Ar"
                Title = "الرئيسية | ملفك الشخصي"
            Case Else
                DataLang = "En"
                Title = "Main | Your Profile"
        End Select
    End Sub

End Class