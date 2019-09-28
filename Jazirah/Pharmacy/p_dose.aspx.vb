Public Class p_dose
    Inherits System.Web.UI.Page
    Dim dcl As New DCL.Conn.DataClassLayer
    Dim DataLang As String
    Dim Cash, Insurance, All As String
    Public btnView, btnPay, btnReturn As String

    Public colInvoice, colPatient, colDoctor, colDate, colDepartment, colCompany, colType, colUser, colStatus As String

    Public ByteLanguage As Byte
    Public strUserName, strDateFormat, strTimeFormat As String

    Dim ChangeQuantity_Cash, AddDiscount_Cash, ChangeQuantity_Insurance, AddDiscount_Insurance, AllowExtraItem_Insurance, AutoMoveRejectedToCash_Insurance, AskBeforeSend, AskBeforeReturn As Boolean
    Dim SusbendMax, byteDepartment_Cash As Byte
    Dim byteInvoicesLimitDays As Byte

    Dim PageCount As Integer = 1

    Private Sub medicinesorders_Init(sender As Object, e As EventArgs) Handles Me.Init
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

        byteInvoicesLimitDays = 3
        ' User (Application) Options and Permissions
        ChangeQuantity_Cash = True ' True = Enable user to modify quantity of an item, False = add (1) quantity for each barcode read
        AddDiscount_Cash = False
        ChangeQuantity_Insurance = False ' True = Enable user to modify quantity of an item, False = add (1) quantity for each barcode read
        AddDiscount_Insurance = False
        AllowExtraItem_Insurance = True ' True = Allow more amount of approved item, False = Allow Exact amount or less
        AutoMoveRejectedToCash_Insurance = False 'True = move automaticily, False = Popup a confirm message
        SusbendMax = 5 ' Set a maximum number of suspend invoices
        AskBeforeSend = True 'True = Ask before send invoice to cashier, False = Send Directly
        AskBeforeReturn = True 'True = Ask before return invoice to sales, False = Return Directly
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim lngTransaction As Long
        Dim body As String = ""
        divBody.InnerHtml = ""
        If (Not (Request.QueryString("t") Is Nothing)) And (Not (Request.QueryString("i") Is Nothing)) And (Not (Request.QueryString("e") Is Nothing)) Then
            Select Case ByteLanguage
                Case 2
                    DataLang = "Ar"
                Case Else
                    DataLang = "En"
            End Select
            Try
                lngTransaction = Request.QueryString("t")
                Dim Items As String() = Split(Request.QueryString("i"), ",")
                Dim Dates As String() = Split(Request.QueryString("e"), ",")
                Dim AutoPrint As String = Request.QueryString("ap")
                Dim dsTrans, dsDose As DataSet
                dsTrans = dcl.GetDS("SELECT ST.lngTransaction, ST.lngPatient, P.bSex, P.dateBirth, RTRIM(LTRIM(ISNULL(P.strFirstEn,'') + ' ') + LTRIM(ISNULL(P.strSecondEn,'') + ' ') + LTRIM(ISNULL(P.strThirdEn ,'') + ' ') + LTRIM(ISNULL(P.strLastEn,''))) AS PatientNameEn, RTRIM(LTRIM(ISNULL(P.strFirstAr,'') + ' ') + LTRIM(ISNULL(P.strSecondAr,'') + ' ') + LTRIM(ISNULL(P.strThirdAr ,'') + ' ') + LTRIM(ISNULL(P.strLastAr,''))) AS PatientNameAr, ST.dateEntry, C1.lngContact, C1.strContactEn, ST.strReference, STA.strCreatedBy AS UserName FROM Stock_Trans AS ST LEFT JOIN Stock_Trans_Audit AS STA ON STA.lngTransaction = ST.lngTransaction INNER JOIN Hw_Patients AS P ON ST.lngPatient = P.lngPatient INNER JOIN Hw_Contacts AS C1 ON ST.lngSalesman = C1.lngContact WHERE ST.lngTransaction=" & lngTransaction)
                If dsTrans.Tables(0).Rows.Count > 0 Then
                    Dim counter As Integer = 0
                    Dim Sex As Char = "M"
                    Dim Age As Integer = 0
                    If Not (IsDBNull(dsTrans.Tables(0).Rows(0).Item("dateBirth"))) Then Age = DateDiff(DateInterval.Year, dsTrans.Tables(0).Rows(0).Item("dateBirth"), Today)
                    If dsTrans.Tables(0).Rows(0).Item("bSex") = 1 Then Sex = "M" Else Sex = "F"
                    For Each item As String In Items
                        dsDose = dcl.GetDS("SELECT SI.strItemEn, SI.strItemAr, HTP.Moredetails, HTP.Notes, ISNULL(PQM.strQtyEn,'') + ' ' + ISNULL(PDM.strDoseEn,'') + ' ' + ISNULL(PRM.strRepetitionEn,'') + ' ' + ISNULL(PTM.strTimeEn,'') + ' ' + ISNULL(PPM.strPeriodEn,'') AS DoseEn, ISNULL(PQM.strQtyAr,'') + ' ' + ISNULL(PDM.strDoseAr,'') + ' ' + ISNULL(PRM.strRepetitionAr,'') + ' ' + ISNULL(PTM.strTimeAr,'') + ' ' + ISNULL(PPM.strPeriodAr,'') AS DoseAr FROM Stock_Trans AS ST INNER JOIN Hw_Treatments_Pharmacy AS HTP ON ST.strReference=HTP.strReference AND ST.lngPatient=HTP.lngPatient INNER JOIN Stock_Items AS SI ON HTP.strItem=SI.strItem LEFT JOIN Hw_Medicines_Approval AS HDA ON ST.lngPatient=HDA.lngPatient AND ST.strReference=HDA.strReference AND HTP.strItem=HDA.strItem LEFT JOIN Ph_Qty_Med AS PQM ON PQM.byteQty = SUBSTRING(HTP.strDose,0,2) LEFT JOIN Ph_Dose_Med AS PDM ON PDM.byteDose = SUBSTRING(HTP.strDose,3,3) LEFT JOIN Ph_Repetition_Med AS PRM ON PRM.byteRepetition = SUBSTRING(HTP.strDose,6,2) LEFT JOIN Ph_Time_Med AS PTM ON PTM.byteTime = SUBSTRING(HTP.strDose,8,2) LEFT JOIN Ph_Period_Med AS PPM ON PPM.bytePeriod = SUBSTRING(HTP.strDose,10,2) WHERE ST.lngTransaction=" & lngTransaction & " AND SI.strItem='" & item & "'")
                        If dsDose.Tables(0).Rows.Count > 0 Then
                            If PageCount > 1 Then body = body & "<P style=""page-break-before: always"">"
                            body = body & "<table>"
                            body = body & "<tr><td class=""half left"">Khubar الخبر 8930044</td><td class=""half right"">Jazirah Pharmacy صيدلية الجزيرة</td></tr>"
                            body = body & "<tr><td class=""center"">" & dsTrans.Tables(0).Rows(0).Item("strContactEn") & "</td><td class=""center"">30/07/2019</td></tr>"
                            body = body & "<tr><td class=""half left"">" & dsTrans.Tables(0).Rows(0).Item("PatientNameEn") & "</td><td class=""half right"">" & dsTrans.Tables(0).Rows(0).Item("PatientNameAr") & "</td></tr>"
                            body = body & "<tr><td class=""half right"">Sex : " & Sex & "</td><td class=""half left"">Age : " & Age & "</td></tr>"
                            body = body & "<tr><td class=""half left"">" & dsDose.Tables(0).Rows(0).Item("strItemEn") & "</td><td class=""half right"">" & dsDose.Tables(0).Rows(0).Item("strItemAr") & "</td></tr>"
                            body = body & "<tr><td class=""half left"">How to use:</td><td class=""half right"">طريقة الاستخدام:</td></tr>"
                            body = body & "<tr><td class=""half left"">" & dsDose.Tables(0).Rows(0).Item("DoseEn") & "</td><td class=""half right"">" & dsDose.Tables(0).Rows(0).Item("DoseAr") & "</td></tr>"
                            body = body & "<tr><td colspan=""2"" class=""center"" >EXP DATE : " & CDate(Dates(counter)).ToString("MM/yyyy") & " تاريخ الانتهاء:</td></tr>"
                            body = body & "<tr><td class=""half left"">Note</td><td class=""half right"">ملاحظات</td></tr>"
                            body = body & "<tr><td colspan=""2"" class=""center"">" & dsDose.Tables(0).Rows(0).Item("Moredetails") & " " & dsDose.Tables(0).Rows(0).Item("Notes") & "</td></tr>"
                            body = body & "<tr><td colspan=""2"" class=""center fit"">" & strUserName & "</td></tr>"
                            body = body & "</table>"
                        End If
                        PageCount = PageCount + 1
                    Next
                End If
                If AutoPrint = "" Then divBody.InnerHtml = body & "<script type=""text/javascript"">window.print();window.close();</script>" Else divBody.InnerHtml = body
            Catch ex As Exception
                'divBody.InnerHtml = "<script type=""text/javascript"">window.close();</script>"
            End Try
        End If
    End Sub

End Class