$(window).on('load', function () {
    /* footable  */
    $(".footable").footable({
        "paging": {
            "enabled": true,
            "position": "center"
        }
    });
});


var datos = [];

$(function () {
    cargarBodegas();
    

    //accion para cargar la tabla
    $('#bt-agregar').click(function () {

        var cantidadentabla = 0;


        for (var i=0; i < datos.length; i++) {

            if (datos[i].codigo == $('#codigoproducto').val() && datos[i].origen == $('#bodegaorigen').val() ) {
                cantidadentabla += parseInt(datos[i].cantidad);
            }
        }

        

        if ($('#cantidad').val() == "" || $('#producto').val() == "" || $('#bodegaorigen').val() == "" || $('#bodegadestino').val() == "") {
            $('.jq-toast-wrap').remove();
            $.toast({
                heading: '¡ERROR!',
                text: "ES  NECESARIO INGRESAR EL NOMBRE DEL PRODUCTO, CODIGO, CANTIDAD Y PRECIO",
                position: 'bottom-right',
                showHideTransition: 'plain',
                icon: 'error',
                stack: false
            });

        } else if ($('#idproducto').val() == "") {

            $('.jq-toast-wrap').remove();
            $.toast({
                heading: '¡ERROR!',
                text: "EL PRODUCTO NO ESTA REGISTRADO EN EL SISTEMA",
                position: 'bottom-right',
                showHideTransition: 'plain',
                icon: 'error',
                stack: false
            });

        }
        else if ($('#cantidadexistente').val() == "") {

            $('.jq-toast-wrap').remove();
            $.toast({
                heading: '¡ERROR!',
                text: "LA CANTIDAD SOLICITADA EXCEDE LA CANTIDAD DE PRODUCTO EXISTENTE",
                position: 'bottom-right',
                showHideTransition: 'plain',
                icon: 'error',
                stack: false
            });

        }
        else if ( parseInt( $('#cantidadexistente').val() ) < (parseInt( $('#cantidad').val() ) +  cantidadentabla ) ) {

             $('.jq-toast-wrap').remove();
             $.toast({
                heading: '¡ERROR!',
                text: "LA CANTIDAD SOLICITADA EXCEDE LA CANTIDAD DE PRODUCTO EXISTENTE",
                position: 'bottom-right',
                showHideTransition: 'plain',
                icon: 'error',
                stack: false
             });

        }
        else {
            var resultado = false;
             for (var i = 0; i < datos.length; i++) {
                 if (datos[i].codigo == $('#codigoproducto').val() && datos[i].origen == $('#bodegaorigen').val() && datos[i].destino == $('#bodegadestino').val()) {
                     if (parseInt($('#cantidadexistente').val()) < (parseInt($('#cantidad').val()) + parseInt(datos[i].cantidad))) {

                         $('.jq-toast-wrap').remove();
                         $.toast({
                             heading: '¡ERROR!',
                             text: "LA CANTIDAD SOLICITADA EXCEDE LA CANTIDAD DE PRODUCTO EXISTENTE",
                             position: 'bottom-right',
                             showHideTransition: 'plain',
                             icon: 'error',
                             stack: false
                         });

                     } else {
                         datos[i].cantidad = parseInt(datos[i].cantidad) + parseInt($('#cantidad').val());
                     }

                    resultado = true;
                 }

             }

             if (!resultado) {
                 var linea = {
                     'cantidad': $('#cantidad').val(), 'codigo': $('#codigoproducto').val(), 'descripcion': $('#nomproducto').val(), 'bo1': $('#bodegaorigen option:selected').text(), 'id': $('#idproducto').val(), 'bo2': $('#bodegadestino option:selected').text(), 'origen': $('#bodegaorigen').val(), 'destino': $('#bodegadestino').val(), 'observacion': $('#observacion').val()
                 };
                 datos.push(linea);
             }

            

            var total = 0; 
            $('#tbody').html(null);
            for (var i = 0; i < datos.length; i++) {
                total += (datos[i].cantidad * datos[i].precio);
                var tds = '<tr><td>' + datos[i].codigo + '</td><td>' + datos[i].descripcion + '</td><td>' + datos[i].cantidad + '</td><td>' + datos[i].bo1 + '</td><td>' + datos[i].bo2 + '</td><td onclick="eliminar(' + i + ')"><center><button class="btn btn-danger btn-sm"><i class="material-icons">delete_forever</i></button></center></td></tr>'

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

            $('#codigoproducto').val(null);
            $('#cantidad').val(null);
            $('#producto').val(null);
            $('#precio').val(null);
            $('#idproducto').val(null);
        }

    });


    $('#bodegaorigen').change(function () {
        if ($(this).val() > 0) {
            //consume el ws para obtener los datos
            $.ajax({
                url: 'wstraslados.asmx/ObtenerBodegasDiferentesA',
                data: '{bodega: ' + $(this).val() + '}',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                success: function (msg) {
                    $('#bodegadestino').html('<option value="0">Seleccione Una Opción</option>');
                    $.each(msg.d, function () {
                        $('#bodegadestino').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
                    });
                }
            });

            var datosAutocomplete = [];

            //consume el ws para obtener los datos
            $.ajax({
                url: 'wstraslados.asmx/ObtenerExistenciasPorBodega',
                data: '{bodega: ' + $(this).val() + '}',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                success: function (msg) {
                    $.each(msg.d, function () {
                        var dat = { 'descripcion': this.descripcion + '[' + this.codigo + ']', 'codigo': this.codigo, 'id': this.id, 'desc': this.descripcion,'cantidad' : this.cantidad }
                        datosAutocomplete.push(dat);
                    });


                    var options = {
                        data: datosAutocomplete,

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


    //accion  para guardar o actualizar los datos
    $('#btn-guardar').click(function () {
        $('#btn-guardar').attr('disabled', true);
        $('#bt-cancelar').attr('disabled', true);
        var usuario = 'admin1';
        if (validarForm()) {
            
            //consume el ws para obtener los datos
            $.ajax({
                url: 'wstraslados.asmx/Trasladar',
                data: '{usuario : "' + usuario + '" ,  observacion : "' + $('#observacion').val() +'",  traslado: ' + JSON.stringify(datos) + '}',
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

                        window.open(arr[2], '_blank');
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
    $('#bt-cancelar').click(function () {
        limpiar();
    })

});


//funcion para cargar las de
function cargarClasificacion() {

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wscargar_datos.asmx/cargarClasificacion',
        data: '',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#clasificacion').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
        }
    });

}



//funcion para cargar las de
function cargarTipoPedido() {

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wscargar_datos.asmx/cargarTipoPedido',
        data: '',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#tipoorden').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
        }
    });

}




function validarForm() {

    var result = true;


    var mensaje = 'Existen Datos que debe ingresar para poder realizar la acción solicitada';
    if (datos.length == 0) {
        result = false
        mensaje = 'Debe Ingresar al menos un producto a la orden'
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
    $('#observacion').val(null)
    $('#tbody').html(null);

    datos = [];

    $(".footable").footable({
        "paging": {
            "enabled": true,
            "position": "center"
        }
    });



    $('#nit').removeClass('is-invalid');
    $('#nit').removeClass('is-valid');

    $('#solicitante').removeClass('is-invalid');
    $('#solicitante').removeClass('is-valid');


    $('#moneda').removeClass('is-invalid');
    $('#moneda').removeClass('is-valid');

    $('#sucursal').removeClass('is-invalid');
    $('#sucursal').removeClass('is-valid');

    $('#tipoorden').removeClass('is-invalid');
    $('#tipoorden').removeClass('is-valid');

    $('#departamento').removeClass('is-invalid');
    $('#departamento').removeClass('is-valid');

    $('#tipoorden').removeClass('is-invalid');
    $('#tipoorden').removeClass('is-valid');

    $('#btn-guardar').removeAttr('disabled', true);
    $('#bt-cancelar').removeAttr('disabled', true);
    $('#btn-guardar').html('<i class="material-icons">add</i>Guardar');
    $('#btn-guardar').removeClass('btn-info');
    $('#btn-guardar').removeClass('btn-warning');
    $('#btn-guardar').addClass('btn-success');
}

// funcion para cargar datos en el formulario
function cargarenFormulario(id, descripcion, codigo, codigo1, codigo2, idcolor, idmarca, tipo, idclasificacion, preciogt, precioes, costo, idsubclasificacion, idsubmarca) {
    limpiar();
    $('#id').val(id);
    $('#descripcion').val(descripcion);

    $('#region').html('<option value="0">Seleccione Una Opción</option>');
    $('#sucursal').html('<option value="0">Seleccione Una Opción</option>');

    $('#codigo').val(codigo);
    $('#descripcion').val(descripcion);
    $('#tipo').val(tipo);
    $('#codigo1').val(codigo1);
    $('#codigo2').val(codigo2);
    $('#marca').val(idmarca);
    $('#submarca').val('<option value="0">Seleccione Una Opción</option>');
    $('#clasificacion').val(idclasificacion);
    $('#sub-clasificacion').val('<option value="0">Seleccione Una Opción</option>');
    $('#color').val(idcolor);
    $('#preciogt').val(preciogt);
    $('#precioes').val(precioes);
    $('#costo').val(costo);


    //consume el ws para obtener los datos
    $.ajax({
        url: 'wscargar_datos.asmx/cargarSubClasificacionAtr',
        data: '{art: ' + idclasificacion + '}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            $.each(msg.d, function () {
                $('#sub-clasificacion').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
            $('#sub-clasificacion').val(idsubclasificacion);
        }
    });

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wscargar_datos.asmx/cargarSubMarcasporMarc',
        data: '{marca: ' + idmarca + '}',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            $.each(msg.d, function () {
                $('#submarca').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
            $('#submarca').val(idsubmarca);
        }
    });




    $('#MdNuevo').modal('toggle')

    $('#btn-guardar').html('<i class="material-icons">cached</i>Actualizar');
    $('#btn-guardar').removeClass('btn-success');
    $('#btn-guardar').removeClass('btn-warning');
    $('#btn-guardar').addClass('btn-info');
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
        var tds = '<tr><td>' + datos[i].codigo + '</td><td>' + datos[i].descripcion + '</td><td>' + datos[i].cantidad + '</td><td>' + datos[i].bo1 + '</td><td>' + datos[i].bo2 + '</td><td onclick="eliminar(' + i + ')"><center><button class="btn btn-danger btn-sm"><i class="material-icons">delete_forever</i></button></center></td></tr>'

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

    $('#codigoproducto').val(null);
    $('#cantidad').val(null);
    $('#producto').val(null);
    $('#precio').val(null);
    $('#idproducto').val(null);
}



function cargarBodegas() {
    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_bodegas.asmx/ObtenerDatos',
        data: '',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            $.each(msg.d, function () {
                $('#bodegaorigen').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
        }
    });
}