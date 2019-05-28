'use strict'
$(window).on('load', function () {
    $('.fecha').daterangepicker({
        singleDatePicker: true,
        showDropdowns: true,
        locale: {
            format: 'DD/MM/YYYY'
        },
        minYear: 1901
    }, function (start, end, label) { });
});


$(function () {
    mostrarDatos();

    //accion  al momento de acepatar elminar
    $('#bt-eliminar').click(function () {
        $('#bt-eliminar').attr('disabled', true);
        $('#bt-no').attr('disabled', true);
        var id = $('#id').val()
        //consume el ws para obtener los datos
        $.ajax({
            url: 'wscotizacion.asmx/Inhabilitar',
            data: '{id: ' + id + '}',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            beforeSend: function () {
                $('#bt-eliminar').removeClass('btn-success');
                $('#bt-eliminar').addClass('btn-warning');
                $('#bt-eliminar').html('<i class="material-icons">query_builder</i>Cargando...')
            },
            success: function (msg) {

                var arr = msg.d.split('|');


                if (arr[0] == 'SUCCESS') {
                    $('.jq-toast-wrap').remove();
                    $.toast({
                        heading: '¡EXITO!',
                        text: arr[1],
                        position: 'bottom-right',
                        showHideTransition: 'plain',
                        icon: 'success',
                        stack: false
                    });

                    $('#MdDeshabilitar').modal('toggle');

                } else {
                    $('.jq-toast-wrap').remove();
                    $.toast({
                        heading: '¡ERROR!',
                        text: arr[1],
                        position: 'bottom-right',
                        showHideTransition: 'plain',
                        icon: 'error',
                        stack: false
                    });
                }

                $('#bt-eliminar').removeAttr('disabled', true);
                $('#bt-no').removeAttr('disabled', true);

                $('#bt-eliminar').removeClass('btn-warning');
                $('#bt-eliminar').addClass('btn-success');
                $('#bt-eliminar').html('<i class="material-icons">done</i>Si')
                mostrarDatos();

            }
        });
    });

    $('#bt-consultar').click(function () {
        mostrarDatos();
    });

});


function UrlExists(url) {
    var http = new XMLHttpRequest();
    http.open('HEAD', url, false);
    http.send();
    return http.status != 404;
} 


function detalle(id,descuento) {

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wscotizacion.asmx/obtenerListProductos',
        data: '{cotizacion: '+ id +'}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
           
        },
        success: function (msg) {
            var tds = "";
            var total = 0;
            $('#tbod-datos-detalle').html(null);
            $.each(msg.d, function () {
                total += this.precio * this.cantidad;
                tds = "<tr class='odd'><td>" + this.cantidad + "</td><td>" + this.codigo + "</td>><td>" + this.descripcion + "</td><td>" + this.bo + "</td><td>" + (this.precio).toFixed(2) + "</td><td>" + (this.precio*this.cantidad).toFixed(2) + "</td></tr>'"
                $("#tbod-datos-detalle").append(tds);
            });

      
            tds = "<tr class='odd'><td>--</td><td>--</td>><td><center><b>DESCUENTO</b></center></td><td>--</td><td>--</td><td><b>" + descuento.toFixed(2) + "</b></td></tr>'" + 
                "<tr class='odd'><td>--</td><td>--</td>><td><center><b>TOTAL</b></center></td><td>--</td><td>--</td><td><b>" + (total - descuento).toFixed(2) + "</b></td></tr>'"
            $("#tbod-datos-detalle").append(tds);
        }
    });

    $('#Mddetalle').modal('toggle');
}

//metodo utilizado para mostrar lista de datos 
function mostrarDatos() {

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wscotizacion.asmx/ObtenerCotizaciones',
        data: '{fechainicio : "' + $('#fechainicio').val() + '", fechafin : "' + $('#fechafin').val() +'"}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
            $('#tbod-datos').html(null);
            $('#tab-datos').dataTable().fnDeleteRow();
            $('#tab-datos').dataTable().fnUpdate();
            $('#tab-datos').dataTable().fnDestroy();
        },
        success: function (msg) {
            var tds = "";
            $('#tbod-datos').html(null);
            $.each(msg.d, function () {
                tds = "<tr class='odd'><td>" + this.nit + "</td><td>" + this.cliente + "</td>><td>" + this.sucursal + "</td><td>" + this.fecha + "</td><td>" + this.total + "</td><td> " +
                    "<span onclick='reimpimir(" + this.id + ")' class='Mdnew btn btn-sm btn-outline-info' data-container='body' data-trigger='hover' data-toggle='popover' data-placement='bottom' data-content='Click para poder cargar los datos en el formulario, para poder actualizar.' data-original-title='' title ='' ><i class='material-icons'>print</i></span> " +
                    "<span onclick='detalle(" + this.id + ", "+this.descuento +")'   class='btn btn-sm btn-outline-warning' data-container='body' data-trigger='hover' data-toggle='popover' data-placement='bottom' data-content='Click para ver el detalle de la cotizacion' data-original-title='' title ='' > <i class='material-icons'> description </i></span>" +
                    "<span style='margin-left: 5px' onclick='eliminar(" + this.id + ")'  class='btn btn-sm btn-outline-danger' data-container='body' data-trigger='hover' data-toggle='popover' data-placement='bottom' data-content='Click para poder Inhabilitar el dato seleccionado, Esto hara que dicho dato no aparesca en ninguna acción, menu o formulario del sistema.' data-original-title='' title=''> <i class='material-icons'> delete_sweep </i></span>" +
                    "</td></tr>'"


                $("#tbod-datos").append(tds);
            });

            $('#tab-datos').dataTable();
            $('[data-toggle="popover"]').popover();
        }
    });

};


function eliminar(id) {
    $('#id').val(id);
    $('#MdDeshabilitar').modal('toggle');
}

function reimpimir(id) {
    $.ajax({
        url: 'wscotizacion.asmx/Reimprimir',
        data: '{idcotizacion : ' + id + '}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {

            var arr = msg.d.split('|');


            if (arr[0] == 'SUCCESS') {

                if (!UrlExists(arr[1])) {
                    $('.jq-toast-wrap').remove();
                    $.toast({
                        heading: '¡ERROR!',
                        text: "ESTA ORDEN DE COMPRA NO EXISTE",
                        position: 'bottom-right',
                        showHideTransition: 'plain',
                        icon: 'error',
                        stack: false
                    });
                } else {

                    window.open(arr[1], '_blank');
                }

            } else {

                $('.jq-toast-wrap').remove();
                $.toast({
                    heading: '¡ERROR!',
                    text: arr[1],
                    position: 'bottom-right',
                    showHideTransition: 'plain',
                    icon: 'error',
                    stack: false
                });
            }
        }
    });
}


function limpiar() {
  //  mostrarDatos();
}