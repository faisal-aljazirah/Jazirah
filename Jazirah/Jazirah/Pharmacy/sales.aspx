<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Pharmacy/PH.Master" CodeBehind="sales.aspx.vb" Inherits="Jazirah.sales" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="../app-assets/vendors/css/datatables/dataTables.bootstrap4.css">
    <link rel="stylesheet" type="text/css" href="../app-assets/vendors/css/forms/selects/select2.min.css">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
<div class="row" style="margin-bottom:-100px;">
    <div class="col-xs-12">
        <div class="card p-1">
            <div class="card-body collapse in">
                <div class="row pl-1 pr-1 text-md-right">
                <%If allowCashbox = true then %>
                    <button type="button" class="btn btn-secondary" onclick="javascript:showCashbox()" <%=C_disabled%>><%=btnCashBox %></button>
                <%End If %>
                    <button type="button" class="btn btn-info" onclick="javascript:createOrder();" <%=S_disabled%>><%=btnCreditInvoice%></button> 
                    <button type="button" class="btn btn-success" onclick="javascript:prepareOrder(-1);" <%=S_disabled%>><%=btnCashInvoice%></button>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="row">
    <div class="col-xs-12" style="min-height:600px;">
        <div class="card">
            <div class="card-body collapse in p-1 mb-1">
                <div class="col-md-12 text-md-center mb-1">
                    <%=getActiveDoctorList1()%>
                </div>
                <div class="col-md-12 mb-1 pl-0 pr-0">
                        <div class="col-md-4 pl-0 pr-0" id="divSearch1">
                        <%=getSearchText1()%>
                        </div>
                        <div class="col-md-3" id="divDepartments1">
                        <%--<%=getDepartmentList() %>--%>
                        </div>
                        <div class="col-md-5 pl-0 pr-0" id="divDoctors1">
                        <%--<%=getDoctorList() %>--%>
                        </div>
                </div>
                <div class="table-responsive" id="tblOrders">
                    <div class="bg-grey text-md-center text-bold-300 bg-lighten-4 pt-2 pb-2"><img src="../app-assets/images/icons/spinner.gif" /> <%=strWait %></div>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="row">
    <div class="col-xs-12" style="min-height:600px;">
        <div class="card bg-grey bg-lighten-3 border border-danger">
            <div class="card-body collapse in p-1">
                <h3 class="card-title text-md-center">Old Orders</h3>
                <div class="col-md-12 text-md-center mb-1">
                    <%=getActiveDoctorList2()%>
                </div>
                <div class="col-md-12 mb-1 pl-0 pr-0">
                        <div class="col-md-4 pl-0 pr-0" id="divSearch2">
                        <%=getSearchText2()%>
                        </div>
                        <div class="col-md-3" id="divDepartments2">
                        <%--<%=getDepartmentList() %>--%>
                        </div>
                        <div class="col-md-5 pl-0 pr-0" id="divDoctors2">
                        <%--<%=getDoctorList() %>--%>
                        </div>
                </div>
                <div class="table-responsive" id="tblOldOrders">
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

      $('#txtOrdersSearch1').keypress(function (e) {
          var key = e.which;
          if (key == 13) {
              fillOrders('', '', this.value);
              //window.location = '?t=' + this.value;
              return false;
          }
      });

      $('#txtOrdersSearch2').keypress(function (e) {
          var key = e.which;
          if (key == 13) {
              fillOldOrders('', '', this.value);
              //window.location = '?t=' + this.value;
              return false;
          }
      });

      function fillOrders(department, salesman, search, prepare) {
          $('#tblOrders').html('<div class="bg-grey text-md-center text-bold-300 bg-lighten-4 pt-2 pb-2"><img src="../app-assets/images/icons/spinner.gif" /> <%=strWait %></div>');
          if (department == '') department = 0;
          if (salesman == '') salesman = 0;
          if (prepare == '' || typeof (prepare) == 'undefined') prepare = false;
          $.ajax({
              type: 'POST',
              url: 'ajax.aspx/fillOrders',
              data: '{byteDepartment: "' + department + '", lngSalesman: "' + salesman + '", strSearch: "' + search + '", ToPrepare: "' + prepare + '"}',
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
                  filterError(xhr, ajaxOptions, thrownError)
              }
          });
      }

      function fillOldOrders(department, salesman, search, prepare) {
          $('#tblOldOrders').html('<div class="bg-grey text-md-center text-bold-300 bg-lighten-4 pt-2 pb-2"><img src="../app-assets/images/icons/spinner.gif" /> <%=strWait %></div>');
                if (department == '') department = 0;
                if (salesman == '') salesman = 0;
                if (prepare == '' || typeof (prepare) == 'undefined') prepare = false;
                $.ajax({
                    type: 'POST',
                    url: 'ajax.aspx/fillOldOrders',
                    data: '{byteDepartment: "' + department + '", lngSalesman: "' + salesman + '", strSearch: "' + search + '", ToPrepare: "' + prepare + '"}',
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    success: function (response) {
                        if (response.d.substr(0, 4) == 'Err:') {
                            msg('', response.d.substr(4, response.d.length), 'error');
                        } else {
                            order = [[3, "desc"], [0, "desc"]];
                            $('#tblOldOrders').html(response.d);
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

      function fillDepartments1() {
          $.ajax({
              type: 'POST',
              url: 'sales.aspx/fillDepartments1',
              data: '{}',
              contentType: 'application/json; charset=utf-8',
              dataType: 'json',
              success: function (response) {
                  if (response.d.substr(0, 4) == 'Err:') {
                      msg('', response.d.substr(4, response.d.length), 'error');
                  } else {
                      $('#divDepartments1').html(response.d);
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

      function fillDepartments2() {
          $.ajax({
              type: 'POST',
              url: 'sales.aspx/fillDepartments2',
              data: '{}',
              contentType: 'application/json; charset=utf-8',
              dataType: 'json',
              success: function (response) {
                  if (response.d.substr(0, 4) == 'Err:') {
                      msg('', response.d.substr(4, response.d.length), 'error');
                  } else {
                      $('#divDepartments2').html(response.d);
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

      function fillDoctors1(department) {
          $.ajax({
              type: 'POST',
              url: 'sales.aspx/fillDoctors1',
              data: '{Department: "' + department + '"}',
              contentType: 'application/json; charset=utf-8',
              dataType: 'json',
              success: function (response) {
                  if (response.d.substr(0, 4) == 'Err:') {
                      msg('', response.d.substr(4, response.d.length), 'error');
                  } else {
                      $('#divDoctors1').html(response.d);
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

      function fillDoctors2(department) {
          $.ajax({
              type: 'POST',
              url: 'sales.aspx/fillDoctors2',
              data: '{Department: "' + department + '"}',
              contentType: 'application/json; charset=utf-8',
              dataType: 'json',
              success: function (response) {
                  if (response.d.substr(0, 4) == 'Err:') {
                      msg('', response.d.substr(4, response.d.length), 'error');
                  } else {
                      $('#divDoctors2').html(response.d);
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
      
      function createOrder() {
          $.ajax({
              type: 'POST',
              url: 'ajax.aspx/createOrder',
              data: '{}',
              contentType: 'application/json; charset=utf-8',
              dataType: 'json',
              success: function (response) {
                  if (response.d.substr(0, 4) == 'Err:') {
                      msg('', response.d.substr(4, response.d.length), 'error');
                  } else {
                      $('#mdlAlpha').html(response.d);
                      $('#mdlAlpha').modal("show");
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

      function createCreditOrder(contact, patient) {
          $.ajax({
              type: 'POST',
              url: 'ajax.aspx/createCreditOrder',
              data: '{lngContact: ' + contact + ', lngPatient: ' + patient + '}',
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

      $(document).ready(function () {
          order = [[3, "desc"], [0, "desc"]];
          fillDepartments1();
          fillDepartments2();
          fillDoctors1(0);
          fillDoctors2(0);
          fillOrders('', '', '');
          fillOldOrders('', '', '');
          $('.select2').select2();
      });

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
          $("div.toolbar").html('<b>Custom tool bar! Text/images etc.</b>');
          def = [{ "visible": false, "targets": [] }];
          dom = "<'row'<'col-sm-12 col-md-4'l><'col-sm-12 col-md-4 text-md-center'B><'col-sm-12 col-md-4'f>><'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>";
          buttons1 = [
              {
                  text: 'All',
                  className: 'btn btn-primary btn-sm',
                  action: function (e, dt, node, config) {
                      fillOrders('', '', '');
                      fillDepartments();
                      fillDoctors(0);
                      $('#txtOrdersSearch').val('');
                  }
              }, {
                  text: 'Refresh',
                  className: 'btn btn-secondry btn-sm',
                  action: function (e, dt, node, config) {
                      fillOrders($('#drpDepartments').val(), $('#drpDoctors').val(), $('#txtOrdersSearch').val());
                  }
              }
          ];
          buttons2 = [
              {
                  text: 'All',
                  className: 'btn btn-primary btn-sm',
                  action: function (e, dt, node, config) {
                      fillOldOrders('', '', '');
                      fillDepartments();
                      fillDoctors(0);
                      $('#txtOrdersSearch').val('');
                  }
              }, {
                  text: 'Refresh',
                  className: 'btn btn-secondry btn-sm',
                  action: function (e, dt, node, config) {
                      fillOldOrders($('#drpDepartments').val(), $('#drpDoctors').val(), $('#txtOrdersSearch').val());
                  }
              }
          ];
      });
  </script>
    <script type="text/javascript">
        //$('.nav-tabs > li > a').hover(function () {
        //    $(this).tab('show');
        //});
        //$('.dep_content').mouseleave(function () {
        //    $('.nav-tabs > li > a').each(function () {
        //        $(this).removeClass('active');
        //    })
        //    $('.tab-pane').each(function () {
        //        $(this).removeClass('active');
        //        $(this).removeClass('show');
        //    })
        //})
    </script>
</asp:Content>
