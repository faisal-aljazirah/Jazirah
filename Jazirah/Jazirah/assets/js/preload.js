var tableLanguage, searching, ordering, paging, dom, buttons, info, order, noorder;
var daysOfWeek, monthNames, separator, applyLabel, cancelLabel, fromLabel, toLabel;

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
        tableLanguage = {};
    }
});

$(document).ready(function () {
    if ($("body").hasClass("rtl")) {
        daysOfWeek = [
            "أح",
            "أث",
            "ث",
            "أر",
            "خ",
            "ج",
            "س"
        ];
        monthNames = [
            "يناير",
            "فباير",
            "مارس",
            "أبريل",
            "مايو",
            "يونيو",
            "يوليو",
            "أغسطس",
            "سبتمبر",
            "أكتوبر",
            "نوفمبر",
            "ديسمبر"
        ];
        separator = ' إلى ';
        applyLabel = 'تطبيق';
        cancelLabel = 'تراجع';
        fromLabel = 'من';
        toLabel = 'إلي';
    } else {
        daysOfWeek = [
            "Su",
            "Mo",
            "Tu",
            "We",
            "Th",
            "Fr",
            "Sa"
        ];
        monthNames = [
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December"
        ];
        separator = ' To ';
        applyLabel = 'Apply';
        cancelLabel = 'Cancel';
        fromLabel = 'From';
        toLabel = 'To';
    }

    firstDay = 0;
});
// Datatables
$(document).ready(function () {
    if ($('table.tablelist').length) {
        if (typeof (searching) == 'undefined') searching = true;
        if (typeof (ordering) == 'undefined') ordering = true;
        if (typeof (paging) == 'undefined') paging = true;
        if (typeof (dom) == 'undefined') dom = "<'row'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6'f>><'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>";
        if (typeof (buttons) == 'undefined') buttons = '';
        if (typeof (info) == 'undefined') info = true;
        if (typeof (order) == 'undefined') order = [[0, 'desc']];
        if (typeof (noorder) == 'undefined') noorder = [$('table.tablelist thead th').length - 1];

        $('table.tablelist').dataTable({
            language: tableLanguage,
            searching: searching,
            ordering: ordering,
            paging: paging,
            dom: dom,
            buttons: buttons,
            'info': info,
            'order': order,
            'columnDefs': [{ orderable: false, targets: noorder }]
        });
    }
});