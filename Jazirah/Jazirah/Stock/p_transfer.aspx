<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="p_transfer.aspx.vb" Inherits="Jazirah.p_transfer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        body{
            font-size: 10px;
        }
        .t1 {
            font-family: "Trebuchet MS", Arial, Helvetica, sans-serif;
            border-collapse: collapse;
            width: 100%;
        }

        td, th {
            border: 1px solid black;
            padding: 8px;
            color: black;
        }

        th {
            padding-top: 12px;
            padding-bottom: 12px;
            text-align: left;
            background-color: lightgray;
        }
      
        .tab1 {
            background-color: lightgray;
            border:none;
            width:20%;
        
        }
        .tab3{
            border:none;
        }
        .bor1 {
            border: none;
        }
        .bor2 {
            background-color: lightgray;
        }
        .ta {
            font-family: "Trebuchet MS", Arial, Helvetica, sans-serif;
            font-size: 20px;
        }
        .no-border{
            border: none;
            border-style: none;
        }
        .p300{
            padding-top: 100px;
        }
        .center{
            text-align: center;
        }
    </style>
    </head>
    <body>
    <table class="t1">
        <tr><td class="tab1">Form Warehouse</td><td class="tab3"><%=FromWarehouse%></td><td class="bor1" colspan="6"></td><td class="tab1">Voucher No</td><td class="tab3"><%=VoucherNo%></td></tr>
        <tr><td class="tab1">To Warehouse</td><td class="tab3"><%=ToWarehouse%></td><td class="bor1" colspan="6" ></td><td class="tab1">Voucher Date</td><td class="tab3"><%=VoucherDate%></td></tr>       
    </table>
    <table class="t1">
        <tr>
            <th></th>
            <th>Item No</th>
            <th>Description</th>
            <th>Expiry</th>
            <th> Unit</th>
            <th>Qty</th>
            <th>Cost/U</th>
            <th>Amount</th>
            <th>Price/U</th>
            <th>Amount</th>
        </tr>
        <%=Table%>
        <tr>
            <td class="bor1" colspan="7"></td>
            <td class="bor2"><%=TotalCost %></td>
            <td class="bor1"></td>
            <td class="bor2"><%=TotalPrice%></td>
        </tr>
    </table>
    <table class="no-border p300 ta" align="center" style="width:100%">
        <tr><td style="width:50%" class="no-border center">Issued by</td><td class="no-border">&nbsp;</td><td class="no-border">&nbsp;&nbsp;</td><td class="no-border">Approval</td></tr>
        <tr><td style="width:50%" class="no-border center">.............</td><td class="no-border">&nbsp;</td><td class="no-border">&nbsp;&nbsp;</td><td class="no-border">.............</td></tr>
    </table>
    <%=PrintScript%>
    </body>
</html>
