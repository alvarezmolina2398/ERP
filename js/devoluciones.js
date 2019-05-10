$(window).on('load', function () {
    /* footable  */
    $(".footable").footable({
        "paging": {
            "enabled": true,
            "position": "center"
        }
    });
});
var usuario = window.atob(getCookie("us"));
var datosAutocomplete = [];

//consume el ws para obtener los datos
$.ajax({
    url: 'wscargar_datos.asmx/cargarProveedor',
    data: '',
    type: 'POST',
    contentType: 'application/json; charset=utf-8',
    success: function (msg) {
        $.each(msg.d, function () {
            var dat = { 'nit': this.extra, 'descripcion': this.descripcion, 'id': this.id }
            datosAutocomplete.push(dat);
        });
    }
});

var datos = [];

var totalfac = 0;



function UrlExists(url) {
    var http = new XMLHttpRequest();
    http.open('HEAD', url, false);
    http.send();
    return http.status != 404;
}

$(function () {
    cargarBodegas();
    $('[data-toggle="tooltip"]').tooltip();
    $('#cod-reimprimir').val(null);


  
    var options = {
        data: datosAutocomplete,

        getValue: function (element) {
            return element.nit
        },


        template: {
            type: "description",
            fields: {
                description: "descripcion"
            }
        },
        list: {
            match: {
                enabled: true
            }, onSelectItemEvent: function () {
                var value = $("#nit").getSelectedItemData().descripcion;
                var value2 = $('#nit').getSelectedItemData().id;
                $("#idproveedor").val(value2).trigger("change");
                $("#nombre").val(value).trigger("change");
            }

        },
    }


    var options2 = {
        data: datosAutocomplete,

        getValue: function (element) {
            $('#nit').val(element.nit);
            $('#idproveedor').val(element.id);
            return element.descripcion
        },


        template: {
            type: "description",
            fields: {
                description: "nit"
            }
        },
        list: {
            match: {
                enabled: true
            }, onSelectItemEvent: function () {
                var value = $("#nombre").getSelectedItemData().nit;
                var value2 = $('#nombre').getSelectedItemData().id;
                $("#idproveedor").val(value2).trigger("change");
                $("#nit").val(value).trigger("change");
            }
        },
    }

    $('#nit').easyAutocomplete(options);
    $('#nombre').easyAutocomplete(options2);


    $('#bodega').change(function () {
        if ($(this).val() > 0) {

            var datos = [];

            //consume el ws para obtener los datos
            $.ajax({
                url: 'wstraslados.asmx/ObtenerExistenciasPorBodega',
                data: '{bodega: ' + $(this).val() + '}',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                success: function (msg) {
                    $.each(msg.d, function () {
                        var dat = { 'descripcion': this.descripcion + '[' + this.codigo + ']', 'codigo': this.codigo, 'id': this.id, 'desc': this.descripcion, 'cantidad': this.cantidad }
                        datos.push(dat);
                    });


                    var options = {
                        data: datos,

                        getValue: function (element) {
                            return element.descripcion
                        },

                        list: {
                            match: {
                                enabled: true
                            },
                            onSelectItemEvent: function () {
                                var value = $("#producto").getSelectedItemData().id;
                                var value2 = $('#producto').getSelectedItemData().codigo;
                                var value3 = $('#producto').getSelectedItemData().desc;
                                var value4 = $('#producto').getSelectedItemData().cantidad;
                                $("#idproducto").val(value).trigger("change");
                                $("#codigoproducto").val(value2).trigger("change");
                                $("#nomproducto").val(value3).trigger("change");
                                $('#cantidadexistente').val(value4);
                            }
                        },
                    }

                    $("#producto").easyAutocomplete(options);

                }
            });

        }
    });



    //accion para cargar la tabla
    $('#bt-agregar').click(function () {
        if ($('#bodega').val() == 0) {
            $('.jq-toast-wrap').remove();
            $.toast({
                heading: '¡ERROR!',
                text: "DEBE SELECCIONAR LA BODEGA",
                position: 'bottom-right',
                showHideTransition: 'plain',
                icon: 'error',
                stack: false
            });
        } else {
         
            var cantidadTotal = 0;
            var existe = false;
            var posicion = 0;


            for (var i = 0; i < datos.length; i++) {


                if (datos[i].codigo == $('#codigoproducto').val() && datos[i].bodega == $('#bodega').val()) {

                    cantidadTotal = parseInt(datos[i].cantidad) + parseInt($('#cantidad').val());
                    posicion = i;
                    existe = true;
                }

            };

            if ($('#cantidad').val() == "") {
                $('.jq-toast-wrap').remove();
                $.toast({
                    heading: '¡ERROR!',
                    text: "INGRESE LA CANTIDAD DE PRODUCTOS A VENDER",
                    position: 'bottom-right',
                    showHideTransition: 'plain',
                    icon: 'error',
                    stack: false
                });

            }
            else if ($('#costo').val() == "") {
                $('.jq-toast-wrap').remove();
                $.toast({
                    heading: '¡ERROR!',
                    text: "INGRESE EL COSTO DE PRODUCTOS A VENDER",
                    position: 'bottom-right',
                    showHideTransition: 'plain',
                    icon: 'error',
                    stack: false
                });

            }
            else if (parseInt($('#cantidad').val()) < parseInt(cantidadTotal)) {

                $('.jq-toast-wrap').remove();
                $.toast({
                    heading: '¡ERROR!',
                    text: "NO EXISTE LA SUFICIENTE CANTIDAD PARA REDUCIR LA EXISTENCIA DEL PRODUCTO",
                    position: 'bottom-right',
                    showHideTransition: 'plain',
                    icon: 'error',
                    stack: false
                });

            }
            else if (parseInt($('#cantidadexistente').val()) < parseInt(cantidad)) {

                $('.jq-toast-wrap').remove();
                $.toast({
                    heading: '¡ERROR!',
                    text: "NO EXISTE LA SUFICIENTE CANTIDAD PARA REDUCIR LA EXISTENCIA DEL PRODUCTO",
                    position: 'bottom-right',
                    showHideTransition: 'plain',
                    icon: 'error',
                    stack: false
                });

            }
            else if (parseInt($('#cantidadexistente').val()) < parseInt($('#cantidad').val())) {

                $('.jq-toast-wrap').remove();
                $.toast({
                    heading: '¡ERROR!',
                    text: "NO EXISTE LA SUFICIENTE CANTIDAD PARA REDUCIR LA EXISTENCIA DEL PRODUCTO",
                    position: 'bottom-right',
                    showHideTransition: 'plain',
                    icon: 'error',
                    stack: false
                });

            }
            else {
                if (!existe) {
                    var linea = { 'cantidad': $('#cantidad').val(), 'codigo': $('#codigoproducto').val(), 'descripcion': $('#nomproducto').val(), 'id': $('#idproducto').val(), 'costo': $('#costo').val(), 'bodega': $('#bodega').val(), 'bo': $('#bodega option:selected').text() };
                    datos.push(linea);


                } else {

                    if (cantidadTotal > cantidad) {

                        $('.jq-toast-wrap').remove();
                        $.toast({
                            heading: '¡ERROR!',
                            text: "NO EXISTE LA SUFICIENTE CANTIDAD PARA REDUCIR LA EXISTENCIA DEL PRODUCTO TOT",
                            position: 'bottom-right',
                            showHideTransition: 'plain',
                            icon: 'error',
                            stack: false
                        });

                    } else {
                        datos[posicion].cantidad = cantidadTotal;
                    }

                }



                var total = 0;
                $('#tbody').html(null);
                for (var i = 0; i < datos.length; i++) {
                    total += parseFloat(datos[i].cantidad) * parseFloat(datos[i].costo);
                    var tds = '<tr><td>' + datos[i].codigo + '</td><td>' + datos[i].descripcion + '</td><td>' + datos[i].bo + '</td><td>' + datos[i].cantidad + '</td><td style="text-align: right">' + parseFloat(datos[i].costo).toFixed(2) + '</td><td style="text-align: right">' + (parseFloat(datos[i].cantidad) * parseFloat(datos[i].costo)).toFixed(2) + '</td><td onclick="eliminar(' + i + ')"><center><button class="btn btn-danger btn-sm"><i class="material-icons">delete_forever</i></button></center></td></tr>'

                    $('#tbody').append(tds);
                };

                totalfac = total;

                $(".footable").footable({
                    "paging": {
                        "enabled": true,
                        "position": "center"
                    }
                });

                $('#codigoproducto').val(null);
                $('#cantidad').val(null);
                $('#producto').val(null);
                $('#idproducto').val(null);
                $('#costo').val(null);
            }
        }
     });



    //accion  al momento de acepatar elminar
    $('#bt-eliminar').click(function () {
        $('#bt-eliminar').attr('disabled', true);
        $('#bt-no').attr('disabled', true);

    });

    $('#departamento').change(function () {
        if ($(this).val() > 0) {
            //consume el ws para obtener los datos
            $.ajax({
                url: 'wscargar_datos.asmx/CargarSolicitante',
                data: '{cia: ' + $(this).val() + '}',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                success: function (msg) {
                    $.each(msg.d, function () {
                        $('#solicitante').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
                    });
                }
            });
        }
    });


    //accion  para guardar o actualizar los datos
    $('#btn-guardar').click(function () {
        $('#btn-guardar').attr('disabled', true);
        $('#bt-cancelar').attr('disabled', true);
        if (validarForm()) {
            var serie = $('#seriefac').val();
            var numero = $('#numerofac').val();
            var idproveedor = $('#idproveedor').val();
            //consume el ws para obtener los datos
            $.ajax({
                url: 'wsdevoluciones.asmx/Devolver',
                data: '{ proveedor : '+ idproveedor +' ,  usuario : "'+ usuario +'" ,  serie : "'+ serie +'" , numero : "'+ numero +'",  total : '+ totalfac +' ,  listproductos: ' + JSON.stringify(datos) + '}',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                beforeSend: function () {
                    $('#btn-guardar').removeClass('btn-info');
                    $('#btn-guardar').removeClass('btn-success');
                    $('#btn-guardar').addClass('btn-warning');
                    $('#btn-guardar').html('<i class="material-icons">query_builder</i>Cargando...')
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

                       // window.open(arr[2], '_blank');

                        limpiar();

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
    });


    //accion  para cancelar los datos
    $('#btn-cancelar').click(function () {
        limpiar();
    })

});





function validarForm() {
    var nit = $('#nit');
    var nombre = $('#nombre');
    var serie = $('#seriefac');
    var numero = $('#numerofac');
    var idnit = $('#idproveedor').val();

    nit.removeClass('is-invalid');
    nit.removeClass('is-valid');

    nombre.removeClass('is-invalid');
    nombre.removeClass('is-valid');

    serie.removeClass('is-invalid');
    serie.removeClass('is-valid');

    numero.removeClass('is-invalid');
    numero.removeClass('is-valid');

    var result = true
    if (nit.val() == "") {
        nit.addClass('is-invalid');
        nit.focus();
        $('#btn-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);



        result = false;
    } else {
        nit.addClass('is-valid');
    }


    if (nombre.val() == "") {
        nombre.addClass('is-invalid');
        nombre.focus();
        $('#btn-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);



        result = false;
    } else {
        nombre.addClass('is-valid');
    }


    if (serie.val() == "") {
        serie.addClass('is-invalid');
        serie.focus();
        $('#btn-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);



        result = false;
    } else {
        serie.addClass('is-valid');
    }



    if (numero.val() == "") {
        numero.addClass('is-invalid');
        numero.focus();
        $('#btn-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);



        result = false;
    } else {
        numero.addClass('is-valid');
    }

   


    var mensaje = 'Existen Datos que debe ingresar para poder realizar la acción solicitada';
    if (datos.length == 0) {
        result = false
        mensaje = 'Debe Ingresar al menos un producto a la orden'
    }
    if (idproveedor == "") {
        result = false
        mensaje = 'El Proveedor es Incorrecto '
    }

    if (!result) {
        $('.jq-toast-wrap').remove();
        $.toast({
            heading: 'ADVERTENCIA',
            text: mensaje,
            position: 'bottom-right',
            showHideTransition: 'plain',
            icon: 'warning',
            stack: false
        });
    }




    return result;

}

//metodo para limpiar el formulario
function limpiar() {
    $('#tbody').html(null);
    var nit = $('#nit');
    var nombre = $('#nombre');
    var serie = $('#seriefac');
    var numero = $('#numerofac');
    $('#idproveedor').val(null);
    nit.val(null);
    nombre.val(null);
    serie.val(null);
    numero.val(null);

    nit.removeClass('is-invalid');
    nit.removeClass('is-valid');

    nombre.removeClass('is-invalid');
    nombre.removeClass('is-valid');

    serie.removeClass('is-invalid');
    serie.removeClass('is-valid');

    numero.removeClass('is-invalid');
    numero.removeClass('is-valid');

    datos = [];

    $(".footable").footable({
        "paging": {
            "enabled": true,
            "position": "center"
        }
    });



   

    $('#btn-guardar').removeAttr('disabled', true);
    $('#bt-cancelar').removeAttr('disabled', true);
    $('#btn-guardar').html('<i class="material-icons">add</i>Guardar');
    $('#btn-guardar').removeClass('btn-info');
    $('#btn-guardar').removeClass('btn-warning');
    $('#btn-guardar').addClass('btn-success');
}


// funcion para cargar datos en el formulario
function eliminar(id) {
    var data = datos.splice(id, 1)
    $('.jq-toast-wrap').remove();
    $.toast({
        heading: '¡Informacion!',
        text: "SE HA REMOVIDO UN PRODUCTO DE LA TABLA",
        position: 'bottom-right',
        showHideTransition: 'plain',
        icon: 'info',
        stack: false
    });



    var total = 0;
    $('#tbody').html(null);
    for (var i = 0; i < datos.length; i++) {
        total += (datos[i].cantidad * datos[i].precio);
        var tds = '<tr><td>' + datos[i].codigo + '</td><td>' + datos[i].descripcion + '</td><td>' + parseFloat(datos[i].precio).toFixed(2) + '</td><td>' + datos[i].cantidad + '</td><td>' + parseFloat(datos[i].cantidad * datos[i].precio).toFixed(2) + '</td><td onclick="eliminar(' + i + ')"><center><button class="btn btn-danger btn-sm"><i class="material-icons">delete_forever</i></button></center></td></tr>'

        $('#tbody').append(tds);
    };
    if (total > 0) {
        td = '<tr><td> -- </td><th> <b>TOTAL</b> </th><td><center> --- </center></td><td><center> --- </center></td><td><b>' + parseFloat(total).toFixed(2) + '</b></td><td></td></tr>'
        $('#tbody').append(td);
    }
    totalfac = total;


    $(".footable").footable({
        "paging": {
            "enabled": true,
            "position": "center"
        }
    });
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


function cargarBodegas() {

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_bodegas.asmx/ObtenerDatosPorSucursal',
        data: '{usuario: "' + usuario + '"}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            $.each(msg.d, function () {

                if (this.prioridad == 1) {
                    $('#bodega').append('<option value="' + this.id + '" selected>' + this.descripcion + '</option>');
                    //consume el ws para obtener los datos
                    $.ajax({
                        url: 'wstraslados.asmx/ObtenerExistenciasPorBodega',
                        data: '{bodega: ' + this.id + '}',
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        success: function (msg) {
                            $('#tbod-datos').html(null);
                            var datos = [];
                            $.each(msg.d, function () {
                                var dat = { 'descripcion': this.descripcion + '[' + this.codigo + ']', 'codigo': this.codigo, 'id': this.id, 'desc': this.descripcion, 'cantidad': this.cantidad }
                                datos.push(dat);
                            });

                           
                            var options = {
                                data: datos,

                                getValue: function (element) {
                                    return element.descripcion
                                },

                                list: {
                                    match: {
                                        enabled: true
                                    },
                                    onSelectItemEvent: function () {
                                        var value = $("#producto").getSelectedItemData().id;
                                        var value2 = $('#producto').getSelectedItemData().codigo;
                                        var value3 = $('#producto').getSelectedItemData().desc;
                                        var value4 = $('#producto').getSelectedItemData().cantidad;
                                        $("#idproducto").val(value).trigger("change");
                                        $("#codigoproducto").val(value2).trigger("change");
                                        $("#nomproducto").val(value3).trigger("change");
                                        $('#cantidadexistente').val(value4);
                                    }
                                },
                            }

                            $("#producto").easyAutocomplete(options);

                        }
                    });

                } else {
                    $('#bodega').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
                }

            });
        }
    });
}