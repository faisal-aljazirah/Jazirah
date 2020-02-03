<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Pharmacy/PH.Master" CodeBehind="invoices.aspx.vb" Inherits="Jazirah.invoices" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="../app-assets/vendors/css/datatables/dataTables.bootstrap4.css">
    <link rel="stylesheet" type="text/css" href="../app-assets/vendors/css/daterangepicker/daterangepicker.css">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
<div class="row">
    <div class="col-xs-12">
        <div class="card p-1">
            <div class="card-body">
                <div class="row">
                    <div class="col-md-12">
                        <div class="col-md-7"><input type="text" class="form-control form-control-sm input-sm" id="txtInvoicesSearch" placeholder="<%=plcInvoice%>" value="" /></div>
                        <div class="col-md-2"><input type="text" class="form-control input-sm text-md-center date-formatter dir-ltr" id="dtpPeriod" value=""><input type="hidden" id="txtDateFrom" value="" /><input type="hidden" id="txtDateTo" value="" /></div>
                        <div class="col-md-2">
                            <select class="form-control form-control-sm" id="drpStatus">
                                <option value="0"><%=InvoiceStatus(0)%></option>
                                <option value="1" selected="selected"><%=InvoiceStatus(1)%></option>
                                <option value="2"><%=InvoiceStatus(2)%></option>
                                <option value="3"><%=InvoiceStatus(3)%></option>
                            </select>
                        </div>
                        <div class="col-md-1"><button type="button" id="btnSearch" class="btn btn-sm btn-outline-orange full-width" onclick="javascript:fillInvoices();"><%=btnSearch%></button></div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="row">
    <div class="col-xs-12">
        <div class="card">
            <div class="card-body collapse in p-1">
                <div class="table-responsive" id="tblInvoices">
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
  <script src="../app-assets/vendors/js/datatables/jquery.dataTables.min.js" type="text/javascript"></script>
  <script src="../app-assets/vendors/js/datatables/dataTables.bootstrap4.min.js" type="text/javascript"></script>
  <script src="../app-assets/vendors/js/moment/moment.js" type="text/javascript"></script>
  <script src="../app-assets/vendors/js/moment/moment-with-locales.js" type="text/javascript"></script>
  <script src="../app-assets/vendors/js/daterangepicker/daterangepicker.min.js" type="text/javascript"></script>
  <script src="../app-assets/vendors/js/forms/extended/formatter/formatter.min.js" type="text/javascript"></script>
  <script type="text/javascript">
      $(document).ready(function () {
          $.getScript('../assets/js/pharmacy.js', function () {
              //
          });
      });

      $('#txtInvoicesSearch, #dtpPeriod').keypress(function (e) {
          var key = e.which;
          if (key == 13) {
              //window.location = '?i=' + this.value;
              fillInvoices();
              return false;
          }
      });

      function fillInvoices() {
          $('#tblInvoices').html('<div class="bg-grey text-md-center text-bold-300 bg-lighten-4 pt-2 pb-2"><img src="../app-assets/images/icons/spinner.gif" /> <%=strWait %></div>');
          search = $('#txtInvoicesSearch').val();
          sstatus = $('#drpStatus').val();
          sdate = $('#txtDateFrom').val();
          if (sstatus == '') sstatus = '1';
          if (sdate == '') sdate = '<%=Today.ToString("yyyy-MM-dd")%>';
          $.ajax({
              type: 'POST',
              url: 'ajax.aspx/fillInvoices',
              data: '{dateInvoice: "' + sdate + '", byteStatus: "' + sstatus + '", strSearch: "' + search + '"}',
              contentType: 'application/json; charset=utf-8',
              dataType: 'json',
              success: function (response) {
                  if (response.d.substr(0, 4) == 'Err:') {
                      msg('', response.d.substr(4, response.d.length), 'error');
                  } else {
                      order = [[3, "desc"], [0, "desc"]];
                      $('#tblInvoices').html(response.d);
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
      $(document).ready(function () {
          order = [[3, "desc"], [0, "desc"]];
          fillInvoices();
          $(".date-formatter").formatter({ pattern: datePattern });
          $('#dtpPeriod').on('apply.daterangepicker', function (ev, picker) {
              fillInvoices();
          });
      });
  </script>
</asp:Content>
