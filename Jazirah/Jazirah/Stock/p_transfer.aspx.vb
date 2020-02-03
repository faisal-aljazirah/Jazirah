Public Class p_transfer
    Inherits System.Web.UI.Page
    Dim dcl As New DCL.Conn.DataClassLayer
    Public DataLang As String
    Public table As String = ""
    Public TotalCost, TotalPrice As Decimal
    Public PrintScript As String
    Public FromWarehouse, ToWarehouse, VoucherNo, VoucherDate As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        DataLang = "En"

        Dim lngTransaction As Long = Request.QueryString("t")
        Dim ds, dsTrans As DataSet
        dsTrans = dcl.GetDS("SELECT T1.lngTransaction, T1.strTransaction AS strSendVoucher, T2.strTransaction AS strReceiveVoucher, T1.dateTransaction AS dateSendDate, T2.dateTransaction AS dateReceiveDate, TT1.strType" & DataLang & " AS strSendType, TT2.strType" & DataLang & " AS strReceiveType, W1.byteWarehouse AS byteSendWarehouse, W1.strWarehouse" & DataLang & " AS strSendWarehouse, W2.byteWarehouse AS byteReceiveWarehouse, W2.strWarehouse" & DataLang & " AS strReceiveWarehouse, TA1.strCreatedBy AS strSendUser, TA2.strCreatedBy AS strReceiveUser, T1.byteStatus AS byteSendStatus, T2.byteStatus AS byteReceiveStatus, T1.strRemarks FROM Stock_Xlink AS X1 LEFT JOIN Stock_Xlink AS X2 ON X1.lngPointer=X2.lngPointer INNER JOIN Stock_Trans AS T1 ON T1.lngTransaction=X1.lngTransaction INNER JOIN Stock_Trans AS T2 ON T2.lngTransaction=X2.lngTransaction INNER JOIN Stock_Warehouses AS W1 ON T1.byteWarehouse=W1.byteWarehouse INNER JOIN Stock_Warehouses AS W2 ON T2.byteWarehouse=W2.byteWarehouse INNER JOIN Stock_Trans_Types AS TT1 ON T1.byteTransType=TT1.byteTransType INNER JOIN Stock_Trans_Types AS TT2 ON T2.byteTransType=TT2.byteTransType INNER JOIN Stock_Trans_Audit AS TA1 ON T1.lngTransaction=TA1.lngTransaction LEFT JOIN Stock_Trans_Audit AS TA2 ON T2.lngTransaction=TA2.lngTransaction WHERE T1.byteBase=19 AND T2.byteBase=20 AND T1.lngTransaction=" & lngTransaction)
        FromWarehouse = dsTrans.Tables(0).Rows(0).Item("strSendWarehouse")
        ToWarehouse = dsTrans.Tables(0).Rows(0).Item("strReceiveWarehouse")
        VoucherNo = dsTrans.Tables(0).Rows(0).Item("strSendVoucher")
        VoucherDate = CDate(dsTrans.Tables(0).Rows(0).Item("dateSendDate")).ToString("yyyy-MM-dd")

        If Request.QueryString("ap") = "" Then PrintScript = "<script type=""text/javascript"">setTimeout(function () {window.print();window.close();}, 1000);</script>" Else PrintScript = ""

        ds = dcl.GetDS("SELECT I.strItem,* FROM Stock_Trans AS T INNER JOIN Stock_Xlink AS X ON T.lngTransaction=X.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON XI.lngXlink=X.lngXlink INNER JOIN Stock_Units AS U ON U.byteUnit = XI.byteUnit INNER JOIN Stock_Items AS I ON I.strItem=XI.strItem WHERE T.lngTransaction = " & lngTransaction & " ORDER BY strItemEn")
        For I = 0 To ds.Tables(0).Rows.Count - 1
            table = table & "<tr>"
            table = table & "<td>" & ds.Tables(0).Rows(I).Item("intEntryNumber") & "</td>"
            table = table & "<td>" & ds.Tables(0).Rows(I).Item("strItem") & "</td>"
            table = table & "<td>" & ds.Tables(0).Rows(I).Item("strItemEn") & "</td>"
            table = table & "<td>" & CDate(ds.Tables(0).Rows(I).Item("dateExpiry")).ToString("yyyy-MM-dd") & "</td>"
            table = table & "<td>" & ds.Tables(0).Rows(I).Item("strUnitEn") & "</td>"
            table = table & "<td>" & Math.Round(ds.Tables(0).Rows(I).Item("curQuantity"), 0, MidpointRounding.AwayFromZero) & "</td>"
            table = table & "<td>" & Math.Round(ds.Tables(0).Rows(I).Item("curUnitCost"), 2, MidpointRounding.AwayFromZero) & "</td>"
            table = table & "<td>" & Math.Round(ds.Tables(0).Rows(I).Item("curUnitCost") * ds.Tables(0).Rows(I).Item("curQuantity"), 2, MidpointRounding.AwayFromZero) & "</td>"
            table = table & "<td>" & Math.Round(ds.Tables(0).Rows(I).Item("curUnitPrice"), 2, MidpointRounding.AwayFromZero) & "</td>"
            table = table & "<td>" & Math.Round(ds.Tables(0).Rows(I).Item("curUnitPrice") * ds.Tables(0).Rows(I).Item("curQuantity"), 2, MidpointRounding.AwayFromZero) & "</td>"
            table = table & "</tr>"
            TotalCost = TotalCost + (ds.Tables(0).Rows(I).Item("curQuantity") * ds.Tables(0).Rows(I).Item("curUnitCost"))
            TotalPrice = TotalPrice + (ds.Tables(0).Rows(I).Item("curQuantity") * ds.Tables(0).Rows(I).Item("curUnitPrice"))
        Next
        TotalCost = Math.Round(TotalCost, 2, MidpointRounding.AwayFromZero)
        TotalPrice = Math.Round(TotalPrice, 2, MidpointRounding.AwayFromZero)
    End Sub

End Class