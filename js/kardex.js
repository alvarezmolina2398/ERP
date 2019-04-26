$(function () {
    $('#codigo').val("");
    $('#nompro').val("");
    getProductos();
    getBodegas();
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
                var value = $("#nompro").getSelectedItemData().id;
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

    
    //metodo utilizado para obtener el listado de productos
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
                    data.push({ 'descripcion': this.descripcion, 'codigo' : this.codigo, 'id': this.id});
                });
            }
        });
    }

    //metodo utilizado para  obtener el listado de bodegas
    function getBodegas(){
         var tds ="";
         $.ajax({
            type: "POST",
            url: "wsrepexist.asmx/bodegas",
            data: '{sucursal: 1}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                tds += '<option selected value="0">Selecione una opcion</option>';
                $.each(msg.d, function () {
                    tds += '<option  value="' + this.id + '">' + this.descripcion + '</option>';

                });

                $('#bod').append(tds);
            }
        });
    }

    //metodo utilizado para obtener el reporte de existencias
    $('#btnConsultar').click(function () {

        $.ajax({
            type: 'POST',
            url: 'wsrepkardex.asmx/consultar',
            data: '{fechaIni: "' + $('#inicio').val() + '", fechaFin: "' + $('#finf').val() + '", prod : ' + $('#codigo').val() + ', bod: '+$('#bod').val()+'}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function () {
                $('#datos').empty();
            },
            success: function (msg) {
                var tds = "";
                $.each(msg.d, function () {
                    tds += '<tr class="odd">';
                    tds = tds + '<td>' + this.articulo + '</td>';
                    tds = tds + '<td>' + this.documento + '</td>';
                    tds = tds + '<td>' + this.fecha + '</td>';
                    tds = tds + '<td>' + this.bodega + '</td>';
                    tds = tds + '<td>' + this.cini + '</td>';
                    tds = tds + '<td>' + this.cmov + '</td>';
                    tds = tds + '<td>' + this.cfin + '</td>';
                    tds = tds + '<td>' + this.usuario + '</td>';
                    tds = tds + '</tr>';
                });
                $("#datos").append(tds);
            }

        });
    });
});



