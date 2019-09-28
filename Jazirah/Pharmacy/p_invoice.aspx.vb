Public Class p_invoice
    Inherits System.Web.UI.Page
    Dim dcl As New DCL.Conn.DataClassLayer
    Dim DataLang As String

    Public PatientNo, PatientNameAr, PatientNameEn, PatientSex, PatientAge, DoctorNo, DoctorNameAr, DoctorNameEn, DepartmentNo, DepartmentNameAr, DepartmentNameEn, InvoiceNo, InvoiceDate, InvoiceTime, InvoiceType, UserName, CompanyNo, CompanyNameEn, ItemsTable, NetPriceText As String
    Public TotalPrice, Discount, NetPrice, PatientPay As Decimal

    Public ByteLanguage As Byte
    Public strUserName, strDateFormat, strTimeFormat As String

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

        Dim lngTransaction As Long
        If Request.QueryString("t") Is Nothing Then
        Else
            Try
                lngTransaction = Request.QueryString("t")
                Dim AutoPrint As String = Request.QueryString("ap")
                ds = dcl.GetDS("SELECT ST.lngPatient AS PatientNo, RTRIM(LTRIM(ISNULL(P.strFirstAr,'') + ' ') + LTRIM(ISNULL(P.strSecondAr,'') + ' ') + LTRIM(ISNULL(P.strThirdAr ,'') + ' ') + LTRIM(ISNULL(P.strLastAr,''))) AS PatientNameAr, RTRIM(LTRIM(ISNULL(P.strFirstEn,'') + ' ') + LTRIM(ISNULL(P.strSecondEn,'') + ' ') + LTRIM(ISNULL(P.strThirdEn ,'') + ' ') + LTRIM(ISNULL(P.strLastEn,''))) AS PatientNameEn, P.bSex, P.dateBirth, C1.lngContact AS DoctorNo, C1.strContactAr AS DoctorNameAr, C1.strContactEn AS DoctorNameEn, C2.lngContact AS CompanyNo, C2.strContactEn AS CompanyNameEn, D.byteDepartment AS DepartmentNo, D.strDepartmentEn AS DepartmentNameEn, D.strDepartmentAr AS DepartmentNameAr, ST.strTransaction AS InvoiceNo, ST.dateEntry AS InvoiceDate, ST.bCash AS InvoiceType, STA.strCreatedBy AS UserName, ST.lngTransaction AS TransactionNo, ST.dateTransaction AS TransactionDate, P.strID AS PatientNationalID, P.strInsuranceNo AS PatientInsuranceNo, ST.strTransaction AS InvoiceNo, ST.strReference AS ClinicInvoiceNo, STA.strCashBy AS CashierName,STA.strVoidBy AS VoidName,STA.dateVoid AS VoidDate, CASE WHEN ST.datePrepeare IS NULL THEN 0 ELSE 1 END AS TransactionStatus, ST.byteStatus AS [Status] FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Departments AS D ON ST.byteDepartment = D.byteDepartment INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact INNER JOIN Hw_Contacts AS C2 ON ST.lngContact = C2.lngContact WHERE ST.byteBase = 40 AND Year(ST.dateTransaction) = 2019 AND ST.lngTransaction = " & lngTransaction)
                If ds.Tables(0).Rows.Count > 0 Then
                    PatientNo = ds.Tables(0).Rows(0).Item("PatientNo").ToString
                    PatientNameAr = ds.Tables(0).Rows(0).Item("PatientNameAr").ToString
                    PatientNameEn = ds.Tables(0).Rows(0).Item("PatientNameEn").ToString
                    If ds.Tables(0).Rows(0).Item("bSex") = 1 Then PatientSex = "Male" Else PatientSex = "Female"
                    If Not (IsDBNull(ds.Tables(0).Rows(0).Item("dateBirth"))) Then PatientAge = DateDiff(DateInterval.Year, ds.Tables(0).Rows(0).Item("dateBirth"), Today) Else PatientAge = 0
                    DoctorNo = ds.Tables(0).Rows(0).Item("DoctorNo").ToString
                    DoctorNameAr = ds.Tables(0).Rows(0).Item("DoctorNameAr").ToString
                    DoctorNameEn = ds.Tables(0).Rows(0).Item("DoctorNameEn").ToString
                    DepartmentNo = ds.Tables(0).Rows(0).Item("DepartmentNo").ToString
                    DepartmentNameAr = ds.Tables(0).Rows(0).Item("DepartmentNameAr").ToString
                    DepartmentNameEn = ds.Tables(0).Rows(0).Item("DepartmentNameEn").ToString
                    InvoiceNo = ds.Tables(0).Rows(0).Item("InvoiceNo").ToString
                    InvoiceDate = CDate(ds.Tables(0).Rows(0).Item("InvoiceDate")).ToString("yyyy-MM-dd")
                    InvoiceTime = CDate(ds.Tables(0).Rows(0).Item("InvoiceDate")).ToString("hh:mm tt")
                    If ds.Tables(0).Rows(0).Item("InvoiceType") = 1 Then InvoiceType = "نقدي/Cash" Else InvoiceType = "آجل/Credit"
                    UserName = ds.Tables(0).Rows(0).Item("UserName").ToString
                    CompanyNo = ds.Tables(0).Rows(0).Item("CompanyNo").ToString
                    CompanyNameEn = ds.Tables(0).Rows(0).Item("CompanyNameEn").ToString

                    ' get general information
                    Dim ph As New Pharmacy.Orders

                    'Dim MaxP, CICov, MICov As Decimal
                    'If IsCash = False Then
                    '    Dim result As String() = ph.getCoverage(PatientNo, CompanyNo)
                    '    If Left(result(0), 4) <> "Err:" Then
                    '        MaxP = result(2)
                    '    Else
                    '        Return result(0)
                    '    End If
                    '    CICov = ph.getTotalClinicInvoices(PatientNo, DoctorNo, TransactionDate)
                    '    MICov = ph.getTotalPharmacyInvoices(PatientNo, DoctorNo, TransactionDate, lngTransaction, True)
                    'Else
                    '    MaxP = 0
                    '    CICov = 0
                    '    MICov = 0
                    'End If

                    ' get invoice items
                    Dim dsItems As DataSet
                    ItemsTable = ""
                    dsItems = dcl.GetDS("SELECT * FROM Stock_Xlink_Items AS XI INNER JOIN Stock_Xlink AS X ON XI.lngXlink=X.lngXlink INNER JOIN Stock_Items AS I ON XI.strItem=I.strItem WHERE X.lngTransaction=" & lngTransaction)
                    For I = 0 To dsItems.Tables(0).Rows.Count - 1
                        ItemsTable = ItemsTable & "<tr><td class=""auto-style1"">" & Math.Round(dsItems.Tables(0).Rows(I).Item("curBasePrice"), 2, MidpointRounding.AwayFromZero) & "</td><td class=""auto-style2"">" & Math.Round(dsItems.Tables(0).Rows(I).Item("curQuantity"), 2, MidpointRounding.AwayFromZero) & "</td><td class=""med"">" & dsItems.Tables(0).Rows(I).Item("strItemEn") & " " & dsItems.Tables(0).Rows(I).Item("strItem") & "</td></tr>"
                        TotalPrice = TotalPrice + dsItems.Tables(0).Rows(I).Item("curBasePrice")
                        Discount = Discount + (dsItems.Tables(0).Rows(I).Item("curBasePrice") * (dsItems.Tables(0).Rows(I).Item("curDiscount") / 100))
                        'InvoiceItems = InvoiceItems & createItemRow(lngTransaction, 1, IsCash, "<input type=""checkbox"" class=""chkItem"" value=""" & dsItems.Tables(0).Rows(I).Item("intEntryNumber") & """ />", dsItems.Tables(0).Rows(I).Item("strBarCode"), dsItems.Tables(0).Rows(I).Item("strItem"), dsItems.Tables(0).Rows(I).Item("strItem" & DataLang), dsItems.Tables(0).Rows(I).Item("byteUnit"), dsItems.Tables(0).Rows(I).Item("dateExpiry"), dsItems.Tables(0).Rows(I).Item("curBasePrice"), dsItems.Tables(0).Rows(I).Item("curDiscount"), dsItems.Tables(0).Rows(I).Item("curQuantity"), dsItems.Tables(0).Rows(I).Item("curBaseDiscount"), dsItems.Tables(0).Rows(I).Item("curCoverage"), 0, dsItems.Tables(0).Rows(I).Item("intService"), dsItems.Tables(0).Rows(I).Item("byteWarehouse"), "", False)
                    Next
                    TotalPrice = Math.Round(TotalPrice, 2, MidpointRounding.AwayFromZero)
                    Discount = Math.Round(Discount, 2, MidpointRounding.AwayFromZero)
                    NetPrice = Math.Round(TotalPrice - Discount, 2, MidpointRounding.AwayFromZero)
                    Dim sh As New Share.Functions
                    NetPriceText = sh.ToArabicLetter(NetPrice)
                End If
                If AutoPrint = "" Then Script.InnerHtml = "<script type=""text/javascript"">window.print();window.close();</script>" Else Script.InnerHtml = ""
            Catch ex As Exception

            End Try
        End If
    End Sub
End Class