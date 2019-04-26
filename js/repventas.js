$(function () {
    getSucursales();
    getCliente();
    getRegiones();

    var data = [];
    var options = {
        data: data,

        getValue: function (element) {
            return element.descripcion
        },

        

        list: {
            match: {
                enabled: true
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

    
    //obtener sucursales
    $('#sucursal').change(function () {
        var tds = "";
        $.ajax({
            type: "POST",
            url: "wsrepexist.asmx/bodegas",
            data: '{sucursal: ' + $(this).val() + '}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                tds += '<option selected value="0">Selecione una opcion</option>';
                $.each(msg.d, function () {
                    tds += '<option  value="' + this.id + '">' + this.descripcion + '</option>';

                });

                $('#bodega').append(tds);
            }
        });
    });

    //metodo utilizado para obtener el reporte de existencias
    $('#btnConsultar').click(function () {

        $.ajax({
            type: 'POST',
            url: 'wsrepventa.asmx/Consultar',
            data: '{fechaIni: "' + $('#inicio').val() + '",fechaFin: "' + $('#fin').val() + '", region : "'+ $('#region').val() +'",  suc: "'+ $('#sucursal').val() +'", cliente: "'+ $('#cliente').val() +'"}',
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
                    tds = tds + '<td>' + this.factura + '</td>';
                    tds = tds + '<td>' + this.fecha + '</td>';
                    tds = tds + '<td>' + this.cliente + '</td>';
                    tds = tds + '<td>' + this.valor + '</td>';
                    tds = tds + '<td>' + this.estado + '</td>';
                    tds = tds + '<td>' + this.saldo + '</td>';
                    tds = tds + "<td><button  class='btn btn-outline-dark' onclick='getDetalle(" + this.id + ",\"" + this.cliente + "\")'><i class='material-icons'>assignment</i></button></td>";
                    tds = tds + '</tr>';
                });

                $("#tbod-datos").append(tds);
                $('#tab-datos').dataTable();
            }

        });
    });
});



//obtener regiones
function getSucursales() {
    var empresa = 1;
    var tds = "";
    $.ajax({
        type: "POST",
        url: "wsrepcompras.asmx/getsuc",
        data: '{id : 1}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            tds += '<option selected value="0">Selecione una opcion</option>';
            $.each(msg.d, function () {
                tds += '<option  value="' + this.id + '">' + this.descripcion + '</option>';
            });

            $('#sucursal').append(tds);
        }
    });
}


//obtener regiones
function getRegiones() {
    var tds = "";
    $.ajax({
        type: "POST",
        url: "wsrepexist.asmx/regiones",
        data: '{empresa : 1}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            tds += '<option selected value="0">Selecione una opcion</option>';
            $.each(msg.d, function () {
                tds += '<option  value="' + this.id + '">' + this.descripcion + '</option>';

            });

            $('#region').append(tds);
        }
    });
}

function getDetalle(compra, prov) {
    $.ajax({
        type: 'POST',
        url: 'wsrepventa.asmx/consultarDet',
        data: '{id: ' + compra + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function () {

            $("#datos").empty();
        },
        success: function (msg) {
            var tot = 0;
            $('#com').text(compra);
            $('#prov').text(prov);
            $('#MdNuevo').modal('toggle');
            var tds = "";
            $.each(msg.d, function () {
                tds += '<tr class="odd">';
                tds = tds + '<td>' + this.bodega + '</td>';
                tds = tds + '<td>' + this.codigo + '</td>';
                tds = tds + '<td>' + this.descripcion + '</td>';
                tds = tds + '<td style="text-align: right;">' + this.cantidad + '</td>';
                tds = tds + '<td style="text-align: right;">' + this.precio + '</td>';
                tds = tds + '<td style="text-align: right;">' + this.total + '</td>';
                tot += this.total;
                tds = tds + '</tr>';

            });
            $('#totalf').html(tot);
            $("#datos").append(tds);

        }

    });    
}
 