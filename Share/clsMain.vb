Imports System.Text
Imports System.Web

Public Class Main
    Dim strUserName As String
    Dim ByteLanguage As Byte
    Dim strDateFormat, strTimeFormat As String
    Dim DataLang As String

    Sub New()
        ' User Options
        If HttpContext.Current.Session("UserName") Is Nothing Then
            'Remove this line after complete user login and settings
            HttpContext.Current.Session("UserName") = ""
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

    Public Function drawNotificationList() As String
        Dim str As New StringBuilder("")
        Dim lblNotification, lblNew, lblReadAll As String
        Dim C_Message, R_Message As String
        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                lblNotification = "الإشعارات"
                lblNew = "جديد"
                lblReadAll = "المزيد من الإشعارات"

                C_Message = "لديك @@ طلبات لإلغاء فواتير"
                R_Message = "لديك @@ طلبات لإعادة أصناف"
            Case Else
                DataLang = "En"
                lblNotification = "Notifications"
                lblNew = "New"
                lblReadAll = "Read all notifications"

                C_Message = "You have @@ request to cancel invoices"
                R_Message = "You have @@ request to return items"
        End Select

        Dim dc As New DCL.Conn.XMLData
        Dim Count, C_Count, R_Count As Integer
        Dim C_List As String = ""

        If dc.CheckExist(HttpContext.Current.Server.MapPath("../data/xml/privileges.xml"), "Cancel_Invoice", "User", strUserName) = True Then
            C_Count = dc.GetDataCount(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"), "Cancel_Invoice", "@status=0")
            Count = Count + C_Count
        End If
        If dc.CheckExist(HttpContext.Current.Server.MapPath("../data/xml/privileges.xml"), "Return_Items", "User", strUserName) = True Then
            R_Count = dc.GetDataCount(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"), "Return_Items", "@status=0")
            Count = Count + R_Count
        End If

        If Count > 0 Then
            'Dropdown Button
            str.Append("<a href=""#"" data-toggle=""dropdown"" class=""nav-link nav-link-label""><i class=""ficon icon-bell4""></i><span class=""tag tag-pill tag-default tag-danger tag-default tag-up"" id=""divRequestCount"" runat=""server"">" & Count & "</span></a>")
            '--Starting
            str.Append("<ul class=""dropdown-menu dropdown-menu-media dropdown-menu-right"">")
            'Header
            str.Append("<li class=""dropdown-menu-header""><h6 class=""dropdown-header m-0""><span class=""grey darken-2"" id=""divNotification"" runat=""server"">" & lblNotification & "</span><span class=""notification-tag tag tag-default tag-danger float-xs-right m-0"" id=""divRequestNew"" runat=""server"">" & Count & " " & lblNew & "</span></h6></li>")
            'Body
            str.Append("<li class=""list-group scrollable-container"" id=""divNotificationList"" runat=""server"">")
            '-->List
            If C_Count > 0 Then
                str.Append("<a href=""../Main/approvals.aspx"" class=""list-group-item""><div class=""media"">")
                str.Append("<div class=""media-left valign-middle""><i class=""icon-medkit2 icon-bg-circle bg-red bg-darken-1""></i></div>")
                str.Append("<div class=""media-body""><h6 class=""media-heading red darken-1"">" & Replace(C_Message, "@@", C_Count) & "</h6><small><time datetime=""2015-06-11T18:29:20+08:00"" class=""media-meta text-muted"">&nbsp;</time></small></div>")
                str.Append("</div></a>")
            End If
            If R_Count > 0 Then
                str.Append("<a href=""../Main/approvals.aspx"" class=""list-group-item""><div class=""media"">")
                str.Append("<div class=""media-left valign-middle""><i class=""icon-medkit2 icon-bg-circle bg-red bg-darken-1""></i></div>")
                str.Append("<div class=""media-body""><h6 class=""media-heading red darken-1"">" & Replace(R_Message, "@@", R_Count) & "</h6><small><time datetime=""2015-06-11T18:29:20+08:00"" class=""media-meta text-muted"">&nbsp;</time></small></div>")
                str.Append("</div></a>")
            End If
            '<--
            str.Append("</li>")
            'Footer
            str.Append("<li class=""dropdown-menu-footer""><a href=""javascript:void(0)"" class=""dropdown-item text-muted text-xs-center"">" & lblReadAll & "</a></li>")
            '--Closing
            str.Append("</ul>")
        End If

        Return str.ToString
    End Function
End Class
