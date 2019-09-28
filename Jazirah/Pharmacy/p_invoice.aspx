<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="p_invoice.aspx.vb" Inherits="Jazirah.p_invoice" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        table{
            font-size: 14px;
        }
        .auto-style1 {
            width:50px;
            text-align: center;
        }
        .auto-style2 {
            width:50px;
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
        </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <br />
        <br />
        <br />
        <br />
        <table style="width: 100%; height: 182px;">
            <tr>
                <td class="auto-style1" style="vertical-align:top; width:50%" colspan="3" rowspan="11">
                    <table style="width: 100%;">
                        <%=ItemsTable %>
                    </table>
                </td>
                   <td class="auto-style1"><%=InvoiceNo%></td>
                <td class="auto-style1"><%=PatientNameAr %></td>
            </tr>
            <tr>
                <td class="auto-style7"><%=InvoiceDate%></td>
                <td class="auto-style7"><%=PatientNameEn%></td>
            </tr>
            <tr>
                <td class="auto-style6 auto-style1"><%=InvoiceTime%></td>
                <td class="auto-style7"><%=PatientSex%></td>
            </tr>
            <tr>
                <td class="auto-style1"><%=PatientNo %></td>
                <td class="auto-style1"><%=PatientAge %></td>
            </tr>
            <tr>
                <td class="auto-style1"><%=InvoiceType%></td>
                <td>&nbsp;</td>
            </tr>
             <tr>
                <td class="auto-style1"><%=UserName%></td>
                <td>&nbsp;</td>
            </tr>
             <tr>
                <td class="auto-style1">copy</td>
                <td>   <table style="width: 100%;" class="auto-style3" > <tr>
                    <td class="auto-style8" ><%=DoctorNameEn%></td>
                <td class="auto-style9"><%=DoctorNameAr%></td>
                <td class="auto-style10" ><%=DoctorNo%></td>
                            </tr></table> </td>
               
            </tr>
             <tr>
                   <td>&nbsp;</td>
                <td> <table style="width: 100%;" class="auto-style3" > <tr>
                    <td><%=DepartmentNameEn%></td>
                <td class="auto-style2" ><%=DepartmentNameAr%></td>
                <td  style="width: 10%;" ><%=DepartmentNo%></td>
                            </tr></table> </td>
               
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td> <table style="width: 100%;" class="auto-style3" > <tr>
                    <td><%=CompanyNameEn%></td>
                <td>&nbsp;</td>
                <td style="width: 10%;" ><%=CompanyNo%></td>
            </tr>
                     </table> </td>
            </tr>
             <tr>
                   <td>&nbsp;</td>
                <td> <table style="width: 100%;" class="auto-style3" > <tr>
                    <td></td>
                <td class="auto-style2" ></td>
                <td style="width: 10%;" ></td>
                            </tr></table> </td>
            </tr>
             <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style1"><%=TotalPrice%></td>
                <td class="auto-style2">&nbsp;</td>
                <td>&nbsp;</td>
                   <td>&nbsp;</td>
                <td>&nbsp;</td>    
            </tr>
            <tr>
                <td class="auto-style1"><%=Discount%></td>
                <td class="auto-style2">&nbsp;</td>
                <td>&nbsp;</td>
                   <td>&nbsp;</td>
                <td>&nbsp;</td>
                
            </tr>  
             <tr>
                <td class="auto-style1"><%=NetPrice%></td>
                <td class="auto-style2">&nbsp;</td>
                <td><%=NetPriceText %></td>
                   <td>&nbsp;</td>
                <td class="auto-style1">&nbsp;</td>
            </tr>
        </table>
    </div>
        <div id="Script" runat="server"></div>
    </form>
</body>
</html>

