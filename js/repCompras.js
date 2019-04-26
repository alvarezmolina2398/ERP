$(function () {
    var data = [];
    getProveedores();

    var options = {
        data: data,

        getValue: function (element) {
            $('#idprov').val(element.id);
            return element.descripcion
        },

        

        list: {
            match: {
                enabled: true
            }
        },
    }
    $("#prov").easyAutocomplete(options);

    
    function getProveedores() {
        $.ajax({
            url: "wsrepcompras.asmx/getprov",
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


    getSucursales();
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
            url: 'wsrepcompras.asmx/consultar',
            data: '{fechaIn: "' + $('#inicio').val() + '",FechaFin: "' + $('#fin').val() + '", proveedor:  "'+ $('#prov').val() +'"}',
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
                    tds = tds + '<td>' + this.noCompra + '</td>';
                    tds = tds + '<td>' + this.fecha + '</td>';
                    tds = tds + '<td>' + this.usuario + '</td>';
                    tds = tds + '<td>' + this.proveedor + '</td>';
                    tds = tds + '<td>' + this.observaciones + '</td>';
                    tds = tds + '<td>' + this.valor + '</td>';
                    tds = tds + "<td><button  class='btn btn-outline-dark' onclick='getDetalle(" + this.noCompra + ",\"" + this.proveedor + "\")'><i class='material-icons'>assignment</i></button></td>";
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

function getDetalle(compra, prov) {
    $.ajax({
        type: 'POST',
        url: 'wsrepcompras.asmx/consultarDet',
        data: '{nocompra: ' + compra + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function () {

            $("#datos").empty();
        },
        success: function (msg) {
            var tot = 0;
            $('#MdNuevo').modal('toggle');
            $('#com').text(compra);
            $('#nomprov').text(prov);
            var tds = "";
            $.each(msg.d, function () {
                tds += '<tr class="odd">';
                tds = tds + '<td>' + this.codigo + '</td>';
                tds = tds + '<td>' + this.descripcion + '</td>';
                tds = tds + '<td>' + this.cantidad + '</td>';
                tds = tds + '<td>' + this.precio + '</td>';
                tds = tds + '<td>' + this.total + '</td>';
                tot = this.totalf
                tds = tds + '</tr>';

            });
            $('#tot').text(tot);
            $("#datos").append(tds);

        }

    });    
}
 