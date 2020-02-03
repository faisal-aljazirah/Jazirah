<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/CP/CP.Master" CodeBehind="Unit.aspx.vb" Inherits="Jazirah.Unit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
      <link rel="stylesheet" type="text/css" href="../app-assets/vendors/css/datatables/dataTables.bootstrap4.css">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <!-- Bordered table start -->
<div class="row">
    <div class="col-xs-12">
        <div class="card">
            <div class="card-header">
                <h4 class="card-title" id="PageHeader" runat="server"></h4>
                <a class="heading-elements-toggle"><i class="icon-ellipsis font-medium-3"></i></a>
                <div class="heading-elements">
                    <ul class="list-inline mb-0">
                        <li><a data-action="collapse"><i class="icon-minus4"></i></a></li>
                        <li><a data-action="reload"><i class="icon-reload"></i></a></li>
                        <li><a data-action="expand"><i class="icon-expand2"></i></a></li>
                        <li><a data-action="close"><i class="icon-cross2"></i></a></li>
                    </ul>
                </div>
            </div>
            <div class="card-body collapse in">
                <div class="table-responsive">
                    <table class="table tablelist table-bordered mb-0">
                        <thead>
                            <tr>
					          <th><%=byteUnit%></th>
					          <th><%=strUnitEn%></th>
					          <th><%=strUnitAr%></th>
					          <th><%=curFactor%></th>
					          <th><%=strUnitGroup%></th>
					       
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="repDepartments" runat="server">
                            <ItemTemplate>
                            <tr>
					          <td><%#DataBinder.Eval(Container.DataItem, "byteUnit")%></td>
					          <td><%#DataBinder.Eval(Container.DataItem, "strUnitEn")%></td>
					          <td><%#DataBinder.Eval(Container.DataItem, "strUnitAr")%></td>
					          <td><%#DataBinder.Eval(Container.DataItem, "curFactor")%></td>
					          <td><%#DataBinder.Eval(Container.DataItem, "strUnitGroup")%></td>
					    
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
<div class="modal fade text-xs-left" id="large" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true">
</div>
<!-- Bordered table end -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterPlaceHolder" runat="server">
     <script src="../app-assets/vendors/js/datatables/jquery.dataTables.min.js" type="text/javascript"></script>
  <script src="../app-assets/vendors/js/datatables/dataTables.bootstrap4.min.js" type="text/javascript"></script>
  <script type="text/javascript">

  </script>
</asp:Content>
