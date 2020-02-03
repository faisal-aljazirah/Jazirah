/*
*/
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

function showOrder(transNo, showing) {
    $.ajax({
        type: "POST",
        url: 'ajax.aspx/viewOrder',
        data: '{TransNo: ' + transNo + ', ShowOnly: ' + showing + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.d.substr(0, 4) == 'Err:') {
                msg('', response.d.substr(4, response.d.length), 'error');
            } else {
                $("#mdlAlpha").html(response.d);
                $("#mdlAlpha").modal('show');
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
        url: '../Pharmacy/ajax.aspx/viewInvoice',
        data: '{TransNo: ' + transNo + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.d.substr(0, 4) == 'Err:') {
                msg('', response.d.substr(4, response.d.length), 'error');
            } else {
                $("#mdlAlpha").html(response.d);
                $("#mdlAlpha").modal('show');
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
                $("#mdlBeta").html(response.d);
                $("#mdlBeta").modal('show');
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

function showCashbox() {
    $.ajax({
        type: "POST",
        url: 'ajax.aspx/viewCashbox',
        data: '{}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.d.substr(0, 4) == 'Err:') {
                msg('', response.d.substr(4, response.d.length), 'error');
            } else {
                $("#mdlBeta").html(response.d);
                $("#mdlBeta").modal('show');
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
                $("#mdlAlpha").html(response.d);
                $("#mdlAlpha").modal('show');
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

function getItemInfo(barcode, transaction, coverage, total, row, I_items, C_items, type, quantity) {
    if (typeof (quantity) == 'undefined' || quantity == '') quantity = 0;
    $.ajax({
        type: 'POST',
        url: 'ajax.aspx/getItemInfo',
        data: '{strBarcode: "' + barcode + '", lngTransaction: "' + transaction + '", curCoverage: "' + coverage + '", curBasePriceTotal: "' + total + '", RowCounter: "' + row + '", SelectedInsuranceItems: "' + I_items + '", SelectedCashItems: "' + C_items + '", ItemType: "' + type + '", Quantity: ' + quantity + '}',
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

//updated in stock.js
function completeBarcode_old(barcode) {
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

function getQuantity(barcode) {
    $.ajax({
        type: 'POST',
        url: 'ajax.aspx/getQuantity',
        data: '{strBarcode: "' + barcode + '"}',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (response) {
            if (response.d.substr(0, 4) == 'Err:') {
                msg('', response.d.substr(4, response.d.length), 'error');
            } else {
                $('#mdlConfirm').html(response.d);
                $('#mdlConfirm').modal('show');
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

function returnItems(transaction, items) {
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

function reopenInvoice(transaction) {
    $.ajax({
        type: 'POST',
        url: 'ajax.aspx/reopenInvoice',
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

function requestReopenInvoice(transaction) {
    $.ajax({
        type: 'POST',
        url: 'ajax.aspx/requestReopenInvoice',
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

function requestReturnItems(transaction, items) {
    $.ajax({
        type: 'POST',
        url: 'ajax.aspx/requestReturnItems',
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

function viewVoucher(transNo) {
    $.ajax({
        type: "POST",
        url: 'ajax.aspx/viewVoucher',
        data: '{TransNo: ' + transNo + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.d.substr(0, 4) == 'Err:') {
                msg('', response.d.substr(4, response.d.length), 'error');
            } else {
                $("#mdlAlpha").html(response.d);
                $("#mdlAlpha").modal('show');
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

function returnOrder(transNo) {
    $.ajax({
        type: "POST",
        url: 'ajax.aspx/returnOrder',
        data: '{TransNo: ' + transNo + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
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

function approveCancelRequest(transNo) {
    $.ajax({
        type: "POST",
        url: '../Pharmacy/ajax.aspx/approveCancelRequest',
        data: '{lngTransaction: ' + transNo + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
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

function approveReturnRequest(transNo) {
    $.ajax({
        type: "POST",
        url: '../Pharmacy/ajax.aspx/approveReturnRequest',
        data: '{lngTransaction: ' + transNo + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
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

function approveReopenRequest(transNo) {
    $.ajax({
        type: "POST",
        url: '../Pharmacy/ajax.aspx/approveReopenRequest',
        data: '{lngTransaction: ' + transNo + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
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

function rejectCancelRequest(transNo) {
    $.ajax({
        type: "POST",
        url: '../Pharmacy/ajax.aspx/rejectCancelRequest',
        data: '{lngTransaction: ' + transNo + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
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

function rejectReturnRequest(transNo) {
    $.ajax({
        type: "POST",
        url: '../Pharmacy/ajax.aspx/rejectReturnRequest',
        data: '{lngTransaction: ' + transNo + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
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

function rejectReopenRequest(transNo) {
    $.ajax({
        type: "POST",
        url: '../Pharmacy/ajax.aspx/rejectReopenRequest',
        data: '{lngTransaction: ' + transNo + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
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

function getCreditPatients(contact) {
    $.ajax({
        type: "POST",
        url: '../Pharmacy/ajax.aspx/getCreditPatients',
        data: '{lngContact: ' + contact + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.d.substr(0, 4) == 'Err:') {
                msg('', response.d.substr(4, response.d.length), 'error');
            } else {
                $('#lstPatient').html(response.d);
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

$('#mdlAlpha').on('shown.bs.modal', function () {
    if ($('#txtBarcode')) $('#txtBarcode').focus();
});

function calculateAll(c) {
    $('#totalCash' + c).html(parseFloat((parseFloat($('#coveredCash' + c).val()) + parseFloat($('#nonCoveredCash' + c).val())), 10).toFixed(2));
    if ($('#totalAll' + c)) $('#totalAll' + c).html(parseFloat(parseFloat($('#coveredCash' + c).val()) + parseFloat($('#nonCoveredCash' + c).val()) + parseFloat($('#deductionCash' + c).val())).toFixed(2));
    if ($('#totalVat' + c)) $('#totalVat' + c).html(parseFloat((parseFloat($('#coveredVat' + c).val()) + parseFloat($('#nonCoveredVat' + c).val())), 10).toFixed(2));
    if ($('#totalTotalVat' + c)) $('#totalTotalVat' + c).html(parseFloat(parseFloat($('#coveredVat' + c).val()) + parseFloat($('#nonCoveredVat' + c).val()) + parseFloat($('#deductionVat' + c).val())).toFixed(2));
}

function calculateInsurance(c) {
    var price = 0;
    var discount = 0;
    var total = 0;
    var cash = 0;
    var tax_cnt = 0;
    var tax_avg = 0;
    var vat = 0;
    var vat_C = 0;
    var vat_I = 0;
    var coverage = 0;
    var quantity = 0;

    var limit = parseFloat($('#Limit_' + c).val());
    var CIcov = parseFloat($('#CICov_' + c).val());
    var MIcov = parseFloat($('#MICov_' + c).val());

    if ($('.insurance' + c + ' .total_I').length > 0) {
        $.each($('.insurance' + c + ' .quantity_I'), function (k, v) {
            quantity = quantity + parseFloat(v.value);
        });
        $.each($('.insurance' + c + ' .coverage_I'), function (k, v) {
            coverage = coverage + parseFloat(v.value);
        });
        $.each($('.insurance' + c + ' .price_I'), function (k, v) {
            price = price + parseFloat(v.value);
        });
        //$.each($('.insurance' + c + ' .total_I'), function (k, v) {
        //    total = total + parseFloat(v.value);
        //});
        $.each($('.insurance' + c + ' .vat_I'), function (k, v) {
            vat = vat + parseFloat(v.value);
        });
        for (i = 0; i < $('.insurance' + c + ' .total_I').length; i++) {
            //alert($('.cash' + c + ' .total_C')[i].value);
            total = total + ($('.insurance' + c + ' .total_I')[i].value * $('.insurance' + c + ' .quantity_I')[i].value)
        }
        var percent = (coverage * 100) / total;
        vat_I = vat - (vat * (percent / 100));
        vat_C = vat * (percent / 100);
        cash = coverage;
        /* if (Percent=0 [Patient doent have to pay]) and (already overflow [Total invoices > limit]) then payment is zero*/

        if (coverage == 0 || (CIcov + MIcov) > limit) {
            vat_C = 0;
            cash = 0;
        } else {
            if ((CIcov + MIcov + cash) > limit) {
                Extra = (CIcov + MIcov + cash) - limit;
                cash = limit - (CIcov + MIcov);
                coverage = coverage + Extra;
            }
        }
        $('#coveredCash' + c).val(parseFloat(cash, 10).toFixed(2));
        $('#coveredVat' + c).val(parseFloat(vat_C, 10).toFixed(2));
        $('#total_I_' + c).html(parseFloat(total, 10).toFixed(2));
        $('#price_I_' + c).html(parseFloat(price, 10).toFixed(2));
        $('#quantity_I_' + c).html(parseFloat(quantity, 10).toFixed(0));
        if ($('#vat_I_' + c)) $('#vat_I_' + c).html(parseFloat(vat, 10).toFixed(2));
    } else {
        $('#coveredCash' + c).val(0);
        $('#coveredVat' + c).val(0);
        $('#total_I_' + c).html("0.00");
        $('#price_I_' + c).html("0.00");
        $('#quantity_I_' + c).html("0");
        if ($('#vat_I_' + c)) $('#vat_I_' + c).html("0.00");
    }
    $('#totalCovered' + c).html(parseFloat(total - cash, 10).toFixed(2));
    $('#totalCoveredVat' + c).html(parseFloat(vat - vat_C, 10).toFixed(2));
    $('#deductionCash' + c).val(parseFloat(total - cash, 10).toFixed(2));
    $('#deductionVat' + c).val(parseFloat(vat - vat_C, 10).toFixed(2));

    if ($('#covInfo' + c)) $('#covInfo' + c).html('<i class="icon-user4" title="Patient Payment"></i> ' + parseFloat(coverage).toFixed(2) + ' + <i class="icon-building-o" title="Company Payment"></i> ' + parseFloat(total - coverage).toFixed(2));
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
    var quantity = 0;

    if ($('.cash' + c + ' .total_C').length > 0) {
        $.each($('.cash' + c + ' .quantity_C'), function (k, v) {
            quantity = quantity + parseFloat(v.value);
        });
        $.each($('.cash' + c + ' .price_C'), function (k, v) {
            price = price + parseFloat(v.value);
        });
        //$.each($('.cash' + c + ' .total_C'), function (k, v) {
        //    total = total + parseFloat(v.value);
        //});
        $.each($('.cash' + c + ' .vat_C'), function (k, v) {
            vat = vat + parseFloat(v.value);
        });
        for (i = 0; i < $('.cash' + c + ' .total_C').length; i++) {
            //alert($('.cash' + c + ' .total_C')[i].value);
            total = total + ($('.cash' + c + ' .total_C')[i].value * $('.cash' + c + ' .quantity_C')[i].value)
        }
        $('#nonCoveredCash' + c).val(parseFloat(total, 10).toFixed(2));
        $('#total_C_' + c).html(parseFloat(total, 10).toFixed(2));
        $('#price_C_' + c).html(parseFloat(price, 10).toFixed(2));
        $('#quantity_C_' + c).html(parseFloat(quantity, 10).toFixed(0));
        if ($('#vat_C_' + c)) $('#vat_C_' + c).html(parseFloat(vat, 10).toFixed(2));
        if ($('#nonCoveredVat' + c)) $('#nonCoveredVat' + c).val(parseFloat(vat, 10).toFixed(2));
    } else {
        $('#nonCoveredCash' + c).val(0);
        $('#total_C_' + c).html("0.00");
        $('#price_C_' + c).html("0.00");
        $('#quantity_C_' + c).html("0");
        if ($('#vat_C_' + c)) $('#vat_C_' + c).html("0.00");
        if ($('#nonCoveredVat' + c)) $('#nonCoveredVat' + c).val(0);
    }
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
var InsuranceOn = [1, 2, 3, 4, 5, 6, 7, 8, 9];
InsuranceOn[9] = true;
changeToInsurance(9);

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
        currentFocus = $('#net_paid');
    } else {
        if (byTotal == false) {
            $('#divTotalCash').hide();
            $('#divTotalCredit').hide();
            $('#divTotalPaid').show();

            $('#btnPayment').hide();
            $('#btnCash').show();
            $('#btnCredit').show();

            $('#btnSplit').html(btnSplit);
            currentFocus = $('#net_paid');
            byTotal = true;
        } else {
            $('#divTotalCash').show();
            $('#divTotalCredit').show();
            $('#divTotalPaid').hide();

            $('#btnPayment').show();
            $('#btnCash').hide();
            $('#btnCredit').hide();

            $('#btnSplit').html(btnJoin);
            currentFocus = $('#net_cash');
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
    $('[data-toggle=tooltip]').tooltip();
    $('.printDose').click(function (event) {
        event.preventDefault();
        printUrl = $(this).parent().attr('app-url');
        popupPrinting = $(this).parent().attr('app-popup');
        if (popupPrinting == 'true')
            window.open(printUrl, 'printDoseWindow', 'width=0,height=0,scrollbars=yes');
        else
            window.open(printUrl, '_blank');
    });
    $('.selAll').focus(function () {
        $(this).select();
    });
}
refreshListeners();

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
    $('#txtBarcode').focus();
}

function move2C(row, barcode) {
    getItemInfo(barcode, $('#trans' + curTab).val(), $('#deductionCash' + curTab).val(), $('#basePrice' + curTab).val(), $('#counter' + curTab).val(), $('#items_I_' + curTab).val(), $('#items_C_' + curTab).val(), 1);
    removeIItems(row);
    removeThis(row);
}

function move2I(row, barcode) {
    ordercount = $('#orderItemsCount' + curTab).val();
    creditcount = $('#items_I_' + curTab).val().split(',').length - 1;
    //if (creditcount < ordercount) {
        getItemInfo(barcode, $('#trans' + curTab).val(), $('#deductionCash' + curTab).val(), $('#basePrice' + curTab).val(), $('#counter' + curTab).val(), $('#items_I_' + curTab).val(), $('#items_C_' + curTab).val(), 2);
        removeCItems(row);
        removeThis(row);
    //} else {
    //    msg('', 'Max credit items is ' + parseInt(ordercount), 'error');
    //}
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
    } else $('#mdlAlpha').modal('hide');
}

$('#dtpPeriod').daterangepicker({
    singleDatePicker: true,
    startDate: moment(),
    endDate: moment(),
    locale: {
        format: 'YYYY-MM-DD',
        separator: separator,
        daysOfWeek: daysOfWeek,
        monthNames: monthNames,
        applyLabel: applyLabel,
        cancelLabel: cancelLabel,
        fromLabel: fromLabel,
        toLabel: toLabel
    }
},
function (start, end, label) {
    $('#txtDateFrom').val(start.format('YYYY-MM-DD')); $('#txtDateTo').val(end.format('YYYY-MM-DD'));
    //fillInvoices();
});

function changeInvoiceType(transNo) {
    $.ajax({
        type: "POST",
        url: 'ajax.aspx/changeInvoiceType',
        data: '{TransNo: ' + transNo + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
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