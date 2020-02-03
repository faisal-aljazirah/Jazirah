Imports System.Web.Script.Serialization

Public Class gajax
    Inherits System.Web.UI.Page

    Private Class NameValue
        Public Property name As String
        Public Property value As String
    End Class

#Region "Website Settings"
    <System.Web.Services.WebMethod()> _
    Public Shared Function modifyWebsiteSettings(ByVal Type As String, ByVal Options As String, ByVal ReferenceID As Integer) As String
        'Dim shrd As New Share.PageParts
        'Return shrd.modifyWebsiteSettings(Type, Options, ReferenceID)
        Dim UserID As Integer = 1
        Dim DataLang As String
        Dim dcl As New DCL.Conn.DataClassLayer
        ' User Settings
        Dim UserLang As Integer = HttpContext.Current.Session("UserLanguage")
        Dim UserFont, DateFormat, TimeFormat, UserCurrency As String

        Dim Content As New StringBuilder("")
        'Dim fld As New Share.FormFields
        Dim Width As Integer = 0
        Dim Header As String = ""
        Dim Footer As String = ""
        Dim lbl_Font, lbl_Language, lbl_DateFormat, lbl_TimeFormat, lbl_Currency As String
        Dim btnSave, btnCancel As String
        Dim FontLabelList(1), FontValueList(1), DateFormatLabelList(1), DateFormatValueList(1), TimeFormatLabelList(1), TimeFormatValueList(1) As String
        Dim CurrencyLabelList(1), CalendarValueList(1), LanguageLabelList(1), LanguageValueList(1) As String
        Dim FontValue, CalendarValue, LanguageValue, DateFormatValue, TimeFormatValue As String

        Select Case UserLang
            Case 2
                DataLang = "AR"
                Header = "تغيير الإعدادات"
                lbl_Font = "الخط:"
                lbl_Language = "اللغة:"
                lbl_DateFormat = "تنسيق التاريخ:"
                lbl_TimeFormat = "تنسيق الوقت:"
                lbl_Currency = "العملة:"
                btnSave = "حفظ التغييرات"
                btnCancel = "تراجع"
                CurrencyLabelList(0) = "ر.س"
                LanguageLabelList(1) = "عربي"
                LanguageLabelList(0) = "إنجليزي"
            Case Else
                DataLang = "EN"
                Header = "Change the settings"
                lbl_Font = "Font:"
                lbl_Language = "Language:"
                lbl_DateFormat = "Date Format:"
                lbl_TimeFormat = "Time Format"
                lbl_Currency = "Currency:"
                btnSave = "Save Changes"
                btnCancel = "Cancel"
                CurrencyLabelList(0) = "S.R"
                LanguageLabelList(1) = "Arabic"
                LanguageLabelList(0) = "English"
        End Select

        Content.Append("<form id=""frmWebsite"">")
        Content.Append("<div class=""row"">")
        Content.Append("<div class=""col-md-12 ml-0 mr-0 pl-0 pr-0""><div class=""col-md-6""><h5 class=""text-md-right"">" & lbl_Font & "</h5></div><div class=""col-md-6""><select class=""form-control"" name=""drpFont""><option value=""1"">Cordoba</option></select></div></div>")
        Content.Append("<div class=""col-md-12 ml-0 mr-0 pl-0 pr-0""><div class=""col-md-6""><h5 class=""text-md-right"">" & lbl_Language & "</h5></div><div class=""col-md-6""><select class=""form-control"" name=""drpLanguage""><option value=""1"">" & LanguageLabelList(0) & "</option><option value=""2"">" & LanguageLabelList(1) & "</option></select></div></div>")
        Content.Append("<div class=""col-md-12 ml-0 mr-0 pl-0 pr-0""><div class=""col-md-6""><h5 class=""text-md-right"">" & lbl_DateFormat & "</h5></div><div class=""col-md-6""><select class=""form-control"" name=""drpDateFormat""><option value=""yyyy-MM-dd"">yyyy-MM-dd</option><option value=""dd-MM-yyy"">dd-MM-yyy</option></select></div></div>")
        Content.Append("<div class=""col-md-12 ml-0 mr-0 pl-0 pr-0""><div class=""col-md-6""><h5 class=""text-md-right"">" & lbl_TimeFormat & "</h5></div><div class=""col-md-6""><select class=""form-control"" name=""drpTimeFormat""><option value=""HH:mm"">HH:mm</option><option value=""hh:mm tt"">hh:mm tt</option></select></div></div>")
        Content.Append("<div class=""col-md-12 ml-0 mr-0 pl-0 pr-0""><div class=""col-md-6""><h5 class=""text-md-right"">" & lbl_Currency & "</h5></div><div class=""col-md-6""><select class=""form-control"" name=""drpCurrency""><option value=""3"">" & CurrencyLabelList(0) & "</option></select></div></div>")
        Content.Append("</div>")
        Content.Append("</form>")
        Content.Append("<script type=""text/javascript"">function updateWebsite() {var valJson = JSON.stringify($('#frmWebsite').serializeArray());var dataJson = { ReferenceID: " & UserID & ", fields: valJson };var dataJsonString = JSON.stringify(dataJson);$.ajax({type: 'POST',url: '../Main/gajax.aspx/updateWebsiteSettings',data: dataJsonString,contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {if (response.d.indexOf('Err:') >= 0) {msg('',response.d.substring(4, response.d.length),'error');} else {window.location.reload();}},failure: function (msg) {alert(msg);},error: function (xhr, ajaxOptions, thrownError) {alert(' write json item, Ajax error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});}</script>")

        Footer = "<div class=""text-md-center""><button type=""button"" id=""btnSave"" class=""btn btn-primary mr-1 ml-1"" onclick=""javascript:updateWebsite()"">" & btnSave & "</button><button id=""btnClose"" type=""button"" onclick=""javascript:$('#mdlSettings').modal('hide');"" class=""btn btn-default"">" & btnCancel & "</button></div>"

        Dim Ret As String = "<div class=""modal-dialog modal-sm"" role=""document""><div class=""modal-content""><div class=""modal-header""><button type=""button"" class=""close"" data-dismiss=""modal"" aria-label=""Close""><span aria-hidden=""true"">&times;</span></button><h4 class=""modal-title"" id="""">" & Header & "</h4></div><div class=""modal-body"">" & Content.ToString & "</div><div class=""modal-footer"">" & Footer & "</div></div></div>"

        Return Ret
    End Function

    <System.Web.Services.WebMethod()> _
    Public Shared Function updateWebsiteSettings(ByVal ReferenceID As Integer, ByVal fields As String) As String
        'Dim shrd As New Share.PageParts
        'Return shrd.updateWebsiteSettings(ReferenceID, fields)
        Dim Msg As String = ""
        Dim WssFont, WssCurrency, WssLanguage, WssDateFormat, WssTimeFormat As String
        WssFont = ""
        WssCurrency = ""
        WssLanguage = ""
        WssDateFormat = ""
        WssTimeFormat = ""

        Dim jss As New JavaScriptSerializer()
        Dim field() As NameValue = jss.Deserialize(Of NameValue())(fields)
        For I = 0 To field.Length - 1
            Select Case field(I).name()
                Case "drpFont"
                    WssFont = field(I).value()
                Case "drpCurrency"
                    WssCurrency = field(I).value()
                Case "drpLanguage"
                    WssLanguage = field(I).value()
                Case "drpDateFormat"
                    WssDateFormat = field(I).value()
                Case "drpTimeFormat"
                    WssTimeFormat = field(I).value()
            End Select
        Next

        '=> Correction

        '=> Validation

        '=> Posting
        If Msg = "" Then
            Try
                'Dim strSQL As String
                'Dim dc As New DC.Conn.DataClassLayer
                'If UserID = 0 Then UserID = HttpContext.Current.Session("UserID")
                'myds = Sql.getData("usersettings", "ugUser=" & UserID)
                'If myds.Tables(0).Rows.Count > 0 Then
                '    strSQL = "UPDATE usersettings SET ugLanguage = " & WssLanguage & ",ugCalendar=" & WssCalendar & ",ugFont=" & WssFont & ",ugDateFormat=" & WssDateFormat & ",ugTimeFormat=" & WssTimeFormat & " WHERE ugUser = " & UserID
                '    dc.ExecSQuery(strSQL)
                'Else
                '    strSQL = "INSERT INTO usersettings (ugUser, ugLanguage, ugCalendar, ugFont, ugDateFormat, ugTimeFormat, ugCurrency) VALUES (" & UserID & "," & WssLanguage & "," & WssCalendar & "," & WssFont & "," & WssDateFormat & "," & WssTimeFormat & ",1)"
                '    dc.ExecSQuery(strSQL)
                'End If
                'Register Logs

                'apply effects if current user
                'If UserID = HttpContext.Current.Session("UserID") Then
                HttpContext.Current.Session("UserLanguage") = WssLanguage
                'HttpContext.Current.Session("UserCal") = WssCalendar
                'HttpContext.Current.Session("UserFont") = WssFont
                HttpContext.Current.Session("UserDTFormat") = WssDateFormat
                HttpContext.Current.Session("UserTMFormat") = WssTimeFormat
                'End If
            Catch ex As Exception
                Msg = ex.Message
            End Try
        End If

        If Msg <> "" Then Return "Err: " & Msg Else Return ""
        Return ""
    End Function
#End Region

    <System.Web.Services.WebMethod()>
    Public Shared Sub selectColumns(ByVal Source As String, ByVal Column As Integer, ByVal State As String)
        Dim MN As New Share.Main
        MN.selectColumns(Source, Column, State)
    End Sub

    <System.Web.Services.WebMethod()>
    Public Shared Function SetStartPage(ByVal strUserName As String, ByVal strStartPage As String) As String
        Dim sh As New Share.Main
        Return sh.SetStartPage(strUserName, strStartPage)
    End Function
End Class