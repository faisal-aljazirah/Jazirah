Public Class p_invoice
    Inherits System.Web.UI.Page
    Dim dcl As New DCL.Conn.DataClassLayer
    Dim DataLang As String

    Public PatientNo, PatientNameAr, PatientNameEn, PatientSex, PatientAge, DoctorNo, DoctorNameAr, DoctorNameEn, DepartmentNo, DepartmentNameAr, DepartmentNameEn, InvoiceNo, InvoiceDate, InvoiceTime, InvoiceType, UserName, CompanyNo, CompanyNameEn, CompanyNameAr, ItemsTable, NetPriceText, PatientPayText, VATText, ContractNo, HolderNameAr, HolderNameEn As String
    Public TotalPrice, Discount, NetPrice, PatientPay, TotalVAT As Decimal
    Public ByteLanguage As Byte
    Public strUserName, strDateFormat, strTimeFormat As String
    Dim bCash As Boolean

    Private Sub p_invoice_Init(sender As Object, e As EventArgs) Handles Me.Init
        ' User Options
        If HttpContext.Current.Session("UserName") Is Nothing Then
            'Remove this line after complete user login and settings
            'HttpContext.Current.Session("UserName") = "SoftNet"
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
        Dim ds As DataSet
        If ByteLanguage = 2 Then DataLang = "Ar" Else DataLang = "En"
        VATText = "الضريبة VAT"
        Dim lngTransaction As Long
        If Request.QueryString("t") Is Nothing Then
        Else
            Try
                lngTransaction = Request.QueryString("t")
                Dim AutoPrint As String = Request.QueryString("ap")
                ds = dcl.GetDS("SELECT ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirstAr,'') + ' ') + LTRIM(ISNULL(P.strSecondAr,'') + ' ') + LTRIM(ISNULL(P.strThirdAr ,'') + ' ') + LTRIM(ISNULL(P.strLastAr,''))) AS PatientNameAr, RTRIM(LTRIM(ISNULL(P.strFirstEn,'') + ' ') + LTRIM(ISNULL(P.strSecondEn,'') + ' ') + LTRIM(ISNULL(P.strThirdEn ,'') + ' ') + LTRIM(ISNULL(P.strLastEn,''))) AS PatientNameEn, P.bSex, P.dateBirth, C1.lngContact AS DoctorNo, C1.strContactAr AS DoctorNameAr, C1.strContactEn AS DoctorNameEn, C2.lngContact AS CompanyNo, C2.strContactEn AS CompanyNameEn, C2.strContactAr AS CompanyNameAr, C3.lngContract AS ContractNo, C3.strHolderEn AS HolderNameEn, C3.strHolderAr AS HolderNameAr, D.byteDepartment AS DepartmentNo, D.strDepartmentEn AS DepartmentNameEn, D.strDepartmentAr AS DepartmentNameAr, ST.strTransaction AS InvoiceNo, STI.dateTransaction AS InvoiceDate, ST.bCash AS InvoiceType, STA.strCreatedBy AS UserName, ST.lngTransaction AS TransactionNo, ST.dateTransaction AS TransactionDate, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.strReference AS ClinicInvoiceNo, STA.strCashBy AS CashierName,STA.strVoidBy AS VoidName,STA.dateVoid AS VoidDate, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus, ST.byteStatus AS [Status], STI.curCash, STI.curCredit, STI.curCashVAT, STI.curCreditVAT FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact LEFT JOIN Ins_Contracts AS C3 ON P.lngContract = C3.lngContract INNER JOIN Stock_Trans_Invoices AS STI ON ST.lngTransaction=STI.lngTransaction WHERE ST.byteBase IN (18, 40) AND ST.lngTransaction = " & lngTransaction)
                If ds.Tables(0).Rows.Count > 0 Then
                    PatientNo = ds.Tables(0).Rows(0).Item("PatientNo").ToString
                    PatientNameAr = ds.Tables(0).Rows(0).Item("PatientNameAr").ToString
                    PatientNameEn = ds.Tables(0).Rows(0).Item("PatientNameEn").ToString
                    If ds.Tables(0).Rows(0).Item("bSex") = True Then PatientSex = "Male" Else PatientSex = "Female"
                    If Not (IsDBNull(ds.Tables(0).Rows(0).Item("dateBirth"))) Then PatientAge = DateDiff(DateInterval.Year, ds.Tables(0).Rows(0).Item("dateBirth"), Today) Else PatientAge = 0
                    DoctorNo = ds.Tables(0).Rows(0).Item("DoctorNo").ToString
                    DoctorNameAr = ds.Tables(0).Rows(0).Item("DoctorNameAr").ToString
                    DoctorNameEn = ds.Tables(0).Rows(0).Item("DoctorNameEn").ToString
                    DepartmentNo = ds.Tables(0).Rows(0).Item("DepartmentNo").ToString
                    DepartmentNameAr = ds.Tables(0).Rows(0).Item("DepartmentNameAr").ToString
                    DepartmentNameEn = ds.Tables(0).Rows(0).Item("DepartmentNameEn").ToString
                    ContractNo = ds.Tables(0).Rows(0).Item("ContractNo").ToString
                    HolderNameAr = ds.Tables(0).Rows(0).Item("HolderNameAr").ToString
                    HolderNameEn = ds.Tables(0).Rows(0).Item("HolderNameEn").ToString
                    InvoiceNo = ds.Tables(0).Rows(0).Item("InvoiceNo").ToString
                    InvoiceDate = CDate(ds.Tables(0).Rows(0).Item("InvoiceDate")).ToString("yyyy-MM-dd")
                    InvoiceTime = CDate(ds.Tables(0).Rows(0).Item("InvoiceDate")).ToString("hh:mm tt")
                    If CBool(ds.Tables(0).Rows(0).Item("InvoiceType")) = True Then InvoiceType = "نقدي/Cash" Else InvoiceType = "آجل/Credit"
                    UserName = ds.Tables(0).Rows(0).Item("UserName").ToString
                    CompanyNo = ds.Tables(0).Rows(0).Item("CompanyNo").ToString
                    CompanyNameEn = ds.Tables(0).Rows(0).Item("CompanyNameEn").ToString
                    CompanyNameAr = ds.Tables(0).Rows(0).Item("CompanyNameAr").ToString
                    bCash = ds.Tables(0).Rows(0).Item("InvoiceType")
                    PatientPay = ds.Tables(0).Rows(0).Item("curCash") + ds.Tables(0).Rows(0).Item("curCashVAT")
                    NetPrice = ds.Tables(0).Rows(0).Item("curCredit") + ds.Tables(0).Rows(0).Item("curCreditVAT")

                    ' get invoice items
                    Dim IsReturned As Boolean = False
                    Dim dsItems As DataSet
                    ItemsTable = ""
                    dsItems = dcl.GetDS("SELECT * FROM Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink=X.lngXlink INNER JOIN Stock_Items AS I ON XI.strItem=I.strItem WHERE X.lngTransaction=" & lngTransaction)
                    Dim IsCopied As Boolean
                    For I = 0 To dsItems.Tables(0).Rows.Count - 1
                        If IsDBNull(dsItems.Tables(0).Rows(I).Item("bCopied")) Then IsCopied = False Else IsCopied = dsItems.Tables(0).Rows(I).Item("bCopied")
                        If IsCopied = True Then
                            IsReturned = True
                            ItemsTable = ItemsTable & "<tr><td class=""auto-style1""><strike>" & Math.Round(dsItems.Tables(0).Rows(I).Item("curBasePrice"), 2, MidpointRounding.AwayFromZero) & "</strike></td><td class=""auto-style2""><strike>" & Math.Round(dsItems.Tables(0).Rows(I).Item("curQuantity"), 2, MidpointRounding.AwayFromZero) & "</strike></td><td class=""med""><strike>" & dsItems.Tables(0).Rows(I).Item("strItemEn") & " " & dsItems.Tables(0).Rows(I).Item("strItem") & "</strike></td></tr>"
                        Else
                            ItemsTable = ItemsTable & "<tr><td class=""auto-style1"">" & Math.Round(dsItems.Tables(0).Rows(I).Item("curBasePrice"), 2, MidpointRounding.AwayFromZero) & "</td><td class=""auto-style2"">" & Math.Round(dsItems.Tables(0).Rows(I).Item("curQuantity"), 2, MidpointRounding.AwayFromZero) & "</td><td class=""med"">" & dsItems.Tables(0).Rows(I).Item("strItemEn") & " " & dsItems.Tables(0).Rows(I).Item("strItem") & "</td></tr>"
                            TotalPrice = TotalPrice + (dsItems.Tables(0).Rows(I).Item("curBasePrice") * dsItems.Tables(0).Rows(I).Item("curQuantity"))
                            Discount = Discount + (dsItems.Tables(0).Rows(I).Item("curBasePrice") * (dsItems.Tables(0).Rows(I).Item("curDiscount") / 100))
                            If Not (IsDBNull(dsItems.Tables(0).Rows(I).Item("curVAT"))) Then TotalVAT = TotalVAT + dsItems.Tables(0).Rows(I).Item("curVAT")
                        End If
                    Next
                    If IsReturned = True Then
                        Dim dsReturned As DataSet = dcl.GetDS("SELECT * FROM Stock_Trans AS T INNER JOIN Stock_Trans_Invoices AS TI ON T.lngTransaction=TI.lngTransaction WHERE T.strTransaction='" & InvoiceNo & "' AND T.byteBase=18 AND T.byteStatus>0")
                        If dsReturned.Tables(0).Rows.Count > 0 Then
                            PatientPay = PatientPay + dsReturned.Tables(0).Rows(0).Item("curCash") + dsReturned.Tables(0).Rows(0).Item("curCashVAT")
                            NetPrice = NetPrice + dsReturned.Tables(0).Rows(0).Item("curCredit") + dsReturned.Tables(0).Rows(0).Item("curCreditVAT")
                        End If
                    End If
                    TotalPrice = Math.Round(TotalPrice, 2, MidpointRounding.AwayFromZero)
                    Discount = Math.Round(Discount, 2, MidpointRounding.AwayFromZero)
                    TotalVAT = Math.Round(TotalVAT, 2, MidpointRounding.AwayFromZero)

                    Dim sh As New Share.Functions
                    If bCash = True Then
                        NetPrice = Math.Round(PatientPay, 2, MidpointRounding.AwayFromZero)
                        NetPriceText = sh.ToArabicLetter(NetPrice)
                        PatientPay = 0
                        PatientPayText = ""
                        divCash.Visible = False

                        ContractNo = ""
                        HolderNameEn = ""
                    Else
                        NetPrice = Math.Round(NetPrice, 2, MidpointRounding.AwayFromZero)
                        NetPriceText = sh.ToArabicLetter(NetPrice)
                        PatientPay = Math.Round(PatientPay, 2, MidpointRounding.AwayFromZero)
                        PatientPayText = "المطلوب دفعه"
                        divCash.Visible = True
                    End If
                End If
                If AutoPrint = "" Then Script.InnerHtml = "<script type=""text/javascript"">setTimeout(function () {window.print();window.close();}, 1000);</script>" Else Script.InnerHtml = ""
            Catch ex As Exception

            End Try
        End If
    End Sub
End Class