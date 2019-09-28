<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Pharmacy/PH.Master" CodeBehind="sales.aspx.vb" Inherits="Jazirah.sales" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="../app-assets/vendors/css/datatables/dataTables.bootstrap4.css">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
<div class="row" style="margin-bottom:-100px;">
    <div class="col-xs-10">
        <div class="card p-1">
            <div class="card-body">
                <div class="row">
                    <div class="col-md-12">
                        <%=getActiveDoctorList() %>
                    </div>
                </div>
                <div class="row pt-1">
                        <div class="col-md-3">
                         <%=getSearchText() %>
                        </div>
                        <div class="col-md-3">
                        <%=getDepartmentList() %>
                        </div>
                        <div class="col-md-5">
                        <%=getDoctorList() %>
                        </div>
                        <div class="col-md-1">
                                    <div class="btn-group">
                                        <button type="button" class="btn btn-primary dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><span class="icon-database"></span></button>
                                        <div class="dropdown-menu">
                                            <a class="dropdown-item" href="#">Action</a>
                                            <a class="dropdown-item" href="#">Another action</a>
                                            <a class="dropdown-item" href="#">Something else here</a>
                                            <div class="dropdown-divider"></div>
                                            <a class="dropdown-item" href="#">Separated link</a>
                                        </div>
                                    </div>
                        </div>
                </div>
            </div>
        </div>
    </div>
    <div class="col-xs-2">
        <div class="card p-1">
            <div class="card-body collapse in">
                <div class="row text-md-center">
                    <button type="button" class="btn btn-success width-80-per" onclick="javascript:prepareOrder(-1);"><%=btnCashInvoice%></button>
                </div>
                <div class="row pt-1 text-md-center">
                    <button type="button" class="btn btn-secondary width-80-per" onclick="javascript:void(-1);"><%=btnCashBox %></button>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="row">
    <div class="col-xs-12">
        <div class="card">
            <div class="card-body collapse in">
                <div class="table-responsive" id="tblOrders">
                    <table class="table tablelist table-bordered mb-0">
                        <thead>
                            <tr>
					          <th><%=colInvoice %></th>
					          <th><%=colPatient %></th>
					          <th><%=colDoctor %></th>
					          <th><%=colDate %></th>
					          <th><%=colDepartment %></th>
					          <th><%=colCompany %></th>
					          <th><%=colType %></th>
					          <th><%=colStatus %></th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="repInvoices" runat="server">
                            <ItemTemplate>
                            <tr id="row<%#DataBinder.Eval(Container.DataItem, "TransactionNo")%>">
					          <td><%#DataBinder.Eval(Container.DataItem, "InvoiceNo")%></td>
					          <td><%#DataBinder.Eval(Container.DataItem, "PatientName")%></td>
					          <td><%#DataBinder.Eval(Container.DataItem, "DoctorName")%></td>
					          <td class="center"><%#CDate(DataBinder.Eval(Container.DataItem, "InvoiceDate")).ToString(strDateFormat)%></td>
					          <td><%#DataBinder.Eval(Container.DataItem, "DepartmentName")%></td>
					          <td><%#DataBinder.Eval(Container.DataItem, "CompanyName")%></td>
					          <td><%#DataBinder.Eval(Container.DataItem, "PaymentType")%></td>
					          <td><%#showStatus(DataBinder.Eval(Container.DataItem, "TransactionNo"), DataBinder.Eval(Container.DataItem, "TransactionStatus"))%></td>
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
    <button type="button" style="visibility:hidden;" class="full-width fit" onclick="fillOrders()">Reload</button>
<div id="prtJS"></div>
<div class="modal fade text-xs-left" id="mdlSales" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true"></div>
<div class="modal fade text-xs-left" id="mdlPrepare" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true"></div>
<div class="modal fade" id="mdlInfo" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true" data-backdrop="false"></div>
<div class="modal fade" id="mdlMessage" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true" data-backdrop="false"></div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterPlaceHolder" runat="server">
  <script src="../app-assets/vendors/js/datatables/jquery.dataTables.min.js" type="text/javascript"></script>
  <script src="../app-assets/vendors/js/datatables/dataTables.bootstrap4.min.js" type="text/javascript"></script>
  <script src="../app-assets/vendors/js/autocomplete/jquery.autocomplete.min.js" type="text/javascript"></script>
  <script type="text/javascript">
      function fillOrders() {
          $.ajax({
              type: 'POST',
              url: 'ajax.aspx/fillOrders',
              data: '',
              contentType: 'application/json; charset=utf-8',
              dataType: 'json',
              success: function (response) {
                  if (response.d.substr(0, 4) == 'Err:') {
                      //msg('', response.d.substr(4, response.d.length) + ' ' + patient + ',' + tdate + ',' + reference, 'error');
                      msg('', response.d.substr(4, response.d.length), 'error');
                  } else {
                      //var item = '<tr><td>' + barcode + '</td><td>' + response.d + '</td><td>Item Name</td><td>Amount</td><td>Expire Date</td><td>Price</td><td>Discount</td><td>Total</td><td class=""text-nowrap""><a href=""#"" class=""tag tag-info tag-xs"">Print</a> <a href=""#"" class=""tag tag-danger tag-xs"">Delete</button></td></tr>';
                      //$(item).insertBefore("#last");
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

      function getItemInfo(barcode, transaction, coverage, total, row, I_items, C_items) {
          $.ajax({
              type: 'POST',
              url: 'ajax.aspx/getItemInfo',
              data: '{strBarcode: "' + barcode + '", lngTransaction: "' + transaction + '", curCoverage: "' + coverage + '", curBasePriceTotal: "' + total + '", RowCounter: "' + row + '", SelectedInsuranceItems: "' + I_items + '", SelectedCashItems: "' + C_items + '"}',
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
      
      function completeBarcode(barcode) {
          $.ajax({
              type: 'POST',
              url: 'ajax.aspx/completeBarcode',
              data: '{strBarcode: "' + barcode + '"}',
              contentType: 'application/json; charset=utf-8',
              dataType: 'json',
              success: function (response) {
                  if (response.d.substr(0, 4) == 'Err:') {
                      msg('', response.d.substr(4, response.d.length), 'error');
                  } else {
                      $('#mdlMessage').html(response.d);
                      $('#mdlMessage').modal('show');
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

      function suspendInvoice(transaction, cashOnly, IItems, CItems, status) {
          var func;
          if (status == '0') func = 'SuspendInvoice'; else func = 'UnsuspendInvoice';
          $.ajax({
              type: 'POST',
              url: 'ajax.aspx/' + func,
              data: '{lngTransaction: "' + transaction + '", CashOnly: "' + cashOnly + '", InsuranceItems: "' + IItems + '", CashItems: "' + CItems + '"}',
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
      PNotify.defaults.styling = 'bootstrap4';

      function getPaid1(transaction, cash, span, type) {
          $.ajax({
              type: 'POST',
              url: 'ajax.aspx/getPaid1',
              data: '{lngTransaction: "' + transaction + '", P_Cash: "' + cash + '", P_SPAN: "' + span + '", PaymentType: "' + type + '"}',
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

      function getPaid2(tabCount, fields, cash, span, type) {
          $.ajax({
              type: 'POST',
              url: 'ajax.aspx/getPaid2',
              data: '{TabCounter: "' + tabCount + '", Fields: "' + fields + '", P_Cash: "' + cash + '", P_SPAN: "' + span + '", PaymentType: "' + type + '"}',
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

      function showOrder(transNo) {
          $.ajax({
              type: "POST",
              url: 'ajax.aspx/viewOrder',
              data: '{TransNo: ' + transNo + '}',
              contentType: "application/json; charset=utf-8",
              dataType: "json",
              success: function (response) {
                  if (response.d.substr(0, 4) == 'Err:') {
                      msg('', response.d.substr(4, response.d.length), 'error');
                  } else {
                      $("#mdlSales").html(response.d);
                      $("#mdlSales").modal('show');
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
      function viewInvoice(transNo) {
          $.ajax({
              type: "POST",
              url: 'ajax.aspx/viewInvoice',
              data: '{TransNo: ' + transNo + '}',
              contentType: "application/json; charset=utf-8",
              dataType: "json",
              success: function (response) {
                  if (response.d.substr(0, 4) == 'Err:') {
                      msg('', response.d.substr(4, response.d.length), 'error');
                  } else {
                      $("#mdlSales").html(response.d);
                      $("#mdlSales").modal('show');
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

      function showCashier(transNo) {
          $.ajax({
              type: "POST",
              url: 'ajax.aspx/viewCashier',
              data: '{lngTransaction: ' + transNo + '}',
              contentType: "application/json; charset=utf-8",
              dataType: "json",
              success: function (response) {
                  if (response.d.substr(0, 4) == 'Err:') {
                      msg('', response.d.substr(4, response.d.length), 'error');
                  } else {
                      $("#mdlMessage").html(response.d);
                      $("#mdlMessage").modal('show');
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

      function showInfo(transNo) {
          $.ajax({
              type: "POST",
              url: 'ajax.aspx/viewInfo',
              data: '{lngTransaction: ' + transNo + '}',
              contentType: "application/json; charset=utf-8",
              dataType: "json",
              success: function (response) {
                  if (response.d.substr(0, 4) == 'Err:') {
                      msg('', response.d.substr(4, response.d.length), 'error');
                  } else {
                      $("#mdlInfo").html(response.d);
                      $("#mdlInfo").modal('show');
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

      function prepareOrder(transNo) {
          $.ajax({
              type: "POST",
              url: 'ajax.aspx/prepareOrder',
              data: '{TransNo: ' + transNo + '}',
              contentType: "application/json; charset=utf-8",
              dataType: "json",
              success: function (response) {
                  if (response.d.substr(0, 4) == 'Err:') {
                      msg('', response.d.substr(4, response.d.length), 'error');
                  } else {
                      $("#mdlSales").html(response.d);
                      $("#mdlSales").modal('show');
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

      function calculateAll2() {
          var total = 0;
          var coverage = 0;

          if ($('.total').length > 0) {
              $.each($('.total'), function (k, v) {
                  total = total + parseFloat(v.value);
              });
              $('#totalCash').html(parseFloat(total, 10).toFixed(2));
          } else {
              $('#totalCash').html('0');
          }

          if ($('.coverage').length > 0) {
              $.each($('.coverage'), function (k, v) {
                  coverage = coverage + parseFloat(v.value);
              });
              $('#totalCovered').html(parseFloat(coverage, 10).toFixed(2));
          } else {
              $('#totalCovered').html('0');
          }
      }

      function calculateAll(c) {
          //alert($('#coveredCash' + c).val());
          //alert($('#nonCoveredCash' + c).val());
          $('#totalCash' + c).html(parseFloat((parseFloat($('#coveredCash' + c).val()) + parseFloat($('#nonCoveredCash' + c).val())), 10).toFixed(2));
          $('#totalVat' + c).html(parseFloat((parseFloat($('#coveredVat' + c).val()) + parseFloat($('#nonCoveredVat' + c).val())), 10).toFixed(2));
      }

      function calculateInsurance(c) {
          var price = 0;
          var discount = 0;
          var total = 0;
          var cash = 0;
          var tax = 0;
          var vat_C = 0;
          var vat_I = 0;
          var coverage = 0;
          var cashList = $('.insurance' + c + ' .total_I');
          var coverageList = $('.insurance' + c + ' .coverage_I');
          var taxList = $('.insurance' + c + ' .vat_I');

          var limit = parseFloat($('#Limit_' + c).val());
          var CIcov = parseFloat($('#CICov_' + c).val());
          var MIcov = parseFloat($('#MICov_' + c).val());

          if ($('.insurance' + c + ' .total_I').length > 0) {
              $.each($('.insurance' + c + ' .coverage_I'), function (k, v) {
                  coverage = coverage + parseFloat(v.value);
              });
              $.each($('.insurance' + c + ' .price_I'), function (k, v) {
                  price = price + parseFloat(v.value);
              });
              $.each($('.insurance' + c + ' .total_I'), function (k, v) {
                  total = total + parseFloat(v.value);
              });
              for (i = 0; i < cashList.length; i++) {
                  vat_C = vat_C + parseFloat(coverageList[i].value) * (taxList[i].value / 100);
                  vat_I = vat_I + (parseFloat(cashList[i].value) - parseFloat(coverageList[i].value)) * (taxList[i].value / 100);
              }
              cash = coverage;
              if ((CIcov + MIcov + cash) > limit) {
                  Extra = (CIcov + MIcov + cash) - limit;
                  cash = limit - (CIcov + MIcov);
                  coverage = coverage + Extra;
              }
              $('#coveredCash' + c).val(parseFloat(cash, 10).toFixed(2));
              $('#coveredVat' + c).val(parseFloat(vat_C, 10).toFixed(2));
              $('#total_I_' + c).html(parseFloat(total, 10).toFixed(2));
              $('#price_I_' + c).html(parseFloat(price, 10).toFixed(2));
          } else {
              $('#coveredCash' + c).val(0);
              $('#coveredVat' + c).val(0);
              $('#total_I_' + c).html("0.00");
              $('#price_I_' + c).html("0.00");
          }
          $('#totalCovered' + c).html(parseFloat(total - cash, 10).toFixed(2));
          $('#totalCoveredVat' + c).html(parseFloat(vat_I, 10).toFixed(2));
          $('#deductionCash' + c).val(parseFloat(total - cash, 10).toFixed(2));
          $('#deductionVat' + c).val(parseFloat(vat_I, 10).toFixed(2));

          $('#covInfo' + c).html('<i class="icon-user4" title="Patient Payment"></i> ' + parseFloat(coverage).toFixed(2) + ' + <i class="icon-building-o" title="Company Payment"></i> ' + parseFloat(total - coverage).toFixed(2));
          $('#basePrice' + c).val(parseFloat($('#coveredCash' + c).val()) + parseFloat($('#deductionCash' + c).val()));
          calculateAll(c);
      }

      function calculateCash(c) {
          var price = 0;
          var discount = 0;
          var total = 0;
          var tax = 0;
          var vat = 0;
          var coverage = 0;
          var cashList = $('.cash' + c + ' .total_C');
          var taxList = $('.cash' + c + ' .vat_C');

          if ($('.cash' + c + ' .total_C').length > 0) {
              $.each($('.cash' + c + ' .price_C'), function (k, v) {
                  price = price + parseFloat(v.value);
              });
              $.each($('.cash' + c + ' .total_C'), function (k, v) {
                  total = total + parseFloat(v.value);
              });
              $.each($('.cash' + c + ' .vat_C'), function (k, v) {
                  tax = tax + parseFloat(v.value);
              });
              for (i = 0; i < cashList.length; i++) {
                  vat = vat + parseFloat(cashList[i].value) * (taxList[i].value / 100);
              }
              $('#nonCoveredCash' + c).val(parseFloat(total, 10).toFixed(2));
              $('#total_C_' + c).html(parseFloat(total, 10).toFixed(2));
              $('#price_C_' + c).html(parseFloat(price, 10).toFixed(2));
              if ($('#nonCoveredVat' + c)) $('#nonCoveredVat' + c).val(parseFloat(vat, 10).toFixed(2));
          } else {
              $('#nonCoveredCash' + c).val(0);
              $('#total_C_' + c).html("0.00");
              $('#price_C_' + c).html("0.00");
              if ($('#nonCoveredVat' + c)) $('#nonCoveredVat' + c).val(0);
          }
          
          //if ($('.coverage').length > 0) {
          //    $.each($('.coverage'), function (k, v) {
          //        coverage = coverage + parseFloat(v.value);
          //    });
          //    $('#totalCovered' + c).html(parseFloat(coverage, 10).toFixed(2));
          //} else {
          //    $('#totalCovered' + c).html('0.00');
          //}
          calculateAll(c);
      }

      function changeToCash(c) {
          if (cashOn[c] == false) {
              $('#divInsurance' + c).hide();
              $('#divCash' + c).removeClass('col-md-4').addClass('col-md-12');
              $('.cash' + c + ' .dynCash').each(function () {
                  $(this).show();
              });
              $('.cash' + c + ' .itemName').each(function () {
                  $(this).removeClass('width-150').addClass('width-200');
                  if ($(this).prop('title').length > 20) $(this).html($(this).prop('title').substr(0, 20) + '...'); else $(this).html($(this).prop('title'));
              });
              $('.cash' + c + ' .company').each(function () {
                  if ($(this).prop('title').length > 25) $(this).html($(this).prop('title').substr(0, 25) + '...'); else $(this).html($(this).prop('title'));
              });
              cashOn[c] = true;
          } else {
              if ($('#cashOnly' + c).val() == "0") {
                  $('#divCash' + c).removeClass('col-md-12').addClass('col-md-4');
                  $('.cash' + c + ' .dynCash').each(function () {
                      $(this).hide();
                  });
                  $('.cash' + c + ' .itemName').each(function () {
                      $(this).removeClass('width-200').addClass('width-150');
                      if ($(this).prop('title').length > 15) $(this).html($(this).prop('title').substr(0, 15) + '...'); else $(this).html($(this).prop('title'));
                  });
                  $('.cash' + c + ' .company').each(function () {
                      if ($(this).prop('title').length > 15) $(this).html($(this).prop('title').substr(0, 15) + '...'); else $(this).html($(this).prop('title'));
                  });
                  $('#divInsurance' + c).show();
                  cashOn[c] = false;
              }
          }
          if ($('.cash' + c + ' #btnCashIcon').hasClass('icon-circle-right')) {
              $('.cash' + c + ' #btnCashIcon').removeClass('icon-circle-right').addClass('icon-circle-left');
          } else {
              $('.cash' + c + ' #btnCashIcon').removeClass('icon-circle-left').addClass('icon-circle-right');
          }

      }
      var cashOn = [1, 2, 3, 4, 5, 6, 7, 8, 9];
      cashOn[9] = true;
      changeToCash(9);

      function changeToInsurance(c) {
          if (InsuranceOn[c] == false) {
              $('#divCash' + c).hide();
              $('#divInsurance' + c).removeClass('col-md-4').addClass('col-md-12');
              $('.insurance' + c + ' .dynInsurance').each(function () {
                  $(this).show();
              });
              $('.insurance' + c + ' .itemName').each(function () {
                  $(this).removeClass('width-150').addClass('width-200');
                  if ($(this).prop('title').length > 20) $(this).html($(this).prop('title').substr(0, 20) + '...'); else $(this).html($(this).prop('title'));
              });
              $('.insurance' + c + ' .company').each(function () {
                  if ($(this).prop('title').length > 25) $(this).html($(this).prop('title').substr(0, 25) + '...'); else $(this).html($(this).prop('title'));
              });
              InsuranceOn[c] = true;
          } else {
              $('#divInsurance' + c).removeClass('col-md-12').addClass('col-md-4');
              $('.insurance' + c + ' .dynInsurance').each(function () {
                  $(this).hide();
              });
              $('.insurance' + c + ' .itemName').each(function () {
                  $(this).removeClass('width-200').addClass('width-150');
                  if ($(this).prop('title').length > 15) $(this).html($(this).prop('title').substr(0, 15) + '...'); else $(this).html($(this).prop('title'));
              });
              $('.insurance' + c + ' .company').each(function () {
                  if ($(this).prop('title').length > 15) $(this).html($(this).prop('title').substr(0, 15) + '...'); else $(this).html($(this).prop('title'));
              });
              $('#divCash' + c).show();
              InsuranceOn[c] = false;
          }
          if ($('.insurance' + c + ' #btnInsuranceIcon').hasClass('icon-circle-right')) {
              $('.insurance' + c + ' #btnInsuranceIcon').removeClass('icon-circle-right').addClass('icon-circle-left');
          } else {
              $('.insurance' + c + ' #btnInsuranceIcon').removeClass('icon-circle-left').addClass('icon-circle-right');
          }

      }
      var InsuranceOn = [1,2,3,4,5,6,7,8,9];
      InsuranceOn[9] = true;
      changeToInsurance(9);
  </script>
  <script type="text/javascript">
      $(document).ready(function () {
          $.getScript('../assets/js/pharmacy.js', function () {
              //
          });

          $('#txtOrdersSearch').keypress(function (e) {
              var key = e.which;
              if (key == 13) {
                  window.location = '?t=' + this.value;
                  return false;
              }
          });

          $('#txtBarcode').autocomplete({
              triggerSelectOnValidInput: true,
              onInvalidateSelection: function () {
                  $('#txtBarcode').val('');
              }, lookup: function (query, done) {
                  if ($('#txtBarcode').val().length > 4) {
                      $.ajax({
                          type: "POST",
                          url: "ajax.aspx/findItem",
                          data: '{query: "' + query + '"}',
                          contentType: "application/json; charset=utf-8",
                          dataType: "json",
                          success: function (response) {
                              done(jQuery.parseJSON(response.d));
                          },
                          failure: function (msg) {
                              alert(msg);
                          }
                          , error: function (xhr, ajaxOptions, thrownError) {
                              alert("Load Form, update form error! " + xhr.status + " error =" + thrownError + " xhr.responseText = " + xhr.responseText);
                          }
                      });
                  } else {
                      done(jQuery.parseJSON(''));
                  }
              }, onSelect: function (suggestion) {
                  $('#txtBarcode').val(suggestion.id);
                  //$('#myTable > tbody:last-child').append('<tr>...</tr><tr>...</tr>');
              }
          });

          $('#txtBarcode').on('change paste keyup', function () {
              var barcode = $(this).val();
              if (barcode.length != 0) {
                  if ($.isNumeric(barcode) == true) {
                      if (event.which == 13 || barcode.length >= 14) {
                          event.preventDefault();
                          $(this).val('');
                          $('#tblInsurance > tbody:last-child').append('<tr id=""tr_""><td></td><td>10191</td><td>CONCOR</td><td>2020-02-01</td><td>10.00</td>0.00</td><td>10.00</td><td>1</td><td>10.00</td><td class=""text-nowrap""><a href=""#"" class=""tag tag-info tag-xs"">Print</a> <a href=""javascript:$().remove();"" class=""tag tag-danger tag-xs"">Delete</button></td></tr>');
                      }
                  }
              }
          });

      });
      
      function dialog() {
          confirm("تأكيد", "هل أنت متأكد؟", yys, '');
      }
      function yys() {
          alert('yes');
      }

      $('#paid').on('change paste keyup', calcRemind);
      $('#cash').on('change paste keyup', calcRemind);
      $('#credit').on('change paste keyup', calcRemind);

      function calcRemind() {
          var total = $('#net_total').val();
          var paid = $('#net_paid').val();
          var cash = $('#net_cash').val();
          var credit = $('#net_credit').val();
          var remind = $('#net_remind').val();
          if (onePayment == true) {
              if (paid >= 0) {
                  $('#net_remind').val(parseFloat(parseFloat(total) - parseFloat(paid)).toFixed(2));
              } else {
                  $('#net_remind').val('-1');
              }
          } else {
              if (byTotal == true) {
                  if (paid >= 0) {
                      $('#net_remind').val(parseFloat(parseFloat(total) - parseFloat(paid)).toFixed(2));
                  } else {
                      $('#net_remind').val('-1');
                  }
              } else {
                  if (cash >= 0 && credit >= 0) {
                      $('#net_remind').val(parseFloat(parseFloat(total) - (parseFloat(cash) + parseFloat(credit))).toFixed(2));
                  } else {
                      $('#net_remind').val('-1');
                  }
              }
          }
      }

      $('.calcNum').click(function () {
          if (byTotal == true) $('#net_paid').val($(this).text()); else $('#net_cash').val($(this).text());
          calcRemind();
      });

      var byTotal = false;
      var onePayment = true;
      var btnSplit = '<i class="icon-calculator"></i> Split Payment';
      var btnJoin = '<i class="icon-calculator"></i> Join Payment';
      function changePayment() {
          if (onePayment == true) {
              $('#divTotalCash').hide();
              $('#divTotalCredit').hide();
              $('#divTotalPaid').show();

              $('#btnPayment').show();
              $('#btnCash').hide();
              $('#btnCredit').hide();
          } else {
              if (byTotal == false) {
                  $('#divTotalCash').hide();
                  $('#divTotalCredit').hide();
                  $('#divTotalPaid').show();

                  $('#btnPayment').hide();
                  $('#btnCash').show();
                  $('#btnCredit').show();

                  $('#btnSplit').html(btnSplit);
                  byTotal = true;
              } else {
                  $('#divTotalCash').show();
                  $('#divTotalCredit').show();
                  $('#divTotalPaid').hide();

                  $('#btnPayment').show();
                  $('#btnCash').hide();
                  $('#btnCredit').hide();

                  $('#btnSplit').html(btnJoin);
                  byTotal = false;
              }
          }

          $('#net_paid').val(0);
          $('#net_cash').val(0);
          $('#net_credit').val(0);
          calcRemind();
      }
      $('#btnSplit').click(changePayment);
      changePayment();

      function refreshListeners() {
          //$('#txtBarcode').on('change paste keyup', function () { var barcode = $(this).val(); if (barcode.length != 0) { if ($.isNumeric(barcode) == true) { if (event.which == 13 || barcode.length >= 14) { event.preventDefault(); $(this).val(''); getItemInfo(barcode, $('#trans' + curTab).val(), $('#coveredCash' + curTab).val(), $('#basePrice' + curTab).val(), $('#counter' + curTab).val()); } } } });
          $('[data-toggle=tooltip]').tooltip();

          $('.printDose').click(function (event) {
              event.preventDefault();
              printUrl = $(this).parent().attr('app-url');
              popupPrinting = $(this).parent().attr('app-popup');
              if (popupPrinting=='true')
                window.open(printUrl, 'printDoseWindow', 'width=0,height=0,scrollbars=yes');
              else
                window.open(printUrl, '_blank');
          });
      }
      refreshListeners();

      function testTab() {
          //if ($('tab1').hasClass('active')) {
          //    alert('is active');
          //}
          alert(curTab);
      }

      function collectIItems(c) {
          var col = '';
          if ($('#tblInsurance' + c + ' .Itr').length > 0) {
              $.each($('#tblInsurance' + c + ' .Itr'), function (k, v) {
                  col = col + $(v).html().replace(/'/g, '|').replace(/"/g, '^') + '!!';
              });
              return col;
          } else {
              return '';
          }
      }

      function collectCItems(c) {
          var col = '';
          if ($('#tblCash' + c + ' .Ctr').length > 0) {
              $.each($('#tblCash' + c + ' .Ctr'), function (k, v) {
                  col = col + $(v).html().replace(/'/g, '|').replace(/"/g, '^') + '!!';
              });
              return col;
          } else {
              return '';
          }
      }

      function createPrintDoseLink(c) {
          var item = '';
          var expire = '';
          var url = '';
          var trans = $('#trans' + c).val();
          if ($('.item').length > 0) {
              $.each($('.item'), function (k, v) {
                  item = item + $(v).val() + ',';
              });
              $.each($('.expire'), function (k, v) {
                  expire = expire + $(v).val() + ',';
              });
              url = 'p_dose.aspx?t=' + trans + '&i=' + item + '&e=' + expire;
              $('#divPrintAll' + c).attr('app-url', url);
              $('#btnPrintAll' + c).prop('disabled', false);
          } else {
              $('#divPrintAll' + c).attr('app-url', 'p_dose.aspx');
              $('#btnPrintAll' + c).prop('disabled', true);
          }
      }

      function printAllDose(c) {
          printUrl = $('#btnPrintAll' + c).parent().attr('app-url');
          popupPrinting = $('#btnPrintAll' + c).parent().attr('app-popup');
          if (popupPrinting == 'true')
              window.open(printUrl, 'printDoseWindow', 'width=0,height=0,scrollbars=yes');
          else
              window.open(printUrl, '_blank');
      }

      function printInvoice(btn) {
          printUrl = $(btn).parent().attr('app-url');
          popupPrinting = $(btn).parent().attr('app-popup');
          if (popupPrinting == 'true')
              window.open(printUrl, 'printDoseWindow', 'width=0,height=0,scrollbars=yes');
          else
              window.open(printUrl, '_blank');
      }

      function getIItems(c) {
          var item = '';
          if ($('#tblInsurance' + c + ' .item_I').length > 0) {
              $.each($('#tblInsurance' + c + ' .item_I'), function (k, v) {
                  item = item + $(v).val() + ',';
              });
              $('#items_I_' + c).val(item);
          } else {
              $('#items_I_' + c).val('');
          }
          alert($('#divPrintAll' + c).attr('app-url'));
          $('#divPrintAll' + c).attr('app-url', 'xxxx');
          alert($('#divPrintAll' + c).attr('app-url'));
      }

      function getCItems(c) {
          var item = '';
          if ($('#tblInsurance' + c + ' .item_C').length > 0) {
              $.each($('#tblInsurance' + c + ' .item_C'), function (k, v) {
                  item = item + $(v).val() + ',';
              });
              $('#items_C_' + c).val(item);
          } else {
              $('#items_C_' + c).val('');
          }
      }

      function removeThis(row) {
          $(row).parent().parent().remove();
          createPrintDoseLink(curTab);
      }

      function moveThis(row) {
          //var counter = parseInt($('#counter' + curTab).val()) + 1;
          //var item = $(row).parent().parent();
          //alert($(item).children().length);
          //$(item).children().each(function () {
          //    alert($(this).html());
          //});
          //var flag = $(item).children().first().children().first();
          //flag.removeClass('icon-checkmark success').addClass('icon-share2 info');
          //flag.attr('data-original-title', 'Moved');
          //flag.prop('title=', 'Moved');
          //var price = $(item).children().children('#price').val();
          //$(item).children().children('#coverage').val(price);
          //alert($(item).children().children('#coverage').val());
          ////var item = '<tr id="tr_' + counter + '" class="Ctr_' + counter + '"><td style="width:32px;">' + '<input type="hidden" name="barcode_C" value="' + barcode + '"/><input type="hidden" name="dose_C" value=""/><input type="hidden" name="item_C" class="item_" & Typ & """ value=""" & strItem & """/></td><td style=""width:70px;"" class=""dynCash"">" & strItem & "</td><td class=""itemName width-150"" title=""" & strItemName & """>" & strItemName & "</td><td style=""width:100px;"" class=""dynCash red"">" & ExpireDate & "</td><td style=""width:80px;"" class=""dynCash"">" & curBasePrice & "<input type=""hidden"" id=""price"" name=""price_" & Typ & """ class=""price_" & Typ & """ value=""" & curBasePrice & """/><input type=""hidden"" name=""service_" & Typ & """ value=""" & intService & """/><input type=""hidden"" name=""warehouse_" & Typ & """ value=""" & byteWarehouse & """/></td><td style=""width:80px;"" class=""dynCash"">" & Discount & "<input type=""hidden"" name=""percent_" & Typ & """ value=""" & curBaseDiscount & """/><input type=""hidden"" id=""discount"" name=""discount_" & Typ & """ class=""discount_" & Typ & """ value=""" & thisDiscount & """/></td><td style=""width:44px;"">" & Quantity & "</td><td style=""width:80px;"">" & PatientCash & "<input type=""hidden"" id=""total"" name=""total_" & Typ & """ class=""total_" & Typ & """ value=""" & PatientCash & """/><input type=""hidden"" id=""coverage"" class=""coverage"" value=""" & InsuranceCoverage & """/></td><td class=""text-nowrap""><a href=""#"" class=""tag btn-blue-grey tag-xs"">" & btnPrint & "</a> <a href=""javascript:"" onclick=""javascript:removeCItems(this);removeThis(this);" & Func & "(curTab);"" class=""tag btn-red btn-lighten-3 tag-xs"">" & btnDelete & "</a></td></tr>';
          //var newitem = '<tr id="' + counter + '" class="Ctr_' + counter + '">' + item.html() + '</tr>';
          //newitem = newitem.replace(/_I/g, '_C');
          //newitem = newitem.replace(/dynInsurance/g, 'dynCash');
          //$('#tblCash' + curTab + ' > tbody:last-child').append(newitem);
          ////alert($('#tblCash' + curTab + ' > tbody:last-child').html());
          //$(row).parent().parent().remove();
          //cashOn[curTab] = !(cashOn[curTab]);
          //changeToCash(curTab);
          //calculateCash(curTab);
          //$('[data-toggle=tooltip]').tooltip();
          //refreshListeners();
      }

      var curTab = 1; $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) { curTab = e.target.toString().substr(e.target.toString().length - 1, 1); });
      function removeIItems(row) {
          var item = $(row).parent().parent().children('td:nth-child(2)').text();
          $('#items_I_' + curTab).val($('#items_I_' + curTab).val().replace(item + ',', ''));
      }

      function removeCItems(row) {
          var item = $(row).parent().parent().children('td:nth-child(2)').text();
          $('#items_C_' + curTab).val($('#items_C_' + curTab).val().replace(item + ',', ''));
      }

      function closeCurrentTab() {
          if (tabCount > 1) {
              $('#tabCashier li:nth-child(' + curTab + ')').remove();
              tabCount--;
              $('#tabCashier li:first-child a').tab('show');
          } else $('#mdlSales').modal('hide');
      }

      function pad(str, max) {
          str = str.toString();
          return str.length < max ? pad("0" + str, max) : str;
      }
  </script>
</asp:Content>
