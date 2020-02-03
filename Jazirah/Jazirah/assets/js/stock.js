/*
*/
function completeBarcode(barcode, warehouse, base, func) {
    $.ajax({
        type: 'POST',
        url: '../Stock/ajax.aspx/completeBarcode',
        data: '{strBarcode: "' + barcode + '", byteWarehouse: ' + warehouse + ', byteBase: ' + base + ', strFunction: "' + func + '"}',
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