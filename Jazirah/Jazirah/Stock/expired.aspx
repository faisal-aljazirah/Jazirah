﻿<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Stock/ST.Master" CodeBehind="expired.aspx.vb" Inherits="Jazirah.expired" %>
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
                        <div class="col-md-7"><input type="text" class="form-control form-control-sm input-sm" id="txtItemSearch" placeholder="" value="" /></div>
                        <div class="col-md-2"><select id="drpWarehouseSearch" class="form-control input-sm"><%=WarehouseList %></select></div>
                        <div class="col-md-2"><input type="text" class="form-control input-sm text-md-center date-formatter dir-ltr" id="dtpPeriod" value=""><input type="hidden" id="txtDateFrom" value="" /><input type="hidden" id="txtDateTo" value="" /></div>
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
                <div class="table-responsive" id="tblReturns">
                    <div class="bg-grey text-md-center text-bold-300 bg-lighten-4 pt-2 pb-2"><img src="../app-assets/images/icons/spinner.gif" /> <%=strWait %></div>
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
  <script src="../app-assets/vendors/js/datatables/jquery.dataTables.js" type="text/javascript"></script>
  <script src="../app-assets/vendors/js/datatables/dataTables.bootstrap4.js" type="text/javascript"></script>
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

      function showModal(func, param, modal) {
          $.ajax({
              type: "POST",
              url: 'ajax.aspx/' + func,
              data: param,
              contentType: "application/json; charset=utf-8",
              dataType: "json",
              success: function (response) {
                  if (response.d.substr(0, 4) == 'Err:') {
                      msg('', response.d.substr(4, response.d.length), 'error');
                  } else {
                      $(modal).html(response.d);
                      $(modal).modal('show');
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

      $('#txtItemSearch, #dtpPeriod').keypress(function (e) {
          var key = e.which;
          if (key == 13) {
              //window.location = '?i=' + this.value;
              fillBalance();
              return false;
          }
      });

      function fillTransfer() {
          $('#tblTransfer').html('<div class="bg-grey text-md-center text-bold-300 bg-lighten-4 pt-2 pb-2"><img src="../app-assets/images/icons/spinner.gif" /> <%=strWait %></div>');
          
<%--          titem = $('#txtItemSearch').val();
          //sstatus = $('#drpStatus').val();
          tdate = $('#txtDateFrom').val();
          
          //if (sstatus == '') sstatus = '1';
          if (tdate == '') tdate = '<%=Today.ToString("yyyy-MM-dd")%>';--%>
          
          $.ajax({
              type: 'POST',
              url: '../Stock/ajax.aspx/fillTransfer',
              data: '{}',
              contentType: 'application/json; charset=utf-8',
              dataType: 'json',
              success: function (response) {
                  if (response.d.substr(0, 4) == 'Err:') {
                      msg('', response.d.substr(4, response.d.length), 'error');
                  } else {
                      $('#tblTransfer').html(response.d);
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
          fillReturns();
          $(".date-formatter").formatter({ pattern: datePattern });
          $('#dtpPeriod').on('apply.daterangepicker', function (ev, picker) {
              fillReturns();
          });
      });
      
      function removeThis(row) {
          $(row).parent().parent().remove();
          $('#txtBarcode').focus();
      }

      function getItemInfo(barcode) {
          $.ajax({
              type: 'POST',
              url: 'ajax.aspx/getItemInfo',
              data: '{strBarcode: "' + barcode + '"}',
              contentType: 'application/json; charset=utf-8',
              dataType: 'json',
              success: function (response) {
                  if (response.d.substr(0, 4) == 'Err:') {
                      msg('', response.d.substr(4, response.d.length), 'error');
                  } else {
                      $('#prtJS').html(response.d);
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

      function approveSendItems(trans, modal) {
          $.ajax({
              type: 'POST',
              url: 'ajax.aspx/approveSendItems',
              data: '{lngTransaction: ' + trans + ', modal: "' + modal + '"}',
              contentType: 'application/json; charset=utf-8',
              dataType: 'json',
              success: function (response) {
                  if (response.d.substr(0, 4) == 'Err:') {
                      msg('', response.d.substr(4, response.d.length), 'error');
                  } else {
                      $('#prtJS').html(response.d);
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

      function approveReceiveItems(trans, modal) {
          $.ajax({
              type: 'POST',
              url: 'ajax.aspx/approveReceiveItems',
              data: '{lngTransaction: ' + trans + ', modal: "' + modal + '"}',
              contentType: 'application/json; charset=utf-8',
              dataType: 'json',
              success: function (response) {
                  if (response.d.substr(0, 4) == 'Err:') {
                      msg('', response.d.substr(4, response.d.length), 'error');
                  } else {
                      $('#prtJS').html(response.d);
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

      function fillReturns() {
          $('#tblReturns').html('<div class="bg-grey text-md-center text-bold-300 bg-lighten-4 pt-2 pb-2"><img src="../app-assets/images/icons/spinner.gif" /> <%=strWait %></div>');
          sdate = $('#txtDateFrom').val();
          if (sdate == '') sdate = '<%=Today.ToString("yyyy-MM-dd")%>';
          //sdate = '<%=Today.ToString("yyyy-MM-dd")%>';
          $.ajax({
              type: 'POST',
              url: 'ajax.aspx/fillReturns',
              data: '{dateInvoice: "' + sdate + '"}',
              contentType: 'application/json; charset=utf-8',
              dataType: 'json',
              success: function (response) {
                  if (response.d.substr(0, 4) == 'Err:') {
                      msg('', response.d.substr(4, response.d.length), 'error');
                  } else {
                      order = [[3, "desc"], [0, "desc"]];
                      $('#tblReturns').html(response.d);
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

      function fillExpiredItems() {
          $('#tblExpired').html('<div class="bg-grey text-md-center text-bold-300 bg-lighten-4 pt-2 pb-2"><img src="../app-assets/images/icons/spinner.gif" /> <%=strWait %></div>');
          item = $('#txtItemSearch').val();
          edate = $('#txtDateFrom').val();
          warehouse = $('#drpWarehouseSearch').val();
          if (edate == '') edate = '<%=Today.ToString("yyyy-MM-dd")%>';
          if (warehouse == '') warehouse = 0;
          $.ajax({
              type: 'POST',
              url: 'ajax.aspx/fillExpiredItems',
              data: '{strItem: "' + item + '", dateExpiry: "' + edate + '", byteWarehouse: ' + warehouse + '}',
              contentType: 'application/json; charset=utf-8',
              dataType: 'json',
              success: function (response) {
                  if (response.d.substr(0, 4) == 'Err:') {
                      msg('', response.d.substr(4, response.d.length), 'error');
                  } else {
                      order = [[3, "desc"], [0, "desc"]];
                      $('#tblExpired').html(response.d);
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


	</script>
</asp:Content>
