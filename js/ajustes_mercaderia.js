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

var totalfac = 0;



$(function () {
    cargarBodegas();

    var usuario = window.atob(getCookie("us"));

    $('#bodega').change(function () {
        if ($(this).val() > 0) {
            var datosAutocomplete = [];

            //consume el ws para obtener los datos
            $.ajax({
                url: 'wstraslados.asmx/ObtenerExistenciasPorBodega',
                data: '{bodega: ' + $(this).val() + '}',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                success: function (msg) {
                    $.each(msg.d, function () {
                        var dat = { 'descripcion': this.descripcion + '[' + this.codigo + ']', 'codigo': this.codigo, 'id': this.id, 'desc': this.descripcion, 'cantidad': this.cantidad }
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


    //accion para cargar la tabla
    $('#bt-agregar').click(function () {

        var cantidadentabla = 0;


        for (var i = 0; i < datos.length; i++) {

            if (datos[i].codigo == $('#codigoproducto').val() && datos[i].bodega == $('#bodega').val()) {
                cantidadentabla += parseInt(datos[i].cantidad);
            }
        }


        if ($('#cantidad').val() == "" || $('#producto').val() == "" || $('#tipo').val() == "") {
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

        } else if ($('#tipo').val() == 2 && (parseInt($('#cantidad').val()) +  parseInt(cantidadentabla)) > parseInt($('#cantidadexistente').val()) ) {

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


            for (var i = 0; i < datos.length; i++) {
               

               var resultado = false;
               if (datos[i].codigo == $('#codigoproducto').val() && datos[i].bodega == $('#bodega').val() && datos[i].tipo == $('#tipo').val()) {


                    
                    if ($('#tipo').val() == 2 && (parseInt($('#cantidad').val()) + parseInt(datos[i].cantidad)) > parseInt($('#cantidadexistente').val())) {

                        $('.jq-toast-wrap').remove();
                        $.toast({
                            heading: '¡ERROR!',
                            text: "NO EXISTE LA SUFICIENTE CANTIDAD PARA REDUCIR LA EXISTENCIA DEL PRODUCTO",
                            position: 'bottom-right',
                            showHideTransition: 'plain',
                            icon: 'error',
                            stack: false
                        });

                    } else {
                        datos[i].cantidad = parseInt($('#cantidad').val()) + parseInt(datos[i].cantidad)
                    }


                   resultado = true;

                    
                }
            }


            if (!resultado) {
                var linea = { 'cantidad': $('#cantidad').val(), 'codigo': $('#codigoproducto').val(), 'descripcion': $('#nomproducto').val(), 'tipo': $('#tipo').val(), 'id': $('#idproducto').val(), 'observacion': $('#observacion').val(), 'bodega': $('#bodega').val(), 'bod': $('#bodega option:selected').text() };
                datos.push(linea);

            }

           
            var total = 0;
            $('#tbody').html(null);
            for (var i = 0; i < datos.length; i++) {
                var tipo = '';


               if ($('#tipo').val() == 1) {
                    tipo = '+';
               } else {
                    tipo = '-'
               }


                var tds = '<tr><td>' + datos[i].codigo + '</td><td>' + datos[i].descripcion + '</td><td>' + tipo + datos[i].cantidad + '</td><td>' + datos[i].observacion + '</td><td>' + datos[i].bod + '</td><td onclick="eliminar(' + i + ')"><center><button class="btn btn-danger btn-sm"><i class="material-icons">delete_forever</i></button></center></td></tr>'

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
            $('#idproducto').val(null);
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
        $('#btn-cancelar').attr('disabled', true);

        if (validarForm()) {
            //consume el ws para obtener los datos
            $.ajax({
                url: 'wsajuste_mercaderia.asmx/RealizarAjuste',
                data: '{usuario:"' + usuario + '",  datos_ajuste: ' + JSON.stringify(datos) + '}',
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
    $('#btn-cancelar').click(function () {
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
   

    var result = true
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
    $('#bodega').val(0);
    $('#tipo').val(0);
    datos = [];

    $(".footable").footable({
        "paging": {
            "enabled": true,
            "position": "center"
        }
    });



    

    $('#btn-guardar').removeAttr('disabled', true);
    $('#btn-cancelar').removeAttr('disabled', true);
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
        var tipo = '';


        if ($('#tipo').val() == 1) {
            tipo = '+';
        } else {
            tipo = '-'
        }


        var tds = '<tr><td>' + datos[i].codigo + '</td><td>' + datos[i].descripcion + '</td><td>' + tipo + datos[i].cantidad + '</td><td>' + datos[i].observacion + '</td><td>' + datos[i].bod + '</td><td onclick="eliminar(' + i + ')"><center><button class="btn btn-danger btn-sm"><i class="material-icons">delete_forever</i></button></center></td></tr>'

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
                $('#bodega').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
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
