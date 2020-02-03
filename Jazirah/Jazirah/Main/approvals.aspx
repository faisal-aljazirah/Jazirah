<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Main/MN.Master" CodeBehind="approvals.aspx.vb" Inherits="Jazirah.approvals" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="../app-assets/vendors/css/datatables/dataTables.bootstrap4.css">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
<div class="row" id="divCancel" runat="server">
    <div class="col-xs-12">
        <div class="card">
            <div class="card-body collapse in">
                <div class="table-responsive p-1">
                    <h3><%=lblCancelRequests%></h3>
                    <table class="table tablelist table-bordered">
                        <thead>
                            <tr>
					          <th><%=colInvoice %></th>
					          <th><%=colPatient %></th>
					          <th><%=colDoctor %></th>
					          <th><%=colDate %></th>
                              <th><%=colUser%></th>
					          <th><%=colType %></th>
					          <th><%=colRequestUser%></th>
                              <th><%=colRequestDate%></th>
                              <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="repCancel" runat="server">
                            <ItemTemplate>
                            <tr id="row<%#DataBinder.Eval(Container.DataItem, "TransactionNo")%>">
					          <td><%#DataBinder.Eval(Container.DataItem, "InvoiceNo")%></td>
					          <td><%#DataBinder.Eval(Container.DataItem, "PatientName")%></td>
					          <td><%#DataBinder.Eval(Container.DataItem, "DoctorName")%></td>
					          <td class="center"><%#CDate(DataBinder.Eval(Container.DataItem, "TransactionDate")).ToString(strDateFormat)%></td>
                              <td><%#DataBinder.Eval(Container.DataItem, "UserName")%></td>
					          <td><%#DataBinder.Eval(Container.DataItem, "PaymentType")%></td>
                              <td><%#DataBinder.Eval(Container.DataItem, "RequestUser")%></td>
                              <td class="center"><%#CDate(DataBinder.Eval(Container.DataItem, "RequestDate")).ToString(strDateFormat)%></td>
					          <td>
                                  <button type="button" onclick="javascript:viewInvoice(<%#DataBinder.Eval(Container.DataItem, "TransactionNo")%>);" class="btn btn-sm btn-primary"><%=btnView%></button>
                                  <button type="button" onclick="javascript:approveCancelRequest(<%#DataBinder.Eval(Container.DataItem, "TransactionNo")%>);" class="btn btn-sm btn-success"><%=btnApprove%></button>
                                  <button type="button" onclick="javascript:rejectCancelRequest(<%#DataBinder.Eval(Container.DataItem, "TransactionNo")%>);" class="btn btn-sm btn-danger"><%=btnReject%></button>
					          </td>
                            </tr>
                            </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="row" id="divReturn" runat="server">
    <div class="col-xs-12">
        <div class="card">
            <div class="card-body collapse in">
                <div class="table-responsive p-1">
                    <h3><%=lblReturnRequests%></h3>
                    <table class="table tablelist table-bordered">
                        <thead>
                            <tr>
					          <th><%=colInvoice %></th>
					          <th><%=colPatient %></th>
					          <th><%=colDoctor %></th>
					          <th><%=colDate %></th>
                              <th><%=colUser%></th>
					          <th><%=colType %></th>
					          <th><%=colRequestUser%></th>
                              <th><%=colRequestDate%></th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="repReturn" runat="server">
                            <ItemTemplate>
                            <tr id="row<%#DataBinder.Eval(Container.DataItem, "TransactionNo")%>">
					          <td><%#DataBinder.Eval(Container.DataItem, "InvoiceNo")%></td>
					          <td><%#DataBinder.Eval(Container.DataItem, "PatientName")%></td>
					          <td><%#DataBinder.Eval(Container.DataItem, "DoctorName")%></td>
					          <td class="center"><%#CDate(DataBinder.Eval(Container.DataItem, "TransactionDate")).ToString(strDateFormat)%></td>
                              <td><%#DataBinder.Eval(Container.DataItem, "UserName")%></td>
					          <td><%#DataBinder.Eval(Container.DataItem, "PaymentType")%></td>
                              <td><%#DataBinder.Eval(Container.DataItem, "RequestUser")%></td>
                              <td class="center"><%#CDate(DataBinder.Eval(Container.DataItem, "RequestDate")).ToString(strDateFormat)%></td>
					          <td>
                                  <button type="button" onclick="javascript:viewInvoice(<%#DataBinder.Eval(Container.DataItem, "TransactionNo")%>);" class="btn btn-sm btn-primary"><%=btnView%></button>
                                  <button type="button" onclick="javascript:approveReturnRequest(<%#DataBinder.Eval(Container.DataItem, "TransactionNo")%>);" class="btn btn-sm btn-success"><%=btnApprove%></button>
                                  <button type="button" onclick="javascript:rejectReturnRequest(<%#DataBinder.Eval(Container.DataItem, "TransactionNo")%>);" class="btn btn-sm btn-danger"><%=btnReject%></button>
					          </td>
                            </tr>
                            </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="row" id="divReopen" runat="server">
    <div class="col-xs-12">
        <div class="card">
            <div class="card-body collapse in">
                <div class="table-responsive p-1">
                    <h3><%=lblReopenRequests%></h3>
                    <table class="table tablelist table-bordered">
                        <thead>
                            <tr>
					          <th><%=colInvoice %></th>
					          <th><%=colPatient %></th>
					          <th><%=colDoctor %></th>
					          <th><%=colDate %></th>
                              <th><%=colUser%></th>
					          <th><%=colType %></th>
					          <th><%=colRequestUser%></th>
                              <th><%=colRequestDate%></th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="repReopen" runat="server">
                            <ItemTemplate>
                            <tr id="row<%#DataBinder.Eval(Container.DataItem, "TransactionNo")%>">
					          <td><%#DataBinder.Eval(Container.DataItem, "InvoiceNo")%></td>
					          <td><%#DataBinder.Eval(Container.DataItem, "PatientName")%></td>
					          <td><%#DataBinder.Eval(Container.DataItem, "DoctorName")%></td>
					          <td class="center"><%#CDate(DataBinder.Eval(Container.DataItem, "TransactionDate")).ToString(strDateFormat)%></td>
                              <td><%#DataBinder.Eval(Container.DataItem, "UserName")%></td>
					          <td><%#DataBinder.Eval(Container.DataItem, "PaymentType")%></td>
                              <td><%#DataBinder.Eval(Container.DataItem, "RequestUser")%></td>
                              <td class="center"><%#CDate(DataBinder.Eval(Container.DataItem, "RequestDate")).ToString(strDateFormat)%></td>
					          <td>
                                  <button type="button" onclick="javascript:viewInvoice(<%#DataBinder.Eval(Container.DataItem, "TransactionNo")%>);" class="btn btn-sm btn-primary"><%=btnView%></button>
                                  <button type="button" onclick="javascript:approveReopenRequest(<%#DataBinder.Eval(Container.DataItem, "TransactionNo")%>);" class="btn btn-sm btn-success"><%=btnApprove%></button>
                                  <button type="button" onclick="javascript:rejectReopenRequest(<%#DataBinder.Eval(Container.DataItem, "TransactionNo")%>);" class="btn btn-sm btn-danger"><%=btnReject%></button>
					          </td>
                            </tr>
                            </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
</div>
<div id="prtJS"></div>
<div class="modal fade" id="mdlAlpha" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true"></div>
<div class="modal fade" id="mdlBeta" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true"></div>
<div class="modal fade" id="mdlGamma" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true"></div>
<div class="modal fade" id="mdlInfo" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true" data-backdrop="false"></div>
<div class="modal fade" id="mdlMessage" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true" data-backdrop="false"></div>
<div class="modal fade" id="mdlConfirm" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true" data-backdrop="false"></div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterPlaceHolder" runat="server">
  <script src="../app-assets/vendors/js/datatables/jquery.dataTables.min.js" type="text/javascript"></script>
  <script src="../app-assets/vendors/js/datatables/dataTables.bootstrap4.min.js" type="text/javascript"></script>
  <script src="../app-assets/vendors/js/autocomplete/jquery.autocomplete.min.js" type="text/javascript"></script>
  <script type="text/javascript">
      $(document).ready(function () {
          $.getScript('../assets/js/pharmacy.js', function () {
              //
          });
      });
  </script>
</asp:Content>
