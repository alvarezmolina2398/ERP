$(function () {
    $('#codigo').val("");
    $('#nompro').val("");
    getTipo();
    getSucursales();
    getProductos();

    //arreglo utilizado para almacenar los productos 
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
                var value = $("#nompro").getSelectedItemData().codigo;
                $("#codigo").val(value).trigger("change");
            }
        },
       
    }

    $("#nompro").easyAutocomplete(options);

    $('input[name="datepicker"]').daterangepicker({
        singleDatePicker: true,
        showDropdowns: true,
        minYear: 1901,
        locale: {
            format: 'YYYY-MM-DD'
        }
    }, function (start, end, label) { });

    

    function getProductos() {
        var tds = "";
        $.ajax({
            type: "POST",
            url: "wsrepventaproducto.asmx/getProductos",
            data: '{}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                $.each(msg.d, function () {
                    data.push({ 'descripcion': this.descripcion, 'codigo' : this.codigo });
                });
            }
        });
    }

    //metodo utilizado para obtener el reporte de existencias
    $('#btnConsultar').click(function () {

        $.ajax({
            type: 'POST',
            url: 'wsrepventaproducto.asmx/consultar',
            data: '{fechaIn: "' + $('#inicio').val() + '", FechaFin: "' + $('#finf').val() + '", sucursal : ' + $('#sucursales').val() + ', tipo: ' + $('#tipo').val() + ', codigo: "'+$('#codigo').val()+'"}',
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
                    tds = tds + '<td>' + this.codigo + '</td>';
                    tds = tds + '<td>' + this.articulo + '</td>';
                    tds = tds + '<td style="text-align: right;">' + this.cantidad + '</td>';
                    tds = tds + '<td style="text-align: right;">' + this.valor + '</td>';
                    tds = tds + '<td>' + this.factura + '</td>';
                    tds = tds + '<td>' + this.fecha + '</td>';
                    tds = tds + '<td>' + this.tipo + '</td>';
                    tds = tds + '<td>' + this.marca + '</td>';
                    tds = tds + '</tr>';
                });
                $('#codigo').val("");
                $("#tbod-datos").append(tds);
                $('#tab-datos').dataTable();
            }

        });
    });
});


//obtener regiones
function getSucursales() {
    var tds = "";
    $.ajax({
        type: "POST",
        url: "wsrepventaproducto.asmx/sucursales",
        data: '{empresa : 1}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            tds += '<option selected value="0">Selecione una opcion</option>';
            $.each(msg.d, function () {
                tds += '<option  value="' + this.id + '">' + this.descripcion + '</option>';

            });

            $('#sucursales').append(tds);
        }
    });

}

function getTipo() {
    var tds = "";
    $.ajax({
        type: "POST",
        url: "wsrepventaproducto.asmx/getTipo",
        data: '{empresa : 1}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            tds += '<option selected value="0">Selecione una opcion</option>';
            $.each(msg.d, function () {
                tds += '<option  value="' + this.id + '">' + this.descripcion + '</option>';

            });

            $('#tipo').append(tds);
        }
    });


}
