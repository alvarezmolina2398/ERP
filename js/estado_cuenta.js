$(function () {
    getCliente();

    var data = [];
    var options = {
        data: data,

        getValue: function (element) {
            return element.descripcion
        },

        

      list: {
            match: {
                enabled: true
            },
            onSelectItemEvent: function () {
                var value = $("#cliente").getSelectedItemData().id;
                $("#idclt").val(value).trigger("change");
            }
        },
    }
    $("#cliente").easyAutocomplete(options);

    
    function getCliente() {
        $.ajax({
            url: "wsrepventa.asmx/getClientes",
            data: '{}',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            success: function (msg) {
                $.each(msg.d, function () {
                    data.push({ 'descripcion': this.descripcion, 'id' : this.id });
                });
            }
        });
    }

    //formato para date picker
    $('input[name="datepicker"]').daterangepicker({
        singleDatePicker: true,
        showDropdowns: true,
        minYear: 1901,
        locale: {
            format: 'YYYY-MM-DD'
        }
    }, function (start, end, label) { });

    //metodo utilizado para obtener el reporte de existencias
    $('#btnConsultar').click(function () {
        $.ajax({
            type: 'POST',
            url: 'wsrepestadoCuenta.asmx/Consultar',
            data: '{fechaIn: "' + $('#inicio').val() + '",FechaFin: "' + $('#fin').val() + '", idClt: "'+ $('#idclt').val() +'"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function () {
                $('#datos').empty();
            },
            success: function (msg) {
                var tds = "";
                $.each(msg.d, function () {
                    tds += '<tr class="odd">';
                    tds = tds + '<td>' + this.fecha + '</td>';
                    tds = tds + '<td>' + this.concepto + '</td>';
                    tds = tds + '<td>' + this.factura + '</td>';
                    tds = tds + '<td style="text-align: right;">' + this.recibo + '</td>';
                    tds = tds + '<td style="text-align: right;">' + this.debe + '</td>';
                    tds = tds + '<td style="text-align: right;">' + this.haber + '</td>';
                    tds = tds + '<td style="text-align: right;">' + this.saldoM + '</td>';
                    tds = tds + '<td style="text-align: right;">' + this.saldoA + '</td>';
                    tds = tds + '</tr>';
                });

                $("#datos").append(tds);
            }

        });
    });
});
 