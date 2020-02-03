Imports System.Web
Public Class User
    Const CookiesExpire As Integer = 30
    Const DefaultPage As String = "main.aspx"
    Dim dcl As New DCL.Conn.DataClassLayer
    Function ApplyUserLogin(ByVal strUserName As String, ByVal bRememberMe As Boolean) As String
        Try
            Dim ds As DataSet
            ds = dcl.GetDS("SELECT * FROM Cmn_Users AS U INNER JOIN Cmn_Users_Details AS UD ON U.strUserName=UD.strUserName INNER JOIN Hw_Departments AS D ON U.byteDepartment = D.byteDepartment AND U.strUserName='" & strUserName & "' AND bAccountStatus=1 AND (datePasswordExpiry>='" & Today.ToString("yyyy-MM-dd") & "' OR datePasswordExpiry Is Null)")
            HttpContext.Current.Session("UserName") = strUserName
            If CBool(ds.Tables(0).Rows(0).Item("bUILanguage")) = True Then
                HttpContext.Current.Session("UserLanguage") = 2
            Else
                HttpContext.Current.Session("UserLanguage") = 1
            End If

            dcl.ExecSQuery("UPDATE Cmn_Users SET dateLastLogin=GETDATE() WHERE strUserName='" & strUserName & "'")
            Dim ip As String = HttpContext.Current.Request.UserHostAddress.ToString
            If ip <> "::1" Then AddLog(strUserName, Now, 0, "Login", 0, 6, "IP Address[" & ip & "]")

            If bRememberMe = True Then
                If HttpContext.Current.Request.Cookies("UserName") Is Nothing Then
                    HttpContext.Current.Response.Cookies.Set(New HttpCookie("UserName", strUserName))
                Else
                    HttpContext.Current.Response.Cookies("UserName").Value = strUserName
                End If
                HttpContext.Current.Response.Cookies("UserName").Expires = DateTime.Now.AddYears(CookiesExpire)
            End If

            'Redirect to start page
            If ds.Tables(0).Rows(0).Item("strStartPage").ToString <> "" Then
                HttpContext.Current.Response.Redirect(ds.Tables(0).Rows(0).Item("strStartPage"))
            Else
                HttpContext.Current.Response.Redirect(ds.Tables(0).Rows(0).Item("strDepartmentEn") & "/" & DefaultPage)
            End If

            Return ""
        Catch ex As Exception
            Return "Err:" & ex.Message
        End Try
    End Function

    Sub AddError(ByVal strUserName As String, ByVal dateError As Date, ByVal byteApp As Byte, ByVal strSection As String, ByVal lngID As Long, ByVal byteType As Byte, ByVal strDescription As String)
        Try
            dcl.ExecScalar("INSERT INTO Cmn_Errors VALUES ('" & strUserName & "','" & dateError.ToString("yyyy-MM-dd HH:mm:ss") & "'," & byteApp & ",'" & strSection & "'," & lngID & "," & byteType & ",'" & strDescription & "')")
        Catch ex As Exception

        End Try
    End Sub

    Sub AddLog(ByVal strUserName As String, ByVal dateLog As Date, ByVal byteApp As Byte, ByVal strSection As String, ByVal lngID As Long, ByVal byteAction As Byte, ByVal strDescription As String)
        'id     Action
        '1      add
        '2      update
        '3      delete
        '4      approve
        '5      reject
        '6      login
        '7      logout

        Try
            dcl.ExecScalar("INSERT INTO Cmn_Logs VALUES ('" & strUserName & "','" & dateLog.ToString("yyyy-MM-dd HH:mm:ss") & "'," & byteApp & ",'" & strSection & "'," & lngID & "," & byteAction & ",'" & strDescription & "')")
        Catch ex As Exception

        End Try
    End Sub
End Class
