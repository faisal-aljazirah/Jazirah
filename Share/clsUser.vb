Imports System.Web
Public Class User
    Const CookiesExpire As Integer = 30
    Const DefaultPage As String = "main.aspx"
    Dim dcl As New DCL.Conn.DataClassLayer
    Function ApplyUserLogin(ByVal strUserName As String, ByVal bRememberMe As Boolean) As String
        Try
            Dim ds As DataSet
            ds = dcl.GetDS("SELECT * FROM Cmn_Users AS U INNER JOIN Hw_Departments AS D ON U.byteDepartment = D.byteDepartment AND strUserName='" & strUserName & "' AND bAccountStatus=1 AND (datePasswordExpiry>='" & Today.ToString("yyyy-MM-dd") & "' OR datePasswordExpiry Is Null)")
            HttpContext.Current.Session("UserName") = strUserName
            'gbytePriv = 15
            HttpContext.Current.Session("UserLanguage") = ds.Tables(0).Rows(0).Item("bUILanguage") + 1
            'Application.MenuBar = "SoftNetMainMenu"
            'If (CommandBars("SoftNetMainMenu").Controls(1).Caption = "&File" And gblanguage = ARABIC) Or (CommandBars("SoftNetMainMenu").Controls(1).Caption = "&ãáÝ" And gblanguage = ENGLISH) Then ChangeMenuOrientation CommandBars("SoftNetMainMenu").Controls
            dcl.ExecSQuery("UPDATE Cmn_Users SET dateLastLogin=GETDATE() WHERE strUserName='" & strUserName & "'")
            'SaveSetting "SoftNet", "General", "User Name", [User Name]
            'DisableMenuItems [User Name], gbyteSystem

            If bRememberMe = True Then
                If HttpContext.Current.Request.Cookies("UserName") Is Nothing Then
                    HttpContext.Current.Response.Cookies.Set(New HttpCookie("UserName", strUserName))
                Else
                    HttpContext.Current.Response.Cookies.Set(HttpContext.Current.Request.Cookies("UserName"))
                End If
                HttpContext.Current.Response.Cookies("UserName").Expires = DateTime.Now.AddYears(CookiesExpire)
            End If
            HttpContext.Current.Response.Redirect(ds.Tables(0).Rows(0).Item("strDepartmentEn") & "/" & DefaultPage)
            Return ""
        Catch ex As Exception
            Return "Err:" & ex.Message
        End Try
    End Function
End Class
