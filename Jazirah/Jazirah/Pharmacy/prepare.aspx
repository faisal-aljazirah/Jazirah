<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Pharmacy/PH.Master" CodeBehind="prepare.aspx.vb" Inherits="Jazirah.prepare" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="../app-assets/vendors/css/datatables/dataTables.bootstrap4.css">
    <link rel="stylesheet" type="text/css" href="../app-assets/vendors/css/forms/selects/select2.min.css">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
<div class="row">
    <div class="col-xs-12" style="min-height:600px;">
        <div class="card">
            <div class="card-body collapse in p-1 mb-1">
                <div class="col-md-12 text-md-center mb-1">
                    <%=getActiveDoctorList1("fillPrepare")%>
                </div>
                <div class="table-responsive" id="tblOrders">
                    <div class="bg-grey text-md-center text-bold-300 bg-lighten-4 pt-2 pb-2"><img src="../app-assets/images/icons/spinner.gif" /> <%=strWait %></div>
                </div>
            </div>
        </div>
    </div>
</div>
<div id="prtJS"></div>
<div class="modal fade" id="mdlAlpha" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true" data-backdrop="static"></div>
<div class="modal fade" id="mdlBeta" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true" data-backdrop="static"></div>
<div class="modal fade" id="mdlGamma" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true" data-backdrop="static"></div>
<div class="modal fade" id="mdlInfo" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true" data-backdrop="false"></div>
<div class="modal fade" id="mdlMessage" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true" data-backdrop="false"></div>
<div class="modal fade" id="mdlConfirm" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true" data-backdrop="false"></div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterPlaceHolder" runat="server">
  <script src="../app-assets/vendors/js/datatables/jquery.dataTables.js" type="text/javascript"></script>
  <script src="../app-assets/vendors/js/datatables/dataTables.bootstrap4.js" type="text/javascript"></script>
  <script src="../app-assets/vendors/js/datatables/dataTables.buttons.js" type="text/javascript"></script>
  <script src="../app-assets/vendors/js/datatables/buttons.bootstrap4.js" type="text/javascript"></script>
  <script src="../app-assets/vendors/js/datatables/buttons.colVis.js" type="text/javascript"></script>
  <script src="../app-assets/vendors/js/datatables/buttons.flash.js" type="text/javascript"></script>
  <script src="../app-assets/vendors/js/datatables/buttons.html5.js" type="text/javascript"></script>
  <script src="../app-assets/vendors/js/datatables/buttons.print.js" type="text/javascript"></script>
  <script src="../app-assets/vendors/js/autocomplete/jquery.autocomplete.min.js" type="text/javascript"></script>
  <script src="../app-assets/vendors/js/forms/selects/select2.full.min.js" type="text/javascript"></script>
  <script type="text/javascript">
      $(document).ready(function () {
          $.getScript('../assets/js/pharmacy.js', function () {
              //
          });
          $.getScript('../assets/js/stock.js', function () {
              //
          });
      });
      
      function fillPrepare() {
          $('#tblOrders').html('<div class="bg-grey text-md-center text-bold-300 bg-lighten-4 pt-2 pb-2"><img src="../app-assets/images/icons/spinner.gif" /> <%=strWait %></div>');
          //if (department == '') department = 0;
          //if (salesman == '') salesman = 0;
          //if (prepare == '' || typeof (prepare) == 'undefined') prepare = true;
          $.ajax({
              type: 'POST',
              url: 'ajax.aspx/fillPrepare',
              data: '{}',
              contentType: 'application/json; charset=utf-8',
              dataType: 'json',
              success: function (response) {
                  if (response.d.substr(0, 4) == 'Err:') {
                      msg('', response.d.substr(4, response.d.length), 'error');
                  } else {
                      order = [[3, "desc"], [0, "desc"]];
                      $('#tblOrders').html(response.d);
                  }
              },
              failure: function (msg) {
                  alert(msg);
              },
              error: function (xhr, ajaxOptions, thrownError) {
                  alert("Load Form, update form error! " + xhr.status + " error =" + thrownError + " xhr.responseText = " + xhr.responseText);
              }
          });
      }

      $(document).ready(function () {
          order = [[3, "desc"], [0, "desc"]];
          fillPrepare();
          $('.select2').select2();
      });
  </script>
</asp:Content>
