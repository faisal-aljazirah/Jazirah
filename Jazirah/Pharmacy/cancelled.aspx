<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Pharmacy/PH.Master" CodeBehind="cancelled.aspx.vb" Inherits="Jazirah.cancelled" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="../app-assets/vendors/css/datatables/dataTables.bootstrap4.css">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
<div class="row">
    <div class="col-xs-12">
        <div class="card">
            <div class="card-body collapse in">
                <div class="table-responsive">
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
                              <th><%=colUser%></th>
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
					          <td class="center"><%#CDate(DataBinder.Eval(Container.DataItem, "TransactionDate")).ToString(strDateFormat)%></td>
					          <td><%#DataBinder.Eval(Container.DataItem, "DepartmentName")%></td>
					          <td><%#DataBinder.Eval(Container.DataItem, "CompanyName")%></td>
					          <td><%#DataBinder.Eval(Container.DataItem, "PaymentType")%></td>
                              <td><%#DataBinder.Eval(Container.DataItem, "UserName")%></td>
					          <td>
                                  <button type="button" onclick="javascript:viewInvoice(<%#DataBinder.Eval(Container.DataItem, "TransactionNo")%>);" class="btn btn-sm btn-primary"><%=btnView%></button>
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
<div class="modal fade text-xs-left" id="large" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true"></div>
<div class="modal fade text-xs-left" id="mdlProcess" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true"></div>
<div class="modal fade" id="mdlInfo" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true" data-backdrop="false"></div>
<div class="modal fade" id="mdlMessage" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true" data-backdrop="false"></div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterPlaceHolder" runat="server">
  <script src="../app-assets/vendors/js/datatables/jquery.dataTables.min.js" type="text/javascript"></script>
  <script src="../app-assets/vendors/js/datatables/dataTables.bootstrap4.min.js" type="text/javascript"></script>
  <script src="../app-assets/vendors/js/autocomplete/jquery.autocomplete.min.js" type="text/javascript"></script>
  <script type="text/javascript">
      function getItemInfo(barcode, transaction, coverage, total, row, I_items) {
          $.ajax({
              type: 'POST',
              url: 'ajax.aspx/getItemInfo',
              data: '{strBarcode: "' + barcode + '", lngTransaction: "' + transaction + '", curCoverage: "' + coverage + '", curBasePriceTotal: "' + total + '", RowCounter: "' + row + '", SelectedInsuranceItems: "' + I_items + '"}',
              contentType: 'application/json; charset=utf-8',
              dataType: 'json',
              success: function (response) {
                  if (response.d.substr(0, 4) == 'Err:') {
                      //msg('', response.d.substr(4, response.d.length) + ' ' + patient + ',' + tdate + ',' + reference, 'error');
                      msg('', response.d.substr(4, response.d.length), 'error');
                  } else {
                      //var item = '<tr><td>' + barcode + '</td><td>' + response.d + '</td><td>Item Name</td><td>Amount</td><td>Expire Date</td><td>Price</td><td>Discount</td><td>Total</td><td class=""text-nowrap""><a href=""#"" class=""tag tag-info tag-xs"">Print</a> <a href=""#"" class=""tag tag-danger tag-xs"">Delete</button></td></tr>';
                      //$(item).insertBefore("#last");
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

      function returnToSales(transaction) {
          $.ajax({
              type: 'POST',
              url: 'ajax.aspx/ReturnToSales',
              data: '{lngTransaction: "' + transaction + '"}',
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

      function cancelInvoice(transaction) {
          $.ajax({
              type: 'POST',
              url: 'ajax.aspx/cancelInvoice',
              data: '{lngTransaction: "' + transaction + '"}',
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

      function returnItems(transaction, items) {
          alert(items);
          $.ajax({
              type: 'POST',
              url: 'ajax.aspx/returnItems',
              data: '{lngTransaction: "' + transaction + '", lstItems: "' + items + '"}',
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

      function requestCancelInvoice(transaction) {
          $.ajax({
              type: 'POST',
              url: 'ajax.aspx/requestCancelInvoice',
              data: '{lngTransaction: "' + transaction + '"}',
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
                      $("#large").html(response.d);
                      $("#large").modal('show');
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
                      $("#mdlProcess").html(response.d);
                      $("#mdlProcess").modal('show');
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
              url: 'ajax.aspx/viewCashier2',
              data: '{lngTransaction: ' + transNo + '}',
              contentType: "application/json; charset=utf-8",
              dataType: "json",
              success: function (response) {
                  if (response.d.substr(0, 4) == 'Err:') {
                      msg('', response.d.substr(4, response.d.length), 'error');
                  } else {
                      $("#large").html(response.d);
                      $("#large").modal('show');
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
                      $("#large").html(response.d);
                      $("#large").modal('show');
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
          $('#totalCash' + c).html(parseFloat(parseFloat($('#coveredCash' + c).val()) + parseFloat($('#nonCoveredCash' + c).val())).toFixed(2));
          $('#totalAll' + c).html(parseFloat(parseFloat($('#coveredCash' + c).val()) + parseFloat($('#nonCoveredCash' + c).val()) + parseFloat($('#deductionCash' + c).val())).toFixed(2));
      }

      function calculateInsurance(c) {
          var price = 0;
          var discount = 0;
          var total = 0;
          var cash = 0;
          var coverage = 0;

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
              cash = coverage;
              if ((CIcov + MIcov + cash) > limit) {
                  Extra = (CIcov + MIcov + cash) - limit;
                  cash = limit - (CIcov + MIcov);
                  coverage = coverage + Extra;
              }
              $('#coveredCash' + c).val(parseFloat(cash, 10).toFixed(2));
              $('#total_I_' + c).html(parseFloat(total, 10).toFixed(2));
              $('#price_I_' + c).html(parseFloat(price, 10).toFixed(2));
          } else {
              $('#coveredCash' + c).val(0);
              $('#total_I_' + c).html("0.00");
              $('#price_I_' + c).html("0.00");
          }
          $('#totalCovered' + c).html(parseFloat(total - cash, 10).toFixed(2));
          $('#deductionCash' + c).val(parseFloat(total - cash, 10).toFixed(2));

          $('#covInfo' + c).html('<i class="icon-user4" title="Patient Payment"></i> ' + parseFloat(coverage).toFixed(2) + ' + <i class="icon-building-o" title="Company Payment"></i> ' + parseFloat(total - coverage).toFixed(2));
          $('#basePrice' + c).val(parseFloat($('#coveredCash' + c).val()) + parseFloat($('#deductionCash' + c).val()));
          calculateAll(c);
      }

      function calculateCash(c) {
          var price = 0;
          var discount = 0;
          var total = 0;
          var coverage = 0;

          if ($('.cash' + c + ' .total_C').length > 0) {
              $.each($('.cash' + c + ' .price_C'), function (k, v) {
                  price = price + parseFloat(v.value);
              });
              $.each($('.cash' + c + ' .total_C'), function (k, v) {
                  total = total + parseFloat(v.value);
              });
              $('#nonCoveredCash' + c).val(parseFloat(total, 10).toFixed(2));
              $('#total_C_' + c).html(parseFloat(total, 10).toFixed(2));
              $('#price_C_' + c).html(parseFloat(price, 10).toFixed(2));
          } else {
              $('#nonCoveredCash' + c).val(0);
              $('#total_C_' + c).html("0.00");
              $('#price_C_' + c).html("0.00");
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
          }else{
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
      }

      function testTab() {
          //if ($('tab1').hasClass('active')) {
          //    alert('is active');
          //}
          alert(curTab);
      }

      function collectIItems(c) {
          var col = '';
          if ($('.Itr_' + c).length > 0) {
              $.each($('.Itr_' + c), function (k, v) {
                  col = col + $(v).html().replace(/'/g, '|').replace(/"/g, '^') + '!!';
              });
              return col;
          } else {
              return '';
          }
      }

      function collectCItems(c) {
          var col = '';
          if ($('.Ctr_' + c).length > 0) {
              $.each($('.Ctr_' + c), function (k, v) {
                  col = col + $(v).html().replace(/'/g, '|').replace(/"/g, '^') + '!!';
              });
              return col;
          } else {
              return '';
          }
      }

      function removeThis(row) {
          $(row).parent().parent().remove();
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
  </script>
</asp:Content>
