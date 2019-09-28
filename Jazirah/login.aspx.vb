Public Class login
    Inherits System.Web.UI.Page
    Dim dcl As New DCL.Conn.DataClassLayer
    Public user_name As String

    Public Declare Function desinit Lib "des3w32.dll" (ByVal strKey As String) As Integer
    Public Declare Function ecbencode Lib "des3w32.dll" (ByVal strOutput As String, ByVal strInput As String) As Integer
    Public Declare Function ecbdecode Lib "des3w32.dll" (ByVal strOutput As String, ByVal strInput As String) As Integer
    Const DesKey As String = "$SoftNet"
    Dim PW As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("UserName") = Nothing
        If Request.Form("submit") = "1" Then
            Dim UserName As String = Request.Form("username")
            Dim UserPass As String = Request.Form("userpassword")
            Dim RememberMe As Boolean
            If Request.Form("rememberme") = "1" Then RememberMe = True Else RememberMe = False

            If UserName <> "" And UserPass <> "" Then
                Dim sh As New Share.Security
                If sh.AuthenticateUser(UserName, UserPass) = 0 Then
                    Dim login As New Share.User
                    login.ApplyUserLogin(UserName, RememberMe)
                Else
                    msg.Visible = True
                End If
            End If
        Else
            Session("UserName") = Nothing
            If Request.Cookies("UserName") Is Nothing Then
                user_name = ""
            Else
                user_name = Request.Cookies("UserName").Value
            End If
        End If
    End Sub
End Class