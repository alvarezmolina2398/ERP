$(function () {
    //formato para date picker
    $('input[name="datepicker"]').daterangepicker({
        singleDatePicker: true,
        showDropdowns: true,
        minYear: 1901,
        locale: {
            format: 'YYYY-MM-DD'
        }
    }, function (start, end, label) { });

    
    //metodo utilizado para obtener el reporte de orden de compra
    $('#btnConsultar').click(function () {

        $.ajax({
            type: 'POST',
            url: 'wsrepordencompra.asmx/consultar',
            data: '{fechaIni: "' + $('#inicio').val() + '",fechaFin: "' + $('#fin').val() + '", estatus: '+$('#estatus').val()+' }',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function () {
                $('#tbod-datos').empty();
                $('#tab-datos').dataTable().fnDeleteRow();
                $('#tab-datos').dataTable().fnUpdate();
                $('#tab-datos').dataTable().fnDestroy();
            },
            success: function (msg) {
                var tds = "";
                $.each(msg.d, function () {
                    tds += '<tr class="odd">';
                    tds = tds + '<td>' + this.noorden + '</td>';
                    tds = tds + '<td>' + this.proveedor + '</td>';
                    tds = tds + '<td>' + this.fecha + '</td>';
                    tds = tds + '<td style="text-align: right;">' + this.valor + '</td>';
                    tds = tds + '<td>' + this.observacion + '</td>';
                    tds = tds + '<td>' + this.estatus + '</td>';
                    tds = tds + '</tr>';
                });

                $("#tbod-datos").append(tds);
                $('#tab-datos').dataTable();
            }

        });
    });

});
