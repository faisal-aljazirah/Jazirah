Public Class ajax
    Inherits System.Web.UI.Page

    <System.Web.Services.WebMethod()>
    Public Shared Function changeLanguage(ByVal ReferenceID As Integer) As String
        Dim str As String = ""
        HttpContext.Current.Session("UserLanguage") = ReferenceID
        str = str & "<script>location.reload();</script>"
        Return str
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function drawNotificationList() As String
        Dim sh As New Share.Main
        Return sh.drawNotificationList()
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function viewChangePassword() As String
        Dim sh As New Share.Main
        Return sh.viewChangePassword()
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ChangePassword(ByVal UserName As String, ByVal OldPassword As String, ByVal NewPassword As String) As String
        Dim sh As New Share.Security
        Return sh.ChangePassword(UserName, OldPassword, NewPassword)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function UpdateProfile(ByVal strUserName As String, ByVal strFullName As String, ByVal strPosition As String, ByVal strMobile As String, ByVal strEmail As String, ByVal strExtension As String) As String
        Dim sh As New Share.Main
        Return sh.UpdateProfile(strUserName, strFullName, strPosition, strMobile, strEmail, strExtension)
    End Function
End Class