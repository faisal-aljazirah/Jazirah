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
End Class