Imports System.Web
Imports System.Xml
Imports System.Text

Class Functions
    Dim dcl As New DCL.Conn.DataClassLayer
    Dim strUserName As String
    Dim ByteLanguage As Byte
    Dim strDateFormat, strTimeFormat As String
    Dim DataLang As String

    Dim byteLocalCurrency As Byte
    Dim intStartupFY As Integer
    Dim intYear As Integer
    Const byteDepartment As Byte = 15
    Const byteBase As Byte = 50
    Dim byteCurrencyRound As Byte
    Dim ChangeQuantity_Cash, AddDiscount_Cash, ChangeQuantity_Insurance, AddDiscount_Insurance, AllowExtraItem_Insurance, AutoMoveRejectedToCash_Insurance, AutoMoveExtraToCash_Insurance, AskBeforeSend, AskBeforeReturn, OnePaymentForCashier, ForcePaymentOnCloseInvoice, OneQuantityPerItem, DirectCancelInvoice, PopupToPrint, TaxEnabled, AllowPrintEmptyDose As Boolean
    Dim SusbendMax, byteDepartment_Cash, DaysToCalculateMedicalInvoices, DaysToCalculateMedicineInvoices, OrdersLimitDays, CancelLimitDays, PrintDose, PrintInvoice As Byte
    Dim lngContact_Cash, lngSalesman_Cash, lngPatient_Cash As Long
    Dim strContact_Cash, strSalesman_Cash, strPatient_Cash, strDepartment_Cash, DosePrinter, InvoicePrinter As String

    Dim AllowCancel As Boolean

    Private Class NameValue
        Public Property name As String
        Public Property value As String
    End Class

    Sub New()
        ' User Options
        If (HttpContext.Current.Session("UserName") Is Nothing) Or (HttpContext.Current.Session("UserName") = "") Then
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
        ByteLanguage = HttpContext.Current.Session("UserLanguage")
        strDateFormat = HttpContext.Current.Session("UserDTFormat")
        strTimeFormat = HttpContext.Current.Session("UserTMFormat")

        loadSettings()

        Dim dc As New DCL.Conn.XMLData
        AllowCancel = dc.CheckExistNode(HttpContext.Current.Server.MapPath("../data/xml/privileges.xml"), "Cancel_Invoice", "User", strUserName)
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
        If application.SelectSingleNode("OneQuantityPerItem") Is Nothing Then OneQuantityPerItem = True Else OneQuantityPerItem = application.SelectSingleNode("OneQuantityPerItem").InnerText
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

        If application.SelectSingleNode("PopupToPrint") Is Nothing Then PopupToPrint = True Else PopupToPrint = application.SelectSingleNode("PopupToPrint").InnerText
        If application.SelectSingleNode("PrintDose") Is Nothing Then PrintDose = 3 Else PrintDose = application.SelectSingleNode("PrintDose").InnerText
        If application.SelectSingleNode("PrintInvoice") Is Nothing Then PrintInvoice = 2 Else PrintInvoice = application.SelectSingleNode("PrintInvoice").InnerText
        If application.SelectSingleNode("DosePrinter") Is Nothing Then DosePrinter = "ZDesigner GK420t" Else DosePrinter = application.SelectSingleNode("DosePrinter").InnerText
        If application.SelectSingleNode("InvoicePrinter") Is Nothing Then InvoicePrinter = "HP LaserJet Professional P1566" Else InvoicePrinter = application.SelectSingleNode("InvoicePrinter").InnerText
        If application.SelectSingleNode("TaxEnabled") Is Nothing Then TaxEnabled = True Else TaxEnabled = application.SelectSingleNode("TaxEnabled").InnerText
        If application.SelectSingleNode("AllowPrintEmptyDose") Is Nothing Then AllowPrintEmptyDose = True Else AllowPrintEmptyDose = application.SelectSingleNode("AllowPrintEmptyDose").InnerText

        If ByteLanguage = 2 Then DataLang = "Ar" Else DataLang = "En"
        Dim ds As DataSet
        ds = dcl.GetDS("SELECT * FROM Hw_Contacts WHERE lngContact = " & lngContact_Cash & "; SELECT * FROM Hw_Contacts WHERE lngContact = " & lngSalesman_Cash & "; SELECT RTRIM(LTRIM(ISNULL(strFirst" & DataLang & ",'') + ' ') + LTRIM(ISNULL(strSecond" & DataLang & ",'') + ' ') + LTRIM(ISNULL(strThird" & DataLang & " ,'') + ' ') + LTRIM(ISNULL(strLast" & DataLang & ",''))) AS PatientName, * FROM Hw_Patients WHERE lngPatient = " & lngPatient_Cash & "; SELECT * FROM Hw_Departments WHERE byteDepartment = " & byteDepartment_Cash)
        If ds.Tables(0).Rows.Count > 0 Then strContact_Cash = ds.Tables(0).Rows(0).Item("strContact" & DataLang).ToString Else strContact_Cash = ""
        If ds.Tables(1).Rows.Count > 0 Then strSalesman_Cash = ds.Tables(1).Rows(0).Item("strContact" & DataLang).ToString Else strSalesman_Cash = ""
        If ds.Tables(2).Rows.Count > 0 Then strPatient_Cash = ds.Tables(2).Rows(0).Item("PatientName").ToString Else strPatient_Cash = ""
        If ds.Tables(3).Rows.Count > 0 Then strDepartment_Cash = ds.Tables(3).Rows(0).Item("strDepartment" & DataLang).ToString Else strDepartment_Cash = ""
    End Sub

    Public Function getCoverage(ByVal lngPatient As Long, ByVal lngContact As Long) As String() 'return (0) bPercentValue, (1) curCoverage, (2) MaxLimit
        Dim curCoverage As Decimal
        Dim lngContract As Long
        Dim byteScheme, bytePrimaryDep As Byte
        Dim bPercentValue As Boolean
        Dim MaxP As Decimal
        Try
            Dim dsGuar As DataSet
            dsGuar = dcl.GetDS("SELECT * FROM Ins_Contracts WHERE lngGuarantor = " & lngContact)
            If dsGuar.Tables(0).Rows.Count > 0 Then
                Dim dsIns As DataSet
                dsIns = dcl.GetDS("SELECT HP.bytePrimaryDep, HP.lngGuarantor, IC.lngContract, IC.byteScheme, IC.curDeductionValueP, IC.curDeductionPercentP, IC.curDeductionValueD, IC.curDeductionPercentD FROM Hw_Patients AS HP INNER JOIN Ins_Coverage AS IC ON HP.byteScheme = IC.byteScheme AND HP.lngContract = IC.lngContract WHERE IC.byteScope=2 AND lngPatient=" & lngPatient)
                If dsIns.Tables(0).Rows.Count > 0 Then
                    lngContract = dsIns.Tables(0).Rows(0).Item("lngContract")
                    byteScheme = dsIns.Tables(0).Rows(0).Item("byteScheme")
                    bytePrimaryDep = dsIns.Tables(0).Rows(0).Item("bytePrimaryDep")
                    If bytePrimaryDep = 1 Then
                        If IsDBNull(dsIns.Tables(0).Rows(0).Item("curDeductionValueP")) Then curCoverage = CDec("0" & dsIns.Tables(0).Rows(0).Item("curDeductionPercentP")) Else curCoverage = CDec("0" & dsIns.Tables(0).Rows(0).Item("curDeductionValueP"))
                        bPercentValue = IsDBNull(dsIns.Tables(0).Rows(0).Item("curDeductionValueP"))
                    Else
                        If IsDBNull(dsIns.Tables(0).Rows(0).Item("curDeductionValueD")) Then curCoverage = CDec("0" & dsIns.Tables(0).Rows(0).Item("curDeductionPercentD")) Else curCoverage = CDec("0" & dsIns.Tables(0).Rows(0).Item("curDeductionValueD"))
                        bPercentValue = IsDBNull(dsIns.Tables(0).Rows(0).Item("curDeductionValueD"))
                    End If

                    Dim dsGroup As DataSet
                    dsGroup = dcl.GetDS("SELECT curYearLimitP, curCaseLimitP,curDeductionMaxP FROM Ins_Coverage WHERE byteScope=2 AND lngContract=" & dsIns.Tables(0).Rows(0).Item("lngContract") & " AND byteScheme=" & dsIns.Tables(0).Rows(0).Item("byteScheme"))
                    MaxP = CDec("0" & dsGroup.Tables(0).Rows(0).Item("curDeductionMaxP").ToString)

                    Return {bPercentValue, curCoverage, MaxP}
                Else
                    Return {"Err: Credit invoice,Complete insurance information"}
                End If
            Else
                Return {"Err: There are no contracts with this company."}
            End If
        Catch ex As Exception
            Return {"Err:" & ex.Message}
        End Try
    End Function

    Public Function getTotalClinicInvoices(ByVal lngPatient As Long, ByVal lngSalesMan As Long, ByVal dateTransaction As Date) As Decimal
        Dim dsClinic As DataSet
        dsClinic = dcl.GetDS("SELECT Sum(Amount) AS SumOfAmount, lngSalesman, Sum(curCoverage) AS Coverage FROM Clinic_Invoices WHERE dateTransaction Between '" & DateAdd(DateInterval.Day, (DaysToCalculateMedicalInvoices * -1), dateTransaction).ToString("yyyy-MM-dd") & "' And '" & dateTransaction.ToString("yyyy-MM-dd") & "' AND lngPatient=" & lngPatient & " AND lngSalesMan=" & lngSalesMan & " GROUP BY lngSalesman")
        If dsClinic.Tables(0).Rows.Count > 0 Then
            Dim total As Decimal
            If IsDBNull(dsClinic.Tables(0).Rows(0).Item("Coverage")) Then
                total = 0
            Else
                total = dsClinic.Tables(0).Rows(0).Item("Coverage")
            End If
            Return total
        Else
            Return 0
        End If
    End Function

    Public Function getTotalPharmacyInvoices(ByVal lngPatient As Long, ByVal lngSalesMan As Long, ByVal dateTransaction As Date, ByVal lngTransaction As Long, ByVal InculdeCurrent As Boolean) As Decimal
        Dim dsMidicine As DataSet
        Dim TrnasSQL As String = ""
        If InculdeCurrent = False Then TrnasSQL = " AND ST.lngTransaction<>" & lngTransaction
        'dsMidicine = dcl.GetDS("SELECT SUM(SXI.curUnitPrice) AS Amount, SUM(SXI.curCoverage) AS Cov FROM Stock_Trans AS ST INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink WHERE dateTransaction BETWEEN '" & DateAdd(DateInterval.Day, (DaysToCalculateMedicineInvoices * -1), dateTransaction).ToString("yyyy-MM-dd") & "' AND '" & dateTransaction.ToString("yyyy-MM-dd") & "' AND lngPatient=" & lngPatient & " AND lngSalesMan=" & lngSalesMan & " AND (ST.byteBase = 40 OR ST.byteBase = 50) AND ST.byteStatus > 0 " & TrnasSQL & " GROUP BY ST.lngSalesman")
        dsMidicine = dcl.GetDS("SELECT SUM(SXI.curUnitPrice) AS Amount, SUM(SXI.curCoverage) AS Cov FROM Stock_Trans AS ST INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink WHERE dateTransaction BETWEEN '" & DateAdd(DateInterval.Day, (DaysToCalculateMedicineInvoices * -1), dateTransaction).ToString("yyyy-MM-dd") & "' AND '" & dateTransaction.ToString("yyyy-MM-dd") & "' AND lngPatient=" & lngPatient & " AND lngSalesMan=" & lngSalesMan & " AND ST.byteBase = 40 AND ST.byteStatus > 0 " & TrnasSQL & " GROUP BY ST.lngSalesman")
        If dsMidicine.Tables(0).Rows.Count > 0 Then
            Return CDec("0" & dsMidicine.Tables(0).Rows(0).Item("Cov").ToString)
        Else
            Return 0
        End If
    End Function

    Public Function calcCoveredCash(ByVal curTotal As Decimal, ByVal curPercent As Decimal, ByVal bPercent As Boolean, ByVal curMaxDeduction As Decimal, ByVal curTotalClinicInvoices As Decimal, ByVal curTotalPharmacyInvoices As Decimal) As Decimal
        Dim curCoverd As Decimal = curTotal * (curPercent / 100)
        Dim curExter As Decimal = (curTotalClinicInvoices + curTotalPharmacyInvoices + curTotal) - curMaxDeduction
        If bPercent = True Then
            If curCoverd = 0 Then
                Return 0
            Else
                If curCoverd > curMaxDeduction - (curTotalClinicInvoices + curTotalPharmacyInvoices) Then
                    Return 0
                Else
                    Return curCoverd
                End If
            End If
        Else
            Return 0
        End If
    End Function

#Region "UI"
    Public Function createItemRow(ByVal lngTransaction As Long, ByVal Counter As Integer, ByVal IsCash As Boolean, ByVal Flag As String, ByVal strBarcode As String, ByVal strItem As String, ByVal strItemName As String, ByVal byteUnit As Byte, ByVal dateExpiry As Date, ByVal curBasePrice As Decimal, ByVal curDiscount As Decimal, ByVal curQuantity As Decimal, ByVal curBaseDiscount As Decimal, ByVal curCoverage As Decimal, ByVal curVAT As Decimal, ByVal intService As Integer, ByVal byteWarehouse As Byte, ByVal strDose As String, ByVal Editable As Boolean, Optional ByVal Returned As Boolean = False, Optional ByVal AutoPrintDose As Boolean = False, Optional ByVal CanMove As Boolean = False) As String
        Dim item As String
        Dim btnPrint, btnDelete As String
        Dim Typ, Cls, Func, ExpireDate, moveButton, printButton, allButtons, MoveC, MoveI As String
        Dim curTotal, curUnitPrice As Decimal

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
                btnPrint = "طباعة"
                btnDelete = "حذف"
                MoveC = "نقدي"
                MoveI = "آجل"
            Case Else
                DataLang = "En"
                btnPrint = "Print"
                btnDelete = "Delete"
                MoveC = "Cash"
                MoveI = "Credit"
        End Select

        If IsCash = False Then
            Typ = "I"
            Cls = "dynInsurance"
            Func = "calculateInsurance"
            'curUnitPrice = Math.Round((curBasePrice * curQuantity) - ((curBasePrice * curQuantity) * (curDiscount / 100)), byteCurrencyRound, MidpointRounding.AwayFromZero)
            curUnitPrice = Math.Round(curBasePrice - (curBasePrice * (curDiscount / 100)), byteCurrencyRound, MidpointRounding.AwayFromZero)
            'curTotal = curCoverage
            curTotal = Math.Round(curUnitPrice * curQuantity, byteCurrencyRound, MidpointRounding.AwayFromZero)
            curBaseDiscount = curBaseDiscount
            If CanMove = True Then moveButton = "<button type=""button"" onclick=""javascript:move2C(this,\'" & strBarcode & "\');" & Func & "(curTab);"" class=""btn btn-info btn-lighten-3 btn-xs"">" & MoveC & "</button>" Else moveButton = ""
        Else
            Typ = "C"
            Cls = "dynCash"
            Func = "calculateCash"
            'curUnitPrice = Math.Round((curBasePrice * curQuantity) - ((curBasePrice * curQuantity) * (curDiscount / 100)), byteCurrencyRound, MidpointRounding.AwayFromZero)
            curUnitPrice = Math.Round(curBasePrice - (curBasePrice * (curDiscount / 100)), byteCurrencyRound, MidpointRounding.AwayFromZero)
            'curTotal = curUnitPrice
            curTotal = Math.Round(curUnitPrice * curQuantity, byteCurrencyRound, MidpointRounding.AwayFromZero)
            curBaseDiscount = 0
            If CanMove = True Then moveButton = "<button type=""button"" onclick=""javascript:move2I(this,\'" & strBarcode & "\');" & Func & "(curTab);"" class=""btn btn-info btn-lighten-3 btn-xs"">" & MoveI & "</button>" Else moveButton = ""
        End If

        If dateExpiry <= DateAdd(DateInterval.Month, 3, Today) Then
            ExpireDate = "<span class=""tag tag-danger tag-xs"">" & CDate(dateExpiry).ToString("yyyy-MM") & "</span><input type=""hidden"" name=""expire_" & Typ & """ class=""expire"" value=""" & CDate(dateExpiry).ToString("yyyy-MM-dd") & """/>"
        Else
            ExpireDate = CDate(dateExpiry).ToString("yyyy-MM") & "<input type=""hidden"" name=""expire_" & Typ & """ class=""expire"" value=""" & CDate(dateExpiry).ToString("yyyy-MM-dd") & """/>"
        End If

        Dim PrintData As String = ""
        'PrintData = Replace(createDosePrint(lngTransaction, strItem, dateExpiry), """", "|")
        'PrintData = Replace(PrintData, "'", "\'")

        If (lngTransaction > 0) And ((PrintDose = 2) Or (PrintDose = 3)) And ((strDose <> "0000000000") And (strDose <> "") Or (AllowPrintEmptyDose = True)) Then
            printButton = "<span app-print=""true"" app-popup=""" & PopupToPrint.ToString.ToLower & """ app-printer=""" & DosePrinter & """ app-url=""p_dose.aspx?t=" & lngTransaction & "&i=" & strItem & "&e=" & dateExpiry.ToString("yyyy-MM-dd") & """ app-data=""" & PrintData & """><button type=""button"" class=""btn btn-blue-grey btn-xs printDose " & Cls & """>" & btnPrint & "</button></span>"
        Else
            printButton = ""
        End If

        Dim AutoPrint As String = ""
        If (AutoPrintDose = True) And ((PrintDose = 1) Or (PrintDose = 3)) And ((strDose <> "0000000000") And (strDose <> "") Or (AllowPrintEmptyDose = True)) Then AutoPrint = "<i><span id=""directPrint"" app-printer=""" & DosePrinter & """ app-url=""p_dose.aspx?t=" & lngTransaction & "&i=" & strItem & "&e=" & CDate(dateExpiry).ToString("yyyy-MM-dd") & """ app-data=""" & PrintData & """></span></i>"

        If Editable = True Then allButtons = printButton & " <button type=""button"" onclick=""javascript:remove" & Typ & "Items(this);removeThis(this);" & Func & "(curTab);"" class=""btn btn-red btn-lighten-3 btn-xs"">" & btnDelete & "</button> " & moveButton & AutoPrint Else allButtons = ""
        Dim DelStart, DelEnd, Lit As String
        If Returned = True Then
            DelStart = "<del>"
            DelEnd = "</del>"
            Lit = "bg-grey bg-lighten-4"
        Else
            DelStart = ""
            DelEnd = ""
            Lit = ""
        End If
        'add row
        item = "<tr id=""tr_" & Counter & """ class=""" & Typ & "tr " & Lit & """>"
        'add flag + barcode + item + dose + unit
        item = item & "<td style=""width:32px;"">" & Flag & "<input type=""hidden"" name=""barcode_" & Typ & """ value=""" & strBarcode & """/><input type=""hidden"" name=""dose_" & Typ & """ class=""dose"" value=""" & strDose & """/><input type=""hidden"" name=""item_" & Typ & """ class=""item"" value=""" & strItem & """/><input type=""hidden"" name=""unit_" & Typ & """ value=""" & byteUnit & """/></td>"
        'add item
        item = item & "<td style=""width:70px;"" class=""" & Cls & """>" & DelStart & strItem & DelEnd & "</td>"
        'add item name
        item = item & "<td class=""itemName width-150"" title=""" & strItemName & """>" & DelStart & strItemName & DelEnd & "</td>"
        'add expire date
        item = item & "<td style=""width:80px;"" class=""" & Cls & " red"">" & ExpireDate & "</td>"
        'add baseprice + service + warehouse
        item = item & "<td style=""width:80px;"" class=""" & Cls & """>" & Math.Round(curBasePrice, byteCurrencyRound, MidpointRounding.AwayFromZero) & "<input type=""hidden"" id=""price"" name=""baseprice_" & Typ & """ class=""price_" & Typ & """ value=""" & curBasePrice & """/><input type=""hidden"" name=""service_" & Typ & """ value=""" & intService & """/><input type=""hidden"" name=""warehouse_" & Typ & """ value=""" & byteWarehouse & """/></td>"
        'add unitprice + unitdicsount
        item = item & "<td style=""width:80px;"" class=""" & Cls & """>" & Math.Round(curDiscount, byteCurrencyRound, MidpointRounding.AwayFromZero) & " %<input type=""hidden"" name=""basediscount_" & Typ & """ value=""" & curBaseDiscount & """/><input type=""hidden"" id=""discount"" name=""discount_" & Typ & """ class=""discount_" & Typ & """ value=""" & curDiscount & """/></td>"
        'add quantity
        item = item & "<td style=""width:44px;"">" & Math.Round(curQuantity, byteCurrencyRound, MidpointRounding.AwayFromZero) & "<input type=""hidden"" name=""quantity_" & Typ & """ class=""quantity_" & Typ & """ value=""" & curQuantity & """/></td>"
        'add total + coverage + vat
        item = item & "<td style=""width:80px;"">" & Math.Round(curTotal, byteCurrencyRound, MidpointRounding.AwayFromZero) & "<input type=""hidden"" id=""total"" name=""unitprice_" & Typ & """ class=""total_" & Typ & """ value=""" & curUnitPrice & """/><input type=""hidden"" id=""coverage_" & Typ & """ name=""coverage_" & Typ & """ class=""coverage_" & Typ & """ value=""" & curCoverage & """/><input type=""hidden"" class=""vat_" & Typ & """ name=""vat_" & Typ & """ value=""" & curVAT & """/></td>"
        If TaxEnabled = True Then
            'add vat view only
            item = item & "<td style=""width:80px;"" class=""" & Cls & """>" & Math.Round(curVAT, byteCurrencyRound, MidpointRounding.AwayFromZero) & "</td>"
        End If
        'add buttons
        item = item & "<td class=""text-nowrap"">" & allButtons & "</td>"
        'close row
        item = item & "</tr>"

        Return item
    End Function
#End Region

    Public Function createDosePrint(ByVal lngTransaction As Long, ByVal strItem As String, ByVal dateExpiry As Date) As String
        Dim html As String = ""
        Dim body As String = ""
        Dim PageCount As Integer = 0

        Select Case ByteLanguage
            Case 2
                DataLang = "Ar"
            Case Else
                DataLang = "En"
        End Select
        Try
            html = html & "<!DOCTYPE html><html xmlns=""http://www.w3.org/1999/xhtml""><head><title></title><style type=""text/css"">table{width: 100%;font-size:9px;}.right{text-align: right;}.left{text-align: left;}.center{text-align: center;}.half{width: 50%;}.fit{height: 25px;}</style></head><body>"

            Dim dsTrans, dsDose, dsItem As DataSet
            Dim strItemEn, strItemAr, DoseEn, DoseAr, Moredetails, Notes As String
            dsTrans = dcl.GetDS("SELECT ST.lngTransaction, ST.lngPatient, P.bSex, P.dateBirth, RTRIM(LTRIM(ISNULL(P.strFirstEn,'') + ' ') + LTRIM(ISNULL(P.strSecondEn,'') + ' ') + LTRIM(ISNULL(P.strThirdEn ,'') + ' ') + LTRIM(ISNULL(P.strLastEn,''))) AS PatientNameEn, RTRIM(LTRIM(ISNULL(P.strFirstAr,'') + ' ') + LTRIM(ISNULL(P.strSecondAr,'') + ' ') + LTRIM(ISNULL(P.strThirdAr ,'') + ' ') + LTRIM(ISNULL(P.strLastAr,''))) AS PatientNameAr, ST.dateEntry, C1.lngContact, C1.strContactEn, ST.strReference, STA.strCreatedBy AS UserName FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact WHERE ST.lngTransaction=" & lngTransaction)
            If dsTrans.Tables(0).Rows.Count > 0 Then
                Dim counter As Integer = 0
                Dim Sex As Char = "M"
                Dim Age As Integer = 0
                Dim TDate As Date = Today
                If Not (IsDBNull(dsTrans.Tables(0).Rows(0).Item("dateBirth"))) Then Age = DateDiff(DateInterval.Year, dsTrans.Tables(0).Rows(0).Item("dateBirth"), Today)
                If Not (IsDBNull(dsTrans.Tables(0).Rows(0).Item("dateEntry"))) Then TDate = dsTrans.Tables(0).Rows(0).Item("dateEntry")
                If dsTrans.Tables(0).Rows(0).Item("bSex") = 1 Then Sex = "M" Else Sex = "F"
                dsDose = dcl.GetDS("SELECT SI.strItemEn, SI.strItemAr, HTP.Moredetails, HTP.Notes, ISNULL(PQM.strQtyEn,'') + ' ' + ISNULL(PDM.strDoseEn,'') + ' ' + ISNULL(PRM.strRepetitionEn,'') + ' ' + ISNULL(PTM.strTimeEn,'') + ' ' + ISNULL(PPM.strPeriodEn,'') AS DoseEn, ISNULL(PQM.strQtyAr,'') + ' ' + ISNULL(PDM.strDoseAr,'') + ' ' + ISNULL(PRM.strRepetitionAr,'') + ' ' + ISNULL(PTM.strTimeAr,'') + ' ' + ISNULL(PPM.strPeriodAr,'') AS DoseAr FROM Stock_Trans AS ST INNER JOIN Hw_Treatments_Pharmacy AS HTP ON ST.strReference=HTP.strReference AND ST.lngPatient=HTP.lngPatient INNER JOIN Stock_Items AS SI ON HTP.strItem=SI.strItem LEFT JOIN Hw_Medicines_Approval AS HDA ON ST.lngPatient=HDA.lngPatient AND ST.strReference=HDA.strReference AND HTP.strItem=HDA.strItem LEFT JOIN Ph_Qty_Med AS PQM ON PQM.byteQty = SUBSTRING(HTP.strDose,1,2) LEFT JOIN Ph_Dose_Med AS PDM ON PDM.byteDose = SUBSTRING(HTP.strDose,3,3) LEFT JOIN Ph_Repetition_Med AS PRM ON PRM.byteRepetition = SUBSTRING(HTP.strDose,6,2) LEFT JOIN Ph_Time_Med AS PTM ON PTM.byteTime = SUBSTRING(HTP.strDose,8,2) LEFT JOIN Ph_Period_Med AS PPM ON PPM.bytePeriod = SUBSTRING(HTP.strDose,10,2) WHERE ST.lngTransaction=" & lngTransaction & " AND SI.strItem='" & strItem & "'")
                If dsDose.Tables(0).Rows.Count > 0 Then
                    strItemEn = dsDose.Tables(0).Rows(0).Item("strItemEn")
                    strItemAr = dsDose.Tables(0).Rows(0).Item("strItemAr")
                    DoseEn = dsDose.Tables(0).Rows(0).Item("DoseEn")
                    DoseAr = dsDose.Tables(0).Rows(0).Item("DoseAr")
                    Notes = dsDose.Tables(0).Rows(0).Item("Notes")
                    Moredetails = dsDose.Tables(0).Rows(0).Item("Moredetails")
                Else
                    dsItem = dcl.GetDS("SELECT SI.strItemEn, SI.strItemAr FROM  Stock_Items AS SI WHERE SI.strItem='" & strItem & "'")
                    If dsItem.Tables(0).Rows.Count > 0 Then
                        strItemEn = dsItem.Tables(0).Rows(0).Item("strItemEn")
                        strItemAr = dsItem.Tables(0).Rows(0).Item("strItemAr")
                    Else
                        strItemEn = ""
                        strItemAr = ""
                    End If
                    DoseEn = ""
                    DoseAr = ""
                    Notes = ""
                    Moredetails = ""
                End If
                If PageCount > 1 Then body = body & "<P style=""page-break-before: always"">"
                body = body & "<table>"
                body = body & "<tr><td class=""half left"">Khubar الخبر 8930044</td><td class=""half right"" style=""direction:rtl"">Jazirah Pharmacy صيدلية الجزيرة</td></tr>"
                body = body & "<tr><td class=""center"">" & dsTrans.Tables(0).Rows(0).Item("strContactEn") & "</td><td class=""center"">" & strUserName & " -- " & TDate.ToString("yyyy-MM-dd") & "</td></tr>"
                body = body & "<tr><td class=""half left bold"">" & dsTrans.Tables(0).Rows(0).Item("PatientNameEn") & "</td><td class=""half right bold"" style=""direction:rtl"">" & dsTrans.Tables(0).Rows(0).Item("PatientNameAr") & "</td></tr>"
                body = body & "<tr><td class=""half right"">Sex : " & Sex & "</td><td class=""half left"">Age : " & Age & "</td></tr>"
                body = body & "<tr><td class=""half left bold"">" & strItemEn & "</td><td class=""half right bold"">" & strItemAr & "</td></tr>"
                body = body & "<tr><td class=""half left"">How to use:</td><td class=""half right"" style=""direction:rtl"">طريقة الاستخدام:</td></tr>"
                body = body & "<tr><td class=""half left bold"">" & DoseEn & "</td><td class=""half right bold"" style=""direction:rtl"">" & DoseAr & "</td></tr>"
                body = body & "<tr><td colspan=""2"" class=""center"" >EXP DATE : " & CDate(dateExpiry).ToString("MM/yyyy") & " تاريخ الانتهاء:</td></tr>"
                body = body & "<tr><td class=""half left"">Note</td><td class=""half right"" style=""direction:rtl"">ملاحظات</td></tr>"
                body = body & "<tr><td colspan=""2"" class=""center"">" & Moredetails & " " & Notes & "</td></tr>"
                body = body & "</table>"

                Return html & body & "</body></html>"
            Else
                Return ""
            End If
        Catch ex As Exception
            'divBody.InnerHtml = "<script type=""text/javascript"">window.close();</script>"
            Return ""
        End Try
    End Function

    Public Function confirmChangeInvoice(ByVal curTotal1 As Decimal, ByVal curTotal2 As Decimal, ByVal curPercent As Decimal, ByVal bPercent As Boolean, ByVal curMaxDeduction As Decimal, ByVal curTotalClinicInvoices As Decimal, ByVal curTotalPharmacyInvoices As Decimal, ByVal NextFunction As String) As Decimal
        Dim curCoverd1 As Decimal = curTotal1 * (curPercent / 100)
        Dim curCoverage1, curDeduction1 As Decimal
        If bPercent = True Then
            If curCoverd1 = 0 Then
                curCoverage1 = 0
            Else
                If (curTotalClinicInvoices + curTotalPharmacyInvoices + curCoverd1) > curMaxDeduction Then
                    curCoverage1 = curMaxDeduction - (curTotalClinicInvoices + curTotalPharmacyInvoices)
                Else
                    curCoverage1 = curCoverd1
                End If
            End If
        Else
            curCoverage1 = 0
        End If
        curDeduction1 = curTotal1 - curCoverage1
        Dim curCoverd2 As Decimal = curTotal2 * (curPercent / 100)
        Dim curCoverage2, curDeduction2 As Decimal
        If bPercent = True Then
            If curCoverd2 = 0 Then
                curCoverage2 = 0
            Else
                If (curTotalClinicInvoices + curTotalPharmacyInvoices + curCoverd2) > curMaxDeduction Then
                    curCoverage2 = curMaxDeduction - (curTotalClinicInvoices + curTotalPharmacyInvoices)
                Else
                    curCoverage2 = curCoverd2
                End If
            End If
        Else
            curCoverage2 = 0
        End If
        curDeduction2 = curTotal2 - curCoverage2

        Dim str As String = ""
        str = str & "<div class=""row""><div class=""col-md-3""></div><div class=""col-md-3"" style=""text-align:center""><label >total</label></div><div class=""col-md-3"" style=""text-align:center""><label >credit</label></div><div class=""col-md-3"" style=""text-align:center""><label >cash</label></div></div>"
        str = str & "<div class=""row""><div class=""col-md-3""><label>currentInvoice</label></div><div class=""col-md-3""><input type=""number"" style=""width:120px""/></div><div class=""col-md-3""><input type=""number"" style=""width:120px""/></div><div class=""col-md-3""><input type=""number"" style=""width:120px""/></div></div>"
        str = str & "<div class=""row""><div class=""col-md-3""><label>newInvoice</label></div><div class=""col-md-3""><input type=""number"" style=""width:120px""/></div><div class=""col-md-3""><input type=""number"" style=""width:120px""/></div><div class=""col-md-3""><input type=""number"" style=""width:120px""/></div></div>"
        str = str & "<div class=""row""><div class=""col-md-9""><label>you have to return to patient</label></div><div class=""col-md-3""><input type=""number"" style=""width:120px""/></div></div></div>"

        Dim btns As String = "<button type=""button"" class=""btn btn-warning ml-1"" data-dismiss=""modal""><i class=""icon-cross2""></i> Cancel</button>"

        Dim mdl As New Share.UI
        Return mdl.drawModal("Confirm", str, btns, Share.UI.ModalSize.Small)
    End Function

    Public Function getHeaderSubMenu(ByVal HeaderText As String, ByVal ViewAction As Boolean) As String
        Dim mnuShowPaitent, mnuShowOrder, mnuShowCoverage, mnuShowDeveloper, mnuChangeInvoiceType As String
        Select Case ByteLanguage
            Case 2
                ' Menus
                mnuShowPaitent = "عرض معلومات المريض"
                mnuShowOrder = "عرض عناصر الطلب"
                mnuShowCoverage = "عرض معلومات التغطية"
                mnuShowDeveloper = "عرض نافذة المطور"
                mnuChangeInvoiceType = "تغيير نوع الفاتورة"
            Case Else
                ' Menus
                mnuShowPaitent = "View Patient Information"
                mnuShowOrder = "View Order Items"
                mnuShowCoverage = "View Coverage Information"
                mnuShowDeveloper = "View Developer Window"
                mnuChangeInvoiceType = "Change Invoice Type"
        End Select
        Dim str As String = ""
        'str = "<div class=""text-md-center""><div class=""btn-group float-left""><button type=""button"" class=""btn btn-outline-primary btn-xs dropdown-toggle"" data-toggle=""dropdown"" aria-haspopup=""true"" aria-expanded=""false""><i class=""icon-info2""></i></button><div class=""dropdown-menu bg-grey bg-lighten-2""><a class=""dropdown-item"" href=""javascript:"" onclick=""javascript:showModal('viewPatient','{lngTransaction: ' + $('#trans' + curTab).val() + '}','#mdlBeta')""><i class=""icon-user""></i> " & mnuShowPaitent & "</a><a class=""dropdown-item"" href=""javascript:"" onclick=""javascript:showModal('viewOrder', '{TransNo: ' + $('#trans' + curTab).val() + ', ShowOnly: true}', '#mdlBeta');""><i class=""icon-ios-medkit""></i> " & mnuShowOrder & "</a><a class=""dropdown-item"" href=""javascript:"" onclick=""javascript:showModal('viewCoverage','{lngTransaction: ' + $('#trans' + curTab).val() + '}','#mdlBeta')""><i class=""icon-h-square""></i> " & mnuShowCoverage & "</a><a class=""dropdown-item"" href=""javascript:"" onclick=""javascript:showModal('viewInfo','{lngTransaction: ' + $('#trans' + curTab).val() + '}','#mdlBeta')""><i class=""icon-code2""></i> " & mnuShowDeveloper & "</a></div></div>" & HeaderText & "</div>"
        str = str & "<div class=""text-md-center"">"
        str = str & "<div class=""btn-group float-left"">"
        str = str & "<div class=""btn-group"">"
        str = str & "<button type=""button"" class=""btn btn-outline-primary btn-xs dropdown-toggle"" data-toggle=""dropdown"" aria-haspopup=""true"" aria-expanded=""false""><i class=""icon-info2""></i> Info</button>"
        str = str & "<div class=""dropdown-menu bg-grey bg-lighten-2""><a class=""dropdown-item"" href=""javascript:"" onclick=""javascript:showModal('viewPatient','{lngTransaction: ' + $('#trans' + curTab).val() + '}','#mdlBeta')""><i class=""icon-user""></i> " & mnuShowPaitent & "</a><a class=""dropdown-item"" href=""javascript:"" onclick=""javascript:showModal('viewOrder', '{TransNo: ' + $('#trans' + curTab).val() + ', ShowOnly: true, ToPrepare: false}', '#mdlBeta');""><i class=""icon-ios-medkit""></i> " & mnuShowOrder & "</a><a class=""dropdown-item"" href=""javascript:"" onclick=""javascript:showModal('viewCoverage','{lngTransaction: ' + $('#trans' + curTab).val() + '}','#mdlBeta')""><i class=""icon-h-square""></i> " & mnuShowCoverage & "</a><a class=""dropdown-item"" href=""javascript:"" onclick=""javascript:showModal('viewInfo','{lngTransaction: ' + $('#trans' + curTab).val() + '}','#mdlBeta')""><i class=""icon-code2""></i> " & mnuShowDeveloper & "</a></div>"
        str = str & "</div>"
        If ViewAction = True Then
            str = str & "<div class=""btn-group"">"
            str = str & "<button type=""button"" class=""btn btn-outline-primary btn-xs dropdown-toggle"" data-toggle=""dropdown"" aria-haspopup=""true"" aria-expanded=""false""><i class=""icon-gear""></i> Action</button>"
            str = str & "<div class=""dropdown-menu bg-grey bg-lighten-2""><a class=""dropdown-item"" href=""javascript:"" onclick=""javascript:changeInvoiceType($('#trans' + curTab).val())""><i class=""icon-paste""></i> " & mnuChangeInvoiceType & "</a></div>"
            str = str & "</div>"
        End If
        str = str & "</div>"
        str = str & HeaderText
        str = str & "</div>"
        Return str
    End Function


#Region "Process"
    Public Function getItemApprovalStatus(ByVal strItem As String, ByVal lngPatient As Long, ByVal dateTransaction As Date, ByVal strReference As String) As Integer
        Dim ds As DataSet
        Dim intVisit As Integer
        Dim returnValue As Integer
        Dim bApproval As Boolean

        ' => Get intVisit
        ds = dcl.GetDS("SELECT intVisit,bApproval FROM Hw_Treatments_Pharmacy WHERE strReference='" & strReference & "' AND dateTransaction='" & dateTransaction.ToString("yyyy-MM-dd") & "' AND lngPatient=" & lngPatient & " AND strItem='" & strItem & "'")
        If ds.Tables(0).Rows.Count > 0 Then
            intVisit = ds.Tables(0).Rows(0).Item("intVisit")
            bApproval = ds.Tables(0).Rows(0).Item("bApproval")

            If bApproval = True Then
                ' => Get Approvals
                ds = dcl.GetDS("SELECT * FROM Hw_Medicines_Approval Where intVisit=" & intVisit & " AND lngPatient=" & lngPatient & " AND strItem='" & strItem & "'")
                ' => Check Status (Approved  or Rejected)
                If ds.Tables(0).Rows.Count > 0 Then
                    If Not (IsDBNull(ds.Tables(0).Rows(0).Item("strRejectedBy")) And IsDBNull(ds.Tables(0).Rows(0).Item("strApprovedBy"))) Then
                        'TODO: assign here if rejected or approved
                        If ds.Tables(0).Rows(0).Item("strApprovedBy").ToString <> "" Then
                            returnValue = 1
                        Else
                            If ds.Tables(0).Rows(0).Item("strRejectedBy").ToString <> "" Then
                                returnValue = 0
                                'CheckItemCoverage()
                            Else
                                returnValue = 1
                            End If
                        End If
                    Else
                        returnValue = -3 '"Err:not approved or rejected yet"
                    End If
                Else
                    'returnValue = -2 '"Err:no record"
                    returnValue = 2 'Change to cash
                End If
            Else
                'Already Approved (bApproval=false means not required approvals)
                returnValue = 1
            End If
        Else
            'returnValue = -1 '"Err:intVisit not found"
            returnValue = 2 'Change to cash
        End If
        Return returnValue
    End Function

    Public Function CalculateCoverage(ByVal lngTransaction As Long, ByVal strItem As String, ByVal curUnitPrice As Decimal, ByVal curQuantity As Decimal, ByVal Cov As Decimal, ByVal lngContract As Long, ByVal byteScheme As Byte, ByVal bytePriceType As Integer, ByVal bytePrimaryDep As Byte, ByVal lngPatient As Long, ByVal lngSalesman As Long, ByVal dateTransaction As Date, ByVal intGroup As Integer) As Decimal
        Dim curCoverage As Decimal
        Dim ds, dsCovPlans, dsGroup, dsPHinvs As DataSet
        Dim curUnitPrice1, curCoverage1, curCoverage2, curCoverage3 As Decimal
        ' ???
        Dim bCash As Boolean = False
        Dim strBatch As String = ""
        Dim curUnitNetPrice, curDiscount, curCovAmount As Decimal

        'Get required data

        ' === > Check Coverage <===
        dsCovPlans = dcl.GetDS("SELECT ICP.curCoveragePercent,ICP.curCoverageValue, ICP.curYearlyLimit, ICP.curMonthlyLimit, IMI.strItem FROM Ins_Med_Items AS IMI INNER JOIN Ins_Coverage_Plans AS ICP ON IMI.lngMed = ICP.lngMed WHERE ICP.byteScope=2 AND ICP.lngContract=" & lngContract & " AND ICP.byteScheme=" & byteScheme & " AND IMI.strItem='" & strItem & "'")
        If dsCovPlans.Tables(0).Rows.Count > 0 Then
            If dsCovPlans.Tables(0).Rows(0).Item("curCoveragePercent").ToString <> "" Then
                curCoverage = curUnitPrice * curQuantity * dsCovPlans.Tables(0).Rows(0).Item("curCoveragePercent") / 100
            ElseIf dsCovPlans.Tables(0).Rows(0).Item("curCoverageValue").ToString <> "" Then
                curCoverage = dsCovPlans.Tables(0).Rows(0).Item("curCoverageValue")
            Else
                dsCovPlans = dcl.GetDS("SELECT curDeductionValueP, curDeductionPercentP, curDeductionValueD, curDeductionPercentD FROM Ins_Coverage WHERE byteScope=2 AND lngContract=" & lngContract & " AND byteScheme=" & byteScheme)
                If dsCovPlans.Tables(0).Rows.Count > 0 Then
                    If bytePrimaryDep = 1 Then ' Primary
                        If dsCovPlans.Tables(0).Rows(0).Item("curDeductionPercentP").ToString <> "" Then
                            If dsCovPlans.Tables(0).Rows(0).Item("curDeductionPercentP") = 100 Then
                                curCoverage = (curUnitPrice * curQuantity) * dsCovPlans.Tables(0).Rows(0).Item("curDeductionPercentP") / 100
                            Else
                                curCoverage = (curUnitPrice * curQuantity) * dsCovPlans.Tables(0).Rows(0).Item("curDeductionPercentP") / 100
                            End If
                        ElseIf dsCovPlans.Tables(0).Rows(0).Item("curDeductionValueP").ToString <> "" Then
                            curCoverage = dsCovPlans.Tables(0).Rows(0).Item("curDeductionValueP")
                        Else
                            curCoverage = 0
                        End If
                    Else ' Dependent
                        If dsCovPlans.Tables(0).Rows(0).Item("curDeductionPercentD").ToString <> "" Then
                            If dsCovPlans.Tables(0).Rows(0).Item("curDeductionPercentD") = 100 Then
                                curCoverage = (curUnitPrice * curQuantity) * dsCovPlans.Tables(0).Rows(0).Item("curDeductionPercentD") / 100
                            Else
                                curCoverage = (curUnitPrice * curQuantity) * dsCovPlans.Tables(0).Rows(0).Item("curDeductionPercentD") / 100
                            End If
                        ElseIf dsCovPlans.Tables(0).Rows(0).Item("curDeductionValueD").ToString <> "" Then
                            curCoverage = dsCovPlans.Tables(0).Rows(0).Item("curDeductionValueD")
                        Else
                            curCoverage = 0
                        End If
                    End If
                Else
                    curCoverage = 0
                End If
            End If
        End If

        ' === > Check Deduction Limit <===
        If bCash = False Then
            Dim MaxP As Decimal = 0
            Dim CICov As Decimal = 0
            Dim MICov As Decimal = 0
            '1> Get Contract & Scheme Info (Case Limit - Year Limit - Max Deduction)
            dsGroup = dcl.GetDS("SELECT curYearLimitP, curCaseLimitP,curDeductionMaxP FROM Ins_Coverage WHERE byteScope=2 AND lngContract=" & lngContract & " AND byteScheme=" & byteScheme)
            If dsGroup.Tables(0).Rows.Count > 0 Then
                MaxP = CDec("0" & dsGroup.Tables(0).Rows(0).Item("curDeductionMaxP").ToString)
                '2> Get Clinic Invoices (Sum of Amount - Sum of Coverage)
                ds = dcl.GetDS("SELECT Sum(Amount) AS SumOfAmount, lngSalesman, Sum(curCoverage) AS Coverage FROM Clinic_Invoices WHERE dateTransaction Between '" & DateAdd(DateInterval.Day, (DaysToCalculateMedicalInvoices * -1), dateTransaction).ToString("yyyy-MM-dd") & "' And '" & dateTransaction.ToString("yyyy-MM-dd") & "' AND lngPatient=" & lngPatient & " AND lngSalesMan=" & lngSalesman & " GROUP BY lngSalesman")
                If ds.Tables(0).Rows.Count > 0 Then
                    If IsDBNull(ds.Tables(0).Rows(0).Item("Coverage")) Then CICov = 0 Else CICov = ds.Tables(0).Rows(0).Item("Coverage")
                    ''CICov = CDec("0" & ds.Tables(0).Rows(0).Item("Coverage").ToString)
                    '3> Get Medicins Invoices  (Sum of Amount - Sum of Coverage) ===== MUST NOT INCLUDE CURRENT TRANSACTION ======
                    dsPHinvs = dcl.GetDS("SELECT SUM(SXI.curUnitPrice) AS Amount, SUM(SXI.curCoverage) AS Cov FROM Stock_Trans AS ST INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink WHERE dateTransaction BETWEEN '" & DateAdd(DateInterval.Day, (DaysToCalculateMedicineInvoices * -1), dateTransaction).ToString("yyyy-MM-dd") & "' AND '" & dateTransaction.ToString("yyyy-MM-dd") & "' AND lngPatient=" & lngPatient & " AND lngSalesMan=" & lngSalesman & " AND (ST.byteBase = 40 OR ST.byteBase = 50) AND ST.byteStatus > 0 AND ST.lngTransaction<>" & lngTransaction & " GROUP BY ST.lngSalesman")
                    If dsPHinvs.Tables(0).Rows.Count > 0 Then
                        If IsDBNull(dsPHinvs.Tables(0).Rows(0).Item("Cov")) Then MICov = 0 Else MICov = dsPHinvs.Tables(0).Rows(0).Item("Cov")
                        ''MICov = CDec("0" & dsPHinvs.Tables(0).Rows(0).Item("Cov").ToString)
                    End If
                    '4> Check if reach MAX or Limit
                    If CICov + MICov + Cov + curCoverage > MaxP Then
                        '5> Split coverage or cover all
                        'This is my code
                        'curCoverage = curCoverage - ((CICov + MICov + Cov + curCoverage) - MaxP)
                        'If curCoverage < 0 Then curCoverage = 0


                        'curCoverage = (CICov + MICon + Cov) - MaxP
                        'If curCoverage < 0 Then curCoverage = 0

                        ' '' '' ''MsgBox(IIf(gblanguage, "ÍÏ ÇáÇÞÊØÇÚ ááãÑíÖ åæ: " & rsGroup!curDeductionMaxP & " ÑíÇá", "Max Deduction limit for this Patient is " & rsGroup!curDeductionMaxP & " Ryials") & vbCrLf & IIf(gblanguage, "æãÌãæÚ ãÇ ÏÝÚå åæ: " & Nz(rsCinvs!Coverage, 0) + [Forms]![Ph_Prepearing_Med].Form![curCovAmount] + [Cov] + rsPHinvs!Cov & " ÑíÇá", "?And the total is " & Nz(rsCinvs!Coverage, 0) + [Forms]![Ph_Prepearing_Med].Form![curCovAmount] + [Cov] + rsPHinvs!Cov & " Ryials"), vbCritical)
                        '' '' ''curUnitPrice1 = (curUnitPrice * curQuantity) + (CDec("0" & ds.Tables(0).Rows(0).Item("Coverage").ToString) + Cov - dsGroup.Tables(0).Rows(0).Item("curDeductionMaxP"))
                        '' '' ''curCoverage2 = Cov
                        '' '' ''curCoverage3 = Cov - ((curCoverage2 + CDec("0" & dsPHinvs.Tables(0).Rows(0).Item("Cov").ToString))) - (dsGroup.Tables(0).Rows(0).Item("curDeductionMaxP") - CDec("0" & ds.Tables(0).Rows(0).Item("Coverage").ToString))
                        '' '' ''curCoverage1 = curUnitPrice - (curCoverage3 - Cov)
                        '' '' ''If curCoverage > curCoverage3 Then
                        '' '' ''    curCoverage = IIf(curCoverage3 < 0, 0, curCoverage3)
                        '' '' ''    If curCoverage = 0 Then
                        '' '' ''        curUnitNetPrice = curUnitPrice - curCoverage2
                        '' '' ''        ds = dcl.GetDS("SELECT ABS(curPercent) AS curPercent FROM Stock_Price_Groups WHERE bytePriceType=" & bytePriceType & " AND intGroup=" & intGroup)
                        '' '' ''        If ds.Tables(0).Rows.Count > 0 Then curDiscount = CDec("0" & ds.Tables(0).Rows(0).Item("curPercent").ToString) Else curDiscount = 0
                        '' '' ''    End If
                        '' '' ''    curCoverage2 = curCoverage2 - IIf(curCoverage3 < 0, 0, curCoverage3)
                        '' '' ''    'MsgBox(IIf(gblanguage, "Êã ÊÍæíá ÇáÝÇÆÖ Úä ÍÏ ÇáÇÞÊØÇÚ Úáì ÍÓÇÈ ÇáÔÑßÉ", "Flow Over Deduction Was Transfered To Company Account"), vbApplicationModal, "Al-Jazira Clinic")
                        '' '' ''    curCoverage = IIf(curCoverage3 < 0, 0, curCoverage3)
                        '' '' ''    curCoverage2 = curCoverage2 - IIf(curCoverage3 < 0, 0, curCoverage3)
                        '' '' ''End If
                    End If
                Else
                    curCoverage = 0 'Err:There Is No Clinic Invoice For THis PATIENT Please Go to Reception
                End If
            Else
                If CDec("0" & ds.Tables(0).Rows(0).Item("Coverage").ToString) + (Cov + curCovAmount) > dsGroup.Tables(0).Rows(0).Item("curDeductionMaxP") Then
                    If strBatch = "" Then
                        'MsgBox(IIf(gblanguage, "ÍÏ ÇáÇÞÊØÇÚ ááãÑíÖ åæ : " & rsGroup!curDeductionMaxP & " ÑíÇá", "Max Deduction limit for this Patient is " & rsGroup!curDeductionMaxP & " Ryials") & vbCrLf & IIf(gblanguage, "æãÌãæÚ ãÇ ÇÎÐå åæ " & Nz(rsCinvs!Coverage, 0) + [Forms]![Ph_Prepearing_Med].Form![curCovAmount] + [Cov] & " ÑíÇá", "?And the total is " & Nz(rsCinvs!Coverage, 0) + [Forms]![Ph_Prepearing_Med].Form![curCovAmount] + [Cov] & " Ryials"), vbCritical)
                        curUnitPrice1 = curUnitPrice * curQuantity + (CDec("0" & ds.Tables(0).Rows(0).Item("Coverage").ToString) + Cov - dsGroup.Tables(0).Rows(0).Item("curDeductionMaxP"))
                        curCoverage2 = Cov
                        curCoverage3 = Cov - ((curCoverage2) - (dsGroup.Tables(0).Rows(0).Item("curDeductionMaxP") - CDec("0" & ds.Tables(0).Rows(0).Item("Coverage").ToString)))
                        If curCoverage3 <= 0 Then
                            curCoverage = IIf(curCoverage3 < 0, 0, curCoverage3)
                            GoTo Label1
                        End If
                        curCoverage1 = curUnitPrice - (curCoverage2 - Cov)
                        If curCoverage <= curCoverage3 Then Exit Function
                        curCoverage = IIf(curCoverage3 < 0, 0, curCoverage3)
                        If curCoverage = 0 Then
                            curUnitNetPrice = curUnitPrice - curCoverage2
                            ds = dcl.GetDS("SELECT ABS(curPercent) AS curPercent FROM Stock_Price_Groups WHERE bytePriceType=" & bytePriceType & " AND intGroup=" & intGroup)
                            curDiscount = CDec("0" & ds.Tables(0).Rows(0).Item("curPercent").ToString)
                        End If
Label1:
                        curCoverage2 = curCoverage2 - IIf(curCoverage3 < 0, 0, curCoverage3)
                        'MsgBox(IIf(gblanguage, "Êã ÊÍæíá ÇáÝÇÆÖ ãä ÇáÇÞÊØÇÚ Úáì ÍÓÇÈ ÇáÔÑßÉ", "Flow Over Deduction Was Transfered To Company Account"), vbApplicationModal, "Al-Jazira Clinic")
                        curCoverage = IIf(curCoverage3 < 0, 0, curCoverage3)
                        curCoverage2 = curCoverage2 - IIf(curCoverage3 < 0, 0, curCoverage3)
                    End If
                End If
            End If
        End If
        Return curCoverage
    End Function

    ''' <summary>
    ''' Check the item not exceeded the <para /> limitation of coverage of insurance company
    ''' </summary>
    ''' <remarks>
    ''' Created by Faisal Al-Aseery 08-2019 Version: 1.0
    ''' </remarks>
    ''' <returns>Nothing if not exceeded, Otherwise will return Error as string</returns>
    ''' <param name="lngTransaction">The transaction ID.</param>
    ''' <param name="strItem">The item ID.</param>
    ''' <param name="curBasePrice">The base price of the item</param>
    Public Function CheckItemCoverage(ByVal lngTransaction As Long, ByVal lngContract As Long, ByVal byteScheme As Byte, ByVal strItem As String, ByVal curBasePrice As Decimal, ByVal curQuantity As Decimal, ByVal dateTransaction As Date, ByVal lngPatient As Long, ByVal curBasePriceTotal As Decimal) As String

        Dim ds, dsGroup, dsTrans As DataSet
        Dim strBatch As String = ""

        ' Get Information

        '1> Check in (Ins_Coverage_Items) Table for ????
        ds = dcl.GetDS("SELECT bAuthRequired, intWaitDays, curMaxQty, bVisits, bCovered FROM Ins_Coverage_Items WHERE byteScope=2 AND lngContract=" & lngContract & " AND byteScheme=" & byteScheme & " AND strItem='" & strItem & "'")
        If ds.Tables(0).Rows.Count = 0 Then ' No record
            '>2 Check in contracts for the service
            dsGroup = dcl.GetDS("SELECT ICG.bCovered, ICG.bAutorization FROM Ins_Coverage_Groups AS ICG INNER JOIN Ins_Med_Items AS IMI ON ICG.lngGroup = IMI.lngGroup AND ICG.lngMed = IMI.lngMed WHERE ICG.lngContract=" & lngContract & " AND ICG.byteScheme=" & byteScheme & " AND ICG.byteScope=2 AND IMI.strItem='" & strItem & "'")
            If dsGroup.Tables(0).Rows.Count = 0 Then ' No record
                Return "Err: This service is not included the contracts."
            Else
                '3> Check if is coverd
                If dsGroup.Tables(0).Rows(0).Item("bCovered") = False Then
                    Return "Err: This service is not covered"
                Else
                    '4> Check if authorized
                    If dsGroup.Tables(0).Rows(0).Item("bAutorization") = True Then
                        Return "Err: Autherization required for this service"
                    End If
                    '5> Check if (Yearly limit) Exist => By Item
                    dsGroup = dcl.GetDS("SELECT curYearlyLimit, ICP.lngMed FROM Ins_Med_Items AS IMI INNER JOIN Ins_Coverage_Plans AS ICP ON IMI.lngMed = ICP.lngMed WHERE ICP.byteScope=2 AND ICP.lngContract=" & lngContract & " AND ICP.byteScheme=" & byteScheme & " AND strItem = '" & strItem & "'")
                    If dsGroup.Tables(0).Rows(0).Item("curYearlyLimit").ToString <> "" Then
                        '5.1> Check if (Yearly Amount) is Greeter Than (Yearly Limit) ==== MUST NOT INCLUDE CURRENT TRANSACTION ====
                        dsTrans = dcl.GetDS("SELECT SUM(curUnitPrice*curQuantity) AS curAmount FROM Stock_Trans AS ST INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink INNER JOIN Ins_Med_Items AS IMI ON SXI.strItem = IMI.strItem WHERE byteStatus > 0 AND Year(dateTransaction)=" & Year(dateTransaction) & " AND lngMed= " & dsGroup.Tables(0).Rows(0).Item("lngMed") & " AND ST.lngPatient=" & lngPatient & " AND ST.lngTransaction <> " & lngTransaction)
                        If curBasePriceTotal + CDec("0" & dsTrans.Tables(0).Rows(0).Item("curAmount").ToString) + (curBasePrice * curQuantity) > dsGroup.Tables(0).Rows(0).Item("curYearLimitP") Then
                            Return "Err: Max yearly limit for this contract is " & Math.Round(dsGroup.Tables(0).Rows(0).Item("curYearLimitP"), byteCurrencyRound, MidpointRounding.AwayFromZero) & " Riyals"
                        End If
                    End If
                    '6> Check if (Yearly limit) Exist => By Contract
                    dsGroup = dcl.GetDS("SELECT curYearLimitP, curCaseLimitP,curDeductionMaxP,curMonthlyLimitP FROM Ins_Coverage WHERE byteScope=2 AND lngContract=" & lngContract & " AND byteScheme=" & byteScheme)
                    If dsGroup.Tables(0).Rows(0).Item("curYearLimitP").ToString <> "" Then
                        '6.1> Check if (Yearly Amount) is Greeter Than (Yearly Limit) ==== MUST NOT INCLUDE CURRENT TRANSACTION ====
                        dsTrans = dcl.GetDS("SELECT SUM(curUnitPrice*curQuantity) AS curAmount FROM Stock_Trans AS ST INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink INNER JOIN Ins_Med_Items AS IMI ON SXI.strItem = IMI.strItem WHERE byteStatus > 0 AND Year(dateTransaction)=" & Year(dateTransaction) & " AND ST.lngPatient=" & lngPatient & " AND ST.lngTransaction <> " & lngTransaction)
                        If curBasePriceTotal + CDec("0" & dsTrans.Tables(0).Rows(0).Item("curAmount").ToString) + (curBasePrice * curQuantity) > dsGroup.Tables(0).Rows(0).Item("curYearLimitP") Then
                            Return "Err: Max yearly limit for this contract is " & Math.Round(dsGroup.Tables(0).Rows(0).Item("curYearLimitP"), byteCurrencyRound, MidpointRounding.AwayFromZero) & " Riyals"
                        End If
                    End If
                    '7> Check if (Monthly Limit) Exist
                    If dsGroup.Tables(0).Rows.Count > 0 Then
                        If dsGroup.Tables(0).Rows(0).Item("curMonthlyLimitP").ToString <> "" Then
                            '7.1> Check if (Monthly Amount) is Greeter Than (Monthly Limit) ==== MUST NOT INCLUDE CURRENT TRANSACTION ====
                            dsTrans = dcl.GetDS("SELECT SUM(curUnitPrice*curQuantity) AS curAmount FROM Stock_Trans AS ST INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink INNER JOIN Ins_Med_Items AS IMI ON SXI.strItem = IMI.strItem WHERE byteStatus > 0 AND dateTransaction BETWEEN '" & DateAdd(DateInterval.Month, 1, dateTransaction).ToString("yyyy-MM-dd") & "' AND '" & dateTransaction.ToString("yyyy-MM-dd") & "' AND ST.lngPatient=" & lngPatient & " AND ST.lngTransaction <> " & lngTransaction)
                            If curBasePriceTotal + CDec("0" & dsTrans.Tables(0).Rows(0).Item("curAmount").ToString) + (curBasePrice * curQuantity) > dsGroup.Tables(0).Rows(0).Item("curMonthlyLimitP") Then
                                Return "Err: Max yearly limit for this contract is " & Math.Round(dsGroup.Tables(0).Rows(0).Item("curMonthlyLimitP"), byteCurrencyRound, MidpointRounding.AwayFromZero) & " Riyals"
                            End If
                        End If
                    End If
                    '8> Check if (Case Limit) Exist
                    If dsGroup.Tables(0).Rows.Count > 0 Then
                        If dsGroup.Tables(0).Rows(0).Item("curCaseLimitP").ToString <> "" Then
                            '8.1> Check if (Current Amount) is Greeter Than (Case Limit) ==== MUST NOT INCLUDE CURRENT TRANSACTION ====
                            dsTrans = dcl.GetDS("SELECT SUM(curUnitPrice*curQuantity) AS curAmount FROM Stock_Trans AS ST INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink INNER JOIN Ins_Med_Items AS IMI ON SXI.strItem = IMI.strItem WHERE byteStatus > 0 AND dateTransaction = '" & dateTransaction.ToString("yyyy-MM-dd") & "' AND ST.lngPatient=" & lngPatient & " AND ST.lngTransaction <> " & lngTransaction)
                            Dim total As Decimal = CDec("0" & dsTrans.Tables(0).Rows(0).Item("curAmount").ToString)
                            If curBasePriceTotal + CDec("0" & dsTrans.Tables(0).Rows(0).Item("curAmount").ToString) + (curBasePrice * curQuantity) > dsGroup.Tables(0).Rows(0).Item("curCaseLimitP") Then
                                Return "Err: Max limit for the case is " & Math.Round(dsGroup.Tables(0).Rows(0).Item("curCaseLimitP"), byteCurrencyRound, MidpointRounding.AwayFromZero) & " Riyals"
                            End If
                        End If
                    End If
                End If
            End If
        Else
            '2> Check if is coverd
            If ds.Tables(0).Rows(0).Item("bCovered") = True Then
                '3> Check if authorized
                If ds.Tables(0).Rows(0).Item("bAutorization") = True Then
                    If strBatch = "" Then
                        '4> Check if still in waitting
                        If ds.Tables(0).Rows(0).Item("intWaitDays").ToString <> "" And ds.Tables(0).Rows(0).Item("intWaitDays").ToString <> "0" Then
                            '5> Check if (Max Quantity) Exist
                            If ds.Tables(0).Rows(0).Item("curMaxQty").ToString <> "" And ds.Tables(0).Rows(0).Item("curMaxQty").ToString <> "0" Then
                                '5.1> Check if (Current Amount) is Greeter Than (Max Quantity) ==== MUST NOT INCLUDE CURRENT TRANSACTION ====
                                dsTrans = dcl.GetDS("SELECT SUM(curBasePrice*curQuantity) AS curAmount, SUM(SXI.curQuantity) AS curQty FROM Stock_Trans AS ST INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink WHERE byteStatus > 0 AND Year(dateTransaction)=" & Year(dateTransaction) & " AND SXI.strItem='" & strItem & "' AND ST.lngPatient=" & lngPatient & " AND ST.lngTransaction <> " & lngTransaction)
                                If CDec("0" & dsTrans.Tables(0).Rows(0).Item("curQty").ToString) + curQuantity > 1 Then
                                    Return "Err: Max yearly limit for this service is " & Math.Round(ds.Tables(0).Rows(0).Item("curMaxQty"), byteCurrencyRound, MidpointRounding.AwayFromZero)
                                End If
                            End If
                            '6> Check if (Yearly limit) Exist => By Item
                            dsGroup = dcl.GetDS("SELECT curYearlyLimit, ICP.lngMed FROM Ins_Med_Items AS IMI INNER JOIN Ins_Coverage_Plans AS ICP ON IMI.lngMed = ICP.lngMed WHERE ICP.byteScope=2 AND ICP.lngContract=" & lngContract & " AND ICP.byteScheme=" & byteScheme & " AND strItem = '" & strItem & "'")
                            If dsGroup.Tables(0).Rows(0).Item("curYearlyLimit").ToString <> "" Then
                                '6.1> Check if (Yearly Amount) is Greeter Than (Yearly Limit) ==== MUST NOT INCLUDE CURRENT TRANSACTION ====
                                dsTrans = dcl.GetDS("SELECT SUM(curUnitPrice*curQuantity) AS curAmount FROM Stock_Trans AS ST INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink INNER JOIN Ins_Med_Items AS IMI ON SXI.strItem = IMI.strItem WHERE byteStatus > 0 AND Year(dateTransaction)=" & Year(dateTransaction) & " AND lngMed= " & dsGroup.Tables(0).Rows(0).Item("lngMed") & " AND ST.lngPatient=" & lngPatient & " AND ST.lngTransaction <> " & lngTransaction)
                                If curBasePriceTotal + CDec("0" & dsTrans.Tables(0).Rows(0).Item("curAmount").ToString) + (curBasePrice * curQuantity) > dsGroup.Tables(0).Rows(0).Item("curYearLimitP") Then
                                    Return "Err: Max yearly limit for this contract is " & Math.Round(dsGroup.Tables(0).Rows(0).Item("curYearLimitP"), byteCurrencyRound, MidpointRounding.AwayFromZero) & " Riyals"
                                End If
                            End If
                            '7> Check if (Yearly limit) Exist => By Contract
                            dsGroup = dcl.GetDS("SELECT curYearLimitP, curCaseLimitP,curDeductionMaxP,curMonthlyLimitP FROM Ins_Coverage WHERE byteScope=2 AND lngContract=" & lngContract & " AND byteScheme=" & byteScheme)
                            If dsGroup.Tables(0).Rows(0).Item("curYearLimitP").ToString <> "" Then
                                '7.1> Check if (Yearly Amount) is Greeter Than (Yearly Limit) ==== MUST NOT INCLUDE CURRENT TRANSACTION ====
                                dsTrans = dcl.GetDS("SELECT SUM(curUnitPrice*curQuantity) AS curAmount FROM Stock_Trans AS ST INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink INNER JOIN Ins_Med_Items AS IMI ON SXI.strItem = IMI.strItem WHERE byteStatus > 0 AND Year(dateTransaction)=" & Year(dateTransaction) & " AND ST.lngPatient=" & lngPatient & " AND ST.lngTransaction <> " & lngTransaction)
                                If curBasePriceTotal + CDec("0" & dsTrans.Tables(0).Rows(0).Item("curAmount").ToString) + (curBasePrice * curQuantity) > dsGroup.Tables(0).Rows(0).Item("curYearLimitP") Then
                                    Return "Err: Max yearly limit for this contract is " & Math.Round(dsGroup.Tables(0).Rows(0).Item("curYearLimitP"), byteCurrencyRound, MidpointRounding.AwayFromZero) & " Riyals"
                                End If
                            End If
                            '8> Check if (Case Limit) Exist
                            If dsGroup.Tables(0).Rows.Count > 0 Then
                                If dsGroup.Tables(0).Rows(0).Item("curCaseLimitP").ToString <> "" Then
                                    '8.1> Check if (Current Amount) is Greeter Than (Case Limit) ==== MUST NOT INCLUDE CURRENT TRANSACTION ====
                                    dsTrans = dcl.GetDS("SELECT SUM(curUnitPrice*curQuantity) AS curAmount FROM Stock_Trans AS ST INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink INNER JOIN Ins_Med_Items AS IMI ON SXI.strItem = IMI.strItem WHERE byteStatus > 0 AND dateTransaction = '" & dateTransaction.ToString("yyyy-MM-dd") & "' AND ST.lngPatient=" & lngPatient & " AND ST.lngTransaction <> " & lngTransaction)
                                    If curBasePriceTotal + CDec("0" & dsTrans.Tables(0).Rows(0).Item("curAmount").ToString) + (curBasePrice * curQuantity) > dsGroup.Tables(0).Rows(0).Item("curCaseLimitP") Then
                                        Return "Err: Max limit for the case is " & Math.Round(dsGroup.Tables(0).Rows(0).Item("curCaseLimitP"), byteCurrencyRound, MidpointRounding.AwayFromZero) & " Riyals"
                                    End If
                                End If
                            End If
                        Else
                            Return "Err: Service coverage starts after " & ds.Tables(0).Rows(0).Item("intWaitDays").ToString
                        End If
                    End If
                Else
                    Return "Err: Autherization required for this service"
                End If
            Else
                Return "Err: This service is not covered"
            End If
        End If
        Return True

        'notCoverage:

        '        If Me.OpenArgs = 1 Then
        '            If DLookup("bCreatCash", "Stock_Trans", "lngTransaction=" & Forms![PH_Medicine_Orders]![lngTransaction]) = True Then
        '                [lngTranslink] = CLng(DLookup("lngTransaction", "stock_Trans", "byteBase=50 AND bSubCash=1 AND dateTransaction=#" & Format(Forms![PH_Medicine_Orders]![dateTransaction], "mm\/dd\/yyyy") & "# AND strReference='" & Forms![PH_Medicine_Orders]![strReference] & "'"))
        '                [lngXlinkLink] = CLng(DLookup("lngXlink", "Stock_Xlink", "lngTransaction=" & [lngTranslink]))
        '                [lstCashOrder].RowSource = "SELECT XI.intEntryNumber, XI.strBarCode, XI.strItem, I.strItemEn, U.strUnitEn, Format(XI.curQuantity, '#,##0.00'), Format(XI.dateExpiry,'mm\/dd\/yyyy'), format(XI.curBasePrice, '#,##0.00'), format(NZ(XI.curDiscount,0), '#,##0.00'), Format(XI.curUnitPrice, '#,##0.00'), XI.strRemarks, XI.strDose1, XI.lngXlink FROM (((Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink = X.lngXlink) INNER JOIN Stock_Trans AS T ON X.lngTransaction = T.lngTransaction) INNER JOIN Stock_Items AS I ON XI.strItem = I.strItem) INNER JOIN Stock_Units AS U ON XI.byteUnit = U.byteUnit WHERE XI.lngXlink=" & [lngXlinkLink]
        '                [lstCashOrder].Requery()
        '            End If
        '        Else
        '            If Me.OpenArgs = 0 Then
        '                On Error Resume Next
        '                If DLookup("bCreatCash", "Stock_Trans", "lngTransaction=" & Forms![PH_Medicine_Cash]![lngTransaction]) = True Or DLookup("bCreatCash", "Stock_Trans", "lngTransaction=" & Forms![PH_Doctor_Medicine_Orders]![lngTransaction]) = True Then
        '                    [lngTranslink] = CLng(DLookup("lngTransaction", "stock_Trans", "byteBase=50 AND bSubCash=1 AND dateTransaction=#" & Format(Forms![PH_Medicine_Cash]![dateTransaction], "mm\/dd\/yyyy") & "# AND strReference='" & Forms![PH_Medicine_Cash]![strReference] & "'"))
        '                    [lngXlinkLink] = CLng(DLookup("lngXlink", "Stock_Xlink", "lngTransaction=" & [lngTranslink]))
        '                    [lstCashOrder].RowSource = "SELECT XI.intEntryNumber, XI.strBarCode, XI.strItem, I.strItemEn, U.strUnitEn, Format(XI.curQuantity, '#,##0.00'), Format(XI.dateExpiry,'mm\/dd\/yyyy'), format(XI.curBasePrice, '#,##0.00'), format(NZ(XI.curDiscount,0), '#,##0.00'), Format(XI.curUnitPrice, '#,##0.00'), XI.strRemarks, XI.strDose1, XI.lngXlink FROM (((Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink = X.lngXlink) INNER JOIN Stock_Trans AS T ON X.lngTransaction = T.lngTransaction) INNER JOIN Stock_Items AS I ON XI.strItem = I.strItem) INNER JOIN Stock_Units AS U ON XI.byteUnit = U.byteUnit WHERE XI.lngXlink=" & [lngXlinkLink]
        '                    [lstCashOrder].Requery()
        '                End If
        '            Else
        '                On Error Resume Next
        '                If DLookup("bCreatCash", "Stock_Trans", "lngTransaction=" & Forms![PH_Doctor_Medicine_Orders]![lngTransaction]) = True Then 'Or DLookup("bCreatCash", "Stock_Trans", "lngTransaction=" & Forms![PH_Doctor_Medicine_Orders]![lngTransaction]) = True Then
        '                    [lngTranslink] = CLng(DLookup("lngTransaction", "stock_Trans", "byteBase=50 AND bSubCash=1 AND dateTransaction=#" & Format(Forms![PH_Doctor_Medicine_Orders]![dateTransaction], "mm\/dd\/yyyy") & "# AND strReference='" & Forms![PH_Doctor_Medicine_Orders]![strReference] & "'"))
        '                    [lngXlinkLink] = CLng(DLookup("lngXlink", "Stock_Xlink", "lngTransaction=" & [lngTranslink]))
        '                    [lstCashOrder].RowSource = "SELECT XI.intEntryNumber, XI.strBarCode, XI.strItem, I.strItemEn, U.strUnitEn, Format(XI.curQuantity, '#,##0.00'), Format(XI.dateExpiry,'mm\/dd\/yyyy'), format(XI.curBasePrice, '#,##0.00'), format(NZ(XI.curDiscount,0), '#,##0.00'), Format(XI.curUnitPrice, '#,##0.00'), XI.strRemarks, XI.strDose1, XI.lngXlink FROM (((Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink = X.lngXlink) INNER JOIN Stock_Trans AS T ON X.lngTransaction = T.lngTransaction) INNER JOIN Stock_Items AS I ON XI.strItem = I.strItem) INNER JOIN Stock_Units AS U ON XI.byteUnit = U.byteUnit WHERE XI.lngXlink=" & [lngXlinkLink]
        '                    [lstCashOrder].Requery()
        '                End If
        '            End If
        '        End If

        '        If bApprCheck = True Then
        '            [strBatch] = 1
        '            Exit Function
        '        Else
        '            db = CurrentDb
        '            ws = CreateWorkspace("ODBC", "", "", dbUseODBC)
        '            con = ws.OpenConnection("ODBC", dbDriverNoPrompt, False, CurrentDb.TableDefs(0).Connect)

        '            rs = con.OpenRecordset("SELECT Max(Convert(int, strTransaction)) AS Last FROM Stock_Trans WHERE DatePart(Year, dateTransaction)=" & Year(Of Date)() & " AND byteTransType=38" & " AND byteBase=50", dbOpenSnapshot)
        '            strTrans = Nz(rs!Last, 0) + 1
        '            rs.Close()
        '            [strItem] = DLookup("strItem", "Stock_Items", "strItem=""" & Left([strBarCode], 5) & """")
        '            [dateExpiry] = DateSerial(Mid$([strBarCode], 8, 2), Mid$([strBarCode], 6, 2), 1)
        '            [byteUnit] = 1
        '            If DLookup("bControl", "cmn_Users", "strUserName='" & [Forms]![Cmn_Defaults].[strUserName] & "'") = True Then
        '                If MsgBox(IIf(gblanguage, "åá ÊæÏ ÇÖÇÝÉ ÇáÏæÇÁ Ýí ÇáÝÇÊæÑÉ ÇáäÞÏíÉ ¿ ", "Do You want Add Item In Cash Invoice ?"), vbQuestion + vbYesNo + vbDefaultButton2, "Warning...") = vbYes Then
        '                    rs = CurrentDb.OpenRecordset("SELECT T.lngTransaction, T.byteBase, T.byteTransType, T.lngContact, T.bCreatCash, T.strReference FROM Stock_Trans AS T WHERE T.byteBase=50 AND T.byteTransType=38 AND T.lngContact=27 AND T.bSubCash=True AND T.strReference='" & Forms![Ph_Prepearing_Med]![strReference] & "'", dbOpenSnapshot)
        '                    If rs.EOF Then
        '                    CurrentDb.Execute ("UPDATE Stock_Trans SET bCreatCash=True WHERE lngTransaction=" & Me.Parent![lngTransaction]), dbSeeChanges
        '                        strSQL = "INSERT INTO Stock_Trans (byteBase, byteTransType, strTransaction, byteDepartment, lngContact, dateTransaction, byteStatus, bCash, byteCurrency, lngSalesman, lngPatient, strRemarks, strReference, bCollected1, bApproved1, strUserPrint, bSubCash)"
        '                        strSQL = strSQL & " SELECT 50, 38,'" & strTrans & "', " & [byteDepartment] & " , 27, #" & Format(Forms![Ph_Prepearing_Med]![dateTransaction], "mm\/dd\/yyyy") & "#, 1, 1, 3, " & Forms![Ph_Prepearing_Med]![lngSalesmanNo] & ", " & Forms![Ph_Prepearing_Med]![lngPatient] & ", """ & Forms![Ph_Prepearing_Med]![strPatient] & """,'" & Forms![Ph_Prepearing_Med]![strReference] & "', 1, 0, """ & Forms![Cmn_Defaults]![strUserName] & """,True  "
        '                        CurrentDb.Execute strSQL
        '                        CurrentDb.Execute "INSERT INTO Stock_Trans_Audit (lngTransaction, strCreatedBy, dateCreated) VALUES (" & DLookup("lngTransaction", "stock_Trans", "strTransaction='" & strTrans & "'" & " AND dateTransaction=#" & Format(Forms![Ph_Prepearing_Med]![dateTransaction], "mm\/dd\/yyyy") & "#") & " , """ & Forms![Cmn_Defaults]![strUserName] & """, #" & Format(Now, "mm\/dd\/yyyy hh:nn") & "#)"
        '                        lngTransactionNew = DLookup("lngTransaction", "Stock_Trans", "strTransaction='" & strTrans & "' AND Year(dateTransaction)=" & Year(Of Date)() & " AND byteTransType=38" & " And byteBase=50")
        '                        rsXlink = con.OpenRecordset("SELECT lngXlink FROM Stock_Xlink WHERE lngTransaction=" & lngTransactionNew, dbOpenDynaset)
        '                        If rsXlink.EOF Then
        '                            CurrentDb().Execute "INSERT INTO Stock_Xlink (lngTransaction,lngPointer) VALUES(" & lngTransactionNew & "," & lngTransactionNew & ")"
        '                        End If
        '                    Else
        '                        lngTransactionNew = rs!lngTransaction
        '                    End If
        '                    If Forms![Ph_Prepearing_Med]![lngXlink1] & "" = "" Then Forms![Ph_Prepearing_Med]![lngXlink1] = DLookup("lngXlink", "Stock_Xlink", "lngPointer=" & lngTransactionNew)
        '                    CurrentDb.Execute "INSERT INTO Stock_Xlink_Items ( lngXlink, intEntryNumber, byteDepartment, intService, strItem, byteUnit, byteQuantityType, curQuantity, dateExpiry, curBasePrice, curUnitPrice, curUnitNetPrice, bCopied, byteWarehouse, dateEntry, strBarCode, strDose1 )" _
        '                     & " VALUES( " & Forms![Ph_Prepearing_Med]![lngXlink1] & ", " & Nz(DMax("intEntryNumber", "Stock_Xlink_Items", "lngXlink=" & Forms![Ph_Prepearing_Med]![lngXlink1]), 0) + 1 & ", " & [byteDepartment] & ", " & [intService] & ", " & [strItem] & ", " & [byteUnit] & ", 1, " & Forms![Ph_Prepearing_Med]![sfrm0].Form![curQuantity] & ", #" & dateExpiry & "#, " & Mid$([strBarCode], 10, 3) & "." & Mid$([strBarCode], 13, 2) & ", " & Mid$([strBarCode], 10, 3) & "." & Mid$([strBarCode], 13, 2) & ", " & Mid$([strBarCode], 10, 3) & "." & Mid$([strBarCode], 13, 2) & ", 0, " & [byteWarehouse] & ", #" & Format(Now(), "mm\/dd\/yyyy hh:nn:ss AmPm") & "#, '" & strBarCode & "','" & [strDose] & "')"
        '                    Undo()
        '                    '             Me.Requery
        '                    Forms![Ph_Prepearing_Med]![lstCashOrder].Requery()
        '                    con.Close()
        '                    ws.Close()
        '                    '            CheckItemCoverage = False
        '                    [lstCashOrder].Requery()
        '                    bConvertedCash = True
        '                    CheckItemCoverage = True
        '                    Exit Function
        '                Else
        '                    '                    [bApproval] = False
        '                    [strBatch] = 1
        '                    Forms![Ph_Prepearing_Med]![lstCashOrder].Requery()
        '                    CheckItemCoverage()
        '                End If
        '            Else
        '                rs = CurrentDb.OpenRecordset("SELECT T.lngTransaction, T.byteBase, T.byteTransType, T.lngContact, T.bCreatCash, T.strReference FROM Stock_Trans AS T WHERE T.byteBase=50 AND T.byteTransType=38 AND T.lngContact=27 AND T.bSubCash=True AND T.strReference='" & Forms![Ph_Prepearing_Med]![strReference] & "'", dbOpenSnapshot)
        '                If rs.EOF Then
        '                CurrentDb.Execute ("UPDATE Stock_Trans SET bCreatCash=True WHERE lngTransaction=" & Me.Parent![lngTransaction]), dbSeeChanges
        '                    strSQL = "INSERT INTO Stock_Trans (byteBase, byteTransType, strTransaction, byteDepartment, lngContact, dateTransaction, byteStatus, bCash, byteCurrency, lngSalesman, lngPatient, strRemarks, strReference, bCollected1, bApproved1, strUserPrint, bSubCash)"
        '                    strSQL = strSQL & " SELECT 50, 38,'" & strTrans & "', " & [byteDepartment] & " , 27, #" & Format(Forms![Ph_Prepearing_Med]![dateTransaction], "mm\/dd\/yyyy") & "#, 1, 1, 3, " & Forms![Ph_Prepearing_Med]![lngSalesmanNo] & ", " & Forms![Ph_Prepearing_Med]![lngPatient] & ", """ & Forms![Ph_Prepearing_Med]![strPatient] & """,'" & Forms![Ph_Prepearing_Med]![strReference] & "', 1, 0, """ & Forms![Cmn_Defaults]![strUserName] & """,True  "
        '                    CurrentDb.Execute strSQL
        '                    CurrentDb.Execute "INSERT INTO Stock_Trans_Audit (lngTransaction, strCreatedBy, dateCreated) VALUES (" & DLookup("lngTransaction", "stock_Trans", "strTransaction='" & strTrans & "'" & " AND dateTransaction=#" & Format(Forms![Ph_Prepearing_Med]![dateTransaction], "mm\/dd\/yyyy") & "#") & " , """ & Forms![Cmn_Defaults]![strUserName] & """, #" & Format(Now, "mm\/dd\/yyyy hh:nn") & "#)"
        '                    lngTransactionNew = DLookup("lngTransaction", "Stock_Trans", "strTransaction='" & strTrans & "' AND Year(dateTransaction)=" & Year(Of Date)() & " AND byteTransType=38" & " And byteBase=50")
        '                    rsXlink = con.OpenRecordset("SELECT lngXlink FROM Stock_Xlink WHERE lngTransaction=" & lngTransactionNew, dbOpenDynaset)
        '                    If rsXlink.EOF Then
        '                        CurrentDb().Execute "INSERT INTO Stock_Xlink (lngTransaction,lngPointer) VALUES(" & lngTransactionNew & "," & lngTransactionNew & ")"
        '                    End If
        '                Else
        '                    lngTransactionNew = rs!lngTransaction
        '                End If
        '                If Forms![Ph_Prepearing_Med]![lngXlink1] & "" = "" Then Forms![Ph_Prepearing_Med]![lngXlink1] = DLookup("lngXlink", "Stock_Xlink", "lngPointer=" & lngTransactionNew)
        '                CurrentDb.Execute "INSERT INTO Stock_Xlink_Items ( lngXlink, intEntryNumber, byteDepartment, intService, strItem, byteUnit, byteQuantityType, curQuantity, dateExpiry, curBasePrice, curUnitPrice, curUnitNetPrice, bCopied, byteWarehouse, dateEntry, strBarCode, strDose1 )" _
        '                 & " VALUES( " & Forms![Ph_Prepearing_Med]![lngXlink1] & ", " & Nz(DMax("intEntryNumber", "Stock_Xlink_Items", "lngXlink=" & Forms![Ph_Prepearing_Med]![lngXlink1]), 0) + 1 & ", " & Forms![Ph_Prepearing_Med]![sfrm0].Form![byteDepartment] & ", " & Forms![Ph_Prepearing_Med]![sfrm0].Form![intService] & ", " & Forms![Ph_Prepearing_Med]![sfrm0].Form![strItem] & ", " & Nz(Forms![Ph_Prepearing_Med]![sfrm0].Form![byteUnit], 1) & ", 1, " & Forms![Ph_Prepearing_Med]![sfrm0].Form![curQuantity] & ", #" & dateExpiry & "#, " & Mid$([strBarCode], 10, 3) & "." & Mid$([strBarCode], 13, 2) & ", " & Mid$([strBarCode], 10, 3) & "." & Mid$([strBarCode], 13, 2) & ", " & Mid$([strBarCode], 10, 3) & "." & Mid$([strBarCode], 13, 2) & ", 0, " & [byteWarehouse] & ", #" & Format(Now(), "mm\/dd\/yyyy hh:nn:ss AmPm") & "#, '" & strBarCode & "', '" & [strDose] & "')"
        '                Undo()
        '                '             Me.Requery
        '                Forms![Ph_Prepearing_Med]![lstCashOrder].Requery()
        '                con.Close()
        '                ws.Close()
        '                '            CheckItemCoverage = False
        '                [lstCashOrder].Requery()
        '            End If
        '            rs.Close()
        '        End If
    End Function

    Public Function DeterminePromotionClass(ByVal lngPatient As Long, ByVal dateTransaction As Date) As Byte
        'Dim rs As Recordset
        'Dim rsAmount As Recordset
        'Dim strAmount As String
        'Dim rsPromotions As Recordset
        Dim dsPromotions, ds As DataSet

        'dsPromotions = dcl.GetDS("SELECT * FROM Hw_Promotions")
        ds = dcl.GetDS("SELECT dateParticipation, byteClass, dateClass, lngFamily FROM Hw_Patients WHERE lngPatient=" & lngPatient)
        If ds.Tables(0).Rows(0).Item("lngFamily").ToString = "" Then
            ' Single patient
            Select Case ds.Tables(0).Rows(0).Item("byteClass").ToString
                Case "1" ' VIP Class
                    If Year(ds.Tables(0).Rows(0).Item("dateClass")) = Year(dateTransaction) Then
                        Return ds.Tables(0).Rows(0).Item("byteClass")
                    Else
                        '=> this code checks the class in non-existing table ( need to be fixed by Diea)
                        Return 1
                        'strAmount = "SELECT Sum(([curUnitPrice]*[curQuantity])) AS curAmount, ClinicInv.SumOfAmount AS ClinicInv "
                        'strAmount = strAmount & " FROM ((((((Stock_Trans AS T INNER JOIN Stock_Base AS B ON T.byteBase = B.byteBase) INNER JOIN Stock_Xlink AS X ON T.lngTransaction = X.lngTransaction) INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink) INNER JOIN Stock_Items AS I ON XI.strItem = I.strItem) INNER JOIN Stock_Groups AS G ON I.intGroup = G.intGroup) INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient) LEFT JOIN Clinic_Invoices_Amount AS ClinicInv ON P.lngPatient = ClinicInv.lngPatient"
                        'strAmount = strAmount & " WHERE T.byteBase In (40,18) AND T.byteStatus>0 AND T.lngPatient=" & [lngPatient] & " AND G.bPromotionIncluded=1 AND dateTransaction>=#" & Format(Nz(DLookup("dateLastService", "Hw_Patients", "lngPatient=" & [lngPatient]), #1/1/2006#), "mm\/dd\/yyyy") & "#"
                        'strAmount = strAmount & " GROUP BY ClinicInv.SumOfAmount"
                        'rsAmount = CurrentDb.OpenRecordset(strAmount, dbOpenSnapshot)
                        'rsPromotions.FindFirst "byteClass=1" ' VIP
                        'If Nz(rsAmount![curAmount], 0) + Nz(rsAmount![ClinicInv], 0) >= rsPromotions![curAmountFrom] Then
                        '    [bytePromotionClass] = 1
                        '    db.Execute("UPDATE Hw_Patients SET byteClass=1, dateClass=#" & Format([dateTransaction], "mm\/dd\/yyyy") & "# WHERE lngPatient=" & [lngPatient], dbFailOnError)
                        'Else
                        '    [bytePromotionClass] = 2
                        '    db.Execute("UPDATE Hw_Patients SET byteClass=2, dateClass=#" & Format([dateTransaction], "mm\/dd\/yyyy") & "# WHERE lngPatient=" & [lngPatient], dbFailOnError)
                        'End If
                    End If
                Case "2" ' A Class
                    If Year(ds.Tables(0).Rows(0).Item("dateClass")) = Year(dateTransaction) Then
                        Return ds.Tables(0).Rows(0).Item("byteClass")
                    Else
                        '=> this code checks the class in non-existing table ( need to be fixed by Diea)
                        Return 1
                        'strAmount = "SELECT Sum(([curUnitPrice]*[curQuantity])) AS curAmount, ClinicInv.SumOfAmount AS ClinicInv "
                        'strAmount = strAmount & " FROM ((((((Stock_Trans AS T INNER JOIN Stock_Base AS B ON T.byteBase = B.byteBase) INNER JOIN Stock_Xlink AS X ON T.lngTransaction = X.lngTransaction) INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink) INNER JOIN Stock_Items AS I ON XI.strItem = I.strItem) INNER JOIN Stock_Groups AS G ON I.intGroup = G.intGroup) INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient) LEFT JOIN Clinic_Invoices_Amount AS ClinicInv ON P.lngPatient = ClinicInv.lngPatient"
                        'strAmount = strAmount & " WHERE T.byteBase In (40,18) AND T.byteStatus>0 AND T.lngPatient=" & [lngPatient] & " AND G.bPromotionIncluded=1 AND dateTransaction>=#" & Format(Nz(DLookup("dateLastService", "Hw_Patients", "lngPatient=" & [lngPatient]), #1/1/2006#), "mm\/dd\/yyyy") & "#"
                        'strAmount = strAmount & " GROUP BY ClinicInv.SumOfAmount"
                        'rsAmount = CurrentDb.OpenRecordset(strAmount, dbOpenSnapshot)
                        'rsPromotions.FindFirst "byteClass=1" ' VIP
                        'If Nz(rsAmount![curAmount], 0) + Nz(rsAmount![ClinicInv], 0) >= rsPromotions![curAmountFrom] Then
                        '    [bytePromotionClass] = 1
                        '    db.Execute("UPDATE Hw_Patients SET byteClass=1, dateClass=#" & Format([dateTransaction], "mm\/dd\/yyyy") & "# WHERE lngPatient=" & [lngPatient], dbFailOnError)
                        'Else
                        '    [bytePromotionClass] = 2
                        '    db.Execute("UPDATE Hw_Patients SET byteClass=2, dateClass=#" & Format([dateTransaction], "mm\/dd\/yyyy") & "# WHERE lngPatient=" & [lngPatient], dbFailOnError)
                        'End If
                    End If
                Case Else
                    Return CDec("0" & ds.Tables(0).Rows(0).Item("byteClass").ToString)
            End Select
        Else
            ' Family
            Dim lngFamily As Long = ds.Tables(0).Rows(0).Item("lngFamily")
            Dim intPatients As Integer
            Dim dsTemp As DataSet = dcl.GetDS("SELECT lngPatient FROM Hw_Patients WHERE lngFamily=" & lngFamily)
            If dsTemp.Tables(0).Rows.Count > 0 Then
                intPatients = dsTemp.Tables(0).Rows(0).Item("lngPatient")
            Else
                intPatients = 0
            End If
            Select Case ds.Tables(0).Rows(0).Item("byteClass").ToString
                Case "1" ' VIP Class
                    If Year(ds.Tables(0).Rows(0).Item("dateClass")) = Year(dateTransaction) Then
                        Return ds.Tables(0).Rows(0).Item("byteClass")
                    Else
                        '=> this code checks the class in non-existing table ( need to be fixed by Diea)
                        Return 1
                        'strAmount = "SELECT Sum(([curUnitPrice]*[curQuantity])) AS curAmount, ClinicInv.SumOfAmount AS ClinicInv "
                        'strAmount = strAmount & " FROM ((((((Stock_Trans AS T INNER JOIN Stock_Base AS B ON T.byteBase = B.byteBase) INNER JOIN Stock_Xlink AS X ON T.lngTransaction = X.lngTransaction) INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink) INNER JOIN Stock_Items AS I ON XI.strItem = I.strItem) INNER JOIN Stock_Groups AS G ON I.intGroup = G.intGroup) INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient) LEFT JOIN Clinic_Invoices_Amount AS ClinicInv ON P.lngPatient = ClinicInv.lngPatient"
                        'strAmount = strAmount & " WHERE T.byteBase In (40,18) AND T.byteStatus>0 AND T.lngPatient=" & [lngPatient] & " AND G.bPromotionIncluded=1 AND dateTransaction>=#" & Format(Nz(DLookup("dateLastService", "Hw_Patients", "lngPatient=" & [lngPatient]), #1/1/2006#), "mm\/dd\/yyyy") & "#"
                        'strAmount = strAmount & " GROUP BY ClinicInv.SumOfAmount"
                        'rsAmount = CurrentDb.OpenRecordset(strAmount, dbOpenSnapshot)
                        'rsPromotions.FindFirst "byteClass=1" ' VIP
                        'If Nz(rsAmount![curAmount], 0) + Nz(rsAmount![ClinicInv], 0) >= rsPromotions![curAmountFrom] * intPatients Then
                        '    [bytePromotionClass] = 1
                        '    db.Execute("UPDATE Hw_Patients SET byteClass=1, dateClass=#" & Format([dateTransaction], "mm\/dd\/yyyy") & "# WHERE lngFamily=" & [lngFamily], dbFailOnError)
                        'Else
                        '    [bytePromotionClass] = 2
                        '    db.Execute("UPDATE Hw_Patients SET byteClass=2, dateClass=#" & Format([dateTransaction], "mm\/dd\/yyyy") & "# WHERE lngFamily=" & [lngFamily], dbFailOnError)
                        'End If
                    End If
                Case "2" ' A Class
                    If Year(ds.Tables(0).Rows(0).Item("dateClass")) = Year(dateTransaction) Then
                        Return ds.Tables(0).Rows(0).Item("byteClass")
                    Else
                        '=> this code checks the class in non-existing table ( need to be fixed by Diea)
                        Return 1
                        'strAmount = "SELECT Sum(([curUnitPrice]*[curQuantity])) AS curAmount, ClinicInv.SumOfAmount AS ClinicInv "
                        'strAmount = strAmount & " FROM ((((((Stock_Trans AS T INNER JOIN Stock_Base AS B ON T.byteBase = B.byteBase) INNER JOIN Stock_Xlink AS X ON T.lngTransaction = X.lngTransaction) INNER JOIN Stock_Xlink_Items AS XI ON X.lngXlink = XI.lngXlink) INNER JOIN Stock_Items AS I ON XI.strItem = I.strItem) INNER JOIN Stock_Groups AS G ON I.intGroup = G.intGroup) INNER JOIN Hw_Patients AS P ON T.lngPatient = P.lngPatient) LEFT JOIN Clinic_Invoices_Amount AS ClinicInv ON P.lngPatient = ClinicInv.lngPatient"
                        'strAmount = strAmount & " WHERE T.byteBase In (40,18) AND T.byteStatus>0 AND T.lngPatient=" & [lngPatient] & " AND G.bPromotionIncluded=1 AND dateTransaction>=#" & Format(Nz(DLookup("dateLastService", "Hw_Patients", "lngPatient=" & [lngPatient]), #1/1/2006#), "mm\/dd\/yyyy") & "#"
                        'strAmount = strAmount & " GROUP BY ClinicInv.SumOfAmount"
                        'rsAmount = CurrentDb.OpenRecordset(strAmount, dbOpenSnapshot)
                        'rsPromotions.FindFirst "byteClass=1" ' VIP
                        'If Nz(rsAmount![curAmount], 0) + Nz(rsAmount![ClinicInv], 0) >= rsPromotions![curAmountFrom] * intPatients Then
                        '    [bytePromotionClass] = 1
                        '    db.Execute("UPDATE Hw_Patients SET byteClass=1, dateClass=#" & Format([dateTransaction], "mm\/dd\/yyyy") & "# WHERE lngFamily=" & rs!lngFamily, dbFailOnError)
                        'Else
                        '    [bytePromotionClass] = 2
                        '    db.Execute("UPDATE Hw_Patients SET byteClass=2, dateClass=#" & Format([dateTransaction], "mm\/dd\/yyyy") & "# WHERE lngFamily=" & rs!lngFamily, dbFailOnError)
                        'End If
                    End If
                Case Else
                    Return CDec("0" & ds.Tables(0).Rows(0).Item("byteClass").ToString)
            End Select
        End If

    End Function

    ' Get ByteTransType
    Public Function GetTransType(ByVal lngGuarantor As Long) As Byte
        ' Parameter: lngGuarantor Equals 0, no guarantor
        Dim dsTransTypes As DataSet
        ' This code by me
        dsTransTypes = dcl.GetDS("SELECT * FROM Stock_Trans_Types AS STT INNER JOIN Stock_TransType_Departments AS STTD ON STT.byteTransType = STTD.byteTransType INNER JOIN Cmn_User_Departments AS CUD ON STTD.byteDepartment = CUD.byteDepartment WHERE STT.byteBase=" & byteBase & " AND CUD.strUserName = '" & strUserName & "'")
        If dsTransTypes.Tables(0).Rows.Count > 0 Then
            Return dsTransTypes.Tables(0).Rows(0).Item("byteTransType")
        Else
            Return 0
        End If

        'If lngGuarantor <> 0 Then
        '    rsTransTypes = CurrentDb().OpenRecordset("SELECT TT.byteTransType FROM (Stock_TransType_Departments AS TD INNER JOIN Stock_Trans_Types AS TT ON TD.byteTransType = TT.byteTransType) INNER JOIN Cmn_User_Departments AS UD ON TD.byteDepartment=UD.byteDepartment WHERE byteCash=2 AND byteBase=" & [byteBase].DefaultValue & " AND strUserName='" & Forms!Cmn_Defaults![strUserName] & "'", dbOpenSnapshot)
        '    If Not rsTransTypes.EOF Then
        '        [byteTransType] = rsTransTypes!byteTransType
        '    Else
        '        rsTransTypes.Close()
        '        rsTransTypes = CurrentDb().OpenRecordset("SELECT TT.byteTransType FROM (Stock_TransType_Departments AS TD INNER JOIN Stock_Trans_Types AS TT ON TD.byteTransType = TT.byteTransType) INNER JOIN Cmn_User_Departments AS UD ON TD.byteDepartment=UD.byteDepartment WHERE byteCash=3 AND byteBase=" & [byteBase].DefaultValue & " AND strUserName='" & Forms!Cmn_Defaults![strUserName] & "'", dbOpenSnapshot)
        '        If Not rsTransTypes.EOF Then
        '            [byteTransType] = rsTransTypes!byteTransType
        '            [bCash] = False
        '        End If
        '    End If
        '    byteTransType_AfterUpdate()
        '    [lngContact] = lngGuarantor
        '    [lngContactNo] = [lngContact]
        '    rsTransTypes.Close()
        'Else
        '    rsTransTypes = CurrentDb().OpenRecordset("SELECT TT.byteTransType FROM (Stock_TransType_Departments AS TD INNER JOIN Stock_Trans_Types AS TT ON TD.byteTransType = TT.byteTransType) INNER JOIN Cmn_User_Departments AS UD ON TD.byteDepartment=UD.byteDepartment WHERE byteCash=1 AND byteBase=" & [byteBase].DefaultValue & " AND strUserName='" & Forms!Cmn_Defaults![strUserName] & "'", dbOpenSnapshot)
        '    If Not rsTransTypes.EOF Then
        '        [byteTransType] = rsTransTypes!byteTransType
        '        byteTransType_AfterUpdate()
        '    Else
        '        rsTransTypes.Close()
        '        rsTransTypes = CurrentDb().OpenRecordset("SELECT TT.byteTransType FROM (Stock_TransType_Departments AS TD INNER JOIN Stock_Trans_Types AS TT ON TD.byteTransType = TT.byteTransType) INNER JOIN Cmn_User_Departments AS UD ON TD.byteDepartment=UD.byteDepartment WHERE byteCash=3 AND byteBase=" & [byteBase].DefaultValue & " AND strUserName='" & Forms!Cmn_Defaults![strUserName] & "'", dbOpenSnapshot)
        '        [byteTransType] = rsTransTypes!byteTransType
        '        [bCash] = True
        '        byteTransType_AfterUpdate()
        '    End If
        '    rsTransTypes.Close()
        'End If
        ''SetInsInfo Nz(lngGuarantor, 0)
    End Function

    Public Function checkDuplicateItem(ByVal curOrderQuantity As Decimal, ByVal strItem As String, ByVal SelectedInsuranceItems As String) As Boolean
        Dim Count As Integer = 0
        Dim items As String() = Split(SelectedInsuranceItems, ",")
        For Each item As String In items
            If item = strItem Then Count = Count + 1
        Next
        ' Count + 1 => Old Items + Current
        If (Count + 1) > curOrderQuantity Then Return False Else Return True
    End Function

    Public Function checkStock(ByVal strItem As String, ByVal curQuantity As Decimal, ByVal dateTransaction As Date, ByVal byteWarehouse As Byte, ByVal SelectedInsuranceItems As String, ByVal SelectedCashItems As String) As Boolean
        Dim bNegative As Boolean = False
        Dim curBalance As Decimal
        Dim ds As DataSet

        Dim Count As Integer = 0
        Dim I_items As String() = Split(SelectedInsuranceItems, ",")
        For Each item As String In I_items
            If item = strItem Then Count = Count + 1
        Next

        Dim C_items As String() = Split(SelectedCashItems, ",")
        For Each item As String In C_items
            If item = strItem Then Count = Count + 1
        Next

        'If DLookup("bNegative", "Hw_Firm") = False Then
        If bNegative = False Then '=> TODO: I don't know what is that for!
            Dim strSQL As String = "SELECT SUM(SB.intSign * SXI.curQuantity * SU.curFactor)/1 AS curBalance FROM Stock_Base AS SB INNER JOIN Stock_Trans AS ST ON SB.byteBase = ST.byteBase INNER JOIN Stock_Xlink AS SX ON ST.lngTransaction = SX.lngTransaction INNER JOIN Stock_Xlink_Items AS SXI ON SX.lngXlink = SXI.lngXlink INNER JOIN Stock_Units AS SU ON SU.byteUnit = SXI.byteUnit WHERE ST.byteStatus > 0 And SB.bInclude <> 0 And Year(dateTransaction) = " & intYear & " And SXI.byteWarehouse = " & byteWarehouse & " AND SXI.strItem='" & strItem & "' AND ST.dateTransaction <= '" & Today.ToString("yyyy-MM-dd") & "'"

            ds = dcl.GetDS(strSQL)
            If ds.Tables(0).Rows.Count > 0 Then
                curBalance = CDec("0" & ds.Tables(0).Rows(0).Item("curBalance").ToString)
            Else
                curBalance = 0
            End If

            If curBalance - (curQuantity + Count) < 0 Then
                Return False
            End If
        End If
        Return True
    End Function
    Public Function getQuantity(ByVal strReference As String, ByVal lngPatient As Long, ByVal strItem As String) As Decimal
        If strReference <> "" Then
            Dim ds As DataSet
            ds = dcl.GetDS("SELECT * FROM Hw_Treatments_Pharmacy WHERE strReference='" & strReference & "' AND lngPatient=" & lngPatient & " AND strItem='" & strItem & "'")
            If ds.Tables(0).Rows.Count > 0 Then
                Return ds.Tables(0).Rows(0).Item("curQuantity")
            Else
                Return 1
            End If
        Else
            Return 1
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
                        'strItem_AfterUpdate()
                        'curBasePrice = Mid(strBarcode, 9, 4) & "." & Mid([strBarcode], 13, 2)
                        'dateExpiry = DateSerial(Mid(strBarcode, 17, 2), Mid(strBarcode, 15, 2), 1)
                        'ret = "OK => Less"
                    Else
                        'MsgBox(IIf(gblanguage, "ÇáÕäÝ ÛíÑ ãÚÑÝ.", "Item not defined."))
                        str(0) = "Err:item not defined"
                    End If
                Case Is < 12
                    strItem = Mid(strBarcode, 1, 8)
                    ds = dcl.GetDS("SELECT * FROM Stock_Items WHERE strItem='" & strItem & "'")
                    If ds.Tables(0).Rows.Count > 0 Then
                        str(0) = strItem
                        str(1) = Mid(strBarcode, 9, 4) & "." & Mid(strBarcode, 13, 2)
                        str(2) = "20" & Mid(strBarcode, 17, 2) & "-" & Mid(strBarcode, 15, 2) & "-01"
                        'strItem_AfterUpdate()
                        'curBasePrice = Mid(strBarcode, 9, 4) & "." & Mid([strBarcode], 13, 2)
                        'dateExpiry = DateSerial(Mid(strBarcode, 17, 2), Mid(strBarcode, 15, 2), 1)
                        'ret = "OK => Less"
                    Else
                        'MsgBox(IIf(gblanguage, "ÇáÕäÝ ÛíÑ ãÚÑÝ.", "Item not defined."))
                        str(0) = "Err:item not defined"
                    End If
                Case 12
                    strItem = Left(strBarcode, 8)
                    ds = dcl.GetDS("SELECT * FROM Stock_Item_Info WHERE strOldReference='" & strItem & "'")
                    If ds.Tables(0).Rows.Count > 0 Then
                        str(0) = strItem
                        str(1) = ""
                        str(2) = "20" & Mid(strBarcode, 12, 1) & "-" & Mid(strBarcode, 10, 2) & "-01"
                        'strItem_AfterUpdate()
                        'dateExpiry = DateSerial(Mid(strBarcode, 12, 1), Mid(strBarcode, 10, 2), 1)
                        'ret = "OK => 12"
                    Else
                        'MsgBox(IIf(gblanguage, "ÇáÕäÝ ÛíÑ ãÚÑÝ.", "Item not defined."))
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

                        'dateExpiry = DateSerial(Mid(strBarcode, 8, 2), Mid(strBarcode, 6, 2), 1)
                        'curBasePrice = Mid(strBarcode, 10, 3) & "." & Mid(strBarcode, 13, 2)
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
                    Else
                        'MsgBox(IIf(gblanguage, "ÇáÕäÝ ÛíÑ ãÚÑÝ.", "Item not defined."))
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

    Public Function getPrice(ByVal curBasePrice As Decimal, ByVal strItem As String) As Decimal
        Dim ds As DataSet
        Dim byteUnit As Byte
        Dim ret, curPrice, curFactor As Decimal

        If curBasePrice = 0 Then
            ds = dcl.GetDS("SELECT * FROM Stock_Items AS SI LEFT JOIN Stock_Units AS SU ON SI.byteIssueUnit = SU.byteUnit LEFT JOIN Stock_Item_Prices AS SIP ON SI.strItem = SIP.strItem WHERE SI.strItem='" & strItem & "'")
            byteUnit = CByte("0" & ds.Tables(0).Rows(0).Item("byteUnit").ToString)
            curPrice = CDec("0" & ds.Tables(0).Rows(0).Item("curPrice").ToString)
            curFactor = CDec("0" & ds.Tables(0).Rows(0).Item("curFactor").ToString)
            If curPrice <> 0 Then
                If byteUnit <> 0 Then
                    ret = CDec((curPrice * curFactor) / curFactor)
                Else
                    ret = curPrice
                End If
            Else
                ret = 0
            End If
        Else
            ret = curBasePrice
        End If
        Return ret
    End Function

    Public Function getDiscount(ByVal bCash As Boolean, ByVal strItem As String, ByVal bytePriceType As Integer, ByVal intGroup As Integer, ByVal lngPatient As Long, ByVal dateTransaction As Date) As Decimal 'bytePriceType from contacts table (company)
        Dim dsDiscount, dsPromotion As DataSet
        'Dim intSerice, intGroup As Integer
        'Dim byteWarehouse As Byte
        Dim curDiscount As Decimal
        Dim bytePromotionClass As Byte

        'ds = dcl.GetDS("SELECT * FROM Hw_Department_Items AS HDI INNER JOIN Hw_Department_Warehouse AS HDW ON HDI.intService = HDW.intService AND HDI.byteDepartment = HDW.byteDepartment INNER JOIN Stock_Items ON HDI.strItem = Stock_Items.strItem WHERE HDI.byteDepartment=" & byteDepartment & " AND HDI.strItem='" & strItem & "'")
        'intSerice = ds.Tables(0).Rows(0).Item("intService")
        'byteWarehouse = ds.Tables(0).Rows(0).Item("byteWarehouse")
        'intGroup = ds.Tables(0).Rows(0).Item("intGroup")
        '==>[intService] = DLookup("intService", "Hw_Department_Items", "byteDepartment=15 AND strItem='" & [strItem] & "'")
        '==>[byteWarehouse] = [intService].Column(2)
        'enabled
        '[dateExpiry].Enabled = Nz([strItemDesc].Column(3)) And Nz([byteWarehouse].Column(1), True) And [strItem].Column(3) = "0"

        If bCash = False Then
            dsDiscount = dcl.GetDS("SELECT * FROM Healthware..Stock_Price_Groups WHERE bytePriceType=" & bytePriceType & " AND intGroup=" & intGroup)
            If dsDiscount.Tables(0).Rows.Count > 0 Then
                curDiscount = Math.Abs(dsDiscount.Tables(0).Rows(0).Item("curPercent"))
            Else
                curDiscount = 0
            End If
        Else
            ' no need for [discount code] from old system
            curDiscount = 0

            '' ''bytePromotionClass = DeterminePromotionClass(lngPatient, dateTransaction)
            '' ''If bytePromotionClass <> 0 Then
            '' ''    dsPromotion = dcl.GetDS("SELECT curDiscount FROM Hw_Promotion_Groups WHERE byteClass=" & bytePromotionClass & " AND intGroup=" & intGroup)
            '' ''    If dsPromotion.Tables(0).Rows.Count > 0 Then
            '' ''        curDiscount = dsPromotion.Tables(0).Rows(0).Item("curDiscount")
            '' ''        '[curUnitPrice] = [curUnitPrice] - ([curUnitPrice] * Abs([curDiscount] / 100))
            '' ''    Else
            '' ''        curDiscount = 0
            '' ''    End If
            '' ''Else
            '' ''    curDiscount = 0
            '' ''End If
        End If
        '==>[curDiscount] = Abs(Nz(DLookup("curPercent", "Stock_Price_Groups", "bytePriceType=" & Nz(Me.Parent![lngContact].Column(2), 0) & " AND intGroup=" & Nz([strItem].Column(2), 0)), 0))
        '==>[curBaseDiscount] = [curDiscount]

        '    CheckItemCoverage

        'If Me.Parent![bCash] And Me.Parent![bytePromotionClass] & "" <> "" Then
        '    [curDiscount] = Nz(DLookup("curDiscount", "Hw_Promotion_Groups", "byteClass=" & Me.Parent![bytePromotionClass] & " AND intGroup=" & Val([strItemDesc].Column(6))), 0)
        '    [curUnitPrice] = [curUnitPrice] - ([curUnitPrice] * Abs([curDiscount] / 100))
        'End If

        '==>Dim curNet As Currency
        '==>curNet = ([curBasePrice] * [curQuantity]) - ([curBasePrice] * [curQuantity] * [curDiscount] / 100)
        Return curDiscount
    End Function

    Public Function getDiscount2(ByVal strItem As String, ByVal lngPatient As Long, ByVal dateTransaction As Date) As Decimal
        Dim ds As DataSet
        Dim curDiscount As Decimal
        Dim bytePromotionClass As Byte
        ds = dcl.GetDS("SELECT * FROM Hw_Contacts AS HC LEFT JOIN Stock_Price_Groups AS SPG ON HC.bytePriceType=SPG.bytePriceType WHERE lngContact=27 AND bResident=1")
        If ds.Tables(0).Rows.Count > 0 Then
            curDiscount = CDec("0" & ds.Tables(0).Rows(0).Item("curPercent").ToString)
        Else
            curDiscount = 0
        End If

        bytePromotionClass = DeterminePromotionClass(lngPatient, dateTransaction)
        If bytePromotionClass <> 0 Then
            ds = dcl.GetDS(" SELECT * FROM Stock_Items WHERE strItem='" & strItem & "'")
            Dim intGroup As Integer = ds.Tables(0).Rows(0).Item("intGroup")
            ds = dcl.GetDS("SELECT curDiscount FROM Hw_Promotion_Groups WHERE byteClass=" & bytePromotionClass & " AND intGroup=" & intGroup)
            If ds.Tables(0).Rows.Count > 0 Then
                curDiscount = ds.Tables(0).Rows(0).Item("curDiscount")
                '[curUnitPrice] = [curUnitPrice] - ([curUnitPrice] * Abs([curDiscount] / 100))
            End If
        End If
        Return curDiscount
    End Function

    Public Function getTax(ByVal curNet As Decimal, ByVal strItem As String) As Decimal
        'Dim ds As DataSet
        'Dim bTax As Boolean
        'Dim curTax As Decimal

        'ds = dcl.GetDS("SELECT * FROM Stock_Items WHERE strItem='" & strItem & "'")
        'bTax = ds.Tables(0).Rows(0).Item("bTax")
        'curTax = ds.Tables(0).Rows(0).Item("curTax")

        ''Dim curNet As Currency
        ''curNet = ([curBasePrice] * [curQuantity]) - ([curBasePrice] * [curQuantity] * [curDiscount] / 100)
        ''If Not Me.Parent![bCash] Then
        'If bTax = True And curTax <> 0 Then

        '    If Me.Parent!bPercentValue = True And Me.Parent!curCoverage <> 0 Then
        '        curVAT = (curNet * Me.Parent!curCoverage / 100) * curTax / 100
        '        curVATI = (curNet * (100 - Me.Parent!curCoverage) / 100) * curTax / 100
        '    Else
        '        curVAT = 0
        '        curVATI = 0
        '    End If
        'Else
        '    Return 0
        '    'curVAT = 0
        '    'curVATI = 0
        'End If
        ''End If
    End Function

    Public Function CalculateAmount() As Decimal
        'Dim rsValue As Recordset
        'Dim curValue As Currency, curDiscount As Currency, curTotal As Currency

        'If [sfrm].Form![lngXlink] & "" = "" Then Exit Function
        'curTotal = Nz(DSum("curUnitPrice*curQuantity", "Stock_Xlink_Items", "lngXlink=" & [sfrm].Form![lngXlink]), 0)
        'curDiscount = curTotal
        'rsValue = CurrentDb().OpenRecordset("SELECT X.*, V.intValueSign, V.bytePercentCalculation FROM Stock_Xlink_Values AS X INNER JOIN Stock_Value_Types AS V ON (X.byteValueType=V.byteValueType) WHERE lngXlink=" & [sfrm].Form![lngXlink] & " ORDER BY X.byteValueType", dbOpenSnapshot)
        'Do Until rsValue.EOF
        '    Select Case rsValue!bytePercentCalculation
        '        Case 1 ' Prime Cost
        '            If rsValue!bPercentValue Then
        '                curValue = rsValue!curValue * curTotal / 100
        '            Else
        '                curValue = rsValue!curValue
        '            End If
        '            If rsValue!byteValueType = 1 Then curDiscount = curTotal - curValue

        '        Case 2 ' Prime - Discount
        '            If rsValue!bPercentValue Then
        '                curValue = rsValue!curValue * curDiscount / 100
        '            Else
        '                curValue = rsValue!curValue
        '            End If
        '    End Select
        '    curTotal = curTotal + rsValue!intValueSign * curValue
        '    rsValue.MoveNext()
        'Loop
        'rsValue.Close()
        'CalculateAmount = curTotal
    End Function
#End Region

    Public Function getReturnedItemsValues(ByVal lngTransaction As Long, ByVal lstItems As String) As Decimal()
        If lstItems.Length > 0 Then
            Dim dsTrans As DataSet
            dsTrans = dcl.GetDS("SELECT T.lngTransaction as lngTransaction,* FROM Stock_Trans AS T LEFT JOIN Stock_Trans_Invoices AS TI ON T.lngTransaction=TI.lngTransaction WHERE T.lngTransaction=" & lngTransaction)
            If dsTrans.Tables(0).Rows.Count > 0 Then
                Dim lngPatient As Long = dsTrans.Tables(0).Rows(0).Item("lngPatient")
                Dim lngContect As Long = dsTrans.Tables(0).Rows(0).Item("lngContact")
                Dim lngSalesman As Long = dsTrans.Tables(0).Rows(0).Item("lngSalesman")
                Dim dateTransaction As Date = dsTrans.Tables(0).Rows(0).Item("dateTransaction")
                Dim bCash As Boolean = dsTrans.Tables(0).Rows(0).Item("bCash")
                Dim Payment As Decimal = dsTrans.Tables(0).Rows(0).Item("curCash")
                Dim VAT As Decimal = dsTrans.Tables(0).Rows(0).Item("curCashVAT")
                '
                Dim TotalAmount, TotalVAT As Decimal
                Dim TotalCash, TotalCashVAT As Decimal
                Dim dsListItems, dsLeftItems As DataSet
                'Price × Quantity = Returned Total
                dsListItems = dcl.GetDS("SELECT SUM(XI.curUnitPrice * XI.curQuantity) AS Total, SUM(XI.curVAT * XI.curQuantity) AS TotalVat FROM Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink=X.lngXlink INNER JOIN Stock_Items AS I ON XI.strItem=I.strItem WHERE X.lngTransaction=" & lngTransaction & " AND intEntryNumber IN (" & lstItems & ")")
                If IsDBNull(dsListItems.Tables(0).Rows(0).Item("Total")) Then TotalCash = 0 Else TotalCash = dsListItems.Tables(0).Rows(0).Item("Total")
                If IsDBNull(dsListItems.Tables(0).Rows(0).Item("TotalVat")) Then TotalCashVAT = 0 Else TotalCashVAT = dsListItems.Tables(0).Rows(0).Item("TotalVat")
                '
                Dim MaxP, CICov, MICov As Decimal
                Dim bPercent As Boolean
                Dim curPercent As Decimal
                Dim cashAmount, cashVAT, creditAmount, creditVAT As Decimal
                If bCash = False Then
                    dsLeftItems = dcl.GetDS("SELECT SUM(XI.curUnitPrice) AS Total, SUM(XI.curVAT) AS TotalVat FROM Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink=X.lngXlink INNER JOIN Stock_Items AS I ON XI.strItem=I.strItem WHERE X.lngTransaction=" & lngTransaction & " AND intEntryNumber Not IN (" & lstItems & ")")
                    If IsDBNull(dsLeftItems.Tables(0).Rows(0).Item("Total")) Then TotalAmount = 0 Else TotalAmount = dsLeftItems.Tables(0).Rows(0).Item("Total")
                    If IsDBNull(dsLeftItems.Tables(0).Rows(0).Item("TotalVat")) Then TotalVAT = 0 Else TotalVAT = dsLeftItems.Tables(0).Rows(0).Item("TotalVat")

                    Dim result As String() = getCoverage(lngPatient, lngContect)
                    If Left(result(0), 4) <> "Err:" Then
                        MaxP = result(2)
                        curPercent = result(1)
                        bPercent = result(0)
                    Else
                        Return {0, 0, 0, 0}
                    End If
                    CICov = getTotalClinicInvoices(lngPatient, lngSalesman, dateTransaction)
                    MICov = getTotalPharmacyInvoices(lngPatient, lngSalesman, dateTransaction, lngTransaction, False)
                    Dim Value As Decimal = TotalAmount * (curPercent / 100)
                    Dim ValueVAT As Decimal = TotalVAT * (curPercent / 100)
                    If Value >= (MaxP - (CICov + MICov)) Then
                        cashAmount = 0
                        cashVAT = 0
                    Else
                        cashAmount = Payment - Value
                        cashVAT = VAT - ValueVAT
                    End If
                    creditAmount = TotalCash - cashAmount
                    creditVAT = TotalCashVAT - creditVAT
                Else
                    MaxP = 0
                    CICov = 0
                    MICov = 0
                    cashAmount = TotalCash
                    cashVAT = TotalCashVAT
                    creditAmount = 0
                    creditVAT = 0
                End If
                Return {creditAmount, cashAmount, creditVAT, cashVAT}
            Else
                Return {0, 0, 0, 0}
            End If
        Else
            Return {0, 0, 0, 0}
        End If
    End Function

    Public Function calcCoveredCash_New2(ByVal curTotalItems As Decimal, ByVal Payment As Decimal, ByVal curPercent As Decimal, ByVal bPercent As Boolean, ByVal curMaxDeduction As Decimal, ByVal curTotalClinicInvoices As Decimal, ByVal curTotalPharmacyInvoices As Decimal) As Decimal
        Dim Price As Decimal = curTotalItems
        Dim Value As Decimal = Price * (curPercent / 100)
        'Dim curCoverd As Decimal = curTotal * (curPercent / 100)
        'Dim curExter As Decimal = (curTotalClinicInvoices + curTotalPharmacyInvoices + curTotal) - curMaxDeduction
        If bPercent = True Then
            If Value >= (curMaxDeduction - (curTotalClinicInvoices + curTotalPharmacyInvoices)) Then
                Return 0
            Else
                Return Payment - Value
            End If
            'If curCoverd = 0 Then
            '    Return 0
            'Else
            '    'If (curTotalClinicInvoices + curTotalPharmacyInvoices + curCoverd) > curMaxDeduction Then
            '    If curCoverd > curMaxDeduction - (curTotalClinicInvoices + curTotalPharmacyInvoices) Then
            '        Return 0 'curMaxDeduction - (curTotalClinicInvoices + curTotalPharmacyInvoices)
            '    Else
            '        Return curCoverd
            '    End If
            'End If
        Else
            Return 0
        End If
    End Function

    Public Function getReturnedItemsValue(ByVal lngTransaction As Long, ByVal lstItems As String) As Decimal()
        If lstItems.Length > 0 Then
            Dim dsTrans As DataSet
            dsTrans = dcl.GetDS("SELECT T.lngPatient, T.lngContact, T.lngSalesman, CONVERT(varchar(10), TI.dateTransaction, 120), T.bCash, SUM(XI.curUnitPrice) AS curTotal, TI.curCash AS curPaid FROM Stock_Trans AS T INNER JOIN Stock_Xlink AS X ON X.lngTransaction=T.lngTransaction INNER JOIN Stock_Xlink_Items AS XI ON XI.lngXlink=X.lngXlink INNER JOIN Stock_Trans_Invoices AS TI ON T.lngTransaction=TI.lngTransaction WHERE T.lngTransaction=" & lngTransaction & " GROUP BY T.lngPatient, T.lngContact, T.lngSalesman, CONVERT(varchar(10), TI.dateTransaction, 120), T.bCash, TI.curCash")
            If dsTrans.Tables(0).Rows.Count > 0 Then
                Dim TotalInvoice As Decimal = dsTrans.Tables(0).Rows(0).Item("curTotal")
                Dim TotalPaied As Decimal = dsTrans.Tables(0).Rows(0).Item("curPaid")
                Dim lngPatient As Long = dsTrans.Tables(0).Rows(0).Item("lngPatient")
                Dim lngContect As Long = dsTrans.Tables(0).Rows(0).Item("lngContact")
                Dim lngSalesman As Long = dsTrans.Tables(0).Rows(0).Item("lngSalesman")
                Dim dateTransaction As Date = dsTrans.Tables(0).Rows(0).Item("dateTransaction")
                Dim bCash As Boolean = dsTrans.Tables(0).Rows(0).Item("bCash")
                '
                Dim TotalAmount, TotalVAT As Decimal
                Dim dsListItems As DataSet
                dsListItems = dcl.GetDS("SELECT SUM(XI.curUnitPrice) AS Total, SUM(XI.curVAT) AS TotalVat FROM Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink=X.lngXlink INNER JOIN Stock_Items AS I ON XI.strItem=I.strItem WHERE X.lngTransaction=" & lngTransaction & " AND intEntryNumber IN (" & lstItems & ")")
                TotalAmount = dsListItems.Tables(0).Rows(0).Item("Total")
                TotalVAT = dsListItems.Tables(0).Rows(0).Item("TotalVat")
                '
                Dim MaxP, CICov, MICov As Decimal
                Dim bPercent As Boolean
                Dim curPercent As Decimal
                Dim cashAmount, cashVAT, creditAmount, creditVAT As Decimal
                If bCash = False Then
                    Dim result As String() = getCoverage(lngPatient, lngContect)
                    If Left(result(0), 4) <> "Err:" Then
                        MaxP = result(2)
                        curPercent = result(1)
                        bPercent = result(0)
                    Else
                        Return {0, 0, 0, 0}
                    End If
                    CICov = getTotalClinicInvoices(lngPatient, lngSalesman, dateTransaction)
                    MICov = getTotalPharmacyInvoices(lngPatient, lngSalesman, dateTransaction, lngTransaction, False)
                    Dim Price As Decimal = TotalInvoice - TotalAmount
                    Dim Value As Decimal = (Price * curPercent) / 100
                    If Value >= (MaxP - (MICov + CICov)) Then
                        cashAmount = 0
                        cashVAT = 0
                    Else
                        cashAmount = TotalPaied - Value
                        cashVAT = TotalVAT
                    End If
                    creditAmount = TotalAmount - cashAmount
                    creditVAT = TotalVAT - creditVAT
                Else
                    MaxP = 0
                    CICov = 0
                    MICov = 0
                    cashAmount = TotalAmount
                    cashVAT = TotalVAT
                    creditAmount = 0
                    creditVAT = 0
                End If
                Return {creditAmount, cashAmount, creditVAT, cashVAT}
            Else
                Return {0, 0, 0, 0}
            End If
        Else
            Return {0, 0, 0, 0}
        End If
    End Function
End Class
