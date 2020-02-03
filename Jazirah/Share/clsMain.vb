Imports System.Text
Imports System.Web
Imports System.Xml

Public Class Main
    Dim strUserName As String
    Dim ByteLanguage As Byte
    Dim strDateFormat, strTimeFormat As String
    Dim DataLang As String

    Sub New()
        ' User Options
        If HttpContext.Current.Session("UserName") Is Nothing Then
            'Remove this line after complete user login and settings
            'HttpContext.Current.Session("UserName") = ""
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
        Dim C_Message, R_Message, O_Message As String
        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                lblNotification = "الإشعارات"
                lblNew = "جديد"
                lblReadAll = "المزيد من الإشعارات"

                C_Message = "لديك @@ طلبات لإلغاء فواتير"
                R_Message = "لديك @@ طلبات لإعادة أصناف"
                O_Message = "لديك @@ طلبات لإعادة فواتير"
            Case Else
                DataLang = "En"
                lblNotification = "Notifications"
                lblNew = "New"
                lblReadAll = "Read all notifications"

                C_Message = "You have @@ request to cancel invoices"
                R_Message = "You have @@ request to return items"
                O_Message = "You have @@ request to re-open invoices"
        End Select

        Dim dc As New DCL.Conn.XMLData
        Dim Count, C_Count, R_Count, O_Count As Integer
        Dim C_List As String = ""

        If dc.CheckExistNode(HttpContext.Current.Server.MapPath("../data/xml/privileges.xml"), "Cancel_Invoice", "User", strUserName) = True Then
            C_Count = dc.GetDataCount(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"), "Cancel_Invoice", "@status=0")
            Count = Count + C_Count
        End If
        If dc.CheckExistNode(HttpContext.Current.Server.MapPath("../data/xml/privileges.xml"), "Return_Items", "User", strUserName) = True Then
            R_Count = dc.GetDataCount(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"), "Return_Items", "@status=0")
            Count = Count + R_Count
        End If
        If dc.CheckExistNode(HttpContext.Current.Server.MapPath("../data/xml/privileges.xml"), "Reopen_Invoice", "User", strUserName) = True Then
            O_Count = dc.GetDataCount(HttpContext.Current.Server.MapPath("../data/xml/requests.xml"), "Reopen_Invoice", "@status=0")
            Count = Count + O_Count
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
            If O_Count > 0 Then
                str.Append("<a href=""../Main/approvals.aspx"" class=""list-group-item""><div class=""media"">")
                str.Append("<div class=""media-left valign-middle""><i class=""icon-medkit2 icon-bg-circle bg-red bg-darken-1""></i></div>")
                str.Append("<div class=""media-body""><h6 class=""media-heading red darken-1"">" & Replace(O_Message, "@@", O_Count) & "</h6><small><time datetime=""2015-06-11T18:29:20+08:00"" class=""media-meta text-muted"">&nbsp;</time></small></div>")
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

    Public Function drawNavMenu(ByVal CurrentFolder As String, ByVal CurrentFile As String) As String
        Dim Nav As New StringBuilder("")
        Dim mnuDashboard, mnuUser, mnuActions As String
        Dim mnuProfile, mnuApprovals As String

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                mnuDashboard = "لوحة التقييم"
                mnuUser = "المستخدم"
                mnuActions = "الإجراءات"
                mnuProfile = "الملف الشخصي"
                mnuApprovals = "التعميدات"
            Case Else
                DataLang = "En"
                mnuDashboard = "Dashboard"
                mnuUser = "User"
                mnuActions = "Actions"
                mnuProfile = "Profile"
                mnuApprovals = "Approvals"
        End Select

        '==> Open
        Nav.Append("<ul id=""main-menu-navigation"" data-menu=""menu-navigation"" class=""navigation navigation-main"">")
        'Dashbord
        Nav.Append("<li class=""nav-item""><a href=""main.aspx""><i class=""icon-home3""></i><span data-i18n=""nav.dash.main"" class=""menu-title"">" & mnuDashboard & "</span></a></li>")
        'Get Application ID from (current Folder)
        If CurrentFolder <> "Main" Then
            Dim AppID As Integer
            Dim doc, docp As New XmlDocument()
            doc.Load(HttpContext.Current.Server.MapPath("../data/xml/ui.xml"))
            Dim app As XmlNode = doc.DocumentElement.SelectSingleNode("Application[@Enabled=1 and Folder=""" & CurrentFolder & """]")
            If Not (app Is Nothing) Then
                AppID = app.SelectSingleNode("ID").InnerText
            Else
                AppID = 0
            End If
            Dim Active As String
            If CurrentFolder = "CP" Then
                doc.Load(HttpContext.Current.Server.MapPath("../data/xml/cp.xml"))
            Else
                doc.Load(HttpContext.Current.Server.MapPath("../data/xml/ui.xml"))
            End If
            docp.Load(HttpContext.Current.Server.MapPath("../data/xml/permissions.xml"))
            Dim p_nodes As XmlNode = docp.DocumentElement.SelectSingleNode("User[@ID=""" & strUserName & """]")
            Dim section_nodes As XmlNodeList = doc.DocumentElement.SelectNodes("Section[App=" & AppID & "]")
            For Each s_node As XmlNode In section_nodes
                Nav.Append("<li class=""navigation-header""><span data-i18n=""nav.category.support"">" & s_node.SelectSingleNode("Name" & DataLang).InnerText & "</span><i data-toggle=""tooltip"" data-placement=""left"" data-original-title=""" & s_node.SelectSingleNode("Name" & DataLang).InnerText & """ class=""icon-ellipsis icon-ellipsis""></i></li>")
                Dim menu_nodes As XmlNodeList = doc.DocumentElement.SelectNodes("Menu[@Enabled=1 and Section=" & s_node.SelectSingleNode("ID").InnerText & " and Parent=0]")
                For Each m_node As XmlNode In menu_nodes
                    If m_node.SelectSingleNode("File").InnerText <> "" Then
                        Dim id As Integer = m_node.SelectSingleNode("ID").InnerText
                        p_nodes = docp.DocumentElement.SelectSingleNode("Menus/User[@ID=""" & strUserName & """ and (Menu=" & id & " or Menu=""*"")]")
                        If Not (p_nodes Is Nothing) Then
                            If CurrentFile = m_node.SelectSingleNode("File").InnerText Then Active = "active" Else Active = ""
                            Nav.Append("<li class=""nav-item " & Active & """><a href=""../" & CurrentFolder & "/" & m_node.SelectSingleNode("File").InnerText & """><i class=""" & m_node.SelectSingleNode("Icon").InnerText & """></i><span data-i18n=""nav.orders.main"" class=""menu-title"">" & m_node.SelectSingleNode("Name" & DataLang).InnerText & "</span></a></li>")
                        End If
                    Else
                        Dim retMenu As String = getSubMenu(CurrentFolder, CurrentFile, m_node.SelectSingleNode("ID").InnerText)
                        If retMenu <> "" Then
                            Nav.Append("<li class=""nav-item""><a href=""#""><i class=""" & m_node.SelectSingleNode("Icon").InnerText & """></i><span data-i18n=""nav.orders.main"" class=""menu-title"">" & m_node.SelectSingleNode("Name" & DataLang).InnerText & "</span></a>")
                            Nav.Append("<ul class=""menu-content"">")
                            Nav.Append(retMenu)
                            Nav.Append("</ul>")
                            Nav.Append("</li>")
                        End If
                    End If
                Next
            Next
        Else
            Dim active As String
            Nav.Append("")
            Nav.Append("<li class="" nav-item""><a href=""#""><i class=""icon-user""></i><span class=""menu-title"">" & mnuUser & "</span></a>")
            If CurrentFile = "profile.aspx" Then active = "class=""active""" Else active = ""
            Nav.Append("<ul class=""menu-content""><li " & active & "><a href=""..\Main\profile.aspx"" class=""menu-item"">" & mnuProfile & "</a></li></ul>")
            Nav.Append("</li>")
            Nav.Append("<li class=""nav-item""><a href=""#""><i class=""icon-paste""></i><span class=""menu-title"">" & mnuActions & "</span></a>")
            If CurrentFile = "approvals.aspx" Then active = "class=""active""" Else active = ""
            Nav.Append("<ul class=""menu-content""><li " & active & "><a href=""..\Main\approvals.aspx"" class=""menu-item"">" & mnuApprovals & "</a></li></ul>")
            Nav.Append("</li>")
        End If
        '==> Close
        Nav.Append("</ul>")
        Return Nav.ToString
    End Function

    Private Function getSubMenu(ByVal CurrentFolder As String, ByVal CurrentFile As String, ByVal Parent As Integer) As String
        Dim Nav As New StringBuilder("")
        Dim Active As String
        Dim doc, docp As New XmlDocument()
        If CurrentFolder = "CP" Then
            doc.Load(HttpContext.Current.Server.MapPath("../data/xml/cp.xml"))
        Else
            doc.Load(HttpContext.Current.Server.MapPath("../data/xml/ui.xml"))
        End If
        docp.Load(HttpContext.Current.Server.MapPath("../data/xml/permissions.xml"))
        Dim p_nodes As XmlNode ' = docp.DocumentElement.SelectSingleNode("User[@ID=""" & strUserName & """]")
        Dim menu_nodes As XmlNodeList = doc.DocumentElement.SelectNodes("Menu[@Enabled=1 and Parent=" & Parent & "]")
        For Each m_node As XmlNode In menu_nodes
            If m_node.SelectSingleNode("File").InnerText <> "" Then
                Dim id As Integer = m_node.SelectSingleNode("ID").InnerText
                p_nodes = docp.DocumentElement.SelectSingleNode("Menus/User[@ID=""" & strUserName & """ and (Menu=" & id & " or Menu=""*"")]")
                If Not (p_nodes Is Nothing) Then
                    If currentFile = m_node.SelectSingleNode("File").InnerText Then Active = "active" Else Active = ""
                    Nav.Append("<li class=""nav-item " & Active & """><a href=""../" & CurrentFolder & "/" & m_node.SelectSingleNode("File").InnerText & """><i class=""" & m_node.SelectSingleNode("Icon").InnerText & """></i><span data-i18n=""nav.orders.main"" class=""menu-title"">" & m_node.SelectSingleNode("Name" & DataLang).InnerText & "</span></a></li>")
                End If
            Else
                Dim retMenu As String = getSubMenu(CurrentFolder, CurrentFile, m_node.SelectSingleNode("ID").InnerText)
                If retMenu <> "" Then
                    Nav.Append("<li class=""nav-item""><a href=""#""><i class=""" & m_node.SelectSingleNode("Icon").InnerText & """></i><span data-i18n=""nav.orders.main"" class=""menu-title"">" & m_node.SelectSingleNode("Name" & DataLang).InnerText & "</span></a>")
                    Nav.Append("<ul class=""menu-content"">")
                    Nav.Append(retMenu)
                    Nav.Append("</ul>")
                    Nav.Append("</li>")
                End If
            End If
        Next
        Return Nav.ToString
    End Function

    Public Function drawApplicationList() As String
        Dim Pharmacy, CP As String
        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                Pharmacy = "الصيدلية"
                CP = "لوحة التحكم"
            Case Else
                DataLang = "En"
                Pharmacy = "Pharmacy"
                CP = "Control Panel"
        End Select
        Dim str As String = ""
        Dim doc, docp As New XmlDocument()
        doc.Load(HttpContext.Current.Server.MapPath("../data/xml/ui.xml"))
        Dim app_nodes As XmlNodeList = doc.DocumentElement.SelectNodes("Application[@Enabled=1]")
        docp.Load(HttpContext.Current.Server.MapPath("../data/xml/permissions.xml"))
        Dim p_nodes As XmlNode

        'str = str & "<div aria-labelledby=""dropdown-flag"" class=""dropdown-menu"">"
        For Each a_node As XmlNode In app_nodes
            Dim id As Integer = a_node.SelectSingleNode("ID").InnerText
            p_nodes = docp.DocumentElement.SelectSingleNode("Applications/User[@ID=""" & strUserName & """ and (Menu=" & id & " or Menu=""*"")]")
            If Not (p_nodes Is Nothing) Then
                str = str & "<a href=""../" & a_node.SelectSingleNode("Folder").InnerText & "/main.aspx"" class=""dropdown-item""><i class=""" & a_node.SelectSingleNode("Icon").InnerText & """></i> " & a_node.SelectSingleNode("Name" & DataLang).InnerText & "</a>"
            End If
        Next

        'If strUserName = "SoftNet" Then
        '    str = str & "<a href=""../CP/main.aspx"" class=""dropdown-item""><i class=""ficon icon-database2""></i> " & CP & "</a>"
        'End If
        'str = str & "</div>"
        Return str
    End Function

    Public Function drawUserList() As String
        Dim mnuHomePage, mnuProfile, mnuSettings, mnuStartPage, mnuHelp, mnuLogout As String
        Dim msgStartPage As String

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                'Menus
                mnuHomePage = "الرئيسية"
                mnuProfile = "الملف الشخصي"
                mnuSettings = "الإعدادات"
                mnuStartPage = "تعيين كصفحة البداية"
                mnuHelp = "المساعدة"
                mnuLogout = "تسجيل خروج"
                'Messages
                msgStartPage = "هل أنت متأكد من تعيين هذه الصفحة كصفحة البداية؟"
            Case Else
                DataLang = "En"
                'Menus
                mnuHomePage = "Home"
                mnuProfile = "Profile"
                mnuSettings = "Settings"
                mnuStartPage = "Set As Start Page"
                mnuHelp = "Help"
                mnuLogout = "Logout"
                'Messages
                msgStartPage = "Are you sure to set this page as a start page?"
        End Select

        Dim currentFile As String = IO.Path.GetFileName(HttpContext.Current.Request.Url.AbsolutePath)
        Dim currentFolder As String = IO.Directory.GetParent(HttpContext.Current.Request.Url.AbsolutePath).Name

        Dim str As String = ""
        'logo and name
        str = str & "<a href=""#"" data-toggle=""dropdown"" class=""dropdown-toggle nav-link dropdown-user-link""><span class=""avatar avatar-online""><img src=""../app-assets/images/portrait/small/avatar-s-1.png"" alt=""avatar""><i></i></span><span class=""user-name"">" & strUserName & "</span></a>"
        'menu
        str = str & "<div class=""dropdown-menu dropdown-menu-right"">"
        str = str & "<a href=""../Main/profile.aspx"" class=""dropdown-item""><i class=""icon-head""></i> " & mnuProfile & "</a>"
        str = str & "<a href=""javascript:showGlobalForm('../Main/gajax.aspx/modifyWebsiteSettings','modal','#mdlSettings',0,0,0,'');"" class=""dropdown-item""><i class=""icon-gear-b""></i> " & mnuSettings & "</a>"
        str = str & "<a href=""javascript:confirm('','" & msgStartPage & "',setstartPage);"" class=""dropdown-item""><i class=""icon-home4""></i> " & mnuStartPage & "</a>"
        str = str & "<script>function setstartPage() { executeGlobalmethod('../Main/gajax.aspx/SetStartPage', '#mdlSettings', { strUserName: '" & strUserName & "', strStartPage: '" & currentFolder & "/" & currentFile & "' }); }</script>"
        str = str & "<a href=""javascript:void()"" class=""dropdown-item""><i class=""icon-help-buoy""></i> " & mnuHelp & "</a>"
        str = str & "<div class=""dropdown-divider""></div>"
        str = str & "<a href=""../login.aspx"" class=""dropdown-item""><i class=""icon-power3""></i> " & mnuLogout & "</a>"
        str = str & "</div>"

        Return str
    End Function

    Public Sub selectColumns(ByVal Source As String, ByVal Column As Integer, ByVal State As String)
        If Column <> 0 Then
            If HttpContext.Current.Session(Source) Is Nothing Then HttpContext.Current.Session(Source) = ""
            If State = "false" Then
                If HttpContext.Current.Session(Source) = "" Then HttpContext.Current.Session(Source) = Column.ToString Else HttpContext.Current.Session(Source) = HttpContext.Current.Session(Source) & "," & Column
            Else
                Dim str As String = ""
                Dim col() As String = Split(HttpContext.Current.Session(Source), ",")
                For Each c In col
                    If c <> "" Then
                        If c <> Column Then str = str & c & ","
                    End If
                Next
                If str.Length = 0 Then HttpContext.Current.Session(Source) = "" Else HttpContext.Current.Session(Source) = Left(str, str.Length - 1)
                'HttpContext.Current.Session("r_sales_col") = Replace(HttpContext.Current.Session("r_sales_col"), "," & Column, "")
            End If
        End If
    End Sub

    Public Function viewChangePassword() As String
        Dim Header As String
        Dim lblOldPassword, lblNewPassword, lblConfirmPassword As String
        Dim btnSave, btnClose As String

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                Header = "تغيير كلمة المرور"
                lblOldPassword = "كلمة المرور القديمة"
                lblNewPassword = "كلمة المرور الجديدة"
                lblConfirmPassword = "تأكيد كلمة المرور"
                btnSave = "حفظ"
                btnClose = "إغلاق"
            Case Else
                DataLang = "En"
                Header = "Change Password"
                lblOldPassword = "Old Password"
                lblNewPassword = "New Password"
                lblConfirmPassword = "Confirm Password"
                btnSave = "Save"
                btnClose = "Close"
        End Select

        Dim body As New StringBuilder("")
        body.Append("<form id=""frmPassword"">")
        body.Append("<div class=""row"">")
        body.Append("<div class=""col-md-12 form-group""><div class=""col-md-6""><h6 class=""text-md-right"">" & lblOldPassword & ":</h6></div><div class=""col-md-6""><input class=""form-control form-control-sm input-sm"" id=""txtOldPassword"" type=""password"" /></div></div>")
        body.Append("<div class=""col-md-12 form-group""><div class=""col-md-6""><h6 class=""text-md-right"">" & lblNewPassword & ":</h6></div><div class=""col-md-6""><input class=""form-control form-control-sm input-sm"" id=""txtNewPassword"" type=""password"" /></div></div>")
        body.Append("<div class=""col-md-12 form-group""><div class=""col-md-6""><h6 class=""text-md-right"">" & lblConfirmPassword & ":</h6></div><div class=""col-md-6""><input class=""form-control form-control-sm input-sm"" id=""txtConfirmPassword"" type=""password"" /></div></div>")
        body.Append("</div>")
        body.Append("</form>")
        body.Append("<script>function validateChangePassword() {$('#txtOldPassword, #txtNewPassword, #txtConfirmPassword').removeClass('border-red');if ($('#txtOldPassword').val() == '') {$('#txtOldPassword').addClass('border-red');$('#txtOldPassword').focus();return false;} else {if ($('#txtNewPassword').val() == '') {$('#txtNewPassword').addClass('border-red');$('#txtNewPassword').focus();return false;} else {if ($('#txtConfirmPassword').val() == '') {$('#txtConfirmPassword').addClass('border-red');$('#txtConfirmPassword').focus();return false;} else {if ($('#txtNewPassword').val() != $('#txtConfirmPassword').val()) {$('#txtConfirmPassword').addClass('border-red');$('#txtConfirmPassword').focus();return false;} else {return true;}}}}}</script>")

        Dim mdl As New Share.UI
        Return mdl.drawModal(Header, body.ToString, "<button type=""button"" id=""btnSave"" class=""btn btn-success ml-1"" onclick=""javascript:if(validateChangePassword()) changePassword('" & strUserName & "',$('#txtOldPassword').val(),$('#txtNewPassword').val());""><i class=""icon-check2""></i>" & btnSave & "</button><button type=""button"" class=""btn btn-warning ml-1"" data-dismiss=""modal""><i class=""icon-cross2""></i>" & btnClose & "</button>", UI.ModalSize.Small)
    End Function

    Public Function UpdateProfile(ByVal strUserName As String, ByVal strFullName As String, ByVal strPosition As String, ByVal strMobile As String, ByVal strEmail As String, ByVal strExtension As String) As String
        Try
            Dim dcl As New DCL.Conn.DataClassLayer
            dcl.ExecScalar("UPDATE Cmn_Users_Details SET strFullName='" & strFullName & "',strPosition='" & strPosition & "',strMobile='" & strMobile & "',strEmail='" & strEmail & "',strExtension='" & strExtension & "' WHERE strUserName='" & strUserName & "'")
            Dim usr As New Share.User
            usr.AddLog(strUserName, Now, 0, "Profile", 0, 2, "Update Profile")
            Return "<script>msg('','Your profile has updated successfully!','success');</script>"
        Catch ex As Exception
            Return "Err:" & ex.Message
        End Try
    End Function

    Public Function SetStartPage(ByVal strUserName As String, ByVal strStartPage As String) As String
        Dim msgSuccess As String
        Select Case ByteLanguage
            Case 2
                msgSuccess = "تم تحديث صفحة البداية الخاصة بك بنجاح!"
            Case Else
                msgSuccess = "Your start page has updated successfully!"
        End Select

        Try
            Dim dcl As New DCL.Conn.DataClassLayer
            dcl.ExecScalar("UPDATE Cmn_Users_Details SET strStartPage='" & strStartPage & "' WHERE strUserName='" & strUserName & "'")
            Dim usr As New Share.User
            usr.AddLog(strUserName, Now, 0, "Profile", 0, 2, "Update Start Page")
            Return "<script>msg('','" & msgSuccess & "','success');</script>"
        Catch ex As Exception
            Return "Err:" & ex.Message
        End Try
    End Function
End Class
