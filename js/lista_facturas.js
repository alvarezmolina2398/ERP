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

var usuario = window.atob(getCookie("us"));

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


    //anula la factura
    $('#bt-anular').click(function () {

        var observacion = $('#observaciones_anular').val();


        if (observacion != "") {
            $.ajax({
                url: 'wslista_facturas.asmx/AnularFactura',
                data: '{id : ' + $('#id').val() + ', observacion: "' + observacion + '", usuario: "' + usuario + '"}',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                success: function (msg) {

                    var arr = msg.d.split('|');


                    if (arr[0] == 'SUCCESS') {

                        $('.jq-toast-wrap').remove();
                        $.toast({
                            heading: '¡INFORMACION!',
                            text: arr[1],
                            position: 'bottom-right',
                            showHideTransition: 'plain',
                            icon: 'info',
                            stack: false
                        });

                        $('#MdAnularFac').modal('toggle');
                        $('#observaciones_anular').val(null);
                        mostrarDatos();
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
        } else {
            $('.jq-toast-wrap').remove();
            $.toast({
                heading: '¡ERROR!',
                text: "DEBE INGRESAR LAS OBSERVACIONES",
                position: 'bottom-right',
                showHideTransition: 'plain',
                icon: 'error',
                stack: false
            });
            $('#observaciones_anular').focus();
        }
        
    });

    //nota de credito
    $('#bt-notacredito').click(function () {

        var observacion = $('#observaciones_nota').val();


        if (observacion != "") {
            $.ajax({
                url: 'wslista_facturas.asmx/RealizarNotaCredito',
                data: '{id : ' + $('#id').val() + ', observacion: "' + observacion + '", usuario: "' + usuario + '"}',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                success: function (msg) {

                    var arr = msg.d.split('|');


                    if (arr[0] == 'SUCCESS') {

                        $('.jq-toast-wrap').remove();
                        $.toast({
                            heading: '¡INFORMACION!',
                            text: arr[1],
                            position: 'bottom-right',
                            showHideTransition: 'plain',
                            icon: 'info',
                            stack: false
                        });

                        $('#MdNotaCredito').modal('toggle');
                        $('#observaciones_nota').val(null);
                        mostrarDatos();

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
                        $('#observaciones_nota').focus();
                    }
                }
            });
        } else {
            $('.jq-toast-wrap').remove();
            $.toast({
                heading: '¡ERROR!',
                text: "DEBE INGRESAR LAS OBSERVACIONES",
                position: 'bottom-right',
                showHideTransition: 'plain',
                icon: 'error',
                stack: false
            });
        }

    });

});


function UrlExists(url) {
    var http = new XMLHttpRequest();
    http.open('HEAD', url, false);
    http.send();
    return http.status != 404;
}


function detalle(id, descuento) {

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wscotizacion.asmx/obtenerListProductos',
        data: '{cotizacion: ' + id + '}',
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
                tds = "<tr class='odd'><td>" + this.cantidad + "</td><td>" + this.codigo + "</td>><td>" + this.descripcion + "</td><td>" + this.bo + "</td><td>" + (this.precio).toFixed(2) + "</td><td>" + (this.precio * this.cantidad).toFixed(2) + "</td></tr>'"
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
        url: 'wslista_facturas.asmx/ObtenerFacturas',
        data: '{fechainicio : "' + $('#fechainicio').val() + '", fechafin : "' + $('#fechafin').val() + '"}',
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
                tds = "<tr class='odd'><td>" + this.nit + "</td><td>" + this.cliete + "</td><td>" + this.factura + "</td><td>" + this.fecha + "</td><td>" + this.total + "</td><td> " +
                    "<span onclick='reimpimir(" + this.id + ")' class='Mdnew btn btn-sm btn-outline-info' data-container='body' data-trigger='hover' data-toggle='popover' data-placement='bottom' data-content='Click para poder cargar los datos en el formulario, para poder actualizar.' data-original-title='' title ='' ><i class='material-icons'>print</i></span> " +
                    "<span style='margin-left: 5px' onclick='eliminar(" + this.id + ",\"" + this.fecha + "\",\"" + this.nit + "\")'  class='btn btn-sm btn-outline-danger' data-container='body' data-trigger='hover' data-toggle='popover' data-placement='bottom' data-content='Click para poder Inhabilitar el dato seleccionado, Esto hara que dicho dato no aparesca en ninguna acción, menu o formulario del sistema.' data-original-title='' title=''> <i class='material-icons'> delete_sweep </i></span>" +
                    "</td></tr>'"


                $("#tbod-datos").append(tds);
            });

            $('#tab-datos').dataTable();
            $('[data-toggle="popover"]').popover();
        }
    });

};


function eliminar(id, fecha, nit) {
    var fech = fecha.split('/');


    var f = new Date();
    var mesactual = f.getMonth() + 1;

    var mesfac = fech[1];

    var diferencia  = mesactual-mesfac;


    if (diferencia >= 2) {
        $('#observaciones_nota').val(null);
        $('#MdNotaCredito').modal('toggle');
    } else {
        $('#observaciones_anular').val(null);
        $('#MdAnularFac').modal('toggle');
    }


    $('#id').val(id);
    
}

function reimpimir(id) {
    $.ajax({
        url: 'wsfacturacion.asmx/Reimprimir',
        data: '{id : ' + id + '}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {

            if (UrlExists(msg.d)) {
                window.open(msg.d, '_blank');
            }else {
                $('.jq-toast-wrap').remove();
                $.toast({
                    heading: '¡ERROR!',
                    text: 'Error de Reimpresion de Factura',
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


//metodo para obtener la sesion
function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}
