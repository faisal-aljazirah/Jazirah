Public Class ajax2
    Inherits System.Web.UI.Page

    <System.Web.Services.WebMethod()>
    Public Shared Function getBalance(ByVal strItem As String, ByVal dateTransaction As Date, ByVal byteWarehouse As Byte, ByVal Factor As Integer, ByVal Quantity As Integer) As String
        Dim ST As New Stock.Stock
        Return ST.getBalance(strItem, dateTransaction, byteWarehouse, Factor, Quantity)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function getBalance(ByVal strItem As String, ByVal dateTransaction As Date, ByVal byteWarehouse As Byte) As String
        Dim ST As New Stock.Stock
        Return ST.getBalance(strItem, dateTransaction, byteWarehouse)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function viewBalance(ByVal strItem As String) As String
        Dim ST As New Stock.Stock
        Return ST.viewBalance(strItem)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function fillItems(ByVal strItem As String, ByVal intGroup As Integer, ByVal intAvailable As Byte) As String
        Dim ST As New Stock.Stock
        Return ST.fillItems(strItem, intGroup, intAvailable)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function fillTransactions(ByVal strItem As String, ByVal byteWarehouse As Byte, ByVal dateExpiry As Date) As String
        Dim ST As New Stock.Stock
        Return ST.fillTransactions(strItem, byteWarehouse, dateExpiry)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function fillTracking(ByVal strItem As String, ByVal byteWarehouse As Byte, ByVal dateExpiry As Date) As String
        Dim ST As New Stock.Stock
        Return ST.fillTracking(strItem, byteWarehouse, dateExpiry)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function viewItem(ByVal strItem As String) As String
        Dim ST As New Stock.Stock
        Return ST.viewItem(strItem)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function saveItem(ByVal strItem As String, ByVal Fields As String) As String
        Dim ST As New Stock.Stock
        Return ST.saveItem(strItem, Fields)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function fillTransfer(ByVal dateFrom As Date, ByVal dateTo As Date, ByVal byteWarehouse As Byte) As String
        Dim ST As New Stock.Stock
        Return ST.fillTransfer(dateFrom, dateTo, byteWarehouse)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function viewTransfer(ByVal lngTransaction As Long, ByVal ReceiveMode As Boolean) As String
        Dim ST As New Stock.Stock
        Return ST.viewTransfer(lngTransaction, ReceiveMode)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function getItemInfo(ByVal strBarcode As String, ByVal byteWarehouse As Byte, ByVal curQuantity As Decimal) As String
        Dim ST As New Stock.Stock
        Return ST.getItemInfo(strBarcode, byteWarehouse, curQuantity)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function saveTransfer(ByVal lngTransaction As Long, ByVal Fields As String) As String
        Dim ST As New Stock.Stock
        Return ST.saveTransfer(lngTransaction, Fields)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function fillRequests(ByVal dateFrom As Date, ByVal dateTo As Date, ByVal byteWarehouse As Byte) As String
        Dim ST As New Stock.Stock
        Return ST.fillRequests(dateFrom, dateTo, byteWarehouse)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function approveSendItems(ByVal lngTransaction As String, ByVal modal As String) As String
        Dim ST As New Stock.Stock
        Return ST.approveSendItems(lngTransaction, modal)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function approveReceiveItems(ByVal lngTransaction As String, ByVal modal As String) As String
        Dim ST As New Stock.Stock
        Return ST.approveReceiveItems(lngTransaction, modal)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function fillReturns(ByVal dateInvoice As Date) As String
        Dim PH As New Pharmacy.Invoices
        Return PH.fillInvoices(dateInvoice, 3, "")
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function fillSExpired(ByVal dateFrom As Date, ByVal dateTo As Date, ByVal byteWarehouse As Byte) As String
        Dim ST As New Stock.Stock
        Return ST.fillSExpired(dateFrom, dateTo, byteWarehouse)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function viewSExpired(ByVal lngTransaction As Long, ByVal IsOut As Boolean) As String
        Dim ST As New Stock.Stock
        Return ST.viewSExpired(lngTransaction, IsOut)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function saveTransferExpired(ByVal lngTransaction As Long, ByVal Fields As String) As String
        Dim ST As New Stock.Stock
        Return ST.saveTransferExpired(lngTransaction, Fields)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function approveItems(ByVal lngTransaction As String, ByVal modal As String) As String
        Dim ST As New Stock.Stock
        Return ST.approveItems(lngTransaction, modal)

    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function fillSReturned(ByVal dateFrom As Date, ByVal dateTo As Date, ByVal byteWarehouse As Byte) As String
        Dim ST As New Stock.Stock
        Return ST.fillSReturned(dateFrom, dateTo, byteWarehouse)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function viewSReturned(ByVal lngTransaction As Long, ByVal IsOut As Boolean) As String
        Dim ST As New Stock.Stock
        Return ST.viewSReturned(lngTransaction, IsOut)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function saveTransferReturned(ByVal lngTransaction As Long, ByVal Fields As String) As String
        Dim ST As New Stock.Stock
        Return ST.saveTransferReturned(lngTransaction, Fields)
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
    Public Shared Function completeBarcode(ByVal strBarcode As String, ByVal byteWarehouse As Byte, ByVal byteBase As Byte, ByVal strFunction As String) As String
        Dim ST As New Stock.Stock
        Return ST.completeBarcode(strBarcode, byteWarehouse, byteBase, strFunction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function getQuantity(ByVal strBarcode As String, ByVal strFunction As String) As String
        Dim ST As New Stock.Stock
        Return ST.getQuantity(strBarcode, strFunction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function changeLimit() As String
        Dim ST As New Stock.Stock
        Return ST.changeLimit()
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function changeLimitItems(ByVal Fields As String) As String
        Dim ST As New Stock.Stock
        Return ST.changeLimitItems(Fields)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function changeTax() As String
        Dim ST As New Stock.Stock
        Return ST.changeTax()
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function changeTaxItems(ByVal Fields As String) As String
        Dim ST As New Stock.Stock
        Return ST.changeTaxItems(Fields)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function changeAvailablity() As String
        Dim ST As New Stock.Stock
        Return ST.changeAvailablity()
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function changeAvailablityItems(ByVal Fields As String) As String
        Dim ST As New Stock.Stock
        Return ST.changeAvailablityItems(Fields)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ViewCollectItems(ByVal byteWarehouse As Byte, ByVal CheckBalance As Boolean) As String
        Dim ST As New Stock.Stock
        Return ST.ViewCollectItems(byteWarehouse, CheckBalance)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function CollectItems(ByVal byteWarehouse As Byte, ByVal Fields As String, ByVal CheckBalance As Boolean) As String
        Dim ST As New Stock.Stock
        Return ST.CollectItems(byteWarehouse, Fields, CheckBalance)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function cancelTransfer(ByVal lngTransaction As Long) As String
        Dim ST As New Stock.Stock
        Return ST.cancelTransfer(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function returnTransferedItems(ByVal lngTransaction As Long) As String
        Dim ST As New Stock.Stock
        Return ST.returnTransferedItems(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function fillExpiredItems(ByVal strItem As String, ByVal dateExpiry As Date, ByVal byteWarehouse As Byte) As String
        Dim ST As New Stock.Stock
        Return ST.fillExpiredItems(strItem, dateExpiry, byteWarehouse)
    End Function
End Class