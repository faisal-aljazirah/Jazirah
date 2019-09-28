(function(window, undefined) {
  'use strict';

  /*
  NOTE:
  ------
  PLACE HERE YOUR OWN JAVASCRIPT CODE IF NEEDED
  WE WILL RELEASE FUTURE UPDATES SO IN ORDER TO NOT OVERWRITE YOUR JAVASCRIPT CODE PLEASE CONSIDER WRITING YOUR SCRIPT HERE.  */
})(window);

  function showGlobalForm(path, type, tag, app, cls, ref, options) {
      if (typeof (id) == 'undefined' || id == '') id = 0;
      if (type == '') type = 'out';
      $.ajax({
          type: "POST",
          url: path,
          data: '{AppID: ' + app + ', ClassID: ' + cls + ', ReferenceID: ' + ref + ', Type: "' + type + '", Options: "' + options + '"}',
          contentType: "application/json; charset=utf-8",
          dataType: "json",
          success: function (response) {
              switch (type) {
                  case 'in':
                      if (tag == "") tag = "#prtUpdate"
                      $(tag).html(response.d);
                      $(tag).show();
                      $("#prtView").hide();
                      break;
                  case 'out':
                      if (tag == "") tag = "#prtNew"
                      $(tag).html(response.d);
                      $(tag).show();
                      $("#prtMain").hide();
                      break;
                  case 'modal':
                      if (tag == "") tag = "#prtModal"
                      $(tag).html(response.d);
                      $(tag).modal('show');
                      break;
                  case 'approve':
                      if (tag == "") tag = "#prtModal"
                      $(tag).html(response.d);
                      $(tag).modal('show');
                      break;
              };
              $("#prtToolsUpdate").show();
              $("#prtToolsView").hide();
              $("#prtToolsNew").show();
              $("#prtToolsMain").hide();
          },
          failure: function (msg) {
              alert(msg);
          },
          error: function (xhr, ajaxOptions, thrownError) {
              alert("Load Form, update form error! " + xhr.status + " error =" + thrownError + " xhr.responseText = " + xhr.responseText);
          }
      });
  }

  function executeGlobalmethod(path, tag, app, cls, ref, params) {
      if (typeof (id) == 'undefined' || id == '') id = 0;
      $.ajax({
          type: "POST",
          url: path,
          data: '{AppID: ' + app + ', ClassID: ' + cls + ', ReferenceID: ' + ref + ', Params: "' + params + '"}',
          contentType: "application/json; charset=utf-8",
          dataType: "json",
          success: function (response) {
              if (tag == "") tag = "#prtUpdate"
              $(tag).html(response.d);
          },
          failure: function (msg) {
              alert(msg);
          },
          error: function (xhr, ajaxOptions, thrownError) {
              alert("Load Form, update form error! " + xhr.status + " error =" + thrownError + " xhr.responseText = " + xhr.responseText);
          }
      });
  }

  var tableLanguage;

  $(document).ready(function () {
      if ($("body").hasClass("rtl")) {
          tableLanguage = {
              "decimal": "",
              "emptyTable": "لا يوجد بيانات",
              "info": "عرض _START_ إلى _END_ من _TOTAL_ سجل",
              "infoEmpty": "عرض 0 إلى 0 من 0 سجل",
              "infoFiltered": "(filtered from _MAX_ total entries)",
              "infoPostFix": "",
              "thousands": ",",
              "lengthMenu": "عرض _MENU_ سجل",
              "loadingRecords": "تحميل...",
              "processing": "معالجة...",
              "search": "بحث:",
              "zeroRecords": "لم يتم العثور على مطابقة",
              "paginate": {
                  "first": "الأول",
                  "last": "الأخير",
                  "next": "التالي",
                  "previous": "السابق"
              },
              "aria": {
                  "sortAscending": ": activate to sort column ascending",
                  "sortDescending": ": activate to sort column descending"
              }
          };
      } else {
          tableLanguage = {}
      }
  });
  // Datatables
  $(document).ready(function () {
      if ($('table.tablelist').length) {
          if (typeof (searching) == 'undefined') searching = true;
          if (typeof (ordering) == 'undefined') ordering = true;
          if (typeof (paging) == 'undefined') paging = true;
          if (typeof (info) == 'undefined') info = true;
          if (typeof (order) == 'undefined') order = [[0, 'desc']];
          if (typeof (noorder) == 'undefined') noorder = [$('table.tablelist thead th').length - 1];
          
          $('table.tablelist').dataTable({
              language: tableLanguage,
              searching: searching,
              ordering: ordering,
              paging: paging,
              'info': info,
              'order': order,
              'columnDefs': [{ orderable: false, targets: noorder }]
          });
      }
  });

  function msg(title, message, type) {
      if (typeof window.stackTopLeft === 'undefined') {
          window.stackTopLeft = {
              'dir1': 'down',
              'dir2': 'right',
              'firstpos1': 25,
              'firstpos2': 25,
              'push': 'top'
          };
      }
      if (typeof window.stackTopRight === 'undefined') {
          window.stackTopRight = {
              'dir1': 'down',
              'dir2': 'left',
              'firstpos1': 25,
              'firstpos2': 25,
              'push': 'top'
          };
      }
      var stack;
      if ($("body").hasClass("rtl")) stack = window.stackTopLeft; else stack = stackTopRight;

      if (title == '') {
          switch (type) {
              case 'error':
                  if ($("body").hasClass("rtl")) { title = "حصل خطأ ما!"; } else { title = "Something went wrong!"; };
                  break;
              case 'success':
                  if ($("body").hasClass("rtl")) { title = "تمت العملية!"; } else { title = "Done successfully!"; };
                  break;
              case 'notice':
                  if ($("body").hasClass("rtl")) { title = "انتبه!"; } else { title = "Notice!"; };
                  break;
              case 'info':
                  if ($("body").hasClass("rtl")) { title = "معلومة!"; } else { title = "Information!"; };
                  break;
          }
      }

      PNotify.alert({
          title: title,
          text: message,
          type: type,
          styling: 'bootstrap4',
          stack: stack
      });
  }

  function confirm(title, message, yesFunction, noFunction, icon) {
      if (icon == '') icon = 'fas fa-question-circle';
      var btnOK, btnCancel;
      if ($("body").hasClass("rtl")) {
          btnOK = 'موافق';
          btnCancel = 'إلغاء الأمر';
      } else {
          btnOK = 'OK';
          btnCancel = 'Cancel';
      }
      var notice = PNotify.alert({
          title: title,
          text: message,
          textTrusted: true,
          icon: icon,
          hide: false,
          stack: {
              'modal': true
          },

          modules: {
              Confirm: {
                  confirm: true,
                  focus: null,
                  buttons: [{
                      text: btnOK,
                      textTrusted: true,
                      addClass: 'btn-success',
                      primary: true,
                      promptTrigger: true,
                      click: function click(notice, value) {
                          notice.close();
                          notice.fire('pnotify.confirm', { notice: notice, value: value });
                      }
                  }, {
                      text: btnCancel,
                      textTrusted: true,
                      addClass: 'btn-danger',
                      click: function click(notice) {
                          notice.close();
                          notice.fire('pnotify.cancel', { notice: notice });
                      }
                  }],
              },
              Buttons: {
                  closer: false,
                  sticker: false
              },
              History: {
                  history: false
              },
          }
      });
      if (typeof (yesFunction) != 'undefined') {
          notice.on('pnotify.confirm', function () {
              yesFunction();
          });
      }
      if (typeof (noFunction) != 'undefined') {
          notice.on('pnotify.cancel', function () {
              noFunction();
          });
      }
  }

  // Set timeout variables.
  var timoutWarning = 60000; // Display warning in 14 Mins.
  var timoutNow = 60000; // Timeout in 15 mins.
  var refreshUI = 15000; //Timeout in 15 Seconds
  var logoutUrl = "about.html"; // URL to logout page.

  var warningTimer;
  var timeoutTimer;
  var UITimer;

  // Start timers.
  function StartTimers() {
      //warningTimer = setTimeout("IdleWarning()", timoutWarning);
      //timeoutTimer = setTimeout("IdleTimeout()", timoutNow);
      UITimer = setTimeout(updateUI, refreshUI);
  }

  // Reset timers.
  function ResetTimers() {
      clearTimeout(warningTimer);
      clearTimeout(timeoutTimer);
      StartTimers();
      //$("#timeout").dialog('close');
  }

  // Show idle timeout warning dialog.
  function IdleWarning() {
      $("#timeout").dialog({
          modal: true
      });
  }

  // Logout the user.
  function IdleTimeout() {
      //window.location = logoutUrl;
      $('#animation').modal('show')
  }

  updateUI();
  function updateUI() {
      drawNotificationList();
      ResetTimers();
      //alert(0);
  }

  function drawNotificationList() {
      $.ajax({
          type: 'POST',
          url: '../Main/ajax.aspx/drawNotificationList',
          data: '{}',
          contentType: 'application/json; charset=utf-8',
          dataType: 'json',
          success: function (response) {
              if (response.d.substr(0, 4) == 'Err:') {
                  msg('', response.d.substr(4, response.d.length), 'error');
              } else {
                  $('#divNotification').html(response.d);
              }
          },
          failure: function (msg) {
              //alert(msg);
          },
          error: function (xhr, ajaxOptions, thrownError) {
              //alert("Load Form, update form error! " + xhr.status + " error =" + thrownError + " xhr.responseText = " + xhr.responseText);
          }
      });
  }