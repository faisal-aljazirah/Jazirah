Public Class ajax1
    Inherits System.Web.UI.Page

    Const byteLocalCurrency As Byte = 3
    Const strUserName As String = "SoftNet"
    Const intStartupFY As Integer = 2017
    Const byteDepartment As Byte = 15
    Const byteCurrencyRound As Byte = 2

    <System.Web.Services.WebMethod()>
    Public Shared Function getTable() As String
        Return "{""data"":[{""Invoice No"":1,""Patient Name"":""Faisal"",""Doctor Name"":""Me"",""Invoice Date"": ""2019-07-28""}]}"
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function viewInfo(ByVal lngTransaction As Long) As String
        Dim PH As New Pharmacy.Orders
        Return PH.viewInfo(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function viewOrder(ByVal TransNo As Long) As String
        Dim PH As New Pharmacy.Orders
        Return PH.viewOrder(TransNo)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function viewInvoice(ByVal TransNo As Long) As String
        Dim PH As New Pharmacy.Orders
        Return PH.viewInvoice(TransNo)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function viewVoucher(ByVal TransNo As Long) As String
        Dim PH As New Pharmacy.Orders
        Return PH.viewVoucher(TransNo)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function prepareOrder(ByVal TransNo As Long) As String
        Dim PH As New Pharmacy.Orders
        Return PH.prepareOrder(TransNo)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function SendToCashier(ByVal Fields As String) As String
        Dim PH As New Pharmacy.Orders
        Return PH.SendToCashier(Fields)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ReturnToSales(ByVal lngTransaction As Long) As String
        Dim PH As New Pharmacy.Orders
        Return PH.ReturnToSales(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function cancelInvoice(ByVal lngTransaction As Long) As String
        Dim PH As New Pharmacy.Orders
        Return PH.cancelInvoice(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function returnItems(ByVal lngTransaction As Long, ByVal lstItems As String) As String
        Dim PH As New Pharmacy.Orders
        Return PH.returnItems(lngTransaction, lstItems)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function requestCancelInvoice(ByVal lngTransaction As Long) As String
        Dim PH As New Pharmacy.Orders
        Return PH.requestCancelInvoice(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function requestReturnItems(ByVal lngTransaction As Long, ByVal lstItems As String) As String
        Dim PH As New Pharmacy.Orders
        Return PH.requestReturnItems(lngTransaction, lstItems)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function rejectCancelRequest(ByVal lngTransaction As Long) As String
        Dim PH As New Pharmacy.Orders
        Return PH.rejectCancelRequest(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function rejectReturnRequest(ByVal lngTransaction As Long) As String
        Dim PH As New Pharmacy.Orders
        Return PH.rejectReturnRequest(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function approveCancelRequest(ByVal lngTransaction As Long) As String
        Dim PH As New Pharmacy.Orders
        Return PH.approveCancelRequest(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function approveReturnRequest(ByVal lngTransaction As Long) As String
        Dim PH As New Pharmacy.Orders
        Return PH.approveReturnRequest(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function viewCashier1(ByVal TabCounter As Integer, ByVal Fields As String) As String
        Dim PH As New Pharmacy.Orders
        Return PH.viewCashier(TabCounter, Fields)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function viewCashier2(ByVal lngTransaction As Long) As String
        Dim PH As New Pharmacy.Orders
        Return PH.viewCashier(lngTransaction)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function getPaid1(ByVal lngTransaction As Long, ByVal P_Cash As Decimal, ByVal P_SPAN As Decimal, ByVal PaymentType As Byte) As String
        Dim PH As New Pharmacy.Orders
        Return PH.GetPaid(lngTransaction, P_Cash, P_SPAN, PaymentType)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function getPaid2(ByVal TabCounter As Integer, ByVal Fields As String, ByVal P_Cash As Decimal, ByVal P_SPAN As Decimal, ByVal PaymentType As Byte) As String
        Dim PH As New Pharmacy.Orders
        Return PH.GetPaid(TabCounter, Fields, P_Cash, P_SPAN, PaymentType)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function fillOrders() As String
        Dim ph As New Pharmacy.Orders
        Return ph.fillOrders()
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function getItemInfo(ByVal strBarcode As String, ByVal lngTransaction As Long, ByVal curCoverage As Decimal, ByVal curBasePriceTotal As Decimal, ByVal RowCounter As Byte, ByVal SelectedInsuranceItems As String, ByVal SelectedCashItems As String) As String
        Dim ph As New Pharmacy.Orders
        Return ph.getItemInfo(strBarcode, lngTransaction, curCoverage, curBasePriceTotal, RowCounter, SelectedInsuranceItems, SelectedCashItems)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function completeBarcode(ByVal strBarcode As String) As String
        Dim ph As New Pharmacy.Orders
        Return ph.completeBarcode(strBarcode)
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

    Function getItemInfo2(ByVal strBarcode As String, ByVal lngPatient As Long, ByVal dateTransaction As Date, ByVal strReference As String) As String
        Dim intVisit As Long
        'Dim strReference As String = "1786"
        'Dim dateTransaction As Date = CDate("2019-01-02")
        'Dim lngPatient As Long = 745296
        'Dim strBarcode As String = "10191"
        Dim Approved, Rejected As Boolean
        Dim dcl As New DCL.Conn.DataClassLayer
        Dim ds As DataSet
        Dim ret As String = ""
        Dim strItem As String
        Dim curBasePrice As String
        Dim dateExpiry As String
        Dim bConvertedCash As Boolean
        ' =>

        ' => Get intVisit
        ds = dcl.GetDS("SELECT DISTINCT intVisit FROM Hw_Treatments_Pharmacy WHERE strReference='" & strReference & "' AND dateTransaction='" & dateTransaction.ToString("yyyy-MM-dd") & "'")
        If ds.Tables(0).Rows.Count > 0 Then
            intVisit = ds.Tables(0).Rows(0).Item("intVisit")
            ' => Get Approvals
            ds = dcl.GetDS("SELECT * FROM Hw_Medicines_Approval Where intVisit=" & intVisit & " AND lngPatient=" & lngPatient & " AND strItem='" & Left(strBarcode, 5) & "'")
            ' => Check Status (Approved  or Rejected)
            If ds.Tables(0).Rows.Count > 0 Then
                If Not (IsDBNull(ds.Tables(0).Rows(0).Item("strRejectedBy")) And IsDBNull(ds.Tables(0).Rows(0).Item("strApprovedBy"))) Then
                    'TODO: assign here if rejected or approved
                    If ds.Tables(0).Rows(0).Item("strApprovedBy").ToString <> "" Then
                        Approved = True
                    Else
                        If ds.Tables(0).Rows(0).Item("strRejectedBy") = True Then
                            Rejected = True
                            'CheckItemCoverage()
                        Else
                            Approved = False
                        End If
                    End If
                    '-------------------
                    If strBarcode <> "" Then
                        Select Case strBarcode.Length
                            Case Is < 12
                                strItem = Mid(strBarcode, 1, 8)
                                ds = dcl.GetDS("SELECT * FROM Stock_Items WHERE strItem='" & strItem & "'")
                                If ds.Tables(0).Rows.Count > 0 Then
                                    'strItem_AfterUpdate()
                                    curBasePrice = Mid(strBarcode, 9, 4) & "." & Mid([strBarcode], 13, 2)
                                    'dateExpiry = DateSerial(Mid(strBarcode, 17, 2), Mid(strBarcode, 15, 2), 1)
                                    ret = "OK => Less"
                                Else
                                    'MsgBox(IIf(gblanguage, "ÇáÕäÝ ÛíÑ ãÚÑÝ.", "Item not defined."))
                                    strItem = Nothing
                                    ret = "Err:item not defined"
                                End If
                            Case 12
                                strItem = Left(strBarcode, 8)
                                ds = dcl.GetDS("SELECT * FROM Stock_Item_Info WHERE strOldReference='" & strItem & "'")
                                If ds.Tables(0).Rows.Count > 0 Then
                                    'strItem_AfterUpdate()
                                    'dateExpiry = DateSerial(Mid(strBarcode, 12, 1), Mid(strBarcode, 10, 2), 1)
                                    ret = "OK => 12"
                                Else
                                    'MsgBox(IIf(gblanguage, "ÇáÕäÝ ÛíÑ ãÚÑÝ.", "Item not defined."))
                                    strItem = Nothing
                                    ret = "Err:item not defined"
                                End If
                            Case 14 ' New Barcode
                                'If bConvertedCash = True Then Exit Function
                                If bConvertedCash = False Then
                                    strItem = Left(strBarcode, 5)
                                    ds = dcl.GetDS("SELECT * FROM Stock_Items WHERE strItem='" & strItem & "'")
                                    If ds.Tables(0).Rows.Count > 0 Then
                                        'dateExpiry = DateSerial(Mid(strBarcode, 8, 2), Mid(strBarcode, 6, 2), 1)
                                        curBasePrice = Mid(strBarcode, 10, 3) & "." & Mid(strBarcode, 13, 2)
                                        'Me.Parent![cmdSendCash].Enabled = True
                                        'strItem_AfterUpdate()

                                        ''                    If [Forms]![Ph_Prepearing_Med].[Flag] = True Then
                                        'rsItems = CurrentDb.OpenRecordset("SELECT * FROM Hw_Treatments_Pharmacy WHERE strReference='" & [Forms]![Ph_Prepearing_Med].[strReference] & "' AND year(dateTransaction)=year(date())", dbOpenDynaset)
                                        'rsItems.MoveFirst()
                                        'rsItems.MoveLast()
                                        'rsItems.MoveFirst()
                                        'If DLookup("bControl", "cmn_Users", "strUserName='" & [Forms]![Cmn_Defaults].[strUserName] & "'") = False Then
                                        '    For I = 1 To rsItems.RecordCount
                                        '        If [strItem] = rsItems!strItem Then
                                        '            db = CurrentDb
                                        '            ws = CreateWorkspace("ODBC", "", "", dbUseODBC)
                                        '            con = ws.OpenConnection("ODBC", dbDriverNoPrompt, False, CurrentDb.TableDefs(0).Connect)
                                        '            ws.BeginTrans()
                                        '            CurrentDb.Execute "UPDATE Stock_Xlink_Items SET strDose1='" & rsItems!strDose & "'" & " WHERE lngXlink=" & [lngXlink] & " AND strItem='" & [strItem] & "'"
                                        '            '                                    con.Execute "UPDATE Healthware..Hw_Treatments_Pharmacy SET byteCheck=1 WHERE strreference=" & [Forms]![Ph_Prepearing_Med].[strReference] & " AND strItem='" & [strItem] & "' AND Year(dateTransaction)=year(date())"
                                        '            con.Close()
                                        '            [strDose1] = rsItems!strDose
                                        '            [curQuantity] = 1 'rsItems!curQuantity
                                        '            [bApproval] = rsItems!bApproval
                                        '            rsItems.MoveNext()
                                        '            ws.Close()
                                        '            Exit Function
                                        '        End If

                                        '        '    I = I + 1
                                        '        rsItems.MoveNext()
                                        '    Next I
                                        '    If Not [Forms]![Ph_Prepearing_Med].[bCash] Or DLookup("bControl", "cmn_Users", "strUserName='" & [Forms]![Cmn_Defaults].[strUserName] & "'") = False Then
                                        '        Undo()
                                        '    End If
                                        '    MsgBox(IIf(gblanguage, "ÇáÕäÝ ÛíÑ ãæÌæÏ ÈÇáØáÈ .", "Item out of order."), vbOKOnly)
                                        '    End
                                        'Else
                                        '    [bApproval] = True
                                        '    CurrentDb.Execute "UPDATE Stock_Xlink_Items SET bApproval=True WHERE lngXlink=" & [lngXlink] & " AND strItem='" & [strItem] & "'"
                                        'End If
                                        ' ''                    End If
                                        'If [strDose1] & "" = "" Then
                                        '    [strDose1] = "0000000000"
                                        'End If
                                        'strItem_AfterUpdate()
                                        'Dose_Click()
                                        ret = "OK => 14"
                                    Else
                                        'MsgBox(IIf(gblanguage, "ÇáÕäÝ ÛíÑ ãÚÑÝ.", "Item not defined."))
                                        strItem = Nothing
                                        ret = "Err:item not defined"
                                    End If
                                End If
                            Case Else
                                strItem = Nothing
                                ret = "Err:item not defined"
                        End Select
                    Else
                        ret = "Err:no barcode"
                    End If
                Else
                    ret = "Err:not approved or rejected yet"
                End If
            Else
                ret = "Err:no record"
            End If
        Else
            ret = "Err:intVisit not found"
        End If
        Return ret
    End Function

    Sub updateItemInfo(ByVal strItem As String, ByVal curBasePrice As String)
        If strItem <> "" Then
            Dim bQtySales As Boolean

            Dim bTax As Boolean
            Dim curTax As Decimal

            Dim dcl As New DCL.Conn.DataClassLayer
            Dim ds, dsTemp As DataSet

            ds = dcl.GetDS("SELECT * FROM Stock_Items AS SI LEFT JOIN Stock_Units AS SU ON SI.byteIssueUnit = SU.byteUnit LEFT JOIN Stock_Item_Prices AS SIP ON SI.strItem = SIP.strItem LEFT JOIN Hw_Department_Items AS HDI ON SI.strItem = HDI.strItem AND HDI.byteDepartment = 15 LEFT JOIN Hw_Department_Warehouse AS HDW ON HDI.intService = HDW.intService AND HDI.byteDepartment = HDW.byteDepartment WHERE SI.strItem='" & strItem & "'")
            bTax = ds.Tables(0).Rows(0).Item("bTax")
            curTax = ds.Tables(0).Rows(0).Item("curTax")

            ' Price and Unit
            Dim byteUnit As Integer
            Dim curFactor As Decimal = ds.Tables(0).Rows(0).Item("curFactor")
            Dim curPrice As Decimal = 0
            '[cboPrice].Requery()

            If curBasePrice <> "" Then
                If Not (IsDBNull(ds.Tables(0).Rows(0).Item("byteUnit"))) Then
                    byteUnit = ds.Tables(0).Rows(0).Item("byteUnit")
                    curPrice = ds.Tables(0).Rows(0).Item("curPrice")
                    'ds = dcl.GetDS("SELECT * FROM Stock_Items AS SI LEFT JOIN Stock_Units AS SU ON SI.byteStockUnit = SU.byteUnit INNER JOIN Hw_Department_Items AS HDI ON SI.strItem = HDI.strItem WHERE HDI.byteDepartment=" & byteDepartment)
                    '[curBasePrice] = CCur(Format(Nz([cboPrice].ItemData(0), 0) * [byteUnit].Column(2) / [strItemDesc].Column(5), "0." & String$(Forms![Cmn_Defaults]![byteRound], "0")))
                    curBasePrice = CDec((curPrice * curFactor) / curFactor)
                Else
                    '[curBasePrice] = CCur(Format(Nz([cboPrice].ItemData(0), 0), "0." & String$(Forms![Cmn_Defaults]![byteRound], "0")))
                    curBasePrice = CDbl(curPrice)
                End If
            End If
            ' End New Code
            If curBasePrice = 0 Then curBasePrice = Nothing

            Dim intService As Integer = ds.Tables(0).Rows(0).Item("intService")
            Dim byteWarehouse As Byte = ds.Tables(0).Rows(0).Item("byteWarehouse")

            Dim intGroup As Integer = ds.Tables(0).Rows(0).Item("intGroup")
            Dim curDiscount, curBaseDiscount As Decimal
            dsTemp = dcl.GetDS("SELECT * FROM Hw_Contacts AS HC LEFT JOIN Stock_Price_Groups AS SPG ON HC.bytePriceType=SPG.bytePriceType WHERE lngContact=27 AND bResident=1")
            If dsTemp.Tables(0).Rows.Count > 0 Then
                curDiscount = CDec("0" & dsTemp.Tables(0).Rows(0).Item("curPercent").ToString)
            Else
                curDiscount = 0
            End If
            curBaseDiscount = curDiscount
            '    CheckItemCoverage
            Dim byteTransType, bytePromotionClass As Byte
            Dim bCash As Boolean
            dsTemp = dcl.GetDS("SELECT * FROM Stock_Trans_Types AS STT INNER JOIN Stock_TransType_Departments AS STD ON STT.byteTransType = STD.byteTransType INNER JOIN Cmn_User_Departments AS CUD ON STD.byteDepartment = CUD.byteDepartment WHERE CUD.strUserName='SoftNet' AND STT.byteBase=50")
            If dsTemp.Tables(0).Rows.Count > 0 Then
                byteTransType = dsTemp.Tables(0).Rows(0).Item("byteCash")
            Else
                byteTransType = 3
            End If

            bCash = getByteTransType(byteTransType)
            bytePromotionClass = DeterminePromotionClass()
            If bCash And bytePromotionClass <> 0 Then
                'curDiscount = Nz(DLookup("curDiscount", "Hw_Promotion_Groups", "byteClass=" & Me.Parent![bytePromotionClass] & " AND intGroup=" & Val([strItemDesc].Column(6))), 0)
                '[curUnitPrice] = [curUnitPrice] - ([curUnitPrice] * Abs([curDiscount] / 100))
            End If
            Dim curNet, curQuantity As Decimal
            curQuantity = 1
            curNet = (curBasePrice * curQuantity) - (curBasePrice * curQuantity * curDiscount / 100)

            Dim bPercentValue As Boolean = False
            Dim curCoverage, curVAT, curVATI As Decimal
            curCoverage = 0
            If Not bCash Then
                If bTax = True And curTax <> 0 Then
                    If bPercentValue = True And curCoverage <> 0 Then
                        curVAT = (curNet * curCoverage / 100) * curTax / 100
                        curVATI = (curNet * (100 - curCoverage) / 100) * curTax / 100
                    Else
                        curVAT = 0
                        curVATI = 0
                    End If
                Else
                    curVAT = 0
                    curVATI = 0
                End If
            End If
        End If
    End Sub
    Function getByteTransType(ByVal byteTransType As Byte) As Boolean
        Select Case byteTransType
            Case 1 ' Cash
                '[bCash] = True
                '[bCash].Enabled = False
                '[lngCRCard].Enabled = True
                Return True
            Case 2 ' Credit
                '[bCash] = False
                '[bCash].Enabled = False
                '[lngCRCard].Enabled = [bCash]
                '[lngCRCard] = Null
                Return False
            Case 3 ' Both
                '[bCash].Enabled = True
                '[lngCRCard].Enabled = [bCash]
                Return True
            Case Else
                Return True
        End Select

        'Contact_Requery False

        '27
        'If [bCash] Then
        '    [lngContact] = [byteTransType].Column(4)
        '    [lngContactNo] = [lngContact]
        'Else
        '    [lngContact] = Null
        '    [lngContactNo] = [lngContact]
        'End If
        '[lstPatintInvoices].Requery()
    End Function

    Function DeterminePromotionClass() As Byte
        'Dim rs As Recordset
        'Dim rsAmount As Recordset
        'Dim strAmount As String
        'Dim rsPromotions As Recordset

        'rsPromotions = db.OpenRecordset("Hw_Promotions", dbOpenSnapshot)
        'rs = db.OpenRecordset("SELECT dateParticipation, byteClass, dateClass, lngFamily FROM Hw_Patients WHERE lngPatient=" & [lngPatient])
        'If rs!lngFamily & "" = "" Then ' Single patient
        '    Select Case rs![byteClass]
        '        Case 1 ' VIP Class
        '            If Year(rs![dateClass]) = Year([dateTransaction]) Then
        '                [bytePromotionClass] = rs![byteClass]
        '            Else
        '                '                    Set rsAmount = DB.OpenRecordset("SELECT Sum(curUnitPrice*curQuantity) AS curAmount FROM ((((Stock_Trans AS T INNER JOIN Stock_Base AS B ON T.byteBase = B.byteBase) INNER JOIN Stock_Xlink AS X ON t.lngTransaction = X.lngTransaction) INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink) INNER JOIN Stock_Items AS I ON XI.strItem = I.strItem) INNER JOIN Stock_Groups AS G ON I.intGroup= G.intGroup WHERE Year(dateTransaction)=" & Year(rs![dateClass]) & " AND T.byteBase IN(41, 18) AND T.byteStatus >0 AND T.bCash=1 AND G.bPromotionIncluded = 1 AND lngPatient=" & [lngPatient])
        '                strAmount = "SELECT Sum(([curUnitPrice]*[curQuantity])) AS curAmount, ClinicInv.SumOfAmount AS ClinicInv "
        '                strAmount = strAmount & " FROM ((((((Stock_Trans AS T INNER JOIN Stock_Base AS B ON T.byteBase = B.byteBase) INNER JOIN Stock_Xlink AS X ON T.lngTransaction = X.lngTransaction) INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink) INNER JOIN Stock_Items AS I ON XI.strItem = I.strItem) INNER JOIN Stock_Groups AS G ON I.intGroup = G.intGroup) INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient) LEFT JOIN Clinic_Invoices_Amount AS ClinicInv ON P.lngPatient = ClinicInv.lngPatient"
        '                strAmount = strAmount & " WHERE T.byteBase In (40,18) AND T.byteStatus>0 AND T.lngPatient=" & [lngPatient] & " AND G.bPromotionIncluded=1 AND dateTransaction>=#" & Format(Nz(DLookup("dateLastService", "Hw_Patients", "lngPatient=" & [lngPatient]), #1/1/2006#), "mm\/dd\/yyyy") & "#"
        '                strAmount = strAmount & " GROUP BY ClinicInv.SumOfAmount"
        '                rsAmount = CurrentDb.OpenRecordset(strAmount, dbOpenSnapshot)
        '                rsPromotions.FindFirst "byteClass=1" ' VIP
        '                If Nz(rsAmount![curAmount], 0) + Nz(rsAmount![ClinicInv], 0) >= rsPromotions![curAmountFrom] Then
        '                    [bytePromotionClass] = 1
        '                    db.Execute("UPDATE Hw_Patients SET byteClass=1, dateClass=#" & Format([dateTransaction], "mm\/dd\/yyyy") & "# WHERE lngPatient=" & [lngPatient], dbFailOnError)
        '                Else
        '                    [bytePromotionClass] = 2
        '                    db.Execute("UPDATE Hw_Patients SET byteClass=2, dateClass=#" & Format([dateTransaction], "mm\/dd\/yyyy") & "# WHERE lngPatient=" & [lngPatient], dbFailOnError)
        '                End If
        '            End If
        '        Case 2 ' A Class
        '            If Year(rs![dateClass]) = Year([dateTransaction]) Then
        '                [bytePromotionClass] = rs![byteClass]
        '            Else
        '                '                    Set rsAmount = DB.OpenRecordset("SELECT Sum(curUnitPrice*curQuantity) AS curAmount FROM ((((Stock_Trans AS T INNER JOIN Stock_Base AS B ON T.byteBase = B.byteBase) INNER JOIN Stock_Xlink AS X ON t.lngTransaction = X.lngTransaction) INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink) INNER JOIN Stock_Items AS I ON XI.strItem = I.strItem) INNER JOIN Stock_Groups AS G ON I.intGroup= G.intGroup WHERE Year(dateTransaction)=" & Year(rs![dateClass]) & " AND dateTransaction>=#" & Format(rs![dateClass], "mm\/dd\/yyyy") & "# AND T.byteBase IN(41, 18) AND T.byteStatus >0 AND T.bCash=1 AND G.bPromotionIncluded = 1 AND lngPatient=" & [lngPatient])
        '                strAmount = "SELECT Sum(([curUnitPrice]*[curQuantity])) AS curAmount, ClinicInv.SumOfAmount AS ClinicInv "
        '                strAmount = strAmount & " FROM ((((((Stock_Trans AS T INNER JOIN Stock_Base AS B ON T.byteBase = B.byteBase) INNER JOIN Stock_Xlink AS X ON T.lngTransaction = X.lngTransaction) INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink) INNER JOIN Stock_Items AS I ON XI.strItem = I.strItem) INNER JOIN Stock_Groups AS G ON I.intGroup = G.intGroup) INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient) LEFT JOIN Clinic_Invoices_Amount AS ClinicInv ON P.lngPatient = ClinicInv.lngPatient"
        '                strAmount = strAmount & " WHERE T.byteBase In (40,18) AND T.byteStatus>0 AND T.lngPatient=" & [lngPatient] & " AND G.bPromotionIncluded=1 AND dateTransaction>=#" & Format(Nz(DLookup("dateLastService", "Hw_Patients", "lngPatient=" & [lngPatient]), #1/1/2006#), "mm\/dd\/yyyy") & "#"
        '                strAmount = strAmount & " GROUP BY ClinicInv.SumOfAmount"
        '                rsAmount = CurrentDb.OpenRecordset(strAmount, dbOpenSnapshot)
        '                rsPromotions.FindFirst "byteClass=1" ' VIP
        '                If Nz(rsAmount![curAmount], 0) + Nz(rsAmount![ClinicInv], 0) >= rsPromotions![curAmountFrom] Then
        '                    [bytePromotionClass] = 1
        '                    db.Execute("UPDATE Hw_Patients SET byteClass=1, dateClass=#" & Format([dateTransaction], "mm\/dd\/yyyy") & "# WHERE lngPatient=" & [lngPatient], dbFailOnError)
        '                Else
        '                    [bytePromotionClass] = 2
        '                    db.Execute("UPDATE Hw_Patients SET byteClass=2, dateClass=#" & Format([dateTransaction], "mm\/dd\/yyyy") & "# WHERE lngPatient=" & [lngPatient], dbFailOnError)
        '                End If
        '            End If
        '        Case Else
        '            [bytePromotionClass] = rs![byteClass]
        '    End Select

        'Else ' Family
        '    lngFamily = rs!lngFamily
        '    intPatients = DCount("lngPatient", "Hw_Patients", "lngFamily=" & rs!lngFamily)
        '    Select Case rs![byteClass]
        '        Case 1 ' VIP Class
        '            If Year(rs![dateClass]) = Year([dateTransaction]) Then
        '                [bytePromotionClass] = rs![byteClass]
        '            Else
        '                '                    Set rsAmount = DB.OpenRecordset("SELECT Sum(curUnitPrice*curQuantity) AS curAmount FROM (((((Stock_Trans AS T INNER JOIN Stock_Base AS B ON T.byteBase = B.byteBase) INNER JOIN Stock_Xlink AS X ON t.lngTransaction = X.lngTransaction) INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink) INNER JOIN Stock_Items AS I ON XI.strItem = I.strItem) INNER JOIN Stock_Groups AS G ON I.intGroup= G.intGroup) INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient WHERE Year(dateTransaction)=" & Year(rs![dateClass]) & " AND T.byteBase IN(41, 18) AND T.byteStatus >0 AND T.bCash=1 AND G.bPromotionIncluded = 1 AND P.lngFamily=" & rs!lngFamily)
        '                strAmount = "SELECT Sum(([curUnitPrice]*[curQuantity])) AS curAmount, ClinicInv.SumOfAmount AS ClinicInv "
        '                strAmount = strAmount & " FROM ((((((Stock_Trans AS T INNER JOIN Stock_Base AS B ON T.byteBase = B.byteBase) INNER JOIN Stock_Xlink AS X ON T.lngTransaction = X.lngTransaction) INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink) INNER JOIN Stock_Items AS I ON XI.strItem = I.strItem) INNER JOIN Stock_Groups AS G ON I.intGroup = G.intGroup) INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient) LEFT JOIN Clinic_Invoices_Amount AS ClinicInv ON P.lngPatient = ClinicInv.lngPatient"
        '                strAmount = strAmount & " WHERE T.byteBase In (40,18) AND T.byteStatus>0 AND T.lngPatient=" & [lngPatient] & " AND G.bPromotionIncluded=1 AND dateTransaction>=#" & Format(Nz(DLookup("dateLastService", "Hw_Patients", "lngPatient=" & [lngPatient]), #1/1/2006#), "mm\/dd\/yyyy") & "#"
        '                strAmount = strAmount & " GROUP BY ClinicInv.SumOfAmount"
        '                rsAmount = CurrentDb.OpenRecordset(strAmount, dbOpenSnapshot)
        '                rsPromotions.FindFirst "byteClass=1" ' VIP
        '                If Nz(rsAmount![curAmount], 0) + Nz(rsAmount![ClinicInv], 0) >= rsPromotions![curAmountFrom] * intPatients Then
        '                    [bytePromotionClass] = 1
        '                    db.Execute("UPDATE Hw_Patients SET byteClass=1, dateClass=#" & Format([dateTransaction], "mm\/dd\/yyyy") & "# WHERE lngFamily=" & [lngFamily], dbFailOnError)
        '                Else
        '                    [bytePromotionClass] = 2
        '                    db.Execute("UPDATE Hw_Patients SET byteClass=2, dateClass=#" & Format([dateTransaction], "mm\/dd\/yyyy") & "# WHERE lngFamily=" & [lngFamily], dbFailOnError)
        '                End If
        '            End If
        '        Case 2 ' A Class
        '            If Year(rs![dateClass]) = Year([dateTransaction]) Then
        '                [bytePromotionClass] = rs![byteClass]
        '            Else
        '                '                    Set rsAmount = DB.OpenRecordset("SELECT Sum(curUnitPrice*curQuantity) AS curAmount FROM (((((Stock_Trans AS T INNER JOIN Stock_Base AS B ON T.byteBase = B.byteBase) INNER JOIN Stock_Xlink AS X ON t.lngTransaction = X.lngTransaction) INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink) INNER JOIN Stock_Items AS I ON XI.strItem = I.strItem) INNER JOIN Stock_Groups AS G ON I.intGroup= G.intGroup) INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient  WHERE Year(dateTransaction)=" & Year(rs![dateClass]) & " AND dateTransaction>=#" & Format(rs![dateClass], "mm\/dd\/yyyy") & "# AND T.byteBase IN(41, 18) AND T.byteStatus >0 AND T.bCash=1 AND G.bPromotionIncluded = 1 AND lngFamily=" & rs!lngFamily)
        '                strAmount = "SELECT Sum(([curUnitPrice]*[curQuantity])) AS curAmount, ClinicInv.SumOfAmount AS ClinicInv "
        '                strAmount = strAmount & " FROM ((((((Stock_Trans AS T INNER JOIN Stock_Base AS B ON T.byteBase = B.byteBase) INNER JOIN Stock_Xlink AS X ON T.lngTransaction = X.lngTransaction) INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink) INNER JOIN Stock_Items AS I ON XI.strItem = I.strItem) INNER JOIN Stock_Groups AS G ON I.intGroup = G.intGroup) INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient) LEFT JOIN Clinic_Invoices_Amount AS ClinicInv ON P.lngPatient = ClinicInv.lngPatient"
        '                strAmount = strAmount & " WHERE T.byteBase In (40,18) AND T.byteStatus>0 AND T.lngPatient=" & [lngPatient] & " AND G.bPromotionIncluded=1 AND dateTransaction>=#" & Format(Nz(DLookup("dateLastService", "Hw_Patients", "lngPatient=" & [lngPatient]), #1/1/2006#), "mm\/dd\/yyyy") & "#"
        '                strAmount = strAmount & " GROUP BY ClinicInv.SumOfAmount"
        '                rsAmount = CurrentDb.OpenRecordset(strAmount, dbOpenSnapshot)
        '                rsPromotions.FindFirst "byteClass=1" ' VIP
        '                If Nz(rsAmount![curAmount], 0) + Nz(rsAmount![ClinicInv], 0) >= rsPromotions![curAmountFrom] * intPatients Then
        '                    [bytePromotionClass] = 1
        '                    db.Execute("UPDATE Hw_Patients SET byteClass=1, dateClass=#" & Format([dateTransaction], "mm\/dd\/yyyy") & "# WHERE lngFamily=" & rs!lngFamily, dbFailOnError)
        '                Else
        '                    [bytePromotionClass] = 2
        '                    db.Execute("UPDATE Hw_Patients SET byteClass=2, dateClass=#" & Format([dateTransaction], "mm\/dd\/yyyy") & "# WHERE lngFamily=" & rs!lngFamily, dbFailOnError)
        '                End If
        '            End If
        '        Case Else
        '            [bytePromotionClass] = rs![byteClass]
        '    End Select
        'End If
        Return 0
    End Function
End Class