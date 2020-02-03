Public Class r_cash
    Inherits System.Web.UI.Page
    Dim dcl As New DCL.Conn.DataClassLayer
    Public DataLang As String
    Public searchWord, btnSearch, Both, PaymentType, dte, InvType, byUser, byDoctor, byInvType, doct As String
    Dim Insurance As String
    Public colInvoiceNo, colDate, colTime, colUser, colPatientNo, colPatientName, colCompany, colAmount, colDiscount, colCash, colCredit, colPayment, colVAT, colStatus As String
    Dim byteOrdersLimitDays As Byte
    Public ByteLanguage As Byte
    Public strUserName, strDateFormat, strTimeFormat As String
    Public strFilter As String
    Public ReportType As Byte
    Dim Paid, Cancelled As String
    Public totalInvoice, totalAmount, totalDiscount, totalVAT, totalCash, totalCredit, totalPayment As Decimal
    Public strWait As String
    Public selectedColumns As String

    Private Sub r_sales_Init(sender As Object, e As EventArgs) Handles Me.Init
        ' User Options
        If (HttpContext.Current.Session("UserName") Is Nothing) Or (HttpContext.Current.Session("UserName") = "") Then
            'Remove this line after complete user login and settings
            'HttpContext.Current.Session("UserName") = "SoftNet"
            Response.Redirect("../login.aspx")
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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        loadLanguage()

        If Session("r_sales_col") Is Nothing Or Session("r_sales_col") = "" Then
            selectedColumns = ""
            Session("r_sales_col") = selectedColumns
        Else
            selectedColumns = Session("r_sales_col")
        End If

        Dim dateFrom, dateTo, invoiceNo, invoiceStatus, paymentType, doctor, user, reportView As String

        If Request.QueryString("f") Is Nothing Or Request.QueryString("f") = "" Then
            ReportType = 1
            dateFrom = Today.ToString("yyyy-MM-dd")
            dateTo = Today.ToString("yyyy-MM-dd")
            invoiceNo = ""
            invoiceStatus = "1100"
            paymentType = "2"
            doctor = ""
            user = ""
            reportView = "0"
        Else
            Dim param() As String = Split(Request.QueryString("f"), ",")
            ReportType = param(0)
            dateFrom = param(1)
            dateTo = param(2)
            invoiceNo = param(3)
            invoiceStatus = param(4)
            paymentType = param(5)
            doctor = param(6)
            user = param(7)
            reportView = param(8)
        End If

        strFilter = ReportType & "," & dateFrom & "," & dateTo & "," & invoiceNo & "," & invoiceStatus & "," & paymentType & "," & doctor & "," & user & "," & reportView
        'divFilter.InnerHtml = getDescription(strFilter)

        Dim ds As DataSet
        Dim Query As String = ""
        Dim Where As String = "(T.byteBase BETWEEN 40 AND 49) AND TT.lngContact=27 AND T.byteCurrency=3 AND T.dateTransaction BETWEEN '" & dateFrom & "' AND '" & dateTo & "'"
        If invoiceNo <> "" Then Where = Where & " AND T.strTransaction = '" & invoiceNo & "'"
        If (doctor <> "" And doctor <> "0") Then Where = Where & " AND T.lngSalesman=" & doctor
        If user <> "" Then Where = Where & " AND TA.strCreatedBy='" & user & "'"
        If paymentType <> "2" Then Where = Where & " AND T.bCash = " & paymentType
        'If invoiceStatus <> "2" Then Where = Where & " AND T.byteStatus = " & invoiceStatus 'Else Where = Where & " AND T.byteStatus IN (0,1)"
        Select Case invoiceStatus
            Case 0 ' Cancelled
                Where = Where & " AND T.byteStatus = 0"
            Case 1 ' Paid
                Where = Where & " AND T.byteStatus = 1"
            Case 2 ' Unpaid
                Where = Where & " AND T.byteStatus = 2"
            Case 3 ' Not Cancelled
                Where = Where & " AND T.byteStatus > 0"
            Case Else 'All
                '
        End Select

        Select Case ReportType
            Case 1
                Query = buildQuery2(Where)
        End Select
        'tblBady.innerHTML = buildTable(Query)
        'ds = dcl.GetDS(Query)
        'repInvoices.DataSource = ds
        'repInvoices.DataBind()
    End Sub

    Function getDescription(ByVal strFilter As String) As String
        Dim lblReportType, lblDateFrom, lblDateTo, lblInvoiceNo, lblInvoiceStatus, lblPaymentType, lblDoctor, lblUser As String
        Dim Cash, Credit, Both, Paid, Unpaid, Cancelled, NotCancelled, All As String
        Dim btnSave, btnClose, btnApply, btnCancel As String
        Dim ReportTypeItem(6), InvoiceStatus(5), PaymentType(3) As String

        Select Case ByteLanguage
            Case 2
                'Lables
                lblReportType = "نوع التقرير"
                lblDateFrom = "من تاريخ"
                lblDateTo = "إلى تاريخ"
                lblInvoiceNo = "رقم الفاتورة"
                lblInvoiceStatus = "حالة الفاتورة"
                lblPaymentType = "نوع الدفع"
                lblDoctor = "الطبيب"
                lblUser = "المستخدم"
                'Buttons
                btnSave = "بدء الفرز"
                btnClose = "إغلاق"
                btnApply = "تطبيق"
                btnCancel = "تراجع"
                'Variables
                PaymentType(1) = "نقدي"
                PaymentType(0) = "آجل"
                PaymentType(2) = "كلاهما"
                InvoiceStatus(1) = "مدفوعة"
                InvoiceStatus(2) = "غير مدفوعة"
                InvoiceStatus(0) = "ملغية"
                InvoiceStatus(3) = "غير ملغية"
                InvoiceStatus(4) = "الكل"
                strWait = "فضلا انتظر..."
                'List
                ReportTypeItem(0) = "حسب نوع الفاتورة"
                ReportTypeItem(1) = "حسب الطبيب"
                ReportTypeItem(2) = "حسب المستخدم"
                ReportTypeItem(3) = "حسب المستخدم(ملخص)"
                ReportTypeItem(4) = "الفواتير غير المطبوعة"
                ReportTypeItem(5) = "حسب الطبيب(ملخص)"
            Case Else
                'Lables
                lblReportType = "Report Type"
                lblDateFrom = "From Date"
                lblDateTo = "To Date"
                lblInvoiceNo = "Invoice No"
                lblInvoiceStatus = "Invoice Status"
                lblPaymentType = "Payment Type"
                lblDoctor = "Doctor"
                lblUser = "User"
                'Buttons
                btnSave = "Start Filter"
                btnClose = "Close"
                btnApply = "Apply"
                btnCancel = "Cancel"
                'Variables
                PaymentType(1) = "Cash"
                PaymentType(0) = "Credit"
                PaymentType(2) = "Both"
                InvoiceStatus(1) = "Paid"
                InvoiceStatus(2) = "Unpaid"
                InvoiceStatus(0) = "Cancelled"
                InvoiceStatus(3) = "Not Cancelled"
                InvoiceStatus(4) = "All"
                strWait = "Please wait..."
                'List
                ReportTypeItem(0) = "By Invoice Type"
                ReportTypeItem(1) = "By Doctor"
                ReportTypeItem(2) = "By User"
                ReportTypeItem(3) = "By User (Summary)"
                ReportTypeItem(4) = "By Unprinted Invoices"
                ReportTypeItem(5) = "By Doctor (Summary)"
        End Select

        Dim filter() As String = Split(strFilter, ",")
        If filter.Length > 0 Then
            Dim str As String = ""
            str = str & "<b>" & lblReportType & ":</b> " & ReportTypeItem(filter(0) - 1) & "<br />"
            str = str & "<b>" & lblDateFrom & ":</b> " & CDate(filter(1)).ToString(strDateFormat) & " <b>" & lblDateTo & ":</b> " & CDate(filter(2)).ToString(strDateFormat) & "<br />"
            If filter(3) <> "" Then str = str & "<b> " & lblInvoiceNo & ":</b> " & filter(3) & "<br />"
            str = str & "<b>" & lblInvoiceStatus & ":</b> " & InvoiceStatus(filter(4)) & "<br />"
            str = str & "<b>" & lblPaymentType & ":</b> " & PaymentType(filter(5)) & "<br />"
            If filter(6) <> "" Then str = str & "<b>" & lblDoctor & ":</b> " & getDoctorName(filter(6)) & "<br />"
            If filter(7) <> "" Then str = str & "<b>" & lblUser & ":</b> " & filter(7) & "<br />"
            Return str
        Else
            Return ""
        End If
    End Function

    Function getDoctorName(ByVal lngContact As Long) As String
        Dim ds As DataSet
        ds = dcl.GetDS("SELECT * FROM Hw_Contacts WHERE lngContact=" & lngContact)
        If ds.Tables(0).Rows.Count > 0 Then
            Return ds.Tables(0).Rows(0).Item("strContact" & DataLang).ToString
        Else
            Return ""
        End If
    End Function

    Function buildTable(ByVal Query As String) As String
        Dim tbl As New StringBuilder("")
        Dim ds As DataSet
        ds = dcl.GetDS(Query)
        For I = 0 To ds.Tables(0).Rows.Count - 1
            tbl.Append("<tr>")
            tbl.Append("<td>" & ds.Tables(0).Rows(I).Item("strTransaction") & "</td>")
            tbl.Append("<td>" & CDate(ds.Tables(0).Rows(I).Item("dateEntry")).ToString(strDateFormat) & "</td>")
            tbl.Append("<td>" & CDate(ds.Tables(0).Rows(I).Item("dateEntry")).ToString(strTimeFormat) & "</td>")
            tbl.Append("<td>" & ds.Tables(0).Rows(I).Item("strCreatedBy").ToString & "</td>")
            tbl.Append("<td>" & ds.Tables(0).Rows(I).Item("lngPatient").ToString & "</td>")
            tbl.Append("<td>" & ds.Tables(0).Rows(I).Item("strPatient" & DataLang).ToString & "</td>")
            tbl.Append("<td>" & ds.Tables(0).Rows(I).Item("strContact" & DataLang).ToString & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curAmount")) & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curDiscount")) & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curVAT")) & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curCash")) & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curCredit")) & "</td>")
            tbl.Append("<td>" & formatNumber(ds.Tables(0).Rows(I).Item("curPayment")) & "</td>")
            tbl.Append("<td>" & getStatus(ds.Tables(0).Rows(I).Item("byteStatus")) & "</td>")
            tbl.Append("</tr>")

            totalInvoice = totalInvoice + 1
            totalAmount = totalAmount + ds.Tables(0).Rows(I).Item("curAmount")
            totalDiscount = totalDiscount + ds.Tables(0).Rows(I).Item("curDiscount")
            totalVAT = totalVAT + ds.Tables(0).Rows(I).Item("curVAT")
            totalCash = totalCash + ds.Tables(0).Rows(I).Item("curCash")
            totalCredit = totalCredit + ds.Tables(0).Rows(I).Item("curCredit")
            totalPayment = totalPayment + ds.Tables(0).Rows(I).Item("curPayment")
        Next
        Return tbl.ToString
    End Function

    Public Function getStatus(ByVal byteStatus As Byte) As String
        Select Case byteStatus
            Case 0
                Return "<span class=""tag tag-sm tag-danger"">" & Cancelled & "</span>"
            Case 1
                Return "<span class=""tag tag-sm tag-success"">" & Paid & "</span>"
            Case Else
                Return byteStatus
        End Select
    End Function

    Public Function formatNumber(ByVal Number As Decimal) As String
        Number = Math.Round(Number, 2, MidpointRounding.AwayFromZero)
        If Number > 0 Then Return "<b>" & Number & "</b>" Else Return Number
    End Function

    Function buildQuery1(ByVal Where As String) As String
        Dim Query As String = ""
        Query = Query & "DECLARE @Stock_Total AS TABLE (lngTransaction int, lngXlink int, TotalCost money, TotalNet money, TotalQuantity money, CashVAT money, CreditVAT money);"
        Query = Query & "INSERT INTO @Stock_Total SELECT T.lngTransaction, XI.lngXlink, SUM(B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curBasePrice, 0)) + ISNULL(XI.curVAT, 0) + ISNULL(XI.curVATI, 0))) AS TotalCost, SUM(B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curUnitPrice, 0)) + ISNULL(XI.curVAT, 0) + ISNULL(XI.curVATI, 0))) AS TotalNet, SUM(B.intSign * ISNULL(XI.curQuantity, 0) * U.curFactor) AS TotalQuantity, SUM(ISNULL(XI.curVAT, 0)) AS CashVAT, SUM(ISNULL(XI.curVATI, 0)) AS CreditVAT FROM Stock_Trans AS T INNER JOIN Stock_Xlink AS X ON T.lngTransaction=X.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink=XI.lngXlink INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType=TT.byteTransType INNER JOIN Stock_Base AS B ON T.byteBase=B.byteBase INNER JOIN Stock_Units AS U ON XI.byteUnit=U.byteUnit WHERE " & Where & " GROUP BY T.lngTransaction, XI.lngXlink;"
        Query = Query & "DECLARE @Stock_OutPatPayments_Query AS TABLE (lngTransaction int, Payment money);"
        Query = Query & "INSERT INTO @Stock_OutPatPayments_Query SELECT TA.lngTransaction, SUM(ISNULL(curValue, 0)) AS Payment FROM Stock_Trans_Amounts AS TA INNER JOIN Stock_Trans AS T ON TA.lngTransaction = T.lngTransaction INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType = TT.byteTransType WHERE TA.byteAmountType=4 AND " & Where & " GROUP BY TA.lngTransaction;"
        Query = Query & "DECLARE @Stock_Values AS TABLE (lngXlink int, byteValueType int, strValueEn varchar(max), strValueAr varchar(max), curValue money, bPercentValue bit, intValueSign int, bytePercentCalculation int, byteAllocationBase int);"
        Query = Query & "INSERT INTO @Stock_Values SELECT Stock_Xlink_Values.lngXlink, Stock_Xlink_Values.byteValueType, Stock_Value_Types.strValueEn, Stock_Value_Types.strValueAr, Stock_Xlink_Values.curValue, Stock_Xlink_Values.bPercentValue, Stock_Value_Types.intValueSign, Stock_Value_Types.bytePercentCalculation, Stock_Value_Types.byteAllocationBase FROM Stock_Xlink_Values INNER JOIN Stock_Value_Types ON Stock_Xlink_Values.byteValueType = Stock_Value_Types.byteValueType;"
        Query = Query & "DECLARE @Stock_Discount AS TABLE (lngTransaction int, lngXlink int, TotalCost money, TotalQuantity money, Discount money, TotalNet money);"
        Query = Query & "INSERT INTO @Stock_Discount SELECT T.lngTransaction, T.lngXlink, T.TotalCost, T.TotalQuantity, SUM(T.TotalCost - CASE WHEN V.byteValueType=1 Then CASE WHEN V.bPercentValue = 1 THEN ISNULL(V.curValue,0) * (T.TotalCost/100) ELSE ISNULL(V.curValue,0) END ELSE 0 END) AS Discount, T.TotalNet FROM @Stock_Total AS T LEFT JOIN @Stock_Values AS V ON T.lngXlink = V.lngXlink GROUP BY T.lngTransaction, T.lngXlink, T.TotalCost, T.TotalQuantity, T.TotalNet;"
        Query = Query & "SELECT T.strTransaction, T.byteTransType, T.lngPatient, RTRIM(LTRIM(ISNULL(P.strFirstEn,'') + ' ') + LTRIM(ISNULL(P.strSecondEn,'') + ' ') + LTRIM(ISNULL(P.strThirdEn ,'') + ' ') + LTRIM(ISNULL(P.strLastEn,''))) AS strPatientEn, RTRIM(LTRIM(ISNULL(P.strFirstAr,'') + ' ') + LTRIM(ISNULL(P.strSecondAr,'') + ' ') + LTRIM(ISNULL(P.strThirdAr ,'') + ' ') + LTRIM(ISNULL(P.strLastAr,''))) AS strPatientAr, C.strContactEn, C.strContactAr, TT.strTypeAr, TT.strTypeEn, T.dateTransaction, T.bCash, T.byteCurrency, T.byteTransType, ABS(Q.TotalCost) AS Total, ABS((Q.TotalCost-Q.Discount)+(Q.TotalCost-Q.TotalNet)) AS curDisc, CASE WHEN T.bCash=1 THEN ABS(Q.TotalCost + ((Q.TotalCost-Q.Discount)+(Q.TotalCost-Q.TotalNet))) ELSE 0 END AS curCash, CASE WHEN T.bCash=0 THEN ABS(Q.TotalCost - ((Q.TotalCost-Q.Discount)+(Q.TotalCost-Q.TotalNet))) ELSE 0 END AS curOutStanding, IsNull(Q1.Payment, 0) AS Payment, T.dateEntry, TA.strCreatedBy FROM Stock_Trans AS T INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType = TT.byteTransType LEFT JOIN @Stock_Discount AS Q ON T.lngTransaction = Q.lngTransaction INNER JOIN Hw_Contacts AS C ON T.lngContact = C.lngContact LEFT JOIN @Stock_OutPatPayments_Query AS Q1 ON T.lngTransaction = Q1.lngTransaction LEFT JOIN Lab_Order_Header AS O ON T.lngTransaction = O.lngTransaction LEFT JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient INNER JOIN Stock_Trans_Audit AS TA ON T.lngTransaction = TA.lngTransaction WHERE " & Where & " ORDER BY T.strTransaction;"
        Return Query
    End Function

    Function buildQuery2_Old(ByVal Where As String) As String
        Dim Query As String = ""
        Query = Query & "DECLARE @Stock_Invoice_Total AS TABLE (lngTransaction int, lngXlink int, curAmount money, curNet money, curQuantity money, curPayment money, curCashVAT money, curCreditVAT money);"
        Query = Query & "INSERT INTO @Stock_Invoice_Total SELECT T.lngTransaction, XI.lngXlink, SUM(B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curBasePrice, 0)) + ISNULL(XI.curVAT, 0) + ISNULL(XI.curVATI, 0))) AS curAmount, SUM(B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curUnitPrice, 0)) + ISNULL(XI.curVAT, 0) + ISNULL(XI.curVATI, 0))) AS curNet, SUM(B.intSign * ISNULL(XI.curQuantity, 0) * U.curFactor) AS curQuantity, SUM(B.intSign * (XI.curCoverage + CASE WHEN T.bCash=0 THEN ISNULL(XI.curVAT, 0) ELSE 0 END)) AS curPayment, SUM(ISNULL(XI.curVAT, 0)) AS curCashVAT, SUM(ISNULL(XI.curVATI, 0)) AS curCreditVAT FROM Stock_Trans AS T INNER JOIN Stock_Xlink AS X ON T.lngTransaction=X.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink=XI.lngXlink INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType=TT.byteTransType INNER JOIN Stock_Base AS B ON T.byteBase=B.byteBase INNER JOIN Stock_Units AS U ON XI.byteUnit=U.byteUnit INNER JOIN Stock_Trans_Audit AS TA ON T.lngTransaction = TA.lngTransaction WHERE " & Where & " GROUP BY T.lngTransaction, XI.lngXlink;"
        Query = Query & "DECLARE @Stock_Invoice_Discount AS TABLE (lngTransaction int, lngXlink int, curValue money);"
        Query = Query & "INSERT INTO @Stock_Invoice_Discount SELECT X.lngTransaction, X.lngXlink, hhh.curValue FROM Stock_Xlink_Values AS hhh INNER JOIN Stock_Xlink AS X ON hhh.lngXlink = X.lngXlink;"
        Query = Query & "SELECT T.strTransaction, T.byteTransType, T.lngPatient, RTRIM(LTRIM(ISNULL(P.strFirstEn,'') + ' ') + LTRIM(ISNULL(P.strSecondEn,'') + ' ') + LTRIM(ISNULL(P.strThirdEn ,'') + ' ') + LTRIM(ISNULL(P.strLastEn,''))) AS strPatientEn, RTRIM(LTRIM(ISNULL(P.strFirstAr,'') + ' ') + LTRIM(ISNULL(P.strSecondAr,'') + ' ') + LTRIM(ISNULL(P.strThirdAr ,'') + ' ') + LTRIM(ISNULL(P.strLastAr,''))) AS strPatientAr, C.strContactEn, C.strContactAr, TT.strTypeAr, TT.strTypeEn, T.dateTransaction, T.bCash, T.byteCurrency, T.byteTransType, ABS(IT.curAmount) AS curAmount, ABS(IT.curAmount-(IT.curNet + ISNULL(curValue,0))) AS curDiscount, IT.curCashVAT + IT.curCreditVAT AS curVAT, CASE WHEN T.bCash=1 THEN ABS(IT.curAmount - (IT.curAmount-(IT.curNet + ISNULL(curValue,0)))) ELSE 0 END AS curCash, CASE WHEN T.bCash=0 THEN  ABS(IT.curAmount - (IT.curAmount-(IT.curNet + ISNULL(curValue,0)))) ELSE 0 END AS curCredit, ABS(ISNULL(IT.curPayment, 0)) AS curPayment, TA.strCreatedBy, T.dateEntry, T.byteStatus FROM Stock_Trans AS T INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType = TT.byteTransType INNER JOIN Hw_Contacts AS C ON T.lngContact = C.lngContact INNER JOIN @Stock_Invoice_Total AS IT ON T.lngTransaction = IT.lngTransaction INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient INNER JOIN Stock_Trans_Audit AS TA ON T.lngTransaction = TA.lngTransaction LEFT JOIN @Stock_Invoice_Discount AS ID ON IT.lngTransaction = ID.lngTransaction WHERE " & Where & ";"
        Return Query
    End Function

    Function buildQuery2(ByVal Where As String) As String
        Dim Query As String = ""
        Query = Query & "DECLARE @Stock_Invoice_Total AS TABLE (lngTransaction int, lngXlink int, curAmount money, curNet money, curQuantity money, curPayment money, curCashVAT money, curCreditVAT money);"
        Query = Query & "INSERT INTO @Stock_Invoice_Total SELECT T.lngTransaction, XI.lngXlink, SUM(-1 * B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curBasePrice, 0)))) AS curAmount, SUM(-1 * B.intSign * ((ISNULL(XI.curQuantity, 0) * ISNULL(XI.curUnitPrice, 0)))) AS curNet, SUM(-1 * B.intSign * ISNULL(XI.curQuantity, 0) * U.curFactor) AS curQuantity, SUM(-1 * B.intSign * (XI.curCoverage + CASE WHEN T.bCash=0 THEN ISNULL(XI.curVAT, 0) ELSE 0 END)) AS curPayment, SUM(ISNULL(XI.curVAT, 0)) AS curCashVAT, SUM(ISNULL(XI.curVATI, 0)) AS curCreditVAT FROM Stock_Trans AS T INNER JOIN Stock_Xlink AS X ON T.lngTransaction=X.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink=XI.lngXlink INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType=TT.byteTransType INNER JOIN Stock_Base AS B ON T.byteBase=B.byteBase INNER JOIN Stock_Units AS U ON XI.byteUnit=U.byteUnit INNER JOIN Stock_Trans_Audit AS TA ON T.lngTransaction = TA.lngTransaction WHERE " & Where & " GROUP BY T.lngTransaction, XI.lngXlink;"
        Query = Query & "DECLARE @Stock_Invoice_Discount AS TABLE (lngTransaction int, lngXlink int, curValue money);"
        Query = Query & "INSERT INTO @Stock_Invoice_Discount SELECT X.lngTransaction, X.lngXlink, hhh.curValue FROM Stock_Xlink_Values AS hhh INNER JOIN Stock_Xlink AS X ON hhh.lngXlink = X.lngXlink;"
        Query = Query & "SELECT T.strTransaction, T.byteTransType, T.lngPatient, RTRIM(LTRIM(ISNULL(P.strFirstEn,'') + ' ') + LTRIM(ISNULL(P.strSecondEn,'') + ' ') + LTRIM(ISNULL(P.strThirdEn ,'') + ' ') + LTRIM(ISNULL(P.strLastEn,''))) AS strPatientEn, RTRIM(LTRIM(ISNULL(P.strFirstAr,'') + ' ') + LTRIM(ISNULL(P.strSecondAr,'') + ' ') + LTRIM(ISNULL(P.strThirdAr ,'') + ' ') + LTRIM(ISNULL(P.strLastAr,''))) AS strPatientAr, C.strContactEn, C.strContactAr, TT.strTypeAr, TT.strTypeEn, T.dateTransaction, T.bCash, T.byteCurrency, T.byteTransType, IT.curAmount AS curAmount, IT.curAmount-(IT.curNet + ISNULL(curValue,0)) AS curDiscount, IT.curCashVAT + IT.curCreditVAT AS curVAT, CASE WHEN T.bCash=1 THEN IT.curAmount - (IT.curAmount-(IT.curNet + ISNULL(curValue,0))) + IT.curCashVAT ELSE 0 END AS curCash, CASE WHEN T.bCash=0 THEN  IT.curAmount - (IT.curAmount-(IT.curNet + ISNULL(curValue,0))) + IT.curCreditVAT ELSE 0 END AS curCredit, ISNULL(IT.curPayment, 0) AS curPayment, TA.strCreatedBy, T.dateEntry, T.byteStatus FROM Stock_Trans AS T INNER JOIN Stock_Trans_Types AS TT ON T.byteTransType = TT.byteTransType INNER JOIN Hw_Contacts AS C ON T.lngContact = C.lngContact INNER JOIN @Stock_Invoice_Total AS IT ON T.lngTransaction = IT.lngTransaction INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient INNER JOIN Stock_Trans_Audit AS TA ON T.lngTransaction = TA.lngTransaction LEFT JOIN @Stock_Invoice_Discount AS ID ON IT.lngTransaction = ID.lngTransaction WHERE " & Where & ";"
        Return Query
    End Function

    Sub loadLanguage()
        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                Title = "تقرير |  الفواتير"
                'Variables
                btnSearch = "بحث"
                Both = "الكل"
                PaymentType = "طريقة الدفع"
                dte = "التاريخ بين"
                InvType = "نوع الفاتوره"
                byUser = "حسب المستخدم"
                byDoctor = "حسب الدكتور"
                byInvType = "حسب نوع الفاتوره"
                doct = "الدكتور"
                Paid = "مدفوعة"
                Cancelled = "ملغاة"
                'Columns
                colInvoiceNo = "الرقم"
                colDate = "التاريخ"
                colTime = "الوقت"
                colUser = "المستخدم"
                colPatientNo = "الملف"
                colPatientName = "المريض"
                colCompany = "الشركة"
                colAmount = "المبلغ"
                colDiscount = "الخصم"
                colVAT = "الضريبة"
                colCash = "النقد"
                colCredit = "الآجل"
                colPayment = "الدفعات"
                colStatus = "الحالة"
            Case Else
                DataLang = "En"
                Title = "Report | Invoices"
                'Variables
                btnSearch = "Search"
                Both = "Both"
                PaymentType = "Payment Type"
                dte = "Date between"
                InvType = "Invoice Type"
                byUser = "By User"
                byDoctor = "By Doctor"
                byInvType = "By Invoice Type"
                doct = "Doctor"
                Paid = "Paid"
                Cancelled = "Cancelled"
                'Columns
                colInvoiceNo = "No"
                colDate = "Date"
                colTime = "Time"
                colUser = "User"
                colPatientNo = "File"
                colPatientName = "Patient Name"
                colCompany = "Company"
                colAmount = "Amount"
                colDiscount = "Discount"
                colVAT = "VAT"
                colCash = "Cash"
                colCredit = "Credit"
                colPayment = "Payments"
                colStatus = "Status"
        End Select
    End Sub


End Class