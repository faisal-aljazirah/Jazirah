<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Pharmacy/PH.Master" CodeBehind="r_cash.aspx.vb" Inherits="Jazirah.r_cash" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="../app-assets/vendors/css/datatables/dataTables.bootstrap4.css">
    <link rel="stylesheet" type="text/css" href="../app-assets/vendors/css/datatables/buttons.bootstrap4.css">
    <link rel="stylesheet" type="text/css" href="../app-assets/vendors/css/daterangepicker/daterangepicker.css">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
<div class="row">
    <div class="col-xs-10">
        <div class="card">
            <div class="card-body bg-grey bg-lighten-3">
                <div class="card-block" id="divFilter">
					
			    </div>
            </div>
        </div>
    </div>
    <div class="col-xs-2">
        <div class="card">
            <div class="card-body">
                <div class="card-block">
					<button type="button" class="btn btn-outline-deep-orange full-width" onclick="javascript:filterReport(filter)"><%=btnSearch%></button>
			    </div>
            </div>
        </div>
    </div>
</div>
<div class="row">
    <div class="col-xs-12">
        <div class="card p-1">
            <div class="card-body">
                <div class="table-responsive" id="tblSalesReport">
                    
                </div>
            </div>
        </div>
    </div>
</div>
<div id="prtJS"></div>
<div class="modal fade text-xs-left" id="mdlFilter" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true"></div>
<div class="modal fade" id="mdlInfo" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true" data-backdrop="false"></div>
<div class="modal fade" id="mdlMessage" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true" data-backdrop="false"></div>
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
  <script src="../app-assets/vendors/js/moment/moment.js" type="text/javascript"></script>
  <script src="../app-assets/vendors/js/moment/moment-with-locales.js" type="text/javascript"></script>
  <script src="../app-assets/vendors/js/daterangepicker/daterangepicker.min.js" type="text/javascript"></script>
  <script src="../app-assets/vendors/js/forms/extended/formatter/formatter.min.js" type="text/javascript"></script>
  <script type="text/javascript">
      //moment.locale('ar-sa');
      //dom = 'Bfrtip';
      //buttons = "[{extend: 'print',text: 'Print current page',autoPrint: false}]";
      //buttons = "['print']";
      function filterReport(filter) {
          $.ajax({
              type: 'POST',
              url: 'ajax.aspx/filterReport',
              data: '{Source: "Cash", Filter: "' + filter + '"}',
              contentType: 'application/json; charset=utf-8',
              dataType: 'json',
              success: function (response) {
                  if (response.d.substr(0, 4) == 'Err:') {
                      msg('', response.d.substr(4, response.d.length), 'error');
                  } else {
                      $('#mdlFilter').html(response.d);
                      $('#mdlFilter').modal('show');
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
          if ($("body").hasClass("rtl")) {
              btnCopy = 'نسخ';
              btnExcel = 'إكسل';
              btnPrint = 'طباعة';
              btnColumns = 'الأعمدة';
          } else {
              btnCopy = 'Copy';
              btnExcel = 'Excel';
              btnPrint = 'Print';
              btnColumns = 'Columns';
          }
          def = [{ "visible": false, "targets": [] }];
          dom = "<'row'<'col-sm-12 col-md-4'l><'col-sm-12 col-md-4 text-md-center'B><'col-sm-12 col-md-4'f>><'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>";
          buttons = [
              {
                  extend: 'copy',
                  text: btnCopy,
                  className: 'btn btn-secondry btn-sm',
                  exportOptions: {
                      columns: ':visible'
                  }
              }, {
                  extend: 'csv',
                  text: btnExcel,
                  className: 'btn btn-secondry btn-sm',
                  exportOptions: {
                      columns: ':visible'
                  }
              }, {
                  extend: 'print',
                  title: '',
                  footer: true,
                  messageTop: '',
                  text: btnPrint,
                  autoPrint: true,
                  className: 'btn btn-secondry btn-sm',
                  exportOptions: {
                      columns: ':visible'
                  },
                  customize: function (win) {
                      $(win.document.body)
                          .css('font-size', '10pt')
                          .prepend(
                              '<h1>Aljazirah</h1>'
                          );

                      $(win.document.body).find('table')
                          .addClass('compact')
                          .css('font-size', 'inherit');
                      $(win.document.body).find('#cal')
                          .remove();
                  }
              }, {
                  extend: 'colvis',
                  text: btnColumns,
                  className: 'btn btn-secondry btn-sm',
                  columns: ':gt(0)'
              }
          ];
      });

      //$('.table').DataTable({
      //    dom: "<'row'<'col-sm-12 col-md-4'l><'col-sm-12 col-md-4 text-md-center'B><'col-sm-12 col-md-4'f>><'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
      //    buttons: ['copy', 'csv', { extend: 'print', text: 'Print', autoPrint: false, className: 'btn btn-secondry btn-sm', exportOptions: { columns: ':visible' } }, { extend: 'colvis', text: 'Columns', className: 'btn btn-secondry btn-sm', columns: ':gt(0)' }]
      //});
      var filter = '<%=strFilter%>';

      function fillCashReport(f) {
          $('#tblSalesReport').html('<div class="bg-grey text-md-center text-bold-300 bg-lighten-4 pt-2 pb-2"><img src="../app-assets/images/icons/spinner.gif" /> <%=strWait %></div>');
          $.ajax({
              type: 'POST',
              url: 'ajax.aspx/fillCashReport',
              data: '{Filter: "' + f + '"}',
              contentType: 'application/json; charset=utf-8',
              dataType: 'json',
              success: function (response) {
                  if (response.d.substr(0, 4) == 'Err:') {
                      msg('', response.d.substr(4, response.d.length), 'error')
                  } else {
                      $('#tblSalesReport').html(response.d);
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

      var dateFormat = '<%=strDateFormat%>';
      var datePattern = dateFormat.replace('yyyy', '{{9999}}').replace('MM', '{{99}}').replace('dd', '{{99}}');

      fillCashReport(filter);
	</script>
</asp:Content>
