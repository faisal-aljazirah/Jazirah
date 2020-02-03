Imports System.Web
Imports System.Xml
Imports System.Text
Imports System.Web.Script.Serialization

Public Class Stock
    Dim dcl As New DCL.Conn.DataClassLayer
    Public byteLocalCurrency As Byte
    Public intStartupFY As Integer
    Public intYear As Integer
    Const byteDepartment As Byte = 15
    Const byteBase As Byte = 50
    Public byteCurrencyRound As Byte

    Dim strUserName As String
    Dim byteLanguage As Byte
    Dim strDateFormat, strTimeFormat As String
    Dim DataLang As String
    Dim byteWarehouse As Byte

    Dim TransferApprove, SuspendExpiredApprove, SuspendReturnedApprove As Boolean
    Dim ChangeQuantity_Cash, AddDiscount_Cash, ChangeQuantity_Insurance, AddDiscount_Insurance, AllowExtraItem_Insurance, AutoMoveRejectedToCash_Insurance, AutoMoveExtraToCash_Insurance, AskBeforeSend, AskBeforeReturn, OnePaymentForCashier, ForcePaymentOnCloseInvoice, OneQuantityPerItem, DirectCancelInvoice, PopupToPrint, TaxEnabled, DirectChangeTransfer As Boolean
    Dim SusbendMax, byteDepartment_Cash, DaysToCalculateMedicalInvoices, DaysToCalculateMedicineInvoices, OrdersLimitDays, CancelLimitDays, PrintDose, PrintInvoice As Byte
    Dim lngContact_Cash, lngSalesman_Cash, lngPatient_Cash As Long
    Dim strContact_Cash, strSalesman_Cash, strPatient_Cash, strDepartment_Cash, DosePrinter, InvoicePrinter As String

    Dim TransferLimitDays As Byte

    Dim AllowCancel As Boolean

    Private Class NameValue
        Public Property name As String
        Public Property value As String
    End Class

    Sub New()
        ' User Options
        If (HttpContext.Current.Session("UserName") Is Nothing) Or (HttpContext.Current.Session("UserName") = "") Then
            'Remove this line after complete user login and settings
            'HttpContext.Current.Session("UserName") = "SoftNet"
            Throw New Exception("Login Error")
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
        byteLanguage = HttpContext.Current.Session("UserLanguage")
        strDateFormat = HttpContext.Current.Session("UserDTFormat")
        strTimeFormat = HttpContext.Current.Session("UserTMFormat")

        loadSettings()
        LoadPermissions()

        Dim dc As New DCL.Conn.XMLData
        AllowCancel = dc.CheckExistNode(HttpContext.Current.Server.MapPath("../data/xml/privileges.xml"), "Cancel_Transfer", "User", strUserName)
    End Sub

    Dim BalanceString As String = ""
    Dim BalanceList As String()
    Dim TransferString As String = ""
    Dim TransferList As String()
    Dim SExpiredString As String = ""
    Dim SExpiredList As String()
    Dim SReturnedString As String = ""
    Dim SReturnedList As String()

    Private Sub LoadPermissions()
        Dim doc As New XmlDocument
        doc.Load(HttpContext.Current.Server.MapPath("../data/xml/permissions.xml"))
        Dim balance As XmlNodeList = doc.SelectNodes("Permissions/Warehouses/User[@ID='" & strUserName & "']/Balance")
        For Each w As XmlNode In balance
            BalanceString = BalanceString & w.InnerText & ","
        Next
        If BalanceString <> "" Then
            BalanceString = Left(BalanceString, BalanceString.Length - 1)
            BalanceList = Split(BalanceString, ",")
        Else
            'Throw New Exception("Wharehouse not assigned")
        End If
        Dim transfer As XmlNodeList = doc.SelectNodes("Permissions/Warehouses/User[@ID='" & strUserName & "']/Transfer")
        For Each w As XmlNode In transfer
            TransferString = TransferString & w.InnerText & ","
        Next
        If TransferString <> "" Then
            TransferString = Left(TransferString, TransferString.Length - 1)
            TransferList = Split(TransferString, ",")
        Else
            'Throw New Exception("Wharehouse not assigned")
        End If
        Dim suspend_expire As XmlNodeList = doc.SelectNodes("Permissions/Warehouses/User[@ID='" & strUserName & "']/Suspend_Expired")
        For Each w As XmlNode In suspend_expire
            SExpiredString = SExpiredString & w.InnerText & ","
        Next
        If SExpiredString <> "" Then
            SExpiredString = Left(SExpiredString, SExpiredString.Length - 1)
            SExpiredList = Split(SExpiredString, ",")
        Else
            'Throw New Exception("Wharehouse not assigned")
        End If
        Dim suspend_return As XmlNodeList = doc.SelectNodes("Permissions/Warehouses/User[@ID='" & strUserName & "']/Suspend_Returned")
        For Each w As XmlNode In suspend_return
            SReturnedString = SReturnedString & w.InnerText & ","
        Next
        If SReturnedString <> "" Then
            SReturnedString = Left(SReturnedString, SReturnedString.Length - 1)
            SReturnedList = Split(SReturnedString, ",")
        Else
            'Throw New Exception("Wharehouse not assigned")
        End If
    End Sub
    Private Sub loadSettings()
        Dim doc As New XmlDocument
        doc.Load(HttpContext.Current.Server.MapPath("../data/xml/settings.xml"))
        'get count
        Dim items As String = ""
        Dim application As XmlNode = doc.SelectSingleNode("Settings/Pharmacy")
        intYear = application.SelectSingleNode("intYear").InnerText
        intStartupFY = application.SelectSingleNode("intStartupFY").InnerText
        byteLocalCurrency = application.SelectSingleNode("byteLocalCurrency").InnerText
        byteCurrencyRound = application.SelectSingleNode("byteCurrencyRound").InnerText

        If application.SelectSingleNode("byteDepartment_Cash") Is Nothing Then byteDepartment_Cash = "" Else byteDepartment_Cash = application.SelectSingleNode("byteDepartment_Cash").InnerText
        lngContact_Cash = application.SelectSingleNode("lngContact_Cash").InnerText
        lngSalesman_Cash = application.SelectSingleNode("lngSalesman_Cash").InnerText
        lngPatient_Cash = application.SelectSingleNode("lngPatient_Cash").InnerText

        ChangeQuantity_Cash = application.SelectSingleNode("ChangeQuantity_Cash").InnerText
        ChangeQuantity_Insurance = application.SelectSingleNode("ChangeQuantity_Insurance").InnerText
        AddDiscount_Cash = application.SelectSingleNode("AddDiscount_Cash").InnerText
        AddDiscount_Insurance = application.SelectSingleNode("AddDiscount_Insurance").InnerText
        AllowExtraItem_Insurance = application.SelectSingleNode("AllowExtraItem_Insurance").InnerText
        AutoMoveRejectedToCash_Insurance = application.SelectSingleNode("Auto_MoveRejectedToCash_Insurance").InnerText
        AskBeforeSend = application.SelectSingleNode("AskBeforeSend").InnerText
        AskBeforeReturn = application.SelectSingleNode("AskBeforeReturn").InnerText
        OnePaymentForCashier = application.SelectSingleNode("OnePaymentForCashier").InnerText
        ForcePaymentOnCloseInvoice = application.SelectSingleNode("ForcePaymentOnCloseInvoice").InnerText
        'DirectCancelInvoic = application.SelectSingleNode("DirectCancelInvoic").InnerText
        SusbendMax = application.SelectSingleNode("SusbendMax").InnerText

        'byteInvoicesLimitDay = application.SelectSingleNode("byteInvoicesLimitDay").InnerText
        OrdersLimitDays = application.SelectSingleNode("OrdersLimitDays").InnerText
        CancelLimitDays = application.SelectSingleNode("CancelLimitDays").InnerText
        DaysToCalculateMedicalInvoices = application.SelectSingleNode("DaysToCalculateMedicalInvoices").InnerText
        DaysToCalculateMedicineInvoices = application.SelectSingleNode("DaysToCalculateMedicineInvoices").InnerText

        If application.SelectSingleNode("DirectChangeTransfer") Is Nothing Then DirectChangeTransfer = False Else DirectChangeTransfer = application.SelectSingleNode("DirectChangeTransfer").InnerText

        If application.SelectSingleNode("PopupToPrint") Is Nothing Then PopupToPrint = True Else PopupToPrint = application.SelectSingleNode("PopupToPrint").InnerText
        If application.SelectSingleNode("PrintDose") Is Nothing Then PrintDose = 3 Else PrintDose = application.SelectSingleNode("PrintDose").InnerText
        If application.SelectSingleNode("PrintInvoice") Is Nothing Then PrintInvoice = 2 Else PrintInvoice = application.SelectSingleNode("PrintInvoice").InnerText
        If application.SelectSingleNode("DosePrinter") Is Nothing Then DosePrinter = "ZDesigner GK420t" Else DosePrinter = application.SelectSingleNode("DosePrinter").InnerText
        If application.SelectSingleNode("InvoicePrinter") Is Nothing Then InvoicePrinter = "HP LaserJet Professional P1566" Else InvoicePrinter = application.SelectSingleNode("InvoicePrinter").InnerText
        If application.SelectSingleNode("TaxEnabled") Is Nothing Then TaxEnabled = True Else TaxEnabled = application.SelectSingleNode("TaxEnabled").InnerText

        If application.SelectSingleNode("TransferLimitDays") Is Nothing Then TransferLimitDays = 4 Else TransferLimitDays = application.SelectSingleNode("TransferLimitDays").InnerText
        If application.SelectSingleNode("TransferApprove") Is Nothing Then TransferApprove = True Else TransferApprove = application.SelectSingleNode("TransferApprove").InnerText
        If application.SelectSingleNode("SuspendExpiredApprove") Is Nothing Then SuspendExpiredApprove = True Else SuspendExpiredApprove = application.SelectSingleNode("SuspendExpiredApprove").InnerText
        If application.SelectSingleNode("SuspendReturnedApprove") Is Nothing Then SuspendReturnedApprove = True Else SuspendReturnedApprove = application.SelectSingleNode("SuspendReturnedApprove").InnerText

        If byteLanguage = 2 Then DataLang = "Ar" Else DataLang = "En"
        Dim ds As DataSet
        ds = dcl.GetDS("SELECT * FROM Hw_Contacts WHERE lngContact = " & lngContact_Cash & "; SELECT * FROM Hw_Contacts WHERE lngContact = " & lngSalesman_Cash & "; SELECT RTRIM(LTRIM(ISNULL(strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(strLast" & DataLang & ",''))) AS PatientName, * FROM Hw_Patients WHERE lngPatient = " & lngPatient_Cash & "; SELECT * FROM Hw_Departments WHERE byteDepartment = " & byteDepartment_Cash)
        If ds.Tables(0).Rows.Count > 0 Then strContact_Cash = ds.Tables(0).Rows(0).Item("strContact" & DataLang).ToString Else strContact_Cash = ""
        If ds.Tables(1).Rows.Count > 0 Then strSalesman_Cash = ds.Tables(1).Rows(0).Item("strContact" & DataLang).ToString Else strSalesman_Cash = ""
        If ds.Tables(2).Rows.Count > 0 Then strPatient_Cash = ds.Tables(2).Rows(0).Item("PatientName").ToString Else strPatient_Cash = ""
        If ds.Tables(3).Rows.Count > 0 Then strDepartment_Cash = ds.Tables(3).Rows(0).Item("strDepartment" & DataLang).ToString Else strDepartment_Cash = ""

        'byteWarehouse = 3
        Dim dsWarehouse As DataSet
        dsWarehouse = dcl.GetDS("SELECT * FROM Cmn_Users WHERE strUserName='" & strUserName & "'")
        If dsWarehouse.Tables(0).Rows.Count > 0 Then
            If IsDBNull(dsWarehouse.Tables(0).Rows(0).Item("byteWarehouse")) Then
                Throw New Exception("Wharehouse not assigned")
            Else
                byteWarehouse = dsWarehouse.Tables(0).Rows(0).Item("byteWarehouse")
            End If
        Else
            'Throw New Exception("Wharehouse not assigned")
        End If
        'byteWarehouse = 3
    End Sub

#Region "Balance"

    Public Function getBalance(ByVal strItem As String, ByVal dateTransaction As Date, ByVal byteWarehouse As Byte, Optional ByVal Factor As Integer = 1, Optional ByVal Quantity As Integer = 0) As String
        Dim colItemNo, colItemName, colBalance As String
        Dim table As New StringBuilder("")
        Dim Where As String = ""
        Dim Having As String = ""

        Select Case byteLanguage
            Case 2
                DataLang = "Ar"
                'Variables

                'Columns
                colItemNo = "رقم الصنف"
                colItemName = "اسم الصنف"
                colBalance = "الرصيد"
            Case Else
                DataLang = "En"
                'Variables

                'Columns
                colItemNo = "Item No"
                colItemName = "Item Name"
                colBalance = "Balance"
        End Select
        Where = " AND ST.dateTransaction <= '" & dateTransaction.ToString("yyyy-MM-dd") & "'"
        If strItem <> "" Then Where = Where & " AND (SXI.strItem = '" & strItem & "' OR SI.strItem" & DataLang & " LIKE '%" & strItem & "%')"
        Select Case Factor
            Case 0
                Having = "HAVING SUM(SB.intSign * SXI.curQuantity * SU.curFactor)/1 = " & Quantity
            Case 1
                Having = "HAVING SUM(SB.intSign * SXI.curQuantity * SU.curFactor)/1 >= " & Quantity
            Case 2
                Having = "HAVING SUM(SB.intSign * SXI.curQuantity * SU.curFactor)/1 > " & Quantity
            Case 3
                Having = "HAVING SUM(SB.intSign * SXI.curQuantity * SU.curFactor)/1 <= " & Quantity
            Case 4
                Having = "HAVING SUM(SB.intSign * SXI.curQuantity * SU.curFactor)/1 < " & Quantity
            Case 5
                Having = "HAVING SUM(SB.intSign * SXI.curQuantity * SU.curFactor)/1 <> " & Quantity
        End Select

        If byteWarehouse <> 0 Then
            Where = Where & " AND SXI.byteWarehouse = " & byteWarehouse
        End If

        table.Append("<table class=""table tableAjax table-hover table-bordered mb-0"">")
        table.Append("<thead><tr><th>" & colItemNo & "</th><th>" & colItemName & "</th><th>" & colBalance & "</th></tr></thead>")
        Try
            Dim ds As DataSet
            ds = dcl.GetDS("SELECT SXI.strItem, SI.strItemEn, SI.strItemAr, SUM(SB.intSign * SXI.curQuantity * SU.curFactor)/1 AS curBalance FROM Stock_Base AS SB INNER JOIN Stock_Trans AS ST ON SB.byteBase = ST.byteBase INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink INNER JOIN Stock_Units AS SU ON SU.byteUnit = SXI.byteUnit INNER JOIN Stock_Items AS SI ON SI.strItem = SXI.stritem WHERE ST.byteStatus > 0 And SB.bInclude <> 0 And Year(dateTransaction) = " & intYear & " " & Where & " GROUP BY SXI.stritem, SI.strItemAr, SI.strItemEn " & Having)
            For I = 0 To ds.Tables(0).Rows.Count - 1
                table.Append("<tr class=""cursor-pointer"" id=""row" & ds.Tables(0).Rows(I).Item("strItem") & """ onclick=""javascript:showModal('viewBalance','{strItem: \'" & ds.Tables(0).Rows(I).Item("strItem") & "\'}','#mdlAlpha');"">")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("strItem") & "</td>")
                table.Append("<td class=""text-bold-900"">" & ds.Tables(0).Rows(I).Item("strItem" & DataLang) & "</td>")
                table.Append("<td>" & Math.Round(ds.Tables(0).Rows(I).Item("curBalance")) & "</td>")
                table.Append("</tr>")
            Next
        Catch ex As Exception
            Return "Err: No Updates"
        End Try
        table.Append("</tbody></table>")
        table.Append("<script>$('table.tableAjax').dataTable({language: tableLanguage});</script>")

        Return table.ToString
    End Function

    Public Function viewBalance(ByVal strItem As String) As String
        Dim ds As DataSet
        Dim DataLang As String
        Dim btnClose As String
        Dim tabBalance, tabTransactions, tabTracking, tabStatistic As String
        Dim lblTotal, lblTotalBalance As String
        Select Case byteLanguage
            Case 2
                DataLang = "Ar"
                tabBalance = "الرصيد"
                tabTransactions = "العمليات"
                tabTracking = "التتبع"
                tabStatistic = "إحصائيات"
                lblTotal = "المجموع"
                lblTotalBalance = "إجمالي الرصيد"
                btnClose = "إغلاق"
            Case Else
                DataLang = "En"
                tabBalance = "Balance"
                tabTransactions = "Transactions"
                tabTracking = "Tracking"
                tabStatistic = "Statistic"
                lblTotal = "Total"
                lblTotalBalance = "Total Balance"
                btnClose = "Close"
        End Select

        Dim Content As New StringBuilder("")
        Content.Append("<div class=""font-small-3 height-400 overflow-auto"">")
        If strItem <> "" Then
            'tabs
            Content.Append("<ul class=""nav nav-tabs m-0"">")
            Content.Append("<li class=""nav-item""><a class=""nav-link active"" id=""base-tab1"" data-toggle=""tab"" aria-controls=""tab1"" href=""#tab1"" aria-expanded=""true"" >" & tabBalance & "</a></li>")
            Content.Append("<li class=""nav-item""><a class=""nav-link"" id=""base-tab2"" data-toggle=""tab"" aria-controls=""tab2"" href=""#tab2"" aria-expanded=""true"" >" & tabTransactions & "</a></li>")
            Content.Append("<li class=""nav-item""><a class=""nav-link"" id=""base-tab3"" data-toggle=""tab"" aria-controls=""tab3"" href=""#tab3"" aria-expanded=""true"" >" & tabTracking & "</a></li>")
            'Content.Append("<li class=""nav-item""><a class=""nav-link"" id=""base-tab4"" data-toggle=""tab"" aria-controls=""tab4"" href=""#tab4"" aria-expanded=""true"" >" & tabStatistic & "</a></li>")
            Content.Append("</ul>")
            'contents = Start
            Content.Append("<div class=""tab-content m-0 p-1"">")
            'content 1
            Content.Append("<div role=""tabpanel"" class=""tab-pane active"" id=""tab1"" aria-expanded=""true"" aria-labelledby=""base-tab1"">")
            Content.Append(viewBalanceDetails(strItem))
            Content.Append("</div>")
            'content 2
            Content.Append("<div role=""tabpanel"" class=""tab-pane"" id=""tab2"" aria-expanded=""true"" aria-labelledby=""base-tab2"">")
            Content.Append(viewTransactionDetails(strItem))
            Content.Append("</div>")
            'content 3
            Content.Append("<div role=""tabpanel"" class=""tab-pane"" id=""tab3"" aria-expanded=""true"" aria-labelledby=""base-tab3"">")
            Content.Append(viewTrankingDetails(strItem))
            Content.Append("</div>")


            'contents = End
            Content.Append("</div>")
            'Dim Total As Integer = 0
            'Dim CurrentWarehouse As Integer = 0
            'Dim FirstLoop As Boolean = True
            'ds = dcl.GetDS(strSQL)
            'If ds.Tables(0).Rows.Count > 0 Then
            '    Content.Append("<h5><b>Item Balance:</b></h5>")
            '    Content.Append("<table class=""table table-bordered"">")
            '    Content.Append("<tr><th>Warehouse</th><th>Expire Date</th><th>Balance</th></tr>")
            '    For I = 0 To ds.Tables(0).Rows.Count - 1
            '        If CurrentWarehouse <> ds.Tables(0).Rows(I).Item("byteWarehouse") Then
            '            If FirstLoop = False Then
            '                Content.Append("<tr><th></th><th></th><th>0</th></tr>")
            '                Content.Append("</table>")
            '                Content.Append("<table class=""table table-bordered"">")
            '                Content.Append("<tr><th>Warehouse</th><th>Expire Date</th><th>Balance</th></tr>")
            '            End If
            '            FirstLoop = False
            '            CurrentWarehouse = ds.Tables(0).Rows(I).Item("byteWarehouse")
            '        End If
            '        Content.Append("<tr><td>" & ds.Tables(0).Rows(I).Item("strWarehouse" & DataLang) & "</td><td>" & CDate(ds.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM") & "</td><td><b>" & Math.Round(ds.Tables(0).Rows(I).Item("curBalance"), 0, MidpointRounding.AwayFromZero) & "</b></td></tr>")
            '    Next
            '    Content.Append("<tr><th></th><th></th><th>0</th></tr>")
            '    Content.Append("</table>")

            '    'Dim strReference As String = ds.Tables(0).Rows(0).Item("strReference").ToString
            '    'Dim lngPatient As Long = ds.Tables(0).Rows(0).Item("lngPatient")
            '    'Dim lngSalesman As Long = ds.Tables(0).Rows(0).Item("lngSalesman")
            '    'Dim dateTransaction As Date = ds.Tables(0).Rows(0).Item("dateTransaction")
            '    'Dim CloseDate As Date
            '    'If IsDBNull(ds.Tables(0).Rows(0).Item("CloseDate")) Then CloseDate = Today Else CloseDate = ds.Tables(0).Rows(0).Item("CloseDate")
            '    'Dim intVisit As Integer = 0
            '    'Dim byteWarehouse As Byte = 3
            '    'Dim lstItems As String = ""

            '    'Content.Append("<h5><b>Stock_Trans:</b></h5>")
            '    'Content.Append("<table class=""full-width border"">")
            '    'Content.Append("<tr><td>lngTransaction:</td><td class=""blue-grey""><b>" & ds.Tables(0).Rows(0).Item("lngTransaction") & "</b></td><td>byteBase:</td><td class=""blue-grey"">" & ds.Tables(0).Rows(0).Item("byteBase") & "</td><td>byteTransType:</td><td class=""blue-grey"">" & ds.Tables(0).Rows(0).Item("byteTransType") & "</td></tr>")
            '    'Content.Append("<tr><td>dateTransaction:</td><td class=""blue-grey""><b>" & CDate(ds.Tables(0).Rows(0).Item("dateTransaction")).ToString("yyyy-MM-dd") & "</b></td><td>byteStatus:</td><td class=""blue-grey"">" & ds.Tables(0).Rows(0).Item("byteStatus") & "</td><td>strReference:</td><td class=""blue-grey""><b>" & ds.Tables(0).Rows(0).Item("strReference") & "</b></td></tr>")
            '    'Content.Append("<tr><td>bCollected1:</td><td class=""blue-grey"">" & ds.Tables(0).Rows(0).Item("bCollected1") & "</td><td>bApproved1:</td><td class=""blue-grey"">" & ds.Tables(0).Rows(0).Item("bApproved1") & "</td><td>bClinicSent:</td><td class=""blue-grey"">" & ds.Tables(0).Rows(0).Item("bClinicSent") & "</td></tr>")
            '    'Content.Append("</table>")
            '    'Content.Append("<h5><b>Hw_Patients:</b></h5>")
            '    'Content.Append("<table class=""full-width border"">")
            '    'Content.Append("<tr><td>lngPatient:</td><td class=""blue-grey""><b>" & ds.Tables(0).Rows(0).Item("lngPatient") & "</b></td><td>strFirst" & DataLang & ":</td><td class=""blue-grey"">" & ds.Tables(0).Rows(0).Item("strFirst" & DataLang) & "</td><td>lngGuarantor:</td><td class=""blue-grey"">" & ds.Tables(0).Rows(0).Item("lngGuarantor") & "</td></tr>")
            '    'Content.Append("</table>")
            '    'Content.Append("<h5><b>Hw_Contacts: (Doctor)</b></h5>")
            '    'Content.Append("<table class=""full-width border"">")
            '    'Content.Append("<tr><td>lngContact:</td><td class=""blue-grey""><b>" & ds.Tables(0).Rows(0).Item("DoctorNo") & "</b></td><td>strContact" & DataLang & ":</td><td class=""blue-grey"">" & ds.Tables(0).Rows(0).Item("DoctorName") & "</td><td>intSpeciality:</td><td class=""blue-grey"">" & ds.Tables(0).Rows(0).Item("intSpeciality") & "</td></tr>")
            '    'Content.Append("</table>")
            '    'Content.Append("<h5><b>Hw_Contacts: (Company)</b></h5>")
            '    'Content.Append("<table class=""full-width border"">")
            '    'Content.Append("<tr><td>lngContact:</td><td class=""blue-grey""><b>" & ds.Tables(0).Rows(0).Item("CompanyNo") & "</b></td><td>strContact" & DataLang & ":</td><td class=""blue-grey"">" & ds.Tables(0).Rows(0).Item("CompanyName") & "</td><td>bytePriceType:</td><td class=""blue-grey"">" & ds.Tables(0).Rows(0).Item("bytePriceType") & "</td></tr>")
            '    'Content.Append("</table>")
            'Else
            '    Content.Append("Item No:" & strItem & " not available!")
            'End If
        Else
            Content.Append("Item No is wrong!")
        End If
        Content.Append("</div>")

        Dim mdl As New Share.UI
        Return mdl.drawModal("Balance Info", Content.ToString, "<button type=""button"" class=""btn btn-secondary"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnClose & "</button>", Share.UI.ModalSize.Medium, "", "p-0")
    End Function

    Private Function viewTrankingDetails(ByVal strItem As String) As String
        Dim DataLang As String
        Dim lblWait As String
        Select Case byteLanguage
            Case 2
                DataLang = "Ar"
                lblWait = "انتظر"
                'btnClose = "إغلاق"
            Case Else
                DataLang = "En"
                lblWait = "Wait"
                'btnClose = "Close"
        End Select

        Dim Content As New StringBuilder("")

        Dim listWarehouse As String = "<select class=""form-control form-control-sm input-sm"" id=""drpTrackingWarehouse"" onchange=""javascript:fillTracking();"">"
        Dim dsWarehouse As DataSet
        If BalanceString <> "*" Then dsWarehouse = dcl.GetDS("SELECT * FROM Stock_Warehouses WHERE byteWarehouse IN (" & BalanceString & ")") Else dsWarehouse = dcl.GetDS("SELECT * FROM Stock_Warehouses WHERE bActive=1")
        For W = 0 To dsWarehouse.Tables(0).Rows.Count - 1
            listWarehouse = listWarehouse & "<option value=""" & dsWarehouse.Tables(0).Rows(W).Item("byteWarehouse") & """>" & dsWarehouse.Tables(0).Rows(W).Item("strWarehouse" & DataLang).ToString & "</option>"
        Next
        listWarehouse = listWarehouse & "</select>"

        Dim listExpiry As String = "<select class=""form-control form-control-sm input-sm"" id=""drpTrackingExpiry"" onchange=""javascript:fillTracking();"">"
        Dim dsExpiry As DataSet
        dsExpiry = dcl.GetDS("SELECT XI.dateExpiry FROM Stock_Base AS B INNER JOIN Stock_Trans AS T ON B.byteBase = T.byteBase INNER JOIN Stock_Xlink AS X ON T.lngTransaction = X.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink INNER JOIN Stock_Units AS U ON U.byteUnit = XI.byteUnit INNER JOIN Stock_Warehouses AS W ON XI.byteWarehouse=W.byteWarehouse WHERE XI.strItem='" & strItem & "' AND YEAR(T.dateTransaction) = " & intYear & " AND T.byteStatus > 0 And B.bInclude <> 0 GROUP BY XI.dateExpiry ORDER BY XI.dateExpiry")
        For E = 0 To dsExpiry.Tables(0).Rows.Count - 1
            listExpiry = listExpiry & "<option value=""" & CDate(dsExpiry.Tables(0).Rows(E).Item("dateExpiry")).ToString("yyyy-MM-dd") & """>" & CDate(dsExpiry.Tables(0).Rows(E).Item("dateExpiry")).ToString("yyyy-MM") & "</option>"
        Next
        listExpiry = listExpiry & "</select>"

        Content.Append("<div class=""col-md-12"">")
        Content.Append("<div class=""col-md-6"">" & listWarehouse & "</div>")
        Content.Append("<div class=""col-md-6"">" & listExpiry & "</div>")
        Content.Append("</div>")
        Content.Append("<div class=""col-md-12 p-1"" id=""tblTracking"">")
        Content.Append("</div>")

        Content.Append("<script>function fillTracking() {$('#tblTracking').html('<div class=""bg-grey text-md-center text-bold-300 bg-lighten-4 pt-2 pb-2""><img src=""../app-assets/images/icons/spinner.gif"" /> " & lblWait & "</div>');$.ajax({type:'POST',url:'ajax.aspx/fillTracking',data:'{strItem: """ & strItem & """, byteWarehouse: ' + $('#drpTrackingWarehouse').val() + ', dateExpiry: ""' + $('#drpTrackingExpiry').val() + '""}',contentType:'application/json; charset=utf-8',dataType:'json',success: function (response) {if (response.d.substr(0, 4) == 'Err:') {msg('', response.d.substr(4, response.d.length), 'error');} else {$('#tblTracking').html(response.d);}},failure: function (msg) {alert(msg);},error: function (xhr, ajaxOptions, thrownError) {alert('Load Form, update form error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});}; fillTracking();</script>")

        Return Content.ToString
    End Function

    Private Function viewTransactionDetails(ByVal strItem As String) As String
        Dim DataLang As String
        Dim lblWait As String
        Select Case byteLanguage
            Case 2
                DataLang = "Ar"
                lblWait = "انتظر"
                'btnClose = "إغلاق"
            Case Else
                DataLang = "En"
                lblWait = "Wait"
                'btnClose = "Close"
        End Select
        Dim Content As New StringBuilder("")

        Dim listWarehouse As String = "<select class=""form-control form-control-sm input-sm"" id=""drpWarehouse"" onchange=""javascript:fillTransactions();"">"
        Dim dsWarehouse As DataSet
        If BalanceString <> "*" Then dsWarehouse = dcl.GetDS("SELECT * FROM Stock_Warehouses WHERE byteWarehouse IN (" & BalanceString & ")") Else dsWarehouse = dcl.GetDS("SELECT * FROM Stock_Warehouses WHERE bActive=1")
        For W = 0 To dsWarehouse.Tables(0).Rows.Count - 1
            listWarehouse = listWarehouse & "<option value=""" & dsWarehouse.Tables(0).Rows(W).Item("byteWarehouse") & """>" & dsWarehouse.Tables(0).Rows(W).Item("strWarehouse" & DataLang).ToString & "</option>"
        Next
        listWarehouse = listWarehouse & "</select>"

        Dim listExpiry As String = "<select class=""form-control form-control-sm input-sm"" id=""drpExpiry"" onchange=""javascript:fillTransactions();"">"
        Dim dsExpiry As DataSet
        dsExpiry = dcl.GetDS("SELECT XI.dateExpiry FROM Stock_Base AS B INNER JOIN Stock_Trans AS T ON B.byteBase = T.byteBase INNER JOIN Stock_Xlink AS X ON T.lngTransaction = X.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink INNER JOIN Stock_Units AS U ON U.byteUnit = XI.byteUnit INNER JOIN Stock_Warehouses AS W ON XI.byteWarehouse=W.byteWarehouse WHERE XI.strItem='" & strItem & "' AND YEAR(T.dateTransaction) = " & intYear & " AND T.byteStatus > 0 And B.bInclude <> 0 GROUP BY XI.dateExpiry ORDER BY XI.dateExpiry")
        For E = 0 To dsExpiry.Tables(0).Rows.Count - 1
            listExpiry = listExpiry & "<option value=""" & CDate(dsExpiry.Tables(0).Rows(E).Item("dateExpiry")).ToString("yyyy-MM-dd") & """>" & CDate(dsExpiry.Tables(0).Rows(E).Item("dateExpiry")).ToString("yyyy-MM") & "</option>"
        Next
        listExpiry = listExpiry & "</select>"

        Content.Append("<div class=""col-md-12"">")
        Content.Append("<div class=""col-md-6"">" & listWarehouse & "</div>")
        Content.Append("<div class=""col-md-6"">" & listExpiry & "</div>")
        Content.Append("</div>")
        Content.Append("<div class=""col-md-12"" id=""tblTransactions"">")
        Content.Append("</div>")

        Content.Append("<script>function fillTransactions() {$('#tblTransactions').html('<div class=""bg-grey text-md-center text-bold-300 bg-lighten-4 pt-2 pb-2""><img src=""../app-assets/images/icons/spinner.gif"" /> " & lblWait & "</div>');$.ajax({type:'POST',url:'ajax.aspx/fillTransactions',data:'{strItem: """ & strItem & """, byteWarehouse: ' + $('#drpWarehouse').val() + ', dateExpiry: ""' + $('#drpExpiry').val() + '""}',contentType:'application/json; charset=utf-8',dataType:'json',success: function (response) {if (response.d.substr(0, 4) == 'Err:') {msg('', response.d.substr(4, response.d.length), 'error');} else {$('#tblTransactions').html(response.d);}},failure: function (msg) {alert(msg);},error: function (xhr, ajaxOptions, thrownError) {alert('Load Form, update form error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});}; fillTransactions();</script>")

        Return Content.ToString
    End Function

    Public Function fillTracking(ByVal strItem As String, ByVal byteWarehouse As Byte, ByVal dateExpiry As Date) As String
        Dim DataLang As String
        Dim Direction As String
        Select Case byteLanguage
            Case 2
                DataLang = "Ar"
                Direction = "left"
                'btnClose = "إغلاق"
            Case Else
                DataLang = "En"
                Direction = "right"
                'btnClose = "Close"
        End Select
        Dim Content As New StringBuilder("")
        Dim sql As String = "SELECT T.lngTransaction, T.dateTransaction, T.strTransaction, B.intSign, B.byteBase, B.strBase" & DataLang & ", XI.byteWarehouse, CONVERT(varchar(10), XI.dateExpiry, 120) AS dateExpiry, XI.curQuantity FROM Stock_Base AS B INNER JOIN Stock_Trans AS T ON B.byteBase = T.byteBase INNER JOIN Stock_Xlink AS X ON T.lngTransaction = X.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink INNER JOIN Stock_Units AS U ON U.byteUnit = XI.byteUnit INNER JOIN Stock_Warehouses AS W ON XI.byteWarehouse=W.byteWarehouse WHERE T.byteStatus > 0 And B.bInclude <> 0 And Year(T.dateTransaction) = " & intYear & " AND W.bActive = 1 AND XI.strItem='" & strItem & "' AND T.dateTransaction <= '" & Today.ToString("yyyy-MM-dd") & "' AND XI.byteWarehouse = " & byteWarehouse & " AND XI.dateExpiry = '" & dateExpiry.ToString("yyyy-MM-dd") & "' ORDER BY XI.byteWarehouse,T.dateTransaction"
        Dim ds As DataSet = dcl.GetDS(sql)
        Dim Total As Integer = 0
        Dim TotalString As String
        Dim CurrentDate As Date = Today
        Content.Append("<table class=""text-md-center full-width no-border-top no-border-button border-left border-right"">")
        'Content.Append("<tr><th>Transaction</th><th>Amount</th></tr>")
        For I = 0 To ds.Tables(0).Rows.Count - 1
            If CurrentDate <> CDate(ds.Tables(0).Rows(I).Item("dateTransaction")) Then
                Content.Append("<tr><td style=""width:7%"" class=""border-right bg-cyan bg-lighten-4""></td><td style=""width:35%""></td><td colspan=""3""><span class=""tag tag-sm tag-success""><i class=""icon-calendar""></i> " & CDate(ds.Tables(0).Rows(I).Item("dateTransaction")).ToString("yyyy-MM-dd") & "</span></td><td style=""width:35%""></td><td style=""width:7%"" class=""border-left bg-cyan bg-lighten-4""></td></tr>")
                CurrentDate = CDate(ds.Tables(0).Rows(I).Item("dateTransaction"))
            End If
            'Get total
            Total = Total + (ds.Tables(0).Rows(I).Item("intSign") * ds.Tables(0).Rows(I).Item("curQuantity"))
            'coloring total
            If Total < 0 Then
                TotalString = "<span class=""red text-bold-500"">" & Math.Round(Total, 0, MidpointRounding.AwayFromZero) & "</span>"
            Else
                If Total = 0 Then
                    TotalString = "<b>" & Math.Round(Total, 0, MidpointRounding.AwayFromZero) & "</b>"
                Else
                    TotalString = Math.Round(Total, 0, MidpointRounding.AwayFromZero)
                End If
            End If

            If ds.Tables(0).Rows(I).Item("intSign") > 0 Then
                Content.Append("<tr><td class=""border-right bg-cyan bg-lighten-4"">" & ds.Tables(0).Rows(I).Item("strTransaction") & "</td><td class=""text-md-right text-primary"">" & ds.Tables(0).Rows(I).Item("strBase" & DataLang) & "</td><td style=""width:5%""><i class=""icon-long-arrow-" & Direction & " text-primary""></i></td><td><span class=""tag tag-pill tag-primary"">" & Math.Round(ds.Tables(0).Rows(I).Item("curQuantity"), 0, MidpointRounding.AwayFromZero) & "</span></td><td style=""width:5%""></td><td></td><td class=""border-left bg-cyan bg-lighten-4"">" & TotalString & "</td></tr>")
            Else
                Content.Append("<tr><td class=""border-right bg-cyan bg-lighten-4"">" & ds.Tables(0).Rows(I).Item("strTransaction") & "</td><td></td><td style=""width:5%""></td><td><span class=""tag tag-pill tag-info"">" & Math.Round(ds.Tables(0).Rows(I).Item("curQuantity"), 0, MidpointRounding.AwayFromZero) & "</span></td><td style=""width:5%""><i class=""icon-long-arrow-" & Direction & " text-info""></i></td><td class=""text-md-left text-info"">" & ds.Tables(0).Rows(I).Item("strBase" & DataLang) & "</td><td class=""border-left bg-cyan bg-lighten-4"">" & TotalString & "</td></tr>")
            End If
            'Content.Append("<tr><td>" & ds.Tables(0).Rows(I).Item("strBaseEn") & "</td><td>" & Math.Round(ds.Tables(0).Rows(I).Item("curQuantity"), 0, MidpointRounding.AwayFromZero) & "</td></tr>")
            'Total = Total + ds.Tables(0).Rows(I).Item("curQuantity")
        Next
        'Content.Append("<tr><th>Total</th><th>" & Math.Round(Total, 0, MidpointRounding.AwayFromZero) & "</th></tr>")
        Content.Append("</table>")
        'Content.Append("<script>$('table.tableAjax2').dataTable({language: tableLanguage});</script>")

        Return Content.ToString
    End Function

    Public Function fillTransactions(ByVal strItem As String, ByVal byteWarehouse As Byte, ByVal dateExpiry As Date) As String
        Dim DataLang As String
        Dim lblTransaction, lblAmount, lblTotal As String
        Select Case byteLanguage
            Case 2
                DataLang = "Ar"
                lblTransaction = "العملية"
                lblAmount = "الكمية"
                lblTotal = "المجموع"
                'btnClose = "إغلاق"
            Case Else
                DataLang = "En"
                lblTransaction = "Transaction"
                lblAmount = "Amount"
                lblTotal = "Total"
                'btnClose = "Close"
        End Select
        Dim Content As New StringBuilder("")
        Dim ds As DataSet = dcl.GetDS("SELECT B.byteBase, B.strBase" & DataLang & ", XI.byteWarehouse, CONVERT(varchar(10), XI.dateExpiry, 120) AS dateExpiry, SUM(B.intSign * XI.curQuantity * U.curFactor) AS curBalance FROM Stock_Base AS B INNER JOIN Stock_Trans AS T ON B.byteBase = T.byteBase INNER JOIN Stock_Xlink AS X ON T.lngTransaction = X.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink INNER JOIN Stock_Units AS U ON U.byteUnit = XI.byteUnit INNER JOIN Stock_Warehouses AS W ON XI.byteWarehouse=W.byteWarehouse WHERE T.byteStatus > 0 And B.bInclude <> 0 And Year(T.dateTransaction) = 2020 AND W.bActive = 1 AND XI.strItem='" & strItem & "' AND T.dateTransaction <= '" & Today.ToString("yyyy-MM-dd") & "' AND XI.byteWarehouse = " & byteWarehouse & " AND XI.dateExpiry = '" & dateExpiry.ToString("yyyy-MM-dd") & "' GROUP BY B.byteBase, B.strBase" & DataLang & ", XI.byteWarehouse, W.strWarehouseEn, XI.dateExpiry HAVING SUM(B.intSign * XI.curQuantity * U.curFactor) <> 0 ORDER BY XI.byteWarehouse")
        Dim Total As Integer = 0
        Content.Append("<table class=""table tableAjax2 table-bordered mt-1"">")
        Content.Append("<tr><th>" & lblTransaction & "</th><th>" & lblAmount & "</th></tr>")
        For I = 0 To ds.Tables(0).Rows.Count - 1
            Content.Append("<tr><td>" & ds.Tables(0).Rows(I).Item("strBase" & DataLang) & "</td><td>" & Math.Round(ds.Tables(0).Rows(I).Item("curBalance"), 0, MidpointRounding.AwayFromZero) & "</td></tr>")
            Total = Total + ds.Tables(0).Rows(I).Item("curBalance")
        Next
        Content.Append("<tr><th>" & lblTotal & "</th><th>" & Math.Round(Total, 0, MidpointRounding.AwayFromZero) & "</th></tr>")
        Content.Append("</table>")
        'Content.Append("<script>$('table.tableAjax2').dataTable({language: tableLanguage});</script>")

        Return Content.ToString
    End Function

    Private Function viewBalanceDetails(ByVal strItem As String) As String
        Dim DataLang As String
        Dim lblTotal, lblTotalBalance As String
        Select Case byteLanguage
            Case 2
                DataLang = "Ar"
                lblTotal = "المجموع"
                lblTotalBalance = "إجمالي الرصيد"
                'btnClose = "إغلاق"
            Case Else
                DataLang = "En"
                lblTotal = "Total"
                lblTotalBalance = "Total Balance"
                'btnClose = "Close"
        End Select
        Dim Content As New StringBuilder("")
        Dim ds As DataSet
        Dim Where As String = ""
        If BalanceString <> "*" Then Where = " AND XI.byteWarehouse IN (" & BalanceString & ")"
        'Dim strSQL As String = "SELECT SUM(SB.intSign * SXI.curQuantity * SU.curFactor)/1 AS curBalance FROM Stock_Base AS SB INNER JOIN Stock_Trans AS ST ON SB.byteBase = ST.byteBase INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink INNER JOIN Stock_Units AS SU ON SU.byteUnit = SXI.byteUnit WHERE ST.byteStatus > 0 And SB.bInclude <> 0 And Year(dateTransaction) = " & intYear & " And SXI.byteWarehouse = " & byteWarehouse & " AND SXI.strItem='" & strItem & "' AND ST.dateTransaction <= '" & Today.ToString("yyyy-MM-dd") & "'"
        Dim strSQL As String = "SELECT XI.byteWarehouse, W.strWarehouse" & DataLang & ", CONVERT(varchar(10), XI.dateExpiry, 120) AS dateExpiry, SUM(B.intSign * XI.curQuantity * U.curFactor) AS curBalance FROM Stock_Base AS B INNER JOIN Stock_Trans AS T ON B.byteBase = T.byteBase INNER JOIN Stock_Xlink AS X ON T.lngTransaction = X.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink INNER JOIN Stock_Units AS U ON U.byteUnit = XI.byteUnit INNER JOIN Stock_Warehouses AS W ON XI.byteWarehouse=W.byteWarehouse WHERE T.byteStatus > 0 And B.bInclude <> 0 And Year(T.dateTransaction) = " & intYear & " AND W.bActive = 1 AND XI.strItem='" & strItem & "' AND T.dateTransaction <= '" & Today.ToString("yyyy-MM-dd") & "' " & Where & " GROUP BY XI.byteWarehouse, W.strWarehouseEn, XI.dateExpiry HAVING SUM(B.intSign * XI.curQuantity * U.curFactor) > 0 ORDER BY XI.byteWarehouse, XI.dateExpiry"
        Dim SubTotal, Total As Integer
        Dim dsWarehouse As DataSet
        If BalanceString <> "*" Then dsWarehouse = dcl.GetDS("SELECT * FROM Stock_Warehouses WHERE byteWarehouse IN (" & BalanceString & ")") Else dsWarehouse = dcl.GetDS("SELECT * FROM Stock_Warehouses WHERE bActive=1")
        Dim TableHead, TableData, Alert As String
        TableHead = ""
        TableData = ""
        Total = 0
        For W = 0 To dsWarehouse.Tables(0).Rows.Count - 1
            SubTotal = 0
            TableHead = TableHead & "<th style=""width:" & CInt(100 / dsWarehouse.Tables(0).Rows.Count) & "%; text-align:center;"">" & dsWarehouse.Tables(0).Rows(W).Item("strWarehouse" & DataLang).ToString & "</th>"
            ds = dcl.GetDS("SELECT XI.byteWarehouse, W.strWarehouse" & DataLang & ", CONVERT(varchar(10), XI.dateExpiry, 120) AS dateExpiry, SUM(B.intSign * XI.curQuantity * U.curFactor) AS curBalance FROM Stock_Base AS B INNER JOIN Stock_Trans AS T ON B.byteBase = T.byteBase INNER JOIN Stock_Xlink AS X ON T.lngTransaction = X.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink INNER JOIN Stock_Units AS U ON U.byteUnit = XI.byteUnit INNER JOIN Stock_Warehouses AS W ON XI.byteWarehouse=W.byteWarehouse WHERE T.byteStatus > 0 And B.bInclude <> 0 And Year(T.dateTransaction) = " & intYear & " AND W.bActive = 1 AND XI.strItem='" & strItem & "' AND T.dateTransaction <= '" & Today.ToString("yyyy-MM-dd") & "' AND XI.byteWarehouse = " & dsWarehouse.Tables(0).Rows(W).Item("byteWarehouse") & " GROUP BY XI.byteWarehouse, W.strWarehouse" & DataLang & ", XI.dateExpiry HAVING SUM(B.intSign * XI.curQuantity * U.curFactor) <> 0 ORDER BY XI.byteWarehouse, XI.dateExpiry")
            TableData = TableData & "<td><table class=""table table-bordered"">" '<tr><th>Expire Date</th><th>Balance</th></tr>"
            For I = 0 To ds.Tables(0).Rows.Count - 1
                If ds.Tables(0).Rows(I).Item("curBalance") < 0 Then Alert = " class=""text-danger""" Else Alert = ""
                TableData = TableData & "<tr><td>" & CDate(ds.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM") & "</td><td " & Alert & "><b>" & Math.Round(ds.Tables(0).Rows(I).Item("curBalance"), 0, MidpointRounding.AwayFromZero) & "</b></td></tr>"
                Total = Total + ds.Tables(0).Rows(I).Item("curBalance")
                SubTotal = SubTotal + ds.Tables(0).Rows(I).Item("curBalance")
            Next
            TableData = TableData & "<tr><td><b>" & lblTotal & "</b></td><td><b>" & Math.Round(SubTotal, 0, MidpointRounding.AwayFromZero) & "</b></td></tr></table></td>"
        Next
        Content.Append("<table class=""table table-bordered text-md-center"">")
        Content.Append("<tr>" & TableHead & "</tr>")
        Content.Append("<tr>" & TableData & "</tr>")
        Content.Append("<tr><td colspan=""" & dsWarehouse.Tables(0).Rows.Count & """><h4><b>" & lblTotalBalance & ": <span class=""text-success"">" & Math.Round(Total, 0, MidpointRounding.AwayFromZero) & "</span></b></h4></td></tr>")
        Content.Append("</table>")

        Return Content.ToString
    End Function
#End Region

    Public Function fillTransfer(ByVal dateFrom As Date, ByVal dateTo As Date, ByVal byteWarehouse As Byte) As String
        Dim colTransNo, colDate, colType, colWarehouseFrom, colSender, colCount, colQuantity, colStatus1, colRecipient, colWarehouseTo, colStatus2 As String
        Dim Pending, Approved, Cancelled As String
        Dim table As New StringBuilder("")
        Dim Where As String = ""
        Dim Having As String = ""

        Select Case byteLanguage
            Case 2
                DataLang = "Ar"
                'Variables
                Pending = "<span class=""tag tag-warning"">في الانتظار</span>"
                Approved = "<span class=""tag tag-success"">معمدة</span>"
                Cancelled = "<span class=""tag tag-danger"">ملغية</span>"
                'Columns
                colTransNo = "رقم السند"
                colDate = "التاريخ"
                colType = "السند"
                colWarehouseFrom = "من"
                colSender = "المرسل"
                colCount = "عدد الأصناف"
                colQuantity = "إجمالي الكمية"
                colStatus1 = "حالة الارسال"
                colWarehouseTo = "إلى"
                colRecipient = "المستلم"
                colStatus2 = "حالة الاستلام"
            Case Else
                DataLang = "En"
                'Variables
                Pending = "<span class=""tag tag-warning"">Pending</span>"
                Approved = "<span class=""tag tag-success"">Approved</span>"
                Cancelled = "<span class=""tag tag-danger"">Cancelled</span>"
                'Columns
                colTransNo = "Voucher No"
                colDate = "Date"
                colType = "Voucher"
                colWarehouseFrom = "From"
                colSender = "Sender"
                colCount = "Items Count"
                colQuantity = "Total Quantity"
                colStatus1 = "Send Status"
                colWarehouseTo = "To"
                colRecipient = "Recipient"
                colStatus2 = "Receive Status"
        End Select
        Dim status1 As String = ""
        Dim status2 As String = ""
        'Dim Searching As Boolean = False
        'Dim Period As String
        'If Searching = True Then
        'Period = " AND YEAR(T.dateTransaction) = " & intYear
        'Else
        Where = " AND CONVERT(varchar(10), T1.dateTransaction, 120) BETWEEN '" & dateFrom.ToString("yyyy-MM-dd") & "' AND '" & dateTo.ToString("yyyy-MM-dd") & "'"
        If byteWarehouse <> 0 Then
            Where = Where & " AND T1.byteWarehouse = " & byteWarehouse
        End If
        'End If
        'Where = " AND T.byteBase IN (19,20) " & Period
        'If strItem <> "" Then Where = Where & " AND (SXI.strItem = '" & strItem & "' OR SI.strItem" & DataLang & " LIKE '%" & strItem & "%')"
        'Select Case Factor
        '    Case 0
        '        Having = "HAVING SUM(SB.intSign * SXI.curQuantity * SU.curFactor)/1 = " & Quantity
        '    Case 1
        '        Having = "HAVING SUM(SB.intSign * SXI.curQuantity * SU.curFactor)/1 >= " & Quantity
        '    Case 2
        '        Having = "HAVING SUM(SB.intSign * SXI.curQuantity * SU.curFactor)/1 > " & Quantity
        '    Case 3
        '        Having = "HAVING SUM(SB.intSign * SXI.curQuantity * SU.curFactor)/1 <= " & Quantity
        '    Case 4
        '        Having = "HAVING SUM(SB.intSign * SXI.curQuantity * SU.curFactor)/1 < " & Quantity
        '    Case 5
        '        Having = "HAVING SUM(SB.intSign * SXI.curQuantity * SU.curFactor)/1 <> " & Quantity
        'End Select

        table.Append("<table class=""table tableAjax table-hover table-bordered mb-0"">")
        table.Append("<thead><tr><th>" & colTransNo & "</th><th>" & colDate & "</th><th>" & colType & "</th><th>" & colWarehouseFrom & "</th><th>" & colSender & "</th><th>" & colCount & "</th><th>" & colQuantity & "</th><th>" & colStatus1 & "</th><th>" & colWarehouseTo & "</th><th>" & colRecipient & "</th><th>" & colStatus2 & "</th></tr></thead>")
        Try
            Dim ds As DataSet
            'ds = dcl.GetDS("SELECT T1.lngTransaction, T1.strTransaction, T1.dateTransaction, TT1.strType" & DataLang & ", W1.strWarehouse" & DataLang & " AS strWarehouseFrom, W2.strWarehouse" & DataLang & " AS strWarehouseTo, TA1.strCreatedBy AS Sender, TA2.strCreatedBy AS Recipient, T1.byteStatus AS SenderStatus, T2.byteStatus AS RecipientStatus, COUNT(XI.lngXlink) AS intCount, SUM(ISNULL(XI.curQuantity, 0)) AS curQuantity FROM Stock_Trans AS T1 LEFT JOIN Stock_Trans AS T2 ON T1.strTransaction=T2.strTransaction AND T1.lngTransaction<>T2.lngTransaction INNER JOIN Stock_Warehouses AS W1 ON T1.byteWarehouse=W1.byteWarehouse INNER JOIN Stock_Warehouses AS W2 ON T2.byteWarehouse=W2.byteWarehouse INNER JOIN Stock_Trans_Types AS TT1 ON T1.byteTransType=TT1.byteTransType INNER JOIN Stock_Trans_Audit AS TA1 ON T1.lngTransaction=TA1.lngTransaction INNER JOIN Stock_Trans_Audit AS TA2 ON T2.lngTransaction=TA2.lngTransaction INNER JOIN Stock_Xlink AS X ON X.lngTransaction=T1.lngTransaction LEFT JOIN Stock_Xlink_Items AS XI ON X.lngXlink=XI.lngXlink WHERE YEAR(T1.dateTransaction) = 2019 AND YEAR(T2.dateTransaction) = 2019 AND T1.byteBase=19 AND T2.byteBase=20 " & Where & " GROUP BY T1.lngTransaction, T1.strTransaction, T1.dateTransaction, TT1.strType" & DataLang & ", W1.strWarehouse" & DataLang & ", W2.strWarehouse" & DataLang & ", TA1.strCreatedBy, TA2.strCreatedBy, T1.byteStatus, T2.byteStatus")
            ds = dcl.GetDS("SELECT T1.lngTransaction, T1.strTransaction, T1.dateTransaction, TT1.strType" & DataLang & ", W1.strWarehouse" & DataLang & " AS strWarehouseFrom, W2.strWarehouse" & DataLang & " AS strWarehouseTo, TA1.strCreatedBy AS Sender, TA2.strCreatedBy AS Recipient, T1.byteStatus AS SenderStatus, T2.byteStatus AS RecipientStatus, COUNT(XI1.lngXlink) AS intCount, SUM(ISNULL(XI1.curQuantity, 0)) AS curQuantity FROM Stock_Xlink AS X1 LEFT JOIN Stock_Xlink AS X2 ON X1.lngPointer=X2.lngPointer INNER JOIN Stock_Trans AS T1 ON T1.lngTransaction=X1.lngTransaction INNER JOIN Stock_Trans AS T2 ON T2.lngTransaction=X2.lngTransaction INNER JOIN Stock_Warehouses AS W1 ON T1.byteWarehouse=W1.byteWarehouse INNER JOIN Stock_Warehouses AS W2 ON T2.byteWarehouse=W2.byteWarehouse INNER JOIN Stock_Trans_Types AS TT1 ON T1.byteTransType=TT1.byteTransType INNER JOIN Stock_Trans_Audit AS TA1 ON T1.lngTransaction=TA1.lngTransaction LEFT JOIN Stock_Trans_Audit AS TA2 ON T2.lngTransaction=TA2.lngTransaction LEFT JOIN Stock_Xlink_Items AS XI1 ON X1.lngXlink=XI1.lngXlink WHERE T1.byteBase=19 AND T2.byteBase=20 " & Where & " GROUP BY T1.lngTransaction, T1.strTransaction, T1.dateTransaction, TT1.strType" & DataLang & ", W1.strWarehouse" & DataLang & ", W2.strWarehouse" & DataLang & ", TA1.strCreatedBy, TA2.strCreatedBy, T1.byteStatus, T2.byteStatus")
            For I = 0 To ds.Tables(0).Rows.Count - 1
                Select Case ds.Tables(0).Rows(I).Item("SenderStatus")
                    Case 0
                        status1 = Cancelled
                    Case 1
                        status1 = Pending
                    Case 2
                        status1 = Approved
                End Select
                Select Case ds.Tables(0).Rows(I).Item("RecipientStatus")
                    Case 0
                        status2 = Cancelled
                    Case 1
                        status2 = Pending
                    Case 2
                        status2 = Approved
                End Select
                table.Append("<tr class=""cursor-pointer"" id=""row" & ds.Tables(0).Rows(I).Item("lngTransaction") & """ onclick=""javascript:showModal('viewTransfer', '{lngTransaction: " & ds.Tables(0).Rows(I).Item("lngTransaction") & ", ReceiveMode: false}', '#mdlAlpha')"">")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("strTransaction") & "</td>")
                table.Append("<td>" & CDate(ds.Tables(0).Rows(I).Item("dateTransaction")).ToString(strDateFormat) & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("strType" & DataLang) & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("strWarehouseFrom") & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("Sender") & "</td>")
                table.Append("<td><b>" & Math.Round(ds.Tables(0).Rows(I).Item("intCount"), 2, MidpointRounding.AwayFromZero) & "</b></td>")
                table.Append("<td><b>" & Math.Round(ds.Tables(0).Rows(I).Item("curQuantity"), 2, MidpointRounding.AwayFromZero) & "</b></td>")
                table.Append("<td>" & status1 & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("strWarehouseTo") & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("Recipient") & "</td>")
                table.Append("<td>" & status2 & "</td>")
                table.Append("</tr>")
            Next
        Catch ex As Exception
            Return "Err:" & ex.Message
        End Try
        table.Append("</tbody></table>")
        table.Append("<script>$('table.tableAjax').dataTable({language: tableLanguage, order: [[0, 'desc']]});</script>")

        Return table.ToString
    End Function

    Public Function fillRequests(ByVal dateFrom As Date, ByVal dateTo As Date, ByVal byteWarehouse As Byte) As String
        Dim colTransNo, colDate, colType, colWarehouseFrom, colSender, colCount, colQuantity, colStatus1, colRecipient, colWarehouseTo, colStatus2 As String
        Dim Pending, Approved, Cancelled As String
        Dim table As New StringBuilder("")
        Dim Where As String = ""
        Dim Having As String = ""

        Select Case byteLanguage
            Case 2
                DataLang = "Ar"
                'Variables
                Pending = "<span class=""tag tag-warning"">في الانتظار</span>"
                Approved = "<span class=""tag tag-success"">معمدة</span>"
                Cancelled = "<span class=""tag tag-danger"">ملغية</span>"
                'Columns
                colTransNo = "رقم السند"
                colDate = "التاريخ"
                colType = "السند"
                colWarehouseFrom = "من"
                colSender = "المرسل"
                colCount = "عدد الأصناف"
                colQuantity = "إجمالي الكمية"
                colStatus1 = "حالة الارسال"
                colWarehouseTo = "إلى"
                colRecipient = "المستلم"
                colStatus2 = "حالة الاستلام"
            Case Else
                DataLang = "En"
                'Variables
                Pending = "<span class=""tag tag-warning"">Pending</span>"
                Approved = "<span class=""tag tag-success"">Approved</span>"
                Cancelled = "<span class=""tag tag-danger"">Cancelled</span>"
                'Columns
                colTransNo = "Voucher No"
                colDate = "Date"
                colType = "Voucher"
                colWarehouseFrom = "From"
                colSender = "Sender"
                colCount = "Items Count"
                colQuantity = "Total Quantity"
                colStatus1 = "Send Status"
                colWarehouseTo = "To"
                colRecipient = "Recipient"
                colStatus2 = "Receive Status"
        End Select
        Dim status1 As String = ""
        Dim status2 As String = ""

        Where = " AND CONVERT(varchar(10), T1.dateTransaction, 120) BETWEEN '" & dateFrom.ToString("yyyy-MM-dd") & "' AND '" & dateTo.ToString("yyyy-MM-dd") & "'"
        Where = Where & " AND YEAR(T1.dateTransaction) = " & intYear
        If byteWarehouse <> 0 Then
            Where = Where & " AND T2.byteWarehouse = " & byteWarehouse
        End If
        'Where = Where & " AND T2.byteWarehouse = " & byteWarehouse
        Where = Where & " AND T1.byteStatus = 2"

        table.Append("<table class=""table tableAjax table-hover table-bordered mb-0"">")
        table.Append("<thead><tr><th>" & colTransNo & "</th><th>" & colDate & "</th><th>" & colType & "</th><th>" & colWarehouseFrom & "</th><th>" & colSender & "</th><th>" & colCount & "</th><th>" & colQuantity & "</th><th>" & colStatus1 & "</th><th>" & colWarehouseTo & "</th><th>" & colRecipient & "</th><th>" & colStatus2 & "</th></tr></thead>")
        Try
            Dim ds As DataSet
            'ds = dcl.GetDS("SELECT T1.lngTransaction, T1.strTransaction, T1.dateTransaction, TT1.strType" & DataLang & ", W1.strWarehouse" & DataLang & " AS strWarehouseFrom, W2.strWarehouse" & DataLang & " AS strWarehouseTo, TA1.strCreatedBy AS Sender, TA2.strCreatedBy AS Recipient, T1.byteStatus AS SenderStatus, T2.byteStatus AS RecipientStatus, COUNT(XI.lngXlink) AS intCount, SUM(ISNULL(XI.curQuantity, 0)) AS curQuantity FROM Stock_Trans AS T1 LEFT JOIN Stock_Trans AS T2 ON T1.strTransaction=T2.strTransaction AND T1.lngTransaction<>T2.lngTransaction INNER JOIN Stock_Warehouses AS W1 ON T1.byteWarehouse=W1.byteWarehouse INNER JOIN Stock_Warehouses AS W2 ON T2.byteWarehouse=W2.byteWarehouse INNER JOIN Stock_Trans_Types AS TT1 ON T1.byteTransType=TT1.byteTransType INNER JOIN Stock_Trans_Audit AS TA1 ON T1.lngTransaction=TA1.lngTransaction INNER JOIN Stock_Trans_Audit AS TA2 ON T2.lngTransaction=TA2.lngTransaction INNER JOIN Stock_Xlink AS X ON X.lngTransaction=T1.lngTransaction LEFT JOIN Stock_Xlink_Items AS XI ON X.lngXlink=XI.lngXlink WHERE YEAR(T1.dateTransaction) = 2019 AND YEAR(T2.dateTransaction) = 2019 AND T1.byteBase=19 AND T2.byteBase=20 " & Where & " GROUP BY T1.lngTransaction, T1.strTransaction, T1.dateTransaction, TT1.strType" & DataLang & ", W1.strWarehouse" & DataLang & ", W2.strWarehouse" & DataLang & ", TA1.strCreatedBy, TA2.strCreatedBy, T1.byteStatus, T2.byteStatus")
            ds = dcl.GetDS("SELECT T1.lngTransaction, T1.strTransaction, T1.dateTransaction, TT1.strType" & DataLang & ", W1.strWarehouse" & DataLang & " AS strWarehouseFrom, W2.strWarehouse" & DataLang & " AS strWarehouseTo, TA1.strCreatedBy AS Sender, TA2.strCreatedBy AS Recipient, T1.byteStatus AS SenderStatus, T2.byteStatus AS RecipientStatus, COUNT(XI1.lngXlink) AS intCount, SUM(ISNULL(XI1.curQuantity, 0)) AS curQuantity FROM Stock_Xlink AS X1 LEFT JOIN Stock_Xlink AS X2 ON X1.lngPointer=X2.lngPointer INNER JOIN Stock_Trans AS T1 ON T1.lngTransaction=X1.lngTransaction INNER JOIN Stock_Trans AS T2 ON T2.lngTransaction=X2.lngTransaction INNER JOIN Stock_Warehouses AS W1 ON T1.byteWarehouse=W1.byteWarehouse INNER JOIN Stock_Warehouses AS W2 ON T2.byteWarehouse=W2.byteWarehouse INNER JOIN Stock_Trans_Types AS TT1 ON T1.byteTransType=TT1.byteTransType INNER JOIN Stock_Trans_Audit AS TA1 ON T1.lngTransaction=TA1.lngTransaction LEFT JOIN Stock_Trans_Audit AS TA2 ON T2.lngTransaction=TA2.lngTransaction LEFT JOIN Stock_Xlink_Items AS XI1 ON X1.lngXlink=XI1.lngXlink WHERE T1.byteBase=19 AND T2.byteBase=20 " & Where & " GROUP BY T1.lngTransaction, T1.strTransaction, T1.dateTransaction, TT1.strType" & DataLang & ", W1.strWarehouse" & DataLang & ", W2.strWarehouse" & DataLang & ", TA1.strCreatedBy, TA2.strCreatedBy, T1.byteStatus, T2.byteStatus")
            For I = 0 To ds.Tables(0).Rows.Count - 1
                Select Case ds.Tables(0).Rows(I).Item("SenderStatus")
                    Case 0
                        status1 = Cancelled
                    Case 1
                        status1 = Pending
                    Case 2
                        status1 = Approved
                End Select
                Select Case ds.Tables(0).Rows(I).Item("RecipientStatus")
                    Case 0
                        status2 = Cancelled
                    Case 1
                        status2 = Pending
                    Case 2
                        status2 = Approved
                End Select
                table.Append("<tr class=""cursor-pointer"" id=""row" & ds.Tables(0).Rows(I).Item("lngTransaction") & """ onclick=""javascript:showModal('viewTransfer', '{lngTransaction: " & ds.Tables(0).Rows(I).Item("lngTransaction") & ", ReceiveMode: true}', '#mdlAlpha')"">")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("strTransaction") & "</td>")
                table.Append("<td>" & CDate(ds.Tables(0).Rows(I).Item("dateTransaction")).ToString(strDateFormat) & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("strType" & DataLang) & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("strWarehouseFrom") & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("Sender") & "</td>")
                table.Append("<td><b>" & Math.Round(ds.Tables(0).Rows(I).Item("intCount"), 2, MidpointRounding.AwayFromZero) & "</b></td>")
                table.Append("<td><b>" & Math.Round(ds.Tables(0).Rows(I).Item("curQuantity"), 2, MidpointRounding.AwayFromZero) & "</b></td>")
                table.Append("<td>" & status1 & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("strWarehouseTo") & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("Recipient") & "</td>")
                table.Append("<td>" & status2 & "</td>")
                table.Append("</tr>")
            Next
        Catch ex As Exception
            Return "Err:" & ex.Message
        End Try
        table.Append("</tbody></table>")
        table.Append("<script>$('table.tableAjax').dataTable({language: tableLanguage, order: [[0, 'desc']]});</script>")

        Return table.ToString
    End Function

    Public Function fillItems(ByVal strItem As String, ByVal intGroup As Integer, ByVal intAvailable As Byte) As String
        Dim colItemNo, colItemName, colGroup, colTaxAmount, colTax As String
        Dim Enabled, Disabled As String
        Dim table As New StringBuilder("")
        Dim Where As String = ""
        Dim Having As String = ""

        Select Case byteLanguage
            Case 2
                DataLang = "Ar"
                'Variables
                Enabled = "مفعل"
                Disabled = "معطل"

                'Columns
                colItemNo = "رقم الصنف"
                colItemName = "اسم الصنف"
                colTaxAmount = "الضريبة"
                colTax = "تفعيل الضريبة"
                colGroup = "المجموعة"
            Case Else
                DataLang = "En"
                'Variables
                Enabled = "Enabled"
                Disabled = "Disabled"

                'Columns
                colItemNo = "Item No"
                colItemName = "Item Name"
                colTaxAmount = "Tax"
                colTax = "Tax Enable"
                colGroup = "Group"
        End Select
        Where = " strItem <> ''"
        If strItem <> "" Then Where = Where & " AND (I.strItem = '" & strItem & "' OR I.strItem" & DataLang & " LIKE '%" & strItem & "%')"
        If intGroup <> 0 Then Where = Where & " AND G.intGroup = " & intGroup
        Select Case intAvailable
            Case 1
                Where = Where & " AND I.bEnabled = 1"
            Case 2
                Where = Where & " AND (I.bEnabled = 0 OR I.bEnabled IS NULL)"
            Case Else
                'all
        End Select

        table.Append("<table class=""table tableAjax table-hover table-bordered mb-0"">")
        table.Append("<thead><tr><th>" & colItemNo & "</th><th>" & colItemName & "</th><th>" & colGroup & "</th><th>" & colTaxAmount & "</th><th>" & colTax & "</th></tr></thead>")
        Try
            Dim TaxEnabled As String
            Dim ds As DataSet
            Dim h As New Healthware.Stock.Items
            ds = dcl.GetDS("SELECT * FROM Stock_Items AS I INNER JOIN Stock_Groups AS G ON I.intGroup=G.intGroup WHERE " & Where)
            'ds = h.getItems()
            'ds = h.getItems(strItem, strItem, intGroup, True)
            For I = 0 To ds.Tables(0).Rows.Count - 1
                If IsDBNull(ds.Tables(0).Rows(I).Item("bTax")) Then
                    TaxEnabled = "<span class=""text-danger"">" & Disabled & "</span>"
                Else
                    If CBool(ds.Tables(0).Rows(I).Item("bTax")) = True Then TaxEnabled = "<span class=""text-success"">" & Enabled & "</span>" Else TaxEnabled = "<span class=""text-danger"">" & Disabled & "</span>"
                End If

                table.Append("<tr class=""cursor-pointer"" id=""row" & ds.Tables(0).Rows(I).Item("strItem") & """ onclick=""javascript:showModal('viewItem', '{strItem: " & ds.Tables(0).Rows(I).Item("strItem") & "}', '#mdlAlpha');"">")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("strItem") & "</td>")
                table.Append("<td class=""text-bold-900"">" & ds.Tables(0).Rows(I).Item("strItem" & DataLang) & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("strGroup" & DataLang) & "</td>")
                table.Append("<td>" & Math.Round(CDec("0" & ds.Tables(0).Rows(I).Item("curTax").ToString), 2, MidpointRounding.AwayFromZero) & "</td>")
                table.Append("<td>" & TaxEnabled & "</td>")
                table.Append("</tr>")
            Next
        Catch ex As Exception
            Return "Err: No Updates"
        End Try
        table.Append("</tbody></table>")
        table.Append("<script>$('table.tableAjax').dataTable({language: tableLanguage});</script>")

        Return table.ToString
    End Function

    Public Function viewItem(ByVal strItem As String) As String
        Dim tabGeneral As String
        Dim lblNo, lblNameAr, lblNameEn, lblGroup, lblTax, lblAvailability As String
        Dim Enabled, Disabled, Available, Unavailable As String
        Dim btnClose, btnSave As String
        Select Case byteLanguage
            Case 2
                DataLang = "Ar"

                tabGeneral = "عــام"

                lblNo = "رقم الصنف"
                lblNameAr = "الاسم العربي"
                lblNameEn = "الاسم الإنجليزي"
                lblGroup = "المجموعة"
                lblTax = "الضريبة / VAT"
                lblAvailability = "التوفر"

                Enabled = "مفعل"
                Disabled = "معطل"
                Available = "متوفر"
                Unavailable = "غير متوفر"

                btnSave = "حفظ التغييرات"
                btnClose = "إغلاق"
            Case Else
                DataLang = "En"

                tabGeneral = "General"

                lblNo = "Item No"
                lblNameAr = "Arabic Name"
                lblNameEn = "English Name"
                lblGroup = "Group"
                lblTax = "Tax / VAT"
                lblAvailability = "Availability"

                Enabled = "Enabled"
                Disabled = "Disabled"
                Available = "Available"
                Unavailable = "Unavailable"

                btnSave = "Save Changes"
                btnClose = "Close"
        End Select

        Dim ds As DataSet
        ds = dcl.GetDS("SELECT * FROM Stock_Items WHERE strItem='" & strItem & "'")

        Dim selected As String = ""
        Dim drpGroups As String = "<select id=""drpGroup"" name=""drpGroup"" class=""form-control form-control-sm"">"
        Dim dsGroups As DataSet = dcl.GetDS("SELECT * FROM Stock_Groups WHERE bPharmacy=1")
        For I = 0 To dsGroups.Tables(0).Rows.Count - 1
            If dsGroups.Tables(0).Rows(I).Item("intGroup") = ds.Tables(0).Rows(0).Item("intGroup") Then selected = " selected=""selected""" Else selected = ""
            drpGroups = drpGroups & "<option value=""" & dsGroups.Tables(0).Rows(I).Item("intGroup") & """ " & selected & ">" & dsGroups.Tables(0).Rows(I).Item("strGroup" & DataLang) & "</option>"
        Next
        drpGroups = drpGroups & "</select>"

        Dim bTax As Boolean
        If IsDBNull(ds.Tables(0).Rows(0).Item("bTax")) Then bTax = False Else bTax = ds.Tables(0).Rows(0).Item("bTax")
        Dim rbtnTax As String
        Dim chkTax As String
        If bTax = True Then
            rbtnTax = "<div class=""btn-group btn-group-toggle radio"" data-toggle=""buttons""><label class=""btn btn-secondary btn-sm active""><input type=""radio"" name=""rbtnTax"" id=""rbtnTrue"" value=""1"" checked /> " & Enabled & "</label><label class=""btn btn-sm btn-secondary""><input type=""radio"" name=""rbtnTax"" id=""rbtnFalse"" value=""0"" /> " & Disabled & "</label></div>"
            chkTax = " checked=""checked"""
        Else
            rbtnTax = "<div class=""btn-group btn-group-toggle radio"" data-toggle=""buttons""><label class=""btn btn-secondary btn-sm""><input type=""radio"" name=""rbtnTax"" id=""rbtnTrue"" value=""1"" /> " & Enabled & "</label><label class=""btn btn-sm btn-secondary active""><input type=""radio"" name=""rbtnTax"" id=""rbtnFalse"" value=""0"" checked /> " & Disabled & "</label></div>"
            chkTax = ""
        End If

        Dim bEnabled As Boolean
        If IsDBNull(ds.Tables(0).Rows(0).Item("bEnabled")) Then bEnabled = False Else bEnabled = ds.Tables(0).Rows(0).Item("bEnabled")
        Dim chkEnabled As String
        If bEnabled = True Then
            'rbtnTax = "<div class=""btn-group btn-group-toggle radio"" data-toggle=""buttons""><label class=""btn btn-secondary btn-sm active""><input type=""radio"" name=""rbtnTax"" id=""rbtnTrue"" value=""1"" checked /> " & Enabled & "</label><label class=""btn btn-sm btn-secondary""><input type=""radio"" name=""rbtnTax"" id=""rbtnFalse"" value=""0"" /> " & Disabled & "</label></div>"
            chkEnabled = " checked=""checked"""
        Else
            'rbtnTax = "<div class=""btn-group btn-group-toggle radio"" data-toggle=""buttons""><label class=""btn btn-secondary btn-sm""><input type=""radio"" name=""rbtnTax"" id=""rbtnTrue"" value=""1"" /> " & Enabled & "</label><label class=""btn btn-sm btn-secondary active""><input type=""radio"" name=""rbtnTax"" id=""rbtnFalse"" value=""0"" checked /> " & Disabled & "</label></div>"
            chkEnabled = ""
        End If

        Dim body As New StringBuilder("")
        'tabs
        body.Append("<ul class=""nav nav-tabs m-0"">")
        body.Append("<li class=""nav-item""><a class=""nav-link active"" id=""base-tab1"" data-toggle=""tab"" aria-controls=""tab1"" href=""#tab1"" aria-expanded=""true"" >" & tabGeneral & "</a></li>")
        body.Append("</ul>")
        'contents
        body.Append("<div class=""tab-content m-0 p-1"">")
        body.Append("<div role=""tabpanel"" class=""tab-pane active"" id=""tab1"" aria-expanded=""true"" aria-labelledby=""base-tab1"">")
        body.Append("<form id=""frmItem"">")
        body.Append("<div class=""row"">")
        body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-3 text-sm-right"">" & lblNo & ":</div><div class=""col-md-9""><input type=""text"" id=""txtItemNo"" name=""txtItemNo"" class=""form-control form-control-sm"" value=""" & ds.Tables(0).Rows(0).Item("strItem") & """ /></div></div>")
        body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-3 text-sm-right"">" & lblNameAr & ":</div><div class=""col-md-9""><input type=""text"" id=""txtItemNameAr"" name=""txtItemNameAr"" class=""form-control form-control-sm dir-rtl"" value=""" & ds.Tables(0).Rows(0).Item("strItemAr") & """ /></div></div>")
        body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-3 text-sm-right"">" & lblNameEn & ":</div><div class=""col-md-9""><input type=""text"" id=""txtItemNameEn"" name=""txtItemNameEn"" class=""form-control form-control-sm dir-ltr"" value=""" & ds.Tables(0).Rows(0).Item("strItemEn") & """ /></div></div>")
        body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-3 text-sm-right"">" & lblGroup & ":</div><div class=""col-md-9"">" & drpGroups & "</div></div>")
        'body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-3 text-sm-right"">" & lblTax & ":</div><div class=""col-md-4"">" & rbtnTax & "</div><div class=""col-md-5""><input type=""number"" id=""txtTaxAmount"" name=""txtTaxAmount"" class=""form-control form-control-sm"" value=""" & Math.Round(CDec("0" & ds.Tables(0).Rows(0).Item("curTax").ToString), 2, MidpointRounding.AwayFromZero) & """ /></div></div>")
        body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-3 text-sm-right"">" & lblTax & ":</div><div class=""col-md-4""><fieldset><div class=""float-left""><input type=""checkbox"" class=""switchBootstrap"" name=""chkTax"" value=""1"" data-size=""small"" data-on-text=""" & Enabled & """ data-off-text=""" & Disabled & """ " & chkTax & "/></div></fieldset></div><div class=""col-md-5""><input type=""number"" id=""txtTaxAmount"" name=""txtTaxAmount"" class=""form-control form-control-sm"" value=""" & Math.Round(CDec("0" & ds.Tables(0).Rows(0).Item("curTax").ToString), 2, MidpointRounding.AwayFromZero) & """ /></div></div>")
        body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-3 text-sm-right"">" & lblAvailability & ":</div><div class=""col-md-9""><fieldset><div class=""float-left""><input type=""checkbox"" class=""switchBootstrap"" name=""chkEnabled"" value=""1"" data-size=""small"" data-on-text=""" & Available & """ data-off-text=""" & Unavailable & """ " & chkEnabled & "/></div></fieldset></div></div>")
        'body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-3 text-sm-right""></div><div class=""col-md-9""><button type=""button"" class=""btn btn-success"" onclick=""javascript:saveItem();""><i class=""icon-save""></i> " & btnSave & "</button></div></div>")
        body.Append("</div>")
        body.Append("</form>")
        body.Append("</div>")
        body.Append("</div>")

        body.Append("<script>")
        body.Append("function saveItem() {var valJson = JSON.stringify($('#frmItem').serializeArray());var dataJson = { strItem: """ & strItem & """, Fields: valJson };var dataJsonString = JSON.stringify(dataJson);$.ajax({type: 'POST',url: '../Stock/ajax.aspx/saveItem',data: dataJsonString,contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {if (response.d.indexOf('Err:') >= 0) {msg('',response.d.substring(4, response.d.length),'error');} else {$('#prtJS').html(response.d);}},failure: function (msg) {alert(msg);},error: function (xhr, ajaxOptions, thrownError) {alert(' write json item, Ajax error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});}")
        body.Append("$('.switchBootstrap').bootstrapSwitch();")
        body.Append("</script>")

        Dim sh As New Share.UI
        Return sh.drawModal("", body.ToString, "<button type=""button"" class=""btn btn-success"" onclick=""javascript:saveItem();""><i class=""icon-save""></i> " & btnSave & "</button> <button type=""button"" class=""btn btn-secondary"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnClose & "</button>", Share.UI.ModalSize.Medium, "", "p-0")
    End Function

    Public Function ViewCollectItems(ByVal byteWarehouse As Byte, ByVal CheckBalance As Boolean) As String
        Dim btnClose, btnStart, btnBack As String
        Dim Header As String

        Select Case byteLanguage
            Case 2
                DataLang = "Ar"
                Header = "تجميع الأصناف من مصادر أخرى"
                btnStart = "ابدأ"
                btnBack = "رجوع"
                btnClose = "إغلاق"
            Case Else
                DataLang = "En"
                Header = "Collect items from another resouces"
                btnStart = "Start"
                btnBack = "Back"
                btnClose = "Close"
        End Select

        Dim body As New StringBuilder("")
        body.Append("<div class=""height-200 p-1"">")
        body.Append("<form id=""frmCollect"">")

        body.Append("<div class=""row"" id=""divMain"">")
        body.Append("<div class=""col-md-12""><label class=""display-inline-block custom-control custom-radio ml-1""><input type=""radio"" name=""rbtnAction"" value=""1"" class=""custom-control-input""><span class=""custom-control-indicator""></span><span class=""custom-control-description ml-0""> Expired Items</span></label></div>")
        body.Append("<div class=""col-md-12""><label class=""display-inline-block custom-control custom-radio ml-1""><input type=""radio"" name=""rbtnAction"" value=""2"" class=""custom-control-input""><span class=""custom-control-indicator""></span><span class=""custom-control-description ml-0""> Returned Items</span></label></div>")
        body.Append("<div class=""col-md-12""><label class=""display-inline-block custom-control custom-radio ml-1""><input type=""radio"" name=""rbtnAction"" value=""3"" class=""custom-control-input""><span class=""custom-control-indicator""></span><span class=""custom-control-description ml-0""> Transfered Items</span></label></div>")
        body.Append("</div>")

        body.Append("<div class=""row"" id=""divExpired"" style=""display:none;"">")
        body.Append("<div class=""col-md-12"">Select max date of expired items:</div>")
        body.Append("<div class=""col-md-12""><input id=""dtpExpired"" value="""" type=""text"" class=""form-control form-control-sm input-sm date-formatter1 dir-ltr""><input type=""hidden"" id=""txtExpiredFrom"" name=""txtExpiredFrom"" value=""" & Today.ToString("yyyy-MM-dd") & """ /><input type=""hidden"" id=""txtExpiredTo"" name=""txtExpiredTo"" value=""" & Today.ToString("yyyy-MM-dd") & """ /></div>")
        body.Append("</div>")

        body.Append("<div class=""row"" id=""divReturned"" style=""display:none;"">")
        body.Append("<div class=""col-md-12"">Select date range for returned items:</div>")
        body.Append("<div class=""col-md-12""><input id=""dtpReturned"" value="""" type=""text"" class=""form-control form-control-sm input-sm date-formatter dir-ltr""><input type=""hidden"" id=""txtReturnedFrom"" name=""txtReturnedFrom"" value=""" & Today.ToString("yyyy-MM-dd") & """ /><input type=""hidden"" id=""txtReturnedTo"" name=""txtReturnedTo"" value=""" & Today.ToString("yyyy-MM-dd") & """ /></div>")
        body.Append("</div>")

        body.Append("<div class=""row"" id=""divTransfered"" style=""display:none;"">")
        body.Append("<div class=""col-md-12"">Type the transfer voucher:</div>")
        body.Append("<div class=""col-md-12""><input id=""txtTransfer"" name=""txtTransfer"" value="""" type=""text"" class=""form-control form-control-sm input-sm"" /></div>")
        body.Append("</div>")

        body.Append("</form>")
        body.Append("</div>")
        'singleDatePicker: true,
        body.Append("<script type=""text/javascript"">")
        body.Append("$('#dtpReturned').daterangepicker({ startDate: '" & Today.ToString("yyyy-MM-dd") & "', endDate: '" & Today.ToString("yyyy-MM-dd") & "',locale: {format: 'YYYY-MM-DD', separator: separator, daysOfWeek: daysOfWeek, monthNames: monthNames, applyLabel: applyLabel, cancelLabel: cancelLabel, fromLabel: fromLabel, toLabel: toLabel} }, function(start, end, label) {$('#txtReturnedFrom').val(start.format('YYYY-MM-DD'));$('#txtReturnedTo').val(end.format('YYYY-MM-DD'));});")
        body.Append("$('#dtpExpired').daterangepicker({ startDate: '" & Today.ToString("yyyy-MM-dd") & "', endDate: '" & Today.ToString("yyyy-MM-dd") & "', singleDatePicker: true, locale: {format: 'YYYY-MM-DD', separator: separator, daysOfWeek: daysOfWeek, monthNames: monthNames, applyLabel: applyLabel, cancelLabel: cancelLabel, fromLabel: fromLabel, toLabel: toLabel} }, function(start, end, label) {$('#txtExpiredFrom').val(start.format('YYYY-MM-DD'));$('#txtExpiredTo').val(end.format('YYYY-MM-DD'));});")
        body.Append("datePatternNew = datePattern + separator + datePattern;$('.date-formatter').formatter({ pattern: datePatternNew });")
        body.Append("datePatternNew1 = datePattern;$('.date-formatter1').formatter({ pattern: datePatternNew1 });")
        body.Append("$('input[type=radio][name=rbtnAction]').change(function() {changeView(this.value);});")
        body.Append("function changeView(view){$('#divMain').hide();$('#btnBack').show();$('#btnStart').show(); switch(parseInt(view)) {case 1:$('#divExpired').show();break;case 2:$('#divReturned').show();break;case 3:$('#divTransfered').show();break;}}")
        body.Append("function goBack(){$('#divMain').show();$('#btnBack').hide();$('#btnStart').hide();$('#divExpired').hide();$('#divReturned').hide();$('#divTransfered').hide();}")
        body.Append("function collectItems(e) {disableMe(e, '#btnBack'); var valJson = JSON.stringify($('#frmCollect').serializeArray());var dataJson = { byteWarehouse: " & byteWarehouse & ", Fields: valJson, CheckBalance: " & CheckBalance.ToString.ToLower & "};var dataJsonString = JSON.stringify(dataJson);$.ajax({type: 'POST',url: '../Stock/ajax.aspx/CollectItems',data: dataJsonString,contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {if (response.d.indexOf('Err:') >= 0) {msg('',response.d.substring(4, response.d.length),'error');} else {$('#prtJS').html(response.d);}},failure: function (msg) {alert(msg);},error: function (xhr, ajaxOptions, thrownError) {alert(' write json item, Ajax error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});}")
        body.Append("</script>")

        Dim sh As New Share.UI
        Return sh.drawModal(Header, body.ToString, " <button type=""button"" id=""btnBack"" style=""display:none;"" class=""btn btn-outline-primary float-left"" onclick=""javascript:goBack();""><i class=""icon-rotate-left""></i> " & btnBack & "</button> <button type=""button"" class=""btn btn-success"" id=""btnStart"" style=""display:none;"" onclick=""javascript:collectItems(this);""><i class=""icon-bolt""></i> " & btnStart & "</button> <button type=""button"" class=""btn btn-secondary"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnClose & "</button>", Share.UI.ModalSize.Small, "", "p-0")
    End Function

    Public Function CollectItems(ByVal byteWarehouse As Byte, ByVal Fields As String, ByVal CheckBalance As Boolean) As String
        Dim Action, ReturnedFrom, ReturnedTo, ExpiredFrom, TransferVoucher As String
        Action = ""
        ReturnedFrom = ""
        ReturnedTo = ""

        Select Case byteLanguage
            Case 2
                DataLang = "Ar"
            Case Else
                DataLang = "En"
        End Select

        ' get form values
        Dim jss As New JavaScriptSerializer()
        Dim field() As NameValue = jss.Deserialize(Of NameValue())(Fields)
        For I = 0 To field.Length - 1
            Select Case field(I).name()
                Case "rbtnAction"
                    Action = field(I).value()
                Case "txtReturnedFrom"
                    ReturnedFrom = field(I).value()
                Case "txtReturnedTo"
                    ReturnedTo = field(I).value()
                Case "txtExpiredFrom"
                    ExpiredFrom = field(I).value()
                Case "txtTransfer"
                    TransferVoucher = field(I).value()
            End Select
        Next
        'If lstItems.Length > 0 Then lstItems = Left(lstItems, lstItems.Length - 1)
        Dim script As String = "var balance='';"
        Dim strItemName As String = ""
        Dim status As String = ""
        Select Case Action
            Case 1
                Dim Errors As String = ""
                Dim ds As DataSet = dcl.GetDS("SELECT XI.byteWarehouse, W.strWarehouseEn, XI.strItem, I.strItem" & DataLang & ", CONVERT(varchar(10), XI.dateExpiry, 120) AS dateExpiry, SUM(B.intSign * XI.curQuantity * U.curFactor) AS curBalance FROM Stock_Base AS B INNER JOIN Stock_Trans AS T ON B.byteBase = T.byteBase INNER JOIN Stock_Xlink AS X ON T.lngTransaction = X.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink INNER JOIN Stock_Units AS U ON U.byteUnit = XI.byteUnit INNER JOIN Stock_Warehouses AS W ON XI.byteWarehouse=W.byteWarehouse INNER JOIN Stock_Items AS I ON XI.strItem=I.strItem WHERE YEAR(T.dateTransaction) = " & intYear & " AND T.byteStatus > 0 And B.bInclude <> 0 AND T.dateTransaction <= '" & Today.ToString("yyyy-MM-dd") & "' AND XI.byteWarehouse=" & byteWarehouse & " GROUP BY XI.byteWarehouse, W.strWarehouseEn, XI.strItem, I.strItem" & DataLang & ", XI.dateExpiry HAVING XI.dateExpiry <= '" & ExpiredFrom & "' AND SUM(B.intSign * XI.curQuantity * U.curFactor) > 0")
                For I = 0 To ds.Tables(0).Rows.Count - 1
                    strItemName = Replace(ds.Tables(0).Rows(I).Item("strItem" & DataLang), "'", "\'")
                    Dim ret As String = getItemCost(ds.Tables(0).Rows(I).Item("strItem"))
                    Dim curCost As Decimal = 0
                    If Left(ret, 4) = "Err:" Then
                        status = "<i class=""icon-question2 red""></i>"
                        'script = script & "msg('','" & Replace(ret, "Err:", "") & "','error');"
                        Errors = Errors & Replace(ret, "Err:", "") & "<br />"
                    Else
                        status = "<i class=""icon-check green""></i>"
                        curCost = CDec(ret)
                    End If
                    Dim dsPrice As DataSet
                    Dim curPrice As Decimal
                    'dsPrice = dcl.GetDS("SELECT XI.curUnitPrice As Price FROM Stock_Trans AS T INNER JOIN (Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink = X.lngXlink) ON T.lngTransaction = X.lngTransaction WHERE XI.curUnitPrice Is Not Null AND T.byteStatus>0 AND XI.byteQuantityType=1 AND T.byteBase IN (10, 40) And XI.strItem='" & ds.Tables(0).Rows(I).Item("strItem") & "' ORDER BY T.dateTransaction DESC")
                    dsPrice = dcl.GetDS("SELECT XI.curBasePrice As Price FROM Stock_Trans AS T INNER JOIN (Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink = X.lngXlink) ON T.lngTransaction = X.lngTransaction WHERE XI.curBasePrice Is Not Null AND T.byteStatus>0 AND XI.byteQuantityType=1 AND T.byteBase IN (10,40) And XI.strItem='" & ds.Tables(0).Rows(I).Item("strItem") & "' ORDER BY T.dateTransaction DESC")
                    If dsPrice.Tables(0).Rows.Count > 0 Then
                        curPrice = dsPrice.Tables(0).Rows(0).Item("Price")
                    Else
                        curPrice = 0
                    End If
                    If CheckBalance = True Then
                        script = script & "if (parseInt(" & ds.Tables(0).Rows(I).Item("curBalance") & ") + calculateQuantity('" & ds.Tables(0).Rows(I).Item("strItem") & "','" & CDate(ds.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM-dd") & "') > " & ds.Tables(0).Rows(I).Item("curBalance") & ") { balance = balance + 'The balance of item (" & ds.Tables(0).Rows(I).Item("strItem") & ") is: " & Math.Round(ds.Tables(0).Rows(I).Item("curBalance"), 0, MidpointRounding.AwayFromZero) & "<br />';} else {"
                        script = script & "$('#tblItems > tbody').prepend('<tr><td style=""width:30px"">" & status & "</td><td style=""width:50px""></td><td style=""width:70px"">" & ds.Tables(0).Rows(I).Item("strItem") & "</td><td style=""width:300px"" class=""text-md-left"" title=""" & strItemName & """>" & strItemName & "</td><td style=""width:100px"">" & CDate(ds.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM") & "</td><td style=""width:70px"">" & Math.Round(ds.Tables(0).Rows(I).Item("curBalance"), 0, MidpointRounding.AwayFromZero) & "</td><td style=""width:70px"">" & Math.Round(curPrice, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</td><td><button type=""button"" class=""btn btn-danger btn-xs"" onclick=""javascript:removeThis(this)"">Delete</button><input type=""hidden"" id=""item'+counter+'"" class=""item"" name=""item"" value=""" & ds.Tables(0).Rows(I).Item("strItem") & """ /><input type=""hidden"" name=""expiry"" id=""expiry'+counter+'"" value=""" & CDate(ds.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM-dd") & """ /><input type=""hidden"" id=""quantity'+counter+'"" name=""quantity"" class=""quantity"" value=""" & ds.Tables(0).Rows(I).Item("curBalance") & """ /><input type=""hidden"" name=""price"" value=""" & curPrice & """ /><input type=""hidden"" name=""cost"" value=""" & curCost & """ /></td></tr>');counter++;$('#txtBarcode').focus();"
                        script = script & "}"
                    Else
                        script = script & "$('#tblItems > tbody').prepend('<tr><td style=""width:30px"">" & status & "</td><td style=""width:50px""></td><td style=""width:70px"">" & ds.Tables(0).Rows(I).Item("strItem") & "</td><td style=""width:300px"" class=""text-md-left"" title=""" & strItemName & """>" & strItemName & "</td><td style=""width:100px"">" & CDate(ds.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM") & "</td><td style=""width:70px"">" & Math.Round(ds.Tables(0).Rows(I).Item("curBalance"), 0, MidpointRounding.AwayFromZero) & "</td><td style=""width:70px"">" & Math.Round(curPrice, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</td><td><button type=""button"" class=""btn btn-danger btn-xs"" onclick=""javascript:removeThis(this)"">Delete</button><input type=""hidden"" id=""item'+counter+'"" class=""item"" name=""item"" value=""" & ds.Tables(0).Rows(I).Item("strItem") & """ /><input type=""hidden"" name=""expiry"" id=""expiry'+counter+'"" value=""" & CDate(ds.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM-dd") & """ /><input type=""hidden"" id=""quantity'+counter+'"" name=""quantity"" class=""quantity"" value=""" & ds.Tables(0).Rows(I).Item("curBalance") & """ /><input type=""hidden"" name=""price"" value=""" & curPrice & """ /><input type=""hidden"" name=""cost"" value=""" & curCost & """ /></td></tr>');counter++;$('#txtBarcode').focus();"
                    End If
                Next
                If Errors <> "" Then script = script & "msg('','" & Errors & "','error');"
                script = script & "if(balance!='') {msg('',balance,'error');}"
            Case 2
                Dim Errors As String = ""
                Dim ds As DataSet = dcl.GetDS("SELECT * FROM Stock_Trans AS T INNER JOIN Stock_Xlink AS X ON T.lngTransaction=X.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink=XI.lngXlink INNER JOIN Stock_Items AS I ON XI.strItem=I.strItem WHERE T.byteBase=18 AND T.byteStatus>0 AND XI.byteWarehouse=" & byteWarehouse & " AND T.dateTransaction BETWEEN '" & ReturnedFrom & "' AND '" & ReturnedTo & "'")
                For I = 0 To ds.Tables(0).Rows.Count - 1
                    strItemName = Replace(ds.Tables(0).Rows(I).Item("strItem" & DataLang), "'", "\'")
                    Dim ret As String = getItemCost(ds.Tables(0).Rows(I).Item("strItem"))
                    Dim curCost As Decimal = 0
                    If Left(ret, 4) = "Err:" Then
                        status = "<i class=""icon-question2 yellow""></i>"
                        'script = script & "msg('','" & Replace(ret, "Err:", "") & "','error');"
                        Errors = Errors & Replace(ret, "Err:", "") & "<br />"
                    Else
                        status = "<i class=""icon-check green""></i>"
                        curCost = CDec(ret)
                    End If
                    If CheckBalance = True Then
                        Dim curBalance As Decimal = checkStock(ds.Tables(0).Rows(I).Item("strItem"), Today, byteWarehouse, ds.Tables(0).Rows(I).Item("dateExpiry"))
                        script = script & "if (parseInt(" & ds.Tables(0).Rows(I).Item("curQuantity") & ") + calculateQuantity('" & ds.Tables(0).Rows(I).Item("strItem") & "','" & CDate(ds.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM-dd") & "') > " & curBalance & ") {balance = balance + 'The balance of item (" & ds.Tables(0).Rows(I).Item("strItem") & ") is: " & Math.Round(curBalance, 0, MidpointRounding.AwayFromZero) & "<br />';} else {"
                        script = script & "$('#tblItems > tbody').prepend('<tr><td style=""width:30px"">" & status & "</td><td style=""width:50px""></td><td style=""width:70px"">" & ds.Tables(0).Rows(I).Item("strItem") & "</td><td style=""width:300px"" class=""text-md-left"" title=""" & strItemName & """>" & strItemName & "</td><td style=""width:100px"">" & CDate(ds.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM") & "</td><td style=""width:70px"">" & Math.Round(ds.Tables(0).Rows(I).Item("curQuantity"), 0, MidpointRounding.AwayFromZero) & "</td><td style=""width:70px"">" & Math.Round(ds.Tables(0).Rows(I).Item("curBasePrice"), byteCurrencyRound, MidpointRounding.AwayFromZero) & "</td><td><button type=""button"" class=""btn btn-danger btn-xs"" onclick=""javascript:removeThis(this)"">Delete</button><input type=""hidden"" id=""item'+counter+'"" class=""item"" name=""item"" value=""" & ds.Tables(0).Rows(I).Item("strItem") & """ /><input type=""hidden"" name=""expiry"" id=""expiry'+counter+'"" value=""" & CDate(ds.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM-dd") & """ /><input type=""hidden"" id=""quantity'+counter+'"" name=""quantity"" class=""quantity"" value=""" & ds.Tables(0).Rows(I).Item("curQuantity") & """ /><input type=""hidden"" name=""price"" value=""" & ds.Tables(0).Rows(I).Item("curBasePrice") & """ /><input type=""hidden"" name=""cost"" value=""" & curCost & """ /></td></tr>');counter++;$('#txtBarcode').focus();"
                        script = script & "}"
                    Else
                        script = script & "$('#tblItems > tbody').prepend('<tr><td style=""width:30px"">" & status & "</td><td style=""width:50px""></td><td style=""width:70px"">" & ds.Tables(0).Rows(I).Item("strItem") & "</td><td style=""width:300px"" class=""text-md-left"" title=""" & strItemName & """>" & strItemName & "</td><td style=""width:100px"">" & CDate(ds.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM") & "</td><td style=""width:70px"">" & Math.Round(ds.Tables(0).Rows(I).Item("curQuantity"), 0, MidpointRounding.AwayFromZero) & "</td><td style=""width:70px"">" & Math.Round(ds.Tables(0).Rows(I).Item("curBasePrice"), byteCurrencyRound, MidpointRounding.AwayFromZero) & "</td><td><button type=""button"" class=""btn btn-danger btn-xs"" onclick=""javascript:removeThis(this)"">Delete</button><input type=""hidden"" id=""item'+counter+'"" class=""item"" name=""item"" value=""" & ds.Tables(0).Rows(I).Item("strItem") & """ /><input type=""hidden"" name=""expiry"" id=""expiry'+counter+'"" value=""" & CDate(ds.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM-dd") & """ /><input type=""hidden"" id=""quantity'+counter+'"" name=""quantity"" class=""quantity"" value=""" & ds.Tables(0).Rows(I).Item("curQuantity") & """ /><input type=""hidden"" name=""price"" value=""" & ds.Tables(0).Rows(I).Item("curBasePrice") & """ /><input type=""hidden"" name=""cost"" value=""" & curCost & """ /></td></tr>');counter++;$('#txtBarcode').focus();"
                    End If
                Next
                If Errors <> "" Then script = script & "msg('','" & Errors & "','error');"
                script = script & "if(balance!='') {msg('',balance,'error');}"
            Case 3
                Dim Errors As String = ""
                Dim ds As DataSet = dcl.GetDS("SELECT * FROM Stock_Trans AS T INNER JOIN Stock_Xlink AS X ON T.lngTransaction=X.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink=XI.lngXlink INNER JOIN Stock_Items AS I ON XI.strItem=I.strItem WHERE T.byteBase IN (19,20) AND T.byteStatus>0 AND XI.byteWarehouse=" & byteWarehouse & " AND T.strTransaction='" & TransferVoucher & "' AND YEAR(T.dateTransaction) = " & intYear)
                For I = 0 To ds.Tables(0).Rows.Count - 1
                    strItemName = Replace(ds.Tables(0).Rows(I).Item("strItem" & DataLang), "'", "\'")
                    Dim ret As String = getItemCost(ds.Tables(0).Rows(I).Item("strItem"))
                    Dim curQuantity As Decimal = ds.Tables(0).Rows(I).Item("curQuantity")
                    Dim curCost As Decimal = 0
                    If Left(ret, 4) = "Err:" Then
                        status = "<i class=""icon-question2 yellow""></i>"
                        'script = script & "msg('','" & Replace(ret, "Err:", "") & "','error');"
                        Errors = Errors & Replace(ret, "Err:", "") & "<br />"
                    Else
                        status = "<i class=""icon-check green""></i>"
                        curCost = CDec(ret)
                    End If
                    Dim dsPrice As DataSet
                    Dim curPrice As Decimal
                    dsPrice = dcl.GetDS("SELECT XI.curUnitPrice As Price FROM Stock_Trans AS T INNER JOIN (Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink = X.lngXlink) ON T.lngTransaction = X.lngTransaction WHERE XI.curUnitPrice Is Not Null AND T.byteStatus>0 AND XI.byteQuantityType=1 AND T.byteBase IN (10, 40) And XI.strItem='" & ds.Tables(0).Rows(I).Item("strItem") & "' ORDER BY T.dateTransaction DESC")
                    If dsPrice.Tables(0).Rows.Count > 0 Then
                        curPrice = dsPrice.Tables(0).Rows(0).Item("Price")
                    Else
                        curPrice = 0
                    End If
                    If CheckBalance = True Then
                        Dim curBalance As Decimal = checkStock(ds.Tables(0).Rows(I).Item("strItem"), Today, byteWarehouse, ds.Tables(0).Rows(I).Item("dateExpiry"))
                        script = script & "if (parseInt(" & ds.Tables(0).Rows(I).Item("curQuantity") & ") + calculateQuantity('" & ds.Tables(0).Rows(I).Item("strItem") & "','" & CDate(ds.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM-dd") & "') > " & curBalance & ") {balance = balance + 'The balance of item (" & ds.Tables(0).Rows(I).Item("strItem") & ") is: " & Math.Round(curBalance, 0, MidpointRounding.AwayFromZero) & "<br />';} else {"
                        script = script & "$('#tblItems > tbody').prepend('<tr><td style=""width:30px"">" & status & "</td><td style=""width:50px""></td><td style=""width:70px"">" & ds.Tables(0).Rows(I).Item("strItem") & "</td><td style=""width:300px"" class=""text-md-left"" title=""" & strItemName & """>" & strItemName & "</td><td style=""width:100px"">" & CDate(ds.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM") & "</td><td style=""width:70px"">" & Math.Round(curQuantity, 0, MidpointRounding.AwayFromZero) & "</td><td style=""width:70px"">" & Math.Round(curPrice, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</td><td><button type=""button"" class=""btn btn-danger btn-xs"" onclick=""javascript:removeThis(this)"">Delete</button><input type=""hidden"" id=""item'+counter+'"" class=""item"" name=""item"" value=""" & ds.Tables(0).Rows(I).Item("strItem") & """ /><input type=""hidden"" name=""expiry"" id=""expiry'+counter+'"" value=""" & CDate(ds.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM-dd") & """ /><input type=""hidden"" id=""quantity'+counter+'"" name=""quantity"" class=""quantity"" value=""" & curQuantity & """ /><input type=""hidden"" name=""price"" value=""" & curPrice & """ /><input type=""hidden"" name=""cost"" value=""" & curCost & """ /></td></tr>');counter++;$('#txtBarcode').focus();"
                        script = script & "}"
                    Else
                        script = script & "$('#tblItems > tbody').prepend('<tr><td style=""width:30px"">" & status & "</td><td style=""width:50px""></td><td style=""width:70px"">" & ds.Tables(0).Rows(I).Item("strItem") & "</td><td style=""width:300px"" class=""text-md-left"" title=""" & strItemName & """>" & strItemName & "</td><td style=""width:100px"">" & CDate(ds.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM") & "</td><td style=""width:70px"">" & Math.Round(curQuantity, 0, MidpointRounding.AwayFromZero) & "</td><td style=""width:70px"">" & Math.Round(curPrice, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</td><td><button type=""button"" class=""btn btn-danger btn-xs"" onclick=""javascript:removeThis(this)"">Delete</button><input type=""hidden"" id=""item'+counter+'"" class=""item"" name=""item"" value=""" & ds.Tables(0).Rows(I).Item("strItem") & """ /><input type=""hidden"" name=""expiry"" id=""expiry'+counter+'"" value=""" & CDate(ds.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM-dd") & """ /><input type=""hidden"" id=""quantity'+counter+'"" name=""quantity"" class=""quantity"" value=""" & curQuantity & """ /><input type=""hidden"" name=""price"" value=""" & curPrice & """ /><input type=""hidden"" name=""cost"" value=""" & curCost & """ /></td></tr>');counter++;$('#txtBarcode').focus();"
                    End If
                Next
                If Errors <> "" Then script = script & "msg('','" & Errors & "','error');"
                script = script & "if(balance!='') {msg('',balance,'error');}"
            Case Else

        End Select
        'script = "function calcQuantity(item, expire) {var q=0;$.each($('.item'), function (k, v) { if($('#'+v.id.replace('item','expiry')).val()==expire && $(v).val()==item) q=q+parseInt($('#'+v.id.replace('item','quantity')).val());});return q;}" & script


        Return "<script>" & script & " $('#mdlBeta').modal('hide');</script>"

        'Try
        '    dcl.ExecScalar("UPDATE Stock_Items SET bLimit=0 WHERE bLimit=1")
        '    If lstItems.Length > 0 Then dcl.ExecScalar("UPDATE Stock_Items SET bLimit=1 WHERE strItem IN (" & lstItems & ")")
        'Catch ex As Exception
        '    Return "Err:" & ex.Message
        'End Try

        'Return "<script>msg('', 'Items have changed successfully!', 'success'); $('#mdlConfirm').modal('hide');</script>"
    End Function

    Public Function changeLimit() As String
        Dim btnClose, btnSave As String
        Dim Header As String

        Select Case byteLanguage
            Case 2
                DataLang = "Ar"

                Header = "تغيير الأصناف إلى محدودة"

                btnSave = "حفظ التغييرات"
                btnClose = "إغلاق"
            Case Else
                DataLang = "En"

                Header = "Change items to limited"

                btnSave = "Save Changes"
                btnClose = "Close"
        End Select

        Dim body As New StringBuilder("")

        body.Append("<div class=""row"">")
        body.Append("<form id=""frmItems"">")
        body.Append("<div class=""col-md-12 p-1"">")

        body.Append("<div class=""col-md-5 ml-0 mr-0 pl-0 pr-0"">")
        body.Append("<h5 class=""text-md-center"">All Items</h5>")
        body.Append("<select class=""form-control font-small-3"" size=""10"" id=""selAll"" multiple=""multiple"" style=""height:280px; overflow-y:auto"">")
        Dim dsUnlimit As DataSet = dcl.GetDS("SELECT * FROM Stock_Items WHERE strParentItem > 0 AND (bLimit = 0 OR bLimit IS NULL)")
        For I = 0 To dsUnlimit.Tables(0).Rows.Count - 1
            body.Append("<option value=""" & dsUnlimit.Tables(0).Rows(I).Item("strItem") & """ title=""" & dsUnlimit.Tables(0).Rows(I).Item("strItem" & DataLang) & """>" & dsUnlimit.Tables(0).Rows(I).Item("strItem") & "- " & dsUnlimit.Tables(0).Rows(I).Item("strItem" & DataLang) & "</option>")
        Next
        body.Append("</select>")
        body.Append("<input type=""text"" class=""form-control"" id=""txtAllItems"" placeholder=""find an item in the list"" />")
        body.Append("</div>")

        body.Append("<div class=""col-md-2 ml-0 mr-0 pl-0 pr-0"">")
        body.Append("<h5></h5>")
        body.Append("<br />")
        body.Append("<br />")
        body.Append("<br />")
        body.Append("<div class=""pl-1 pr-1""><button type=""button"" class=""btn btn-sm btn-success full-width"" onclick=""javascript:move2selected();""><i class=""icon icon-angle-right""></i> </button></div>")
        body.Append("<br />")
        body.Append("<div class=""pl-1 pr-1""><button type=""button"" class=""btn btn-sm btn-success full-width"" onclick=""javascript:moveAll2selected();""><i class=""icon icon-angle-double-right""></i> </button></div>")
        body.Append("<br />")
        body.Append("<div class=""pl-1 pr-1""><button type=""button"" class=""btn btn-sm btn-success full-width"" onclick=""javascript:move2all();""><i class=""icon icon-angle-left""></i> </button></div>")
        body.Append("<br />")
        body.Append("<div class=""pl-1 pr-1""><button type=""button"" class=""btn btn-sm btn-success full-width"" onclick=""javascript:moveAll2all();""><i class=""icon icon-angle-double-left""></i> </button></div>")
        body.Append("</div>")

        body.Append("<div class=""col-md-5 ml-0 mr-0 pl-0 pr-0"">")
        body.Append("<h5 class=""text-md-center"">Selected Items</h5>")
        body.Append("<select class=""form-control font-small-3"" size=""10"" id=""selSelected"" name=""drpSelected"" multiple=""multiple"" style=""height:280px; overflow-y:auto"">")
        Dim dslimit As DataSet = dcl.GetDS("SELECT * FROM Stock_Items WHERE bLimit=1")
        For I = 0 To dslimit.Tables(0).Rows.Count - 1
            body.Append("<option value=""" & dslimit.Tables(0).Rows(I).Item("strItem") & """ title=""" & dslimit.Tables(0).Rows(I).Item("strItem" & DataLang) & """>" & dslimit.Tables(0).Rows(I).Item("strItem") & "- " & dslimit.Tables(0).Rows(I).Item("strItem" & DataLang) & "</option>")
        Next
        body.Append("</select>")
        body.Append("<input type=""text"" class=""form-control"" id=""txtSelectedItems"" placeholder=""find an item in the list"" />")
        body.Append("</div>")

        body.Append("</div>")
        body.Append("</form>")
        body.Append("</div>")

        body.Append("<script>")
        body.Append("function move2selected() {$('#selAll option:selected').remove().appendTo('#selSelected');} function move2all() {$('#selSelected option:selected').remove().appendTo('#selAll');}")
        body.Append("function moveAll2selected() {$('#selAll option').remove().appendTo('#selSelected');} function moveAll2all() {$('#selSelected option').remove().appendTo('#selAll');}")
        body.Append("$('#txtAllItems').on('change paste keyup', function() {var theText = $(this).val(); if (theText.length != 0 && event.which == 13){$('#selAll option:selected').prop('selected', false); var select = $('#selAll'); event.preventDefault(); var option = select.find(""option:contains("" + theText + "")""); option.prop('selected', true); var optionTop = option.offset().top;var selectTop = select.offset().top;select.scrollTop(select.scrollTop() + (optionTop - selectTop));}});")
        body.Append("$('#txtSelectedItems').on('change paste keyup', function() {var theText = $(this).val(); if (theText.length != 0 && event.which == 13){$('#selSelected option:selected').prop('selected', false); var select = $('#selSelected'); event.preventDefault(); var option = select.find(""option:contains("" + theText + "")""); option.prop('selected', true); var optionTop = option.offset().top;var selectTop = select.offset().top;select.scrollTop(select.scrollTop() + (optionTop - selectTop));}});")
        body.Append("function saveLimitItems() {$('#selSelected option').prop('selected', true); var valJson = JSON.stringify($('#frmItems').serializeArray());var dataJson = { Fields: valJson };var dataJsonString = JSON.stringify(dataJson);$.ajax({type: 'POST',url: '../Stock/ajax.aspx/changeLimitItems',data: dataJsonString,contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {if (response.d.indexOf('Err:') >= 0) {msg('',response.d.substring(4, response.d.length),'error');} else {$('#prtJS').html(response.d);}},failure: function (msg) {alert(msg);},error: function (xhr, ajaxOptions, thrownError) {alert(' write json item, Ajax error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});}")
        body.Append("</script>")

        Dim sh As New Share.UI
        Return sh.drawModal(Header, body.ToString, "<button type=""button"" class=""btn btn-success"" onclick=""javascript:saveLimitItems();""><i class=""icon-save""></i> " & btnSave & "</button> <button type=""button"" class=""btn btn-secondary"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnClose & "</button>", Share.UI.ModalSize.Medium, "", "p-0")
    End Function

    Public Function changeLimitItems(ByVal Fields As String) As String
        Dim lstItems As String = ""

        Select Case byteLanguage
            Case 2
                DataLang = "Ar"
            Case Else
                DataLang = "En"
        End Select

        ' get form values
        Dim jss As New JavaScriptSerializer()
        Dim field() As NameValue = jss.Deserialize(Of NameValue())(Fields)
        For I = 0 To field.Length - 1
            Select Case field(I).name()
                Case "drpSelected"
                    lstItems = lstItems & "'" & field(I).value() & "',"
            End Select
        Next
        If lstItems.Length > 0 Then lstItems = Left(lstItems, lstItems.Length - 1)

        Try
            dcl.ExecScalar("UPDATE Stock_Items SET bLimit=0 WHERE bLimit=1")
            If lstItems.Length > 0 Then dcl.ExecScalar("UPDATE Stock_Items SET bLimit=1 WHERE strItem IN (" & lstItems & ")")
        Catch ex As Exception
            Return "Err:" & ex.Message
        End Try

        Return "<script>msg('', 'Items have changed successfully!', 'success'); $('#mdlConfirm').modal('hide');</script>"
    End Function

    Public Function changeTax() As String
        Dim btnClose, btnSave As String
        Dim Header, lblSelect, lblAll As String
        Dim plcFind As String

        Select Case byteLanguage
            Case 2
                DataLang = "Ar"

                lblSelect = "كل الاصناف"
                lblAll = "الاصناف المحددة"
                Header = "تغيير ضريبة الأصناف"
                plcFind = "ابحث عن صنف في القائمة"
                btnSave = "حفظ التغييرات"
                btnClose = "إغلاق"
            Case Else
                DataLang = "En"
                lblSelect = "selected Items"
                lblAll = "All Item"
                Header = "Change items tax"
                plcFind = "find an item in the list"
                btnSave = "Save Changes"
                btnClose = "Close"
        End Select

        Dim body As New StringBuilder("")

        body.Append("<div class=""row"">")
        body.Append("<form id=""frmItems"">")
        body.Append("<div class=""col-md-12 p-1"">")

        body.Append("<div class=""col-md-5 ml-0 mr-0 pl-0 pr-0"">")
        body.Append("<h5 class=""text-md-center"">" & lblAll & " </h5>")
        body.Append("<select class=""form-control font-small-3"" size=""10"" id=""selAll"" multiple=""multiple"" style=""height:280px; overflow-y:auto"">")
        Dim dsUnlimit As DataSet = dcl.GetDS("SELECT * FROM Stock_Items WHERE strParentItem > 0 AND (bTax= 0 OR bTax IS NULL)")
        For I = 0 To dsUnlimit.Tables(0).Rows.Count - 1
            body.Append("<option value=""" & dsUnlimit.Tables(0).Rows(I).Item("strItem") & """ title=""" & dsUnlimit.Tables(0).Rows(I).Item("strItem" & DataLang) & """>" & dsUnlimit.Tables(0).Rows(I).Item("strItem") & "- " & dsUnlimit.Tables(0).Rows(I).Item("strItem" & DataLang) & "</option>")
        Next
        body.Append("</select>")
        body.Append("<input type=""text"" class=""form-control"" id=""txtAllItems"" placeholder=""" & plcFind & """  />")
        body.Append("</div>")

        body.Append("<div class=""col-md-2 ml-0 mr-0 pl-0 pr-0"">")
        body.Append("<h5></h5>")
        body.Append("<br />")
        body.Append("<br />")
        body.Append("<br />")
        body.Append("<div class=""pl-1 pr-1""><button type=""button"" class=""btn btn-sm btn-success full-width"" onclick=""javascript:move2selected();""><i class=""icon icon-angle-right""></i> </button></div>")
        body.Append("<br />")
        body.Append("<div class=""pl-1 pr-1""><button type=""button"" class=""btn btn-sm btn-success full-width"" onclick=""javascript:moveAll2selected();""><i class=""icon icon-angle-double-right""></i> </button></div>")
        body.Append("<br />")
        body.Append("<div class=""pl-1 pr-1""><button type=""button"" class=""btn btn-sm btn-success full-width"" onclick=""javascript:move2all();""><i class=""icon icon-angle-left""></i> </button></div>")
        body.Append("<br />")
        body.Append("<div class=""pl-1 pr-1""><button type=""button"" class=""btn btn-sm btn-success full-width"" onclick=""javascript:moveAll2all();""><i class=""icon icon-angle-double-left""></i> </button></div>")
        body.Append("</div>")

        body.Append("<div class=""col-md-5 ml-0 mr-0 pl-0 pr-0"">")
        body.Append("<h5 class=""text-md-center"">" & lblSelect & "</h5>")
        body.Append("<select class=""form-control font-small-3"" size=""10"" id=""selSelected"" name=""drpSelected"" multiple=""multiple"" style=""height:280px; overflow-y:auto"">")
        Dim dslimit As DataSet = dcl.GetDS("SELECT * FROM Stock_Items WHERE bTax=1")
        For I = 0 To dslimit.Tables(0).Rows.Count - 1
            body.Append("<option value=""" & dslimit.Tables(0).Rows(I).Item("strItem") & """ title=""" & dslimit.Tables(0).Rows(I).Item("strItem" & DataLang) & """>" & dslimit.Tables(0).Rows(I).Item("strItem") & "- " & dslimit.Tables(0).Rows(I).Item("strItem" & DataLang) & "</option>")
        Next
        body.Append("</select>")
        body.Append("<input type=""text"" class=""form-control"" id=""txtSelectedItems"" placeholder=""" & plcFind & """  />")
        body.Append("</div>")

        body.Append("</div>")
        body.Append("</form>")
        body.Append("</div>")

        body.Append("<script>")
        body.Append("function move2selected() {$('#selAll option:selected').remove().appendTo('#selSelected');} function move2all() {$('#selSelected option:selected').remove().appendTo('#selAll');}")
        body.Append("function moveAll2selected() {$('#selAll option').remove().appendTo('#selSelected');} function moveAll2all() {$('#selSelected option').remove().appendTo('#selAll');}")
        body.Append("$('#txtAllItems').on('change paste keyup', function() {var theText = $(this).val(); if (theText.length != 0 && event.which == 13){$('#selAll option:selected').prop('selected', false); var select = $('#selAll'); event.preventDefault(); var option = select.find(""option:contains("" + theText + "")""); option.prop('selected', true); var optionTop = option.offset().top;var selectTop = select.offset().top;select.scrollTop(select.scrollTop() + (optionTop - selectTop));}});")
        body.Append("$('#txtSelectedItems').on('change paste keyup', function() {var theText = $(this).val(); if (theText.length != 0 && event.which == 13){$('#selSelected option:selected').prop('selected', false); var select = $('#selSelected'); event.preventDefault(); var option = select.find(""option:contains("" + theText + "")""); option.prop('selected', true); var optionTop = option.offset().top;var selectTop = select.offset().top;select.scrollTop(select.scrollTop() + (optionTop - selectTop));}});")
        body.Append("function saveTaxItems() {$('#selSelected option').prop('selected', true); var valJson = JSON.stringify($('#frmItems').serializeArray());var dataJson = { Fields: valJson };var dataJsonString = JSON.stringify(dataJson);$.ajax({type: 'POST',url: '../Stock/ajax.aspx/changeTaxItems',data: dataJsonString,contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {if (response.d.indexOf('Err:') >= 0) {msg('',response.d.substring(4, response.d.length),'error');} else {$('#prtJS').html(response.d);}},failure: function (msg) {alert(msg);},error: function (xhr, ajaxOptions, thrownError) {alert(' write json item, Ajax error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});}")
        body.Append("</script>")

        Dim sh As New Share.UI
        Return sh.drawModal(Header, body.ToString, "<button type=""button"" class=""btn btn-success"" onclick=""javascript:saveTaxItems();""><i class=""icon-save""></i> " & btnSave & "</button> <button type=""button"" class=""btn btn-secondary"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnClose & "</button>", Share.UI.ModalSize.Medium, "", "p-0")
    End Function

    Public Function changeTaxItems(ByVal Fields As String) As String
        Dim lstItems As String = ""

        Select Case byteLanguage
            Case 2
                DataLang = "Ar"
            Case Else
                DataLang = "En"
        End Select

        ' get form values
        Dim jss As New JavaScriptSerializer()
        Dim field() As NameValue = jss.Deserialize(Of NameValue())(Fields)
        For I = 0 To field.Length - 1
            Select Case field(I).name()
                Case "drpSelected"
                    lstItems = lstItems & "'" & field(I).value() & "',"
            End Select
        Next
        If lstItems.Length > 0 Then lstItems = Left(lstItems, lstItems.Length - 1)

        Try
            dcl.ExecScalar("UPDATE Stock_Items SET bTax=0 WHERE bTax=1")
            If lstItems.Length > 0 Then dcl.ExecScalar("UPDATE Stock_Items SET bTax=1 WHERE strItem IN (" & lstItems & ")")
        Catch ex As Exception
            Return "Err:" & ex.Message
        End Try

        Return "<script>msg('', 'Items have changed successfully!', 'success'); $('#mdlConfirm').modal('hide');</script>"
    End Function

    Public Function changeAvailablity() As String
        Dim btnClose, btnSave As String
        Dim Header, lblAll, lblSelect As String
        Dim plcFind As String

        Select Case byteLanguage
            Case 2
                DataLang = "Ar"

                Header = "تغيير تمكين الأصناف"
                lblSelect = "كل الاصناف"
                lblAll = "الاصناف المحددة"
                plcFind = "ابحث عن صنف في القائمة"
                btnSave = "حفظ التغييرات"
                btnClose = "إغلاق"
            Case Else
                DataLang = "En"
                lblSelect = "selected Items"
                lblAll = "All Item"
                plcFind = "find an item in the list"
                Header = "Change items Availablity"
                btnSave = "Save Changes"
                btnClose = "Close"
        End Select

        Dim body As New StringBuilder("")

        body.Append("<div class=""row"">")
        body.Append("<form id=""frmItems"">")
        body.Append("<div class=""col-md-12 p-1"">")

        body.Append("<div class=""col-md-5 ml-0 mr-0 pl-0 pr-0"">")
        body.Append("<h5 class=""text-md-center"">" & lblAll & "</h5>")
        body.Append("<select class=""form-control font-small-3"" size=""10"" id=""selAll"" multiple=""multiple"" style=""height:280px; overflow-y:auto"">")
        Dim dsUnlimit As DataSet = dcl.GetDS("SELECT * FROM Stock_Items WHERE strParentItem > 0 AND (bEnabled = 0 OR bEnabled IS NULL)")
        For I = 0 To dsUnlimit.Tables(0).Rows.Count - 1
            body.Append("<option value=""" & dsUnlimit.Tables(0).Rows(I).Item("strItem") & """ title=""" & dsUnlimit.Tables(0).Rows(I).Item("strItem" & DataLang) & """>" & dsUnlimit.Tables(0).Rows(I).Item("strItem") & "- " & dsUnlimit.Tables(0).Rows(I).Item("strItem" & DataLang) & "</option>")
        Next
        body.Append("</select>")
        body.Append("<input type=""text"" class=""form-control"" id=""txtAllItems"" placeholder=""" & plcFind & """ />")
        body.Append("</div>")

        body.Append("<div class=""col-md-2 ml-0 mr-0 pl-0 pr-0"">")
        body.Append("<h5></h5>")
        body.Append("<br />")
        body.Append("<br />")
        body.Append("<br />")
        body.Append("<div class=""pl-1 pr-1""><button type=""button"" class=""btn btn-sm btn-success full-width"" onclick=""javascript:move2selected();""><i class=""icon icon-angle-right""></i> </button></div>")
        body.Append("<br />")
        body.Append("<div class=""pl-1 pr-1""><button type=""button"" class=""btn btn-sm btn-success full-width"" onclick=""javascript:moveAll2selected();""><i class=""icon icon-angle-double-right""></i> </button></div>")
        body.Append("<br />")
        body.Append("<div class=""pl-1 pr-1""><button type=""button"" class=""btn btn-sm btn-success full-width"" onclick=""javascript:move2all();""><i class=""icon icon-angle-left""></i> </button></div>")
        body.Append("<br />")
        body.Append("<div class=""pl-1 pr-1""><button type=""button"" class=""btn btn-sm btn-success full-width"" onclick=""javascript:moveAll2all();""><i class=""icon icon-angle-double-left""></i> </button></div>")
        body.Append("</div>")

        body.Append("<div class=""col-md-5 ml-0 mr-0 pl-0 pr-0"">")
        body.Append("<h5 class=""text-md-center"">" & lblSelect & "</h5>")
        body.Append("<select class=""form-control font-small-3"" size=""10"" id=""selSelected"" name=""drpSelected"" multiple=""multiple"" style=""height:280px; overflow-y:auto"">")
        Dim dslimit As DataSet = dcl.GetDS("SELECT * FROM Stock_Items WHERE bEnabled=1")
        For I = 0 To dslimit.Tables(0).Rows.Count - 1
            body.Append("<option value=""" & dslimit.Tables(0).Rows(I).Item("strItem") & """ title=""" & dslimit.Tables(0).Rows(I).Item("strItem" & DataLang) & """>" & dslimit.Tables(0).Rows(I).Item("strItem") & "- " & dslimit.Tables(0).Rows(I).Item("strItem" & DataLang) & "</option>")
        Next
        body.Append("</select>")
        body.Append("<input type=""text"" class=""form-control"" id=""txtSelectedItems"" placeholder=""" & plcFind & """ />")
        body.Append("</div>")

        body.Append("</div>")
        body.Append("</form>")
        body.Append("</div>")

        body.Append("<script>")
        body.Append("function move2selected() {$('#selAll option:selected').remove().appendTo('#selSelected');} function move2all() {$('#selSelected option:selected').remove().appendTo('#selAll');}")
        body.Append("function moveAll2selected() {$('#selAll option').remove().appendTo('#selSelected');} function moveAll2all() {$('#selSelected option').remove().appendTo('#selAll');}")
        body.Append("$('#txtAllItems').on('change paste keyup', function() {var theText = $(this).val(); if (theText.length != 0 && event.which == 13){$('#selAll option:selected').prop('selected', false); var select = $('#selAll'); event.preventDefault(); var option = select.find(""option:contains("" + theText + "")""); option.prop('selected', true); var optionTop = option.offset().top;var selectTop = select.offset().top;select.scrollTop(select.scrollTop() + (optionTop - selectTop));}});")
        body.Append("$('#txtSelectedItems').on('change paste keyup', function() {var theText = $(this).val(); if (theText.length != 0 && event.which == 13){$('#selSelected option:selected').prop('selected', false); var select = $('#selSelected'); event.preventDefault(); var option = select.find(""option:contains("" + theText + "")""); option.prop('selected', true); var optionTop = option.offset().top;var selectTop = select.offset().top;select.scrollTop(select.scrollTop() + (optionTop - selectTop));}});")
        body.Append("function saveAvailablityItems() {$('#selSelected option').prop('selected', true); var valJson = JSON.stringify($('#frmItems').serializeArray());var dataJson = { Fields: valJson };var dataJsonString = JSON.stringify(dataJson);$.ajax({type: 'POST',url: '../Stock/ajax.aspx/changeAvailablityItems',data: dataJsonString,contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {if (response.d.indexOf('Err:') >= 0) {msg('',response.d.substring(4, response.d.length),'error');} else {$('#prtJS').html(response.d);}},failure: function (msg) {alert(msg);},error: function (xhr, ajaxOptions, thrownError) {alert(' write json item, Ajax error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});}")
        body.Append("</script>")

        Dim sh As New Share.UI
        Return sh.drawModal(Header, body.ToString, "<button type=""button"" class=""btn btn-success"" onclick=""javascript:saveAvailablityItems();""><i class=""icon-save""></i> " & btnSave & "</button> <button type=""button"" class=""btn btn-secondary"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnClose & "</button>", Share.UI.ModalSize.Medium, "", "p-0")
    End Function

    Public Function changeAvailablityItems(ByVal Fields As String) As String
        Dim lstItems As String = ""

        Select Case byteLanguage
            Case 2
                DataLang = "Ar"
            Case Else
                DataLang = "En"
        End Select

        ' get form values
        Dim jss As New JavaScriptSerializer()
        Dim field() As NameValue = jss.Deserialize(Of NameValue())(Fields)
        For I = 0 To field.Length - 1
            Select Case field(I).name()
                Case "drpSelected"
                    lstItems = lstItems & "'" & field(I).value() & "',"
            End Select
        Next
        If lstItems.Length > 0 Then lstItems = Left(lstItems, lstItems.Length - 1)

        Try
            dcl.ExecScalar("UPDATE Stock_Items SET bEnabled=0 WHERE bEnabled=1")
            If lstItems.Length > 0 Then dcl.ExecScalar("UPDATE Stock_Items SET bEnabled=1 WHERE strItem IN (" & lstItems & ")")
        Catch ex As Exception
            Return "Err:" & ex.Message
        End Try

        Return "<script>msg('', 'Items have changed successfully!', 'success'); $('#mdlConfirm').modal('hide');</script>"
    End Function

    Public Function viewTransfer(ByVal lngTransaction As Long, ByVal ReceiveMode As Boolean) As String
        Dim lblVoucherNo, lblType, lblWarehouseFrom, lblWarehouseTo, lblDate, lblSender, lblRecipient, lblStatus As String
        Dim txtSendVoucher, txtSendDate, drpSendType, txtSendWarehouse, drpSendWarehouse, txtSendUser, txtSendStatus As String
        Dim txtReceiveVoucher, txtReceiveDate, drpReceiveType, txtReceiveWarehouse, drpReceiveWarehouse, txtReceiveUser, txtReceiveStatus As String
        Dim txtRemarks As String
        Dim plcBarcode, plcRemarks As String
        Dim Header, lblTransferFrom, lblTransferTo As String
        Dim Pending, Approved, Cancelled As String
        Dim tabItems, tabTransfer, tabRemarks As String
        Dim colNo, colItem, colItemName, colExpire, colQuantity, colPrice As String

        Dim btnNew, btnClose, btnSave, btnApproveSend, btnApproveReceive, btnCancel, btnPrint, btnAdd, btnDelete, btnReturn As String
        Select Case byteLanguage
            Case 2
                DataLang = "Ar"

                Pending = "في الانتظار"
                Approved = "معمدة"
                Cancelled = "ملغية"

                Header = "تحويل أصناف إلى مستودع"

                tabTransfer = "تحويل"
                tabItems = "الأصناف"
                tabRemarks = "الملاحظات"

                colNo = "رقم"
                colItem = "الصنف"
                colItemName = "اسم الصنف"
                colExpire = "الانتهاء"
                colQuantity = "الكمية"
                colPrice = "السعر"

                lblTransferFrom = "من"
                lblTransferTo = "إلى"

                lblVoucherNo = "رقم السند"
                lblType = "نوع السند"
                lblWarehouseFrom = "من"
                lblWarehouseTo = "إلى"
                lblDate = "التاريخ"
                lblSender = "المرسل"
                lblRecipient = "المستلم"
                lblStatus = "الحالة"

                btnNew = "تحويل جديد"
                btnSave = "حفظ التغييرات"
                btnApproveSend = "تعميد تسليم"
                btnApproveReceive = "تعميد استلام"
                btnCancel = "إلغاء"
                btnClose = "إغلاق"
                btnPrint = "طباعة"
                btnAdd = "إضافة"
                btnDelete = "حذف"
                btnReturn = "إرجاع الأصناف"
            Case Else
                DataLang = "En"

                Pending = "Pending"
                Approved = "Approved"
                Cancelled = "Cancelled"

                Header = "Transfer items to warehouse"

                tabTransfer = "Transfer"
                tabItems = "Items"
                tabRemarks = "Remarks"

                colNo = "No"
                colItem = "Item"
                colItemName = "Item Name"
                colExpire = "Expire"
                colQuantity = "Quantity"
                colPrice = "Price"

                lblTransferFrom = "From"
                lblTransferTo = "To"

                lblVoucherNo = "Voucher No"
                lblType = "Voucher Type"
                lblWarehouseFrom = "From"
                lblWarehouseTo = "To"
                lblDate = "Date"
                lblSender = "Sender"
                lblRecipient = "Recipient"
                lblStatus = "Status"

                btnNew = "New Transfer"
                btnSave = "Save Changes"
                btnApproveSend = "Approve Send"
                btnApproveReceive = "Approve Receive"
                btnCancel = "Cancel"
                btnClose = "Close"
                btnPrint = "Print"
                btnAdd = "Add"
                btnDelete = "Delete"
                btnReturn = "Return Items"
        End Select

        Dim body As New StringBuilder("")

        Dim EditMode As Boolean = False
        Dim ApproveMode As Boolean = False
        Dim ApproveSendEnabled As Boolean = False
        Dim ApproveReceiveEnabled As Boolean = False
        Dim CancelEnabled As Boolean = False

        Try
            If lngTransaction > 0 Then
                Dim ds As DataSet
                'ds = dcl.GetDS("SELECT T.lngTransaction, T.strTransaction, T.dateTransaction, strType" & DataLang & ", strWarehouse" & DataLang & ", strCreatedBy, T.byteBase, T.byteStatus FROM Stock_Trans AS T INNER JOIN Stock_Warehouses AS W ON T.byteWarehouse=W.byteWarehouse INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType=TT.byteTransType INNER JOIN Stock_Trans_Audit AS TA ON T.lngTransaction=TA.lngTransaction WHERE T.lngTransaction=" & lngTransaction)
                'ds = dcl.GetDS("SELECT T1.lngTransaction, T1.strTransaction AS strSendVoucher, T2.strTransaction AS strReceiveVoucher, T1.dateTransaction AS dateSendDate, T2.dateTransaction AS dateReceiveDate, TT1.strType" & DataLang & " AS strSendType, TT2.strType" & DataLang & " AS strReceiveType, W1.strWarehouse" & DataLang & " AS strSendWarehouse, W2.strWarehouse" & DataLang & " AS strReceiveWarehouse, TA1.strCreatedBy AS strSendUser, TA2.strCreatedBy AS strReceiveUser, T1.byteStatus AS byteSendStatus, T2.byteStatus AS byteReceiveStatus FROM Stock_Trans AS T1 LEFT JOIN Stock_Trans AS T2 ON T1.strTransaction=T2.strTransaction AND T1.lngTransaction<>T2.lngTransaction INNER JOIN Stock_Warehouses AS W1 ON T1.byteWarehouse=W1.byteWarehouse INNER JOIN Stock_Warehouses AS W2 ON T2.byteWarehouse=W2.byteWarehouse INNER JOIN Stock_Trans_Types AS TT1 ON T1.byteTransType=TT1.byteTransType INNER JOIN Stock_Trans_Types AS TT2 ON T2.byteTransType=TT2.byteTransType INNER JOIN Stock_Trans_Audit AS TA1 ON T1.lngTransaction=TA1.lngTransaction INNER JOIN Stock_Trans_Audit AS TA2 ON T2.lngTransaction=TA2.lngTransaction WHERE YEAR(T1.dateTransaction) = " & intYear & " AND YEAR(T2.dateTransaction) = " & intYear & " AND T1.byteBase=19 AND T2.byteBase=20 AND T1.lngTransaction=" & lngTransaction)
                ds = dcl.GetDS("SELECT T1.lngTransaction, T1.strTransaction AS strSendVoucher, T2.strTransaction AS strReceiveVoucher, T1.dateTransaction AS dateSendDate, T2.dateTransaction AS dateReceiveDate, TT1.strType" & DataLang & " AS strSendType, TT2.strType" & DataLang & " AS strReceiveType, W1.byteWarehouse AS byteSendWarehouse, W1.strWarehouse" & DataLang & " AS strSendWarehouse, W2.byteWarehouse AS byteReceiveWarehouse, W2.strWarehouse" & DataLang & " AS strReceiveWarehouse, TA1.strCreatedBy AS strSendUser, TA2.strCreatedBy AS strReceiveUser, T1.byteStatus AS byteSendStatus, T2.byteStatus AS byteReceiveStatus, T1.strRemarks FROM Stock_Xlink AS X1 LEFT JOIN Stock_Xlink AS X2 ON X1.lngPointer=X2.lngPointer INNER JOIN Stock_Trans AS T1 ON T1.lngTransaction=X1.lngTransaction INNER JOIN Stock_Trans AS T2 ON T2.lngTransaction=X2.lngTransaction INNER JOIN Stock_Warehouses AS W1 ON T1.byteWarehouse=W1.byteWarehouse INNER JOIN Stock_Warehouses AS W2 ON T2.byteWarehouse=W2.byteWarehouse INNER JOIN Stock_Trans_Types AS TT1 ON T1.byteTransType=TT1.byteTransType INNER JOIN Stock_Trans_Types AS TT2 ON T2.byteTransType=TT2.byteTransType INNER JOIN Stock_Trans_Audit AS TA1 ON T1.lngTransaction=TA1.lngTransaction LEFT JOIN Stock_Trans_Audit AS TA2 ON T2.lngTransaction=TA2.lngTransaction WHERE T1.byteBase=19 AND T2.byteBase=20 AND T1.lngTransaction=" & lngTransaction)
                If ds.Tables(0).Rows.Count > 0 Then
                    txtSendVoucher = ds.Tables(0).Rows(0).Item("strSendVoucher")
                    drpSendType = ds.Tables(0).Rows(0).Item("strSendType")
                    txtSendWarehouse = ds.Tables(0).Rows(0).Item("strSendWarehouse")
                    drpSendWarehouse = ds.Tables(0).Rows(0).Item("byteSendWarehouse")
                    txtSendUser = ds.Tables(0).Rows(0).Item("strSendUser")
                    txtSendDate = CDate(ds.Tables(0).Rows(0).Item("dateSendDate")).ToString(strDateFormat)
                    txtRemarks = ds.Tables(0).Rows(0).Item("strRemarks").ToString

                    Select Case ds.Tables(0).Rows(0).Item("byteSendStatus").ToString
                        Case 0
                            txtSendStatus = Cancelled
                            EditMode = False
                            ApproveSendEnabled = False
                            CancelEnabled = False
                            ApproveMode = False
                        Case 1
                            txtSendStatus = Pending
                            EditMode = True
                            ApproveSendEnabled = True
                            CancelEnabled = True
                            ApproveMode = False
                        Case 2
                            txtSendStatus = Approved
                            ApproveMode = True
                            'If ds.Tables(0).Rows(0).Item("byteReceiveStatus").ToString = 2 Or ReceiveMode = False Then ApproveMode = False Else ApproveMode = True
                            If ds.Tables(0).Rows(0).Item("byteReceiveStatus").ToString = 1 And ReceiveMode = False Then ApproveMode = True Else ApproveMode = False
                        Case Else
                            txtSendStatus = ""
                            EditMode = False
                            ApproveSendEnabled = False
                            CancelEnabled = True
                            ApproveMode = False
                    End Select

                    If IsDBNull(ds.Tables(0).Rows(0).Item("strReceiveVoucher")) Then txtReceiveVoucher = "" Else txtReceiveVoucher = ds.Tables(0).Rows(0).Item("strReceiveVoucher")
                    If IsDBNull(ds.Tables(0).Rows(0).Item("strReceiveType")) Then drpReceiveType = "" Else drpReceiveType = ds.Tables(0).Rows(0).Item("strReceiveType")
                    If IsDBNull(ds.Tables(0).Rows(0).Item("strReceiveWarehouse")) Then txtReceiveWarehouse = "" Else txtReceiveWarehouse = ds.Tables(0).Rows(0).Item("strReceiveWarehouse")
                    If IsDBNull(ds.Tables(0).Rows(0).Item("byteReceiveWarehouse")) Then drpReceiveWarehouse = "" Else drpReceiveWarehouse = ds.Tables(0).Rows(0).Item("byteReceiveWarehouse")
                    If IsDBNull(ds.Tables(0).Rows(0).Item("strReceiveUser")) Then txtReceiveUser = "" Else txtReceiveUser = ds.Tables(0).Rows(0).Item("strReceiveUser")
                    If IsDBNull(ds.Tables(0).Rows(0).Item("dateReceiveDate")) Then txtReceiveDate = "" Else txtReceiveDate = CDate(ds.Tables(0).Rows(0).Item("dateReceiveDate")).ToString(strDateFormat)

                    Select Case ds.Tables(0).Rows(0).Item("byteReceiveStatus").ToString
                        Case 0
                            txtReceiveStatus = Cancelled
                            ApproveReceiveEnabled = False
                        Case 1
                            txtReceiveStatus = Pending
                            If ds.Tables(0).Rows(0).Item("byteSendStatus").ToString = 2 And ReceiveMode = True Then ApproveReceiveEnabled = True Else ApproveReceiveEnabled = False
                        Case 2
                            txtReceiveStatus = Approved
                            ApproveReceiveEnabled = False
                        Case Else
                            txtReceiveStatus = ""
                            ApproveReceiveEnabled = False
                    End Select

                    'Dim dsOther As DataSet
                    'If ds.Tables(0).Rows(0).Item("byteBase") = 20 Then
                    '    dsOther = dcl.GetDS("SELECT * FROM Stock_Trans AS T INNER JOIN Stock_Warehouses AS W ON T.byteWarehouse=W.byteWarehouse WHERE T.strTransaction = '" & txtVoucherNo & "' AND T.byteBase=19")
                    'Else
                    '    dsOther = dcl.GetDS("SELECT * FROM Stock_Trans AS T INNER JOIN Stock_Warehouses AS W ON T.byteWarehouse=W.byteWarehouse WHERE T.strTransaction = '" & txtVoucherNo & "' AND T.byteBase=20")
                    'End If
                    'drpWarehouseTo = dsOther.Tables(0).Rows(0).Item("strWarehouse" & DataLang)

                    txtSendVoucher = "<input type=""text"" id=""txtSendVoucher"" name=""txtSendVoucher"" class=""form-control form-control-sm input-sm text-md-center"" readonly=""readonly"" value=""" & txtSendVoucher & """ />"
                    txtReceiveVoucher = "<input type=""text"" id=""txtReceiveVoucher"" name=""txtReceiveVoucher"" class=""form-control form-control-sm input-sm text-md-center"" readonly=""readonly"" value=""" & txtReceiveVoucher & """ />"
                    txtSendDate = "<input type=""text"" id=""txtSendDate"" name=""txtSendDate"" class=""form-control form-control-sm input-sm text-md-center"" readonly=""readonly"" value=""" & txtSendDate & """ />"
                    txtReceiveDate = "<input type=""text"" id=""txtReceiveDate"" name=""txtReceiveDate"" class=""form-control form-control-sm input-sm text-md-center"" readonly=""readonly"" value=""" & txtReceiveDate & """ />"
                    drpSendType = "<input type=""hidden"" id=""drpSendType"" name=""drpSendType"" value=""14"" /><input type=""text"" class=""form-control form-control-sm input-sm"" readonly=""readonly"" value=""" & drpSendType & """ />"
                    drpReceiveType = "<input type=""hidden"" id=""drpReceiveType"" name=""drpReceiveType"" value=""15"" /><input type=""text"" class=""form-control form-control-sm input-sm"" readonly=""readonly"" value=""" & drpReceiveType & """ />"
                    drpSendWarehouse = "<input type=""hidden"" id=""drpSendWarehouse"" name=""drpSendWarehouse"" value=""" & drpSendWarehouse & """ /><input type=""text"" class=""form-control form-control-sm input-sm"" readonly=""readonly"" value=""" & txtSendWarehouse & """ />"
                    drpReceiveWarehouse = "<input type=""hidden"" id=""drpReceiveWarehouse"" name=""drpReceiveWarehouse"" value=""" & drpReceiveWarehouse & """ /><input type=""text"" class=""form-control form-control-sm input-sm"" readonly=""readonly"" value=""" & txtReceiveWarehouse & """ />"
                    txtSendUser = "<input type=""text"" id=""txtSendUser"" name=""txtSendUser"" class=""form-control form-control-sm input-sm text-md-center"" readonly=""readonly"" value=""" & txtSendUser & """ />"
                    txtReceiveUser = "<input type=""text"" id=""txtReceiveUser"" name=""txtReceiveUser"" class=""form-control form-control-sm input-sm text-md-center"" readonly=""readonly"" value=""" & txtReceiveUser & """ />"
                    txtSendStatus = "<input type=""text"" id=""txtSendStatus"" name=""txtSendStatus"" class=""form-control form-control-sm input-sm text-md-center"" readonly=""readonly"" value=""" & txtSendStatus & """ />"
                    txtReceiveStatus = "<input type=""text"" id=""txtReceiveStatus"" name=""txtReceiveStatus"" class=""form-control form-control-sm input-sm text-md-center"" readonly=""readonly"" value=""" & txtReceiveStatus & """ />"

                    txtRemarks = "<textarea id=""txtRemarks"" class=""form-control height-300"" name=""txtRemarks"" placeholder=""About Transfer"" readonly=""readonly"">" & txtRemarks & "</textarea>"
                Else
                    Return "Err: Record not found"
                End If
            Else
                EditMode = True
                ApproveMode = False
                ApproveSendEnabled = False
                ApproveReceiveEnabled = False
                CancelEnabled = False
                Dim dsLast As DataSet = dcl.GetDS("SELECT MAX(CAST(strTransaction AS bigint)) AS Last FROM Stock_Trans WHERE YEAR(dateTransaction) = " & intYear & " AND byteBase=19")
                Dim lngNewVoucher = dsLast.Tables(0).Rows(0).Item("Last")

                Dim dsType As DataSet = dcl.GetDS("SELECT * FROM Stock_Trans_Types WHERE byteTransType=14")
                Dim TypeName As String = dsType.Tables(0).Rows(0).Item("strType" & DataLang)

                Dim CurrentWarehouseID As Byte = 0
                Dim CurrectWarehouse, OtherWarehouse As String
                CurrectWarehouse = ""
                OtherWarehouse = ""

                Dim where As String = ""
                Dim WarehouseList As String = ""
                If TransferString <> "*" Then where = "AND byteWarehouse IN (" & TransferString & ")"
                Dim dsWarehouse As DataSet = dcl.GetDS("SELECT * FROM Stock_Warehouses WHERE bActive=1 " & where)
                For I = 0 To dsWarehouse.Tables(0).Rows.Count - 1
                    WarehouseList = WarehouseList & "<option value=""" & dsWarehouse.Tables(0).Rows(I).Item("byteWarehouse") & """>" & dsWarehouse.Tables(0).Rows(I).Item("strWarehouse" & DataLang) & "</option>"
                Next

                Dim WarehouseList2 As String = ""
                Dim dsWarehouse2 As DataSet = dcl.GetDS("SELECT * FROM Stock_Warehouses WHERE bActive=1")
                For I = 0 To dsWarehouse.Tables(0).Rows.Count - 1
                    'If dsWarehouse.Tables(0).Rows(I).Item("byteWarehouse") = byteWarehouse Then
                    '    CurrentWarehouseID = dsWarehouse.Tables(0).Rows(I).Item("byteWarehouse")
                    '    CurrectWarehouse = dsWarehouse.Tables(0).Rows(I).Item("strWarehouse" & DataLang)
                    'Else
                    '    OtherWarehouse = OtherWarehouse & "<option value=""" & dsWarehouse.Tables(0).Rows(I).Item("byteWarehouse") & """>" & dsWarehouse.Tables(0).Rows(I).Item("strWarehouse" & DataLang) & "</option>"
                    'End If
                    WarehouseList2 = WarehouseList2 & "<option value=""" & dsWarehouse2.Tables(0).Rows(I).Item("byteWarehouse") & """>" & dsWarehouse2.Tables(0).Rows(I).Item("strWarehouse" & DataLang) & "</option>"
                Next

                txtSendVoucher = "<input type=""text"" id=""txtSendVoucher"" name=""txtSendVoucher"" class=""form-control form-control-sm input-sm text-md-center"" readonly=""readonly"" value=""" & lngNewVoucher & """ />"
                txtReceiveVoucher = "<input type=""text"" id=""txtReceiveVoucher"" name=""txtReceiveVoucher"" class=""form-control form-control-sm input-sm text-md-center"" readonly=""readonly"" value="""" />"
                txtSendDate = "<input type=""text"" id=""txtSendDate"" name=""txtSendDate"" class=""form-control form-control-sm input-sm text-md-center"" readonly=""readonly"" value=""" & Today.ToString(strDateFormat) & """ />"
                txtReceiveDate = "<input type=""text"" id=""txtReceiveDate"" name=""txtReceiveDate"" class=""form-control form-control-sm input-sm text-md-center"" readonly=""readonly"" value="""" />"
                drpSendType = "<input type=""hidden"" id=""drpSendType"" name=""drpSendType"" value=""14"" /><input type=""text"" class=""form-control form-control-sm input-sm"" readonly=""readonly"" value=""" & TypeName & """ />"
                drpReceiveType = "<input type=""hidden"" id=""drpReceiveType"" name=""drpReceiveType"" value=""15"" /><input type=""text"" class=""form-control form-control-sm input-sm"" readonly=""readonly"" value="""" />"
                'drpSendWarehouse = "<input type=""hidden"" id=""drpSendWarehouse"" name=""drpSendWarehouse"" value=""" & CurrentWarehouseID & """ /><input type=""text"" class=""form-control form-control-sm"" readonly=""readonly"" value=""" & CurrectWarehouse & """ />"
                drpSendWarehouse = "<select id=""drpSendWarehouse"" name=""drpSendWarehouse"" class=""form-control form-control-sm input-sm"">" & WarehouseList & "</select>"
                drpReceiveWarehouse = "<select id=""drpReceiveWarehouse"" name=""drpReceiveWarehouse"" class=""form-control form-control-sm input-sm"">" & WarehouseList2 & "</select>"
                txtSendUser = "<input type=""text"" id=""txtSendUser"" name=""txtSendUser"" class=""form-control form-control-sm input-sm text-md-center"" readonly=""readonly"" value=""" & strUserName & """ />"
                txtReceiveUser = "<input type=""text"" id=""txtReceiveUser"" name=""txtReceiveUser"" class=""form-control form-control-sm input-sm text-md-center"" readonly=""readonly"" value="""" />"
                txtSendStatus = "<input type=""hidden"" id=""txtSendStatus"" name=""txtSendStatus"" value=""1"" /><input type=""text"" class=""form-control form-control-sm input-sm text-md-center"" readonly=""readonly"" value=""" & Pending & """ />"
                txtReceiveStatus = "<input type=""text"" id=""txtReceiveStatus"" name=""txtReceiveStatus"" class=""form-control form-control-sm input-sm text-md-center"" readonly=""readonly"" value="""" />"

                txtRemarks = "<textarea id=""txtRemarks"" class=""form-control height-300"" name=""txtRemarks"" placeholder=""About Transfer"">" & txtRemarks & "</textarea>"
            End If
        Catch ex As Exception
            Return "Err:" & ex.Message
        End Try

        Try
            'tabs
            body.Append("<form id=""frmVoucher"" style=""height:400px"">")
            body.Append("<ul class=""nav nav-tabs m-0"">")
            body.Append("<li class=""nav-item""><a class=""nav-link active"" id=""base-tab1"" data-toggle=""tab"" aria-controls=""tab1"" href=""#tab1"" aria-expanded=""true"" >" & tabTransfer & "</a></li>")
            body.Append("<li class=""nav-item""><a class=""nav-link"" id=""base-tab2"" data-toggle=""tab"" aria-controls=""tab2"" href=""#tab2"" aria-expanded=""true"" >" & tabItems & "</a></li>")
            body.Append("<li class=""nav-item""><a class=""nav-link"" id=""base-tab3"" data-toggle=""tab"" aria-controls=""tab3"" href=""#tab3"" aria-expanded=""true"" >" & tabRemarks & "</a></li>")
            body.Append("</ul>")
            'contents ==> Open
            body.Append("<div class=""tab-content m-0 p-1"">")
            'content 1
            body.Append("<div role=""tabpanel"" class=""tab-pane active"" id=""tab1"" aria-expanded=""true"" aria-labelledby=""base-tab1"">")
            body.Append("<form id=""frmItem"">")
            body.Append("<div class=""row"">")
            body.Append("<div class=""col-md-6"">")
            body.Append("<div class=""col-md-12 border border-cyan border-lighten-4 p-0 pb-1"">")
            body.Append("<div class=""col-md-12 bg-cyan bg-lighten-4 blue""><div class=""font-medium-3"">" & lblTransferFrom & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblVoucherNo & ":</div><div class=""col-md-4"">" & txtSendVoucher & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblType & ":</div><div class=""col-md-8"">" & drpSendType & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblWarehouseFrom & ":</div><div class=""col-md-8"">" & drpSendWarehouse & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblDate & ":</div><div class=""col-md-4"">" & txtSendDate & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblSender & ":</div><div class=""col-md-4"">" & txtSendUser & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblStatus & ":</div><div class=""col-md-4"">" & txtSendStatus & "</div></div>")
            body.Append("</div>")
            body.Append("</div>")
            body.Append("<div class=""col-md-6"">")
            body.Append("<div class=""col-md-12 border border-orange  border-lighten-4 p-0 pb-1"">")
            body.Append("<div class=""col-md-12 bg-orange  bg-lighten-4 orange""><div class=""font-medium-3"">" & lblTransferTo & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblVoucherNo & ":</div><div class=""col-md-4"">" & txtReceiveVoucher & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblType & ":</div><div class=""col-md-8"">" & drpReceiveType & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblWarehouseTo & ":</div><div class=""col-md-8"">" & drpReceiveWarehouse & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblDate & ":</div><div class=""col-md-4"">" & txtReceiveDate & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblRecipient & ":</div><div class=""col-md-4"">" & txtReceiveUser & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblStatus & ":</div><div class=""col-md-4"">" & txtReceiveStatus & "</div></div>")
            body.Append("</div>")
            body.Append("</div>")
            body.Append("</div>")
            body.Append("</form>")
            body.Append("</div>")
            'content 2
            body.Append("<div role=""tabpanel"" class=""tab-pane"" id=""tab2"" aria-expanded=""true"" aria-labelledby=""base-tab2"">")
            body.Append("<div class=""row"">")
            body.Append("<div class=""p-0 m-0"">")
            body.Append("<table class=""table table-bordered p-0 m-0""><thead>")
            body.Append("<tr><th style=""width:30px""></th><th style=""width:50px"">" & colNo & "</th><th style=""width:70px""><center>" & colItem & "</center></th><th style=""width:300px""><center>" & colItemName & "</center></th><th style=""width:100px""><center>" & colExpire & "</center></th><th style=""width:70px""><center>" & colQuantity & "</center></th><th style=""width:70px""><center>" & colPrice & "</center></th><th><center></center></th></tr>")
            body.Append("</thead></table>")
            body.Append("</div>")
            body.Append("<div class=""p-0 m-0"" style=""height:253px; overflow-x:auto;"">")
            body.Append("<table id=""tblItems"" class=""table table-bordered p-0 m-0""><tbody>")
            Dim dsItems As DataSet = dcl.GetDS("SELECT * FROM Stock_Xlink AS X INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink=XI.lngXlink INNER JOIN Stock_Items AS I ON XI.strItem=I.strItem WHERE X.lngTransaction=" & lngTransaction)
            Dim ItemName As String
            Dim Counter As Integer = 0
            Dim TotalQuantity, TotalItem, TotalPrice As Decimal
            For I = 0 To dsItems.Tables(0).Rows.Count - 1
                If Len(dsItems.Tables(0).Rows(I).Item("strItem" & DataLang)) > 27 Then ItemName = Mid(dsItems.Tables(0).Rows(I).Item("strItem" & DataLang), 1, 27) & "..." Else ItemName = dsItems.Tables(0).Rows(I).Item("strItem" & DataLang)
                body.Append("<tr>")
                body.Append("<td style=""width:30px""></td>")
                body.Append("<td style=""width:50px"">" & dsItems.Tables(0).Rows(I).Item("intEntryNumber") & "</td>")
                body.Append("<td style=""width:70px"">" & dsItems.Tables(0).Rows(I).Item("strItem") & "</td>")
                body.Append("<td style=""width:300px"" class=""text-md-left"" title=""" & dsItems.Tables(0).Rows(I).Item("strItem" & DataLang) & """>" & ItemName & "</td>")
                body.Append("<td style=""width:100px"">" & CDate(dsItems.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM-dd") & "</td>")
                body.Append("<td style=""width:70px"">" & Math.Round(dsItems.Tables(0).Rows(I).Item("curQuantity"), 0, MidpointRounding.AwayFromZero) & "</td>")
                body.Append("<td style=""width:70px"">" & Math.Round(dsItems.Tables(0).Rows(I).Item("curUnitPrice"), byteCurrencyRound, MidpointRounding.AwayFromZero) & "</td>")
                If EditMode = True Then
                    body.Append("<td><button type=""button"" class=""btn btn-danger btn-xs"" onclick=""javascript:removeThis(this)"">Delete</button><input type=""hidden"" id=""item" & Counter & """ class=""item"" name=""item"" value=""" & dsItems.Tables(0).Rows(I).Item("strItem") & """ /><input type=""hidden"" name=""expiry"" value=""" & CDate(dsItems.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM-dd") & """ /><input type=""hidden"" id=""quantity" & Counter & """ name=""quantity"" class=""quantity"" value=""" & Math.Round(dsItems.Tables(0).Rows(I).Item("curQuantity"), 0, MidpointRounding.AwayFromZero) & """ /><input type=""hidden"" name=""price"" value=""" & Math.Round(dsItems.Tables(0).Rows(I).Item("curUnitPrice"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """ /><input type=""hidden"" name=""cost"" value=""" & Math.Round(dsItems.Tables(0).Rows(I).Item("curUnitNetCost"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """ /></td>")
                Else
                    body.Append("<td></td>")
                End If
                body.Append("</tr>")
                Counter = Counter + 1
                TotalItem = TotalItem + 1
                TotalQuantity = TotalQuantity + dsItems.Tables(0).Rows(I).Item("curQuantity")
                TotalPrice = TotalPrice + dsItems.Tables(0).Rows(I).Item("curUnitPrice")
            Next
            body.Append("</tbody></table>")
            body.Append("</div>")
            body.Append("<div class=""p-0 m-0"">")
            If EditMode = True Then
                body.Append("<div class=""row""><div class=""col-md-12"">")
                body.Append("<div class=""col-md-9 pl-0""><input type=""text"" id=""txtBarcode"" placeholder=""" & plcBarcode & """ class=""form-control form-control-sm input-sm border-deep-purple"" /></div>")
                body.Append("<div class=""col-md-3 pl-0""><button type=""button"" id=""btnClear"" class=""btn btn-warning btn-sm"" onclick=""javascript:$('#tblItems > tbody').empty();""><i class=""icon-trash-o""></i> Clear</button> <button type=""button"" id=""btnCollect"" class=""btn btn-success btn-sm half-width"" onclick=""javascript:showModal('ViewCollectItems','{byteWarehouse: ' + $('#drpSendWarehouse').val() + ', CheckBalance: true}','#mdlBeta');""><i class=""icon-gears""></i> Collect</button></div>")
                'body.Append("<div class=""col-md-2 pl-0""><input type=""text"" id=""txtBarcode"" placeholder=""" & plcBarcode & """ class=""form-control form-control-sm input-sm"" /></div>")
                'body.Append("<div class=""col-md-3 pl-0""><input type=""text"" id=""txtItemName"" class=""form-control form-control-sm input-sm"" /><input type=""hidden"" id=""txtItem"" /></div>")
                'body.Append("<div class=""col-md-2 pl-0""><input type=""text"" id=""dtpExpiry"" class=""form-control form-control-sm input-sm text-sm-center date-formatter dir-ltr"" /></div>")
                'body.Append("<div class=""col-md-2 pl-0""><input type=""number"" id=""txtPrice"" class=""form-control form-control-sm input-sm text-sm-center"" /><input type=""hidden"" id=""txtCost"" /></div>")
                'body.Append("<div class=""col-md-2 pl-0""><input type=""number"" id=""txtQuantity"" class=""form-control form-control-sm input-sm text-sm-center"" /><input type=""hidden"" id=""txtBalance"" /></div>")
                'body.Append("<div class=""col-md-1 pl-0""><button type=""button"" id=""btnAdd"" class=""btn btn-success btn-sm"">" & btnAdd & "</button></div>")
                body.Append("</div></div>")
            Else
                body.Append("<table class=""table table-bordered p-0 m-0""><tfoot>")
                body.Append("<tr><th style=""width:30px""></th><th style=""width:50px""><center>" & TotalItem & "</center></th><th style=""width:70px""></th><th style=""width:300px""></th><th style=""width:100px""></th><th style=""width:70px""><center>" & Math.Round(TotalQuantity, 0, MidpointRounding.AwayFromZero) & "</center></th><th style=""width:70px""><center>" & Math.Round(TotalPrice, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</center></th><th></th></tr>")
                body.Append("</tfoot></table>")
            End If
            body.Append("</div>")
            body.Append("</div>")
            body.Append("</div>")
            'content 3
            body.Append("<div role=""tabpanel"" class=""tab-pane"" id=""tab3"" aria-expanded=""true"" aria-labelledby=""base-tab3"">")
            body.Append("<div class=""row"">")
            body.Append("<div class=""p-0 m-0"">")
            body.Append(txtRemarks)
            body.Append("</div>")
            body.Append("</div>")
            body.Append("</div>")
            'contents ==> Close
            body.Append("</div>")
            body.Append("</form>")

            Dim scripts As String = ""
            scripts = scripts & "<script type=""text/javascript"">"
            scripts = scripts & "var warehouse = $('#drpSendWarehouse').val();"
            scripts = scripts & "$('#dtpExpiry').daterangepicker({singleDatePicker: true,locale: {format:'YYYY-MM-DD'}},function (start, end, label) {});"
            'scripts = scripts & "var curTab = 1;$('a[data-toggle=""tab""]').on('shown.bs.tab', function (e) { curTab = e.target.toString().substr(e.target.toString().length - 1, 1); if(curTab!=1) {$('#btnSuspend').attr('disabled', false); $('#btnSuspend').html('<i class=""icon-clock5""></i> " & btnUnsuspend & "');} else {$('#btnSuspend').html('<i class=""icon-clock5""></i> " & btnSuspend & "');if(tabCount>" & SusbendMax & ") $('#btnSuspend').attr('disabled', true); else $('#btnSuspend').attr('disabled', false);}});"
            'scripts = scripts & "var counter" & tabCounter & "=" & rowCounter & ";var tabCount=" & tabCounter & ";if(tabCount>" & SusbendMax & ") $('#btnSuspend').attr('disabled', true);"
            'scripts = scripts & "$(document).ready(function () {$('#txtBarcode').autocomplete({triggerSelectOnValidInput: true,onInvalidateSelection: function () {$('#txtBarcode').val('');}, lookup: function (query, done) {if ($('#txtBarcode').val().length > 4) {$.ajax({type: 'POST',url: 'ajax.aspx/findItem',data: '{query: ""' + query + '""}',contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {done(jQuery.parseJSON(response.d));},failure: function (msg) {alert(msg);}, error: function (xhr, ajaxOptions, thrownError) {alert('Load Form, update form error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});} else {done(jQuery.parseJSON(''));}}, onSelect: function (suggestion) {getItemInfo(suggestion.id);$('#txtBarcode').val('');$('#txtBarcode').focus();}});});"
            If EditMode = True Then scripts = scripts & "$(document).ready(function () {$('#txtBarcode').autocomplete({triggerSelectOnValidInput: true,onInvalidateSelection: function () {$('#txtBarcode').val('');}, lookup: function (query, done) {if ($('#txtBarcode').val().length > 4) {$.ajax({type: 'POST',url: 'ajax.aspx/findItem',data: '{query: ""' + query + '""}',contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {done(jQuery.parseJSON(response.d));},failure: function (msg) {alert(msg);}, error: function (xhr, ajaxOptions, thrownError) {alert('Load Form, update form error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});}}, onSelect: function (suggestion) {getItemInfo(suggestion.id);$('#txtBarcode').val('');$('#txtBarcode').focus();}});});"
            'Ret = Ret & "$('#tblItems' + curTab + ' > tbody').prepend('<tr><td>' + $('#txtItem').val() + '</td><td>' + $('#txtItemName').val() + '</td><td>' + $('#dtpExpiry').val() + '</td><td>' + $('#txtQuantity').val() + '</td><td>' + $('#txtPrice').val() + '</td></tr>');"
            scripts = scripts & "var counter=" & Counter & ";"
            scripts = scripts & "$('#txtBarcode').on('change paste keyup', function () {var barcode = $(this).val();if (barcode.length != 0) {if ($.isNumeric(barcode) == true) {if (event.which == 13 || barcode.length >= 14) {event.preventDefault();$(this).val('');getItemInfo(barcode)}}}});"
            scripts = scripts & "function calcQuantity() {var q=0;$.each($('.item'), function (k, v) { if($(v).val()==$('#txtItem').val()) q=q+parseInt($('#'+v.id.replace('item','quantity')).val());});return q;}"
            scripts = scripts & "$('#btnAdd').on('click', function () {if (parseInt($('#txtQuantity').val()) + calcQuantity() > $('#txtBalance').val()) { msg('','The balance of this item is: '+$('#txtBalance').val(),'error') } else {$('#tblItems > tbody').prepend('<tr><td style=""width:70px"">' + $('#txtItem').val() + '</td><td style=""width:300px"" class=""text-md-left"" title=""' + $('#txtItemName').val() + '"">' + $('#txtItemName').val() + '</td><td style=""width:100px"">' + $('#dtpExpiry').val() + '</td><td style=""width:70px"">' + $('#txtQuantity').val() + '</td><td style=""width:70px"">' + $('#txtPrice').val() + '</td><td><button type=""button"" class=""btn btn-danger btn-xs"" onclick=""javascript:removeThis(this)"">Delete</button><input type=""hidden"" id=""item'+counter+'"" class=""item"" name=""item"" value=""'+$('#txtItem').val()+'"" /><input type=""hidden"" name=""expiry"" value=""'+$('#dtpExpiry').val()+'"" /><input type=""hidden"" id=""quantity'+counter+'"" name=""quantity"" class=""quantity"" value=""'+$('#txtQuantity').val()+'"" /><input type=""hidden"" name=""price"" value=""'+$('#txtPrice').val()+'"" /><input type=""hidden"" name=""cost"" value=""'+$('#txtCost').val()+'"" /></td></tr>');counter++;}});"
            '$('#items').val($('#items').val()+$('#txtItem').val()+',');
            'scripts = scripts & "function showCashier() {var valJson = JSON.stringify($('#frmInvoice' + curTab).serializeArray());var dataJson = { TabCounter: curTab, Fields: valJson };var dataJsonString = JSON.stringify(dataJson);$.ajax({type: 'POST',url: 'ajax.aspx/viewCashier1',data: dataJsonString,contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {if (response.d.indexOf('Err:') >= 0) {msg('',response.d.substring(4, response.d.length),'error');} else {$('#mdlMessage').html(response.d);$('#mdlMessage').modal('show');}},failure: function (msg) {alert(msg);},error: function (xhr, ajaxOptions, thrownError) {alert(' write json item, Ajax error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});}"
            'scripts = scripts & "function sendToCashier() {var valJson = JSON.stringify($('#frmInvoice' + curTab).serializeArray());var dataJson = { Fields: valJson };var dataJsonString = JSON.stringify(dataJson);$.ajax({type: 'POST',url: 'ajax.aspx/SendToCashier',data: dataJsonString,contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {if (response.d.indexOf('Err:') >= 0) {msg('',response.d.substring(4, response.d.length),'error');} else {$('#prtJS').html(response.d);closeCurrentTab();}},failure: function (msg) {alert(msg);},error: function (xhr, ajaxOptions, thrownError) {alert(' write json item, Ajax error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});}"
            'scripts = scripts & "calculateInsurance(curTab);calculateCash(curTab);$('#txtBarcode').focus();"
            'scripts = scripts & "function saveVoucher(){if($('#tblItems > tbody > tr').length>0) saveTransfer(); else msg('','No items to transfer','error') };"
            scripts = scripts & "function saveTransfer() {var valJson = JSON.stringify($('#frmVoucher').serializeArray());var dataJson = { lngTransaction: " & lngTransaction & ", Fields: valJson };var dataJsonString = JSON.stringify(dataJson);$.ajax({type: 'POST',url: 'ajax.aspx/saveTransfer',data: dataJsonString,contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {if (response.d.indexOf('Err:') >= 0) {msg('',response.d.substring(4, response.d.length),'error');} else {$('#prtJS').html(response.d);}},failure: function (msg) {alert(msg);},error: function (xhr, ajaxOptions, thrownError) {alert(' write json item, Ajax error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});}"
            scripts = scripts & "function approveSend() {approveSendItems(" & lngTransaction & ", '#mdlAlpha');}"
            scripts = scripts & "function approveReceive() {approveReceiveItems(" & lngTransaction & ", '#mdlAlpha');}"
            scripts = scripts & "function returnItems() {returnTransferedItems(" & lngTransaction & ");}"
            'scripts = scripts & "function cancelThis(){showModal('viewReturnAmount','{lngTransaction: " & lngTransaction & ", lstItems: """", IsCancel: true, NextFunction: ""cancelInvoice""}','#mdlConfirm');} function requestToCancel(){showModal('viewReturnAmount','{lngTransaction: " & lngTransaction & ", lstItems: """", IsCancel: true, NextFunction: ""requestCancelInvoice""}','#mdlConfirm');}"
            scripts = scripts & "function processCancel() {cancelTransfer(" & lngTransaction & ");}"
            scripts = scripts & "function cancelThis(){confirm('', 'Are you sure to cancel this voucher?', processCancel)} function requestToCancel(){confirm('', 'Are you sure to request to cancel this voucher?', requestCancelTransfer)}"
            scripts = scripts & "</script>"
            body.Append(scripts)
        Catch ex As Exception
            'confirm('','" & cnfReturnToDoctor & "',returnToDoctor);
        End Try

        Dim SaveButton As String = ""
        Dim ApproveSendButton As String = ""
        Dim ApproveReceiveButton As String = ""
        Dim CancelButton As String = ""
        Dim PrintButton As String = ""
        Dim ReturnButton As String = ""

        'If ShowCancel = True Then
        '    If DirectChangeInvoice = True Or AllowCancel Then
        '        CancelButton = "<button type=""button"" class=""btn btn-outline-red"" id=""btnCancel"" onclick=""javascript:cancelThis();"" " & ActionDisabled & "><i class=""icon-arrow-down3""></i> " & btnCancel & "</button>"
        '    Else
        '        CancelButton = "<button type=""button"" class=""btn btn-outline-red"" id=""btnCancel"" onclick=""javascript:requestToCancel();"" " & ActionDisabled & "><i class=""icon-arrow-down3""></i> " & btnCancel & "</button>"
        '    End If
        'End If

        If EditMode = True Then SaveButton = " <button type=""button"" class=""btn btn-success ml-1 mr-1"" onclick=""javascript:if($('#tblItems > tbody > tr').length>0) saveTransfer(); else msg('','No items to transfer','error');""><i class=""icon-save""></i> " & btnSave & "</button> "
        If ApproveSendEnabled = True Then ApproveSendButton = " <button type=""button"" class=""btn btn-outline-primary float-left"" onclick=""javascript:confirm('','Are you sure to transfer?',approveSend);""><i class=""icon-upload4""></i> " & btnApproveSend & "</button> "
        If ApproveReceiveEnabled = True Then ApproveReceiveButton = " <button type=""button"" class=""btn btn-outline-primary float-left"" onclick=""javascript:confirm('','Are you sure to transfer?',approveReceive);""><i class=""icon-download4""></i> " & btnApproveReceive & "</button> "
        If ApproveMode = True Then ReturnButton = " <button type=""button"" class=""btn btn-outline-orange float-left"" onclick=""javascript:confirm('','Are you sure to return items?',returnItems);""><i class=""icon-level-down""></i> " & btnReturn & "</button> "

        If EditMode = False Then
            PrintButton = "<span app-print=""true"" app-popup=""" & PopupToPrint.ToString.ToLower & """ app-printer="""" app-url=""p_transfer.aspx?t=" & lngTransaction & """ app-data=""""><button type=""button"" class=""btn btn-primary ml-1 mr-1"" onclick=""javascript:window.open('p_transfer.aspx?t=" & lngTransaction & "', 'transferprint');"">" & btnPrint & "</button></span>"
        Else
            PrintButton = ""
        End If

        If CancelEnabled = True Then
            If DirectChangeTransfer = True Or AllowCancel Then
                'CancelButton = " <button type=""button"" class=""btn btn-outline-red"" id=""btnCancel"" onclick=""javascript:cancelThis();"" " & ActionDisabled & "><i class=""icon-arrow-down3""></i> " & btnCancel & "</button>"
                CancelButton = " <button type=""button"" class=""btn btn-outline-danger float-left ml-1 mr-1"" onclick=""javascript:cancelThis();""><i class=""icon-level-down""></i> " & btnCancel & "</button> "
            Else
                'CancelButton = " <button type=""button"" class=""btn btn-outline-red"" id=""btnCancel"" onclick=""javascript:requestToCancel();"" " & ActionDisabled & "><i class=""icon-arrow-down3""></i> " & btnCancel & "</button>"
                CancelButton = " <button type=""button"" class=""btn btn-outline-danger float-left ml-1 mr-1"" onclick=""javascript:requestToCancel();""><i class=""icon-level-down""></i> " & btnCancel & "</button> "
            End If
        End If

        'If EditMode = False Then PrintButton = "<button type=""button"" class=""btn btn-primary"" onclick=""javascript:window.open('p_transfer.aspx?t=" & lngTransaction & "', 'transferprint');"">" & btnPrint & "</button> "
        Dim Buttons As String = ApproveSendButton & ApproveReceiveButton & CancelButton & ReturnButton & SaveButton & PrintButton & "<button type=""button"" class=""btn btn-secondary"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnClose & "</button>"
        Dim sh As New Share.UI
        Return sh.drawModal(Header, body.ToString, Buttons, Share.UI.ModalSize.Large, "", "p-0")
    End Function

    Private Function CreateTransferTab() As String

    End Function

    Public Function returnTransferedItems(ByVal lngTransaction As Long) As String
        Select Case byteLanguage
            Case 2
                DataLang = "Ar"
            Case Else
                DataLang = "En"
        End Select

        Dim ds As DataSet

        If lngTransaction > 0 Then
            Try
                Dim dsTemp As DataSet = dcl.GetDS("SELECT * FROM Stock_Trans AS T INNER JOIN Stock_Xlink AS X ON X.lngTransaction=T.lngTransaction WHERE T.lngTransaction=" & lngTransaction)
                If dsTemp.Tables(0).Rows.Count > 0 Then
                    If dsTemp.Tables(0).Rows(0).Item("byteStatus") = 0 Then Return "Err: This operation is cancelled..."
                    Dim Xlink As Long = dsTemp.Tables(0).Rows(0).Item("lngXlink")

                    Dim dsOther As DataSet = dcl.GetDS("SELECT T.lngTransaction, T.byteStatus, X.lngXlink FROM Stock_Xlink AS X INNER JOIN Stock_Trans AS T ON T.lngTransaction=X.lngTransaction WHERE lngPointer=" & lngTransaction & " AND X.lngTransaction<>" & lngTransaction)
                    If dsOther.Tables(0).Rows(0).Item("byteStatus") <> 1 Then Return "Err: Stock has been received or cancelled from the other warehouse.."
                    Dim OtherTransaction As Long = dsOther.Tables(0).Rows(0).Item("lngTransaction")
                    Dim OtherXlink As Long = dsOther.Tables(0).Rows(0).Item("lngXlink")
                    'updating status for send warehouse
                    dcl.ExecSQuery("UPDATE Stock_Trans SET byteStatus=1 WHERE lngTransaction=" & lngTransaction)
                    'removing items (important)
                    dcl.ExecScalar("DELETE FROM Stock_Xlink_Items WHERE lngXlink=" & OtherXlink)
                    'update audit

                    'update logs
                    Dim usr As New Share.User
                    usr.AddLog(strUserName, Now, 3, "Transfer", lngTransaction, 2, "Return Transfered Items")
                Else
                    Return "Err:This record is unavailable, please refresh the list again.."
                End If
            Catch ex As Exception
                Return "Err:" & ex.Message
            End Try
        Else
            Return "Err:Not a correct voucher"
        End If

        Return "<script type=""text/javascript"">msg('','This voucher returned successfully!','notice');$('#mdlAlpha').modal('hide');fillTransfer();showModal('viewTransfer', '{lngTransaction: " & lngTransaction & ", ReceiveMode: false}', '#mdlAlpha')</script>"
    End Function

    Public Function cancelTransfer(ByVal lngTransaction As Long, Optional ByVal VoidUser As String = "") As String
        Select Case byteLanguage
            Case 2
                DataLang = "Ar"
            Case Else
                DataLang = "En"
        End Select

        If VoidUser = "" Then VoidUser = strUserName

        Dim ds As DataSet

        If lngTransaction > 0 Then
            Try
                Dim dsTemp As DataSet = dcl.GetDS("SELECT * FROM Stock_Trans AS T INNER JOIN Stock_Xlink AS X ON X.lngTransaction=T.lngTransaction WHERE T.lngTransaction=" & lngTransaction)
                If dsTemp.Tables(0).Rows.Count > 0 Then
                    If dsTemp.Tables(0).Rows(0).Item("byteStatus") = 0 Then Return "Err: This operation is cancelled..."
                    Dim Xlink As Long = dsTemp.Tables(0).Rows(0).Item("lngXlink")

                    Dim dsOther As DataSet = dcl.GetDS("SELECT T.lngTransaction, T.byteStatus FROM Stock_Xlink AS X INNER JOIN Stock_Trans AS T ON T.lngTransaction=X.lngTransaction WHERE lngPointer=" & lngTransaction & " AND X.lngTransaction<>" & lngTransaction)
                    If dsOther.Tables(0).Rows(0).Item("byteStatus") <> 1 Then Return "Err: Stock has been received or cancelled from the other warehouse.."
                    Dim OtherTransaction As Long = dsOther.Tables(0).Rows(0).Item("lngTransaction")
                    'updating status for send warehouse
                    dcl.ExecSQuery("UPDATE Stock_Trans SET byteStatus=0 WHERE lngTransaction=" & lngTransaction)
                    'updating status for receive warehouse
                    dcl.ExecSQuery("UPDATE Stock_Trans SET byteStatus=0 WHERE lngTransaction=" & OtherTransaction)

                    'update audit
                    dcl.ExecSQuery("UPDATE Stock_Trans_Audit SET strVoidBy='" & strUserName & "', dateVoid='" & Today.ToString("yyyy-MM-dd HH:mm:ss") & "' WHERE lngTransaction=" & lngTransaction)
                    'update logs
                    Dim usr As New Share.User
                    usr.AddLog(strUserName, Now, 3, "Transfer", lngTransaction, 2, "Cancel the transfer process")
                Else
                    Return "Err:This record is unavailable, please refresh the list again.."
                End If
            Catch ex As Exception
                Return "Err:" & ex.Message
            End Try
        Else
            Return "Err:Not a correct voucher"
        End If

        Return "<script type=""text/javascript"">msg('','This voucher cancelled successfully!','notice');$('#mdlAlpha').modal('hide');fillTransfer();</script>"
    End Function

    Public Function viewSExpired(ByVal lngTransaction As Long, ByVal IsOut As Boolean) As String
        Dim lblVoucherNo, lblType, lblWarehouseFrom, lblWarehouseTo, lblDate, lblSender, lblRecipient, lblStatus As String
        Dim txtSendVoucher, txtSendDate, drpSendType, txtSendWarehouse, drpSendWarehouse, txtSendUser, txtSendStatus As String
        Dim txtReceiveVoucher, txtReceiveDate, drpReceiveType, txtReceiveWarehouse, drpReceiveWarehouse, txtReceiveUser, txtReceiveStatus As String
        Dim txtRemarks As String
        Dim plcBarcode, plcRemarks As String
        Dim Pending, Approved, Cancelled As String

        Dim btnClose, btnSave, btnApproveSend, btnApproveReceive, btnApprove, btnCancel As String
        Select Case byteLanguage
            Case 2
                DataLang = "Ar"

                Pending = "في الانتظار"
                Approved = "معمدة"
                Cancelled = "ملغية"

                lblVoucherNo = "رقم السند"
                lblType = "نوع السند"
                lblWarehouseFrom = "من"
                lblWarehouseTo = "إلى"
                lblDate = "التاريخ"
                lblSender = "المرسل"
                lblRecipient = "المستلم"
                lblStatus = "الحالة"

                btnSave = "حفظ التغييرات"
                btnApprove = "تعميد"
                btnApproveSend = "تعميد تسليم"
                btnApproveReceive = "تعميد استلام"
                btnCancel = "إلغاء"
                btnClose = "إغلاق"
            Case Else
                DataLang = "En"

                Pending = "Pending"
                Approved = "Approved"
                Cancelled = "Cancelled"

                lblVoucherNo = "Voucher No"
                lblType = "Voucher Type"
                lblWarehouseFrom = "From"
                lblWarehouseTo = "To"
                lblDate = "Date"
                lblSender = "Sender"
                lblRecipient = "Recipient"
                lblStatus = "Status"

                btnSave = "Save Changes"
                btnApprove = "Approve"
                btnApproveSend = "Approve Send"
                btnApproveReceive = "Approve Receive"
                btnCancel = "Cancel"
                btnClose = "Close"
        End Select

        If SExpiredString = "" Then Return "Err:You don't have warehouse assigned"

        Dim body As New StringBuilder("")

        Dim EditMode As Boolean = False
        Dim ApproveSendEnabled As Boolean = False
        Dim ApproveReceiveEnabled As Boolean = False
        Dim CancelEnabled As Boolean = False
        Dim ApproveEnabled As Boolean = False

        Try
            If lngTransaction > 0 Then
                Dim ds As DataSet
                ds = dcl.GetDS("SELECT T1.lngTransaction, T1.strTransaction AS strSendVoucher, T2.strTransaction AS strReceiveVoucher, T1.dateTransaction AS dateSendDate, T2.dateTransaction AS dateReceiveDate, TT1.strType" & DataLang & " AS strSendType, TT2.strType" & DataLang & " AS strReceiveType, W1.byteWarehouse AS byteSendWarehouse, W1.strWarehouse" & DataLang & " AS strSendWarehouse, W2.byteWarehouse AS byteReceiveWarehouse, W2.strWarehouse" & DataLang & " AS strReceiveWarehouse, TA1.strCreatedBy AS strSendUser, TA2.strCreatedBy AS strReceiveUser, T1.byteStatus AS byteSendStatus, T2.byteStatus AS byteReceiveStatus, T1.strRemarks FROM Stock_Xlink AS X1 LEFT JOIN Stock_Xlink AS X2 ON X1.lngPointer=X2.lngPointer INNER JOIN Stock_Trans AS T1 ON T1.lngTransaction=X1.lngTransaction INNER JOIN Stock_Trans AS T2 ON T2.lngTransaction=X2.lngTransaction INNER JOIN Stock_Warehouses AS W1 ON T1.byteWarehouse=W1.byteWarehouse INNER JOIN Stock_Warehouses AS W2 ON T2.byteWarehouse=W2.byteWarehouse INNER JOIN Stock_Trans_Types AS TT1 ON T1.byteTransType=TT1.byteTransType INNER JOIN Stock_Trans_Types AS TT2 ON T2.byteTransType=TT2.byteTransType INNER JOIN Stock_Trans_Audit AS TA1 ON T1.lngTransaction=TA1.lngTransaction LEFT JOIN Stock_Trans_Audit AS TA2 ON T2.lngTransaction=TA2.lngTransaction WHERE ((T1.byteBase=19 AND T2.byteBase=52) OR (T1.byteBase=51 AND T2.byteBase=20)) AND T1.lngTransaction=" & lngTransaction)
                If ds.Tables(0).Rows.Count > 0 Then
                    txtSendVoucher = ds.Tables(0).Rows(0).Item("strSendVoucher")
                    drpSendType = ds.Tables(0).Rows(0).Item("strSendType")
                    txtSendWarehouse = ds.Tables(0).Rows(0).Item("strSendWarehouse")
                    drpSendWarehouse = ds.Tables(0).Rows(0).Item("byteSendWarehouse")
                    txtSendUser = ds.Tables(0).Rows(0).Item("strSendUser")
                    txtSendDate = CDate(ds.Tables(0).Rows(0).Item("dateSendDate")).ToString(strDateFormat)
                    txtRemarks = ds.Tables(0).Rows(0).Item("strRemarks").ToString

                    Select Case ds.Tables(0).Rows(0).Item("byteSendStatus").ToString
                        Case 0
                            txtSendStatus = Cancelled
                            EditMode = False
                            'ApproveSendEnabled = False
                            ApproveEnabled = False
                            CancelEnabled = False
                        Case 1
                            txtSendStatus = Pending
                            EditMode = True
                            'ApproveSendEnabled = True
                            ApproveEnabled = True
                            CancelEnabled = False
                        Case 2
                            txtSendStatus = Approved
                        Case Else
                            txtSendStatus = ""
                            EditMode = False
                            'ApproveSendEnabled = False
                            ApproveEnabled = False
                            CancelEnabled = True
                    End Select

                    If IsDBNull(ds.Tables(0).Rows(0).Item("strReceiveVoucher")) Then txtReceiveVoucher = "" Else txtReceiveVoucher = ds.Tables(0).Rows(0).Item("strReceiveVoucher")
                    If IsDBNull(ds.Tables(0).Rows(0).Item("strReceiveType")) Then drpReceiveType = "" Else drpReceiveType = ds.Tables(0).Rows(0).Item("strReceiveType")
                    If IsDBNull(ds.Tables(0).Rows(0).Item("strReceiveWarehouse")) Then txtReceiveWarehouse = "" Else txtReceiveWarehouse = ds.Tables(0).Rows(0).Item("strReceiveWarehouse")
                    If IsDBNull(ds.Tables(0).Rows(0).Item("byteReceiveWarehouse")) Then drpReceiveWarehouse = "" Else drpReceiveWarehouse = ds.Tables(0).Rows(0).Item("byteReceiveWarehouse")
                    If IsDBNull(ds.Tables(0).Rows(0).Item("strReceiveUser")) Then txtReceiveUser = "" Else txtReceiveUser = ds.Tables(0).Rows(0).Item("strReceiveUser")
                    If IsDBNull(ds.Tables(0).Rows(0).Item("dateReceiveDate")) Then txtReceiveDate = "" Else txtReceiveDate = CDate(ds.Tables(0).Rows(0).Item("dateReceiveDate")).ToString(strDateFormat)

                    Select Case ds.Tables(0).Rows(0).Item("byteReceiveStatus").ToString
                        Case 0
                            txtReceiveStatus = Cancelled
                            'ApproveReceiveEnabled = False
                        Case 1
                            txtReceiveStatus = Pending
                            If ds.Tables(0).Rows(0).Item("byteSendStatus").ToString = 2 And ds.Tables(0).Rows(0).Item("byteReceiveWarehouse") = byteWarehouse Then ApproveReceiveEnabled = True Else ApproveReceiveEnabled = False
                        Case 2
                            txtReceiveStatus = Approved
                            'ApproveReceiveEnabled = False
                        Case Else
                            txtReceiveStatus = ""
                            'ApproveReceiveEnabled = False
                    End Select

                    'Dim dsOther As DataSet
                    'If ds.Tables(0).Rows(0).Item("byteBase") = 20 Then
                    '    dsOther = dcl.GetDS("SELECT * FROM Stock_Trans AS T INNER JOIN Stock_Warehouses AS W ON T.byteWarehouse=W.byteWarehouse WHERE T.strTransaction = '" & txtVoucherNo & "' AND T.byteBase=19")
                    'Else
                    '    dsOther = dcl.GetDS("SELECT * FROM Stock_Trans AS T INNER JOIN Stock_Warehouses AS W ON T.byteWarehouse=W.byteWarehouse WHERE T.strTransaction = '" & txtVoucherNo & "' AND T.byteBase=20")
                    'End If
                    'drpWarehouseTo = dsOther.Tables(0).Rows(0).Item("strWarehouse" & DataLang)

                    txtSendVoucher = "<input type=""text"" id=""txtSendVoucher"" name=""txtSendVoucher"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value=""" & txtSendVoucher & """ />"
                    txtReceiveVoucher = "<input type=""text"" id=""txtReceiveVoucher"" name=""txtReceiveVoucher"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value=""" & txtReceiveVoucher & """ />"
                    txtSendDate = "<input type=""text"" id=""txtSendDate"" name=""txtSendDate"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value=""" & txtSendDate & """ />"
                    txtReceiveDate = "<input type=""text"" id=""txtReceiveDate"" name=""txtReceiveDate"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value=""" & txtReceiveDate & """ />"
                    drpSendType = "<input type=""text"" id=""drpSendType"" name=""drpSendType"" class=""form-control form-control-sm"" readonly=""readonly"" value=""" & drpSendType & """ />"
                    drpReceiveType = "<input type=""text"" id=""drpReceiveType"" name=""drpReceiveType"" class=""form-control form-control-sm"" readonly=""readonly"" value=""" & drpReceiveType & """ />"
                    drpSendWarehouse = "<input type=""hidden"" id=""drpSendWarehouse"" name=""drpSendWarehouse"" value=""" & drpSendWarehouse & """ /><input type=""text"" class=""form-control form-control-sm"" readonly=""readonly"" value=""" & txtSendWarehouse & """ />"
                    drpReceiveWarehouse = "<input type=""hidden"" id=""drpReceiveWarehouse"" name=""drpReceiveWarehouse"" value=""" & drpReceiveWarehouse & """ /><input type=""text"" class=""form-control form-control-sm"" readonly=""readonly"" value=""" & txtReceiveWarehouse & """ />"
                    txtSendUser = "<input type=""text"" id=""txtSendUser"" name=""txtSendUser"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value=""" & txtSendUser & """ />"
                    txtReceiveUser = "<input type=""text"" id=""txtReceiveUser"" name=""txtReceiveUser"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value=""" & txtReceiveUser & """ />"
                    txtSendStatus = "<input type=""text"" id=""txtSendStatus"" name=""txtSendStatus"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value=""" & txtSendStatus & """ />"
                    txtReceiveStatus = "<input type=""text"" id=""txtReceiveStatus"" name=""txtReceiveStatus"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value=""" & txtReceiveStatus & """ />"

                    txtRemarks = "<textarea id=""txtRemarks"" rows=""15"" class=""form-control"" name=""txtRemarks"" placeholder=""About Transfer"" readonly=""readonly"">" & txtRemarks & "</textarea>"
                Else
                    Return "Err: Record not found"
                End If
            Else
                EditMode = True
                ApproveSendEnabled = False
                ApproveReceiveEnabled = False
                CancelEnabled = False
                Dim dsLast As DataSet = dcl.GetDS("SELECT MAX(CAST(strTransaction AS bigint)) AS Last FROM Stock_Trans WHERE YEAR(dateTransaction) = " & intYear & " AND byteBase=52")
                Dim lngNewVoucher As Long
                If IsDBNull(dsLast.Tables(0).Rows(0).Item("Last")) Then
                    lngNewVoucher = 1
                Else
                    lngNewVoucher = dsLast.Tables(0).Rows(0).Item("Last") + 1
                End If

                Dim dsTypeFrom, dsTypeTo As DataSet
                Dim TypeFrom, TypeTo As Integer
                Dim TypeFromName, TypeToName As String
                If IsOut = False Then
                    TypeFrom = 14
                    dsTypeFrom = dcl.GetDS("SELECT * FROM Stock_Trans_Types WHERE byteTransType=14")
                    TypeFromName = dsTypeFrom.Tables(0).Rows(0).Item("strType" & DataLang)
                    TypeTo = 42
                    dsTypeTo = dcl.GetDS("SELECT * FROM Stock_Trans_Types WHERE byteTransType=42")
                    TypeToName = dsTypeTo.Tables(0).Rows(0).Item("strType" & DataLang)
                Else
                    TypeFrom = 41
                    dsTypeFrom = dcl.GetDS("SELECT * FROM Stock_Trans_Types WHERE byteTransType=41")
                    TypeFromName = dsTypeFrom.Tables(0).Rows(0).Item("strType" & DataLang)
                    TypeTo = 15
                    dsTypeTo = dcl.GetDS("SELECT * FROM Stock_Trans_Types WHERE byteTransType=15")
                    TypeToName = dsTypeTo.Tables(0).Rows(0).Item("strType" & DataLang)
                End If

                Dim CurrentWarehouseID As Byte = 0
                Dim CurrectWarehouse, OtherWarehouse As String
                CurrectWarehouse = ""
                OtherWarehouse = ""
                Dim where As String = ""
                Dim WarehouseList As String = ""

                If SExpiredString <> "*" Then where = "AND byteWarehouse IN (" & SExpiredString & ")"
                Dim dsWarehouse As DataSet = dcl.GetDS("SELECT * FROM Stock_Warehouses WHERE bActive=1 " & where)
                For I = 0 To dsWarehouse.Tables(0).Rows.Count - 1
                    WarehouseList = WarehouseList & "<option value=""" & dsWarehouse.Tables(0).Rows(I).Item("byteWarehouse") & """>" & dsWarehouse.Tables(0).Rows(I).Item("strWarehouse" & DataLang) & "</option>"
                Next

                Dim WarehouseList2 As String = ""
                Dim dsWarehouse2 As DataSet = dcl.GetDS("SELECT * FROM Stock_Warehouses WHERE bActive=1 " & where)
                For I = 0 To dsWarehouse2.Tables(0).Rows.Count - 1
                    'If dsWarehouse.Tables(0).Rows(I).Item("byteWarehouse") = byteWarehouse Then
                    '    CurrentWarehouseID = dsWarehouse.Tables(0).Rows(I).Item("byteWarehouse")
                    '    CurrectWarehouse = dsWarehouse.Tables(0).Rows(I).Item("strWarehouse" & DataLang)
                    'Else
                    '    OtherWarehouse = OtherWarehouse & "<option value=""" & dsWarehouse.Tables(0).Rows(I).Item("byteWarehouse") & """>" & dsWarehouse.Tables(0).Rows(I).Item("strWarehouse" & DataLang) & "</option>"
                    'End If
                    WarehouseList2 = WarehouseList2 & "<option value=""" & dsWarehouse2.Tables(0).Rows(I).Item("byteWarehouse") & """>" & dsWarehouse2.Tables(0).Rows(I).Item("strWarehouse" & DataLang) & "</option>"
                Next

                txtSendVoucher = "<input type=""text"" id=""txtSendVoucher"" name=""txtSendVoucher"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value=""" & lngNewVoucher & """ />"
                txtReceiveVoucher = "<input type=""text"" id=""txtReceiveVoucher"" name=""txtReceiveVoucher"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value="""" />"
                txtSendDate = "<input type=""text"" id=""txtSendDate"" name=""txtSendDate"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value=""" & Today.ToString(strDateFormat) & """ />"
                txtReceiveDate = "<input type=""text"" id=""txtReceiveDate"" name=""txtReceiveDate"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value="""" />"
                drpSendType = "<input type=""hidden"" id=""drpSendType"" name=""drpSendType"" value=""" & TypeFrom & """ /><input type=""text"" class=""form-control form-control-sm"" readonly=""readonly"" value=""" & TypeFromName & """ />"
                drpReceiveType = "<input type=""hidden"" id=""drpReceiveType"" name=""drpReceiveType"" value=""" & TypeTo & """ /><input type=""text"" class=""form-control form-control-sm"" readonly=""readonly"" value=""" & TypeToName & """ />"
                'drpSendWarehouse = "<input type=""hidden"" id=""drpSendWarehouse"" name=""drpSendWarehouse"" value=""" & CurrentWarehouseID & """ /><input type=""text"" class=""form-control form-control-sm"" readonly=""readonly"" value=""" & CurrectWarehouse & """ />"
                'drpReceiveWarehouse = "<input type=""hidden"" id=""drpReceiveWarehouse"" name=""drpReceiveWarehouse"" value=""" & CurrentWarehouseID & """ /><input type=""text"" class=""form-control form-control-sm"" readonly=""readonly"" value=""" & CurrectWarehouse & """ />"
                drpSendWarehouse = "<select id=""drpSendWarehouse"" name=""drpSendWarehouse"" class=""form-control form-control-sm"">" & WarehouseList & "</select>"
                drpReceiveWarehouse = "<select id=""drpReceiveWarehouse"" name=""drpReceiveWarehouse"" class=""form-control form-control-sm"">" & WarehouseList2 & "</select>"
                txtSendUser = "<input type=""text"" id=""txtSendUser"" name=""txtSendUser"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value=""" & strUserName & """ />"
                txtReceiveUser = "<input type=""text"" id=""txtReceiveUser"" name=""txtReceiveUser"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value="""" />"
                txtSendStatus = "<input type=""hidden"" id=""txtSendStatus"" name=""txtSendStatus"" value=""1"" /><input type=""text"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value=""" & Pending & """ />"
                txtReceiveStatus = "<input type=""text"" id=""txtReceiveStatus"" name=""txtReceiveStatus"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value="""" />"

                txtRemarks = "<textarea id=""txtRemarks"" rows=""15"" class=""form-control"" name=""txtRemarks"" placeholder=""About Transfer"">" & txtRemarks & "</textarea>"
            End If
        Catch ex As Exception
            Return "Err:" & ex.Message
        End Try

        Try
            'tabs
            body.Append("<form id=""frmVoucher"" style=""height:400px"">")
            body.Append("<ul class=""nav nav-tabs m-0"">")
            body.Append("<li class=""nav-item""><a class=""nav-link active"" id=""base-tab1"" data-toggle=""tab"" aria-controls=""tab1"" href=""#tab1"" aria-expanded=""true"" >Transfer</a></li>")
            body.Append("<li class=""nav-item""><a class=""nav-link"" id=""base-tab2"" data-toggle=""tab"" aria-controls=""tab2"" href=""#tab2"" aria-expanded=""true"" >Items</a></li>")
            body.Append("<li class=""nav-item""><a class=""nav-link"" id=""base-tab3"" data-toggle=""tab"" aria-controls=""tab3"" href=""#tab3"" aria-expanded=""true"" >Remarks</a></li>")
            body.Append("</ul>")
            'contents ==> Open
            body.Append("<div class=""tab-content m-0 p-1"">")
            'content 1
            body.Append("<div role=""tabpanel"" class=""tab-pane active"" id=""tab1"" aria-expanded=""true"" aria-labelledby=""base-tab1"">")
            body.Append("<form id=""frmItem"">")
            body.Append("<div class=""row"">")
            body.Append("<div class=""col-md-6"">")
            body.Append("<div class=""col-md-12 border border-cyan border-lighten-4 p-0 pb-1"">")
            body.Append("<div class=""col-md-12 bg-cyan bg-lighten-4 blue pt-1""><h3>Transfer From</h3></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblVoucherNo & ":</div><div class=""col-md-4"">" & txtSendVoucher & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblType & ":</div><div class=""col-md-8"">" & drpSendType & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblWarehouseFrom & ":</div><div class=""col-md-8"">" & drpSendWarehouse & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblDate & ":</div><div class=""col-md-4"">" & txtSendDate & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblSender & ":</div><div class=""col-md-4"">" & txtSendUser & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblStatus & ":</div><div class=""col-md-4"">" & txtSendStatus & "</div></div>")
            body.Append("</div>")
            body.Append("</div>")
            body.Append("<div class=""col-md-6"">")
            body.Append("<div class=""col-md-12 border border-orange  border-lighten-4 p-0 pb-1"">")
            body.Append("<div class=""col-md-12 bg-orange  bg-lighten-4 orange pt-1""><h3>Transfer To</h3></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblVoucherNo & ":</div><div class=""col-md-4"">" & txtReceiveVoucher & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblType & ":</div><div class=""col-md-8"">" & drpReceiveType & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblWarehouseTo & ":</div><div class=""col-md-8"">" & drpReceiveWarehouse & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblDate & ":</div><div class=""col-md-4"">" & txtReceiveDate & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblRecipient & ":</div><div class=""col-md-4"">" & txtReceiveUser & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblStatus & ":</div><div class=""col-md-4"">" & txtReceiveStatus & "</div></div>")
            body.Append("</div>")
            body.Append("</div>")
            body.Append("</div>")
            body.Append("</form>")
            body.Append("</div>")
            'content 2
            body.Append("<div role=""tabpanel"" class=""tab-pane"" id=""tab2"" aria-expanded=""true"" aria-labelledby=""base-tab2"">")
            body.Append("<div class=""row"">")
            body.Append("<div class=""p-0 m-0"">")
            body.Append("<table class=""table table-bordered p-0 m-0""><thead>")
            body.Append("<tr><th style=""width:70px""><center>Item</center></th><th style=""width:300px""><center>Item Name</center></th><th style=""width:100px""><center>Expire</center></th><th style=""width:70px""><center>Quantity</center></th><th style=""width:70px""><center>Price</center></th><th><center></center></th></tr>")
            body.Append("</thead></table>")
            body.Append("</div>")
            body.Append("<div class=""p-0 m-0"" style=""height:253px; overflow-x:auto;"">")
            body.Append("<table id=""tblItems"" class=""table table-bordered p-0 m-0""><tbody>")
            Dim dsItems As DataSet = dcl.GetDS("SELECT * FROM Stock_Xlink AS X INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink=XI.lngXlink INNER JOIN Stock_Items AS I ON XI.strItem=I.strItem WHERE X.lngTransaction=" & lngTransaction)
            Dim ItemName As String
            Dim Counter As Integer = 0
            Dim TotalQuantity, TotalItem, TotalPrice As Decimal
            For I = 0 To dsItems.Tables(0).Rows.Count - 1
                If Len(dsItems.Tables(0).Rows(I).Item("strItem" & DataLang)) > 27 Then ItemName = Mid(dsItems.Tables(0).Rows(I).Item("strItem" & DataLang), 1, 27) & "..." Else ItemName = dsItems.Tables(0).Rows(I).Item("strItem" & DataLang)
                body.Append("<tr>")
                body.Append("<td style=""width:70px"">" & dsItems.Tables(0).Rows(I).Item("strItem") & "</td>")
                body.Append("<td style=""width:300px"" class=""text-md-left"" title=""" & dsItems.Tables(0).Rows(I).Item("strItem" & DataLang) & """>" & ItemName & "</td>")
                body.Append("<td style=""width:100px"">" & CDate(dsItems.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM-dd") & "</td>")
                body.Append("<td style=""width:70px"">" & Math.Round(dsItems.Tables(0).Rows(I).Item("curQuantity"), 0, MidpointRounding.AwayFromZero) & "</td>")
                body.Append("<td style=""width:70px"">" & Math.Round(dsItems.Tables(0).Rows(I).Item("curUnitPrice"), byteCurrencyRound, MidpointRounding.AwayFromZero) & "</td>")
                If EditMode = True Then
                    body.Append("<td><button type=""button"" class=""btn btn-danger btn-xs"" onclick=""javascript:removeThis(this)"">Delete</button><input type=""hidden"" id=""item" & Counter & """ class=""item"" name=""item"" value=""" & dsItems.Tables(0).Rows(I).Item("strItem") & """ /><input type=""hidden"" name=""expiry"" value=""" & CDate(dsItems.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM-dd") & """ /><input type=""hidden"" id=""quantity" & Counter & """ name=""quantity"" class=""quantity"" value=""" & Math.Round(dsItems.Tables(0).Rows(I).Item("curQuantity"), 0, MidpointRounding.AwayFromZero) & """ /><input type=""hidden"" name=""price"" value=""" & Math.Round(dsItems.Tables(0).Rows(I).Item("curUnitPrice"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """ /><input type=""hidden"" name=""cost"" value=""" & Math.Round(dsItems.Tables(0).Rows(I).Item("curUnitNetCost"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """ /></td>")
                Else
                    body.Append("<td></td>")
                End If
                body.Append("</tr>")
                Counter = Counter + 1
                TotalItem = TotalItem + 1
                TotalQuantity = TotalQuantity + dsItems.Tables(0).Rows(I).Item("curQuantity")
                TotalPrice = TotalPrice + dsItems.Tables(0).Rows(I).Item("curUnitPrice")
            Next
            body.Append("</tbody></table>")
            body.Append("</div>")
            body.Append("<div class=""p-0 m-0"">")
            If EditMode = True Then
                body.Append("<div class=""row""><div class=""col-md-12"">")
                body.Append("<div class=""col-md-2 pl-0""><input type=""text"" id=""txtBarcode"" placeholder=""" & plcBarcode & """ class=""form-control form-control-sm"" /></div>")
                body.Append("<div class=""col-md-3 pl-0""><input type=""text"" id=""txtItemName"" readonly=""readonly"" class=""form-control form-control-sm"" /><input type=""hidden"" id=""txtItem"" /></div>")
                body.Append("<div class=""col-md-2 pl-0""><input type=""text"" id=""dtpExpiry"" class=""form-control form-control-sm text-sm-center date-formatter dir-ltr"" /></div>")
                body.Append("<div class=""col-md-2 pl-0""><input type=""number"" id=""txtPrice"" class=""form-control form-control-sm text-sm-center"" /><input type=""hidden"" id=""txtCost"" /></div>")
                body.Append("<div class=""col-md-2 pl-0""><input type=""number"" id=""txtQuantity"" class=""form-control form-control-sm text-sm-center"" /><input type=""hidden"" id=""txtBalance"" /></div>")
                body.Append("<div class=""col-md-1 pl-0""><button type=""button"" id=""btnAdd"" class=""btn btn-success btn-sm"">Add</button></div>")
                body.Append("</div></div>")
            Else
                body.Append("<table class=""table table-bordered p-0 m-0""><tfoot>")
                body.Append("<tr><th style=""width:70px""><center>" & TotalItem & "</center></th><th style=""width:300x""></th><th style=""width:100px""></th><th style=""width:70px""><center>" & Math.Round(TotalQuantity, 2, MidpointRounding.AwayFromZero) & "</center></th><th style=""width:70px""><center>" & Math.Round(TotalPrice, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</center></th><th></th></tr>")
                body.Append("</tfoot></table>")
            End If
            body.Append("</div>")
            body.Append("</div>")
            body.Append("</div>")
            'content 3
            body.Append("<div role=""tabpanel"" class=""tab-pane"" id=""tab3"" aria-expanded=""true"" aria-labelledby=""base-tab3"">")
            body.Append("<div class=""row"">")
            body.Append("<div class=""p-0 m-0"">")
            body.Append(txtRemarks)
            body.Append("</div>")
            body.Append("</div>")
            body.Append("</div>")
            'contents ==> Close
            body.Append("</div>")
            body.Append("</form>")

            Dim scripts As String = ""
            scripts = scripts & "<script type=""text/javascript"">"
            scripts = scripts & "$('#dtpExpiry').daterangepicker({singleDatePicker: true,locale: {format:'YYYY-MM-DD'}},function (start, end, label) {});"
            'scripts = scripts & "var curTab = 1;$('a[data-toggle=""tab""]').on('shown.bs.tab', function (e) { curTab = e.target.toString().substr(e.target.toString().length - 1, 1); if(curTab!=1) {$('#btnSuspend').attr('disabled', false); $('#btnSuspend').html('<i class=""icon-clock5""></i> " & btnUnsuspend & "');} else {$('#btnSuspend').html('<i class=""icon-clock5""></i> " & btnSuspend & "');if(tabCount>" & SusbendMax & ") $('#btnSuspend').attr('disabled', true); else $('#btnSuspend').attr('disabled', false);}});"
            'scripts = scripts & "var counter" & tabCounter & "=" & rowCounter & ";var tabCount=" & tabCounter & ";if(tabCount>" & SusbendMax & ") $('#btnSuspend').attr('disabled', true);"
            'scripts = scripts & "$(document).ready(function () {$('#txtBarcode').autocomplete({triggerSelectOnValidInput: true,onInvalidateSelection: function () {$('#txtBarcode').val('');}, lookup: function (query, done) {if ($('#txtBarcode').val().length > 4) {$.ajax({type: 'POST',url: 'ajax.aspx/findItem',data: '{query: ""' + query + '""}',contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {done(jQuery.parseJSON(response.d));},failure: function (msg) {alert(msg);}, error: function (xhr, ajaxOptions, thrownError) {alert('Load Form, update form error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});} else {done(jQuery.parseJSON(''));}}, onSelect: function (suggestion) {completeBarcode(suggestion.id);$('#txtBarcode').val('');$('#txtBarcode').focus();}});});"
            'Ret = Ret & "$('#tblItems' + curTab + ' > tbody').prepend('<tr><td>' + $('#txtItem').val() + '</td><td>' + $('#txtItemName').val() + '</td><td>' + $('#dtpExpiry').val() + '</td><td>' + $('#txtQuantity').val() + '</td><td>' + $('#txtPrice').val() + '</td></tr>');"
            scripts = scripts & "var counter=" & Counter & ";"
            scripts = scripts & "$('#txtBarcode').on('change paste keyup', function () {var barcode = $(this).val();if (barcode.length != 0) {if ($.isNumeric(barcode) == true) {if (event.which == 13 || barcode.length >= 14) {event.preventDefault();$(this).val('');getItemInfo(barcode)}}}});"
            scripts = scripts & "function calcQuantity() {var q=0;$.each($('.item'), function (k, v) { if($(v).val()==$('#txtItem').val()) q=q+parseInt($('#'+v.id.replace('item','quantity')).val());});return q;}"
            scripts = scripts & "$('#btnAdd').on('click', function () {if (parseInt($('#txtQuantity').val()) + calcQuantity() > $('#txtBalance').val()) { msg('','The balance of this item is: '+$('#txtBalance').val(),'error') } else {$('#tblItems > tbody').prepend('<tr><td style=""width:70px"">' + $('#txtItem').val() + '</td><td style=""width:300px"" class=""text-md-left"" title=""' + $('#txtItemName').val() + '"">' + $('#txtItemName').val() + '</td><td style=""width:100px"">' + $('#dtpExpiry').val() + '</td><td style=""width:70px"">' + $('#txtQuantity').val() + '</td><td style=""width:70px"">' + $('#txtPrice').val() + '</td><td><button type=""button"" class=""btn btn-danger btn-xs"" onclick=""javascript:removeThis(this)"">Delete</button><input type=""hidden"" id=""item'+counter+'"" class=""item"" name=""item"" value=""'+$('#txtItem').val()+'"" /><input type=""hidden"" name=""expiry"" value=""'+$('#dtpExpiry').val()+'"" /><input type=""hidden"" id=""quantity'+counter+'"" name=""quantity"" class=""quantity"" value=""'+$('#txtQuantity').val()+'"" /><input type=""hidden"" name=""price"" value=""'+$('#txtPrice').val()+'"" /><input type=""hidden"" name=""cost"" value=""'+$('#txtCost').val()+'"" /></td></tr>');counter++;}});"
            '$('#items').val($('#items').val()+$('#txtItem').val()+',');
            'scripts = scripts & "function showCashier() {var valJson = JSON.stringify($('#frmInvoice' + curTab).serializeArray());var dataJson = { TabCounter: curTab, Fields: valJson };var dataJsonString = JSON.stringify(dataJson);$.ajax({type: 'POST',url: 'ajax.aspx/viewCashier1',data: dataJsonString,contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {if (response.d.indexOf('Err:') >= 0) {msg('',response.d.substring(4, response.d.length),'error');} else {$('#mdlMessage').html(response.d);$('#mdlMessage').modal('show');}},failure: function (msg) {alert(msg);},error: function (xhr, ajaxOptions, thrownError) {alert(' write json item, Ajax error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});}"
            'scripts = scripts & "function sendToCashier() {var valJson = JSON.stringify($('#frmInvoice' + curTab).serializeArray());var dataJson = { Fields: valJson };var dataJsonString = JSON.stringify(dataJson);$.ajax({type: 'POST',url: 'ajax.aspx/SendToCashier',data: dataJsonString,contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {if (response.d.indexOf('Err:') >= 0) {msg('',response.d.substring(4, response.d.length),'error');} else {$('#prtJS').html(response.d);closeCurrentTab();}},failure: function (msg) {alert(msg);},error: function (xhr, ajaxOptions, thrownError) {alert(' write json item, Ajax error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});}"
            'scripts = scripts & "calculateInsurance(curTab);calculateCash(curTab);$('#txtBarcode').focus();"
            'scripts = scripts & "function saveVoucher(){if($('#tblItems > tbody > tr').length>0) saveTransfer(); else msg('','No items to transfer','error') };"
            scripts = scripts & "function saveTransferExpired() {var valJson = JSON.stringify($('#frmVoucher').serializeArray());var dataJson = { lngTransaction: " & lngTransaction & ", Fields: valJson };var dataJsonString = JSON.stringify(dataJson);$.ajax({type: 'POST',url: 'ajax.aspx/saveTransferExpired',data: dataJsonString,contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {if (response.d.indexOf('Err:') >= 0) {msg('',response.d.substring(4, response.d.length),'error');} else {$('#prtJS').html(response.d);}},failure: function (msg) {alert(msg);},error: function (xhr, ajaxOptions, thrownError) {alert(' write json item, Ajax error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});}"
            scripts = scripts & "function approve() {approveItems(" & lngTransaction & ", '#mdlAlpha');}"
            'scripts = scripts & "function approveSend() {approveSendItems(" & lngTransaction & ", '#mdlAlpha');}"
            'scripts = scripts & "function approveReceive() {approveReceiveItems(" & lngTransaction & ", '#mdlAlpha');}"
            scripts = scripts & "</script>"
            body.Append(scripts)
        Catch ex As Exception
            'confirm('','" & cnfReturnToDoctor & "',returnToDoctor);
        End Try

        Dim SaveButton As String = ""
        Dim ApproveSendButton As String = ""
        Dim ApproveReceiveButton As String = ""
        Dim ApproveButton As String = ""
        Dim CancelButton As String = ""
        If EditMode = True Then SaveButton = "<button type=""button"" class=""btn btn-success ml-1 mr-1"" onclick=""javascript:if($('#tblItems > tbody > tr').length>0) saveTransferExpired(); else msg('','No items to transfer','error');""><i class=""icon-save""></i> " & btnSave & "</button> "
        If ApproveEnabled = True Then ApproveButton = " <button type=""button"" class=""btn btn-outline-primary float-left"" onclick=""javascript:confirm('','Are you sure to transfer?',approve);""><i class=""icon-upload4""></i> " & btnApprove & "</button> "
        'If ApproveSendEnabled = True Then ApproveSendButton = " <button type=""button"" class=""btn btn-outline-primary float-left"" onclick=""javascript:confirm('','Are you sure to transfer?',approveSend);""><i class=""icon-upload4""></i> " & btnApproveSend & "</button> "
        'If ApproveReceiveEnabled = True Then ApproveReceiveButton = " <button type=""button"" class=""btn btn-outline-primary float-left"" onclick=""javascript:confirm('','Are you sure to transfer?',approveReceive);""><i class=""icon-download4""></i> " & btnApproveReceive & "</button> "
        If CancelEnabled = True Then CancelButton = " <button type=""button"" class=""btn btn-outline-danger float-left ml-1 mr-1"" onclick=""javascript:if($('#tblItems > tbody > tr').length>0) saveTransfer(); else msg('','No items to transfer','error');""><i class=""icon-level-down""></i> " & btnCancel & "</button> "
        Dim Buttons As String = ApproveSendButton & ApproveReceiveButton & ApproveButton & CancelButton & SaveButton & "<button type=""button"" class=""btn btn-secondary"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnClose & "</button>"
        Dim sh As New Share.UI
        Return sh.drawModal("", body.ToString, Buttons, Share.UI.ModalSize.Large, "", "p-0")
    End Function

    Public Function viewSReturned(ByVal lngTransaction As Long, ByVal IsOut As Boolean) As String
        Dim lblVoucherNo, lblType, lblWarehouseFrom, lblWarehouseTo, lblDate, lblSender, lblRecipient, lblStatus As String
        Dim txtSendVoucher, txtSendDate, drpSendType, txtSendWarehouse, drpSendWarehouse, txtSendUser, txtSendStatus As String
        Dim txtReceiveVoucher, txtReceiveDate, drpReceiveType, txtReceiveWarehouse, drpReceiveWarehouse, txtReceiveUser, txtReceiveStatus As String
        Dim txtRemarks As String
        Dim plcBarcode, plcRemarks As String
        Dim Pending, Approved, Cancelled As String

        Dim btnClose, btnSave, btnApproveSend, btnApproveReceive, btnApprove, btnCancel As String
        Select Case byteLanguage
            Case 2
                DataLang = "Ar"

                Pending = "في الانتظار"
                Approved = "معمدة"
                Cancelled = "ملغية"

                lblVoucherNo = "رقم السند"
                lblType = "نوع السند"
                lblWarehouseFrom = "من"
                lblWarehouseTo = "إلى"
                lblDate = "التاريخ"
                lblSender = "المرسل"
                lblRecipient = "المستلم"
                lblStatus = "الحالة"

                btnSave = "حفظ التغييرات"
                btnApprove = "تعميد"
                btnApproveSend = "تعميد تسليم"
                btnApproveReceive = "تعميد استلام"
                btnCancel = "إلغاء"
                btnClose = "إغلاق"
            Case Else
                DataLang = "En"

                Pending = "Pending"
                Approved = "Approved"
                Cancelled = "Cancelled"

                lblVoucherNo = "Voucher No"
                lblType = "Voucher Type"
                lblWarehouseFrom = "From"
                lblWarehouseTo = "To"
                lblDate = "Date"
                lblSender = "Sender"
                lblRecipient = "Recipient"
                lblStatus = "Status"

                btnSave = "Save Changes"
                btnApprove = "Approve"
                btnApproveSend = "Approve Send"
                btnApproveReceive = "Approve Receive"
                btnCancel = "Cancel"
                btnClose = "Close"
        End Select

        If SReturnedString = "" Then Return "Err:You don't have warehouse assigned"

        Dim body As New StringBuilder("")

        Dim EditMode As Boolean = False
        Dim ApproveSendEnabled As Boolean = False
        Dim ApproveReceiveEnabled As Boolean = False
        Dim CancelEnabled As Boolean = False
        Dim ApproveEnabled As Boolean = False

        Try
            If lngTransaction > 0 Then
                Dim ds As DataSet
                ds = dcl.GetDS("SELECT T1.lngTransaction, T1.strTransaction AS strSendVoucher, T2.strTransaction AS strReceiveVoucher, T1.dateTransaction AS dateSendDate, T2.dateTransaction AS dateReceiveDate, TT1.strType" & DataLang & " AS strSendType, TT2.strType" & DataLang & " AS strReceiveType, W1.byteWarehouse AS byteSendWarehouse, W1.strWarehouse" & DataLang & " AS strSendWarehouse, W2.byteWarehouse AS byteReceiveWarehouse, W2.strWarehouse" & DataLang & " AS strReceiveWarehouse, TA1.strCreatedBy AS strSendUser, TA2.strCreatedBy AS strReceiveUser, T1.byteStatus AS byteSendStatus, T2.byteStatus AS byteReceiveStatus, T1.strRemarks FROM Stock_Xlink AS X1 LEFT JOIN Stock_Xlink AS X2 ON X1.lngPointer=X2.lngPointer INNER JOIN Stock_Trans AS T1 ON T1.lngTransaction=X1.lngTransaction INNER JOIN Stock_Trans AS T2 ON T2.lngTransaction=X2.lngTransaction INNER JOIN Stock_Warehouses AS W1 ON T1.byteWarehouse=W1.byteWarehouse INNER JOIN Stock_Warehouses AS W2 ON T2.byteWarehouse=W2.byteWarehouse INNER JOIN Stock_Trans_Types AS TT1 ON T1.byteTransType=TT1.byteTransType INNER JOIN Stock_Trans_Types AS TT2 ON T2.byteTransType=TT2.byteTransType INNER JOIN Stock_Trans_Audit AS TA1 ON T1.lngTransaction=TA1.lngTransaction LEFT JOIN Stock_Trans_Audit AS TA2 ON T2.lngTransaction=TA2.lngTransaction WHERE ((T1.byteBase=19 AND T2.byteBase=52) OR (T1.byteBase=51 AND T2.byteBase=20)) AND T1.lngTransaction=" & lngTransaction)
                If ds.Tables(0).Rows.Count > 0 Then
                    txtSendVoucher = ds.Tables(0).Rows(0).Item("strSendVoucher")
                    drpSendType = ds.Tables(0).Rows(0).Item("strSendType")
                    txtSendWarehouse = ds.Tables(0).Rows(0).Item("strSendWarehouse")
                    drpSendWarehouse = ds.Tables(0).Rows(0).Item("byteSendWarehouse")
                    txtSendUser = ds.Tables(0).Rows(0).Item("strSendUser")
                    txtSendDate = CDate(ds.Tables(0).Rows(0).Item("dateSendDate")).ToString(strDateFormat)
                    txtRemarks = ds.Tables(0).Rows(0).Item("strRemarks").ToString

                    Select Case ds.Tables(0).Rows(0).Item("byteSendStatus").ToString
                        Case 0
                            txtSendStatus = Cancelled
                            EditMode = False
                            'ApproveSendEnabled = False
                            ApproveEnabled = False
                            CancelEnabled = False
                        Case 1
                            txtSendStatus = Pending
                            EditMode = True
                            'ApproveSendEnabled = True
                            ApproveEnabled = True
                            CancelEnabled = False
                        Case 2
                            txtSendStatus = Approved
                        Case Else
                            txtSendStatus = ""
                            EditMode = False
                            'ApproveSendEnabled = False
                            ApproveEnabled = False
                            CancelEnabled = True
                    End Select

                    If IsDBNull(ds.Tables(0).Rows(0).Item("strReceiveVoucher")) Then txtReceiveVoucher = "" Else txtReceiveVoucher = ds.Tables(0).Rows(0).Item("strReceiveVoucher")
                    If IsDBNull(ds.Tables(0).Rows(0).Item("strReceiveType")) Then drpReceiveType = "" Else drpReceiveType = ds.Tables(0).Rows(0).Item("strReceiveType")
                    If IsDBNull(ds.Tables(0).Rows(0).Item("strReceiveWarehouse")) Then txtReceiveWarehouse = "" Else txtReceiveWarehouse = ds.Tables(0).Rows(0).Item("strReceiveWarehouse")
                    If IsDBNull(ds.Tables(0).Rows(0).Item("byteReceiveWarehouse")) Then drpReceiveWarehouse = "" Else drpReceiveWarehouse = ds.Tables(0).Rows(0).Item("byteReceiveWarehouse")
                    If IsDBNull(ds.Tables(0).Rows(0).Item("strReceiveUser")) Then txtReceiveUser = "" Else txtReceiveUser = ds.Tables(0).Rows(0).Item("strReceiveUser")
                    If IsDBNull(ds.Tables(0).Rows(0).Item("dateReceiveDate")) Then txtReceiveDate = "" Else txtReceiveDate = CDate(ds.Tables(0).Rows(0).Item("dateReceiveDate")).ToString(strDateFormat)

                    Select Case ds.Tables(0).Rows(0).Item("byteReceiveStatus").ToString
                        Case 0
                            txtReceiveStatus = Cancelled
                            'ApproveReceiveEnabled = False
                        Case 1
                            txtReceiveStatus = Pending
                            If ds.Tables(0).Rows(0).Item("byteSendStatus").ToString = 2 And ds.Tables(0).Rows(0).Item("byteReceiveWarehouse") = byteWarehouse Then ApproveReceiveEnabled = True Else ApproveReceiveEnabled = False
                        Case 2
                            txtReceiveStatus = Approved
                            'ApproveReceiveEnabled = False
                        Case Else
                            txtReceiveStatus = ""
                            'ApproveReceiveEnabled = False
                    End Select

                    'Dim dsOther As DataSet
                    'If ds.Tables(0).Rows(0).Item("byteBase") = 20 Then
                    '    dsOther = dcl.GetDS("SELECT * FROM Stock_Trans AS T INNER JOIN Stock_Warehouses AS W ON T.byteWarehouse=W.byteWarehouse WHERE T.strTransaction = '" & txtVoucherNo & "' AND T.byteBase=19")
                    'Else
                    '    dsOther = dcl.GetDS("SELECT * FROM Stock_Trans AS T INNER JOIN Stock_Warehouses AS W ON T.byteWarehouse=W.byteWarehouse WHERE T.strTransaction = '" & txtVoucherNo & "' AND T.byteBase=20")
                    'End If
                    'drpWarehouseTo = dsOther.Tables(0).Rows(0).Item("strWarehouse" & DataLang)

                    txtSendVoucher = "<input type=""text"" id=""txtSendVoucher"" name=""txtSendVoucher"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value=""" & txtSendVoucher & """ />"
                    txtReceiveVoucher = "<input type=""text"" id=""txtReceiveVoucher"" name=""txtReceiveVoucher"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value=""" & txtReceiveVoucher & """ />"
                    txtSendDate = "<input type=""text"" id=""txtSendDate"" name=""txtSendDate"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value=""" & txtSendDate & """ />"
                    txtReceiveDate = "<input type=""text"" id=""txtReceiveDate"" name=""txtReceiveDate"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value=""" & txtReceiveDate & """ />"
                    drpSendType = "<input type=""text"" id=""drpSendType"" name=""drpSendType"" class=""form-control form-control-sm"" readonly=""readonly"" value=""" & drpSendType & """ />"
                    drpReceiveType = "<input type=""text"" id=""drpReceiveType"" name=""drpReceiveType"" class=""form-control form-control-sm"" readonly=""readonly"" value=""" & drpReceiveType & """ />"
                    drpSendWarehouse = "<input type=""hidden"" id=""drpSendWarehouse"" name=""drpSendWarehouse"" value=""" & drpSendWarehouse & """ /><input type=""text"" class=""form-control form-control-sm"" readonly=""readonly"" value=""" & txtSendWarehouse & """ />"
                    drpReceiveWarehouse = "<input type=""hidden"" id=""drpReceiveWarehouse"" name=""drpReceiveWarehouse"" value=""" & drpReceiveWarehouse & """ /><input type=""text"" class=""form-control form-control-sm"" readonly=""readonly"" value=""" & txtReceiveWarehouse & """ />"
                    txtSendUser = "<input type=""text"" id=""txtSendUser"" name=""txtSendUser"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value=""" & txtSendUser & """ />"
                    txtReceiveUser = "<input type=""text"" id=""txtReceiveUser"" name=""txtReceiveUser"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value=""" & txtReceiveUser & """ />"
                    txtSendStatus = "<input type=""text"" id=""txtSendStatus"" name=""txtSendStatus"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value=""" & txtSendStatus & """ />"
                    txtReceiveStatus = "<input type=""text"" id=""txtReceiveStatus"" name=""txtReceiveStatus"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value=""" & txtReceiveStatus & """ />"

                    txtRemarks = "<textarea id=""txtRemarks"" rows=""15"" class=""form-control"" name=""txtRemarks"" placeholder=""About Transfer"" readonly=""readonly"">" & txtRemarks & "</textarea>"
                Else
                    Return "Err: Record not found"
                End If
            Else
                EditMode = True
                ApproveSendEnabled = False
                ApproveReceiveEnabled = False
                CancelEnabled = False
                Dim dsLast As DataSet = dcl.GetDS("SELECT MAX(CAST(strTransaction AS bigint)) AS Last FROM Stock_Trans WHERE YEAR(dateTransaction) = " & intYear & " AND byteBase=52")
                Dim lngNewVoucher As Long
                If IsDBNull(dsLast.Tables(0).Rows(0).Item("Last")) Then
                    lngNewVoucher = 1
                Else
                    lngNewVoucher = dsLast.Tables(0).Rows(0).Item("Last") + 1
                End If

                Dim dsTypeFrom, dsTypeTo As DataSet
                Dim TypeFrom, TypeTo As Integer
                Dim TypeFromName, TypeToName As String
                If IsOut = False Then
                    TypeFrom = 14
                    dsTypeFrom = dcl.GetDS("SELECT * FROM Stock_Trans_Types WHERE byteTransType=14")
                    TypeFromName = dsTypeFrom.Tables(0).Rows(0).Item("strType" & DataLang)
                    TypeTo = 40
                    dsTypeTo = dcl.GetDS("SELECT * FROM Stock_Trans_Types WHERE byteTransType=40")
                    TypeToName = dsTypeTo.Tables(0).Rows(0).Item("strType" & DataLang)
                Else
                    TypeFrom = 39
                    dsTypeFrom = dcl.GetDS("SELECT * FROM Stock_Trans_Types WHERE byteTransType=39")
                    TypeFromName = dsTypeFrom.Tables(0).Rows(0).Item("strType" & DataLang)
                    TypeTo = 15
                    dsTypeTo = dcl.GetDS("SELECT * FROM Stock_Trans_Types WHERE byteTransType=15")
                    TypeToName = dsTypeTo.Tables(0).Rows(0).Item("strType" & DataLang)
                End If

                Dim CurrentWarehouseID As Byte = 0
                Dim CurrectWarehouse, OtherWarehouse As String
                CurrectWarehouse = ""
                OtherWarehouse = ""
                Dim where As String = ""
                Dim WarehouseList As String = ""

                If SReturnedString <> "*" Then where = "AND byteWarehouse IN (" & SReturnedString & ")"
                Dim dsWarehouse As DataSet = dcl.GetDS("SELECT * FROM Stock_Warehouses WHERE bActive=1 " & where)
                For I = 0 To dsWarehouse.Tables(0).Rows.Count - 1
                    WarehouseList = WarehouseList & "<option value=""" & dsWarehouse.Tables(0).Rows(I).Item("byteWarehouse") & """>" & dsWarehouse.Tables(0).Rows(I).Item("strWarehouse" & DataLang) & "</option>"
                Next

                Dim WarehouseList2 As String = ""
                Dim dsWarehouse2 As DataSet = dcl.GetDS("SELECT * FROM Stock_Warehouses WHERE bActive=1" & where)
                For I = 0 To dsWarehouse2.Tables(0).Rows.Count - 1
                    'If dsWarehouse.Tables(0).Rows(I).Item("byteWarehouse") = byteWarehouse Then
                    '    CurrentWarehouseID = dsWarehouse.Tables(0).Rows(I).Item("byteWarehouse")
                    '    CurrectWarehouse = dsWarehouse.Tables(0).Rows(I).Item("strWarehouse" & DataLang)
                    'Else
                    '    OtherWarehouse = OtherWarehouse & "<option value=""" & dsWarehouse.Tables(0).Rows(I).Item("byteWarehouse") & """>" & dsWarehouse.Tables(0).Rows(I).Item("strWarehouse" & DataLang) & "</option>"
                    'End If
                    WarehouseList2 = WarehouseList2 & "<option value=""" & dsWarehouse2.Tables(0).Rows(I).Item("byteWarehouse") & """>" & dsWarehouse2.Tables(0).Rows(I).Item("strWarehouse" & DataLang) & "</option>"
                Next


                txtSendVoucher = "<input type=""text"" id=""txtSendVoucher"" name=""txtSendVoucher"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value=""" & lngNewVoucher & """ />"
                txtReceiveVoucher = "<input type=""text"" id=""txtReceiveVoucher"" name=""txtReceiveVoucher"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value="""" />"
                txtSendDate = "<input type=""text"" id=""txtSendDate"" name=""txtSendDate"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value=""" & Today.ToString(strDateFormat) & """ />"
                txtReceiveDate = "<input type=""text"" id=""txtReceiveDate"" name=""txtReceiveDate"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value="""" />"
                drpSendType = "<input type=""hidden"" id=""drpSendType"" name=""drpSendType"" value=""" & TypeFrom & """ /><input type=""text"" class=""form-control form-control-sm"" readonly=""readonly"" value=""" & TypeFromName & """ />"
                drpReceiveType = "<input type=""hidden"" id=""drpReceiveType"" name=""drpReceiveType"" value=""" & TypeTo & """ /><input type=""text"" class=""form-control form-control-sm"" readonly=""readonly"" value=""" & TypeToName & """ />"
                'drpSendWarehouse = "<input type=""hidden"" id=""drpSendWarehouse"" name=""drpSendWarehouse"" value=""" & CurrentWarehouseID & """ /><input type=""text"" class=""form-control form-control-sm"" readonly=""readonly"" value=""" & CurrectWarehouse & """ />"
                'drpReceiveWarehouse = "<input type=""hidden"" id=""drpReceiveWarehouse"" name=""drpReceiveWarehouse"" value=""" & CurrentWarehouseID & """ /><input type=""text"" class=""form-control form-control-sm"" readonly=""readonly"" value=""" & CurrectWarehouse & """ />"
                drpSendWarehouse = "<select id=""drpSendWarehouse"" name=""drpSendWarehouse"" class=""form-control form-control-sm"">" & WarehouseList & "</select>"
                drpReceiveWarehouse = "<select id=""drpReceiveWarehouse"" name=""drpReceiveWarehouse"" class=""form-control form-control-sm"">" & WarehouseList2 & "</select>"
                txtSendUser = "<input type=""text"" id=""txtSendUser"" name=""txtSendUser"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value=""" & strUserName & """ />"
                txtReceiveUser = "<input type=""text"" id=""txtReceiveUser"" name=""txtReceiveUser"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value="""" />"
                txtSendStatus = "<input type=""hidden"" id=""txtSendStatus"" name=""txtSendStatus"" value=""1"" /><input type=""text"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value=""" & Pending & """ />"
                txtReceiveStatus = "<input type=""text"" id=""txtReceiveStatus"" name=""txtReceiveStatus"" class=""form-control form-control-sm text-md-center"" readonly=""readonly"" value="""" />"

                txtRemarks = "<textarea id=""txtRemarks"" rows=""15"" class=""form-control"" name=""txtRemarks"" placeholder=""About Transfer"">" & txtRemarks & "</textarea>"
            End If
        Catch ex As Exception
            Return "Err:" & ex.Message
        End Try

        Try
            'tabs
            body.Append("<form id=""frmVoucher"" style=""height:400px"">")
            body.Append("<ul class=""nav nav-tabs m-0"">")
            body.Append("<li class=""nav-item""><a class=""nav-link active"" id=""base-tab1"" data-toggle=""tab"" aria-controls=""tab1"" href=""#tab1"" aria-expanded=""true"" >Transfer</a></li>")
            body.Append("<li class=""nav-item""><a class=""nav-link"" id=""base-tab2"" data-toggle=""tab"" aria-controls=""tab2"" href=""#tab2"" aria-expanded=""true"" >Items</a></li>")
            body.Append("<li class=""nav-item""><a class=""nav-link"" id=""base-tab3"" data-toggle=""tab"" aria-controls=""tab3"" href=""#tab3"" aria-expanded=""true"" >Remarks</a></li>")
            body.Append("</ul>")
            'contents ==> Open
            body.Append("<div class=""tab-content m-0 p-1"">")
            'content 1
            body.Append("<div role=""tabpanel"" class=""tab-pane active"" id=""tab1"" aria-expanded=""true"" aria-labelledby=""base-tab1"">")
            body.Append("<form id=""frmItem"">")
            body.Append("<div class=""row"">")
            body.Append("<div class=""col-md-6"">")
            body.Append("<div class=""col-md-12 border border-cyan border-lighten-4 p-0 pb-1"">")
            body.Append("<div class=""col-md-12 bg-cyan bg-lighten-4 blue pt-1""><h3>Transfer From</h3></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblVoucherNo & ":</div><div class=""col-md-4"">" & txtSendVoucher & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblType & ":</div><div class=""col-md-8"">" & drpSendType & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblWarehouseFrom & ":</div><div class=""col-md-8"">" & drpSendWarehouse & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblDate & ":</div><div class=""col-md-4"">" & txtSendDate & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblSender & ":</div><div class=""col-md-4"">" & txtSendUser & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblStatus & ":</div><div class=""col-md-4"">" & txtSendStatus & "</div></div>")
            body.Append("</div>")
            body.Append("</div>")
            body.Append("<div class=""col-md-6"">")
            body.Append("<div class=""col-md-12 border border-orange  border-lighten-4 p-0 pb-1"">")
            body.Append("<div class=""col-md-12 bg-orange  bg-lighten-4 orange pt-1""><h3>Transfer To</h3></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblVoucherNo & ":</div><div class=""col-md-4"">" & txtReceiveVoucher & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblType & ":</div><div class=""col-md-8"">" & drpReceiveType & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblWarehouseTo & ":</div><div class=""col-md-8"">" & drpReceiveWarehouse & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblDate & ":</div><div class=""col-md-4"">" & txtReceiveDate & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblRecipient & ":</div><div class=""col-md-4"">" & txtReceiveUser & "</div></div>")
            body.Append("<div class=""col-md-12 mt-1""><div class=""col-md-4 text-sm-right"">" & lblStatus & ":</div><div class=""col-md-4"">" & txtReceiveStatus & "</div></div>")
            body.Append("</div>")
            body.Append("</div>")
            body.Append("</div>")
            body.Append("</form>")
            body.Append("</div>")
            'content 2
            body.Append("<div role=""tabpanel"" class=""tab-pane"" id=""tab2"" aria-expanded=""true"" aria-labelledby=""base-tab2"">")
            body.Append("<div class=""row"">")
            body.Append("<div class=""p-0 m-0"">")
            body.Append("<table class=""table table-bordered p-0 m-0""><thead>")
            body.Append("<tr><th style=""width:70px""><center>Item</center></th><th style=""width:300px""><center>Item Name</center></th><th style=""width:100px""><center>Expire</center></th><th style=""width:70px""><center>Quantity</center></th><th style=""width:70px""><center>Price</center></th><th><center></center></th></tr>")
            body.Append("</thead></table>")
            body.Append("</div>")
            body.Append("<div class=""p-0 m-0"" style=""height:253px; overflow-x:auto;"">")
            body.Append("<table id=""tblItems"" class=""table table-bordered p-0 m-0""><tbody>")
            Dim dsItems As DataSet = dcl.GetDS("SELECT * FROM Stock_Xlink AS X INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink=XI.lngXlink INNER JOIN Stock_Items AS I ON XI.strItem=I.strItem WHERE X.lngTransaction=" & lngTransaction)
            Dim ItemName As String
            Dim Counter As Integer = 0
            Dim TotalQuantity, TotalItem, TotalPrice As Decimal
            For I = 0 To dsItems.Tables(0).Rows.Count - 1
                If Len(dsItems.Tables(0).Rows(I).Item("strItem" & DataLang)) > 27 Then ItemName = Mid(dsItems.Tables(0).Rows(I).Item("strItem" & DataLang), 1, 27) & "..." Else ItemName = dsItems.Tables(0).Rows(I).Item("strItem" & DataLang)
                body.Append("<tr>")
                body.Append("<td style=""width:70px"">" & dsItems.Tables(0).Rows(I).Item("strItem") & "</td>")
                body.Append("<td style=""width:300px"" class=""text-md-left"" title=""" & dsItems.Tables(0).Rows(I).Item("strItem" & DataLang) & """>" & ItemName & "</td>")
                body.Append("<td style=""width:100px"">" & CDate(dsItems.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM-dd") & "</td>")
                body.Append("<td style=""width:70px"">" & Math.Round(dsItems.Tables(0).Rows(I).Item("curQuantity"), 0, MidpointRounding.AwayFromZero) & "</td>")
                body.Append("<td style=""width:70px"">" & Math.Round(dsItems.Tables(0).Rows(I).Item("curUnitPrice"), byteCurrencyRound, MidpointRounding.AwayFromZero) & "</td>")
                If EditMode = True Then
                    body.Append("<td><button type=""button"" class=""btn btn-danger btn-xs"" onclick=""javascript:removeThis(this)"">Delete</button><input type=""hidden"" id=""item" & Counter & """ class=""item"" name=""item"" value=""" & dsItems.Tables(0).Rows(I).Item("strItem") & """ /><input type=""hidden"" name=""expiry"" value=""" & CDate(dsItems.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM-dd") & """ /><input type=""hidden"" id=""quantity" & Counter & """ name=""quantity"" class=""quantity"" value=""" & Math.Round(dsItems.Tables(0).Rows(I).Item("curQuantity"), 0, MidpointRounding.AwayFromZero) & """ /><input type=""hidden"" name=""price"" value=""" & Math.Round(dsItems.Tables(0).Rows(I).Item("curUnitPrice"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """ /><input type=""hidden"" name=""cost"" value=""" & Math.Round(dsItems.Tables(0).Rows(I).Item("curUnitNetCost"), byteCurrencyRound, MidpointRounding.AwayFromZero) & """ /></td>")
                Else
                    body.Append("<td></td>")
                End If
                body.Append("</tr>")
                Counter = Counter + 1
                TotalItem = TotalItem + 1
                TotalQuantity = TotalQuantity + dsItems.Tables(0).Rows(I).Item("curQuantity")
                TotalPrice = TotalPrice + dsItems.Tables(0).Rows(I).Item("curUnitPrice")
            Next
            body.Append("</tbody></table>")
            body.Append("</div>")
            body.Append("<div class=""p-0 m-0"">")
            If EditMode = True Then
                body.Append("<div class=""row""><div class=""col-md-12"">")
                body.Append("<div class=""col-md-10 pl-0""><input type=""text"" id=""txtBarcode"" placeholder=""" & plcBarcode & """ class=""form-control form-control-sm"" /></div>")
                body.Append("<div class=""col-md-2 pl-0""><button type=""button"" id=""btnAdd"" class=""btn btn-success btn-sm"">Append</button> <button type=""button"" id=""btnAdd"" class=""btn btn-success btn-sm"">Collect</button></div>")
                'body.Append("<div class=""col-md-2 pl-0""><input type=""text"" id=""txtBarcode"" placeholder=""" & plcBarcode & """ class=""form-control form-control-sm"" /></div>")
                'body.Append("<div class=""col-md-3 pl-0""><input type=""text"" id=""txtItemName"" readonly=""readonly"" class=""form-control form-control-sm"" /><input type=""hidden"" id=""txtItem"" /></div>")
                'body.Append("<div class=""col-md-2 pl-0""><input type=""text"" id=""dtpExpiry"" class=""form-control form-control-sm text-sm-center date-formatter dir-ltr"" /></div>")
                'body.Append("<div class=""col-md-2 pl-0""><input type=""number"" id=""txtPrice"" class=""form-control form-control-sm text-sm-center"" /><input type=""hidden"" id=""txtCost"" /></div>")
                'body.Append("<div class=""col-md-2 pl-0""><input type=""number"" id=""txtQuantity"" class=""form-control form-control-sm text-sm-center"" /><input type=""hidden"" id=""txtBalance"" /></div>")
                'body.Append("<div class=""col-md-1 pl-0""><button type=""button"" id=""btnAdd"" class=""btn btn-success btn-sm"">Add</button></div>")
                body.Append("</div></div>")
            Else
                body.Append("<table class=""table table-bordered p-0 m-0""><tfoot>")
                body.Append("<tr><th style=""width:70px""><center>" & TotalItem & "</center></th><th style=""width:300x""></th><th style=""width:100px""></th><th style=""width:70px""><center>" & Math.Round(TotalQuantity, 2, MidpointRounding.AwayFromZero) & "</center></th><th style=""width:70px""><center>" & Math.Round(TotalPrice, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</center></th><th></th></tr>")
                body.Append("</tfoot></table>")
            End If
            body.Append("</div>")
            body.Append("</div>")
            body.Append("</div>")
            'content 3
            body.Append("<div role=""tabpanel"" class=""tab-pane"" id=""tab3"" aria-expanded=""true"" aria-labelledby=""base-tab3"">")
            body.Append("<div class=""row"">")
            body.Append("<div class=""p-0 m-0"">")
            body.Append(txtRemarks)
            body.Append("</div>")
            body.Append("</div>")
            body.Append("</div>")
            'contents ==> Close
            body.Append("</div>")
            body.Append("</form>")

            Dim scripts As String = ""
            scripts = scripts & "<script type=""text/javascript"">"
            scripts = scripts & "$('#dtpExpiry').daterangepicker({singleDatePicker: true,locale: {format:'YYYY-MM-DD'}},function (start, end, label) {});"
            'scripts = scripts & "var curTab = 1;$('a[data-toggle=""tab""]').on('shown.bs.tab', function (e) { curTab = e.target.toString().substr(e.target.toString().length - 1, 1); if(curTab!=1) {$('#btnSuspend').attr('disabled', false); $('#btnSuspend').html('<i class=""icon-clock5""></i> " & btnUnsuspend & "');} else {$('#btnSuspend').html('<i class=""icon-clock5""></i> " & btnSuspend & "');if(tabCount>" & SusbendMax & ") $('#btnSuspend').attr('disabled', true); else $('#btnSuspend').attr('disabled', false);}});"
            'scripts = scripts & "var counter" & tabCounter & "=" & rowCounter & ";var tabCount=" & tabCounter & ";if(tabCount>" & SusbendMax & ") $('#btnSuspend').attr('disabled', true);"
            'scripts = scripts & "$(document).ready(function () {$('#txtBarcode').autocomplete({triggerSelectOnValidInput: true,onInvalidateSelection: function () {$('#txtBarcode').val('');}, lookup: function (query, done) {if ($('#txtBarcode').val().length > 4) {$.ajax({type: 'POST',url: 'ajax.aspx/findItem',data: '{query: ""' + query + '""}',contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {done(jQuery.parseJSON(response.d));},failure: function (msg) {alert(msg);}, error: function (xhr, ajaxOptions, thrownError) {alert('Load Form, update form error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});} else {done(jQuery.parseJSON(''));}}, onSelect: function (suggestion) {completeBarcode(suggestion.id);$('#txtBarcode').val('');$('#txtBarcode').focus();}});});"
            'Ret = Ret & "$('#tblItems' + curTab + ' > tbody').prepend('<tr><td>' + $('#txtItem').val() + '</td><td>' + $('#txtItemName').val() + '</td><td>' + $('#dtpExpiry').val() + '</td><td>' + $('#txtQuantity').val() + '</td><td>' + $('#txtPrice').val() + '</td></tr>');"
            scripts = scripts & "var counter=" & Counter & ";"
            scripts = scripts & "$('#txtBarcode').on('change paste keyup', function () {var barcode = $(this).val();if (barcode.length != 0) {if ($.isNumeric(barcode) == true) {if (event.which == 13 || barcode.length >= 14) {event.preventDefault();$(this).val('');getItemInfo(barcode)}}}});"
            scripts = scripts & "function calcQuantity() {var q=0;$.each($('.item'), function (k, v) { if($(v).val()==$('#txtItem').val()) q=q+parseInt($('#'+v.id.replace('item','quantity')).val());});return q;}"
            scripts = scripts & "$('#btnAdd').on('click', function () {if (parseInt($('#txtQuantity').val()) + calcQuantity() > $('#txtBalance').val()) { msg('','The balance of this item is: '+$('#txtBalance').val(),'error') } else {$('#tblItems > tbody').prepend('<tr><td style=""width:70px"">' + $('#txtItem').val() + '</td><td style=""width:300px"" class=""text-md-left"" title=""' + $('#txtItemName').val() + '"">' + $('#txtItemName').val() + '</td><td style=""width:100px"">' + $('#dtpExpiry').val() + '</td><td style=""width:70px"">' + $('#txtQuantity').val() + '</td><td style=""width:70px"">' + $('#txtPrice').val() + '</td><td><button type=""button"" class=""btn btn-danger btn-xs"" onclick=""javascript:removeThis(this)"">Delete</button><input type=""hidden"" id=""item'+counter+'"" class=""item"" name=""item"" value=""'+$('#txtItem').val()+'"" /><input type=""hidden"" name=""expiry"" value=""'+$('#dtpExpiry').val()+'"" /><input type=""hidden"" id=""quantity'+counter+'"" name=""quantity"" class=""quantity"" value=""'+$('#txtQuantity').val()+'"" /><input type=""hidden"" name=""price"" value=""'+$('#txtPrice').val()+'"" /><input type=""hidden"" name=""cost"" value=""'+$('#txtCost').val()+'"" /></td></tr>');counter++;}});"
            '$('#items').val($('#items').val()+$('#txtItem').val()+',');
            'scripts = scripts & "function showCashier() {var valJson = JSON.stringify($('#frmInvoice' + curTab).serializeArray());var dataJson = { TabCounter: curTab, Fields: valJson };var dataJsonString = JSON.stringify(dataJson);$.ajax({type: 'POST',url: 'ajax.aspx/viewCashier1',data: dataJsonString,contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {if (response.d.indexOf('Err:') >= 0) {msg('',response.d.substring(4, response.d.length),'error');} else {$('#mdlMessage').html(response.d);$('#mdlMessage').modal('show');}},failure: function (msg) {alert(msg);},error: function (xhr, ajaxOptions, thrownError) {alert(' write json item, Ajax error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});}"
            'scripts = scripts & "function sendToCashier() {var valJson = JSON.stringify($('#frmInvoice' + curTab).serializeArray());var dataJson = { Fields: valJson };var dataJsonString = JSON.stringify(dataJson);$.ajax({type: 'POST',url: 'ajax.aspx/SendToCashier',data: dataJsonString,contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {if (response.d.indexOf('Err:') >= 0) {msg('',response.d.substring(4, response.d.length),'error');} else {$('#prtJS').html(response.d);closeCurrentTab();}},failure: function (msg) {alert(msg);},error: function (xhr, ajaxOptions, thrownError) {alert(' write json item, Ajax error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});}"
            'scripts = scripts & "calculateInsurance(curTab);calculateCash(curTab);$('#txtBarcode').focus();"
            'scripts = scripts & "function saveVoucher(){if($('#tblItems > tbody > tr').length>0) saveTransfer(); else msg('','No items to transfer','error') };"
            scripts = scripts & "function saveTransferReturned() {var valJson = JSON.stringify($('#frmVoucher').serializeArray());var dataJson = { lngTransaction: " & lngTransaction & ", Fields: valJson };var dataJsonString = JSON.stringify(dataJson);$.ajax({type: 'POST',url: 'ajax.aspx/saveTransferReturned',data: dataJsonString,contentType: 'application/json; charset=utf-8',dataType: 'json',success: function (response) {if (response.d.indexOf('Err:') >= 0) {msg('',response.d.substring(4, response.d.length),'error');} else {$('#prtJS').html(response.d);}},failure: function (msg) {alert(msg);},error: function (xhr, ajaxOptions, thrownError) {alert(' write json item, Ajax error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);}});}"
            scripts = scripts & "function approve() {approveItems(" & lngTransaction & ", '#mdlAlpha');}"
            'scripts = scripts & "function approveSend() {approveSendItems(" & lngTransaction & ", '#mdlAlpha');}"
            'scripts = scripts & "function approveReceive() {approveReceiveItems(" & lngTransaction & ", '#mdlAlpha');}"
            scripts = scripts & "</script>"
            body.Append(scripts)
        Catch ex As Exception
            'confirm('','" & cnfReturnToDoctor & "',returnToDoctor);
        End Try

        Dim SaveButton As String = ""
        Dim ApproveSendButton As String = ""
        Dim ApproveReceiveButton As String = ""
        Dim ApproveButton As String = ""
        Dim CancelButton As String = ""
        If EditMode = True Then SaveButton = "<button type=""button"" class=""btn btn-success ml-1 mr-1"" onclick=""javascript:if($('#tblItems > tbody > tr').length>0) saveTransferReturned(); else msg('','No items to transfer','error');""><i class=""icon-save""></i> " & btnSave & "</button> "
        If ApproveEnabled = True Then ApproveButton = " <button type=""button"" class=""btn btn-outline-primary float-left"" onclick=""javascript:confirm('','Are you sure to transfer?',approve);""><i class=""icon-upload4""></i> " & btnApprove & "</button> "
        'If ApproveSendEnabled = True Then ApproveSendButton = " <button type=""button"" class=""btn btn-outline-primary float-left"" onclick=""javascript:confirm('','Are you sure to transfer?',approveSend);""><i class=""icon-upload4""></i> " & btnApproveSend & "</button> "
        'If ApproveReceiveEnabled = True Then ApproveReceiveButton = " <button type=""button"" class=""btn btn-outline-primary float-left"" onclick=""javascript:confirm('','Are you sure to transfer?',approveReceive);""><i class=""icon-download4""></i> " & btnApproveReceive & "</button> "
        If CancelEnabled = True Then CancelButton = " <button type=""button"" class=""btn btn-outline-danger float-left ml-1 mr-1"" onclick=""javascript:if($('#tblItems > tbody > tr').length>0) saveTransfer(); else msg('','No items to transfer','error');""><i class=""icon-level-down""></i> " & btnCancel & "</button> "
        Dim Buttons As String = ApproveSendButton & ApproveReceiveButton & ApproveButton & CancelButton & SaveButton & "<button type=""button"" class=""btn btn-secondary"" data-dismiss=""modal""><i class=""icon-cross2""></i> " & btnClose & "</button>"
        Dim sh As New Share.UI
        Return sh.drawModal("", body.ToString, Buttons, Share.UI.ModalSize.Large, "", "p-0")
    End Function

    Public Function completeBarcode(ByVal strBarcode As String, ByVal byteWarehouse As Byte, ByVal byteBase As Byte, ByVal strFunction As String) As String
        Dim str As String = ""
        Dim strItem As String
        'Dim ExpireDate As String
        Dim Price As Decimal
        Dim ds As DataSet
        Dim lstExpire As String = ""
        Dim Header As String
        Dim btnSave, btnClose As String
        Dim lblExpiryDate, lblBasePrice As String
        Dim DisablePriceEdit As Boolean = True ' should be in for user permissions

        Select Case byteLanguage
            Case 2
                DataLang = "Ar"
                Header = "تاريخ الإنتهاء والسعر"
                lblExpiryDate = "تاريخ الإنتهاء"
                lblBasePrice = "سعر الصنف"
                btnSave = "حفظ"
                btnClose = "إغلاق"
            Case Else
                DataLang = "En"
                Header = "Expire Date & Price"
                lblExpiryDate = "Expire Date"
                lblBasePrice = "Item Price"
                btnSave = "Save"
                btnClose = "Close"
        End Select

        strItem = strBarcode
        ds = dcl.GetDS("SELECT * FROM Stock_Items WHERE strItem='" & strItem & "'")
        If ds.Tables(0).Rows.Count > 0 Then
            '1=> get list of expire date
            Dim dsExpire As DataSet
            If byteBase > 0 Then
                dsExpire = dcl.GetDS("")
            Else
                'default balance
                dsExpire = dcl.GetDS("SELECT XI.byteWarehouse, W.strWarehouseEn, CONVERT(varchar(10), XI.dateExpiry, 120) AS dateExpiry, SUM(B.intSign * XI.curQuantity * U.curFactor) AS curBalance FROM Stock_Base AS B INNER JOIN Stock_Trans AS T ON B.byteBase = T.byteBase INNER JOIN Stock_Xlink AS X ON T.lngTransaction = X.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink INNER JOIN Stock_Units AS U ON U.byteUnit = XI.byteUnit INNER JOIN Stock_Warehouses AS W ON XI.byteWarehouse=W.byteWarehouse WHERE T.byteStatus > 0 And B.bInclude <> 0 And Year(T.dateTransaction) = " & intYear & " AND XI.byteWarehouse = " & byteWarehouse & " AND XI.strItem='" & strItem & "' AND T.dateTransaction <= '" & Today.ToString("yyyy-MM-dd") & "' GROUP BY XI.byteWarehouse, W.strWarehouseEn, XI.dateExpiry HAVING SUM(B.intSign * XI.curQuantity * U.curFactor) > 0 ORDER BY XI.dateExpiry")
            End If

            Dim firstChecked As Boolean = False
            Dim active, checked As String
            If dsExpire.Tables(0).Rows.Count > 0 Then
                For I = 0 To dsExpire.Tables(0).Rows.Count - 1
                    If firstChecked = False Then
                        firstChecked = True
                        active = "active"
                        checked = "checked=""checked"""
                    Else
                        active = ""
                        checked = ""
                    End If
                    lstExpire = lstExpire & "<label class=""btn btn-secondary " & active & """><input type=""radio"" name=""dateExpiry"" value=""" & CDate(dsExpire.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM-dd") & """ " & checked & ">" & CDate(dsExpire.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM") & " ==> (<b>" & Math.Round(dsExpire.Tables(0).Rows(I).Item("curBalance"), 0, MidpointRounding.AwayFromZero) & "</b>)" & "</label>"
                Next
            Else
                Return "Err:No balance for this item.."
            End If
            '2=> get price from purchases if any
            Dim dsPrice As DataSet
            'dsPrice = dcl.GetDS("SELECT XI.curUnitPrice As Price FROM Stock_Trans AS T INNER JOIN (Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink = X.lngXlink) ON T.lngTransaction = X.lngTransaction WHERE XI.curUnitPrice Is Not Null AND T.byteStatus>0 AND XI.byteQuantityType=1 AND T.byteBase IN (10, 40) And XI.strItem='" & strItem & "' ORDER BY T.dateTransaction DESC")
            dsPrice = dcl.GetDS("SELECT XI.curBasePrice As Price FROM Stock_Trans AS T INNER JOIN (Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink = X.lngXlink) ON T.lngTransaction = X.lngTransaction WHERE XI.curBasePrice Is Not Null AND T.byteStatus>0 AND XI.byteQuantityType=1 AND T.byteBase IN (10,40) And XI.strItem='" & strItem & "' ORDER BY T.dateTransaction DESC")
            If dsPrice.Tables(0).Rows.Count > 0 Then
                Price = dsPrice.Tables(0).Rows(0).Item("Price")
            Else
                Price = 0
            End If

            '3=> buile UI
            Dim disabled As String = ""
            If DisablePriceEdit = True Then disabled = "readonly=""readonly"""
            lstExpire = "<div class=""btn-group btn-group-toggle btn-group-vertical full-width"" data-toggle=""buttons"">" & lstExpire & "</div>"
            str = str & "<div class=""row pl-1 pr-1"">"
            str = str & "<div class=""form-group""><label>" & lblExpiryDate & ":</label>" & lstExpire & "</div>"
            str = str & "<div class=""form-group""><label>" & lblBasePrice & ":</label><input class=""form-control text-md-center"" id=""curBasePrice"" type=""number"" value=""" & Math.Round(Price, byteCurrencyRound, MidpointRounding.AwayFromZero) & """ " & disabled & " /></div>"
            str = str & "</div>"
            str = str & "<script type=""text/javascript"">"
            str = str & "function completeIt(){"
            str = str & "$('input[name=""dateExpiry""]:checked').focus();"
            str = str & "var dateExpiry = $('input[name=""dateExpiry""]:checked').val();"
            str = str & "var eYear=dateExpiry.substr(2,2); var eMonth=dateExpiry.substr(5,2);"
            str = str & "var iPrice=pad(parseFloat($('#curBasePrice').val()).toFixed(2).replace('.',''),6);"
            'str = str & "var newbarcode='" & strItem & "'+eMonth+eYear+iPrice; alert(newbarcode);"
            ''str = str & "var newbarcode='" & strItem & "'+eMonth+eYear+iPrice; getItemInfo(newbarcode,$('#trans'+curTab).val(),$('#deductionCash'+curTab).val(),$('#basePrice'+curTab).val(),$('#counter'+curTab).val(),$('#items_I_'+curTab).val(),$('#items_C_'+curTab).val(),0); $('#mdlMessage').modal('hide');"
            str = str & "var newbarcode='" & strItem & "'+eMonth+eYear+iPrice;" & strFunction.Replace("[BARCODE]", "newbarcode") & ";$('#mdlMessage').modal('hide');"
            str = str & "}"
            str = str & "</script>"

            Dim sh As New Share.UI
            Return sh.drawModal(Header, str, "<div class=""text-md-center""><button type=""button"" id=""btnSaveBarcode"" class=""btn btn-success ml-1"" onclick=""javascript:completeIt();""><i class=""icon-check2""></i>" & btnSave & "</button><button type=""button"" class=""btn btn-warning ml-1"" data-dismiss=""modal""><i class=""icon-cross2""></i>" & btnClose & "</button></div>", Share.UI.ModalSize.XSmall, "bg-grey bg-lighten-2")
        Else
            Return "Err:item not defined"
        End If
    End Function

    Public Function getItemInfo(ByVal strBarcode As String, ByVal byteWarehouse As Byte, ByVal curQuantity As Decimal) As String
        Dim strItem, strItemName As String
        Dim curBasePrice As Decimal
        Dim dateExpiry As Date
        Dim status As String

        If strBarcode.Length = 5 Then Return "<script type=""text/javascript"">$('#mdlMessage').on('shown.bs.modal', function() {$('input[name=""dateExpiry""]:checked').focus();});var modalOn=0;$('#mdlMessage').on('keyup', function (event) {if (event.which == 13 && modalOn==0) {event.preventDefault();completeIt();modalOn++;}});completeBarcode('" & strBarcode & "',$('#drpSendWarehouse').val(),0,'getItemInfo([BARCODE]);');</script>"

        Dim returnedArray As String() = FilterBarcode(strBarcode)
        If Left(returnedArray(0), 4) = "Err:" Then
            Return returnedArray(0)
        End If

        If HttpContext.Current.Session("QCounter") Is Nothing Then HttpContext.Current.Session("QCounter") = 0
        If HttpContext.Current.Session("QCounter") = 0 Then
            HttpContext.Current.Session("QCounter") = HttpContext.Current.Session("QCounter") + 1
            Return "<script type=""text/javascript"">$('#mdlConfirm').on('shown.bs.modal', function() {$('#curQuantity').focus().select();});var modalQOn=0;$('#mdlConfirm').on('keydown', function (event) {if (event.which == 13 && modalQOn==0) {event.preventDefault();assignQuantity();modalQOn++;}});getQuantity('" & strBarcode & "','getItemInfo([BARCODE],$(\'#drpSendWarehouse\').val(),[QTY])');</script>"
        End If
        HttpContext.Current.Session("QCounter") = 0

        Try
            strItem = returnedArray(0)
            curBasePrice = CDec(returnedArray(1))
            dateExpiry = CDate(returnedArray(2))
        Catch ex As Exception
            Return "Err:" & ex.Message
        End Try

        Dim dsGInfo As DataSet
        dsGInfo = dcl.GetDS("SELECT * FROM Stock_Items AS SI LEFT JOIN Stock_Units AS SU ON SI.byteIssueUnit = SU.byteUnit LEFT JOIN Stock_Item_Prices AS SIP ON SI.strItem = SIP.strItem LEFT JOIN Hw_Department_Items AS HDI ON SI.strItem = HDI.strItem AND HDI.byteDepartment = 15 LEFT JOIN Hw_Department_Warehouse AS HDW ON HDI.intService = HDW.intService AND HDI.byteDepartment = HDW.byteDepartment WHERE SI.strItem='" & strItem & "'")
        strItemName = Replace(dsGInfo.Tables(0).Rows(0).Item("strItem" & DataLang), "'", "\'")

        Dim curBalance As Decimal = checkStock(strItem, Today, byteWarehouse, dateExpiry)

        If curBalance - 1 < 0 Then
            Return "Err:No balance of this item."
        End If

        Dim ret As String = getItemCost(strItem)
        Dim curCost As Decimal = 0
        If Left(ret, 4) = "Err:" Then
            'Return ret
            status = "<i class=""icon-question2 red""></i>"
            curCost = 0
        Else
            status = "<i class=""icon-check green""></i>"
            curCost = CDec(ret)
        End If

        Dim script As String = ""
        script = script & "<script type=""text/javascript"">"
        'script = script & "function calcQuantity() {var q=0;$.each($('.item'), function (k, v) { if($('#'+v.id.replace('item','expiry')).val()=='" & dateExpiry.ToString("yyyy-MM-dd") & "' && $(v).val()=='" & strItem & "') q=q+parseInt($('#'+v.id.replace('item','quantity')).val());});return q;}"
        script = script & "if (parseInt(" & curQuantity & ") + calculateQuantity('" & strItem & "','" & dateExpiry.ToString("yyyy-MM-dd") & "') > " & curBalance & ") { msg('','The balance of this item is: " & Math.Round(curBalance, 0, MidpointRounding.AwayFromZero) & "','error') } else {"
        script = script & "$('#tblItems > tbody').prepend('<tr><td style=""width:30px"">" & status & "</td><td style=""width:50px""></td><td style=""width:70px"">" & strItem & "</td><td style=""width:300px"" class=""text-md-left"" title=""" & strItemName & """>" & strItemName & "</td><td style=""width:100px"">" & dateExpiry.ToString("yyyy-MM") & "</td><td style=""width:70px"">" & curQuantity & "</td><td style=""width:70px"">" & curBasePrice & "</td><td><button type=""button"" class=""btn btn-danger btn-xs"" onclick=""javascript:removeThis(this)"">Delete</button><input type=""hidden"" id=""item'+counter+'"" class=""item"" name=""item"" value=""" & strItem & """ /><input type=""hidden"" name=""expiry"" id=""expiry'+counter+'"" value=""" & dateExpiry.ToString("yyyy-MM-dd") & """ /><input type=""hidden"" id=""quantity'+counter+'"" name=""quantity"" class=""quantity"" value=""" & curQuantity & """ /><input type=""hidden"" name=""price"" value=""" & curBasePrice & """ /><input type=""hidden"" name=""cost"" value=""" & curCost & """ /></td></tr>');counter++;$('#txtBarcode').focus();"
        script = script & "};"
        'script = script & "$('#dtpExpiry').val('" & dateExpiry.ToString("yyyy-MM-dd") & "');"
        'script = script & "$('#txtItemName').val('" & strItemName & "');"
        'script = script & "$('#txtItem').val('" & strItem & "');"
        'script = script & "$('#txtPrice').val(" & curBasePrice & ");"
        'script = script & "$('#txtQuantity').val(1);"
        'script = script & "$('#txtBalance').val(" & curBalance & ");"
        'script = script & "$('#txtCost').val(" & curCost & ");"
        script = script & "</script>"

        Return script
    End Function

    Public Function getQuantity(ByVal strBarcode As String, ByVal strFunction As String) As String
        Dim str As String = ""
        Dim strItem As String
        Dim Quantity As String = "1"

        Dim Header As String
        Dim btnSave, btnClose As String
        Dim lblQuantity As String
        Select Case byteLanguage
            Case 2
                DataLang = "Ar"
                Header = "كمية الصنف"
                lblQuantity = "الكمية"
                btnSave = "حفظ"
                btnClose = "إغلاق"
            Case Else
                DataLang = "En"
                Header = "Item Quantity"
                lblQuantity = "Quantity"
                btnSave = "Save"
                btnClose = "Close"
        End Select

        str = str & "<div class=""row pl-1 pr-1"">"
        str = str & "<div class=""form-group""><label>" & lblQuantity & ":</label><input class=""form-control text-md-center"" id=""curQuantity"" type=""number"" value=""" & Quantity & """ /></div>"
        str = str & "</div>"
        str = str & "<script type=""text/javascript"">"
        str = str & "function assignQuantity(){"
        str = str & "if (parseInt($('#curQuantity').val()) > 0) {" & strFunction.Replace("[BARCODE]", strBarcode).Replace("[QTY]", "parseInt($('#curQuantity').val())") & "; $('#mdlConfirm').modal('hide');}"
        str = str & "}"
        str = str & "</script>"

        Dim sh As New Share.UI
        Return sh.drawModal(Header, str, "<div class=""text-md-center""><button type=""button"" id=""btnSaveBarcode"" class=""btn btn-success ml-1"" onclick=""javascript:assignQuantity();""><i class=""icon-check2""></i>" & btnSave & "</button><button type=""button"" class=""btn btn-warning ml-1"" data-dismiss=""modal""><i class=""icon-cross2""></i>" & btnClose & "</button></div>", Share.UI.ModalSize.XSmall, "bg-grey bg-lighten-2")
    End Function

    Public Function saveTransferExpired(ByVal lngTransaction As Long, ByVal Fields As String) As String
        Dim txtSendVoucher, txtReceiveVoucher, txtSendDate, txtReceiveDate, drpSendType, drpReceiveType, drpSendWarehouse, drpReceiveWarehouse, txtSendUser, txtReceiveUser, txtSendStatus, txtReceiveStatus, txtRemarks As String
        Dim lstItem, lstExpiry, lstQuantity, lstPrice, lstCost As String
        txtSendVoucher = ""
        txtReceiveVoucher = ""
        txtSendDate = ""
        txtReceiveDate = ""
        drpSendType = ""
        drpReceiveType = ""
        drpSendWarehouse = ""
        drpReceiveWarehouse = ""
        txtSendUser = ""
        txtReceiveUser = ""
        txtSendStatus = ""
        txtReceiveStatus = ""
        txtRemarks = ""

        lstItem = ""
        lstExpiry = ""
        lstQuantity = ""
        lstPrice = ""
        lstCost = ""

        Dim ErrMsg, ErrMissingVoucher, ErrWrongDate, ErrNoWarehouse, ErrWrongWarehouse, ErrNoItem As String
        ErrMsg = ""
        ErrMissingVoucher = "Voucher No is missing..."
        ErrWrongDate = "Wrong date format..."
        ErrNoWarehouse = "Warehouse is missing..."
        ErrWrongWarehouse = "Cannot transfer items to different warehouse..."
        ErrNoItem = "No item selected"

        ' get form values
        Dim jss As New JavaScriptSerializer()
        Dim field() As NameValue = jss.Deserialize(Of NameValue())(Fields)
        For I = 0 To field.Length - 1
            Select Case field(I).name()
                Case "txtSendVoucher"
                    txtSendVoucher = field(I).value()
                Case "txtReceiveVoucher"
                    txtReceiveVoucher = field(I).value()
                Case "txtSendDate"
                    txtSendDate = field(I).value()
                Case "txtReceiveDate"
                    txtReceiveDate = field(I).value()
                Case "drpSendType"
                    drpSendType = field(I).value()
                Case "drpReceiveType"
                    drpReceiveType = field(I).value()
                Case "drpSendWarehouse"
                    drpSendWarehouse = field(I).value()
                Case "drpReceiveWarehouse"
                    drpReceiveWarehouse = field(I).value()
                Case "txtSendUser"
                    txtSendUser = field(I).value()
                Case "txtReceiveUser"
                    txtReceiveUser = field(I).value()
                Case "txtSendStatus"
                    txtSendStatus = field(I).value()
                Case "txtReceiveStatus"
                    txtReceiveStatus = field(I).value()
                Case "txtRemarks"
                    txtRemarks = field(I).value()
                Case "item"
                    lstItem = lstItem & field(I).value() & ","
                Case "expiry"
                    lstExpiry = lstExpiry & field(I).value() & ","
                Case "quantity"
                    lstQuantity = lstQuantity & field(I).value() & ","
                Case "price"
                    lstPrice = lstPrice & field(I).value() & ","
                Case "cost"
                    lstCost = lstCost & field(I).value() & ","
            End Select
        Next

        'validation
        If txtSendVoucher = "" Then ErrMsg = ErrMissingVoucher
        If txtSendDate = "" Or Not (IsDate(txtSendDate)) Then ErrMsg = ErrWrongDate
        If drpSendWarehouse = "" Then ErrMsg = ErrNoWarehouse
        If drpReceiveWarehouse = "" Then ErrMsg = ErrNoWarehouse
        If drpSendWarehouse <> drpReceiveWarehouse Then ErrMsg = ErrWrongWarehouse
        If lstItem = "" Then ErrMsg = ErrNoItem

        'correction
        If txtSendUser = "" Then txtSendUser = strUserName
        If txtSendStatus = "" Then txtSendStatus = 1
        If drpSendType = "" Then drpSendType = 14
        If drpReceiveType = "" Then drpReceiveType = 42

        Dim lastBase, byteSendBase, byteReceiveBase As Integer
        If drpSendType = 14 Then
            lastBase = 52
            byteSendBase = 19
            byteReceiveBase = 52
        Else
            lastBase = 51
            byteSendBase = 51
            byteReceiveBase = 20
        End If

        Dim byteStatue As Integer
        If SuspendExpiredApprove = True Then byteStatue = 2 Else byteStatue = 1

        If ErrMsg = "" Then
            Try
                Dim dsLast As DataSet = dcl.GetDS("SELECT MAX(CAST(strTransaction AS bigint)) AS Last FROM Stock_Trans WHERE YEAR(dateTransaction) = " & intYear & " AND byteBase=" & lastBase)
                Dim lngNewVoucher As Long
                If IsDBNull(dsLast.Tables(0).Rows(0).Item("Last")) Then
                    lngNewVoucher = 1
                Else
                    lngNewVoucher = dsLast.Tables(0).Rows(0).Item("Last") + 1
                End If

                Dim item As String() = Split(Left(lstItem, Len(lstItem) - 1), ",")
                Dim expiry As String() = Split(Left(lstExpiry, Len(lstExpiry) - 1), ",")
                Dim quantity As String() = Split(Left(lstQuantity, Len(lstQuantity) - 1), ",")
                Dim price As String() = Split(Left(lstPrice, Len(lstPrice) - 1), ",")
                Dim cost As String() = Split(Left(lstCost, Len(lstCost) - 1), ",")

                Dim Counter As Integer = 1
                If lngTransaction > 0 Then
                    'validate not cancelled or not approved from the second warehouse
                    Dim dsTemp As DataSet = dcl.GetDS("SELECT * FROM Stock_Trans AS T INNER JOIN Stock_Xlink AS X ON X.lngTransaction=T.lngTransaction WHERE T.lngTransaction=" & lngTransaction)
                    If dsTemp.Tables(0).Rows(0).Item("byteStatus") = 0 Then Return "Err: This operation is cancelled..."
                    Dim Xlink As Long = dsTemp.Tables(0).Rows(0).Item("lngXlink")
                    Dim dsOther As DataSet = dcl.GetDS("SELECT T.lngTransaction, T.byteStatus FROM Stock_Xlink AS X INNER JOIN Stock_Trans AS T ON T.lngTransaction=X.lngTransaction WHERE lngPointer=" & lngTransaction & " AND X.lngTransaction<>" & lngTransaction)
                    If dsOther.Tables(0).Rows(0).Item("byteStatus") <> 1 Then Return "Err: Stock has been received or cancelled from the other warehouse.."
                    Dim OtherTransaction As Long = dsOther.Tables(0).Rows(0).Item("lngTransaction")
                    'updating
                    dcl.ExecSQuery("UPDATE Stock_Trans SET byteWarehouse=" & drpReceiveWarehouse & ", strRemarks='" & txtRemarks & "' WHERE lngTransaction=" & OtherTransaction)
                    'update audit
                    dcl.ExecSQuery("UPDATE Stock_Trans_Audit SET strLastSavedBy='" & strUserName & "', dateLastSaved='" & Today.ToString("yyyy-MM-dd") & "' WHERE lngTransaction=" & lngTransaction)
                    'delete all items
                    dcl.ExecSQuery("DELETE FROM Stock_Xlink_Items WHERE lngXlink=" & Xlink)
                    'insert all items
                    For I = 0 To item.Length - 1
                        dcl.ExecSQuery("INSERT INTO Stock_Xlink_Items (lngXlink,intEntryNumber,strItem,byteUnit,curQuantity,dateExpiry,curUnitCost,curUnitNetCost,curUnitPrice,bCopied,byteSystem,byteWarehouse,strBarCode) VALUES (" & Xlink & "," & Counter & ",'" & item(I) & "',1," & quantity(I) & ",'" & expiry(I) & "'," & cost(I) & "," & cost(I) & "," & price(I) & ",0,0," & drpSendWarehouse & ",'" & item(I) & "')")
                        Counter = Counter + 1
                    Next
                Else
                    Dim lngTransactionFrom, lngTransactionTo As Long
                    Dim lngXlinkNew, lngXlinkNew2 As Long
                    ' Insert first voucher
                    lngTransactionFrom = dcl.ExecIQuery("INSERT INTO Stock_Trans (byteBase,byteTransType,strTransaction,dateTransaction,byteStatus,bCash,byteCurrency,byteWarehouse,strRemarks) VALUES (" & byteSendBase & "," & drpSendType & ",'" & lngNewVoucher & "','" & txtSendDate & "'," & byteStatue & ",0,3," & drpSendWarehouse & ",'" & txtRemarks & "')")
                    ' Insert second voucher
                    lngTransactionTo = dcl.ExecIQuery("INSERT INTO Stock_Trans (byteBase,byteTransType,strTransaction,dateTransaction,byteStatus,bCash,byteCurrency,byteWarehouse) VALUES (" & byteReceiveBase & "," & drpReceiveType & ",'" & lngNewVoucher & "','" & txtSendDate & "'," & byteStatue & ",0,3," & drpReceiveWarehouse & ")")
                    ' Insert first voucher audit
                    dcl.ExecSQuery("INSERT INTO Stock_Trans_Audit (lngTransaction,strCreatedBy,dateCreated) VALUES (" & lngTransactionFrom & ",'" & txtSendUser & "','" & txtSendDate & "')")
                    ' Insert first voucher audit
                    If byteStatue = 2 Then dcl.ExecSQuery("INSERT INTO Stock_Trans_Audit (lngTransaction,strCreatedBy,dateCreated) VALUES (" & lngTransactionTo & ",'" & txtSendUser & "','" & txtSendDate & "')")
                    ' Insert first voucher xlink
                    lngXlinkNew = dcl.ExecIQuery("INSERT INTO Stock_Xlink (lngTransaction,lngPointer) VALUES(" & lngTransactionFrom & "," & lngTransactionFrom & ")")
                    ' Insert second voucher xlink
                    lngXlinkNew2 = dcl.ExecIQuery("INSERT INTO Stock_Xlink (lngTransaction,lngPointer) VALUES(" & lngTransactionTo & "," & lngTransactionFrom & ")")
                    ' Insert first voucher items
                    For I = 0 To item.Length - 1
                        dcl.ExecSQuery("INSERT INTO Stock_Xlink_Items (lngXlink,intEntryNumber,strItem,byteUnit,curQuantity,dateExpiry,curUnitCost,curUnitNetCost,curUnitPrice,bCopied,byteSystem,byteWarehouse,strBarCode) VALUES (" & lngXlinkNew & "," & Counter & ",'" & item(I) & "',1," & quantity(I) & ",'" & expiry(I) & "'," & cost(I) & "," & cost(I) & "," & price(I) & ",0,0," & drpSendWarehouse & ",'" & item(I) & "')")
                        If byteStatue = 2 Then dcl.ExecSQuery("INSERT INTO Stock_Xlink_Items (lngXlink,intEntryNumber,strItem,byteUnit,curQuantity,dateExpiry,curUnitCost,curUnitNetCost,curUnitPrice,bCopied,byteSystem,byteWarehouse,strBarCode) VALUES (" & lngXlinkNew2 & "," & Counter & ",'" & item(I) & "',1," & quantity(I) & ",'" & expiry(I) & "'," & cost(I) & "," & cost(I) & "," & price(I) & ",0,0," & drpReceiveWarehouse & ",'" & item(I) & "')")
                        Counter = Counter + 1
                    Next
                End If
            Catch ex As Exception
                ErrMsg = ex.Message
            End Try
        End If

        If ErrMsg = "" Then
            Return "<script>msg('', 'Changes saved successfully!' , 'success');$('#mdlAlpha').modal('hide');fillSExpired();</script>"
        Else
            Return "Err:" & ErrMsg
        End If
    End Function

    Public Function saveTransferReturned(ByVal lngTransaction As Long, ByVal Fields As String) As String
        Dim txtSendVoucher, txtReceiveVoucher, txtSendDate, txtReceiveDate, drpSendType, drpReceiveType, drpSendWarehouse, drpReceiveWarehouse, txtSendUser, txtReceiveUser, txtSendStatus, txtReceiveStatus, txtRemarks As String
        Dim lstItem, lstExpiry, lstQuantity, lstPrice, lstCost As String
        txtSendVoucher = ""
        txtReceiveVoucher = ""
        txtSendDate = ""
        txtReceiveDate = ""
        drpSendType = ""
        drpReceiveType = ""
        drpSendWarehouse = ""
        drpReceiveWarehouse = ""
        txtSendUser = ""
        txtReceiveUser = ""
        txtSendStatus = ""
        txtReceiveStatus = ""
        txtRemarks = ""

        lstItem = ""
        lstExpiry = ""
        lstQuantity = ""
        lstPrice = ""
        lstCost = ""

        Dim ErrMsg, ErrMissingVoucher, ErrWrongDate, ErrNoWarehouse, ErrWrongWarehouse, ErrNoItem As String
        ErrMsg = ""
        ErrMissingVoucher = "Voucher No is missing..."
        ErrWrongDate = "Wrong date format..."
        ErrNoWarehouse = "Warehouse is missing..."
        ErrWrongWarehouse = "Cannot transfer items to different warehouse..."
        ErrNoItem = "No item selected"

        ' get form values
        Dim jss As New JavaScriptSerializer()
        Dim field() As NameValue = jss.Deserialize(Of NameValue())(Fields)
        For I = 0 To field.Length - 1
            Select Case field(I).name()
                Case "txtSendVoucher"
                    txtSendVoucher = field(I).value()
                Case "txtReceiveVoucher"
                    txtReceiveVoucher = field(I).value()
                Case "txtSendDate"
                    txtSendDate = field(I).value()
                Case "txtReceiveDate"
                    txtReceiveDate = field(I).value()
                Case "drpSendType"
                    drpSendType = field(I).value()
                Case "drpReceiveType"
                    drpReceiveType = field(I).value()
                Case "drpSendWarehouse"
                    drpSendWarehouse = field(I).value()
                Case "drpReceiveWarehouse"
                    drpReceiveWarehouse = field(I).value()
                Case "txtSendUser"
                    txtSendUser = field(I).value()
                Case "txtReceiveUser"
                    txtReceiveUser = field(I).value()
                Case "txtSendStatus"
                    txtSendStatus = field(I).value()
                Case "txtReceiveStatus"
                    txtReceiveStatus = field(I).value()
                Case "txtRemarks"
                    txtRemarks = field(I).value()
                Case "item"
                    lstItem = lstItem & field(I).value() & ","
                Case "expiry"
                    lstExpiry = lstExpiry & field(I).value() & ","
                Case "quantity"
                    lstQuantity = lstQuantity & field(I).value() & ","
                Case "price"
                    lstPrice = lstPrice & field(I).value() & ","
                Case "cost"
                    lstCost = lstCost & field(I).value() & ","
            End Select
        Next

        'validation
        If txtSendVoucher = "" Then ErrMsg = ErrMissingVoucher
        If txtSendDate = "" Or Not (IsDate(txtSendDate)) Then ErrMsg = ErrWrongDate
        If drpSendWarehouse = "" Then ErrMsg = ErrNoWarehouse
        If drpReceiveWarehouse = "" Then ErrMsg = ErrNoWarehouse
        If drpSendWarehouse <> drpReceiveWarehouse Then ErrMsg = ErrWrongWarehouse
        If lstItem = "" Then ErrMsg = ErrNoItem

        'correction
        If txtSendUser = "" Then txtSendUser = strUserName
        If txtSendStatus = "" Then txtSendStatus = 1
        If drpSendType = "" Then drpSendType = 14
        If drpReceiveType = "" Then drpReceiveType = 40

        Dim lastBase, byteSendBase, byteReceiveBase As Integer
        If drpSendType = 14 Then
            lastBase = 52
            byteSendBase = 19
            byteReceiveBase = 52
        Else
            lastBase = 51
            byteSendBase = 51
            byteReceiveBase = 20
        End If

        Dim byteStatue As Integer
        If SuspendExpiredApprove = True Then byteStatue = 2 Else byteStatue = 1

        If ErrMsg = "" Then
            Try
                Dim dsLast As DataSet = dcl.GetDS("SELECT MAX(CAST(strTransaction AS bigint)) AS Last FROM Stock_Trans WHERE YEAR(dateTransaction) = " & intYear & " AND byteBase=" & lastBase)
                Dim lngNewVoucher As Long
                If IsDBNull(dsLast.Tables(0).Rows(0).Item("Last")) Then
                    lngNewVoucher = 1
                Else
                    lngNewVoucher = dsLast.Tables(0).Rows(0).Item("Last") + 1
                End If

                Dim item As String() = Split(Left(lstItem, Len(lstItem) - 1), ",")
                Dim expiry As String() = Split(Left(lstExpiry, Len(lstExpiry) - 1), ",")
                Dim quantity As String() = Split(Left(lstQuantity, Len(lstQuantity) - 1), ",")
                Dim price As String() = Split(Left(lstPrice, Len(lstPrice) - 1), ",")
                Dim cost As String() = Split(Left(lstCost, Len(lstCost) - 1), ",")

                Dim Counter As Integer = 1
                If lngTransaction > 0 Then
                    'validate not cancelled or not approved from the second warehouse
                    Dim dsTemp As DataSet = dcl.GetDS("SELECT * FROM Stock_Trans AS T INNER JOIN Stock_Xlink AS X ON X.lngTransaction=T.lngTransaction WHERE T.lngTransaction=" & lngTransaction)
                    If dsTemp.Tables(0).Rows(0).Item("byteStatus") = 0 Then Return "Err: This operation is cancelled..."
                    Dim Xlink As Long = dsTemp.Tables(0).Rows(0).Item("lngXlink")
                    Dim dsOther As DataSet = dcl.GetDS("SELECT T.lngTransaction, T.byteStatus FROM Stock_Xlink AS X INNER JOIN Stock_Trans AS T ON T.lngTransaction=X.lngTransaction WHERE lngPointer=" & lngTransaction & " AND X.lngTransaction<>" & lngTransaction)
                    If dsOther.Tables(0).Rows(0).Item("byteStatus") <> 1 Then Return "Err: Stock has been received or cancelled from the other warehouse.."
                    Dim OtherTransaction As Long = dsOther.Tables(0).Rows(0).Item("lngTransaction")
                    'updating
                    dcl.ExecSQuery("UPDATE Stock_Trans SET byteWarehouse=" & drpReceiveWarehouse & ", strRemarks='" & txtRemarks & "' WHERE lngTransaction=" & OtherTransaction)
                    'update audit
                    dcl.ExecSQuery("UPDATE Stock_Trans_Audit SET strLastSavedBy='" & strUserName & "', dateLastSaved='" & Today.ToString("yyyy-MM-dd") & "' WHERE lngTransaction=" & lngTransaction)
                    'delete all items
                    dcl.ExecSQuery("DELETE FROM Stock_Xlink_Items WHERE lngXlink=" & Xlink)
                    'insert all items
                    For I = 0 To item.Length - 1
                        dcl.ExecSQuery("INSERT INTO Stock_Xlink_Items (lngXlink,intEntryNumber,strItem,byteUnit,curQuantity,dateExpiry,curUnitCost,curUnitNetCost,curUnitPrice,bCopied,byteSystem,byteWarehouse,strBarCode) VALUES (" & Xlink & "," & Counter & ",'" & item(I) & "',1," & quantity(I) & ",'" & expiry(I) & "'," & cost(I) & "," & cost(I) & "," & price(I) & ",0,0," & drpSendWarehouse & ",'" & item(I) & "')")
                        Counter = Counter + 1
                    Next
                Else
                    Dim lngTransactionFrom, lngTransactionTo As Long
                    Dim lngXlinkNew, lngXlinkNew2 As Long
                    ' Insert first voucher
                    lngTransactionFrom = dcl.ExecIQuery("INSERT INTO Stock_Trans (byteBase,byteTransType,strTransaction,dateTransaction,byteStatus,bCash,byteCurrency,byteWarehouse,strRemarks) VALUES (" & byteSendBase & "," & drpSendType & ",'" & lngNewVoucher & "','" & txtSendDate & "'," & byteStatue & ",0,3," & drpSendWarehouse & ",'" & txtRemarks & "')")
                    ' Insert second voucher
                    lngTransactionTo = dcl.ExecIQuery("INSERT INTO Stock_Trans (byteBase,byteTransType,strTransaction,dateTransaction,byteStatus,bCash,byteCurrency,byteWarehouse) VALUES (" & byteReceiveBase & "," & drpReceiveType & ",'" & lngNewVoucher & "','" & txtSendDate & "'," & byteStatue & ",0,3," & drpReceiveWarehouse & ")")
                    ' Insert first voucher audit
                    dcl.ExecSQuery("INSERT INTO Stock_Trans_Audit (lngTransaction,strCreatedBy,dateCreated) VALUES (" & lngTransactionFrom & ",'" & txtSendUser & "','" & txtSendDate & "')")
                    ' Insert first voucher audit
                    If byteStatue = 2 Then dcl.ExecSQuery("INSERT INTO Stock_Trans_Audit (lngTransaction,strCreatedBy,dateCreated) VALUES (" & lngTransactionTo & ",'" & txtSendUser & "','" & txtSendDate & "')")
                    ' Insert first voucher xlink
                    lngXlinkNew = dcl.ExecIQuery("INSERT INTO Stock_Xlink (lngTransaction,lngPointer) VALUES(" & lngTransactionFrom & "," & lngTransactionFrom & ")")
                    ' Insert second voucher xlink
                    lngXlinkNew2 = dcl.ExecIQuery("INSERT INTO Stock_Xlink (lngTransaction,lngPointer) VALUES(" & lngTransactionTo & "," & lngTransactionFrom & ")")
                    ' Insert first voucher items
                    For I = 0 To item.Length - 1
                        dcl.ExecSQuery("INSERT INTO Stock_Xlink_Items (lngXlink,intEntryNumber,strItem,byteUnit,curQuantity,dateExpiry,curUnitCost,curUnitNetCost,curUnitPrice,bCopied,byteSystem,byteWarehouse,strBarCode) VALUES (" & lngXlinkNew & "," & Counter & ",'" & item(I) & "',1," & quantity(I) & ",'" & expiry(I) & "'," & cost(I) & "," & cost(I) & "," & price(I) & ",0,0," & drpSendWarehouse & ",'" & item(I) & "')")
                        If byteStatue = 2 Then dcl.ExecSQuery("INSERT INTO Stock_Xlink_Items (lngXlink,intEntryNumber,strItem,byteUnit,curQuantity,dateExpiry,curUnitCost,curUnitNetCost,curUnitPrice,bCopied,byteSystem,byteWarehouse,strBarCode) VALUES (" & lngXlinkNew2 & "," & Counter & ",'" & item(I) & "',1," & quantity(I) & ",'" & expiry(I) & "'," & cost(I) & "," & cost(I) & "," & price(I) & ",0,0," & drpReceiveWarehouse & ",'" & item(I) & "')")
                        Counter = Counter + 1
                    Next
                End If
            Catch ex As Exception
                ErrMsg = ex.Message
            End Try
        End If

        If ErrMsg = "" Then
            Return "<script>msg('', 'Changes saved successfully!' , 'success');$('#mdlAlpha').modal('hide');fillSReturned();</script>"
        Else
            Return "Err:" & ErrMsg
        End If
    End Function

    Public Function saveTransfer(ByVal lngTransaction As Long, ByVal Fields As String) As String
        Dim txtSendVoucher, txtReceiveVoucher, txtSendDate, txtReceiveDate, drpSendType, drpReceiveType, drpSendWarehouse, drpReceiveWarehouse, txtSendUser, txtReceiveUser, txtSendStatus, txtReceiveStatus, txtRemarks As String
        Dim lstItem, lstExpiry, lstQuantity, lstPrice, lstCost As String
        txtSendVoucher = ""
        txtReceiveVoucher = ""
        txtSendDate = ""
        txtReceiveDate = ""
        drpSendType = ""
        drpReceiveType = ""
        drpSendWarehouse = ""
        drpReceiveWarehouse = ""
        txtSendUser = ""
        txtReceiveUser = ""
        txtSendStatus = ""
        txtReceiveStatus = ""
        txtRemarks = ""

        lstItem = ""
        lstExpiry = ""
        lstQuantity = ""
        lstPrice = ""
        lstCost = ""

        Dim ErrMsg, ErrMissingVoucher, ErrWrongDate, ErrNoWarehouse, ErrWrongWarehouse, ErrNoItem As String
        ErrMsg = ""
        ErrMissingVoucher = "Voucher No is missing..."
        ErrWrongDate = "Wrong date format..."
        ErrNoWarehouse = "Warehouse is missing..."
        ErrWrongWarehouse = "Cannot transfer items to same warehouse..."
        ErrNoItem = "No item selected"

        ' get form values
        Dim jss As New JavaScriptSerializer()
        Dim field() As NameValue = jss.Deserialize(Of NameValue())(Fields)
        For I = 0 To field.Length - 1
            Select Case field(I).name()
                Case "txtSendVoucher"
                    txtSendVoucher = field(I).value()
                Case "txtReceiveVoucher"
                    txtReceiveVoucher = field(I).value()
                Case "txtSendDate"
                    txtSendDate = field(I).value()
                Case "txtReceiveDate"
                    txtReceiveDate = field(I).value()
                Case "drpSendType"
                    drpSendType = field(I).value()
                Case "drpReceiveType"
                    drpReceiveType = field(I).value()
                Case "drpSendWarehouse"
                    drpSendWarehouse = field(I).value()
                Case "drpReceiveWarehouse"
                    drpReceiveWarehouse = field(I).value()
                Case "txtSendUser"
                    txtSendUser = field(I).value()
                Case "txtReceiveUser"
                    txtReceiveUser = field(I).value()
                Case "txtSendStatus"
                    txtSendStatus = field(I).value()
                Case "txtReceiveStatus"
                    txtReceiveStatus = field(I).value()
                Case "txtRemarks"
                    txtRemarks = field(I).value()
                Case "item"
                    lstItem = lstItem & field(I).value() & ","
                Case "expiry"
                    lstExpiry = lstExpiry & field(I).value() & ","
                Case "quantity"
                    lstQuantity = lstQuantity & field(I).value() & ","
                Case "price"
                    lstPrice = lstPrice & field(I).value() & ","
                Case "cost"
                    lstCost = lstCost & field(I).value() & ","
            End Select
        Next

        'validation
        If txtSendVoucher = "" Then ErrMsg = ErrMissingVoucher
        If txtSendDate = "" Or Not (IsDate(txtSendDate)) Then ErrMsg = ErrWrongDate
        If drpSendWarehouse = "" Then ErrMsg = ErrNoWarehouse
        If drpReceiveWarehouse = "" Then ErrMsg = ErrNoWarehouse
        If drpSendWarehouse = drpReceiveWarehouse Then ErrMsg = ErrWrongWarehouse
        If lstItem = "" Then ErrMsg = ErrNoItem

        'correction
        If txtSendUser = "" Then txtSendUser = strUserName
        If txtSendStatus = "" Then txtSendStatus = 1
        If drpSendType = "" Then drpSendType = 14
        If drpReceiveType = "" Then drpReceiveType = 15

        If ErrMsg = "" Then
            Try
                'Dim dsLast As DataSet = dcl.GetDS("SELECT MAX(CAST(strTransaction AS bigint)) AS Last FROM Stock_Trans WHERE YEAR(dateTransaction) = " & intYear & " AND byteBase=19")
                Dim dsLast As DataSet = dcl.GetDS("SELECT MAX(CAST(strTransaction AS bigint)) AS Last FROM Stock_Trans WHERE Year(dateTransaction)=" & intYear & " AND byteTransType=" & drpSendType & " AND byteWarehouse=" & drpSendWarehouse)
                Dim lngNewVoucher As Long
                If IsDBNull(dsLast.Tables(0).Rows(0).Item("Last")) Then
                    lngNewVoucher = 1
                Else
                    lngNewVoucher = dsLast.Tables(0).Rows(0).Item("Last") + 1
                End If

                Dim item As String() = Split(Left(lstItem, Len(lstItem) - 1), ",")
                Dim expiry As String() = Split(Left(lstExpiry, Len(lstExpiry) - 1), ",")
                Dim quantity As String() = Split(Left(lstQuantity, Len(lstQuantity) - 1), ",")
                Dim price As String() = Split(Left(lstPrice, Len(lstPrice) - 1), ",")
                Dim cost As String() = Split(Left(lstCost, Len(lstCost) - 1), ",")

                Dim Counter As Integer = 1
                If lngTransaction > 0 Then
                    'validate not cancelled or not approved from the second warehouse

                    Dim dsTemp As DataSet = dcl.GetDS("SELECT * FROM Stock_Trans AS T INNER JOIN Stock_Xlink AS X ON X.lngTransaction=T.lngTransaction WHERE T.lngTransaction=" & lngTransaction)
                    If dsTemp.Tables(0).Rows(0).Item("byteStatus") = 0 Then Return "Err: This operation is cancelled..."
                    Dim Xlink As Long = dsTemp.Tables(0).Rows(0).Item("lngXlink")
                    Dim dsOther As DataSet = dcl.GetDS("SELECT T.lngTransaction, T.byteStatus FROM Stock_Xlink AS X INNER JOIN Stock_Trans AS T ON T.lngTransaction=X.lngTransaction WHERE lngPointer=" & lngTransaction & " AND X.lngTransaction<>" & lngTransaction)
                    If dsOther.Tables(0).Rows(0).Item("byteStatus") <> 1 Then Return "Err: Stock has been received or cancelled from the other warehouse.."
                    Dim OtherTransaction As Long = dsOther.Tables(0).Rows(0).Item("lngTransaction")
                    'updating
                    dcl.ExecSQuery("UPDATE Stock_Trans SET byteWarehouse=" & drpReceiveWarehouse & ", strRemarks='" & txtRemarks & "' WHERE lngTransaction=" & OtherTransaction)
                    'update audit
                    dcl.ExecSQuery("UPDATE Stock_Trans_Audit SET strLastSavedBy='" & strUserName & "', dateLastSaved='" & Today.ToString("yyyy-MM-dd") & "' WHERE lngTransaction=" & lngTransaction)
                    'delete all items
                    dcl.ExecSQuery("DELETE FROM Stock_Xlink_Items WHERE lngXlink=" & Xlink)
                    'insert all items
                    For I = 0 To item.Length - 1
                        dcl.ExecSQuery("INSERT INTO Stock_Xlink_Items (lngXlink,intEntryNumber,strItem,byteUnit,curQuantity,dateExpiry,curUnitCost,curUnitNetCost,curUnitPrice,bCopied,byteSystem,byteWarehouse,strBarCode) VALUES (" & Xlink & "," & Counter & ",'" & item(I) & "',1," & quantity(I) & ",'" & expiry(I) & "'," & cost(I) & "," & cost(I) & "," & price(I) & ",0,0," & drpSendWarehouse & ",'" & item(I) & "')")
                        Counter = Counter + 1
                    Next
                    'insert into logs
                    Dim usr As New Share.User
                    usr.AddLog(strUserName, Now, 3, "Transfer", lngTransaction, 2, "Update Transfer Voucher(" & lngNewVoucher & ") With [" & Counter - 1 & "] Items")
                Else
                    Dim lngTransactionFrom, lngTransactionTo As Long
                    Dim lngXlinkNew As Long
                    ' Insert first voucher
                    lngTransactionFrom = dcl.ExecIQuery("INSERT INTO Stock_Trans (byteBase,byteTransType,strTransaction,dateTransaction,byteStatus,bCash,byteCurrency,byteWarehouse,strRemarks) VALUES (19," & drpSendType & ",'" & lngNewVoucher & "','" & txtSendDate & "',1,0,3," & drpSendWarehouse & ",'" & txtRemarks & "')")
                    ' Insert first voucher audit
                    dcl.ExecSQuery("INSERT INTO Stock_Trans_Audit (lngTransaction,strCreatedBy,dateCreated) VALUES (" & lngTransactionFrom & ",'" & txtSendUser & "','" & txtSendDate & "')")
                    ' Insert first voucher xlink
                    lngXlinkNew = dcl.ExecIQuery("INSERT INTO Stock_Xlink (lngTransaction,lngPointer) VALUES(" & lngTransactionFrom & "," & lngTransactionFrom & ")")
                    ' Insert first voucher items
                    For I = 0 To item.Length - 1
                        dcl.ExecSQuery("INSERT INTO Stock_Xlink_Items (lngXlink,intEntryNumber,strItem,byteUnit,curQuantity,dateExpiry,curUnitCost,curUnitNetCost,curUnitPrice,bCopied,byteSystem,byteWarehouse,strBarCode) VALUES (" & lngXlinkNew & "," & Counter & ",'" & item(I) & "',1," & quantity(I) & ",'" & expiry(I) & "'," & cost(I) & "," & cost(I) & "," & price(I) & ",0,0," & drpSendWarehouse & ",'" & item(I) & "')")
                        Counter = Counter + 1
                    Next
                    ' Insert second voucher
                    lngTransactionTo = dcl.ExecIQuery("INSERT INTO Stock_Trans (byteBase,byteTransType,strTransaction,dateTransaction,byteStatus,bCash,byteCurrency,byteWarehouse) VALUES (20," & drpReceiveType & ",'" & lngNewVoucher & "','" & txtSendDate & "',1,0,3," & drpReceiveWarehouse & ")")
                    ' Insert second voucher xlink
                    dcl.ExecIQuery("INSERT INTO Stock_Xlink (lngTransaction,lngPointer) VALUES(" & lngTransactionTo & "," & lngTransactionFrom & ")")
                    'insert into logs
                    Dim usr As New Share.User
                    usr.AddLog(strUserName, Now, 3, "Transfer", lngTransaction, 1, "Add Transfer Voucher(" & lngNewVoucher & ") With [" & Counter - 1 & "] Items")
                End If
            Catch ex As Exception
                ErrMsg = ex.Message
            End Try
        End If

        If ErrMsg = "" Then
            Return "<script>msg('', 'Changes saved successfully!' , 'success');$('#mdlAlpha').modal('hide');fillTransfer();</script>"
        Else
            Return "Err:" & ErrMsg
        End If
    End Function

    Public Function approveSendItems(ByVal lngTransaction As Long, Optional ByVal modal As String = "") As String
        Try
            Dim ds As DataSet = dcl.GetDS("SELECT T.lngTransaction, T.dateTransaction, T.byteBase, T.byteStatus, COUNT(X.lngXlink) AS intCount FROM Stock_Trans AS T INNER JOIN Stock_Xlink AS X ON T.lngTransaction=X.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink=XI.lngXlink WHERE T.lngTransaction=" & lngTransaction & " GROUP BY T.lngTransaction, T.dateTransaction, T.byteBase, T.byteStatus")
            If ds.Tables(0).Rows.Count > 0 Then
                If ds.Tables(0).Rows(0).Item("byteStatus") = 1 Then
                    If ds.Tables(0).Rows(0).Item("intCount") > 0 Then
                        dcl.ExecSQuery("UPDATE Stock_Trans SET byteStatus=2 WHERE lngTransaction=" & lngTransaction)
                        'insert into logs
                        Dim usr As New Share.User
                        usr.AddLog(strUserName, Now, 3, "Transfer", lngTransaction, 4, "Approve Sending")
                        '
                        Dim str As String = ""
                        str = str & "<script>"
                        If modal <> "" Then str = str & "$('" & modal & "').modal('hide');fillTransfer();"
                        str = str & "msg('','Transfer approved successfully!','success');"
                        str = str & "</script>"
                        Return str
                    Else
                        Return "Err:No items to transfer"
                    End If
                Else
                    Return "Err:Transfer is already approved or cancelled.."
                End If
            Else
                Return "Err:Record not found, please refresh the page"
            End If
        Catch ex As Exception
            Return "Err:" & ex.Message
        End Try
    End Function

    Public Function approveItems(ByVal lngTransaction As Long, Optional ByVal modal As String = "") As String
        Try
            Dim ds As DataSet = dcl.GetDS("SELECT T.lngTransaction, T.dateTransaction, T.byteBase, T.byteStatus, COUNT(X.lngXlink) AS intCount FROM Stock_Trans AS T INNER JOIN Stock_Xlink AS X ON T.lngTransaction=X.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink=XI.lngXlink WHERE T.lngTransaction=" & lngTransaction & " GROUP BY T.lngTransaction, T.dateTransaction, T.byteBase, T.byteStatus")
            If ds.Tables(0).Rows.Count > 0 Then
                If ds.Tables(0).Rows(0).Item("byteStatus") = 1 Then
                    If ds.Tables(0).Rows(0).Item("intCount") > 0 Then
                        'approve first one
                        dcl.ExecSQuery("UPDATE Stock_Trans SET byteStatus=2 WHERE lngTransaction=" & lngTransaction)
                        'approve second one
                        Dim ds2 As DataSet = dcl.GetDS("SELECT T.lngTransaction, X.lngXlink, T.dateTransaction, T.byteBase, T.byteStatus FROM Stock_Xlink AS X INNER JOIN Stock_Trans AS T ON T.lngTransaction=X.lngTransaction WHERE X.lngPointer=" & lngTransaction & " AND X.lngTransaction<>" & lngTransaction)
                        If ds2.Tables(0).Rows.Count > 0 Then
                            If ds2.Tables(0).Rows(0).Item("byteStatus") = 1 Then
                                'get receive transaction
                                Dim lngReceiveTransaction As Long = ds2.Tables(0).Rows(0).Item("lngTransaction")
                                'Change status and date
                                dcl.ExecScalar("UPDATE Stock_Trans SET dateTransaction='" & Today.ToString("yyyy-MM-dd") & "', byteStatus=2 WHERE lngTransaction=" & lngReceiveTransaction)
                                'add or update audit
                                Dim dsAudit As DataSet = dcl.GetDS("SELECT * FROM Stock_Trans_Audit WHERE lngTransaction=" & lngReceiveTransaction)
                                If dsAudit.Tables(0).Rows.Count > 0 Then
                                    dcl.ExecScalar("UPDATE Stock_Trans_Audit SET strLastSavedBy='" & strUserName & "',dateLastSaved='" & Today.ToString("yyyy-MM-dd") & "' WHERE lngTransaction=" & lngReceiveTransaction)
                                Else
                                    dcl.ExecScalar("INSERT INTO Stock_Trans_Audit (lngTransaction,strCreatedBy,dateCreated,strLastSavedBy,dateLastSaved) VALUES (" & lngReceiveTransaction & ",'" & strUserName & "','" & Today.ToString("yyyy-MM-dd") & "','" & strUserName & "','" & Today.ToString("yyyy-MM-dd") & "')")
                                End If
                                'get xlink id
                                Dim lngXlink As Long = ds2.Tables(0).Rows(0).Item("lngXlink")
                                'delete all items
                                dcl.ExecScalar("DELETE FROM Stock_Xlink_Items WHERE lngXlink=" & lngXlink)
                                'copy items from send transaction to receive transaction
                                Dim Counter As Integer = 1
                                Dim dsItems As DataSet = dcl.GetDS("SELECT * FROM Stock_Xlink AS X INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink=XI.lngXlink WHERE X.lngTransaction=" & lngTransaction)
                                For I = 0 To dsItems.Tables(0).Rows.Count - 1
                                    dcl.ExecScalar("INSERT INTO Stock_Xlink_Items (lngXlink,intEntryNumber,strItem,byteUnit,curQuantity,dateExpiry,curUnitCost,curUnitNetCost,curUnitPrice,byteWarehouse,strBarCode) VALUES (" & lngXlink & "," & Counter & ",'" & dsItems.Tables(0).Rows(I).Item("strItem") & "'," & dsItems.Tables(0).Rows(I).Item("byteUnit") & "," & dsItems.Tables(0).Rows(I).Item("curQuantity") & ",'" & CDate(dsItems.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM-dd") & "'," & dsItems.Tables(0).Rows(I).Item("curUnitCost") & "," & dsItems.Tables(0).Rows(I).Item("curUnitNetCost") & "," & dsItems.Tables(0).Rows(I).Item("curUnitPrice") & "," & byteWarehouse & ",'" & dsItems.Tables(0).Rows(I).Item("strBarCode") & "')")
                                    Counter = Counter + 1
                                Next
                            Else
                                Return "Err:Transfer is already approved or cancelled.."
                            End If
                        Else
                            Return "Err:Record not found, please refresh the page"
                        End If
                        'return
                        Dim str As String = ""
                        str = str & "<script>"
                        If modal <> "" Then str = str & "$('" & modal & "').modal('hide');"
                        str = str & "msg('','Transfer approved successfully!','success');"
                        str = str & "</script>"
                        Return str
                    Else
                        Return "Err:No items to transfer"
                    End If
                Else
                    Return "Err:Transfer is already approved or cancelled.."
                End If
            Else
                Return "Err:Record not found, please refresh the page"
            End If
        Catch ex As Exception
            Return "Err:" & ex.Message
        End Try
    End Function

    Public Function approveReceiveItems(ByVal lngTransaction As Long, Optional ByVal modal As String = "") As String
        Try
            Dim ds As DataSet = dcl.GetDS("SELECT T.lngTransaction, X.lngXlink, T.dateTransaction, T.byteBase, T.byteStatus FROM Stock_Xlink AS X INNER JOIN Stock_Trans AS T ON T.lngTransaction=X.lngTransaction WHERE X.lngPointer=" & lngTransaction & " AND X.lngTransaction<>" & lngTransaction)
            If ds.Tables(0).Rows.Count > 0 Then
                If ds.Tables(0).Rows(0).Item("byteStatus") = 1 Then
                    'get receive transaction
                    Dim lngReceiveTransaction As Long = ds.Tables(0).Rows(0).Item("lngTransaction")
                    'Change status and date
                    dcl.ExecScalar("UPDATE Stock_Trans SET dateTransaction='" & Today.ToString("yyyy-MM-dd") & "', byteStatus=2 WHERE lngTransaction=" & lngReceiveTransaction)
                    'add or update audit
                    Dim dsAudit As DataSet = dcl.GetDS("SELECT * FROM Stock_Trans_Audit WHERE lngTransaction=" & lngReceiveTransaction)
                    If dsAudit.Tables(0).Rows.Count > 0 Then
                        dcl.ExecScalar("UPDATE Stock_Trans_Audit SET strCreatedBy='" & strUserName & "',dateCreated='" & Today.ToString("yyyy-MM-dd") & "',strLastSavedBy='" & strUserName & "',dateLastSaved='" & Today.ToString("yyyy-MM-dd") & "' WHERE lngTransaction=" & lngReceiveTransaction)
                    Else
                        dcl.ExecScalar("INSERT INTO Stock_Trans_Audit (lngTransaction,strCreatedBy,dateCreated,strLastSavedBy,dateLastSaved) VALUES (" & lngReceiveTransaction & ",'" & strUserName & "','" & Today.ToString("yyyy-MM-dd") & "','" & strUserName & "','" & Today.ToString("yyyy-MM-dd") & "')")
                    End If
                    'get xlink id
                    Dim lngXlink As Long = ds.Tables(0).Rows(0).Item("lngXlink")
                    'delete all items
                    dcl.ExecScalar("DELETE FROM Stock_Xlink_Items WHERE lngXlink=" & lngXlink)
                    'copy items from send transaction to receive transaction
                    Dim Counter As Integer = 1
                    Dim curUnitCost, curUnitNetCost, curUnitPrice As Decimal
                    Dim dsItems As DataSet = dcl.GetDS("SELECT * FROM Stock_Xlink AS X INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink=XI.lngXlink WHERE X.lngTransaction=" & lngTransaction)
                    For I = 0 To dsItems.Tables(0).Rows.Count - 1
                        If IsDBNull(dsItems.Tables(0).Rows(I).Item("curUnitCost")) Then curUnitCost = 0 Else curUnitCost = dsItems.Tables(0).Rows(I).Item("curUnitCost")
                        If IsDBNull(dsItems.Tables(0).Rows(I).Item("curUnitNetCost")) Then curUnitNetCost = 0 Else curUnitNetCost = dsItems.Tables(0).Rows(I).Item("curUnitNetCost")
                        If IsDBNull(dsItems.Tables(0).Rows(I).Item("curUnitPrice")) Then curUnitPrice = 0 Else curUnitPrice = dsItems.Tables(0).Rows(I).Item("curUnitPrice")
                        dcl.ExecScalar("INSERT INTO Stock_Xlink_Items (lngXlink,intEntryNumber,strItem,byteUnit,curQuantity,dateExpiry,curUnitCost,curUnitNetCost,curUnitPrice,byteWarehouse,strBarCode) VALUES (" & lngXlink & "," & Counter & ",'" & dsItems.Tables(0).Rows(I).Item("strItem") & "'," & dsItems.Tables(0).Rows(I).Item("byteUnit") & "," & dsItems.Tables(0).Rows(I).Item("curQuantity") & ",'" & CDate(dsItems.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM-dd") & "'," & curUnitCost & "," & curUnitNetCost & "," & curUnitPrice & "," & byteWarehouse & ",'" & dsItems.Tables(0).Rows(I).Item("strBarCode").ToString & "')")
                        Counter = Counter + 1
                    Next
                    'insert into logs
                    Dim usr As New Share.User
                    usr.AddLog(strUserName, Now, 3, "Transfer", lngTransaction, 4, "Approve Receiving")
                    'return script
                    Dim str As String = ""
                    str = str & "<script>"
                    If modal <> "" Then str = str & "$('" & modal & "').modal('hide');"
                    str = str & "msg('','Transfer approved successfully!','success');fillRequests();"
                    str = str & "</script>"
                    Return str
                Else
                    Return "Err:Transfer is already approved or cancelled.."
                End If
            Else
                Return "Err:Record not found, please refresh the page"
            End If
        Catch ex As Exception
            Return "Err:" & ex.Message
        End Try
    End Function

    Public Function saveItem(ByVal strItem As String, ByVal Fields As String) As String
        Dim txtItemNo, txtItemNameAr, txtItemNameEn, drpGroup, chkTax, txtTaxAmount, chkEnabled As String
        txtItemNo = ""
        txtItemNameAr = ""
        txtItemNameEn = ""
        drpGroup = ""
        chkTax = ""
        txtTaxAmount = ""
        chkEnabled = ""

        Dim ErrMsg, ErrMissingData, ErrTaxValue As String
        ErrMsg = ""
        ErrMissingData = "Some data are missing..."
        ErrTaxValue = "If Tax is enabled, then the value should be greater the zero.."

        ' get form values
        Dim jss As New JavaScriptSerializer()
        Dim field() As NameValue = jss.Deserialize(Of NameValue())(Fields)
        For I = 0 To field.Length - 1
            Select Case field(I).name()
                Case "txtItemNo"
                    txtItemNo = field(I).value()
                Case "txtItemNameAr"
                    txtItemNameAr = field(I).value()
                Case "txtItemNameEn"
                    txtItemNameEn = field(I).value()
                Case "drpGroup"
                    drpGroup = field(I).value()
                Case "chkTax"
                    chkTax = field(I).value()
                Case "txtTaxAmount"
                    txtTaxAmount = field(I).value()
                Case "chkEnabled"
                    chkEnabled = field(I).value()
            End Select
        Next

        'validation
        If strItem = "" Then ErrMsg = ErrMissingData
        If txtItemNo = "" Or txtItemNameAr = "" Or txtItemNameEn = "" Then ErrMsg = ErrMissingData
        If chkTax = "" Then chkTax = "0"
        If chkTax = "1" And CDec(txtTaxAmount) <= 0 Then ErrMsg = ErrTaxValue
        If chkEnabled = "" Then chkEnabled = "0"


        If ErrMsg = "" Then
            Try
                dcl.ExecSQuery("UPDATE Stock_Items SET strItemEn='" & txtItemNameEn & "', strItemAr='" & txtItemNameAr & "', intGroup=" & drpGroup & ", bTax=" & chkTax & ", curTax=" & txtTaxAmount & ", bEnabled=" & chkEnabled & " WHERE strItem='" & strItem & "'")
            Catch ex As Exception
                ErrMsg = ex.Message & "<br />"
            End Try
        End If

        If ErrMsg = "" Then
            Return "<script>msg('', 'Changes saved successfully!' , 'success'); $('#mdlAlpha').modal('hide'); fillItems();</script>"
        Else
            Return "Err:" & ErrMsg
        End If
    End Function

    Public Function FilterBarcode(ByVal strBarcode As String) As String()
        Dim ds As DataSet
        Dim strItem As String
        Dim str(3) As String

        If strBarcode <> "" Then
            Select Case strBarcode.Length
                Case 5
                    strItem = strBarcode
                    ds = dcl.GetDS("SELECT * FROM Stock_Items WHERE strItem='" & strItem & "'")
                    If ds.Tables(0).Rows.Count > 0 Then
                        str(0) = strItem
                        Dim dsTemp As DataSet
                        dsTemp = dcl.GetDS("SELECT MAX(curBasePrice) AS curBasePrice, MAX(dateExpiry) AS dateExpiry FROM Stock_Xlink_Items WHERE strItem='" & strItem & "' AND dateExpiry > GETDATE() AND curBasePrice IS NOT NULL AND curBasePrice > 0")
                        If dsTemp.Tables(0).Rows.Count > 0 Then
                            If Not (IsDBNull(dsTemp.Tables(0).Rows(0).Item("curBasePrice"))) And Not (IsDBNull(dsTemp.Tables(0).Rows(0).Item("dateExpiry"))) Then
                                str(1) = CDec(dsTemp.Tables(0).Rows(0).Item("curBasePrice")).ToString("F2")
                                str(2) = CDate(dsTemp.Tables(0).Rows(0).Item("dateExpiry")).ToString("yyyy-MM-dd")
                            Else
                                str(0) = "Err:item not defined"
                            End If
                        Else
                            str(0) = "Err:item not defined"
                        End If
                    Else
                        str(0) = "Err:item not defined"
                    End If
                Case Is < 12
                    strItem = Mid(strBarcode, 1, 8)
                    ds = dcl.GetDS("SELECT * FROM Stock_Items WHERE strItem='" & strItem & "'")
                    If ds.Tables(0).Rows.Count > 0 Then
                        str(0) = strItem
                        str(1) = Mid(strBarcode, 9, 4) & "." & Mid(strBarcode, 13, 2)
                        str(2) = "20" & Mid(strBarcode, 17, 2) & "-" & Mid(strBarcode, 15, 2) & "-01"
                    Else
                        str(0) = "Err:item not defined"
                    End If
                Case 12
                    strItem = Left(strBarcode, 8)
                    ds = dcl.GetDS("SELECT * FROM Stock_Item_Info WHERE strOldReference='" & strItem & "'")
                    If ds.Tables(0).Rows.Count > 0 Then
                        str(0) = strItem
                        str(1) = ""
                        str(2) = "20" & Mid(strBarcode, 12, 1) & "-" & Mid(strBarcode, 10, 2) & "-01"
                    Else
                        str(0) = "Err:item not defined"
                    End If
                Case Is >= 14 ' New Barcode
                    strItem = Left(strBarcode, 5)
                    ds = dcl.GetDS("SELECT * FROM Stock_Items WHERE strItem='" & strItem & "'")
                    If ds.Tables(0).Rows.Count > 0 Then
                        str(0) = strItem
                        If strBarcode.Length = 14 Then
                            str(1) = Mid(strBarcode, 10, 3) & "." & Mid(strBarcode, 13, 2)
                            str(2) = "20" & Mid(strBarcode, 8, 2) & "-" & Mid(strBarcode, 6, 2) & "-01"
                        Else
                            Dim restCount As Integer = strBarcode.Length - 9
                            str(1) = Mid(strBarcode, 10, restCount - 2) & "." & Mid(strBarcode, strBarcode.Length - 1, 2)
                            str(2) = "20" & Mid(strBarcode, 8, 2) & "-" & Mid(strBarcode, 6, 2) & "-01"
                        End If
                    Else
                        str(0) = "Err:item not defined"
                    End If
                Case Else
                    str(0) = "Err:item not defined"
            End Select
        Else
            str(0) = "Err:no barcode"
        End If
        Return str
    End Function

    Public Function checkStock(ByVal strItem As String, ByVal dateTransaction As Date, ByVal byteWarehouse As Byte) As Decimal
        Dim bNegative As Boolean = False
        Dim ds As DataSet

        'If DLookup("bNegative", "Hw_Firm") = False Then
        If bNegative = False Then '=> TODO: I don't know what is that for!
            Dim strSQL As String = "SELECT SUM(SB.intSign * SXI.curQuantity * SU.curFactor)/1 AS curBalance FROM Stock_Base AS SB INNER JOIN Stock_Trans AS ST ON SB.byteBase = ST.byteBase INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink INNER JOIN Stock_Units AS SU ON SU.byteUnit = SXI.byteUnit WHERE ST.byteStatus > 0 And SB.bInclude <> 0 And Year(dateTransaction) = " & intYear & " And SXI.byteWarehouse = " & byteWarehouse & " AND SXI.strItem='" & strItem & "' AND ST.dateTransaction <= '" & dateTransaction.ToString("yyyy-MM-dd") & "'"

            ds = dcl.GetDS(strSQL)
            If IsDBNull(ds.Tables(0).Rows(0).Item("curBalance")) Then
                Return 0
            Else
                Return ds.Tables(0).Rows(0).Item("curBalance")
            End If

        End If
        Return 0
    End Function

    Public Function checkStock(ByVal strItem As String, ByVal dateTransaction As Date, ByVal byteWarehouse As Byte, ByVal dateExpiry As Date) As Decimal
        Dim bNegative As Boolean = False
        Dim ds As DataSet

        'If DLookup("bNegative", "Hw_Firm") = False Then
        If bNegative = False Then '=> TODO: I don't know what is that for!
            Dim strSQL As String = "SELECT SUM(SB.intSign * SXI.curQuantity * SU.curFactor)/1 AS curBalance FROM Stock_Base AS SB INNER JOIN Stock_Trans AS ST ON SB.byteBase = ST.byteBase INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink INNER JOIN Stock_Units AS SU ON SU.byteUnit = SXI.byteUnit WHERE ST.byteStatus > 0 And SB.bInclude <> 0 And Year(dateTransaction) = " & intYear & " And SXI.byteWarehouse = " & byteWarehouse & " AND SXI.strItem='" & strItem & "' AND ST.dateTransaction <= '" & dateTransaction.ToString("yyyy-MM-dd") & "' AND SXI.dateExpiry='" & dateExpiry.ToString("yyyy-MM-dd") & "'"

            ds = dcl.GetDS(strSQL)
            If IsDBNull(ds.Tables(0).Rows(0).Item("curBalance")) Then
                Return 0
            Else
                Return ds.Tables(0).Rows(0).Item("curBalance")
            End If

        End If
        Return 0
    End Function

    Public Function getItemCost(ByVal strItem As String) As String
        Dim ds As DataSet
        Try
            ds = dcl.GetDS("SELECT XI.curUnitNetCost, T.strTransaction FROM Stock_Trans AS T INNER JOIN Stock_Xlink AS X ON T.lngTransaction = X.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink WHERE T.byteBase IN (14,15) AND XI.strItem = '" & strItem & "' AND XI.curUnitNetCost IS NOT NULL AND XI.curUnitNetCost > 0 GROUP BY XI.curUnitNetCost, T.strTransaction, T.dateTransaction ORDER BY T.dateTransaction DESC;")
            If ds.Tables(0).Rows.Count > 0 Then
                Return ds.Tables(0).Rows(0).Item("curUnitNetCost")
            Else
                Return "Err: No Cost for item (" & strItem & ")"
            End If
        Catch ex As Exception
            Return "Err: " & ex.Message
        End Try
    End Function

    Public Function fillSExpired(ByVal dateFrom As Date, ByVal dateTo As Date, ByVal byteWarehouse As Byte) As String
        Dim colTransNo, colDate, colType, colWarehouseFrom, colSender, colCount, colQuantity, colStatus1, colRecipient, colWarehouseTo, colStatus2 As String
        Dim Pending, Approved, Cancelled As String
        Dim table As New StringBuilder("")
        Dim Where As String = ""
        Dim Having As String = ""

        Select Case byteLanguage
            Case 2
                DataLang = "Ar"
                'Variables
                Pending = "<span class=""tag tag-warning"">في الانتظار</span>"
                Approved = "<span class=""tag tag-success"">معمدة</span>"
                Cancelled = "<span class=""tag tag-danger"">ملغية</span>"
                'Columns
                colTransNo = "رقم السند"
                colDate = "التاريخ"
                colType = "السند"
                colWarehouseFrom = "من"
                colSender = "المرسل"
                colCount = "عدد الأصناف"
                colQuantity = "إجمالي الكمية"
                colStatus1 = "حالة الارسال"
                colWarehouseTo = "إلى"
                colRecipient = "المستلم"
                colStatus2 = "حالة الاستلام"
            Case Else
                DataLang = "En"
                'Variables
                Pending = "<span class=""tag tag-warning"">Pending</span>"
                Approved = "<span class=""tag tag-success"">Approved</span>"
                Cancelled = "<span class=""tag tag-danger"">Cancelled</span>"
                'Columns
                colTransNo = "Voucher No"
                colDate = "Date"
                colType = "Voucher"
                colWarehouseFrom = "From"
                colSender = "Sender"
                colCount = "Items Count"
                colQuantity = "Total Quantity"
                colStatus1 = "Send Status"
                colWarehouseTo = "To"
                colRecipient = "Recipient"
                colStatus2 = "Receive Status"
        End Select
        Dim status1 As String = ""
        Dim status2 As String = ""

        Where = " AND CONVERT(varchar(10), T1.dateTransaction, 120) BETWEEN '" & dateFrom.ToString("yyyy-MM-dd") & "' AND '" & dateTo.ToString("yyyy-MM-dd") & "'"
        If byteWarehouse <> 0 Then
            Where = Where & " AND T1.byteWarehouse = " & byteWarehouse
        End If

        table.Append("<table class=""table tableAjax table-hover table-bordered mb-0"">")
        table.Append("<thead><tr><th>" & colTransNo & "</th><th>" & colDate & "</th><th>" & colType & "</th><th>" & colWarehouseFrom & "</th><th>" & colSender & "</th><th>" & colCount & "</th><th>" & colQuantity & "</th><th>" & colStatus1 & "</th><th>" & colWarehouseTo & "</th><th>" & colRecipient & "</th><th>" & colStatus2 & "</th></tr></thead>")
        Try
            Dim ds As DataSet
            ds = dcl.GetDS("SELECT T1.lngTransaction, T1.strTransaction, T1.dateTransaction, TT1.strType" & DataLang & ", W1.strWarehouse" & DataLang & " AS strWarehouseFrom, W2.strWarehouse" & DataLang & " AS strWarehouseTo, TA1.strCreatedBy AS Sender, TA2.strCreatedBy AS Recipient, T1.byteStatus AS SenderStatus, T2.byteStatus AS RecipientStatus, COUNT(XI1.lngXlink) AS intCount, SUM(ISNULL(XI1.curQuantity, 0)) AS curQuantity FROM Stock_Xlink AS X1 LEFT JOIN Stock_Xlink AS X2 ON X1.lngPointer=X2.lngPointer INNER JOIN Stock_Trans AS T1 ON T1.lngTransaction=X1.lngTransaction INNER JOIN Stock_Trans AS T2 ON T2.lngTransaction=X2.lngTransaction INNER JOIN Stock_Warehouses AS W1 ON T1.byteWarehouse=W1.byteWarehouse INNER JOIN Stock_Warehouses AS W2 ON T2.byteWarehouse=W2.byteWarehouse INNER JOIN Stock_Trans_Types AS TT1 ON T1.byteTransType=TT1.byteTransType INNER JOIN Stock_Trans_Audit AS TA1 ON T1.lngTransaction=TA1.lngTransaction LEFT JOIN Stock_Trans_Audit AS TA2 ON T2.lngTransaction=TA2.lngTransaction LEFT JOIN Stock_Xlink_Items AS XI1 ON X1.lngXlink=XI1.lngXlink WHERE ((T1.byteBase=19 AND T2.byteBase=52) OR (T1.byteBase=51 AND T2.byteBase=20)) AND ((T1.byteTransType=14 AND T2.byteTransType=42) OR (T1.byteTransType=41 AND T2.byteTransType=15))" & Where & " GROUP BY T1.lngTransaction, T1.strTransaction, T1.dateTransaction, TT1.strType" & DataLang & ", W1.strWarehouse" & DataLang & ", W2.strWarehouse" & DataLang & ", TA1.strCreatedBy, TA2.strCreatedBy, T1.byteStatus, T2.byteStatus")
            For I = 0 To ds.Tables(0).Rows.Count - 1
                Select Case ds.Tables(0).Rows(I).Item("SenderStatus")
                    Case 0
                        status1 = Cancelled
                    Case 1
                        status1 = Pending
                    Case 2
                        status1 = Approved
                End Select
                Select Case ds.Tables(0).Rows(I).Item("RecipientStatus")
                    Case 0
                        status2 = Cancelled
                    Case 1
                        status2 = Pending
                    Case 2
                        status2 = Approved
                End Select
                table.Append("<tr class=""cursor-pointer"" id=""row" & ds.Tables(0).Rows(I).Item("lngTransaction") & """ onclick=""javascript:showModal('viewSExpired', '{lngTransaction: " & ds.Tables(0).Rows(I).Item("lngTransaction") & ", IsOut: false}', '#mdlAlpha')"">")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("strTransaction") & "</td>")
                table.Append("<td>" & CDate(ds.Tables(0).Rows(I).Item("dateTransaction")).ToString(strDateFormat) & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("strType" & DataLang) & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("strWarehouseFrom") & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("Sender") & "</td>")
                table.Append("<td><b>" & Math.Round(ds.Tables(0).Rows(I).Item("intCount"), 2, MidpointRounding.AwayFromZero) & "</b></td>")
                table.Append("<td><b>" & Math.Round(ds.Tables(0).Rows(I).Item("curQuantity"), 2, MidpointRounding.AwayFromZero) & "</b></td>")
                table.Append("<td>" & status1 & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("strWarehouseTo") & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("Recipient") & "</td>")
                table.Append("<td>" & status2 & "</td>")
                table.Append("</tr>")
            Next
        Catch ex As Exception
            Return "Err:" & ex.Message
        End Try
        table.Append("</tbody></table>")
        table.Append("<script>$('table.tableAjax').dataTable({language: tableLanguage, order: [[0, 'desc']]});</script>")

        Return table.ToString
    End Function

    Public Function fillSReturned(ByVal dateFrom As Date, ByVal dateTo As Date, ByVal byteWarehouse As Byte) As String
        Dim colTransNo, colDate, colType, colWarehouseFrom, colSender, colCount, colQuantity, colStatus1, colRecipient, colWarehouseTo, colStatus2 As String
        Dim Pending, Approved, Cancelled As String
        Dim table As New StringBuilder("")
        Dim Where As String = ""
        Dim Having As String = ""

        Select Case byteLanguage
            Case 2
                DataLang = "Ar"
                'Variables
                Pending = "<span class=""tag tag-warning"">في الانتظار</span>"
                Approved = "<span class=""tag tag-success"">معمدة</span>"
                Cancelled = "<span class=""tag tag-danger"">ملغية</span>"
                'Columns
                colTransNo = "رقم السند"
                colDate = "التاريخ"
                colType = "السند"
                colWarehouseFrom = "من"
                colSender = "المرسل"
                colCount = "عدد الأصناف"
                colQuantity = "إجمالي الكمية"
                colStatus1 = "حالة الارسال"
                colWarehouseTo = "إلى"
                colRecipient = "المستلم"
                colStatus2 = "حالة الاستلام"
            Case Else
                DataLang = "En"
                'Variables
                Pending = "<span class=""tag tag-warning"">Pending</span>"
                Approved = "<span class=""tag tag-success"">Approved</span>"
                Cancelled = "<span class=""tag tag-danger"">Cancelled</span>"
                'Columns
                colTransNo = "Voucher No"
                colDate = "Date"
                colType = "Voucher"
                colWarehouseFrom = "From"
                colSender = "Sender"
                colCount = "Items Count"
                colQuantity = "Total Quantity"
                colStatus1 = "Send Status"
                colWarehouseTo = "To"
                colRecipient = "Recipient"
                colStatus2 = "Receive Status"
        End Select
        Dim status1 As String = ""
        Dim status2 As String = ""

        Where = " AND CONVERT(varchar(10), T1.dateTransaction, 120) BETWEEN '" & dateFrom.ToString("yyyy-MM-dd") & "' AND '" & dateTo.ToString("yyyy-MM-dd") & "'"
        If byteWarehouse <> 0 Then
            Where = Where & " AND T1.byteWarehouse = " & byteWarehouse
        End If

        table.Append("<table class=""table tableAjax table-hover table-bordered mb-0"">")
        table.Append("<thead><tr><th>" & colTransNo & "</th><th>" & colDate & "</th><th>" & colType & "</th><th>" & colWarehouseFrom & "</th><th>" & colSender & "</th><th>" & colCount & "</th><th>" & colQuantity & "</th><th>" & colStatus1 & "</th><th>" & colWarehouseTo & "</th><th>" & colRecipient & "</th><th>" & colStatus2 & "</th></tr></thead>")
        Try
            Dim ds As DataSet
            ds = dcl.GetDS("SELECT T1.lngTransaction, T1.strTransaction, T1.dateTransaction, TT1.strType" & DataLang & ", W1.strWarehouse" & DataLang & " AS strWarehouseFrom, W2.strWarehouse" & DataLang & " AS strWarehouseTo, TA1.strCreatedBy AS Sender, TA2.strCreatedBy AS Recipient, T1.byteStatus AS SenderStatus, T2.byteStatus AS RecipientStatus, COUNT(XI1.lngXlink) AS intCount, SUM(ISNULL(XI1.curQuantity, 0)) AS curQuantity FROM Stock_Xlink AS X1 LEFT JOIN Stock_Xlink AS X2 ON X1.lngPointer=X2.lngPointer INNER JOIN Stock_Trans AS T1 ON T1.lngTransaction=X1.lngTransaction INNER JOIN Stock_Trans AS T2 ON T2.lngTransaction=X2.lngTransaction INNER JOIN Stock_Warehouses AS W1 ON T1.byteWarehouse=W1.byteWarehouse INNER JOIN Stock_Warehouses AS W2 ON T2.byteWarehouse=W2.byteWarehouse INNER JOIN Stock_Trans_Types AS TT1 ON T1.byteTransType=TT1.byteTransType INNER JOIN Stock_Trans_Audit AS TA1 ON T1.lngTransaction=TA1.lngTransaction LEFT JOIN Stock_Trans_Audit AS TA2 ON T2.lngTransaction=TA2.lngTransaction LEFT JOIN Stock_Xlink_Items AS XI1 ON X1.lngXlink=XI1.lngXlink WHERE ((T1.byteBase=19 AND T2.byteBase=52) OR (T1.byteBase=51 AND T2.byteBase=20)) AND ((T1.byteTransType=14 AND T2.byteTransType=40) OR (T1.byteTransType=39 AND T2.byteTransType=15)) " & Where & " GROUP BY T1.lngTransaction, T1.strTransaction, T1.dateTransaction, TT1.strType" & DataLang & ", W1.strWarehouse" & DataLang & ", W2.strWarehouse" & DataLang & ", TA1.strCreatedBy, TA2.strCreatedBy, T1.byteStatus, T2.byteStatus")
            For I = 0 To ds.Tables(0).Rows.Count - 1
                Select Case ds.Tables(0).Rows(I).Item("SenderStatus")
                    Case 0
                        status1 = Cancelled
                    Case 1
                        status1 = Pending
                    Case 2
                        status1 = Approved
                End Select
                Select Case ds.Tables(0).Rows(I).Item("RecipientStatus")
                    Case 0
                        status2 = Cancelled
                    Case 1
                        status2 = Pending
                    Case 2
                        status2 = Approved
                End Select
                table.Append("<tr class=""cursor-pointer"" id=""row" & ds.Tables(0).Rows(I).Item("lngTransaction") & """ onclick=""javascript:showModal('viewSExpired', '{lngTransaction: " & ds.Tables(0).Rows(I).Item("lngTransaction") & ", IsOut: false}', '#mdlAlpha')"">")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("strTransaction") & "</td>")
                table.Append("<td>" & CDate(ds.Tables(0).Rows(I).Item("dateTransaction")).ToString(strDateFormat) & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("strType" & DataLang) & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("strWarehouseFrom") & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("Sender") & "</td>")
                table.Append("<td><b>" & Math.Round(ds.Tables(0).Rows(I).Item("intCount"), 2, MidpointRounding.AwayFromZero) & "</b></td>")
                table.Append("<td><b>" & Math.Round(ds.Tables(0).Rows(I).Item("curQuantity"), 2, MidpointRounding.AwayFromZero) & "</b></td>")
                table.Append("<td>" & status1 & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("strWarehouseTo") & "</td>")
                table.Append("<td>" & ds.Tables(0).Rows(I).Item("Recipient") & "</td>")
                table.Append("<td>" & status2 & "</td>")
                table.Append("</tr>")
            Next
        Catch ex As Exception
            Return "Err:" & ex.Message
        End Try
        table.Append("</tbody></table>")
        table.Append("<script>$('table.tableAjax').dataTable({language: tableLanguage, order: [[0, 'desc']]});</script>")

        Return table.ToString
    End Function

    Public Function fillExpiredItems(ByVal strItem As String, ByVal dateExpiry As Date, ByVal byteWarehouse As Byte) As String
        Dim colTransNo, colDate, colType, colWarehouseFrom, colSender, colCount, colQuantity, colStatus1, colRecipient, colWarehouseTo, colStatus2 As String
        Dim Pending, Approved, Cancelled As String
        Dim table As New StringBuilder("")
        Dim Where As String = ""
        Dim Having As String = ""

        'Select Case byteLanguage
        '    Case 2
        '        DataLang = "Ar"
        '        'Variables
        '        Pending = "<span class=""tag tag-warning"">في الانتظار</span>"
        '        Approved = "<span class=""tag tag-success"">معمدة</span>"
        '        Cancelled = "<span class=""tag tag-danger"">ملغية</span>"
        '        'Columns
        '        colTransNo = "رقم السند"
        '        colDate = "التاريخ"
        '        colType = "السند"
        '        colWarehouseFrom = "من"
        '        colSender = "المرسل"
        '        colCount = "عدد الأصناف"
        '        colQuantity = "إجمالي الكمية"
        '        colStatus1 = "حالة الارسال"
        '        colWarehouseTo = "إلى"
        '        colRecipient = "المستلم"
        '        colStatus2 = "حالة الاستلام"
        '    Case Else
        '        DataLang = "En"
        '        'Variables
        '        Pending = "<span class=""tag tag-warning"">Pending</span>"
        '        Approved = "<span class=""tag tag-success"">Approved</span>"
        '        Cancelled = "<span class=""tag tag-danger"">Cancelled</span>"
        '        'Columns
        '        colTransNo = "Voucher No"
        '        colDate = "Date"
        '        colType = "Voucher"
        '        colWarehouseFrom = "From"
        '        colSender = "Sender"
        '        colCount = "Items Count"
        '        colQuantity = "Total Quantity"
        '        colStatus1 = "Send Status"
        '        colWarehouseTo = "To"
        '        colRecipient = "Recipient"
        '        colStatus2 = "Receive Status"
        'End Select
        'Dim status1 As String = ""
        'Dim status2 As String = ""

        'Where = " AND CONVERT(varchar(10), T1.dateTransaction, 120) BETWEEN '" & dateFrom.ToString("yyyy-MM-dd") & "' AND '" & dateTo.ToString("yyyy-MM-dd") & "'"
        'Where = Where & " AND YEAR(T1.dateTransaction) = " & intYear
        'If byteWarehouse <> 0 Then
        '    Where = Where & " AND T2.byteWarehouse = " & byteWarehouse
        'End If
        ''Where = Where & " AND T2.byteWarehouse = " & byteWarehouse
        'Where = Where & " AND T1.byteStatus = 2"

        'table.Append("<table class=""table tableAjax table-hover table-bordered mb-0"">")
        'table.Append("<thead><tr><th>" & colTransNo & "</th><th>" & colDate & "</th><th>" & colType & "</th><th>" & colWarehouseFrom & "</th><th>" & colSender & "</th><th>" & colCount & "</th><th>" & colQuantity & "</th><th>" & colStatus1 & "</th><th>" & colWarehouseTo & "</th><th>" & colRecipient & "</th><th>" & colStatus2 & "</th></tr></thead>")
        'Try
        '    Dim ds As DataSet
        '    'ds = dcl.GetDS("SELECT T1.lngTransaction, T1.strTransaction, T1.dateTransaction, TT1.strType" & DataLang & ", W1.strWarehouse" & DataLang & " AS strWarehouseFrom, W2.strWarehouse" & DataLang & " AS strWarehouseTo, TA1.strCreatedBy AS Sender, TA2.strCreatedBy AS Recipient, T1.byteStatus AS SenderStatus, T2.byteStatus AS RecipientStatus, COUNT(XI.lngXlink) AS intCount, SUM(ISNULL(XI.curQuantity, 0)) AS curQuantity FROM Stock_Trans AS T1 LEFT JOIN Stock_Trans AS T2 ON T1.strTransaction=T2.strTransaction AND T1.lngTransaction<>T2.lngTransaction INNER JOIN Stock_Warehouses AS W1 ON T1.byteWarehouse=W1.byteWarehouse INNER JOIN Stock_Warehouses AS W2 ON T2.byteWarehouse=W2.byteWarehouse INNER JOIN Stock_Trans_Types AS TT1 ON T1.byteTransType=TT1.byteTransType INNER JOIN Stock_Trans_Audit AS TA1 ON T1.lngTransaction=TA1.lngTransaction INNER JOIN Stock_Trans_Audit AS TA2 ON T2.lngTransaction=TA2.lngTransaction INNER JOIN Stock_Xlink AS X ON X.lngTransaction=T1.lngTransaction LEFT JOIN Stock_Xlink_Items AS XI ON X.lngXlink=XI.lngXlink WHERE YEAR(T1.dateTransaction) = 2019 AND YEAR(T2.dateTransaction) = 2019 AND T1.byteBase=19 AND T2.byteBase=20 " & Where & " GROUP BY T1.lngTransaction, T1.strTransaction, T1.dateTransaction, TT1.strType" & DataLang & ", W1.strWarehouse" & DataLang & ", W2.strWarehouse" & DataLang & ", TA1.strCreatedBy, TA2.strCreatedBy, T1.byteStatus, T2.byteStatus")
        '    ds = dcl.GetDS("SELECT T1.lngTransaction, T1.strTransaction, T1.dateTransaction, TT1.strType" & DataLang & ", W1.strWarehouse" & DataLang & " AS strWarehouseFrom, W2.strWarehouse" & DataLang & " AS strWarehouseTo, TA1.strCreatedBy AS Sender, TA2.strCreatedBy AS Recipient, T1.byteStatus AS SenderStatus, T2.byteStatus AS RecipientStatus, COUNT(XI1.lngXlink) AS intCount, SUM(ISNULL(XI1.curQuantity, 0)) AS curQuantity FROM Stock_Xlink AS X1 LEFT JOIN Stock_Xlink AS X2 ON X1.lngPointer=X2.lngPointer INNER JOIN Stock_Trans AS T1 ON T1.lngTransaction=X1.lngTransaction INNER JOIN Stock_Trans AS T2 ON T2.lngTransaction=X2.lngTransaction INNER JOIN Stock_Warehouses AS W1 ON T1.byteWarehouse=W1.byteWarehouse INNER JOIN Stock_Warehouses AS W2 ON T2.byteWarehouse=W2.byteWarehouse INNER JOIN Stock_Trans_Types AS TT1 ON T1.byteTransType=TT1.byteTransType INNER JOIN Stock_Trans_Audit AS TA1 ON T1.lngTransaction=TA1.lngTransaction LEFT JOIN Stock_Trans_Audit AS TA2 ON T2.lngTransaction=TA2.lngTransaction LEFT JOIN Stock_Xlink_Items AS XI1 ON X1.lngXlink=XI1.lngXlink WHERE T1.byteBase=19 AND T2.byteBase=20 " & Where & " GROUP BY T1.lngTransaction, T1.strTransaction, T1.dateTransaction, TT1.strType" & DataLang & ", W1.strWarehouse" & DataLang & ", W2.strWarehouse" & DataLang & ", TA1.strCreatedBy, TA2.strCreatedBy, T1.byteStatus, T2.byteStatus")
        '    For I = 0 To ds.Tables(0).Rows.Count - 1
        '        Select Case ds.Tables(0).Rows(I).Item("SenderStatus")
        '            Case 0
        '                status1 = Cancelled
        '            Case 1
        '                status1 = Pending
        '            Case 2
        '                status1 = Approved
        '        End Select
        '        Select Case ds.Tables(0).Rows(I).Item("RecipientStatus")
        '            Case 0
        '                status2 = Cancelled
        '            Case 1
        '                status2 = Pending
        '            Case 2
        '                status2 = Approved
        '        End Select
        '        table.Append("<tr class=""cursor-pointer"" id=""row" & ds.Tables(0).Rows(I).Item("lngTransaction") & """ onclick=""javascript:showModal('viewTransfer', '{lngTransaction: " & ds.Tables(0).Rows(I).Item("lngTransaction") & "}', '#mdlAlpha')"">")
        '        table.Append("<td>" & ds.Tables(0).Rows(I).Item("strTransaction") & "</td>")
        '        table.Append("<td>" & CDate(ds.Tables(0).Rows(I).Item("dateTransaction")).ToString(strDateFormat) & "</td>")
        '        table.Append("<td>" & ds.Tables(0).Rows(I).Item("strType" & DataLang) & "</td>")
        '        table.Append("<td>" & ds.Tables(0).Rows(I).Item("strWarehouseFrom") & "</td>")
        '        table.Append("<td>" & ds.Tables(0).Rows(I).Item("Sender") & "</td>")
        '        table.Append("<td><b>" & Math.Round(ds.Tables(0).Rows(I).Item("intCount"), 2, MidpointRounding.AwayFromZero) & "</b></td>")
        '        table.Append("<td><b>" & Math.Round(ds.Tables(0).Rows(I).Item("curQuantity"), 2, MidpointRounding.AwayFromZero) & "</b></td>")
        '        table.Append("<td>" & status1 & "</td>")
        '        table.Append("<td>" & ds.Tables(0).Rows(I).Item("strWarehouseTo") & "</td>")
        '        table.Append("<td>" & ds.Tables(0).Rows(I).Item("Recipient") & "</td>")
        '        table.Append("<td>" & status2 & "</td>")
        '        table.Append("</tr>")
        '    Next
        'Catch ex As Exception
        '    Return "Err:" & ex.Message
        'End Try
        'table.Append("</tbody></table>")
        'table.Append("<script>$('table.tableAjax').dataTable({language: tableLanguage, order: [[0, 'desc']]});</script>")

        Return table.ToString
    End Function
End Class
