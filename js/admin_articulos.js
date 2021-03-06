﻿$(function () {
    $('#costo').attr('disabled', false);
    var user = window.atob(getCookie("us"));
    mostrarDatos();
    cargarMarcas();
    cargarClasificacion();
    cargarColores();
    cargarTipo();

    $('#mdNew').click(function () {
        limpiar();
        $('#requiereProduccion').attr('disabled', false);
        $('#costo').attr('disabled', false);
    });

    //accion  al momento de acepatar elminar
    $('#bt-eliminar').click(function () {
        $('#bt-eliminar').attr('disabled', true);
        $('#bt-no').attr('disabled', true);
        var id = $('#id').val()
        //consume el ws para obtener los datos
        $.ajax({
            url: 'wsadmin_articulos.asmx/Inhabilitar',
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

                limpiar();
                mostrarDatos();

            }
        });
    });

    //accion para cargar las regiones al cambiar de agencia
    $('#marca').change(function () {
        $('#submarca').html('<option value="0">Seleccione Una Opción</option>');
        
        if ($(this).val() != 0) {
            
            //consume el ws para obtener los datos
            $.ajax({
                url: 'wscargar_datos.asmx/cargarSubMarcasporMarc',
                data: '{marca: ' + $(this).val() + '}',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                success: function (msg) {
                    $.each(msg.d, function () {
                        $('#submarca').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
                    });
                }
            });

        }
    });

    //accion para cargar las sucursales al cambiar de region
    $('#clasificacion').change(function () {
        $('#sub-clasificacion').html('<option value="0">Seleccione Una Opción</option>');

        if ($(this).val() != 0) {
            //consume el ws para obtener los datos
            $.ajax({
                url: 'wscargar_datos.asmx/cargarSubClasificacionAtr',
                data: '{art: ' + $(this).val() + '}',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                success: function (msg) {
                    $.each(msg.d, function () {
                        $('#sub-clasificacion').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
                    });
                }
            });

        }


    });


    //accion  para guardar o actualizar los datos
    $('#bt-guardar').click(function () {
        $('#bt-guardar').attr('disabled', true);
        $('#bt-cancelar').attr('disabled', true);
        var codigo = $('#codigo');
        var descripcion = $('#descripcion');
        var tipo = $('#tipo');
        var codigo1 = $('#codigo1');
        var codigo2 = $('#codigo2');
        var marca = $('#marca');
        var submarca = $('#submarca');
        var clasificacion = $('#clasificacion');
        var subclasificacion = $('#sub-clasificacion');
        var color = $('#color');
        var preciogt = $('#preciogt');
        var precioes = 0
        var precio3 = 0;
        var costo = $('#costo');
        var requiereProduccion = 0;


        if ($('#requiereProduccion').prop('checked')) {
            requiereProduccion = 1;
        }
        
        var id = $('#id').val();

        if ($('#precioes').val() != 0) {
            precioes = $('#precioes').val();
        }

        if ($('#precio3').val() != 0) {
            precio3 = $('#precio3').val();
        }

        if (validarForm()) {
            var data1 = '';
            var url1 = ''
            if (id != 0) {
                url1 = 'Actualizar'
                data1 = '{ descripcion : "' + descripcion.val() + '",  codigo : "' + codigo.val() + '",  tipo : ' + tipo.val() + ',  cod_pro1 : "' + codigo1.val() + '",  cod_pro2 : "' + codigo2.val() + '",  marca : ' + marca.val() + ', idsubmarca : ' + submarca.val() + ',  id_clasi : ' + clasificacion.val() + ',  id_subclasi : ' + subclasificacion.val() + ',  idcolor : ' + color.val() + ',  preciogt : "' + preciogt.val() + '",  precioEs : ' + precioes + ', costo : ' + costo.val() + ',  usuario : "' + user + '", id : ' + id + ', precio3: ' + precio3 + ', requiereProduccion : ' + requiereProduccion +'}';
            } else {
                url1 = 'Insertar'
                data1 = '{ descripcion : "' + descripcion.val() + '",  codigo : "' + codigo.val() + '",  tipo : ' + tipo.val() + ',  cod_pro1 : "' + codigo1.val() + '",  cod_pro2 : "' + codigo2.val() + '",  marca : ' + marca.val() + ', idsubmarca : ' + submarca.val() + ',  id_clasi : ' + clasificacion.val() + ',  id_subclasi : ' + subclasificacion.val() + ',  idcolor : ' + color.val() + ',  preciogt : "' + preciogt.val() + '",  precioEs : ' + precioes + ', costo : ' + costo.val() + ',  usuario : "' + user + '", precio3: ' + precio3 + ', requiereProduccion : ' + requiereProduccion +'}';
            }

            //consume el ws para obtener los datos
            $.ajax({
                url: 'wsadmin_articulos.asmx/' + url1,
                data: data1,
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                beforeSend: function () {
                    $('#bt-guardar').removeClass('btn-info');
                    $('#bt-guardar').removeClass('btn-success');
                    $('#bt-guardar').addClass('btn-warning');
                    $('#bt-guardar').html('<i class="material-icons">query_builder</i>Cargando...')
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

                        $('#MdNuevo').modal('toggle');

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

                    limpiar();
                    mostrarDatos();
                }
            });
        }
    })


    //accion  para cancelar los datos
    $('#bt-cancelar').click(function () {
        limpiar();
    })

});

//funcion para cargar las de
function cargarMarcas() {

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wscargar_datos.asmx/cargarMarcas',
        data: '',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#marca').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
        }
    });

}

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
function cargarColores() {

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wscargar_datos.asmx/cargarColores',
        data: '',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#color').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
        }
    });

}

//funcion para cargar las de
function cargarTipo() {

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wscargar_datos.asmx/cargarTipo',
        data: '',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
        },
        success: function (msg) {
            $.each(msg.d, function () {
                $('#tipo').append('<option value="' + this.id + '">' + this.descripcion + '</option>')
            });
        }
    });

}




function validarForm() {
    var codigo = $('#codigo');
    var descripcion = $('#descripcion');
    var tipo = $('#tipo');
    var marca = $('#marca');
    var submarca = $('#submarca');
    var clasificacion = $('#clasificacion');
    var sub_clasificacion = $('#sub-clasificacion');
    var color = $('#color');
    var preciogt = $('#preciogt');
    var costo = $('#costo');


    codigo.removeClass('is-invalid');
    codigo.removeClass('is-valid');

    descripcion.removeClass('is-invalid');
    descripcion.removeClass('is-valid');


    tipo.removeClass('is-invalid');
    tipo.removeClass('is-valid');

    marca.removeClass('is-invalid');
    marca.removeClass('is-valid');

    submarca.removeClass('is-invalid');
    submarca.removeClass('is-valid');

    clasificacion.removeClass('is-invalid');
    clasificacion.removeClass('is-valid');

    sub_clasificacion.removeClass('is-invalid');
    sub_clasificacion.removeClass('is-valid');

    color.removeClass('is-invalid');
    color.removeClass('is-valid');

    preciogt.removeClass('is-invalid');
    preciogt.removeClass('is-valid');


    costo.removeClass('is-invalid');
    costo.removeClass('is-valid');

    var result = true
    if (codigo.val() == "") {
        codigo.addClass('is-invalid');
        codigo.focus();
        $('#bt-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);
        $('.jq-toast-wrap').remove();
        $.toast({
            heading: 'ADVERTENCIA',
            text: 'Existen Datos que debe ingresar para poder realizar la acción solicitada',
            position: 'bottom-right',
            showHideTransition: 'plain',
            icon: 'warning',
            stack: false
        });


        result = false;
    } else {
        codigo.addClass('is-valid');
    }


    if (descripcion.val() == "") {
        descripcion.addClass('is-invalid');
        descripcion.focus();
        $('#bt-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);
        $('.jq-toast-wrap').remove();
        $.toast({
            heading: 'ADVERTENCIA',
            text: 'Existen Datos que debe ingresar para poder realizar la acción solicitada',
            position: 'bottom-right',
            showHideTransition: 'plain',
            icon: 'warning',
            stack: false
        });


        result = false;
    } else {
        descripcion.addClass('is-valid');
    }

    if (tipo.val() == 0 || !tipo.val()) {
        tipo.addClass('is-invalid');
        tipo.focus();
        $('#bt-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);
        $('.jq-toast-wrap').remove();
        $.toast({
            heading: 'ADVERTENCIA',
            text: 'Existen Datos que debe ingresar para poder realizar la acción solicitada',
            position: 'bottom-right',
            showHideTransition: 'plain',
            icon: 'warning',
            stack: false
        });


        result = false;
    } else {
        tipo.addClass('is-valid');
    }


    if (marca.val() == 0 || !marca.val()) {
        marca.addClass('is-invalid');
        marca.focus();
        $('#bt-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);
        $('.jq-toast-wrap').remove();
        $.toast({
            heading: 'ADVERTENCIA',
            text: 'Existen Datos que debe ingresar para poder realizar la acción solicitada',
            position: 'bottom-right',
            showHideTransition: 'plain',
            icon: 'warning',
            stack: false
        });


        result = false;
    } else {
        marca.addClass('is-valid');
    }


    if (submarca.val() == 0 || !submarca.val()) {
        submarca.addClass('is-invalid');
        submarca.focus();
        $('#bt-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);
        $('.jq-toast-wrap').remove();
        $.toast({
            heading: 'ADVERTENCIA',
            text: 'Existen Datos que debe ingresar para poder realizar la acción solicitada',
            position: 'bottom-right',
            showHideTransition: 'plain',
            icon: 'warning',
            stack: false
        });


        result = false;
    } else {
        submarca.addClass('is-valid');
    }


    if (clasificacion.val() == 0 || !clasificacion.val()) {
        clasificacion.addClass('is-invalid');
        clasificacion.focus();
        $('#bt-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);
        $('.jq-toast-wrap').remove();
        $.toast({
            heading: 'ADVERTENCIA',
            text: 'Existen Datos que debe ingresar para poder realizar la acción solicitada',
            position: 'bottom-right',
            showHideTransition: 'plain',
            icon: 'warning',
            stack: false
        });


        result = false;
    } else {
        clasificacion.addClass('is-valid');
    }



    if (sub_clasificacion.val() == 0 || !sub_clasificacion.val()) {
        sub_clasificacion.addClass('is-invalid');
        sub_clasificacion.focus();
        $('#bt-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);
        $('.jq-toast-wrap').remove();
        $.toast({
            heading: 'ADVERTENCIA',
            text: 'Existen Datos que debe ingresar para poder realizar la acción solicitada',
            position: 'bottom-right',
            showHideTransition: 'plain',
            icon: 'warning',
            stack: false
        });


        result = false;
    } else {
        sub_clasificacion.addClass('is-valid');
    }

    if (color.val() == 0 || !color.val()) {
        color.addClass('is-invalid');
        color.focus();
        $('#bt-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);
        $('.jq-toast-wrap').remove();
        $.toast({
            heading: 'ADVERTENCIA',
            text: 'Existen Datos que debe ingresar para poder realizar la acción solicitada',
            position: 'bottom-right',
            showHideTransition: 'plain',
            icon: 'warning',
            stack: false
        });


        result = false;
    } else {
        color.addClass('is-valid');
    }

   

    if (preciogt.val() == "") {
        preciogt.addClass('is-invalid');
        preciogt.focus();
        $('#bt-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);
        $('.jq-toast-wrap').remove();
        $.toast({
            heading: 'ADVERTENCIA',
            text: 'Existen Datos que debe ingresar para poder realizar la acción solicitada',
            position: 'bottom-right',
            showHideTransition: 'plain',
            icon: 'warning',
            stack: false
        });


        result = false;
    } else {
        preciogt.addClass('is-valid');
    }

    if (costo.val() == "") {
        costo.addClass('is-invalid');
        costo.focus();
        $('#bt-guardar').removeAttr('disabled', true);
        $('#bt-cancelar').removeAttr('disabled', true);
        $('.jq-toast-wrap').remove();
        $.toast({
            heading: 'ADVERTENCIA',
            text: 'Existen Datos que debe ingresar para poder realizar la acción solicitada',
            position: 'bottom-right',
            showHideTransition: 'plain',
            icon: 'warning',
            stack: false
        });


        result = false;
    } else {
        costo.addClass('is-valid');
    }

    
    return result;

}

//metodo para limpiar el formulario
function limpiar() {
    $('#company').val(0);
    $('#descripcion').val(null);
    $('#sucursal').html('<option value="0">Seleccione Una Opción</option>');
    $('#observacion').val(null);
    $('#region').html('<option value="0">Seleccione Una Opción</option>');
    $('#id').val(0);

    $('#requiereProduccion').prop("checked",false);  

    $('#codigo').val(null);
    $('#descripcion').val(null);
    $('#tipo').val(0);
    $('#codigo1').val(null);
    $('#codigo2').val(null);
    $('#marca').val(0);
    $('#submarca').val('<option value="0">Seleccione Una Opción</option>');
    $('#clasificacion').val(0);
    $('#sub-clasificacion').val('<option value="0">Seleccione Una Opción</option>');
    $('#color').val(0);
    $('#preciogt').val(0);
    $('#precioes').val(0);
    $('#precio3').val(0);
    $('#costo').val(null);


    var codigo = $('#codigo');
    var descripcion = $('#descripcion');
    var tipo = $('#tipo');
    var marca = $('#marca');
    var submarca = $('#submarca');
    var clasificacion = $('#clasificacion');
    var sub_clasificacion = $('#sub-clasificacion');
    var color = $('#color');
    var preciogt = $('#preciogt');
    var costo = $('#costo');


    codigo.removeClass('is-invalid');
    codigo.removeClass('is-valid');

    descripcion.removeClass('is-invalid');
    descripcion.removeClass('is-valid');


    tipo.removeClass('is-invalid');
    tipo.removeClass('is-valid');

    marca.removeClass('is-invalid');
    marca.removeClass('is-valid');

    submarca.removeClass('is-invalid');
    submarca.removeClass('is-valid');

    clasificacion.removeClass('is-invalid');
    clasificacion.removeClass('is-valid');

    sub_clasificacion.removeClass('is-invalid');
    sub_clasificacion.removeClass('is-valid');

    color.removeClass('is-invalid');
    color.removeClass('is-valid');

    preciogt.removeClass('is-invalid');
    preciogt.removeClass('is-valid');


    costo.removeClass('is-invalid');
    costo.removeClass('is-valid');

    $('#bt-guardar').removeAttr('disabled', true);
    $('#bt-cancelar').removeAttr('disabled', true);
    $('#bt-guardar').html('<i class="material-icons">add</i>Guardar');
    $('#bt-guardar').removeClass('btn-info');
    $('#bt-guardar').removeClass('btn-warning');
    $('#bt-guardar').addClass('btn-success');
}

// funcion para cargar datos en el formulario
function cargarenFormulario(id, descripcion, codigo, codigo1, codigo2, idcolor, idmarca, tipo, idclasificacion, preciogt, precioes, costo, idsubclasificacion, idsubmarca, precio3, producir) {
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
    $('#submarca').html('<option value="0">Seleccione Una Opción</option>');
    $('#clasificacion').val(idclasificacion);
    $('#sub-clasificacion').html('<option value="0">Seleccione Una Opción</option>');
    $('#color').val(idcolor);
    $('#preciogt').val(preciogt);
    $('#precioes').val(precioes);
    $('#precio3').val(precio3);
    $('#costo').val(costo);
    $('#costo').attr('disabled', true);
    $('#requiereProduccion').attr('disabled', true);
    $('#requiereProduccion').prop("checked", false);  

    if (producir == 1) {
        $('#requiereProduccion').prop("checked", true);  
    } else {
        $('#requiereProduccion').prop("checked", false);  
    }

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

    $('#bt-guardar').html('<i class="material-icons">cached</i>Actualizar');
    $('#bt-guardar').removeClass('btn-success');
    $('#bt-guardar').removeClass('btn-warning');
    $('#bt-guardar').addClass('btn-info');
}

//metodo utilizado para mostrar lista de datos 
function mostrarDatos() {
    limpiar();

    //consume el ws para obtener los datos
    $.ajax({
        url: 'wsadmin_articulos.asmx/ObtenerDatos',
        data: '',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
            $('#tbod-datos').html(null);
            $('#tab-datos').dataTable().fnDeleteRow();
            $('#tab-datos').dataTable().fnUpdate();
            $('#tab-datos').dataTable().fnDestroy();
        },
        success: function (msg) {
            var i = 1;
            var tds = "";
            $('#tbod-datos').html(null);
            $.each(msg.d, function () {
                tds = "<tr class='odd'><td>" + i + "</td><td>" + this.codigo + "</td><td>" + this.descripcion + "</td><td>" + this.color + "</td><td>" + this.precioGt + "</td><td> " +
                    "<span onclick='cargarenFormulario(" + this.id + ",\"" + this.descripcion + "\",\"" + this.codigo + "\",\"" + this.codigo1 + "\",\"" + this.codigo2 + "\"," + this.idcolor + "," + this.idmarca + "," + this.tipo + "," + this.idclasificacion + ", " + this.precioGt + ", " + this.precioEs + "," + this.costo + ", " + this.id_Subclasificacion + "," + this.idsubmarca + "," + this.precio3 + "," + this.producir +")' class='Mdnew btn btn-sm btn-outline-info' data-container='body' data-trigger='hover' data-toggle='popover' data-placement='bottom' data-content='Click para poder cargar los datos en el formulario, para poder actualizar.' data-original-title='' title ='' > " +
                    "<i class='material-icons'>edit</i> " +
                    "</span> " +
                    "<span onclick='eliminar(" + this.id + ")' class='btn btn-sm btn-outline-danger' data-container='body' data-trigger='hover' data-toggle='popover' data-placement='bottom' data-content='Click para poder Inhabilitar el dato seleccionado, Esto hara que dicho dato no aparesca en ninguna acción, menu o formulario del sistema.' data-original-title='' title=''> " +
                    "<i class='material-icons'> delete_sweep </i> " +
                    "</span></td></tr>'"
                i++;

                $("#tbod-datos").append(tds);
            });

            $('#tab-datos').dataTable();
            $('[data-toggle="popover"]').popover();
        }
    });

};

// funcion para cargar datos en el formulario
function eliminar(id) {
    limpiar();
    $('#id').val(id);
    //muestra la modal de confirmacion
    $('#MdDeshabilitar').modal('toggle');

    //cambia los botones
    $('#bt-guardar').html('<i class="material-icons">cached</i>Actualizar');
    $('#bt-guardar').removeClass('btn-success');
    $('#bt-guardar').removeClass('btn-warning');
    $('#bt-guardar').addClass('btn-info');
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