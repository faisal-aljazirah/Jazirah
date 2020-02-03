<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="p_invoice.aspx.vb" Inherits="Jazirah.p_invoice" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        body{
            padding: 0px;
            margin: 0px;
        }
        table{
            font-size: 14px;
        }
        .auto-style1 {
            width:2cm;
            text-align: center;
        }
        .auto-style2 {
            width:1cm;
            text-align: center;
        }
        .p{
             text-align: center;
             width:10px;
        }
        
        .auto-style3 {
            font-size: x-small;
        }
        
        .auto-style6 {
            height: 32px;
        }
        .auto-style7 {
            text-align: center;
            height: 32px;
        }
        
        .med{
            text-align: left;
            padding-left:10px;
        }
        .auto-style8 {
            height: 24px;
        }
        .auto-style9 {
            text-align: right;
            height: 24px;
        }
        .auto-style10 {
            width: 10%;
            height: 24px;
        }
        .center{
            text-align: center;
        }
        .left{
            text-align: left;
        }
        .right{
            text-align: right;
        }
        .col100{
            width: 100%;
        }
        .col50{
            width: 50%;
        }
        .col40{
            width: 4cm;
        }
        .col10{
            width: 1cm;
        }
        .col15{
            width: 15%;
        }
        .col25{
            width: 25%;
        }
        .col5{
            width: 5%;
        }
        .col20{
            width: 3cm;
        }
        .col30{
            width: 4cm;
        }
        </style>
</head>
<body>
    <form id="form1" runat="server" style="margin:0px; padding:0px;">
    <div>
        <p style="height:2.2cm">

        </p>
        <table>
            <tr>
                <td style="vertical-align:top; width:10.3cm; height: 7.1cm;">
                    <table>
                        <%=ItemsTable %>
                    </table>
                </td>
                <td style="vertical-align:top; width:10.5cm">
                    <table>
                        <tr>
                            <td class="center col20"><%=InvoiceNo%></td>
                            <td class="center" colspan="3"><%=PatientNameAr %></td>
                        </tr>
                        <tr>
                            <td class="center col20"><%=InvoiceDate%></td>
                            <td class="center" colspan="3"><%=PatientNameEn%></td>
                        </tr>
                        <tr>
                            <td class="center col20"><%=InvoiceTime%></td>
                            <td class="center" colspan="3"><%=PatientSex%></td>
                        </tr>
                        <tr>
                            <td class="center col20"><%=PatientNo %></td>
                            <td class="center" colspan="3"><%=PatientAge %></td>
                        </tr>
                        <tr>
                            <td class="center col20"><%=InvoiceType%></td>
                            <td class="center col15"></td>
                            <td class="center col40"></td>
                            <td class="center col25"></td>
                        </tr>
                        <tr>
                            <td class="center col20"><%=UserName%></td>
                            <td class="center col15"></td>
                            <td class="center col40"></td>
                            <td class="center col25"></td>
                        </tr>
                        <tr>
                            <td class="center col20"></td>
                            <td class="left col15"><%=DoctorNo%></td>
                            <td class="left col40"><%=DoctorNameEn%></td>
                            <td class="right col25"><%=DoctorNameAr%></td>
                          
                        </tr>
                        <tr>
                            <td class="center col20"></td>
                            <td class="left col15"><%=DepartmentNo%></td>
                            <td class="left col40"><%=DepartmentNameEn%></td>
                            <td class="right col25"><%=DepartmentNameAr%></td>
                         
                        </tr>
                        <tr>
                            <td class="center col20"></td>
                             <td class="left col15"><%=CompanyNo%></td>
                            <td class="left" colspan="2"><%=CompanyNameEn%></td>
                           
                        </tr>
                        <tr>
                            <td class="center col20"></td>
                             <td class="left col15"></td>
                            <td class="right" colspan="2"><%=CompanyNameAr%></td>
                           
                        </tr>
                        <tr>
                            <td class="center col20"></td>
                             <td class="left col15"><%=ContractNo %></td>
                            <td class="left" colspan="2"><%=HolderNameEn%></td>
                           
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="vertical-align:top; width:50%;">
                    <table>
                        <tr>
                            <td class="auto-style1"><%=TotalPrice%></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td class="auto-style1"><%=Discount%></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td class="auto-style1"><%=TotalVAT%></td>
                            <td class="left"><%=VATText%></td>
                        </tr>
                        <tr>
                            <td class="auto-style1"><%=NetPrice%></td>
                            <td class="right"><%=NetPriceText%></td>
                        </tr>
                        <tr id="divCash" runat="server">
                            <td class="auto-style1"><%=PatientPay%></td>
                            <td class="left"><%=PatientPayText %></td>
                        </tr>
                    </table>
                </td>
                <td></td>
            </tr>
        </table>
    </div>
        <span id="Script" runat="server"></span>
    </form>
</body>
</html>

