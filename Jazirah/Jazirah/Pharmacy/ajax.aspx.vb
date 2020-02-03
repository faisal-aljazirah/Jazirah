Public Class ajax1
    Inherits System.Web.UI.Page
    Dim dcl As New DCL.Conn.DataClassLayer
    Public DataLang As String

    Const byteLocalCurrency As Byte = 3
    Const intStartupFY As Integer = 2017
    Const byteDepartment As Byte = 15
    Const byteCurrencyRound As Byte = 2

    Public byteLanguage As Byte
    Public strUserName, strDateFormat, strTimeFormat As String

    Private Sub form1_Init(sender As Object, e As EventArgs) Handles form1.Init
        ' User Options
        If (HttpContext.Current.Session("UserName") Is Nothing) Or (HttpContext.Current.Session("UserName") = "") Then
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

    <System.Web.Services.WebMethod()>
    Public Shared Function viewInfo(ByVal lngTransaction As Long) As String
        Dim PH As New Pharmacy.Extra
        Return PH.viewInfo(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function viewCoverage(ByVal lngTransaction As Long) As String
        Dim PH As New Pharmacy.Extra
        Return PH.viewCoverage(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function viewPatient(ByVal lngTransaction As Long) As String
        Dim PH As New Pharmacy.Extra
        Return PH.viewPatient(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function viewCashbox() As String
        Dim PH As New Pharmacy.Cashier
        Return PH.viewCashbox()
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function changeInvoiceType(ByVal TransNo As Long) As String
        Dim PH As New Pharmacy.Extra
        Return PH.changeInvoiceType(TransNo)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function createOrder() As String
        Dim PH As New Pharmacy.Orders
        Return PH.createOrder()
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function createCreditOrder(ByVal lngContact As Long, ByVal lngPatient As Long) As String
        Dim PH As New Pharmacy.Orders
        Return PH.createCreditOrder(lngContact, lngPatient)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function viewOrder(ByVal TransNo As Long, ByVal ShowOnly As Boolean, ByVal ToPrepare As Boolean) As String
        Dim PH As New Pharmacy.Orders
        Return PH.viewOrder(TransNo, ShowOnly, ToPrepare)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function returnOrder(ByVal TransNo As Long) As String
        Dim PH As New Pharmacy.Orders
        Return PH.returnOrder(TransNo)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function viewInvoice(ByVal TransNo As Long) As String
        Dim PH As New Pharmacy.Invoices
        Return PH.viewInvoice(TransNo)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function viewVoucher(ByVal TransNo As Long) As String
        Dim PH As New Pharmacy.Invoices
        Return PH.viewVoucher(TransNo)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function prepareOrder(ByVal TransNo As Long) As String
        Dim PH As New Pharmacy.Orders
        Return PH.prepareOrder(TransNo)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function SendToCashier(ByVal Fields As String) As String
        Dim PH As New Pharmacy.Cashier
        Return PH.SendToCashier(Fields)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ReturnToSales(ByVal lngTransaction As Long) As String
        Dim PH As New Pharmacy.Cashier
        Return PH.ReturnToSales(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function cancelInvoice(ByVal lngTransaction As Long) As String
        Dim PH As New Pharmacy.Invoices
        Return PH.cancelInvoice(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function returnItems(ByVal lngTransaction As Long, ByVal lstItems As String) As String
        Dim PH As New Pharmacy.Invoices
        Return PH.returnItems(lngTransaction, lstItems)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function reopenInvoice(ByVal lngTransaction As Long) As String
        Dim PH As New Pharmacy.Invoices
        Return PH.reopenInvoice(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function requestCancelInvoice(ByVal lngTransaction As Long) As String
        Dim PH As New Pharmacy.Extra
        Return PH.requestCancelInvoice(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function requestReturnItems(ByVal lngTransaction As Long, ByVal lstItems As String) As String
        Dim PH As New Pharmacy.Extra
        Return PH.requestReturnItems(lngTransaction, lstItems)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function requestReopenInvoice(ByVal lngTransaction As Long) As String
        Dim PH As New Pharmacy.Extra
        Return PH.requestReopenInvoice(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function rejectCancelRequest(ByVal lngTransaction As Long) As String
        Dim PH As New Pharmacy.Extra
        Return PH.rejectCancelRequest(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function rejectReturnRequest(ByVal lngTransaction As Long) As String
        Dim PH As New Pharmacy.Extra
        Return PH.rejectReturnRequest(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function rejectReopenRequest(ByVal lngTransaction As Long) As String
        Dim PH As New Pharmacy.Extra
        Return PH.rejectReopenRequest(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function approveCancelRequest(ByVal lngTransaction As Long) As String
        Dim PH As New Pharmacy.Extra
        Return PH.approveCancelRequest(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function approveReturnRequest(ByVal lngTransaction As Long) As String
        Dim PH As New Pharmacy.Extra
        Return PH.approveReturnRequest(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function approveReopenRequest(ByVal lngTransaction As Long) As String
        Dim PH As New Pharmacy.Extra
        Return PH.approveReopenRequest(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function viewCashier1(ByVal TabCounter As Integer, ByVal Fields As String) As String
        Dim PH As New Pharmacy.Cashier
        Return PH.viewCashier(TabCounter, Fields)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function viewCashier2(ByVal lngTransaction As Long) As String
        Dim PH As New Pharmacy.Cashier
        Return PH.viewCashier(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function getPaid1(ByVal lngTransaction As Long, ByVal P_Cash As Decimal, ByVal P_SPAN As Decimal, ByVal PaymentType As Byte) As String
        Dim PH As New Pharmacy.Cashier
        Return PH.GetPaid(lngTransaction, P_Cash, P_SPAN, PaymentType)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function getPaid2(ByVal TabCounter As Integer, ByVal Fields As String, ByVal P_Cash As Decimal, ByVal P_SPAN As Decimal, ByVal PaymentType As Byte) As String
        Dim PH As New Pharmacy.Cashier
        Return PH.GetPaid(TabCounter, Fields, P_Cash, P_SPAN, PaymentType)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function fillPrepare() As String
        Dim ph As New Pharmacy.Orders
        Return ph.fillPrepare()
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function fillCheckout(ByVal byteDepartment As Byte, ByVal lngSalesman As Long, ByVal strSearch As String) As String
        Dim ph As New Pharmacy.Orders
        Return ph.fillCheckout(byteDepartment, lngSalesman, strSearch)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function fillOrders(ByVal byteDepartment As Byte, ByVal lngSalesman As Long, ByVal strSearch As String, ByVal ToPrepare As Boolean) As String
        Dim ph As New Pharmacy.Orders
        Return ph.fillOrders(byteDepartment, lngSalesman, strSearch, ToPrepare)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function fillOldOrders(ByVal byteDepartment As Byte, ByVal lngSalesman As Long, ByVal strSearch As String, ByVal ToPrepare As Boolean) As String
        Dim ph As New Pharmacy.Orders
        Return ph.fillOldOrders(byteDepartment, lngSalesman, strSearch, ToPrepare)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function sendToPrapare(ByVal lngTransaction As Long) As String
        Dim ph As New Pharmacy.Orders
        Return ph.sendToPrapare(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function returnToOrder(ByVal lngTransaction As Long) As String
        Dim ph As New Pharmacy.Orders
        Return ph.returnToOrder(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function takeOrder(ByVal lngTransaction As Long, ByVal strUserName As String) As String
        Dim ph As New Pharmacy.Orders
        Return ph.takeOrder(lngTransaction, strUserName)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function viewReturnAmount(ByVal lngTransaction As Long, ByVal lstItems As String, ByVal IsCancel As Boolean, ByVal NextFunction As String) As String
        Dim ph As New Pharmacy.Invoices
        Return ph.viewReturnAmount(lngTransaction, lstItems, IsCancel, NextFunction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function fillInvoices(ByVal dateInvoice As Date, ByVal byteStatus As Byte, ByVal strSearch As String) As String
        Dim ph As New Pharmacy.Invoices
        Return ph.fillInvoices(dateInvoice, byteStatus, strSearch)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function getItemInfo(ByVal strBarcode As String, ByVal lngTransaction As Long, ByVal curCoverage As Decimal, ByVal curBasePriceTotal As Decimal, ByVal RowCounter As Byte, ByVal SelectedInsuranceItems As String, ByVal SelectedCashItems As String, ByVal ItemType As Byte, ByVal Quantity As Decimal) As String
        Dim ph As New Pharmacy.Orders
        Return ph.getItemInfo(strBarcode, lngTransaction, curCoverage, curBasePriceTotal, RowCounter, SelectedInsuranceItems, SelectedCashItems, ItemType, Quantity)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function completeBarcode(ByVal strBarcode As String, ByVal byteWarehouse As Byte, ByVal byteBase As Byte, ByVal strFunction As String) As String
        'Dim ph As New Pharmacy.Orders
        'Return ph.completeBarcode(strBarcode, byteWarehouse, byteBase, strFunction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function getQuantity(ByVal strBarcode As String) As String
        Dim ph As New Pharmacy.Orders
        Return ph.getQuantity(strBarcode)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function SuspendInvoice(ByVal lngTransaction As Long, ByVal CashOnly As Integer, ByVal InsuranceItems As String, ByVal CashItems As String) As String
        Dim ph As New Pharmacy.Orders
        Return ph.SuspendInvoice(lngTransaction, CashOnly, InsuranceItems, CashItems)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function UnsuspendInvoice(ByVal lngTransaction As Long, ByVal CashOnly As Integer, ByVal InsuranceItems As String, ByVal CashItems As String) As String
        Dim ph As New Pharmacy.Orders
        Return ph.UnsuspendInvoice(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function findItem(ByVal query As String) As String
        Dim str As New StringBuilder("")
        Dim filter As String
        Dim s_PadLetter As String = ""
        Dim s_Padding As Integer = 0
        Dim s_SerialID As Boolean = True
        If filter <> "" Then filter = " AND " & filter
        str.Append("{""suggestions"": [ ")
        Dim myds As New DataSet
        Dim dcl As New DCL.Conn.DataClassLayer
        Dim DataLang As String = "En"
        myds = dcl.GetDS("SELECT TOP 5 * FROM Stock_Items WHERE strItem" & DataLang & " LIKE '%" & query & "%'" & filter)
        For I = 0 To myds.Tables(0).Rows.Count - 1
            str.Append("{ ""value"": """ & myds.Tables(0).Rows(I).Item("strItem" & DataLang).ToString & """, ""id"": """ & myds.Tables(0).Rows(I).Item("strItem").ToString & """ },")
        Next
        str.Remove(str.Length - 1, 1)
        str.Append(" ]}")
        Return str.ToString
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function filterReport(ByVal Source As String, ByVal Filter As String) As String
        Dim ph As New Pharmacy.Reports
        Return ph.filterReport(Source, Filter)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function fillSalesReport(ByVal Filter As String) As String
        Dim ph As New Pharmacy.Reports
        Return ph.fillSalesReport(Filter)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function getSalesReport(ByVal Report As String) As String
        Dim ph As New Pharmacy.Reports
        Return ph.getSalesReport(Report)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function fillCashReport(ByVal Filter As String) As String
        Dim ph As New Pharmacy.Reports
        Return ph.fillCashReport(Filter)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function drowChart(ByVal ElementID As String, ByVal Type As String) As String
        Dim sh As New Share.UI
        Return sh.drowChart(ElementID, Type)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function getCreditPatients(ByVal lngContact As Long) As String
        Dim sh As New Pharmacy.Orders
        Return sh.getCreditPatients(lngContact)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function fillReportButtons() As String
        Dim sh As New Pharmacy.Reports
        Return sh.fillReportButtons()
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function generateReport(ByVal Func As String, ByVal Options As String) As String
        Dim sh As New Pharmacy.Reports
        Return sh.generateReport(Func, Options)
    End Function

End Class