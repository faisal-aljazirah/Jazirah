<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Stock/ST.Master" CodeBehind="items.aspx.vb" Inherits="Jazirah.items" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="../app-assets/vendors/css/datatables/dataTables.bootstrap4.css">
    <link rel="stylesheet" type="text/css" href="../app-assets/vendors/css/daterangepicker/daterangepicker.css">
    <link rel="stylesheet" type="text/css" href="../app-assets/vendors/css/forms/toggle/bootstrap-switch.min.css">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
<div class="row">
    <div class="col-xs-3">
        <div class="card p-1">
            <div class="card-body">
                <div class="row">
                    <div class="col-md-12">
                        <div class="col-md-6 pl-0 pr-1"><button type="button" class="btn btn-sm btn-success full-width" onclick="javascript:showModal('viewTransfer', '{lngTransaction: 0}', '#mdlAlpha')">New Item</button></div>
                        <div class="col-md-6 pl-1 pr-0">
                            <div class="btn-group full-width">
                                <button type="button" class="btn btn-outline-secondary btn-sm full-width dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Quick Change</button>
                                <div class="dropdown-menu">
                                    <a class="dropdown-item" href="javascript:showModal('changeAvailablity','{}','#mdlConfirm');">Availablity</a>
                                    <a class="dropdown-item" href="javascript:showModal('changeTax','{}','#mdlConfirm');">Tax</a>
                                    <a class="dropdown-item" href="javascript:showModal('changeLimit','{}','#mdlConfirm');">Limit</a>
                                    <div class="dropdown-divider"></div>
                                    <a class="dropdown-item" href="#">Customize</a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="col-xs-9">
        <div class="card p-1">
            <div class="card-body">
                <div class="row">
                    <div class="col-md-12">
                        <div class="col-md-6"><input type="text" class="form-control form-control-sm input-sm" id="txtItemSearch" placeholder="<%=plcItem %>" value="" /></div>
                        <div class="col-md-2"><select id="drpGroupSearch" class="form-control input-sm"><%=GroupList%></select></div>
                        <div class="col-md-2"><select id="drpAvailableSearch" class="form-control input-sm"><%=AvailableList%></select></div>
                        <div class="col-md-2"><button type="button" id="btnSearch" class="btn btn-sm btn-outline-orange full-width" onclick="javascript:fillItems();"><%=btnSearch%></button></div>
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
                <div class="table-responsive" id="tblBalance">

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
  <script src="../app-assets/vendors/js/forms/toggle/bootstrap-switch.min.js" type="text/javascript"></script>  
  <script src="../app-assets/vendors/js/forms/toggle/bootstrap-checkbox.min.js" type="text/javascript"></script>  
  
    
  <script type="text/javascript">
      $(document).ready(function () {
          $.getScript('../assets/js/pharmacy.js', function () {
              //
          });
      });

      $('#txtItemSearch').keypress(function (e) {
          var key = e.which;
          if (key == 13) {
              //window.location = '?i=' + this.value;
              fillItems();
              return false;
          }
      });

      var dateFormat = '<%=strDateFormat%>';
      var datePattern = dateFormat.replace('yyyy', '{{9999}}').replace('MM', '{{99}}').replace('dd', '{{99}}');

      function fillItems() {
          $('#tblBalance').html('<div class="bg-grey text-md-center text-bold-300 bg-lighten-4 pt-2 pb-2"><img src="../app-assets/images/icons/spinner.gif" /> <%=strWait %></div>');
          
          titem = $('#txtItemSearch').val();
          sgroup = $('#drpGroupSearch').val();
          savailable = $('#drpAvailableSearch').val();
          
          if (sgroup == '') sgroup = '0';
          if (savailable == '') savailable = '1';
          
          $.ajax({
              type: 'POST',
              url: '../Stock/ajax.aspx/fillItems',
              data: '{strItem: "' + titem + '", intGroup: ' + sgroup + ', intAvailable: ' + savailable + '}',
              contentType: 'application/json; charset=utf-8',
              dataType: 'json',
              success: function (response) {
                  if (response.d.substr(0, 4) == 'Err:') {
                      msg('', response.d.substr(4, response.d.length), 'error');
                  } else {
                      $('#tblBalance').html(response.d);
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
      //var dateFormat = '<%=strDateFormat%>';
      //var datePattern = dateFormat.replace('yyyy', '{{9999}}').replace('MM', '{{99}}').replace('dd', '{{99}}');
      $(document).ready(function () {
          //fillItems();
          //$(".date-formatter").formatter({ pattern: datePattern });
          //$('#dtpPeriod').on('apply.daterangepicker', function (ev, picker) {
          //    fillBalance();
          //});
      });

      //function saveItem(strItem, fields) {
      //    $.ajax({
      //        type: 'POST',
      //        url: '../Stock/ajax.aspx/saveItem',
      //        data: '{strItem: "' + titem + '", Fields: "' + fields + '"}',
      //        contentType: 'application/json; charset=utf-8',
      //        dataType: 'json',
      //        success: function (response) {
      //            if (response.d.substr(0, 4) == 'Err:') {
      //                msg('', response.d.substr(4, response.d.length), 'error');
      //            } else {
      //                $('#prtJS').html(response.d);
      //            }
      //        },
      //        failure: function (msg) {
      //            alert(msg);
      //        },
      //        error: function (xhr, ajaxOptions, thrownError) {
      //            alert("Load Form, update form error! " + xhr.status + " error =" + thrownError + " xhr.responseText = " + xhr.responseText);
      //        }
      //    });
      //}
	</script>
</asp:Content>
